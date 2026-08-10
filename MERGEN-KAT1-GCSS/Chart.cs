using System;
using System.Drawing;
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

            // Grafikleri daha cancanlı göstermek için neon renk paleti (Sırasıyla 5 grafik için)
            Color[] neonColors = {
                Color.FromArgb(0, 212, 255),   // Açık Mavi (Basınç)
                Color.FromArgb(255, 193, 7),   // Sarı/Turuncu (Yükseklik)
                Color.FromArgb(0, 230, 118),   // Neon Yeşil (İniş Hızı)
                Color.FromArgb(255, 64, 129),  // Pembe/Kırmızı (Sıcaklık)
                Color.FromArgb(178, 143, 206)  // Mor (Pil Gerilimi)
            };

            for (int i = 0; i < _charts.Length; i++)
            {
                var chart = _charts[i];
                var chartArea = chart.ChartAreas[0];

                // 1. Double buffering (Titremeyi engeller)
                typeof(Chart).GetProperty("DoubleBuffered",
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.NonPublic)?
                    .SetValue(chart, true, null);

                // 2. Arka Planı ve Kenarlıkları Temizleme
                chart.BackColor = Color.Transparent;
                chartArea.BackColor = Color.Transparent;

                // 3. X Ekseni Ayarları (Saat kısmı)
                chartArea.AxisX.LabelStyle.ForeColor = Color.LightGray; // Yazı rengi
                chartArea.AxisX.LabelStyle.Font = new Font("Segoe UI", 8, FontStyle.Regular);
                chartArea.AxisX.LabelStyle.Angle = -45; // Yazıları 45 derece eğik yaz (çakışmayı önler)
                chartArea.AxisX.LineColor = Color.Gray; // Alt çizgi rengi
                chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(30, 255, 255, 255); // Çok silik beyaz ızgara
                chartArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash; // Kesik çizgi

                // 4. Y Ekseni Ayarları (Değerler)
                //chartArea.AxisY.IsStartedFromZero = false; // EN ÖNEMLİSİ: Grafiğin dalgalanmalarını gösterir
                chartArea.AxisY.LabelStyle.ForeColor = Color.LightGray;
                chartArea.AxisY.LabelStyle.Font = new Font("Segoe UI", 8, FontStyle.Regular);
                chartArea.AxisY.LineColor = Color.Gray;
                chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(30, 255, 255, 255);
                chartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

                // 5. Başlık (Title) Varsa Stilini Ayarla
                if (chart.Titles.Count > 0)
                {
                    chart.Titles[0].ForeColor = Color.White;
                    chart.Titles[0].Font = new Font("Segoe UI", 10, FontStyle.Bold);
                }

                // 6. Çizgi (Series) Ayarları
                foreach (var series in chart.Series)
                {
                    series.ChartType = SeriesChartType.Spline; // Keskin hatlar yerine yumuşak kıvrımlı çizgiler çizer
                    series.BorderWidth = 3; // Çizgiyi kalınlaştırdık
                    series.Color = neonColors[i]; // Yukardaki neon renkleri ata

                    // Veri noktalarındaki yuvarlak işaretçileri kapat ki sadece temiz çizgi kalsın
                    series.MarkerStyle = MarkerStyle.None;
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