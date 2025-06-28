using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO.Ports;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GMap.NET.WindowsForms;
using GMap.NET;
using GMap.NET.MapProviders;
using System.IO;


namespace MERGEN_KAT1_GCSS
{
    public partial class Form1 : Form
    {
        private Camera camera;
        Map map ;
        private bool useAlternativeModel = false;

        public float x = 0.0f, y = 0.0f, z = 0.0f;
        private Charts charts;
        private BatteryProgressBar batteryProgressBar;
        private Data dataHandler;
        private DataGridViewHandler dataGridViewHandler;
        private _3DSimulation simulation;

        public Form1()
        {
            InitializeComponent();

            // PictureBox, harita ve chart ayarları
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            camera = new Camera(pictureBox1);
            map=new Map(gMapControl1);
            //map.InitializeMap();
            charts = new Charts(chart1, chart2, chart3, chart4, chart5, chart6, chart7, chart8);

            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            // Batarya göstergesi
            batteryProgressBar = new BatteryProgressBar();
            batteryProgressBar.Location = new Point(this.ClientSize.Width - batteryProgressBar.Width - 10, 10);
            batteryProgressBar.Size = new Size(200, 50);
            this.Controls.Add(batteryProgressBar);

           
            string csvPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TELEMETRİ.csv");
            dataGridViewHandler = new DataGridViewHandler(dataGridView1, csvPath);
            



            // GLControl üzerinden 3D simülasyonu başlatıyoruz.
            // (glControl1, Form1'in tasarımında eklenmiş olmalı)
            simulation = new _3DSimulation(glControl1);


            timer1.Interval = 1000;
            timer2.Interval = 1000;// 1 saniye
            timer1.Tick += timer1_Tick;
            timer1.Start();
            timer2.Start();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            camopenbutton.Enabled = true;
            camclosebutton.Enabled = false;
            map.InitializeMap();
           
            batteryProgressBar.Percentage = 30;


            string[] ports = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(ports);

            if (ports.Length > 0)
                comboBox1.SelectedIndex = 0;
            else
                comboBox1.Items.Add("Port Yok");

            

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
       

       

        private void ayrilmabutton_Click(object sender, EventArgs e)
        {
            useAlternativeModel = !useAlternativeModel;
            glControl1.Invalidate();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                MessageBox.Show("Zaten bağlı.");
                return;
            }

            string selectedPort = comboBox1.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedPort) || selectedPort == "Port Yok")
            {
                MessageBox.Show("Port seçin.");
                return;
            }

            serialPort1.PortName = selectedPort;
            serialPort1.BaudRate = 9600;

            dataHandler = new Data(serialPort1,map, this,charts,dataGridViewHandler,simulation); // event bağlanıyor
            dataHandler.Connect();

            Console.WriteLine("Port açık: " + selectedPort);


        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            dataGridViewHandler.SaveDataGridViewToCSV();
            Console.WriteLine("Timer tick, CSV kaydediliyor.");

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            x = simulation.x;
            y = simulation.y;
            z = simulation.z;

            simulation.UpdateRotation(x, y, z);

            glControl1.Invalidate(); // Yeniden çizim
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(Color.FromArgb(24, 30, 54)); // Arka plan rengi
            GL.Enable(EnableCap.DepthTest);
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
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
            GL.Rotate(x, 1.0, 0.0, 0.0);
            GL.Rotate(z, 0.0, 1.0, 0.0);
            GL.Rotate(y, 0.0, 0.0, 1.0);

            // Modelin çizimi: alternatif model seçimine göre.
            if (useAlternativeModel)
                simulation.DrawNewSatellite();
            else
                simulation.DrawPerforatedShell(2.3f, 12.0f, 16);

            glControl1.SwapBuffers();
        }
    }
}
