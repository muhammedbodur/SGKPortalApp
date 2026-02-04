using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common
{
    public class HaberCreateRequestDto
    {
        [Required(ErrorMessage = "Başlık zorunludur")]
        [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir")]
        public string Baslik { get; set; } = string.Empty;

        [Required(ErrorMessage = "İçerik zorunludur")]
        public string Icerik { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Görsel URL en fazla 500 karakter olabilir")]
        public string? GorselUrl { get; set; }

        [Required(ErrorMessage = "Sıra zorunludur")]
        [Range(1, 999, ErrorMessage = "Sıra 1-999 arasında olmalıdır")]
        public int Sira { get; set; } = 1;

        [Required(ErrorMessage = "Yayın tarihi zorunludur")]
        public DateTime YayinTarihi { get; set; } = DateTime.Now;

        public DateTime? BitisTarihi { get; set; }

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}
