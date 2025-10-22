using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri
{
    public class KanalIslemUpdateRequestDto
    {
        [Required(ErrorMessage = "Kanal ID zorunludur")]
        public int KanalId { get; set; }

        [Required(ErrorMessage = "Hizmet binası ID zorunludur")]
        public int HizmetBinasiId { get; set; }

        [Required(ErrorMessage = "Kanal işlem adı zorunludur")]
        [StringLength(100, ErrorMessage = "Kanal işlem adı en fazla 100 karakter olabilir")]
        public string KanalIslemAdi { get; set; } = string.Empty;

        [Range(1, 999, ErrorMessage = "Sıra 1 ile 999 arasında olmalıdır")]
        public int Sira { get; set; } = 1;

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}
