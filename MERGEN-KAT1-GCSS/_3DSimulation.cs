using System;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace MERGEN_KAT1_GCSS
{
    public partial class _3DSimulation
    {
        private ElementHost _host;
        private HelixViewport3D _viewport;
        private ModelVisual3D _satelliteModel;

        // Helix'in donanımsal eksen rotasyonları
        private AxisAngleRotation3D _yawRotation;
        private AxisAngleRotation3D _pitchRotation;
        private AxisAngleRotation3D _rollRotation;

        // Dışarıdan okunma ihtimaline karşı eski public değişkenleri tutuyoruz
        public float x = 0, y = 0, z = 0;

        // Sensörden gelen "Hedef" açılar
        private float _targetYaw = 0, _targetPitch = 0, _targetRoll = 0;

        // Ekranda o an çizilen "Mevcut" açılar (Yumuşatma için)
        private float _currentYaw = 0, _currentPitch = 0, _currentRoll = 0;

        // Ekran yenileme zamanlayıcısı (Render Loop)
        private Timer _renderTimer;

        public _3DSimulation(Control containerControl)
        {
            if (containerControl == null) throw new ArgumentNullException(nameof(containerControl));

            InitializeHelix(containerControl);

            // Kasıntıyı önlemek için 60 FPS (16ms) hızında çalışan Render Motorunu başlat
            _renderTimer = new Timer();
            _renderTimer.Interval = 16;
            _renderTimer.Tick += RenderTimer_Tick;
            _renderTimer.Start();
        }

        private void InitializeHelix(Control container)
        {
            // WinForms içinde WPF nesnelerini barındıracak Host aracı
            _host = new ElementHost { Dock = DockStyle.Fill };
            container.Controls.Add(_host);

            // Helix 3D Kamerası ve Uzayı
            _viewport = new HelixViewport3D
            {
                ShowViewCube = false,
                ShowCoordinateSystem = true, // X-Y-Z eksen oklarını gösterir
                Background = new SolidColorBrush(Color.FromRgb(24, 30, 54))
            };

            // Işıklandırma
            _viewport.Children.Add(new DefaultLights());

            // Tek ve Sabit Uydu Modelini Oluştur
            _satelliteModel = new ModelVisual3D();
            _satelliteModel.Content = CreateSatelliteModel();

            // Transform3DGroup ile Gimbal Lock önlenir
            var transformGroup = new Transform3DGroup();

            // DONANIMSAL DÜZELTME: Sensörün PCB alt katmanında ters durmasını dengeleyen 180 derecelik sabit dönüş.
            transformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 180)));

            // Dinamik Eksenler
            _yawRotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
            _pitchRotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 0);
            _rollRotation = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0);

            // Sıralama (YAW -> PITCH -> ROLL)
            transformGroup.Children.Add(new RotateTransform3D(_yawRotation));
            transformGroup.Children.Add(new RotateTransform3D(_pitchRotation));
            transformGroup.Children.Add(new RotateTransform3D(_rollRotation));

            _satelliteModel.Transform = transformGroup;
            _viewport.Children.Add(_satelliteModel);

            _host.Child = _viewport;
        }

        public void UpdateRotation(float yaw, float pitch, float roll)
        {
            // Eski sistem değişkenlerini güncel tut
            z = yaw;
            x = pitch;
            y = roll;

            // Yeni gelen 1 Hz'lik veriyi hedef olarak belirliyoruz
            _targetYaw = yaw;
            _targetPitch = pitch;
            _targetRoll = roll;
        }

        private void RenderTimer_Tick(object sender, EventArgs e)
        {
            // LERP (Linear Interpolation) ile Pürüzsüzleştirme
            // Saniyede 1 gelen veri sıçramasını sönümlemek için katsayı 0.05f olarak ayarlandı.
            // Bu sayede animasyon tık tık atmaz, kesintisiz ve yağ gibi akar.
            _currentYaw += (_targetYaw - _currentYaw) * 0.05f;
            _currentPitch += (_targetPitch - _currentPitch) * 0.05f;
            _currentRoll += (_targetRoll - _currentRoll) * 0.05f;

            // WPF UI Thread üzerinden yormadan ekran kartını (GPU) güncelliyoruz
            _yawRotation.Angle = -_currentYaw;
            _pitchRotation.Angle = _currentPitch;
            _rollRotation.Angle = _currentRoll;
        }

        private GeometryModel3D CreateSatelliteModel()
        {
            // Tek, sabit turuncu silindir
            var builder = new MeshBuilder();

            // Boyutlar: Yarıçap (Radius) = 5, Yükseklik = 20 (-10'dan 10'a)
            builder.AddCylinder(new Point3D(0, -10, 0), new Point3D(0, 10, 0), 5, 36);

            var material = MaterialHelper.CreateMaterial(Colors.Orange);

            return new GeometryModel3D(builder.ToMesh(), material);
        }
    }
}