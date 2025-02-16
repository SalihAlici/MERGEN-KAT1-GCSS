using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

[ToolboxItem(true)]  // Toolbox'ta görünmesini sağlar
public class BatteryProgressBar : Control
{
    private int _percentage = 0; // Başlangıçta %0
    private Timer batteryTimer; // Timer değişkeni

    [Category("Battery"), Description("Pil doluluk yüzdesi")]
    public int Percentage
    {
        get { return _percentage; }
        set
        {
            _percentage = Math.Max(0, Math.Min(100, value)); // 0-100 arasında sınırla
            Invalidate(); // Yeniden çiz
        }
    }

    public BatteryProgressBar()
    {
        DoubleBuffered = true;

        // Timer'ı başlat
        batteryTimer = new Timer();
        batteryTimer.Interval = 1000; // 1000 ms = 1 saniye
        batteryTimer.Tick += BatteryTimer_Tick;
        batteryTimer.Start(); // Timer'ı başlat
    }

    private void BatteryTimer_Tick(object sender, EventArgs e)
    {
        // Pil yüzdesini 5 artır
        if (Percentage < 100)
        {
            Percentage += 5;
        }
        else
        {
            batteryTimer.Stop(); // Yüzde 100 olunca timer'ı durdur
        }
    }

    // 5 tonlu renkleri hesaplayan metot
    private Color GetFillColor(int percentage)
    {
        Color fillColor;

        // Pil yüzdesine göre renk belirle
        if (percentage > 50)
        {
            fillColor = Color.Green; // %50 ve üzeri için yeşil
        }
        else if (percentage > 20)
        {
            fillColor = Color.Orange; // %20 ile %50 arasında turuncu
        }
        else
        {
            fillColor = Color.Red; // %20'nin altı için kırmızı
        }

        return fillColor;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        // Arka planı beyaz yap
        g.FillRectangle(Brushes.White, 0, 0, Width, Height);

        // Dolum alanını hesapla
        int fillWidth = (int)((Percentage / 100f) * (Width - 10));
        Rectangle fillRect = new Rectangle(5, 5, fillWidth, Height - 10);

        // Pil dolum rengini hesapla
        Color fillColor = GetFillColor(Percentage);

        // Pil dolum rengini çiz
        using (SolidBrush brush = new SolidBrush(fillColor))
        {
            g.FillRectangle(brush, fillRect);
        }

        // Şarj yüzdesini yazdır
        using (Font font = new Font("Arial", 12, FontStyle.Bold))
        using (SolidBrush textBrush = new SolidBrush(Color.Black))
        {
            string text = $"{Percentage}%";
            SizeF textSize = g.MeasureString(text, font);
            g.DrawString(text, font, textBrush, (Width - textSize.Width) / 2, (Height - textSize.Height) / 2);
        }
    }
}
