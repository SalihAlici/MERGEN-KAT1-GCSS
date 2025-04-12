using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MERGEN_KAT1_GCSS
{
    public class Charts
    {
        private Chart[] charts;

        public Charts(Chart[] charts)
        {
            this.charts = charts;
        }

        public void InitializeCharts()
        {
            // Chart1charts[0].Series["Series1"].Points.AddXY("12:02:10", 989);
            charts[0].Series["Series1"].Points.AddXY("12:02:10", 990);
            charts[0].Series["Series1"].Points.AddXY("12:02:11", 991);
            charts[0].Series["Series1"].Points.AddXY("12:02:12", 993);
            charts[0].Series["Series1"].Points.AddXY("12:02:13", 992);
            charts[0].Series["Series1"].Points.AddXY("12:02:14", 994);

            // Y ekseni sınırlarını ayarla
            charts[0].ChartAreas[0].AxisY.Minimum = 989;
            charts[0].ChartAreas[0].AxisY.Maximum = 995;



            // Chart2
            charts[1].ChartAreas[0].AxisY.Minimum = 10;
            charts[1].ChartAreas[0].AxisY.Maximum = 2000;

            charts[1].Series["Series1"].Points.AddXY("12:02:10", 200); // basınç ~990
            charts[1].Series["Series1"].Points.AddXY("12:02:11", 255); // basınç ~987
            charts[1].Series["Series1"].Points.AddXY("12:02:12", 370); // basınç ~992
            charts[1].Series["Series1"].Points.AddXY("12:02:13", 425); // basınç ~989
            charts[1].Series["Series1"].Points.AddXY("12:02:14", 510); // basınç ~995
            // basınç ~995

            // Chart3
            charts[2].Series["Series1"].Points.AddXY("12:02:10", 10);
            charts[2].Series["Series1"].Points.AddXY("12:02:11", 12);
            charts[2].Series["Series1"].Points.AddXY("12:02:12", 15);
            charts[2].Series["Series1"].Points.AddXY("12:02:13", 15);
            charts[2].Series["Series1"].Points.AddXY("12:02:14", 17);
            charts[2].Series["Series1"].Points.AddXY("12:02:15", 18);
            charts[2].Series["Series1"].Points.AddXY("12:02:16", 19);
            charts[2].ChartAreas[0].AxisY.Minimum = 0;
            charts[2].ChartAreas[0].AxisY.Maximum = 200;

            // Chart4,
            charts[3].ChartAreas[0].AxisY.Minimum = 0;
            charts[3].ChartAreas[0].AxisY.Maximum = 2000;

            charts[3].Series["Series1"].Points.AddXY("12:02:10", 150); // 12 m/s hız, 12 m mesafe
            charts[3].Series["Series1"].Points.AddXY("12:02:11", 162); // 17 m/s hız, 17 m mesafe
            charts[3].Series["Series1"].Points.AddXY("12:02:12", 170); // 15 m/s hız, 15 m mesafe
            charts[3].Series["Series1"].Points.AddXY("12:02:13", 181); // 10 m/s hız, 10 m mesafe
            charts[3].Series["Series1"].Points.AddXY("12:02:14", 189); // 14 m/s hız, 14 m mesafe
            charts[3].Series["Series1"].Points.AddXY("12:02:15", 170); // 15 m/s hız, 15 m mesafe
            charts[3].Series["Series1"].Points.AddXY("12:02:16", 181); // 10 m/s hız, 10 m mesafe
            charts[3].Series["Series1"].Points.AddXY("12:02:17", 189); // 14 m/s hız, 14 m mesafe


            // Chart5
            charts[4].Series["Series1"].Points.AddXY("12:02:10", 25); // 25 m/s hız, 25 m mesafe
            charts[4].Series["Series1"].Points.AddXY("12:02:11", 26); // 26 m/s hız, 26 m mesafe
            charts[4].Series["Series1"].Points.AddXY("12:02:12", 27); // 27 m/s hız, 27 m mesafe
            charts[4].Series["Series1"].Points.AddXY("12:02:13", 28); // 28 m/s hız, 28 m mesafe
            charts[4].Series["Series1"].Points.AddXY("12:02:14", 29); // 25 m/s hız, 25 m mesafe
            charts[4].Series["Series1"].Points.AddXY("12:02:15", 30); // 27 m/s hız, 27 m mesafe
            charts[4].Series["Series1"].Points.AddXY("12:02:16", 31); // 28 m/s hız, 28 m mesafe
            charts[4].Series["Series1"].Points.AddXY("12:02:17", 32); // 25 m/s hız, 25 m mesafe

            charts[4].ChartAreas[0].AxisY.Minimum = -40;
            charts[4].ChartAreas[0].AxisY.Maximum = 40;



            // Chart6
            charts[5].Series["Series1"].Points.AddXY("12:02:10", 15); // 15
            charts[5].Series["Series1"].Points.AddXY("12:02:11", 16); // 16
            charts[5].Series["Series1"].Points.AddXY("12:02:12", 17); // 17
            charts[5].Series["Series1"].Points.AddXY("12:02:13", 18); // 18
            charts[5].Series["Series1"].Points.AddXY("12:02:14", 19); // 19
            charts[5].Series["Series1"].Points.AddXY("12:02:15", 20); // 17
            charts[5].Series["Series1"].Points.AddXY("12:02:16", 21); // 18
            charts[5].Series["Series1"].Points.AddXY("12:02:17", 22); // 19
            charts[5].ChartAreas[0].AxisY.Minimum = -40;
            charts[5].ChartAreas[0].AxisY.Maximum = 40;

            // Chart7
            charts[6].Series["Series1"].Points.AddXY("12:02:10", 3.70); // 3.70V
            charts[6].Series["Series1"].Points.AddXY("12:02:11", 3.70); // 3.68V
            charts[6].Series["Series1"].Points.AddXY("12:02:12", 3.67); // 3.66V
            charts[6].Series["Series1"].Points.AddXY("12:02:13", 3.66); // 3.64V
            charts[6].Series["Series1"].Points.AddXY("12:02:14", 3.66); // 3.62V
            charts[6].Series["Series1"].Points.AddXY("12:02:15", 3.66); // 3.66V
            charts[6].Series["Series1"].Points.AddXY("12:02:16", 3.65); // 3.64V
            charts[6].Series["Series1"].Points.AddXY("12:02:17", 3.65); // 3.62V
            charts[6].ChartAreas[0].AxisY.Minimum = 0;
            charts[6].ChartAreas[0].AxisY.Maximum = 12;

            // Chart8
            charts[7].Series["Series1"].Points.AddXY("12:02:10", 95.0); // %95
            charts[7].Series["Series1"].Points.AddXY("12:02:11", 94.8); // %94.8
            charts[7].Series["Series1"].Points.AddXY("12:02:12", 94.6); // %94.6
            charts[7].Series["Series1"].Points.AddXY("12:02:13", 94.4); // %94.4
            charts[7].Series["Series1"].Points.AddXY("12:02:14", 94.2); // %94.2
            charts[7].Series["Series1"].Points.AddXY("12:02:15", 94.2); // %94.6
            charts[7].Series["Series1"].Points.AddXY("12:02:16", 94.1); // %94.4
            charts[7].Series["Series1"].Points.AddXY("12:02:17", 94.1); // %94.2
            charts[7].ChartAreas[0].AxisY.Minimum = 90;
            charts[7].ChartAreas[0].AxisY.Maximum = 100;




        }
    }
}
