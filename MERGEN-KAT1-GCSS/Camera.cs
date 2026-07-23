using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using Accord.Video.FFMPEG;

namespace MERGEN_KAT1_GCSS
{
    internal class Camera : IDisposable
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private PictureBox pictureBox;
        private VideoFileWriter videoWriter;

        private bool isRecording = false;
        private string outputFilePath;

        private readonly object lockObj = new object();

        private ConcurrentQueue<Bitmap> frameQueue = new ConcurrentQueue<Bitmap>();
        private Thread recordingThread;
        private bool recordingThreadRunning = false;
        private AutoResetEvent frameAvailable = new AutoResetEvent(false);

        private DateTime lastUIUpdate = DateTime.MinValue;

        // Kameranın dinamik FPS değerini tutacağımız değişken
        private int currentFps = 30;

        public Camera(PictureBox pictureBox)
        {
            this.pictureBox = pictureBox;
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        }

        public List<string> GetCameraNames()
        {
            List<string> cameras = new List<string>();
            if (videoDevices != null)
            {
                foreach (FilterInfo device in videoDevices)
                {
                    cameras.Add(device.Name);
                }
            }
            return cameras;
        }

        private void InitializeCamera(int deviceIndex)
        {
            if (videoDevices.Count == 0 || deviceIndex < 0 || deviceIndex >= videoDevices.Count)
            {
                throw new Exception("Geçersiz kamera seçimi veya kamera bulunamadı!");
            }

            videoSource = new VideoCaptureDevice(videoDevices[deviceIndex].MonikerString);

            VideoCapabilities desiredCap = null;
            foreach (var cap in videoSource.VideoCapabilities)
            {
                if (cap.FrameSize.Width == 1280 && cap.FrameSize.Height == 720)
                {
                    desiredCap = cap;
                    break;
                }
            }

            if (desiredCap == null && videoSource.VideoCapabilities.Length > 0)
            {
                desiredCap = videoSource.VideoCapabilities[0];
                foreach (var cap in videoSource.VideoCapabilities)
                {
                    if (cap.FrameSize.Width * cap.FrameSize.Height >
                        desiredCap.FrameSize.Width * desiredCap.FrameSize.Height)
                        desiredCap = cap;
                }
            }

            if (desiredCap != null)
            {
                videoSource.VideoResolution = desiredCap;

                // Kameranın desteklediği gerçek FPS değerini alıyoruz.
                // 0 veya geçersiz bir değerse varsayılan olarak 30 kullanıyoruz.
                currentFps = desiredCap.AverageFrameRate > 0 ? desiredCap.AverageFrameRate : 30;
            }

            videoSource.NewFrame += Video_NewFrame;
        }

        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap originalFrame = (Bitmap)eventArgs.Frame.Clone();

            // UI Güncellemesi (~60FPS limiti ile arayüzü yormamak için)
            if ((DateTime.Now - lastUIUpdate).TotalMilliseconds >= 16)
            {
                lastUIUpdate = DateTime.Now;
                Bitmap preview = (Bitmap)originalFrame.Clone();

                if (pictureBox.InvokeRequired)
                {
                    pictureBox.BeginInvoke(new MethodInvoker(() =>
                    {
                        pictureBox.Image?.Dispose();
                        pictureBox.Image = preview;
                    }));
                }
                else
                {
                    pictureBox.Image?.Dispose();
                    pictureBox.Image = preview;
                }
            }

            // Kayıt için arka plan işlemleri
            if (isRecording)
            {
                Bitmap recordingFrame = (Bitmap)originalFrame.Clone();

                // Kuyruk boyunu kameranın gerçek FPS hızına göre dinamik sınırlıyoruz (1 saniyelik tampon)
                if (frameQueue.Count > currentFps)
                {
                    if (frameQueue.TryDequeue(out Bitmap oldFrame))
                        oldFrame.Dispose();
                }

                frameQueue.Enqueue(recordingFrame);
                frameAvailable.Set();
            }

            originalFrame.Dispose();
        }

        public void StartCamera(int selectedDeviceIndex, bool autoStartRecording = true)
        {
            StopCamera();

            try
            {
                InitializeCamera(selectedDeviceIndex);

                if (videoSource != null && !videoSource.IsRunning)
                {
                    videoSource.Start();
                    if (autoStartRecording)
                    {
                        StartRecording();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kamera başlatılırken hata oluştu: " + ex.Message);
            }
        }

        public void StopCamera()
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                StopRecording();
                videoSource.SignalToStop();
                videoSource.WaitForStop();

                videoSource.NewFrame -= Video_NewFrame;
                videoSource = null;

                pictureBox.Image?.Dispose();
                pictureBox.Image = null;
            }
        }

        public void StartRecording()
        {
            lock (lockObj)
            {
                if (!isRecording)
                {
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string datetimeString = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    outputFilePath = System.IO.Path.Combine(desktopPath, $"KameraKaydi_{datetimeString}.mp4");

                    videoWriter = new VideoFileWriter();

                    // Dinamik FPS (currentFps) değerini video kaydediciye veriyoruz
                    videoWriter.Open(outputFilePath, 1280, 720, currentFps, VideoCodec.MPEG4, 25000000);

                    isRecording = true;
                    recordingThreadRunning = true;
                    recordingThread = new Thread(RecordingWorker)
                    {
                        IsBackground = true
                    };
                    recordingThread.Start();
                }
            }
        }

        public void StopRecording()
        {
            lock (lockObj)
            {
                if (isRecording)
                {
                    isRecording = false;
                    recordingThreadRunning = false;
                    frameAvailable.Set();

                    recordingThread.Join();

                    while (frameQueue.TryDequeue(out Bitmap bmp))
                    {
                        bmp.Dispose();
                    }

                    if (videoWriter != null)
                    {
                        videoWriter.Close();
                        videoWriter.Dispose();
                        videoWriter = null;
                    }

                    MessageBox.Show("Kayıt tamamlandı.\nDosya: " + outputFilePath);
                }
            }
        }

        private void RecordingWorker()
        {
            while (recordingThreadRunning)
            {
                frameAvailable.WaitOne();

                while (frameQueue.TryDequeue(out Bitmap frame))
                {
                    try
                    {
                        if (videoWriter != null && isRecording)
                        {
                            videoWriter.WriteVideoFrame(frame);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Yazma hatası: " + ex.Message);
                    }
                    finally
                    {
                        frame.Dispose();
                    }
                }
            }
        }

        public void Dispose()
        {
            StopCamera();
        }
    }
}