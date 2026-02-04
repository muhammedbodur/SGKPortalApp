using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Dashboard
{
    /// <summary>
    /// Haber Response DTO
    /// </summary>
    public class HaberResponseDto
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

        // Çoklu görsel desteği
        public List<HaberGorselResponseDto> Gorseller { get; set; } = new();
    }

    /// <summary>
    /// Haber Görseli Response DTO
    /// </summary>
    public class HaberGorselResponseDto
    {
        public int HaberGorselId { get; set; }
        public int HaberId { get; set; }
        public string GorselUrl { get; set; } = string.Empty;
        public int Sira { get; set; }
        public bool VitrinFoto { get; set; }
        public string? Aciklama { get; set; }
    }
}
