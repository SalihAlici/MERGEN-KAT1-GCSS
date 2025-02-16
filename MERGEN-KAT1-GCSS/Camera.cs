using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace MERGEN_KAT1_GCSS
{
    internal class Camera
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private PictureBox pictureBox;

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
            if (pictureBox.InvokeRequired)
            {
                pictureBox.Invoke(new MethodInvoker(delegate
                {
                    pictureBox.Image?.Dispose();
                    pictureBox.Image = (Bitmap)eventArgs.Frame.Clone();
                }));
            }
            else
            {
                pictureBox.Image?.Dispose();
                pictureBox.Image = (Bitmap)eventArgs.Frame.Clone();
            }
        }

        public void StartCamera()
        {
            if (videoSource != null && !videoSource.IsRunning)
            {
                videoSource.Start();
            }
        }

        public void StopCamera()
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                pictureBox.Image?.Dispose(); // Resmi serbest bırak
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
