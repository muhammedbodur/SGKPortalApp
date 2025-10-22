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

        [Range(0, 9999, ErrorMessage = "Başlangıç numarası 0 ile 9999 arasında olmalıdır")]
        public int BaslangicNumara { get; set; }

        [Range(0, 9999, ErrorMessage = "Bitiş numarası 0 ile 9999 arasında olmalıdır")]
        public int BitisNumara { get; set; }

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}