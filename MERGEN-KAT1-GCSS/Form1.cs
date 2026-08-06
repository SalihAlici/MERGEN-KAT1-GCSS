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
using System.Threading.Tasks;

namespace MERGEN_KAT1_GCSS
{
    public partial class Form1 : Form
    {
        private Camera camera;
        private Map map;
        private Charts charts;
        private Data dataHandler;
        private DataGridViewHandler dataGridViewHandler;

        private Stopwatch stopwatch = Stopwatch.StartNew();
        private const int targetFPS = 30;

        public Form1()
        {
            InitializeComponent();

            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            camera = new Camera(pictureBox1);
            map = new Map(gMapControl2);
            charts = new Charts(chart1, chart2, chart3, chart4);

            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            string csvPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MERGEN.csv");
            dataGridViewHandler = new DataGridViewHandler(dataGridView1, csvPath);

            LoadAvailablePorts();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            camopenbutton.Enabled = true;
            camclosebutton.Enabled = false;

            // Kameraları tarayıp combobox'a ekler
            LoadAvailableCameras();
        }

        private void LoadAvailablePorts()
        {
            string[] ports = SerialPort.GetPortNames();
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(ports);

            if (ports.Length > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        private void LoadAvailableCameras()
        {
            comboBoxKamera.Items.Clear();
            var kameralar = camera.GetCameraNames();

            foreach (string k in kameralar)
            {
                comboBoxKamera.Items.Add(k);
            }

            if (comboBoxKamera.Items.Count > 0)
            {
                comboBoxKamera.SelectedIndex = 0;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            camera.Dispose();
        }

        private async void camopenbutton_Click(object sender, EventArgs e)
        {
            if (comboBoxKamera.SelectedIndex < 0)
            {
                MessageBox.Show("Lütfen başlatılacak kamerayı seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            camopenbutton.Enabled = false;
            int seciliKameraIndeksi = comboBoxKamera.SelectedIndex;

            await Task.Run(() => camera.StartCamera(seciliKameraIndeksi, true));

            camclosebutton.Enabled = true;
        }

        private async void camclosebutton_Click(object sender, EventArgs e)
        {
            camclosebutton.Enabled = false;

            await Task.Run(() => camera.StopCamera());

            camopenbutton.Enabled = true;
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

        private void button5_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    dataHandler?.Disconnect();
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
                dataHandler = new Data(serialPort1, map, this, charts, dataGridViewHandler);
                dataHandler.Connect();
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
            GL.ClearColor(Color.FromArgb(24, 30, 54));
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            comboBox1.Items.AddRange(ports);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // İçi boş bırakılmış fonksiyon
        }

        private async void button7_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen && dataHandler != null)
            {
                string mesaj = textBox1.Text.Trim();
                if (!string.IsNullOrEmpty(mesaj))
                {
                    string mesajWithC = "C" + mesaj;
                    await Task.Run(() => dataHandler.SendCommand(mesajWithC));
                }
                else
                {
                    MessageBox.Show("Gönderilecek mesaj boş!");
                }
            }
            else
            {
                MessageBox.Show("Port açık değil veya Data nesnesi yok!");
            }
        }

        private async void SendMessageAsync(string message)
        {
            if (serialPort1.IsOpen && dataHandler != null)
            {
                try
                {
                    await Task.Run(() => dataHandler.SendCommand(message));
                    Console.WriteLine($"Mesaj gönderildi: {message}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Mesaj gönderilirken hata: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Port açık değil veya Data nesnesi yok!");
            }
        }

        private void unlockbutton_Click(object sender, EventArgs e)
        {
            SendMessageAsync("A");
        }

        private void lockbutton_Click(object sender, EventArgs e)
        {
            SendMessageAsync("B");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendMessageAsync("E");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SendMessageAsync("R");
        }
    }
}