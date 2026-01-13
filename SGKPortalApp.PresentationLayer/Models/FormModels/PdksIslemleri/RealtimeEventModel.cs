namespace SGKPortalApp.PresentationLayer.Models.FormModels.PdksIslemleri
{
    public class RealtimeEventModel
    {
        public string EnrollNumber { get; set; } = string.Empty;
        public DateTime EventTime { get; set; }
        public int VerifyMethod { get; set; }
        public int InOutMode { get; set; }
        public int WorkCode { get; set; }
        public string DeviceIp { get; set; } = string.Empty;
        public bool IsValid { get; set; }
        public long? CardNumber { get; set; }

        // Personel bilgileri
        public string? PersonelAdSoyad { get; set; }
        public string? PersonelSicilNo { get; set; }
        public string? PersonelDepartman { get; set; }
        public string? PersonelTcKimlikNo { get; set; }
        public long? PersonelKayitNo { get; set; }

        // Cihaz bilgileri
        public string? DeviceName { get; set; }
        public int? HizmetBinasiId { get; set; }
        public string? HizmetBinasiAdi { get; set; }
    }
}
