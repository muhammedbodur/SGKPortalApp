using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    public class GununMenusuResponseDto
    {
        public int MenuId { get; set; }
        public DateTime Tarih { get; set; }
        public string Icerik { get; set; } = string.Empty;
        public Aktiflik Aktiflik { get; set; }
        public DateTime EklenmeTarihi { get; set; }
    }
}
