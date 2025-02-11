using GMap.NET;
using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Windows.Forms.DataVisualization.Charting;

namespace MERGEN_KAT1_GCSS
{
    public partial class Form1 : Form
    {

         
        public float lat = 38.707675f, lng = 35.519550f;
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count == 0)
            {
                MessageBox.Show("Kamera bulunamadı!");
                return;
            }

            // İlk kamerayı seç
            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(Video_NewFrame);

            gMapControl1.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            GMaps.Instance.Mode = AccessMode.ServerAndCache;


            gMapControl1.Position = new PointLatLng(lat, lng);

            gMapControl1.MinZoom = 5;
            gMapControl1.MaxZoom = 25;
            gMapControl1.Zoom = 16;
            // Chart1'i formun tasarımında eklediğinizi varsayıyoruz
            // Bu şekilde doğrudan veri ekleyebilirsiniz.

            // Seri ekleme ve veri noktası girme
            chart1.Series["Series1"].Points.AddXY(1, 5);  
            chart1.Series["Series1"].Points.AddXY(2, 7);  
            chart1.Series["Series1"].Points.AddXY(3, 12); 
            chart1.Series["Series1"].Points.AddXY(4, 94);  
            chart1.Series["Series1"].Points.AddXY(5, 15); 
            chart1.Series["Series1"].Points.AddXY(6, 8);  

            chart1.Series["Series2"].Points.AddXY(1, 1);  
            chart1.Series["Series2"].Points.AddXY(2, 10); 
            chart1.Series["Series2"].Points.AddXY(3, 96);  
            chart1.Series["Series2"].Points.AddXY(4, 82); 
            chart1.Series["Series2"].Points.AddXY(5, 9);  
            chart1.Series["Series2"].Points.AddXY(6, 55); 
            chart1.Series["Series2"].Points.AddXY(7, 85);  







            chart2.Series["Series1"].Points.AddXY(2, 10); 
            chart2.Series["Series1"].Points.AddXY(3, 7);  
            chart2.Series["Series1"].Points.AddXY(4, 12); 
            chart2.Series["Series1"].Points.AddXY(5, 9);  
            chart2.Series["Series1"].Points.AddXY(6, 15); 
            chart2.Series["Series1"].Points.AddXY(7, 8);  
            chart2.Series["Series1"].Points.AddXY(1, 5);  





            chart3.Series["Series1"].Points.AddXY(1, 5); 
            chart3.Series["Series1"].Points.AddXY(2, 10);
            chart3.Series["Series1"].Points.AddXY(3, 7); 
            chart3.Series["Series1"].Points.AddXY(4, 12);
            chart3.Series["Series1"].Points.AddXY(5, 9); 
            chart3.Series["Series1"].Points.AddXY(6, 15);
            chart3.Series["Series1"].Points.AddXY(7, 8); 





            chart4.Series["Series1"].Points.AddXY(1, 5);  
            chart4.Series["Series1"].Points.AddXY(2, 10); 
            chart4.Series["Series1"].Points.AddXY(3, 7);  
            chart4.Series["Series1"].Points.AddXY(4, 12); 
            chart4.Series["Series1"].Points.AddXY(5, 9);  
            chart4.Series["Series1"].Points.AddXY(6, 15); 
            chart4.Series["Series1"].Points.AddXY(7, 8);  




            chart5.Series["Series1"].Points.AddXY(1, 5);  
            chart5.Series["Series1"].Points.AddXY(2, 10); 
            chart5.Series["Series1"].Points.AddXY(3, 7);  
            chart5.Series["Series1"].Points.AddXY(4, 12); 
            chart5.Series["Series1"].Points.AddXY(5, 9);  
            chart5.Series["Series1"].Points.AddXY(6, 15); 
            chart5.Series["Series1"].Points.AddXY(7, 8);  



            chart6.Series["Series1"].Points.AddXY(1, 5);  
            chart6.Series["Series1"].Points.AddXY(2, 10); 
            chart6.Series["Series1"].Points.AddXY(3, 7);  
            chart6.Series["Series1"].Points.AddXY(4, 12); 
            chart6.Series["Series1"].Points.AddXY(5, 9);  
            chart6.Series["Series1"].Points.AddXY(6, 15); 
            chart6.Series["Series1"].Points.AddXY(7, 8);  



            chart7.Series["Series1"].Points.AddXY(1, 5);  
            chart7.Series["Series1"].Points.AddXY(2, 10); 
            chart7.Series["Series1"].Points.AddXY(3, 7);  
            chart7.Series["Series1"].Points.AddXY(4, 12); 
            chart7.Series["Series1"].Points.AddXY(5, 9);  
            chart7.Series["Series1"].Points.AddXY(6, 15); 
            chart7.Series["Series1"].Points.AddXY(7, 8);  


            chart8.Series["Series1"].Points.AddXY(1, 5);  
            chart8.Series["Series1"].Points.AddXY(2, 10); 
            chart8.Series["Series1"].Points.AddXY(3, 7);  
            chart8.Series["Series1"].Points.AddXY(4, 12); 
            chart8.Series["Series1"].Points.AddXY(5, 9);  
            chart8.Series["Series1"].Points.AddXY(6, 15); 
            chart8.Series["Series1"].Points.AddXY(7, 8);

            chart8.Series["Series2"].Points.AddXY(1, 51);
            chart8.Series["Series2"].Points.AddXY(2, 80);
            chart8.Series["Series2"].Points.AddXY(3, 7);
            chart8.Series["Series2"].Points.AddXY(4, 2);
            chart8.Series["Series2"].Points.AddXY(5, 9);
            chart8.Series["Series2"].Points.AddXY(6, 75);
            chart8.Series["Series2"].Points.AddXY(7, 6);
        }
        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // Kameradan gelen yeni görüntüyü PictureBox'a aktar
            Bitmap frame = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = frame;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Uygulama kapanırken kamerayı durdur
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }
        }

        

        private void sagpanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (videoSource != null && !videoSource.IsRunning)
            {
                videoSource.Start();
            }
        }

        private void glControl1_Load(object sender, EventArgs e)
        {

        }

       
    }
}
