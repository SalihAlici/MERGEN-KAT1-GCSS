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
using System.Diagnostics;

namespace MERGEN_KAT1_GCSS
{
    public partial class Form1 : Form
    {
        private Camera camera;
        Map map ;
        private bool useAlternativeModel = false;

        
        private Charts charts;
        private BatteryProgressBar batteryProgressBar;
        private Data dataHandler;
        private DataGridViewHandler dataGridViewHandler;
        private _3DSimulation simulation;

        private Stopwatch stopwatch = Stopwatch.StartNew();
        private const int targetFPS = 30;

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

            Application.Idle += Application_Idle;


            LoadAvailablePorts();

        }

        private void Application_Idle(object sender, EventArgs e)
        {
            if (stopwatch.ElapsedMilliseconds >= 1000 / targetFPS)
            {
                glControl1.Invalidate(); // sürekli çizim
                stopwatch.Restart();
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            camopenbutton.Enabled = true;
            camclosebutton.Enabled = false;
            map.InitializeMap();
           
            batteryProgressBar.Percentage = 30;


           

            

        }

        private void LoadAvailablePorts()
        {
            string[] ports = SerialPort.GetPortNames();

            comboBox1.Items.Clear();
            comboBox2.Items.Clear();

            comboBox1.Items.AddRange(ports);
            comboBox2.Items.AddRange(ports);

            if (ports.Length > 0)
            {
                comboBox1.SelectedIndex = 0;
                comboBox2.SelectedIndex = 0;
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
       

       

        private void ayrilmabutton_Click(object sender, EventArgs e)
        {
            useAlternativeModel = !useAlternativeModel;
            glControl1.Invalidate();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                // Port açık ise kapat
                try
                {
                    dataHandler?.Disconnect();  // Eğer varsa bağlantıyı kes
                    serialPort1.Close();
                    MessageBox.Show("Port kapatıldı.");
                    Console.WriteLine("Port kapatıldı: " + serialPort1.PortName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Port kapatılırken hata oluştu: " + ex.Message);
                }
                return;
            }

            // Port kapalı ise açmaya çalış
            string selectedPort = comboBox1.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedPort) || selectedPort == "Port Yok")
            {
                MessageBox.Show("Port seçin.");
                return;
            }

            serialPort1.PortName = selectedPort;
            serialPort1.BaudRate = 9600;

            try
            {
                dataHandler = new Data(serialPort1, map, this, charts, dataGridViewHandler, simulation); // event bağlanıyor
                dataHandler.Connect();
               // serialPort1.Open();

                MessageBox.Show("Port açıldı: " + selectedPort);
                Console.WriteLine("Port açık: " + selectedPort);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Port açılırken hata oluştu: " + ex.Message);
            }


        }

       

       

        private void glControl1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(Color.FromArgb(24, 30, 54)); // Arka plan rengi
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
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
            

            // Model rotasyonları uygulanıyor.
            GL.Rotate(simulation.x, 0.0, 0.0, 1.0);  // yaw (z)
            GL.Rotate(simulation.z, 0.0, 1.0, 0.0);  // pitch (x)
            GL.Rotate(simulation.y, 1.0, 0.0, 0.0);  // roll (y)

            // Modelin çizimi: alternatif model seçimine göre.
            if (useAlternativeModel)
                simulation.DrawNewSatellite();
            else
                simulation.DrawPerforatedShell(2.3f, 12.0f, 16);

            glControl1.SwapBuffers();
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            comboBox1.Items.AddRange(ports);
        }

        private void comboBox2_DropDown(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            comboBox2.Items.AddRange(ports);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (!serialPort2.IsOpen)
                {
                    serialPort2.PortName = comboBox2.SelectedItem.ToString();
                    serialPort2.BaudRate = 9600; // İstersen burayı ayarla
                    serialPort2.Open();
                    MessageBox.Show($"{serialPort2.PortName} açıldı.");
                }
                else
                {
                    serialPort2.Close();
                    MessageBox.Show($"{serialPort2.PortName} kapandı.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (serialPort2.IsOpen)
            {
                string mesaj = textBox1.Text.Trim();
                if (!string.IsNullOrEmpty(mesaj))
                {
                    serialPort2.WriteLine(mesaj);
                }
            }
            else
            {
                MessageBox.Show("Gönderme portu açık değil!");
            }
        }
    }
}
