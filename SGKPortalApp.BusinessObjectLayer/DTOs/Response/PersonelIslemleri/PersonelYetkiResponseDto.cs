using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri
{
    public class PersonelYetkiResponseDto
    {
        public int PersonelYetkiId { get; set; }
        public string TcKimlikNo { get; set; } = string.Empty;

        public int YetkiId { get; set; }
        public string? YetkiAdi { get; set; }

        public int ModulControllerIslemId { get; set; }
        public string? ModulControllerIslemAdi { get; set; }

        public YetkiTipleri YetkiTipleri { get; set; }

        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
    }
}
