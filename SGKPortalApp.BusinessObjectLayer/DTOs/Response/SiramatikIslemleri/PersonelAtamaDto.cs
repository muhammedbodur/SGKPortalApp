namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    /// <summary>
    /// Bankoya atanmış personel bilgisi
    /// </summary>
    public class PersonelAtamaDto
    {
        public string TcKimlikNo { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty;
        public string ServisAdi { get; set; } = string.Empty;
        public string? Resim { get; set; }
        public DateTime AtanmaTarihi { get; set; }
    }
}
