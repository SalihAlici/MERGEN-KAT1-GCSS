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

        public Camera(PictureBox pictureBox, int deviceIndex = 1)
        {
            this.pictureBox = pictureBox;
            InitializeCamera(deviceIndex);
        }

        private void InitializeCamera(int deviceIndex)
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count == 0)
            {
                MessageBox.Show("Kamera bulunamadı!");
                return;
            }

            if (deviceIndex < 0 || deviceIndex >= videoDevices.Count)
                deviceIndex = 0;

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
                    if (cap.FrameSize.Width * cap.FrameSize.Height > desiredCap.FrameSize.Width * desiredCap.FrameSize.Height)
                        desiredCap = cap;
                }
            }

            videoSource.VideoResolution = desiredCap;
            videoSource.NewFrame += Video_NewFrame;
        }

        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap originalFrame = (Bitmap)eventArgs.Frame.Clone();

            // UI güncellemesi: sadece 30 FPS ile sınırla (~33ms)
            if ((DateTime.Now - lastUIUpdate).TotalMilliseconds >= 33)
            {
                lastUIUpdate = DateTime.Now;

                if (pictureBox.InvokeRequired)
                {
                    pictureBox.Invoke(new MethodInvoker(() =>
                    {
                        pictureBox.Image?.Dispose();
                        pictureBox.Image = (Bitmap)originalFrame.Clone();
                    }));
                }
                else
                {
                    pictureBox.Image?.Dispose();
                    pictureBox.Image = (Bitmap)originalFrame.Clone();
                }
            }

            // Kayıt aktifse kuyruğa ekle
            if (isRecording)
            {
                while (frameQueue.Count > 60 && frameQueue.TryDequeue(out Bitmap oldFrame))
                {
                    oldFrame.Dispose();
                }

                frameQueue.Enqueue(originalFrame);
                frameAvailable.Set();
            }
            else
            {
                originalFrame.Dispose();
            }
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
                    frameAvailable.Set(); // Thread'i uyandır

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
                    lock (lockObj)
                    {
                        if (videoWriter != null && isRecording)
                        {
                            videoWriter.WriteVideoFrame(frame);
                        }
                    }
                    frame.Dispose();
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
