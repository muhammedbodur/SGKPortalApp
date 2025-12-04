namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    /// <summary>
    /// Kiosk Alt İşlem DTO (Vatandaşın menü seçtikten sonra gördüğü ekran)
    /// </summary>
    public class KioskAltIslemDto
    {
        public int KioskMenuIslemId { get; set; }
        public int KanalAltId { get; set; }
        public string KanalAltAdi { get; set; } = string.Empty;
        public string KanalAdi { get; set; } = string.Empty;
        public int MenuSira { get; set; }
        
        /// <summary>
        /// Bu işlem için bekleyen sıra sayısı
        /// </summary>
        public int BekleyenSiraSayisi { get; set; }
        
        /// <summary>
        /// Bu işlem için banko modunda aktif personel var mı?
        /// </summary>
        public bool AktifPersonelVar { get; set; }
        
        /// <summary>
        /// Tahmini bekleme süresi (dakika)
        /// </summary>
        public int TahminiBeklemeSuresi { get; set; }
    }
}
