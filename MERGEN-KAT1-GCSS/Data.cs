using System;
using System.Collections.Concurrent;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MERGEN_KAT1_GCSS
{
    public class Data
    {
        private SerialPort _port;
        private Form1 _form;
        private Map _map;
        private Charts _charts;
        private DataGridViewHandler dataGridViewHandler;
        private _3DSimulation simulation;
        private Aras _aras;
        // Thread-safe kuyruk
        private ConcurrentQueue<TelemetryData> telemetryQueue = new ConcurrentQueue<TelemetryData>();
        private bool isProcessing = false;

        private string serialBuffer = "";

        public Data(SerialPort port, Map map, Form1 form, Charts charts, DataGridViewHandler dataGridViewHandler, _3DSimulation simulation,Aras aras)
        {
            _port = port;
            _map = map;
            _form = form;
            _charts = charts;
            this.dataGridViewHandler = dataGridViewHandler;
            this.simulation = simulation;
            _aras = aras;
            _port.DataReceived += SerialPort_DataReceived;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = _port.ReadExisting();
            serialBuffer += data;
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} - Gelen veri: {data}");
            int newlineIndex;
            while ((newlineIndex = serialBuffer.IndexOf('\n')) >= 0)
            {
                string line = serialBuffer.Substring(0, newlineIndex).Trim();
                serialBuffer = serialBuffer.Substring(newlineIndex + 1);

                TelemetryData telemetry = TelemetryData.Parse(line);
                if (telemetry != null)
                {
                    telemetryQueue.Enqueue(telemetry);
                }
            }

            // Veri geldikten sonra arka planda işlem başlat
            ProcessQueueAsync();
        }

        private async void ProcessQueueAsync()
        {
            if (isProcessing) return; // Aynı anda birden fazla çağrıyı engelle

            isProcessing = true;

            await Task.Run(() =>
            {
                while (telemetryQueue.TryDequeue(out TelemetryData telemetry))
                {
                    // Ağır işlemler varsa burada yapabilirsin (örn. filtreleme, hesaplama)
                    
                    // UI güncellemesini ana thread’de yap
                    _form.BeginInvoke((MethodInvoker)(() =>
                    {
                        //  _map.UpdatePosition(telemetry);
                        simulation.UpdateRotation(telemetry.Yaw, telemetry.Pitch, telemetry.Roll);
                        Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} - UpdateRotation çağrıldı");
                        _map.UpdatePosition(telemetry);
                        Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} - map çağrıldı");
                        _charts.Update(telemetry);
                        Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} - chart çağrıldı");
                        dataGridViewHandler.AddTelemetry(telemetry);
                        Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} - datagrid çağrıldı");
                        _aras.Update(telemetry.HataKodu);
                    }));
                }
            });

            isProcessing = false;
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
