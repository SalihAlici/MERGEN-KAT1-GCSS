using GMap.NET;
using GMap.NET.MapProviders;
using System.Windows.Forms;

namespace MERGEN_KAT1_GCSS
{
    public class Map
    {
        private GMap.NET.WindowsForms.GMapControl gMapControl;
        private float lat = 38.707675f;
        private float lng = 35.519550f;

        public Map(GMap.NET.WindowsForms.GMapControl control)
        {
            gMapControl = control;
        }

        public void InitializeMap()
        {
            // Harita türünü belirliyoruz (örneğin: OpenStreetMap)
            gMapControl.MapProvider = GMapProviders.OpenStreetMap;

            // Harita ayarlarını yapılandırıyoruz
            gMapControl.Position = new PointLatLng(lat, lng);
            gMapControl.MinZoom = 5;
            gMapControl.MaxZoom = 25;
            gMapControl.Zoom = 16;

            // Harita kontrolünü aktifleştiriyoruz
            gMapControl.ShowCenter = false;
        }

        public void SetCoordinates(float latitude, float longitude)
        {
            lat = latitude;
            lng = longitude;
            gMapControl.Position = new PointLatLng(lat, lng);
        }
    }
}
