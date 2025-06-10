using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MERGEN_KAT1_GCSS
{
    public partial class _3DSimulation
    {



   
        public bool useAlternativeModel = false; // Alternatif model durumu

        public float x, y, z;

        



        public void UpdateRotation(float yaw, float pitch, float roll)
        {
            // Not: Parametre isimleri (yaw, pitch, roll) ile uygulamada kullanılan açılar (x, y, z) arasında mantıksal uyum sağlanmalıdır.
            x = pitch;
            y = roll;
            z = yaw;
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
            float discThickness = 0.03f * height;
            float columnInset = 0.15f * radius;
            // --- İÇ DİKDÖRTGEN LEVHA (Turuncu) ---
            GL.Color3(1.0f, 0.5f, 0.0f); // Turuncu
            float plateWidth = radius * 1.2f;  // Daha geniş levha (eski olsaydı mesela 1.4f olurdu)
            float plateThickness = 0.05f * height; // Levha kalınlığı

            GL.Begin(PrimitiveType.Quads);
            // Alt yüzey
            GL.Normal3(0, 1, 0);
            GL.Vertex3(-plateWidth / 2, -halfHeight + discThickness, -plateThickness / 2);
            GL.Vertex3(plateWidth / 2, -halfHeight + discThickness, -plateThickness / 2);
            GL.Vertex3(plateWidth / 2, halfHeight - discThickness, -plateThickness / 2);
            GL.Vertex3(-plateWidth / 2, halfHeight - discThickness, -plateThickness / 2);

            // Üst yüzey
            GL.Normal3(0, -1, 0);
            GL.Vertex3(-plateWidth / 2, -halfHeight + discThickness, plateThickness / 2);
            GL.Vertex3(plateWidth / 2, -halfHeight + discThickness, plateThickness / 2);
            GL.Vertex3(plateWidth / 2, halfHeight - discThickness, plateThickness / 2);
            GL.Vertex3(-plateWidth / 2, halfHeight - discThickness, plateThickness / 2);

            // Sağ kenar
            GL.Normal3(1, 0, 0);
            GL.Vertex3(plateWidth / 2, -halfHeight + discThickness, -plateThickness / 2);
            GL.Vertex3(plateWidth / 2, -halfHeight + discThickness, plateThickness / 2);
            GL.Vertex3(plateWidth / 2, halfHeight - discThickness, plateThickness / 2);
            GL.Vertex3(plateWidth / 2, halfHeight - discThickness, -plateThickness / 2);

            // Sol kenar
            GL.Normal3(-1, 0, 0);
            GL.Vertex3(-plateWidth / 2, -halfHeight + discThickness, -plateThickness / 2);
            GL.Vertex3(-plateWidth / 2, -halfHeight + discThickness, plateThickness / 2);
            GL.Vertex3(-plateWidth / 2, halfHeight - discThickness, plateThickness / 2);
            GL.Vertex3(-plateWidth / 2, halfHeight - discThickness, -plateThickness / 2);

            // Ön yüzey
            GL.Normal3(0, 0, -1);
            GL.Vertex3(-plateWidth / 2, -halfHeight + discThickness, -plateThickness / 2);
            GL.Vertex3(-plateWidth / 2, halfHeight - discThickness, -plateThickness / 2);
            GL.Vertex3(plateWidth / 2, halfHeight - discThickness, -plateThickness / 2);
            GL.Vertex3(plateWidth / 2, -halfHeight + discThickness, -plateThickness / 2);

            // Arka yüzey
            GL.Normal3(0, 0, 1);
            GL.Vertex3(-plateWidth / 2, -halfHeight + discThickness, plateThickness / 2);
            GL.Vertex3(-plateWidth / 2, halfHeight - discThickness, plateThickness / 2);
            GL.Vertex3(plateWidth / 2, halfHeight - discThickness, plateThickness / 2);
            GL.Vertex3(plateWidth / 2, -halfHeight + discThickness, plateThickness / 2);
            GL.End();


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
