using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;


namespace MERGEN_KAT1_GCSS
{
    public class _3DSim
    {
        private GLControl glControl;
        private float yaw = 0, pitch = 0, roll = 0;

        public _3DSim(GLControl control)
        {
            glControl = control;
            glControl.Paint += GlControl_Paint;
            glControl.Resize += GlControl_Resize;
        }

        private void GlControl_Paint(object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.LoadIdentity();
            GL.Translate(0.0f, 0.0f, -5.0f); // Kamera ayarı

            // Yaw, pitch, roll değerlerine göre dönüşüm
            GL.Rotate(yaw, 0.0f, 1.0f, 0.0f); // Yaw
            GL.Rotate(pitch, 1.0f, 0.0f, 0.0f); // Pitch
            GL.Rotate(roll, 0.0f, 0.0f, 1.0f); // Roll

            // Koni ve silindiri çiz
            DrawCylinderWithCone();

            glControl.SwapBuffers(); // OpenGL buffer'ını ekranla değiştir
        }

        private void GlControl_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)glControl.Width / glControl.Height, 0.1f, 100f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        private void DrawCylinderWithCone()
        {
            // Silindir çizimi
            GL.Color3(1.0f, 0.0f, 0.0f); // Kırmızı renk
            GL.PushMatrix();
            GL.Rotate(90, 1.0f, 0.0f, 0.0f); // Silindirin açısını düzelt
            DrawCylinder(1.0f, 3.0f);
            GL.PopMatrix();

            // Koni çizimi
            GL.Color3(0.0f, 1.0f, 0.0f); // Yeşil renk
            DrawCone(1.0f, 2.0f);
        }

        private void DrawCylinder(float radius, float height)
        {
            GL.Begin(PrimitiveType.QuadStrip);
            for (float angle = 0; angle <= (float)(Math.PI * 2); angle += (float)(Math.PI / 30))
            {
                float x = radius * (float)Math.Cos(angle);
                float z = radius * (float)Math.Sin(angle);
                GL.Vertex3(x, -height / 2, z);
                GL.Vertex3(x, height / 2, z);
            }
            GL.End();
        }

        private void DrawCone(float radius, float height)
        {
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Vertex3(0.0f, height / 2, 0.0f); // Koninin tepesi
            for (float angle = 0f; angle <= (float)(Math.PI * 2); angle += (float)(Math.PI / 30))
            {
                float x = radius * (float)Math.Cos(angle);
                float z = radius * (float)Math.Sin(angle);
                GL.Vertex3(x, -height / 2, z);
            }
            GL.End();
        }

        public void UpdateRotation(float yawValue, float pitchValue, float rollValue)
        {
            // Yaw, pitch ve roll verilerini güncelle
            yaw = yawValue;
            pitch = pitchValue;
            roll = rollValue;

            // OpenGL render'ını yeniden başlatıyoruz
            glControl.Invalidate();
        }
    }
}
