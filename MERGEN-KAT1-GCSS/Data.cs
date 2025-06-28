using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Markup;
using OpenTK;





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
        public Data(SerialPort port, Map map, Form1 form, Charts charts, DataGridViewHandler dataGridViewHandler, _3DSimulation simulation)
        {
            _port = port;
            _map = map;
            _form = form;
            _port.DataReceived += SerialPort_DataReceived;
            _charts = charts;
            this.dataGridViewHandler = dataGridViewHandler;
            this.simulation = simulation;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string incoming = _port.ReadLine();
                Console.WriteLine(incoming);
                TelemetryData telemetry = TelemetryData.Parse(incoming);

                if (telemetry != null)
                {
                    _form.Invoke((MethodInvoker)(() =>
                    {
                        _map.UpdatePosition(telemetry);
                        _charts.Update(telemetry);
                        dataGridViewHandler.AddTelemetry(telemetry);
                        simulation.UpdateRotation(telemetry.Yaw, telemetry.Pitch, telemetry.Roll);
                        
                    }));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata: " + ex.Message);
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
