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
       
        
        private bool useAlternativeModel = false; // Alternatif model durumu

        public float x = 0, y = 0, z = 0;
       
        

        public _3DSimulation(GLControl glControl, Button switchModelButton = null)
        {
            this.glControl1 = glControl ?? throw new ArgumentNullException(nameof(glControl));
            

            
           

            if (switchModelButton != null)
            {
                switchModelButton.Click += SwitchModelButton_Click;
            }
        }

        private void SwitchModelButton_Click(object sender, EventArgs e)
        {
            SwitchModel();
        }

        

       

        public void UpdateRotation(float yaw, float pitch, float roll)
        {
            
            x = pitch;
            y = roll;
            z = yaw;
            

        }

        

       

        public void SwitchModel()
        {
            useAlternativeModel = !useAlternativeModel;
            glControl1?.Invalidate(); // Ekranı güncelle
        }

        

        

        // Alternatif model: yeni uydu/silindir modeli.
        // Ölçeklendirme kaldırıldı ki silindirin yüksekliği, perforasyonlu kılıfın yüksekliği ile aynı olsun.
        public void DrawNewSatellite()
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
            float columnInset = 0.15f * radius; // Kolonların merkeze olan mesafesi
            float layerHeight = height / 3.0f; // Her bir tabakanın yüksekliği
            float wallThickness = 0.1f * height; // Üst taban kalınlığı

            // Alt taban
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Color3(1.0f, 0.0f, 0.0f); // Kırmızı renk
            GL.Normal3(0, -1, 0);
            GL.Vertex3(0, -halfHeight, 0);
            for (int i = 0; i <= slices; i++)
            {
                float angle = i * 2.0f * (float)Math.PI / slices;
                float x = (float)Math.Cos(angle) * radius;
                float z = (float)Math.Sin(angle) * radius;
                GL.Vertex3(x, -halfHeight, z);
            }
            GL.End();

            // Ortadaki iki tabaka
            for (int j = 1; j <= 2; j++)
            {
                float currentHeight = -halfHeight + j * layerHeight;
                GL.Begin(PrimitiveType.TriangleFan);
                // Her tabakayı farklı renkte çiz
                switch (j)
                {
                    case 1:
                        GL.Color3(0.5f, 0.5f, 0.0f); // Sarı renk
                        break;
                    case 2:
                        GL.Color3(0.5f, 0.0f, 0.5f); // Mor renk
                        break;
                }
                GL.Normal3(0, 0, 0);
                GL.Vertex3(0, currentHeight, 0);
                for (int i = 0; i <= slices; i++)
                {
                    float angle = i * 2.0f * (float)Math.PI / slices;
                    float x = (float)Math.Cos(angle) * radius;
                    float z = (float)Math.Sin(angle) * radius;
                    GL.Vertex3(x, currentHeight, z);
                }
                GL.End();
            }

            // Üst tabanın duvarları (aynı genişlikte iniyor)
            GL.Begin(PrimitiveType.QuadStrip);
            GL.Color3(0.0f, 0.5f, 1.0f); // Açık mavi renk
            for (int i = 0; i <= slices; i++)
            {
                float angle = i * 2.0f * (float)Math.PI / slices;
                float xOuter = (float)Math.Cos(angle) * radius;
                float zOuter = (float)Math.Sin(angle) * radius;
                float xInner = xOuter;
                float zInner = zOuter;

                GL.Normal3(xOuter, 0, zOuter);
                GL.Vertex3(xOuter, halfHeight, zOuter);
                GL.Vertex3(xInner, halfHeight - wallThickness, zInner);
            }
            GL.End();

            // Üst tabanın alt kısmındaki tabaka
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Color3(0.0f, 0.0f, 1.0f); // Mavi renk
            GL.Normal3(0, -1, 0);
            GL.Vertex3(0, halfHeight - wallThickness, 0);
            for (int i = 0; i <= slices; i++)
            {
                float angle = i * 2.0f * (float)Math.PI / slices;
                float x = (float)Math.Cos(angle) * radius;
                float z = (float)Math.Sin(angle) * radius;
                GL.Vertex3(x, halfHeight - wallThickness, z);
            }
            GL.End();

            // 4 Kolon
            GL.Color3(0.0f, 1.0f, 0.0f); // Yeşil renk
            float columnRadius = 0.1f * radius; // Kolon yarıçapı

            for (int i = 0; i < 4; i++)
            {
                float angle = i * (float)Math.PI / 2;
                float x = (float)Math.Cos(angle) * (radius - columnInset);
                float z = (float)Math.Sin(angle) * (radius - columnInset);

                DrawColumn(x, z, columnRadius, height);
            }
        }

        public void DrawColumn(float x, float z, float columnRadius, float height)
        {
            float halfHeight = height / 2.0f;
            int slices = 16; // Kolon için dilim sayısı

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

            // Kolon alt tabanı
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

            // Kolon üst tabanı
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
