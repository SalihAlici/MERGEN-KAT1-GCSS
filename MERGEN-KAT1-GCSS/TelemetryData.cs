using System;
using System.Globalization;

namespace MERGEN_KAT1_GCSS
{
    public class TelemetryData
    {
        // Sensör Verileri
        public int PaketNumarasi { get; set; }
        public int UyduStatusu { get; set; }
        public string HataKodu { get; set; }
        public string GondermeSaati { get; set; }

        // Basınç ve Yükseklik
        public float Basinc { get; set; }
        public float Yukseklik { get; set; }

        // Hareket Verileri
        public float InisHizi { get; set; }
        public float Sicaklik { get; set; }
        public float PilGerilimi { get; set; }

        // GPS Verileri (Map sınıfı için)
        public double GpsLatitude { get; set; }
        public double GpsLongitude { get; set; }
        public float GpsAltitude { get; set; }

        // Yapay Ufuk / Yönelim için
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public float Yaw { get; set; }

        // Takım Verisi
        public int TakimNo { get; set; }

        public static bool TryParse(string rawData, out TelemetryData telemetry)
        {
            telemetry = null;

            // rawData zaten Data.cs tarafından $ ve # işaretlerinden temizlenmiş olarak geliyor.
            if (string.IsNullOrWhiteSpace(rawData))
                return false;

            try
            {
                // Temiz veriyi doğrudan virgüle göre parçala
                string[] parts = rawData.Split(',');

                // MERSAT'tan gelen 16 elemanlı yapı
                // Eksik veya hatalı paket geldiyse çöpe at
                if (parts.Length < 16)
                    return false;

                telemetry = new TelemetryData
                {
                    PaketNumarasi = SafeParseInt(parts[0]),
                    UyduStatusu = SafeParseInt(parts[1]),
                    HataKodu = parts[2],
                    GondermeSaati = parts[3],
                    Basinc = SafeParseFloat(parts[4]),
                    Yukseklik = SafeParseFloat(parts[5]),
                    InisHizi = SafeParseFloat(parts[6]),
                    Sicaklik = SafeParseFloat(parts[7]),
                    PilGerilimi = SafeParseFloat(parts[8]),
                    GpsLatitude = SafeParseDouble(parts[9]),
                    GpsLongitude = SafeParseDouble(parts[10]),
                    GpsAltitude = SafeParseFloat(parts[11]),
                    Pitch = SafeParseFloat(parts[12]),
                    Roll = SafeParseFloat(parts[13]),
                    Yaw = SafeParseFloat(parts[14]),
                    TakimNo = SafeParseInt(parts[15])
                };
                return true;
            }
            catch
            {
                telemetry = null;
                return false;
            }
        }

        #region Yardımcı Parsing Metodları
        private static int SafeParseInt(string s)
        {
            return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result) ? result : 0;
        }

        private static float SafeParseFloat(string s)
        {
            return float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out float result) ? result : 0f;
        }

        private static double SafeParseDouble(string s)
        {
            return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double result) ? result : 0.0;
        }
        #endregion

        // CSV loglama için optimize ToString
        public override string ToString()
        {
            return $"{PaketNumarasi};{UyduStatusu};{HataKodu};{GondermeSaati};" +
                   $"{Basinc};{Yukseklik};{InisHizi};{Sicaklik};{PilGerilimi};" +
                   $"{GpsLatitude};{GpsLongitude};{GpsAltitude};" +
                   $"{Pitch};{Roll};{Yaw};{TakimNo}";
        }
    }
}