using System;
using System.IO.Ports;

namespace MERGEN_KAT1_GCSS
{
    public class ArduinoReader
    {
        private SerialPort serialPort;

        public event EventHandler<ArduinoDataEventArgs> DataReceived;

        // Constructor
        public ArduinoReader()
        {
            serialPort = new SerialPort
            {
                BaudRate = 9600
            };
        }

        // COM portları alıp döndüren metot
        public string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames(); // Bilgisayara bağlı olan tüm portları al
        }

        // Arduino'ya bağlanma
        public bool Start(string portName)
        {
            if (serialPort.IsOpen)
                return false;

            serialPort.PortName = portName;
            serialPort.DataReceived += OnDataReceived;
            serialPort.Open();
            return true;
        }

        // Arduino'dan gelen veri işleme
        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort.ReadLine(); // cekince burda hata aldım böööyle çıkmasın veri gelmeyebilr
            string[] values = data.Split(',');

            if (values.Length == 3)
            {
                float yaw = float.Parse(values[0]);
                float pitch = float.Parse(values[1]);
                float roll = float.Parse(values[2]);

                DataReceived?.Invoke(this, new ArduinoDataEventArgs(yaw, pitch, roll));
            }
        }

        // Arduino'dan bağlantıyı kesme
        public void Stop()
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }
    }

    public class ArduinoDataEventArgs : EventArgs
    {
        public float Yaw { get; }
        public float Pitch { get; }
        public float Roll { get; }

        public ArduinoDataEventArgs(float yaw, float pitch, float roll)
        {
            Yaw = yaw;
            Pitch = pitch;
            Roll = roll;
        }
    }
}
