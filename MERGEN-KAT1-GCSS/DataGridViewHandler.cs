using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MERGEN_KAT1_GCSS
{
    public class DataGridViewHandler
    {
        private BindingList<TelemetryData> _list;
        private DataGridView _dataGridView;
        private string _csvFilePath;

        public DataGridViewHandler(DataGridView dgv, string csvFilePath)
        {
            _dataGridView = dgv;
            _csvFilePath = csvFilePath;

            _list = new BindingList<TelemetryData>();
            _dataGridView.DataSource = _list;
        }

        public void AddTelemetry(TelemetryData data)
        {
            _list.Insert(0, data);
            AppendNewDataToCSV(data); // Tek kayıtla CSV güncellenir
        }

        private void AppendNewDataToCSV(TelemetryData data)
        {
            try
            {
                var props = typeof(TelemetryData).GetProperties();

                // Dosya yoksa başlık ekle
                if (!File.Exists(_csvFilePath))
                {
                    var header = string.Join(";", props.Select(p => p.Name));
                    File.AppendAllText(_csvFilePath, header + Environment.NewLine, Encoding.UTF8);
                }

                // Yeni veri satırı
                var values = props.Select(p => p.GetValue(data)?.ToString() ?? "");
                string newLine = string.Join(";", values);

                File.AppendAllText(_csvFilePath, newLine + Environment.NewLine, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show("CSV yazım hatası: " + ex.Message);
            }
        }

        public void Clear()
        {
            _list.Clear();
        }
    }
}
