using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MERGEN_KAT1_GCSS;

public class Charts
{
    private Chart chart1, chart2, chart3, chart4, chart5, chart6, chart7, chart8;

    public Charts(Chart ch1, Chart ch2, Chart ch3, Chart ch4, Chart ch5, Chart ch6, Chart ch7, Chart ch8)
    {
        chart1 = ch1;
        chart2 = ch2;
        chart3 = ch3;
        chart4 = ch4;
        chart5 = ch5;
        chart6 = ch6;
        chart7 = ch7;
        chart8 = ch8;
    }

    public void Update(TelemetryData data)
    {
        string raw = data.GondermeSaati;
        string timePart = raw.Contains(",") ? raw.Split(',')[1] : raw;
        string timeLabel = timePart.Replace('/', ':');

        chart1.Series["Series1"].Points.AddXY(timeLabel, data.Basinc1);
        chart1.Series["Series2"].Points.AddXY(timeLabel, data.Basinc2);

        chart2.Series["Series1"].Points.AddXY(timeLabel, data.Yukseklik1);

        chart3.Series["Series1"].Points.AddXY(timeLabel, data.Yukseklik2);

        chart4.Series["Series1"].Points.AddXY(timeLabel, data.IrtifaFarki);

        chart5.Series["Series1"].Points.AddXY(timeLabel, data.InisHizi);

        chart6.Series["Series1"].Points.AddXY(timeLabel, data.Sicaklik);

        chart7.Series["Series1"].Points.AddXY(timeLabel, data.PilGerilimi);

        chart8.Series["Series1"].Points.AddXY(timeLabel, data.IoTS1Data);
        chart8.Series["Series2"].Points.AddXY(timeLabel, data.IoTS2Data);
    }


}
