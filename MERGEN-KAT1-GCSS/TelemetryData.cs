using System;
using System.Globalization;

namespace MERGEN_KAT1_GCSS
{
    public class TelemetryData
    {
        // 1. Tanımlayıcı Bilgiler
        public int TakimID { get; set; }
        public int PaketNumarasi { get; set; }
        public string Zaman { get; set; }

        // 2. Sensör ve Durum Verileri
        public float Basinc { get; set; }
        public float GoreceliYukseklik { get; set; }
        public float InisHizi { get; set; }
        public float PilGerilimi { get; set; }

        // 3. GPS Verileri
        public double GpsEnlem { get; set; }
        public double GpsBoylam { get; set; }
        public float GpsYukseklik { get; set; }

        // 4. Eksen Verileri (3D Simülasyon için)
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public float Yaw { get; set; }

        // 5. Görev Verileri
        public int AyrilmaDurumu { get; set; }
        public string KriptoMesaj { get; set; }

        public static bool TryParse(string rawData, out TelemetryData telemetry)
        {
            telemetry = null;

            if (string.IsNullOrWhiteSpace(rawData))
                return false;

            try
            {
                string[] parts = rawData.Trim().Split(',');

                if (parts.Length < 15)
                    return false;

                telemetry = new TelemetryData
                {
                    TakimID = SafeParseInt(parts[0]),
                    PaketNumarasi = SafeParseInt(parts[1]),
                    Zaman = parts[2],
                    Basinc = SafeParseFloat(parts[3]),
                    GoreceliYukseklik = SafeParseFloat(parts[4]),
                    InisHizi = SafeParseFloat(parts[5]),
                    PilGerilimi = SafeParseFloat(parts[6]),
                    GpsEnlem = SafeParseDouble(parts[7]),
                    GpsBoylam = SafeParseDouble(parts[8]),
                    GpsYukseklik = SafeParseFloat(parts[9]),
                    Pitch = SafeParseFloat(parts[10]),
                    Roll = SafeParseFloat(parts[11]),
                    Yaw = SafeParseFloat(parts[12]),
                    AyrilmaDurumu = SafeParseInt(parts[13]),
                    KriptoMesaj = parts[14]
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

        // CSV dosyası oluştururken ilk satıra eklenecek Azerbaycanca başlıklar
        public static string GetCsvHeader()
        {
            return "KOMANDA ID,PAKET NÖMRƏSİ,VAXT,TƏZYİQ,NİSBİ HÜNDÜRLÜK,ENMƏ SÜRƏTİ,GƏRGİNLİK,GPS LAT,GPS LNG,GPS HÜNDÜRLÜK,PİTCH,ROLL,YAW,AYRILMA,KRİPTOMESAJ";
        }

        // CSV dosyasına veri satırı eklerken kullanılacak metot
        public override string ToString()
        {
            return $"{TakimID},{PaketNumarasi},{Zaman}," +
                   $"{Basinc.ToString(CultureInfo.InvariantCulture)}," +
                   $"{GoreceliYukseklik.ToString(CultureInfo.InvariantCulture)}," +
                   $"{InisHizi.ToString(CultureInfo.InvariantCulture)}," +
                   $"{PilGerilimi.ToString(CultureInfo.InvariantCulture)}," +
                   $"{GpsEnlem.ToString(CultureInfo.InvariantCulture)}," +
                   $"{GpsBoylam.ToString(CultureInfo.InvariantCulture)}," +
                   $"{GpsYukseklik.ToString(CultureInfo.InvariantCulture)}," +
                   $"{Pitch.ToString(CultureInfo.InvariantCulture)}," +
                   $"{Roll.ToString(CultureInfo.InvariantCulture)}," +
                   $"{Yaw.ToString(CultureInfo.InvariantCulture)}," +
                   $"{AyrilmaDurumu},{KriptoMesaj}";
        }
    }
}