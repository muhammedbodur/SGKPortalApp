namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri
{
    /// <summary>
    /// Personel Günlük Mesai DTO - Detay için
    /// </summary>
    public class PersonelMesaiGunlukDto
    {
        public DateTime Tarih { get; set; }
        public TimeSpan? GirisSaati { get; set; }
        public TimeSpan? CikisSaati { get; set; }
        public string MesaiSuresi { get; set; } = "-";
        public string? Durum { get; set; } // İzin, Mazeret, Hafta Sonu, vb.
        public bool HaftaSonu { get; set; }
        public bool GecKalma { get; set; }
    }
}
