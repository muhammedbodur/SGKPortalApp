using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    public class KanalPersonelResponseDto
    {
        public int KanalPersonelId { get; set; }
        public string TcKimlikNo { get; set; } = string.Empty;
        public string PersonelAdSoyad { get; set; } = string.Empty;
        public int KanalAltIslemId { get; set; }
        public string KanalAltIslemAdi { get; set; } = string.Empty;
        public PersonelUzmanlik Uzmanlik { get; set; }
        public Aktiflik Aktiflik { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
    }
}
