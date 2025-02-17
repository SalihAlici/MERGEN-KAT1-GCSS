using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using System;

public class BatteryProgressBar : Control
{
    private int _percentage;
    private int[] batteryLevels;  // Pil yüzdesini tutan dizi
    private int currentIndex = 0; // Dizideki sıralama için bir sayaç
    private Timer batteryUpdateTimer;

    public int Percentage
    {
        get { return _percentage; }
        set
        {
            if (_percentage != value)
            {
                _percentage = Math.Max(0, Math.Min(100, value));
                Invalidate();  // Ekranı yeniden çiz
            }
        }
    }

    public BatteryProgressBar()
    {
        DoubleBuffered = true;

        // Pil yüzdesini tutan diziyi oluşturuyoruz ve rastgele 0-100 arasında değerler atıyoruz
        Random rand = new Random();
        batteryLevels = new int[20];
        for (int i = 0; i < batteryLevels.Length; i++)
        {
            batteryLevels[i] = rand.Next(0, 101);  // 0 ile 100 arasında rastgele sayılar
        }

        // Timer'ı başlatıyoruz
        batteryUpdateTimer = new Timer();
        batteryUpdateTimer.Interval = 1000; // 1 saniyede bir güncelleme (1000 ms)
        batteryUpdateTimer.Tick += BatteryUpdateTimer_Tick;
        batteryUpdateTimer.Start(); // Timer'ı başlatıyoruz
    }

    private void BatteryUpdateTimer_Tick(object sender, EventArgs e)
    {
        // Dizinin şu anki indeksindeki pil yüzdesini alıyoruz
        int batteryPercentage = batteryLevels[currentIndex];

        // Pil yüzdesini güncelle
        Percentage = batteryPercentage;

        // Dizinin bir sonraki elemanına geçiyoruz, eğer son elemana geldiysek başa dönüyoruz
        currentIndex = (currentIndex + 1) % batteryLevels.Length;
    }

    // Pil dolum rengini belirleyen metot
    private Color GetFillColor(int percentage)
    {
        if (percentage > 50) return Color.Lime;
        else if (percentage > 20) return Color.Orange;
        else return Color.Red;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        // Arka planı beyaz yap
        g.FillRectangle(Brushes.White, 0, 0, Width, Height);

        // Dolum alanını hesapla
        int fillWidth = (int)((_percentage / 100f) * (Width - 10)); // % yüzdesine göre dolum genişliğini hesapla
        Rectangle fillRect = new Rectangle(5, 5, fillWidth, Height - 10); // Dolum dikdörtgeni

        // Dolum rengini uygula
        using (SolidBrush brush = new SolidBrush(GetFillColor(_percentage)))
        {
            g.FillRectangle(brush, fillRect);
        }
        // Çerçeve çiz
        using (Pen pen = new Pen(Color.White, 1))
        {
            g.DrawRectangle(pen, 5, 5, Width - 10, Height - 10); // Çerçeve çiz
        }




        // Yüzdelik metni ekle
        using (Font font = new Font("Arial", 12, FontStyle.Bold))
        using (SolidBrush textBrush = new SolidBrush(Color.Black))
        {
            string text = $"{_percentage}%";
            SizeF textSize = g.MeasureString(text, font); // Metin boyutunu hesapla
            g.DrawString(text, font, textBrush, (Width - textSize.Width) / 2, (Height - textSize.Height) / 2); // Metni ortala
        }
    }
}
