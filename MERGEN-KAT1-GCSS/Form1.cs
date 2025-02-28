using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO.Ports;

namespace MERGEN_KAT1_GCSS
{
    public partial class Form1 : Form
    {
        private Camera camera;
        private Map map;
        private Charts charts;
        private BatteryProgressBar batteryProgressBar;
        private _3DSim simulation;
        private ArduinoReader arduinoReader;

        public Form1()
        {
            InitializeComponent();
            // PictureBox'ın görüntü ayarını düzenle (Zoom, görüntüyü orantılı olarak merkezler)
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            camera = new Camera(pictureBox1); // Kamera sınıfını başlat
            map = new Map(gMapControl1); // gMapControl1 form üzerinde yer almalı
            Chart[] chartArray = new Chart[] { chart1, chart2, chart3, chart4, chart5, chart6, chart7, chart8 };
            charts = new Charts(chartArray); // Grafikler için Charts sınıfını başlat

            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            camera = new Camera(pictureBox1);


            // BatteryProgressBar nesnesini oluştur ve form üzerine ekle
            batteryProgressBar = new BatteryProgressBar();
            batteryProgressBar.Location = new Point(this.ClientSize.Width - batteryProgressBar.Width - 10, 10);
            batteryProgressBar.Size = new Size(200, 50);
            this.Controls.Add(batteryProgressBar);

            simulation = new _3DSim(glControl1); // glControl1 formda yer almalı
            arduinoReader = new ArduinoReader(); // ArduinoReader nesnesini başlat
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            camopenbutton.Enabled = true;
            camclosebutton.Enabled = false; // Form açıldığında sadece "Kamera Aç" butonu aktif
            map.InitializeMap();
            charts.InitializeCharts();
            batteryProgressBar.Percentage = 30;
            LoadAvailablePorts();
        }

        private void LoadAvailablePorts()
        {
            comboBox1.Items.Clear();
            string[] ports = arduinoReader.GetAvailablePorts();
            if (ports.Length == 0)
            {
                MessageBox.Show("Bağlı hiçbir seri port bulunamadı.");
            }
            else
            {
                comboBox1.Items.AddRange(ports);
            }
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
            DialogResult firstResponse = MessageBox.Show("Uygulamadan çıkmak istediğinize emin misiniz?",
                                                           "Çıkış Onayı",
                                                           MessageBoxButtons.YesNo,
                                                           MessageBoxIcon.Question);
            if (firstResponse == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir seri port seçin.");
                return;
            }

            string selectedPort = comboBox1.SelectedItem.ToString();

            // Arduino ile bağlantıyı başlat
            bool success = arduinoReader.Start(selectedPort);
            if (success)
            {
                arduinoReader.DataReceived += ArduinoReader_DataReceived;
                MessageBox.Show($"Arduino'ya bağlanıldı: {selectedPort}");
            }
            else
            {
                MessageBox.Show("Bağlantı kurulurken bir hata oluştu.");
            }
        }

        private void ArduinoReader_DataReceived(object sender, ArduinoDataEventArgs e)
        {
            simulation.UpdateRotation(e.Yaw, e.Pitch, e.Roll);
        }
    }
}
