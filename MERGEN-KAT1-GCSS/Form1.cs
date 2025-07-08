using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO.Ports;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MERGEN_KAT1_GCSS
{
    public partial class Form1 : Form
    {
        private Camera camera;
        private Map map;
        private Charts charts;
        private BatteryProgressBar batteryProgressBar;
        private ArduinoReader arduinoReader;
        private _3DSimulation simulation;
        private Compass compass;
      

        public Form1()
        {
            InitializeComponent();

            // PictureBox, harita ve chart ayarları
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            camera = new Camera(pictureBox1);
            map = new Map(gMapControl1);
            Chart[] chartArray = new Chart[] { chart1, chart2, chart3, chart4, chart5, chart6, chart7, chart8 };
            charts = new Charts(chartArray);
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            // Batarya göstergesi
            batteryProgressBar = new BatteryProgressBar();
            batteryProgressBar.Location = new Point(this.ClientSize.Width - batteryProgressBar.Width - 10, 10);
            batteryProgressBar.Size = new Size(200, 50);
            this.Controls.Add(batteryProgressBar);

            // Arduino okuyucu
            arduinoReader = new ArduinoReader();

            // GLControl üzerinden 3D simülasyonu başlatıyoruz.
            // (glControl1, Form1'in tasarımında eklenmiş olmalı)
            simulation = new _3DSimulation(glControl1);

            //compass 
            compass = new Compass();
            panel7.Paint += panel7_Paint;


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            camopenbutton.Enabled = true;
            camclosebutton.Enabled = false;
            map.InitializeMap();
            charts.InitializeCharts();
            batteryProgressBar.Percentage = 30;
            LoadAvailablePorts();

           

            // Manuel güncelleme için timer'ı başlatıyoruz (1 ms interval)
            simulation.StartSimulationTimer(1);
            // Her saniye rastgele rotasyon güncellemesi için randomTimer'ı başlatıyoruz
            simulation.StartRandomRotation();
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
            camera.Dispose();
        }

        private void camopenbutton_Click(object sender, EventArgs e)
        {

        }

        private void camclosebutton_Click(object sender, EventArgs e)
        {

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

        // Arduino bağlantısı için
        private void buttonArduino_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir seri port seçin.");
                return;
            }

            string selectedPort = comboBox1.SelectedItem.ToString();
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
        private void panel7_Paint(object sender, PaintEventArgs e)
        {
            compass.Draw(e.Graphics, panel7.ClientRectangle);
        }

        
    }
}
