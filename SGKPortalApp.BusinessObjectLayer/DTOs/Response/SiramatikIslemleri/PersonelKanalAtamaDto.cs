using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    /// <summary>
    /// Bir personelin bir kanal alt işleme atama bilgisi
    /// </summary>
    public class PersonelKanalAtamaDto
    {
        public int KanalPersonelId { get; set; }
        public int KanalAltIslemId { get; set; }
        public string KanalAltIslemAdi { get; set; } = string.Empty;
        public PersonelUzmanlik Uzmanlik { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
    }
}
