using System;
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

        public Camera(PictureBox pictureBox)
        {
            this.pictureBox = pictureBox;
            InitializeCamera();
        }

        /// <summary>
        /// Otomatik kamera seçimi:
        /// - Eğer sadece 1 cihaz varsa => dahili (0)
        /// - Eğer 2 veya daha fazla cihaz varsa => harici (1)
        /// </summary>
        private void InitializeCamera()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count == 0)
            {
                MessageBox.Show("Kamera bulunamadı!");
                return;
            }

            int deviceIndex = 0; // Varsayılan laptop kamerası
            if (videoDevices.Count > 1)
                deviceIndex = 1; // Harici kamera varsa onu seç

            videoSource = new VideoCaptureDevice(videoDevices[deviceIndex].MonikerString);

            // 1280x720 çözünürlük tercihi
            VideoCapabilities desiredCap = null;
            foreach (var cap in videoSource.VideoCapabilities)
            {
                if (cap.FrameSize.Width == 1280 && cap.FrameSize.Height == 720)
                {
                    desiredCap = cap;
                    break;
                }
            }
            if (desiredCap == null)
            {
                desiredCap = videoSource.VideoCapabilities[0];
                foreach (var cap in videoSource.VideoCapabilities)
                {
                    if (cap.FrameSize.Width * cap.FrameSize.Height >
                        desiredCap.FrameSize.Width * desiredCap.FrameSize.Height)
                        desiredCap = cap;
                }
            }

            videoSource.VideoResolution = desiredCap;
            videoSource.NewFrame += Video_NewFrame;
        }

        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap originalFrame = (Bitmap)eventArgs.Frame.Clone();

            // UI güncellemesi (60 FPS sınırı)
            if ((DateTime.Now - lastUIUpdate).TotalMilliseconds >= 16) // ~16ms ≈ 60FPS
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

            // Kayıt için ayrı kopya
            if (isRecording)
            {
                Bitmap recordingFrame = (Bitmap)originalFrame.Clone();

                // Kuyruk boyunu sınırla (60 kare = 1 saniye)
                if (frameQueue.Count > 60)
                {
                    if (frameQueue.TryDequeue(out Bitmap oldFrame))
                        oldFrame.Dispose();
                }

                frameQueue.Enqueue(recordingFrame);
                frameAvailable.Set();
            }

            originalFrame.Dispose();
        }

        public void StartCamera(bool autoStartRecording = true)
        {
            if (videoSource == null)
            {
                MessageBox.Show("Kamera uygun değil.");
                return;
            }

            if (!videoSource.IsRunning)
            {
                videoSource.Start();
                if (autoStartRecording)
                {
                    StartRecording();
                }
            }
            else
            {
                MessageBox.Show("Kamera zaten çalışıyor.");
            }
        }

        public void StopCamera()
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                StopRecording();
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                pictureBox.Image?.Dispose();
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

                    // 60 FPS kayıt, makul bitrate ile
                    videoWriter.Open(outputFilePath, 1280, 720, 60, VideoCodec.MPEG4, 5000000);

                    isRecording = true;

                    recordingThreadRunning = true;
                    recordingThread = new Thread(RecordingWorker)
                    {
                        IsBackground = true
                    };
                    recordingThread.Start();

                    MessageBox.Show("Kayıt başladı.");
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
            if (videoSource != null)
            {
                videoSource.NewFrame -= Video_NewFrame;
                videoSource = null;
            }
            pictureBox.Image?.Dispose();
        }
    }
}
