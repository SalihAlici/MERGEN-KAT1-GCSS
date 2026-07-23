
using System;
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace MERGEN_KAT1_GCSS
{
    public class Data : IDisposable
    {
        private readonly SerialPort _port;
        private readonly Form1 _form;
        private readonly Map _map;
        private readonly Charts _charts;
        private readonly DataGridViewHandler _dataGridHandler;
        

        private readonly BlockingCollection<TelemetryData> _processingQueue = new BlockingCollection<TelemetryData>(1000);

        private Thread _serialReaderThread;
        private Thread _dataProcessorThread;

        private readonly object _serialLock = new object();
        private volatile bool _running = true;

        private string _serialBuffer = "";  // Gelen veriyi biriktirmek için buffer

        public Data(SerialPort port, Map map, Form1 form, Charts charts,
                    DataGridViewHandler dataGridHandler)
        {
            _port = port;
            _map = map;
            _form = form;
            _charts = charts;
            
            _dataGridHandler = dataGridHandler;
           

            StartThreads();
        }

        private void StartThreads()
        {
            _running = true;

            _serialReaderThread = new Thread(ReadSerialData)
            {
                Name = "SerialReaderThread",
                IsBackground = true,
                Priority = ThreadPriority.Highest
            };
            _serialReaderThread.Start();

            _dataProcessorThread = new Thread(ProcessData)
            {
                Name = "DataProcessorThread",
                IsBackground = true,
                Priority = ThreadPriority.Normal
            };
            _dataProcessorThread.Start();
        }

        private void ReadSerialData()
        {
            while (_running)
            {
                try
                {
                    if (_port != null && _port.IsOpen && _port.BytesToRead > 0)
                    {
                        string incomingData;
                        lock (_serialLock)
                        {
                            incomingData = _port.ReadExisting();
                        }

                        if (!string.IsNullOrEmpty(incomingData))
                        {
                            _serialBuffer += incomingData;
                            Console.WriteLine("[Ham Veri] " + incomingData);

                            // TELEMETRİ paketlerini döngü ile işle
                            while (true)
                            {
                                int startIndex = _serialBuffer.IndexOf('$');
                                int endIndex = _serialBuffer.IndexOf('#');

                                if (startIndex == -1 && endIndex == -1)
                                {
                                    if (_serialBuffer.Length > 0)
                                    {
                                        Console.WriteLine("[UYARI] Buffer'da paket işaretçisi yok, temizleniyor.");
                                        _serialBuffer = "";
                                    }
                                    break;
                                }
                                else if (startIndex == -1 && endIndex != -1)
                                {
                                    _serialBuffer = _serialBuffer.Substring(endIndex + 1);
                                    continue;
                                }
                                else if (startIndex != -1 && (endIndex == -1 || endIndex < startIndex))
                                {
                                    if (_serialBuffer.Length > 5000)
                                    {
                                        Console.WriteLine("[UYARI] Buffer çok büyüdü, temizleniyor.");
                                        _serialBuffer = "";
                                    }
                                    break;
                                }
                                else if (startIndex != -1 && endIndex > startIndex)
                                {
                                    string packet = _serialBuffer.Substring(startIndex + 1, endIndex - startIndex - 1).Trim();

                                    if (TelemetryData.TryParse(packet, out var telemetry))
                                    {
                                        _processingQueue.Add(telemetry);
                                    }
                                    else
                                    {
                                        Console.WriteLine("[Parse Hatası] Paket parse edilemedi, atılıyor: " + packet);
                                    }

                                    _serialBuffer = _serialBuffer.Substring(endIndex + 1);
                                    continue;
                                }
                                else
                                {
                                    Console.WriteLine("[UYARI] Bilinmeyen buffer durumu, temizleniyor.");
                                    _serialBuffer = "";
                                    break;
                                }
                            }
                        }
                    }

                    Thread.Sleep(5);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Serial Read Error: {ex.Message}");
                    Thread.Sleep(50);
                }
            }
        }

        private void ProcessData()
        {
            while (_running)
            {
                try
                {
                    if (_processingQueue.TryTake(out var telemetry, 50))
                    {
                        _form.Invoke((MethodInvoker)(() =>
                        {
                            _map.UpdatePosition(telemetry);
                            _charts.Update(telemetry);
                            _dataGridHandler.AddTelemetry(telemetry);
                            
                          
                            
                        }));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"UI Update Error: {ex.Message}");
                }
            }
        }

        public void SendCommand(string command)
        {
            if (_port == null || !_port.IsOpen) return;

            lock (_serialLock)
            {
                try
                {
                    _port.DiscardInBuffer();
                    _port.WriteLine(command);
                    Console.WriteLine($"Sent command: {command}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Serial Write Error: {ex.Message}");
                }
            }
        }

        public void Connect()
        {
            try
            {
                if (!_port.IsOpen)
                {
                    _port.Open();
                    _port.Encoding = System.Text.Encoding.ASCII;
                    _port.DiscardInBuffer();
                    _port.DiscardOutBuffer();
                    Console.WriteLine("Serial port connected");

                    if (_serialReaderThread == null || !_serialReaderThread.IsAlive ||
                        _dataProcessorThread == null || !_dataProcessorThread.IsAlive)
                    {
                        StartThreads();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connect Error: {ex.Message}");
            }
        }

        public void Disconnect()
        {
            try
            {
                if (_port.IsOpen)
                    _port.Close();

                Console.WriteLine("Serial port disconnected");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Disconnect Error: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _running = false;

            _serialReaderThread?.Join(500);
            _dataProcessorThread?.Join(500);

            _processingQueue.Dispose();
        }
    }
}
