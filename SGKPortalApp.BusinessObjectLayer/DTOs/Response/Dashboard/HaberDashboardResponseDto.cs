using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Dashboard
{
    public class HaberDashboardResponseDto
    {
        public int HaberId { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string Icerik { get; set; } = string.Empty;
        public string? GorselUrl { get; set; }
        public int Sira { get; set; }
        public DateTime YayinTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public Aktiflik Aktiflik { get; set; }

        // UI için ek alanlar
        public string? BadgeColor { get; set; }
        public string? BadgeText { get; set; }
    }
}
