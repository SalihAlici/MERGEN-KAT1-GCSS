using MERGEN_KAT1_GCSS;
using System.Globalization;
using System.IO.Ports;
using System;
using System.Windows.Forms;

internal class veri
{
    private Form1 form;
    private Charts chartHandler;
    private _3DSimulation simulation;
    private SerialPort serialPort1;
    private _3DSimulation _simulation;


    public veri(Form1 form, _3DSimulation sim)
    {
        this.form = form;
        _simulation = sim;
    }

    public void PortAc(string portName, int baudRate)
    {
        if (serialPort1 != null && serialPort1.IsOpen)
            serialPort1.Close();

        serialPort1 = new SerialPort(portName, baudRate);
        serialPort1.DataReceived += SerialPort_DataReceived;
        serialPort1.Open();
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            string gelenVeri = serialPort1.ReadLine().Trim(); // Seri porttan satır oku,

            string[] parcalar = gelenVeri.Split('*'); // '*' ile parçala

            if (parcalar.Length == 22) // 22 parça bekliyoruz
            {
                // Parse işlemleri
                float pitch = float.Parse(parcalar[15], CultureInfo.InvariantCulture);
                float roll = float.Parse(parcalar[16], CultureInfo.InvariantCulture);
                float yaw = float.Parse(parcalar[17], CultureInfo.InvariantCulture);
                Console.WriteLine($"[veri.cs] Gelen => Pitch: {pitch}, Roll: {roll}, Yaw: {yaw}");

                form?.Invoke(new Action(() =>
                {
                    _simulation.UpdateRotation(yaw, pitch, roll);
                }));
            }
           
        }
        catch (Exception ex)
        {
            // Hata yönetimi, örn: parse hatası
        }
    }


    public void PortKapat()
    {
        if (serialPort1 != null && serialPort1.IsOpen)
            serialPort1.Close();
    }
}
