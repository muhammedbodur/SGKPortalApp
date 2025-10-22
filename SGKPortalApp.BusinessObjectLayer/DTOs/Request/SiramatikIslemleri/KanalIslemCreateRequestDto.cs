using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri
{
    public class KanalIslemCreateRequestDto
    {
        [Required(ErrorMessage = "Kanal ID zorunludur")]
        public int KanalId { get; set; }

        [Required(ErrorMessage = "Hizmet binası ID zorunludur")]
        public int HizmetBinasiId { get; set; }

        [Range(0, 9999, ErrorMessage = "Başlangıç numarası 0 ile 9999 arasında olmalıdır")]
        public int BaslangicNumara { get; set; }

        [Range(0, 9999, ErrorMessage = "Bitiş numarası 0 ile 9999 arasında olmalıdır")]
        public int BitisNumara { get; set; }

        [Range(1, 999, ErrorMessage = "Sıra 1 ile 999 arasında olmalıdır")]
        public int Sira { get; set; } = 1;

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}
