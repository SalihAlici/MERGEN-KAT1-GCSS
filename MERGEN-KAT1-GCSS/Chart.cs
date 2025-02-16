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
            charts[0].Series["Series1"].Points.AddXY(1, 5);
            charts[0].Series["Series1"].Points.AddXY(2, 7);
            charts[0].Series["Series1"].Points.AddXY(3, 12);
            charts[0].Series["Series1"].Points.AddXY(4, 94);
            charts[0].Series["Series1"].Points.AddXY(5, 15);
            charts[0].Series["Series1"].Points.AddXY(6, 8);

            charts[0].Series["Series2"].Points.AddXY(1, 1);
            charts[0].Series["Series2"].Points.AddXY(2, 10);
            charts[0].Series["Series2"].Points.AddXY(3, 96);
            charts[0].Series["Series2"].Points.AddXY(4, 82);
            charts[0].Series["Series2"].Points.AddXY(5, 9);
            charts[0].Series["Series2"].Points.AddXY(6, 55);
            charts[0].Series["Series2"].Points.AddXY(7, 85);

            // Chart2
            charts[1].Series["Series1"].Points.AddXY(2, 10);
            charts[1].Series["Series1"].Points.AddXY(3, 7);
            charts[1].Series["Series1"].Points.AddXY(4, 12);
            charts[1].Series["Series1"].Points.AddXY(5, 9);
            charts[1].Series["Series1"].Points.AddXY(6, 15);
            charts[1].Series["Series1"].Points.AddXY(7, 8);
            charts[1].Series["Series1"].Points.AddXY(1, 5);

            // Chart3
            charts[2].Series["Series1"].Points.AddXY(1, 5);
            charts[2].Series["Series1"].Points.AddXY(2, 10);
            charts[2].Series["Series1"].Points.AddXY(3, 7);
            charts[2].Series["Series1"].Points.AddXY(4, 12);
            charts[2].Series["Series1"].Points.AddXY(5, 9);
            charts[2].Series["Series1"].Points.AddXY(6, 15);
            charts[2].Series["Series1"].Points.AddXY(7, 8);

            // Chart4
            charts[3].Series["Series1"].Points.AddXY(1, 5);
            charts[3].Series["Series1"].Points.AddXY(2, 10);
            charts[3].Series["Series1"].Points.AddXY(3, 7);
            charts[3].Series["Series1"].Points.AddXY(4, 12);
            charts[3].Series["Series1"].Points.AddXY(5, 9);
            charts[3].Series["Series1"].Points.AddXY(6, 15);
            charts[3].Series["Series1"].Points.AddXY(7, 8);

            // Chart5
            charts[4].Series["Series1"].Points.AddXY(1, 5);
            charts[4].Series["Series1"].Points.AddXY(2, 10);
            charts[4].Series["Series1"].Points.AddXY(3, 7);
            charts[4].Series["Series1"].Points.AddXY(4, 12);
            charts[4].Series["Series1"].Points.AddXY(5, 9);
            charts[4].Series["Series1"].Points.AddXY(6, 15);
            charts[4].Series["Series1"].Points.AddXY(7, 8);

            // Chart6
            charts[5].Series["Series1"].Points.AddXY(1, 5);
            charts[5].Series["Series1"].Points.AddXY(2, 10);
            charts[5].Series["Series1"].Points.AddXY(3, 7);
            charts[5].Series["Series1"].Points.AddXY(4, 12);
            charts[5].Series["Series1"].Points.AddXY(5, 9);
            charts[5].Series["Series1"].Points.AddXY(6, 15);
            charts[5].Series["Series1"].Points.AddXY(7, 8);

            // Chart7
            charts[6].Series["Series1"].Points.AddXY(1, 5);
            charts[6].Series["Series1"].Points.AddXY(2, 10);
            charts[6].Series["Series1"].Points.AddXY(3, 7);
            charts[6].Series["Series1"].Points.AddXY(4, 12);
            charts[6].Series["Series1"].Points.AddXY(5, 9);
            charts[6].Series["Series1"].Points.AddXY(6, 15);
            charts[6].Series["Series1"].Points.AddXY(7, 8);

            // Chart8
            charts[7].Series["Series1"].Points.AddXY(1, 5);
            charts[7].Series["Series1"].Points.AddXY(2, 10);
            charts[7].Series["Series1"].Points.AddXY(3, 7);
            charts[7].Series["Series1"].Points.AddXY(4, 12);
            charts[7].Series["Series1"].Points.AddXY(5, 9);
            charts[7].Series["Series1"].Points.AddXY(6, 15);
            charts[7].Series["Series1"].Points.AddXY(7, 8);

            charts[7].Series["Series2"].Points.AddXY(1, 51);
            charts[7].Series["Series2"].Points.AddXY(2, 80);
            charts[7].Series["Series2"].Points.AddXY(3, 7);
            charts[7].Series["Series2"].Points.AddXY(4, 2);
            charts[7].Series["Series2"].Points.AddXY(5, 9);
            charts[7].Series["Series2"].Points.AddXY(6, 75);
            charts[7].Series["Series2"].Points.AddXY(7, 6);
        }
    }
}
