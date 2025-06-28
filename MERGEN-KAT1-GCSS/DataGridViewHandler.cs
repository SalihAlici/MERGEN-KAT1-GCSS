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
            
            _list.Insert(0, data); // Yeni veriyi en başa ekle
        }

        public void Clear()
        {
            _list.Clear();
        }



        public void SaveDataGridViewToCSV()
        {
            try
            {
                if (_list.Count == 0) return;

                StringBuilder csvContent = new StringBuilder();

                // Property'leri al
                var props = typeof(TelemetryData).GetProperties();
                csvContent.AppendLine(string.Join(";", props.Select(p => p.Name)));

                foreach (var item in _list)
                {
                    var values = props.Select(p => p.GetValue(item)?.ToString() ?? "");
                    csvContent.AppendLine(string.Join(";", values));
                }

                File.WriteAllText(_csvFilePath, csvContent.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show("CSV kaydı sırasında hata: " + ex.Message);
            }
        }

    }
}
