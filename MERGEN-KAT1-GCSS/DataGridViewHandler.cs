using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MERGEN_KAT1_GCSS
{
    public class DataGridViewHandler
    {
        private readonly BindingList<TelemetryData> _list;
        private readonly DataGridView _dataGridView;
        private readonly string _csvFilePath;
        private readonly object _csvLock = new object();
        private const int MAX_ROWS = 500;

        public DataGridViewHandler(DataGridView dgv, string csvFilePath)
        {
            _dataGridView = dgv;
            _csvFilePath = csvFilePath;
            _list = new BindingList<TelemetryData>();

            // DataGridView performans ayarı: flicker önleme
            typeof(DataGridView).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic)?
                .SetValue(_dataGridView, true, null);

            _dataGridView.DataSource = _list;

            // Kolonların sıralanabilir olmasını engelle
            _dataGridView.DataBindingComplete += (s, e) =>
            {
                foreach (DataGridViewColumn col in _dataGridView.Columns)
                {
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            };
        }

        public void AddTelemetry(TelemetryData data)
        {
            if (_dataGridView.InvokeRequired)
            {
                _dataGridView.BeginInvoke((MethodInvoker)(() => AddTelemetry(data)));
                return;
            }

            // Scroll pozisyonunu hatırla
            int firstDisplayedIndex = -1;
            try
            {
                firstDisplayedIndex = _dataGridView.FirstDisplayedScrollingRowIndex;
            }
            catch { }

            // Yeni veriyi en üste ekle
            _list.Insert(0, data);

            // 500'den fazla ise en alttaki veriyi sil
            if (_list.Count > MAX_ROWS)
            {
                _list.RemoveAt(_list.Count - 1);
            }

            // Scroll pozisyonunu geri yükle
            try
            {
                if (_dataGridView.RowCount > 0 && firstDisplayedIndex >= 0 && firstDisplayedIndex < _dataGridView.RowCount)
                {
                    _dataGridView.FirstDisplayedScrollingRowIndex = firstDisplayedIndex;
                }
            }
            catch { }

            // CSV'ye arka planda yaz
            Task.Run(() => AppendToCsv(data));
        }

        private void AppendToCsv(TelemetryData data)
        {
            lock (_csvLock)
            {
                try
                {
                    var props = typeof(TelemetryData).GetProperties();

                    if (!File.Exists(_csvFilePath))
                    {
                        File.WriteAllText(_csvFilePath,
                            string.Join(";", props.Select(p => p.Name)) + Environment.NewLine,
                            Encoding.UTF8);
                    }

                    File.AppendAllText(_csvFilePath,
                        string.Join(";", props.Select(p => p.GetValue(data)?.ToString() ?? "")) + Environment.NewLine,
                        Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"CSV Write Error: {ex.Message}");
                }
            }
        }

        public void Clear()
        {
            _list.Clear();
        }
    }
}
