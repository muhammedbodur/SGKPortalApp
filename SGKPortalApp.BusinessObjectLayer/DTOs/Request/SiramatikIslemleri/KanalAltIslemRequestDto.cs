using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri
{
    /// <summary>
    /// Kanal Alt İşlem için ortak Request DTO (Create ve Update için kullanılır)
    /// </summary>
    public class KanalAltIslemRequestDto
    {
        [Required(ErrorMessage = "Kanal işlem zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir kanal işlem seçiniz")]
        public int KanalIslemId { get; set; }

        [Required(ErrorMessage = "Kanal alt işlem adı zorunludur")]
        [StringLength(100, ErrorMessage = "Kanal alt işlem adı en fazla 100 karakter olabilir")]
        public string KanalAltAdi { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Aciklama { get; set; }

        [Required(ErrorMessage = "Sıra numarası zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Sıra numarası 1'den büyük olmalıdır")]
        public int Sira { get; set; }

        [Required(ErrorMessage = "Aktiflik durumu zorunludur")]
        public Aktiflik Aktiflik { get; set; }
    }
}
