using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MERGEN_KAT1_GCSS
{
    public class QuarterCircularProgress : Control
    {
        private int angle;
        private string valueLabel;
        private Label centerLabel; // Ortadaki rastgele harfleri gösterecek label
        private static readonly Random random = new Random();
        private Timer updateTimer;

        public string ValueLabel
        {
            get { return valueLabel; }
            set
            {
                valueLabel = value;
                Invalidate();
            }
        }

        public string CenterText
        {
            get { return centerLabel.Text; }
            set { centerLabel.Text = value; }
        }

        public int Angle
        {
            get { return angle; }
            set
            {
                // Açı 0-90 arasında sınırlı
                angle = Math.Max(0, Math.Min(90, value));
                Invalidate();
            }
        }

        public QuarterCircularProgress()
        {
            this.Size = new Size(360, 360);
            this.Padding = new Padding(20);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);

            // Label oluşturuluyor
            centerLabel = new Label();
            centerLabel.TextAlign = ContentAlignment.MiddleCenter;
            centerLabel.BackColor = Color.Transparent;
            centerLabel.Font = new Font("Arial", 20, FontStyle.Bold);
            centerLabel.AutoSize = false;
            centerLabel.ForeColor = Color.White;
            this.Controls.Add(centerLabel);

            // Başlangıç değerleri (rastgele)
            Angle = random.Next(0, 91);
            ValueLabel = $"{Angle}°";
            CenterText = GenerateRandomString(3);

            // Timer: Her saniyede bir güncelleme
            updateTimer = new Timer();
            updateTimer.Interval = 1000;
            updateTimer.Tick += (s, e) =>
            {
                Angle = random.Next(0, 91);
                ValueLabel = $"{Angle}°";
                CenterText = GenerateRandomString(3);
            };
            updateTimer.Start();
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            char[] stringChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            return new string(stringChars);
        }

        // Çeyrek dairenin hemen altına label'ı yerleştiren metot 
        // (sağ ucundan 40 piksel sola kaydırılmış, 2 piksel offset)
        private void UpdateLabelPosition(Rectangle arcRect)
        {
            int labelWidth = arcRect.Width / 3;
            int labelHeight = arcRect.Height / 6;
            int rightX = arcRect.Right;
            int rightY = arcRect.Y + arcRect.Height / 2; // Sağ uç, dikey olarak çemberin ortasında
            int labelX = rightX - (labelWidth / 2) - 75;
            int labelY = rightY + 2;
            centerLabel.Bounds = new Rectangle(labelX, labelY, labelWidth, labelHeight);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int padding = 20;
            int diameter = Width - padding * 2;
            int radius = diameter / 2;
            int centerX = padding + radius;
            int centerY = padding + radius;

            // Çeyrek çemberin boyutları (yarıçapı %65 oranında hesaplanıyor)
            int quarterCircleDiameter = (int)(diameter * 0.65);
            int quarterCircleRadius = quarterCircleDiameter / 2;
            Rectangle arcRect = new Rectangle(
                padding + (diameter - quarterCircleDiameter) / 2,
                padding + (diameter - quarterCircleDiameter) / 2,
                quarterCircleDiameter,
                quarterCircleDiameter);

            // Radial gradient doldurması yalnızca çeyrek daire alanına uygulanacak:
            using (GraphicsPath quarterPath = new GraphicsPath())
            {
                // Arc: 270°'den 90° (yani 270°'den 360°'e) çizgi
                quarterPath.AddArc(arcRect, 270, 90);
                // Arc bitiş noktasından merkeze çizgi (sağ orta)
                quarterPath.AddLine(arcRect.Right, arcRect.Y + arcRect.Height / 2, centerX, centerY);
                // Merkezden arc başlangıç noktasına çizgi (üst orta)
                quarterPath.AddLine(centerX, centerY, arcRect.X + arcRect.Width / 2, arcRect.Y);
                quarterPath.CloseFigure();

                using (PathGradientBrush brush = new PathGradientBrush(quarterPath))
                {
                    brush.CenterColor = Color.LightBlue;
                    brush.SurroundColors = new Color[] { Color.White };
                    g.FillPath(brush, quarterPath);
                }
            }

            // Label konumunu güncelle
            UpdateLabelPosition(arcRect);

            // Çeyrek daireyi çiz (DarkBlue, 3 piksel kalınlık)
            using (Pen arcPen = new Pen(Color.White, 3))
            {
                g.DrawArc(arcPen, arcRect, 270, 90);
                g.DrawLine(arcPen, centerX, centerY, centerX + quarterCircleRadius, centerY);
                g.DrawLine(arcPen, centerX, centerY, centerX, centerY - quarterCircleRadius);
            }

            // 30° aralıklarla yarıçap çizgilerini ve açı etiketlerini çiz
            for (int i = 0; i <= 90; i += 30)
            {
                double rad = (-i) * Math.PI / 180;
                int endX = centerX + (int)(quarterCircleRadius * Math.Cos(rad));
                int endY = centerY + (int)(quarterCircleRadius * Math.Sin(rad));
                // İnce gri çizgi
                using (Pen pen = new Pen(Color.Gray, 1))
                {
                    g.DrawLine(pen, centerX, centerY, endX, endY);
                }
                // Etiketleri çemberin dışına taşımak için offset ekle (15 piksel)
                int offset = 15;
                int textX = centerX + (int)((quarterCircleRadius + offset) * Math.Cos(rad)) - 5;
                int textY = centerY + (int)((quarterCircleRadius + offset) * Math.Sin(rad)) - 5;
                using (Font smallFont = new Font("Arial", 10, FontStyle.Regular))
                using (SolidBrush brush = new SolidBrush(Color.Gray))
                {
                    g.DrawString($"{i}°", smallFont, brush, textX, textY);
                }
            }

            // Güncel açıyı (timer'dan gelen açı) çizelim:
            double currentRad = (-angle) * Math.PI / 180;
            int currentEndX = centerX + (int)(quarterCircleRadius * Math.Cos(currentRad));
            int currentEndY = centerY + (int)(quarterCircleRadius * Math.Sin(currentRad));
            using (Pen currentPen = new Pen(Color.Red, 2))
            {
                g.DrawLine(currentPen, centerX, centerY, currentEndX, currentEndY);
            }
            // Güncel açının metnini daha dışarıda yazmak için ekstra offset (örneğin 37 piksel) ekleyelim
            int extraOffset = 37;
            string currentAngleText = $"{angle}°";
            using (Font largeFont = new Font("Arial", 16, FontStyle.Bold))
            using (SolidBrush redBrush = new SolidBrush(Color.DarkOrange))
            {
                SizeF textSize = g.MeasureString(currentAngleText, largeFont);
                float currentTextX = centerX + (float)((quarterCircleRadius + extraOffset) * Math.Cos(currentRad)) - textSize.Width / 2;
                float currentTextY = centerY + (float)((quarterCircleRadius + extraOffset) * Math.Sin(currentRad)) - textSize.Height / 2;
                g.DrawString(currentAngleText, largeFont, redBrush, currentTextX, currentTextY);
            }
        }
    }
}
