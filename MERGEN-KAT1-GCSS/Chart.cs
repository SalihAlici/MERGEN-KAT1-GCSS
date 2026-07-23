using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MERGEN_KAT1_GCSS
{
    public class Charts
    {
        private readonly Chart[] _charts;
        private const int MAX_POINTS = 1000;

        public Charts(Chart ch1, Chart ch2, Chart ch3, Chart ch4)
        {
            _charts = new[] { ch1, ch2, ch3, ch4 };

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

            string timeLabel = FormatTimeLabel(data.Zaman);

            
            UpdateChart(_charts[0], timeLabel, data.GoreceliYukseklik);

            
            UpdateChart(_charts[1], timeLabel, data.Basinc);

            
            UpdateChart(_charts[2], timeLabel, data.InisHizi);

            
            UpdateChart(_charts[3], timeLabel, data.PilGerilimi);

            
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
            string[] parcalar = rawTime.Split(' ');

            
            if (parcalar.Length > 1)
            {
                
                return parcalar[1];
            }

            
            return rawTime;
        }

        
    }
}