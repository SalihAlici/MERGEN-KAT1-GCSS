using System;
using System.Drawing;
using System.Drawing.Imaging;
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

        private float x = 0, y = 0, z = 0;
        private bool rotateX = false, rotateY = false, rotateZ = false;
        private Random rnd = new Random();
        private int[] textureIds = new int[6];

        public _3DSimulation(GLControl glControl)
        {
            this.glControl1 = glControl ?? throw new ArgumentNullException(nameof(glControl));
            this.glControl1.Paint += GlControl1_Paint;
            this.glControl1.Load += GlControl1_Load;

            simulationTimer = new Timer { Interval = 100 };
            simulationTimer.Tick += SimulationTimer_Tick;

            randomRotationTimer = new Timer { Interval = 1000 };
            randomRotationTimer.Tick += RandomRotationTimer_Tick;
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
            x = pitch;
            y = roll;
            z = yaw;
            glControl1?.Invalidate();
        }

        private void SimulationTimer_Tick(object sender, EventArgs e)
        {
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
            x = rnd.Next(0, 360);
            y = rnd.Next(0, 360);
            z = rnd.Next(0, 360);

            label18?.Invoke((MethodInvoker)(() => label18.Text = x.ToString()));
            label20?.Invoke((MethodInvoker)(() => label20.Text = y.ToString()));
            label21?.Invoke((MethodInvoker)(() => label21.Text = z.ToString()));

            glControl1?.Invalidate();
        }

        private void GlControl1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(Color.FromArgb(24, 30, 54));
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.ColorMaterial);
            GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.AmbientAndDiffuse);

            float[] lightPosition = { 10.0f, 10.0f, 10.0f, 1.0f };
            GL.Light(LightName.Light0, LightParameter.Position, lightPosition);

            float[] lightAmbient = { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] lightDiffuse = { 0.8f, 0.8f, 0.8f, 1.0f };
            GL.Light(LightName.Light0, LightParameter.Ambient, lightAmbient);
            GL.Light(LightName.Light0, LightParameter.Diffuse, lightDiffuse);

            LoadTextures();
        }

        private void GlControl1_Paint(object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

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

            GL.Rotate(x, 1.0, 0.0, 0.0);
            GL.Rotate(z, 0.0, 1.0, 0.0);
            GL.Rotate(y, 0.0, 0.0, 1.0);

            DrawTexturedCube(8.0f);

            glControl1.SwapBuffers();
        }

        private void LoadTextures()
        {
            GL.GenTextures(6, textureIds);
            for (int i = 0; i < 6; i++)
            {
                Bitmap bmp = new Bitmap(256, 256);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.FromArgb(255, 128, 0)); // Turuncu
                    if (i == 0 || i == 1 || i == 2)
                    {
                        using (Font font = new Font("Arial", 48, FontStyle.Bold, GraphicsUnit.Pixel))
                        using (SolidBrush brush = new SolidBrush(Color.DarkBlue))
                        {
                            StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                            g.DrawString("MERGEN", font, brush, new RectangleF(0, 0, bmp.Width, bmp.Height), sf);
                        }
                    }
                }

                BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.BindTexture(TextureTarget.Texture2D, textureIds[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                    data.Width, data.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                bmp.UnlockBits(data);
                bmp.Dispose();
            }
            GL.Enable(EnableCap.Texture2D);
        }

        private void DrawTexturedCube(float size)
        {
            float half = size / 2.0f;
            Vector3[] normals = {
                new Vector3(0, 0, 1), new Vector3(0, 0, -1),
                new Vector3(-1, 0, 0), new Vector3(1, 0, 0),
                new Vector3(0, -1, 0), new Vector3(0, 1, 0)
            };

            Vector3[][] faces = {
                new Vector3[] { new Vector3(-half, -half, half), new Vector3(half, -half, half), new Vector3(half, half, half), new Vector3(-half, half, half) },
                new Vector3[] { new Vector3(half, -half, -half), new Vector3(-half, -half, -half), new Vector3(-half, half, -half), new Vector3(half, half, -half) },
                new Vector3[] { new Vector3(-half, -half, -half), new Vector3(-half, -half, half), new Vector3(-half, half, half), new Vector3(-half, half, -half) },
                new Vector3[] { new Vector3(half, -half, half), new Vector3(half, -half, -half), new Vector3(half, half, -half), new Vector3(half, half, half) },
                new Vector3[] { new Vector3(-half, -half, -half), new Vector3(half, -half, -half), new Vector3(half, -half, half), new Vector3(-half, -half, half) },
                new Vector3[] { new Vector3(-half, half, half), new Vector3(half, half, half), new Vector3(half, half, -half), new Vector3(-half, half, -half) }
            };

            for (int i = 0; i < 6; i++)
            {
                GL.BindTexture(TextureTarget.Texture2D, textureIds[i]);
                GL.Begin(PrimitiveType.Quads);
                GL.Normal3(normals[i]);
                GL.TexCoord2(0, 0); GL.Vertex3(faces[i][0]);
                GL.TexCoord2(1, 0); GL.Vertex3(faces[i][1]);
                GL.TexCoord2(1, 1); GL.Vertex3(faces[i][2]);
                GL.TexCoord2(0, 1); GL.Vertex3(faces[i][3]);
                GL.End();
            }
        }
    }
}