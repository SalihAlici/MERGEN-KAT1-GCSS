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
            // Chart1
            // Chart1charts[0].Series["Series1"].Points.AddXY("12:02:10", 989);

            charts[0].ChartAreas[0].AxisY.Minimum = 600;
            charts[0].ChartAreas[0].AxisY.Maximum = 1200;

            charts[0].Series["Series1"].Points.AddXY("12:02:10", 640);
            charts[0].Series["Series1"].Points.AddXY("12:02:11", 640);
            charts[0].Series["Series1"].Points.AddXY("12:02:12", 641);
            charts[0].Series["Series1"].Points.AddXY("12:02:13", 641);
            charts[0].Series["Series1"].Points.AddXY("12:02:14", 642);
            charts[0].Series["Series1"].Points.AddXY("12:02:15", 642);
            charts[0].Series["Series1"].Points.AddXY("12:02:16", 642);

            charts[0].Series["Series2"].Points.AddXY("12:02:10", 645);
            charts[0].Series["Series2"].Points.AddXY("12:02:11", 645);
            charts[0].Series["Series2"].Points.AddXY("12:02:12", 646);
            charts[0].Series["Series2"].Points.AddXY("12:02:13", 646);
            charts[0].Series["Series2"].Points.AddXY("12:02:14", 647);
            charts[0].Series["Series2"].Points.AddXY("12:02:15", 647);
            charts[0].Series["Series2"].Points.AddXY("12:02:16", 648);


            // Chart2
            charts[1].ChartAreas[0].AxisY.Minimum = 10;
            charts[1].ChartAreas[0].AxisY.Maximum = 1000;
     
            charts[1].Series["Series1"].Points.AddXY("12:02:10", 560.3);
            charts[1].Series["Series1"].Points.AddXY("12:02:11", 560);
            charts[1].Series["Series1"].Points.AddXY("12:02:12", 559.5);
            charts[1].Series["Series1"].Points.AddXY("12:02:14", 558.3);
            charts[1].Series["Series1"].Points.AddXY("12:02:15", 558);
            charts[1].Series["Series1"].Points.AddXY("12:02:16", 557.9);
            charts[1].Series["Series1"].Points.AddXY("12:02:17", 556);

            // Chart3
            charts[2].ChartAreas[0].AxisY.Minimum = 0;
            charts[2].ChartAreas[0].AxisY.Maximum = 1000;

            charts[2].Series["Series1"].Points.AddXY("12:02:10", 540);
            charts[2].Series["Series1"].Points.AddXY("12:02:11", 540);
            charts[2].Series["Series1"].Points.AddXY("12:02:12", 539);
            charts[2].Series["Series1"].Points.AddXY("12:02:14", 538);
            charts[2].Series["Series1"].Points.AddXY("12:02:15", 537);
            charts[2].Series["Series1"].Points.AddXY("12:02:16", 536);
            charts[2].Series["Series1"].Points.AddXY("12:02:17", 535);

            // Chart4
            charts[3].ChartAreas[0].AxisY.Minimum = 0;
            charts[3].ChartAreas[0].AxisY.Maximum = 500;

            charts[3].Series["Series1"].Points.AddXY("12:02:10", 20);
            charts[3].Series["Series1"].Points.AddXY("12:02:11", 22);
            charts[3].Series["Series1"].Points.AddXY("12:02:12", 23);
            charts[3].Series["Series1"].Points.AddXY("12:02:14", 24);
            charts[3].Series["Series1"].Points.AddXY("12:02:15", 26);
            charts[3].Series["Series1"].Points.AddXY("12:02:16", 27);
            charts[3].Series["Series1"].Points.AddXY("12:02:17", 25);

            // Chart5

            charts[4].ChartAreas[0].AxisY.Minimum = 0;
            charts[4].ChartAreas[0].AxisY.Maximum = 30;

            charts[4].Series["Series1"].Points.AddXY("12:02:10", 9);
            charts[4].Series["Series1"].Points.AddXY("12:02:11", 9.1);
            charts[4].Series["Series1"].Points.AddXY("12:02:12", 9.2);
            charts[4].Series["Series1"].Points.AddXY("12:02:14", 9.3);
            charts[4].Series["Series1"].Points.AddXY("12:02:15", 9.3);
            charts[4].Series["Series1"].Points.AddXY("12:02:16", 9.3);
            charts[4].Series["Series1"].Points.AddXY("12:02:17", 9.5);


            // Chart6
            charts[5].ChartAreas[0].AxisY.Minimum = 0;
            charts[5].ChartAreas[0].AxisY.Maximum = 50;

            charts[5].Series["Series1"].Points.AddXY("12:02:10", 22.5);
            charts[5].Series["Series1"].Points.AddXY("12:02:11", 22.4);
            charts[5].Series["Series1"].Points.AddXY("12:02:12", 22.7);
            charts[5].Series["Series1"].Points.AddXY("12:02:14", 22.3);
            charts[5].Series["Series1"].Points.AddXY("12:02:15", 22.6);
            charts[5].Series["Series1"].Points.AddXY("12:02:16", 22.8);
            charts[5].Series["Series1"].Points.AddXY("12:02:17", 23);

            // Chart7
            charts[6].ChartAreas[0].AxisY.Minimum = 0;
            charts[6].ChartAreas[0].AxisY.Maximum = 10;

            charts[6].Series["Series1"].Points.AddXY("12:02:10", 3.89);
            charts[6].Series["Series1"].Points.AddXY("12:02:11", 3.89);
            charts[6].Series["Series1"].Points.AddXY("12:02:12", 3.89);
            charts[6].Series["Series1"].Points.AddXY("12:02:14", 3.89);
            charts[6].Series["Series1"].Points.AddXY("12:02:15", 3.89);
            charts[6].Series["Series1"].Points.AddXY("12:02:16", 3.89);
            charts[6].Series["Series1"].Points.AddXY("12:02:17", 3.89);

            // Chart8




            charts[7].ChartAreas[0].AxisY.Minimum = 0;
            charts[7].ChartAreas[0].AxisY.Maximum = 50;

            charts[7].Series["Series1"].Points.AddXY("12:02:10", 23);
            charts[7].Series["Series1"].Points.AddXY("12:02:11", 22);
            charts[7].Series["Series1"].Points.AddXY("12:02:12", 23);
            charts[7].Series["Series1"].Points.AddXY("12:02:14", 22.5);
            charts[7].Series["Series1"].Points.AddXY("12:02:15", 22.6);
            charts[7].Series["Series1"].Points.AddXY("12:02:16", 22.9);
            charts[7].Series["Series1"].Points.AddXY("12:02:17", 23);

        

            charts[7].Series["Series2"].Points.AddXY("12:02:10", 22.3);
            charts[7].Series["Series2"].Points.AddXY("12:02:11", 22);
            charts[7].Series["Series2"].Points.AddXY("12:02:12", 23.5);
            charts[7].Series["Series2"].Points.AddXY("12:02:14", 24.3);
            charts[7].Series["Series2"].Points.AddXY("12:02:15", 24.3);
            charts[7].Series["Series2"].Points.AddXY("12:02:16", 24.9);
            charts[7].Series["Series2"].Points.AddXY("12:02:17", 23.6);
        }
    }
}
