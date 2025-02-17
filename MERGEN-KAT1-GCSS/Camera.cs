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
            videoSource.NewFrame += new NewFrameEventHandler(Video_NewFrame);
        }

        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap frame = (Bitmap)eventArgs.Frame.Clone();

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

            // Eğer kayıt yapılıyorsa kareyi kaydet
            if (isRecording && videoWriter != null)
            {
                videoWriter.WriteVideoFrame(frame);
            }

            frame.Dispose();
        }

        public void StartCamera()
        {
            if (videoSource != null && !videoSource.IsRunning)
            {
                videoSource.Start();
                StartRecording();  // Kamera açılınca kayıt başlasın
            }
        }

        public void StopCamera()
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                StopRecording();  // Kamera kapanınca kayıt dursun
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                pictureBox.Image?.Dispose(); // Resmi serbest bırak
            }
        }

        private void StartRecording()
        {
            if (!isRecording)
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                outputFilePath = Path.Combine(desktopPath, "KameraKaydi.mp4");

                videoWriter = new VideoFileWriter();
                videoWriter.Open(outputFilePath, 640, 480, 30, VideoCodec.MPEG4);

                isRecording = true;
                MessageBox.Show("Kayıt Başladı!");
            }
        }

        private void StopRecording()
        {
            if (isRecording)
            {
                isRecording = false;
                videoWriter.Close();
                videoWriter.Dispose();
                videoWriter = null;

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
