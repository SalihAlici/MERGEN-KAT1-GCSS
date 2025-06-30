using OpenTK;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows.Forms;

namespace MERGEN_KAT1_GCSS
{
    public class Data
    {
        private SerialPort _port;
        private Map _map;
        private Form1 _form;
        private Charts _charts;
        private DataGridViewHandler dataGridViewHandler;
        private _3DSimulation simulation;

        private Queue<TelemetryData> telemetryQueue = new Queue<TelemetryData>();
        private Timer processTimer;

        public Data(SerialPort port, Map map, Form1 form, Charts charts, DataGridViewHandler dataGridViewHandler, _3DSimulation simulation)
        {
            _port = port;
            _map = map;
            _form = form;
            _charts = charts;
            this.dataGridViewHandler = dataGridViewHandler;
            this.simulation = simulation;

            _port.DataReceived += SerialPort_DataReceived;

            // Timer ayarları
            processTimer = new Timer();
            processTimer.Interval = 100; // 10 Hz (saniyede 10 kez kontrol)
            processTimer.Tick += ProcessTelemetry;
            processTimer.Start();
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = _port.ReadLine();
                string[] lines = data.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string line in lines)
                {
                    TelemetryData telemetry = TelemetryData.Parse(line.Trim());
                    if (telemetry != null)
                    {
                        lock (telemetryQueue)
                        {
                            telemetryQueue.Enqueue(telemetry);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata: " + ex.Message);
            }
        }

        private void ProcessTelemetry(object sender, EventArgs e)
        {
            if (telemetryQueue.Count > 0)
            {
                TelemetryData telemetry = null;
                lock (telemetryQueue)
                {
                    telemetry = telemetryQueue.Dequeue();
                }

                if (telemetry != null)
                {
                    // Ana UI Thread'de güncelleme
                    _form.BeginInvoke((MethodInvoker)(() =>
                    {
                        _map.UpdatePosition(telemetry);
                        _charts.Update(telemetry);
                        dataGridViewHandler.AddTelemetry(telemetry);
                        simulation.UpdateRotation(telemetry.Yaw, telemetry.Pitch, telemetry.Roll);
                        
                    }));
                }
            }
        }

        public void Connect()
        {
            if (!_port.IsOpen)
                _port.Open();
        }

        public void Disconnect()
        {
            if (_port.IsOpen)
                _port.Close();
        }
    }
}
