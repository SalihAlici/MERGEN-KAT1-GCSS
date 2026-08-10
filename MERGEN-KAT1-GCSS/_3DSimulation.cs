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

        // Dışarıdan okunma ihtimaline karşı public değişkenler
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

            // --- 1. KAMERAYI BİRAZ GERİ ÇEK (Model Küçülür) ---
            _viewport.Camera.Position = new Point3D(0, -35, 0);
            _viewport.Camera.LookDirection = new Vector3D(0, 35, 0);
            _viewport.Camera.UpDirection = new Vector3D(0, 0, 1);

            // --- 2. YUMUŞATILMIŞ IŞIKLANDIRMA (Parlamayı Önler) ---
            var lightGroup = new Model3DGroup();

            // Genel ortam ışığını hafif artırıyoruz ki her yer eşit aydınlansın
            lightGroup.Children.Add(new AmbientLight(Color.FromRgb(120, 120, 120)));

            // Ortadaki beyaz patlamayı önlemek için saf beyaz yerine daha kısık (gri) ve açılı bir ışık veriyoruz
            lightGroup.Children.Add(new DirectionalLight(Color.FromRgb(130, 130, 130), new Vector3D(0, 1, -0.5)));

            _viewport.Children.Add(new ModelVisual3D { Content = lightGroup });

            // Tek ve Sabit Uydu Modelini Oluştur
            _satelliteModel = new ModelVisual3D();
            _satelliteModel.Content = CreateSatelliteModel();

            // Transform3DGroup ile Gimbal Lock önlenir
            var transformGroup = new Transform3DGroup();

            // DONANIMSAL DÜZELTME: Sensör PCB'nin alt katmanında (ters) olduğu için 180 derece takla attırıyoruz.
            transformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 180)));

            // DİNAMİK EKSENLER: Model Z ekseninde dik durduğu için eksenleri Z-Up sistemine göre ayarladık.
            _yawRotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 0);
            _pitchRotation = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0);
            _rollRotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);

            // Sıralama
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

        // Açıların en kısa yolunu hesaplar (Kendi etrafında fırıldak gibi dönmeyi engeller)
        private float LerpAngle(float current, float target, float speed)
        {
            float diff = target - current;

            // Aradaki farkı her zaman -180 ile +180 derece arasına sıkıştırır
            while (diff < -180f) diff += 360f;
            while (diff > 180f) diff -= 360f;

            return current + diff * speed;
        }

        private void RenderTimer_Tick(object sender, EventArgs e)
        {
            // Saniyede 1 gelen veri atlamalarını sönümler, açıları en kısa yoldan pürüzsüz çevirir
            _currentYaw = LerpAngle(_currentYaw, _targetYaw, 0.05f);
            _currentPitch = LerpAngle(_currentPitch, _targetPitch, 0.05f);
            _currentRoll = LerpAngle(_currentRoll, _targetRoll, 0.05f);

            _yawRotation.Angle = -_currentYaw;

            // --- ÇAPRAZ EKSEN DÜZELTMESİ ---
            // Sensör dizgisi 90 derece yatık olduğu için Pitch açısını Roll'a, Roll açısını Pitch'e atıyoruz.
            _pitchRotation.Angle = -_currentRoll;
            _rollRotation.Angle = -_currentPitch;
        }

        private GeometryModel3D CreateSatelliteModel()
        {
            var builder = new MeshBuilder();

            // Boyu (-7, 7) ve yarıçapı (8) olan tam dolu, kapalı katı silindir
            builder.AddCylinder(new Point3D(0, 0, -7), new Point3D(0, 0, 7), 8, 36);

            // Orijinal saf turuncu materyal
            var material = MaterialHelper.CreateMaterial(Colors.Orange);

            var geometryModel = new GeometryModel3D(builder.ToMesh(), material);

            // Arkaya veya içeri bakan yüzeylerin de turuncu görünmesini sağlar (kesikliği önler)
            geometryModel.BackMaterial = material;

            return geometryModel;
        }
    }
}