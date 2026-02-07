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
        public float Basinc1 { get; set; }
        public float Basinc2 { get; set; }
        public float Yukseklik1 { get; set; }
        public float Yukseklik2 { get; set; }
        public float IrtifaFarki { get; set; }

        // Hareket Verileri
        public float InisHizi { get; set; }
        public float Sicaklik { get; set; }
        public float PilGerilimi { get; set; }

        // GPS Verileri (Map sınıfı için)
        public double Gps1Latitude { get; set; }
        public double Gps1Longitude { get; set; }
        public float Gps1Altitude { get; set; }

        // 3D Simülasyon için yönelim
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public float Yaw { get; set; }

        // Diğer Veriler
        public string RHRH { get; set; }
        public float IoTS1Data { get; set; }
        public float IoTS2Data { get; set; }
        public int TakimNo { get; set; }

        public static bool TryParse(string rawData, out TelemetryData telemetry)
        {
            telemetry = null;

            if (string.IsNullOrWhiteSpace(rawData) || !rawData.Contains("*"))
                return false;

            try
            {
                string[] parts = rawData.Trim().Split('*');
                if (parts.Length < 22)
                    return false;

                telemetry = new TelemetryData
                {
                    PaketNumarasi = SafeParseInt(parts[0]),
                    UyduStatusu = SafeParseInt(parts[1]),
                    HataKodu = parts[2],
                    GondermeSaati = parts[3],
                    Basinc1 = SafeParseFloat(parts[4]),
                    Basinc2 = SafeParseFloat(parts[5]),
                    Yukseklik1 = SafeParseFloat(parts[6]),
                    Yukseklik2 = SafeParseFloat(parts[7]),
                    IrtifaFarki = SafeParseFloat(parts[8]),
                    InisHizi = SafeParseFloat(parts[9]),
                    Sicaklik = SafeParseFloat(parts[10]),
                    PilGerilimi = SafeParseFloat(parts[11]),
                    Gps1Latitude = SafeParseDouble(parts[12]),
                    Gps1Longitude = SafeParseDouble(parts[13]),
                    Gps1Altitude = SafeParseFloat(parts[14]),
                    Pitch = SafeParseFloat(parts[15]),
                    Roll = SafeParseFloat(parts[16]),
                    Yaw = SafeParseFloat(parts[17]),
                    RHRH = parts[18],
                    IoTS1Data = SafeParseFloat(parts[19]),
                    IoTS2Data = SafeParseFloat(parts[20]),
                    TakimNo = SafeParseInt(parts[21])
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

        // CSV için optimize ToString
        public override string ToString()
        {
            return $"{PaketNumarasi};{UyduStatusu};{HataKodu};{GondermeSaati};" +
                   $"{Basinc1};{Basinc2};{Yukseklik1};{Yukseklik2};{IrtifaFarki};" +
                   $"{InisHizi};{Sicaklik};{PilGerilimi};{Gps1Latitude};" +
                   $"{Gps1Longitude};{Gps1Altitude};{Pitch};{Roll};{Yaw};" +
                   $"{RHRH};{IoTS1Data};{IoTS2Data};{TakimNo}";
        }
    }
}