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
        private Camera camera;
        private Map map;
        private Charts charts;
        private BatteryProgressBar batteryProgressBar;  // BatteryProgressBar'ı formda tanımlıyoruz

        public Form1()
        {
            InitializeComponent();
            camera = new Camera(pictureBox1); // Kamera sınıfını başlat
            map = new Map(gMapControl1); // gMapControl1 form üzerinde yer almalı
            Chart[] chartArray = new Chart[] { chart1, chart2, chart3, chart4, chart5, chart6, chart7, chart8 };
            charts = new Charts(chartArray); // Charts sınıfını başlat

            // BatteryProgressBar nesnesini oluşturuyoruz ve form üzerine ekliyoruz
            batteryProgressBar = new BatteryProgressBar();
            batteryProgressBar.Location = new Point(this.ClientSize.Width - batteryProgressBar.Width - 10, 10);  // Sağ üst köşeye yerleştiriyoruz
            batteryProgressBar.Size = new Size(200, 50);  // Boyut ayarı
            this.Controls.Add(batteryProgressBar);  // Pil göstergesini formun kontrol listesine ekliyoruz
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            camopenbutton.Enabled = true; camclosebutton.Enabled = false; // Form açıldığında sadece kamera open butonu basılabilir
            map.InitializeMap(); // Haritayı başlat
            charts.InitializeCharts(); // Grafiklere veri ekle
        }

        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // Kameradan gelen yeni görüntüyü PictureBox'a aktar
            Bitmap frame = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = frame;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            camera.Dispose(); // Form kapanırken kamerayı kapat
        }

        private void camopenbutton_Click(object sender, EventArgs e)
        {
            camera.StartCamera();
            camopenbutton.Enabled = false;
            camclosebutton.Enabled = true;
        }

        private void camclosebutton_Click(object sender, EventArgs e)
        {
            camera.StopCamera();
            camopenbutton.Enabled = true;
            camclosebutton.Enabled = false;
        }


        private void cikisbutton_Click_1(object sender, EventArgs e)
        {
            camera.StopCamera();
            DialogResult firstResponse = MessageBox.Show("Uygulamadan çıkmak istediğinize emin misiniz?", "Çıkış Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (firstResponse == DialogResult.Yes)
            {
                Application.Exit();
            }

        }
    }
}