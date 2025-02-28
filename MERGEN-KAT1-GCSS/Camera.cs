using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using Accord.Video.FFMPEG;

namespace MERGEN_KAT1_GCSS
{
    internal class Camera
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private PictureBox pictureBox;
        private VideoFileWriter videoWriter;
        private bool isRecording = false;
        private string outputFilePath;

        public Camera(PictureBox pictureBox)
        {
            this.pictureBox = pictureBox;
            InitializeCamera();
        }

        private void InitializeCamera()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count == 0)
            {
                MessageBox.Show("Kamera bulunamadı!");
                return;
            }

            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);

            // Çözünürlük iyileştirmesi: 1280x720 veya daha yüksek destekleyen bir çözünürlük seç
            if (videoSource.VideoCapabilities != null && videoSource.VideoCapabilities.Length > 0)
            {
                VideoCapabilities desiredCap = null;
                foreach (var cap in videoSource.VideoCapabilities)
                {
                    if (cap.FrameSize.Width >= 1280 && cap.FrameSize.Height >= 720)
                    {
                        desiredCap = cap;
                        break;
                    }
                }
                if (desiredCap == null)
                {
                    // 1280x720 veya üzeri yoksa, en yüksek çözünürlüğü seç
                    VideoCapabilities maxCap = videoSource.VideoCapabilities[0];
                    foreach (var cap in videoSource.VideoCapabilities)
                    {
                        if (cap.FrameSize.Width * cap.FrameSize.Height > maxCap.FrameSize.Width * maxCap.FrameSize.Height)
                            maxCap = cap;
                    }
                    desiredCap = maxCap;
                }
                videoSource.VideoResolution = desiredCap;
            }

            videoSource.NewFrame += new NewFrameEventHandler(Video_NewFrame);
        }

        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap frame = (Bitmap)eventArgs.Frame.Clone();

            // Yatay aynalama: Sağa hareket ettiğinizde ekranın doğru yönde hareket etmesi için
            frame.RotateFlip(RotateFlipType.RotateNoneFlipX);

            if (pictureBox.InvokeRequired)
            {
                pictureBox.Invoke(new MethodInvoker(delegate
                {
                    pictureBox.Image?.Dispose();
                    pictureBox.Image = (Bitmap)frame.Clone();
                }));
            }
            else
            {
                pictureBox.Image?.Dispose();
                pictureBox.Image = (Bitmap)frame.Clone();
            }

            // Kayıt yapılıyorsa, videoya kare ekle
            if (isRecording && videoWriter != null)
            {
                videoWriter.WriteVideoFrame(frame);
            }
            frame.Dispose();
        }

        public void StartCamera()
        {
            if (videoSource == null)
            {
                MessageBox.Show("Kamera bulunamadı veya uygun değil.");
                return;
            }

            if (!videoSource.IsRunning)
            {
                videoSource.Start();
                StartRecording();  // Kamera açıldığında kayıt başlasın
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
            else
            {
                MessageBox.Show("Kamera zaten kapalı.");
            }
        }

        private void StartRecording()
        {
            if (!isRecording)
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                // Dosya adına geçerli tarih-saat bilgisini ekle
                string datetimeString = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                outputFilePath = Path.Combine(desktopPath, $"KameraKaydi_{datetimeString}.mp4");

                videoWriter = new VideoFileWriter();
                int width = 1280, height = 720;
                if (videoSource.VideoResolution != null)
                {
                    width = videoSource.VideoResolution.FrameSize.Width;
                    height = videoSource.VideoResolution.FrameSize.Height;
                }
                // Bitrate ekleyerek video kalitesini artırıyoruz (örneğin: 4000000 bps)
                videoWriter.Open(outputFilePath, width, height, 30, VideoCodec.MPEG4, 4000000);
                isRecording = true;
                MessageBox.Show("Kayıt Başladı!");
            }
        }

        private void StopRecording()
        {
            if (isRecording)
            {
                isRecording = false;
                if (videoWriter != null)
                {
                    videoWriter.Close();
                    videoWriter.Dispose();
                    videoWriter = null;
                }
                MessageBox.Show("Kayıt tamamlandı!\nKaydedilen Dosya: " + outputFilePath);
            }
        }

        public void Dispose()
        {
            StopCamera();
            if (videoSource != null)
            {
                videoSource.NewFrame -= new NewFrameEventHandler(Video_NewFrame);
                videoSource = null;
            }
        }
    }
}
