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


namespace MERGEN_KAT1_GCSS
{
    public partial class Form1 : Form
    {
        private Camera camera;
        Map map ;

        private Charts charts;
        private BatteryProgressBar batteryProgressBar;
        private Data dataHandler;
       
        private _3DSimulation simulation;

        public Form1()
        {
            InitializeComponent();

            // PictureBox, harita ve chart ayarları
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            camera = new Camera(pictureBox1);
            map=new Map(gMapControl1);
            //map.InitializeMap();


            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            // Batarya göstergesi
            batteryProgressBar = new BatteryProgressBar();
            batteryProgressBar.Location = new Point(this.ClientSize.Width - batteryProgressBar.Width - 10, 10);
            batteryProgressBar.Size = new Size(200, 50);
            this.Controls.Add(batteryProgressBar);

            
          

            // GLControl üzerinden 3D simülasyonu başlatıyoruz.
            // (glControl1, Form1'in tasarımında eklenmiş olmalı)
            simulation = new _3DSimulation(glControl1);

 
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
            simulation.SwitchModel();

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

            dataHandler = new Data(serialPort1,map, this); // event bağlanıyor
            dataHandler.Connect();

            Console.WriteLine("Port açık: " + selectedPort);


        }

        
    }
}
