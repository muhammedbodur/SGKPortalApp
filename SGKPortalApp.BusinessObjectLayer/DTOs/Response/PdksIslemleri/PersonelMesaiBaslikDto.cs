namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri
{
    /// <summary>
    /// Personel Mesai Başlık Bilgisi DTO
    /// </summary>
    public class PersonelMesaiBaslikDto
    {
        public string AdSoyad { get; set; } = string.Empty;
        public string DepartmanAdi { get; set; } = string.Empty;
        public string? BirimAdi { get; set; }
        public int SicilNo { get; set; }
    }
}
