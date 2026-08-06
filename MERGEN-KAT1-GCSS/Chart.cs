using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MERGEN_KAT1_GCSS
{
    public class Charts
    {
        private readonly Chart[] _charts;
        private const int MAX_POINTS = 1000;

        public Charts(Chart ch1, Chart ch2, Chart ch3, Chart ch4, Chart ch5)
        {
            _charts = new[] { ch1, ch2, ch3, ch4, ch5 };

            // Chart optimizasyonları
            foreach (var chart in _charts)
            {
                // Double buffering
                typeof(Chart).GetProperty("DoubleBuffered",
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.NonPublic)?
                    .SetValue(chart, true, null);

                foreach (var series in chart.Series)
                {
                   // series.ChartType = SeriesChartType.FastLine;
                    series.BorderWidth = 1;
                }
            }
        }

        public void Update(TelemetryData data)
        {
            if (_charts[0].InvokeRequired)
            {
                _charts[0].BeginInvoke((MethodInvoker)(() => Update(data)));
                return;
            }

            string timeLabel = FormatTimeLabel(data.GondermeSaati);

            // Chart 1: Basınç 1 ve 2
            UpdateChart(_charts[0], timeLabel, data.Basinc);

            // Chart 2: Yükseklik 1
            UpdateChart(_charts[1], timeLabel, data.Yukseklik);

            // Chart 3: Yükseklik 2
            UpdateChart(_charts[2], timeLabel, data.InisHizi);

            // Chart 4: İrtifa Farkı
            UpdateChart(_charts[3], timeLabel, data.Sicaklik);

            // Chart 5: İniş Hızı
            UpdateChart(_charts[4], timeLabel, data.PilGerilimi);

           
        }

        private void UpdateChart(Chart chart, string timeLabel, params double[] values)
        {
            for (int i = 0; i < values.Length && i < chart.Series.Count; i++)
            {
                var series = chart.Series[i];
                if (series.Points.Count > MAX_POINTS)
                {
                    series.Points.RemoveAt(0);
                }
                series.Points.AddXY(timeLabel, values[i]);
            }
        }

        private string FormatTimeLabel(string rawTime)
        {
            // rawTime örneği: "06/08/2026 10:38:24"
            if (string.IsNullOrWhiteSpace(rawTime))
                return "";

            // DateTime olarak parse etmeyi dener, başarılıysa sadece HH:mm:ss formatında saat kısmını döndürür
            if (DateTime.TryParse(rawTime, out DateTime dt))
            {
                return dt.ToString("HH:mm:ss");
            }

            // Eğer standart bir DateTime formatında değilse boşluğa göre ayırıp ikinci kısmı (saati) almayı dener
            string[] parts = rawTime.Trim().Split(' ');
            if (parts.Length > 1)
            {
                return parts[parts.Length - 1];
            }

            // Hiçbir şart sağlanmazsa (beklenmeyen bir veri gelirse) çökmemesi için orijinal veriyi döndür
            return rawTime;
        }
    }
}