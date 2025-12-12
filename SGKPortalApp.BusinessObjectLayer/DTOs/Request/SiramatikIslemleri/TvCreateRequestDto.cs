using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri
{
    public class TvCreateRequestDto
    {
        [Required(ErrorMessage = "TV adı zorunludur")]
        [StringLength(100, ErrorMessage = "TV adı en fazla 100 karakter olabilir")]
        public string TvAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hizmet binası seçimi zorunludur")]
        public int HizmetBinasiId { get; set; }

        [Required(ErrorMessage = "Kat tipi seçimi zorunludur")]
        public KatTipi KatTipi { get; set; }

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? TvAciklama { get; set; }

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}
