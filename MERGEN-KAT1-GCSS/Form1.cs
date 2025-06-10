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
        public float x = 0.0f, y = 0.0f, z = 0.0f;
        private Camera camera;
        private Map map;
        private Charts charts;
        private BatteryProgressBar batteryProgressBar;
        private ArduinoReader arduinoReader;
        private _3DSimulation simulation;
        private veri veriOku;
        private Button switchModelButton;


        public Form1()
        {
            InitializeComponent();
            Initialize3DComponents();

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

 

 
        }

        private void Initialize3DComponents()
        {
            // GLControl1 olaylarını bağla
            glControl1.Load += GlControl1_Load;
            glControl1.Paint += GlControl1_Paint;
            simulation = new _3DSimulation();

            // 3D Simülasyon nesnesini başlat
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            camopenbutton.Enabled = true;
            camclosebutton.Enabled = false;
            map.InitializeMap();
            charts.InitializeCharts();
            batteryProgressBar.Percentage = 30;
            timer1.Tick += timer1_Tick;
            LoadAvailablePorts();
            timer1.Start();
            veriOku = new veri(this, simulation);


        }


        public void SwitchModel()
        {
            simulation.useAlternativeModel = !simulation.useAlternativeModel;
            glControl1?.Invalidate(); // Ekranı güncelle
        }
        private void GlControl1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(Color.FromArgb(24, 30, 54)); // Arka plan rengi
            GL.Enable(EnableCap.DepthTest);
        }

        private void GlControl1_Paint(object sender, PaintEventArgs e)
        {
            // Önce buffer temizleniyor.
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Perspektif ve kamera ayarları.
            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(1.04f, (float)glControl1.Width / glControl1.Height, 1, 10000);
            Matrix4 lookAt = Matrix4.LookAt(25, 0, 0, 0, 0, 0, 0, 1, 0);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.LoadMatrix(ref perspective);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref lookAt);

            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            // Model rotasyonları uygulanıyor.
            GL.Rotate(x, -1.0, 0.0, 0.0);
            GL.Rotate(z, 0.0, -1.0, 0.0);
            GL.Rotate(y, 0.0, 0.0, 2.0);

            // Modelin çizimi: alternatif model seçimine göre.
            if (simulation.useAlternativeModel)
                simulation.DrawNewSatellite();
            else
                simulation.DrawPerforatedShell(2.3f, 12.0f, 16);

            glControl1.SwapBuffers();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            x = simulation.x;
            y = simulation.y;
            z = simulation.z;


            glControl1.Invalidate(); // Yeniden çizim
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

        private void ayrilmabutton_Click(object sender, EventArgs e)
        {
            SwitchModel();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            string selectedPort = comboBox1.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedPort))
            {
                veriOku.PortAc(selectedPort, 9600); // Seçilen COM port ve sabit baud rate
                MessageBox.Show("Bağlantı başarılı.");
            }
            else
            {
                MessageBox.Show("Lütfen bir COM port seçin.");
            }
        }
    }
}
