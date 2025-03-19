using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MERGEN_KAT1_GCSS
{
    public partial class _3DSimulation
    {
        private GLControl glControl1;
        private Timer simulationTimer;
        private Timer randomRotationTimer;
        public Label label18 { get; set; }
        public Label label20 { get; set; }
        public Label label21 { get; set; }
        private bool useAlternativeModel = false; // Alternatif model durumu

        private float x = 0, y = 0, z = 0;
        private bool rotateX = false, rotateY = false, rotateZ = false;
        private Random rnd = new Random();

        public _3DSimulation(GLControl glControl, Button switchModelButton = null)
        {
            this.glControl1 = glControl ?? throw new ArgumentNullException(nameof(glControl));
            this.glControl1.Paint += GlControl1_Paint;
            this.glControl1.Load += GlControl1_Load;

            // Simülasyon ve rastgele dönüş timer'ları ayarlanıyor.
            simulationTimer = new Timer { Interval = 100 };
            simulationTimer.Tick += SimulationTimer_Tick;

            randomRotationTimer = new Timer { Interval = 1000 };
            randomRotationTimer.Tick += RandomRotationTimer_Tick;

            if (switchModelButton != null)
            {
                switchModelButton.Click += SwitchModelButton_Click;
            }
        }

        private void SwitchModelButton_Click(object sender, EventArgs e)
        {
            SwitchModel();
        }

        public void StartSimulationTimer(int interval)
        {
            simulationTimer.Interval = interval;
            simulationTimer.Start();
        }

        public void StopSimulationTimer() => simulationTimer.Stop();

        public void StartRandomRotation() => randomRotationTimer.Start();

        public void StopRandomRotation() => randomRotationTimer.Stop();

        public void UpdateRotation(float yaw, float pitch, float roll)
        {
            // Not: Parametre isimleri (yaw, pitch, roll) ile uygulamada kullanılan açılar (x, y, z) arasında mantıksal uyum sağlanmalıdır.
            x = pitch;
            y = roll;
            z = yaw;
            glControl1?.Invalidate();
        }

        private void SimulationTimer_Tick(object sender, EventArgs e)
        {
            // Belirlenen eksenlerde sürekli dönüş sağlanıyor.
            if (label18 != null && rotateX)
            {
                x = (x < 360) ? x + 5 : 0;
                label18.Text = x.ToString();
            }
            if (label20 != null && rotateY)
            {
                y = (y < 360) ? y + 5 : 0;
                label20.Text = y.ToString();
            }
            if (label21 != null && rotateZ)
            {
                z = (z < 360) ? z + 5 : 0;
                label21.Text = z.ToString();
            }
            glControl1?.Invalidate();
        }

        private void RandomRotationTimer_Tick(object sender, EventArgs e)
        {
            // Rastgele açılar üretilip label'lara yazdırılıyor.
            x = rnd.Next(0, 360);
            y = rnd.Next(0, 360);
            z = rnd.Next(0, 360);

            // UI thread'de label güncellemesi için Invoke kullanılıyor.
            label18?.Invoke((MethodInvoker)(() => label18.Text = x.ToString()));
            label20?.Invoke((MethodInvoker)(() => label20.Text = y.ToString()));
            label21?.Invoke((MethodInvoker)(() => label21.Text = z.ToString()));

            glControl1?.Invalidate();
        }

        public void SwitchModel()
        {
            useAlternativeModel = !useAlternativeModel;
            glControl1?.Invalidate(); // Ekranı güncelle
        }

        private void GlControl1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(Color.FromArgb(24, 30, 54)); // Arka plan rengi
            GL.Enable(EnableCap.DepthTest);
        }

        private void GlControl1_Paint(object sender, PaintEventArgs e)
        {
            // Önce buffer temizleniyor.
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Perspektif ve kamera ayarları.
            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(1.04f, (float)glControl1.Width / glControl1.Height, 1, 10000);
            Matrix4 lookAt = Matrix4.LookAt(25, 0, 0, 0, 0, 0, 0, 1, 0);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.LoadMatrix(ref perspective);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref lookAt);

            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            // Model rotasyonları uygulanıyor.
            GL.Rotate(x, 1.0, 0.0, 0.0);
            GL.Rotate(z, 0.0, 1.0, 0.0);
            GL.Rotate(y, 0.0, 0.0, 1.0);

            // Modelin çizimi: alternatif model seçimine göre.
            if (useAlternativeModel)
                DrawNewSatellite();
            else
                DrawPerforatedShell(2.3f, 12.0f, 16);

            glControl1.SwapBuffers();
        }

        // Alternatif model: yeni uydu/silindir modeli.
        // Ölçeklendirme kaldırıldı ki silindirin yüksekliği, perforasyonlu kılıfın yüksekliği ile aynı olsun.
        private void DrawNewSatellite()
        {
            DrawCylinder(3.0f, 12.0f, 16);
        }

        // Perforasyonlu kılıf modeli
        public void DrawPerforatedShell(float radius, float height, int slices)
        {
            float halfHeight = height / 2.0f;
            float shellThickness = 0.1f * radius; // Kılıf kalınlığı
            float holeSpacing = (float)(2.0 * Math.PI / slices); // Deliklerin açılacağı aralık

            // Kılıfın dış yüzeyi (delikli)
            GL.Begin(PrimitiveType.QuadStrip);
            GL.Color3(1.0f, 0.5f, 0.0f); // Turuncu
            for (int i = 0; i <= slices; i++)
            {
                float angle = i * 2.0f * (float)Math.PI / slices;
                float dx = (float)Math.Cos(angle) * (radius + shellThickness);
                float dz = (float)Math.Sin(angle) * (radius + shellThickness);
                GL.Normal3(dx, 0, dz);
                GL.Vertex3(dx, -halfHeight, dz);
                GL.Vertex3(dx, halfHeight, dz);
            }
            GL.End();

            // Yan yüzeylere daha büyük ve daha fazla sayıda yuvarlak siyah delikler ekleme (iç ve dış yüzey)
            float holeRadius = 0.08f * radius; // Daha büyük delikler
            int holeRows = 5; // Daha fazla sıra
            GL.Color3(0.0f, 0.0f, 0.0f); // Siyah delikler
            for (int i = 0; i < slices; i++)
            {
                for (int j = 1; j <= holeRows; j++) // 5 sıra delik
                {
                    float angle = i * 2.0f * (float)Math.PI / slices;
                    float dx = (float)Math.Cos(angle) * (radius + shellThickness * 0.5f);
                    float dz = (float)Math.Sin(angle) * (radius + shellThickness * 0.5f);
                    float holeY = -halfHeight + j * (height / (holeRows + 1)); // Daha sık aralıklarla delikler
                    DrawCircle(dx, holeY, dz, holeRadius, 16); // İç yüzey delikleri
                    DrawCircle(dx * 1.1f, holeY, dz * 1.1f, holeRadius, 16); // Dış yüzey delikleri
                }
            }

            // Kılıfın üst kenar yüzeyi
            GL.Begin(PrimitiveType.QuadStrip);
            GL.Color3(1.0f, 0.5f, 0.0f);
            for (int i = 0; i <= slices; i++)
            {
                float angle = i * 2.0f * (float)Math.PI / slices;
                float dx = (float)Math.Cos(angle) * (radius + shellThickness);
                float dz = (float)Math.Sin(angle) * (radius + shellThickness);
                GL.Normal3(dx, 1, dz);
                GL.Vertex3(dx, halfHeight, dz);
                GL.Vertex3(dx, halfHeight + shellThickness / 2, dz);
            }
            GL.End();

            // Kılıfın alt kenar yüzeyi
            GL.Begin(PrimitiveType.QuadStrip);
            GL.Color3(1.0f, 0.5f, 0.0f);
            for (int i = 0; i <= slices; i++)
            {
                float angle = i * 2.0f * (float)Math.PI / slices;
                float dx = (float)Math.Cos(angle) * (radius + shellThickness);
                float dz = (float)Math.Sin(angle) * (radius + shellThickness);
                GL.Normal3(dx, -1, dz);
                GL.Vertex3(dx, -halfHeight, dz);
                GL.Vertex3(dx, -halfHeight - shellThickness / 2, dz);
            }
            GL.End();
        }

        public void DrawCircle(float x, float y, float z, float radius, int segments)
        {
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Vertex3(x, y, z);
            for (int i = 0; i <= segments; i++)
            {
                float angle = i * 2.0f * (float)Math.PI / segments;
                float dx = (float)Math.Cos(angle) * radius;
                float dz = (float)Math.Sin(angle) * radius;
                GL.Vertex3(x + dx, y, z + dz);
            }
            GL.End();
        }




        // Silindir çizim metodu: Üst ve alt diskler hacimli olarak çiziliyor, ortadaki levha turuncu, kolonlar beyaz.
        public void DrawCylinder(float radius, float height, int slices)
        {
            float halfHeight = height / 2.0f;
            float discThickness = 0.03f * height;
            float columnInset = 0.15f * radius;

            // --- ALT DİSK (TABAN) HACMİ ---
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Color3(1.0f, 0.65f, 0.0f); // Turuncu
            GL.Normal3(0, -1, 0);
            GL.Vertex3(0, -halfHeight, 0);
            for (int i = 0; i <= slices; i++)
            {
                float angle = i * 2.0f * (float)Math.PI / slices;
                float dx = (float)Math.Cos(angle) * radius;
                float dz = (float)Math.Sin(angle) * radius;
                GL.Vertex3(dx, -halfHeight, dz);
            }
            GL.End();

            GL.Begin(PrimitiveType.TriangleFan);
            GL.Color3(1.0f, 0.65f, 0.0f);
            GL.Normal3(0, 1, 0);
            GL.Vertex3(0, -halfHeight + discThickness, 0);
            for (int i = 0; i <= slices; i++)
            {
                float angle = i * 2.0f * (float)Math.PI / slices;
                float dx = (float)Math.Cos(angle) * radius;
                float dz = (float)Math.Sin(angle) * radius;
                GL.Vertex3(dx, -halfHeight + discThickness, dz);
            }
            GL.End();

            GL.Begin(PrimitiveType.QuadStrip);
            GL.Color3(1.0f, 0.65f, 0.0f);
            for (int i = 0; i <= slices; i++)
            {
                float angle = i * 2.0f * (float)Math.PI / slices;
                float dx = (float)Math.Cos(angle) * radius;
                float dz = (float)Math.Sin(angle) * radius;
                GL.Normal3(dx, 0, dz);
                GL.Vertex3(dx, -halfHeight, dz);
                GL.Vertex3(dx, -halfHeight + discThickness, dz);
            }
            GL.End();

            // --- ÜST DİSK (TAVAN) HACMİ ---
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Color3(0.5f, 0.0f, 0.5f);
            GL.Normal3(0, -1, 0);
            GL.Vertex3(0, halfHeight - discThickness, 0);
            for (int i = 0; i <= slices; i++)
            {
                float angle = i * 2.0f * (float)Math.PI / slices;
                float dx = (float)Math.Cos(angle) * radius;
                float dz = (float)Math.Sin(angle) * radius;
                GL.Vertex3(dx, halfHeight - discThickness, dz);
            }
            GL.End();

            GL.Begin(PrimitiveType.TriangleFan);
            GL.Color3(0.5f, 0.0f, 0.5f);
            GL.Normal3(0, 1, 0);
            GL.Vertex3(0, halfHeight, 0);
            for (int i = 0; i <= slices; i++)
            {
                float angle = i * 2.0f * (float)Math.PI / slices;
                float dx = (float)Math.Cos(angle) * radius;
                float dz = (float)Math.Sin(angle) * radius;
                GL.Vertex3(dx, halfHeight, dz);
            }
            GL.End();

            GL.Begin(PrimitiveType.QuadStrip);
            GL.Color3(0.5f, 0.0f, 0.5f);
            for (int i = 0; i <= slices; i++)
            {
                float angle = i * 2.0f * (float)Math.PI / slices;
                float dx = (float)Math.Cos(angle) * radius;
                float dz = (float)Math.Sin(angle) * radius;
                GL.Normal3(dx, 0, dz);
                GL.Vertex3(dx, halfHeight - discThickness, dz);
                GL.Vertex3(dx, halfHeight, dz);
            }
            GL.End();

            // --- KOLONLAR (Beyaz) ---
            float columnRadius = 0.1f * radius;
            GL.Color3(1.0f, 1.0f, 1.0f);
            for (int i = 0; i < 4; i++)
            {
                float angle = i * (float)Math.PI / 2;
                float dx = (float)Math.Cos(angle) * (radius - columnInset);
                float dz = (float)Math.Sin(angle) * (radius - columnInset);
                DrawColumn(dx, dz, columnRadius, height);
            }
        }

        // Kolon çizim metodu (beyaz renkte)
        public void DrawColumn(float x, float z, float columnRadius, float height)
        {
            GL.Color3(1.0f, 1.0f, 1.0f); // Beyaz
            float halfHeight = height / 2.0f;
            int slices = 16; // Kolon için dilim sayısı

            // Kolon yan yüzeyi
            GL.Begin(PrimitiveType.QuadStrip);
            for (int i = 0; i <= slices; i++)
            {
                float angle = i * 2.0f * (float)Math.PI / slices;
                float dx = (float)Math.Cos(angle) * columnRadius;
                float dz = (float)Math.Sin(angle) * columnRadius;
                GL.Normal3(dx, 0, dz);
                GL.Vertex3(x + dx, -halfHeight, z + dz);
                GL.Vertex3(x + dx, halfHeight, z + dz);
            }
            GL.End();

            // Kolon alt diski
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Normal3(0, -1, 0);
            GL.Vertex3(x, -halfHeight, z);
            for (int i = 0; i <= slices; i++)
            {
                float angle = i * 2.0f * (float)Math.PI / slices;
                float dx = (float)Math.Cos(angle) * columnRadius;
                float dz = (float)Math.Sin(angle) * columnRadius;
                GL.Vertex3(x + dx, -halfHeight, z + dz);
            }
            GL.End();

            // Kolon üst diski
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Normal3(0, 1, 0);
            GL.Vertex3(x, halfHeight, z);
            for (int i = 0; i <= slices; i++)
            {
                float angle = i * 2.0f * (float)Math.PI / slices;
                float dx = (float)Math.Cos(angle) * columnRadius;
                float dz = (float)Math.Sin(angle) * columnRadius;
                GL.Vertex3(x + dx, halfHeight, z + dz);
            }
            GL.End();
        }
    }
}
