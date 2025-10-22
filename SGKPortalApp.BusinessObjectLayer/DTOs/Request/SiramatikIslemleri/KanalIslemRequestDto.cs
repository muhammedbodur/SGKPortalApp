using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri
{
    /// <summary>
    /// Kanal İşlem için ortak Request DTO (Create ve Update için kullanılır)
    /// </summary>
    public class KanalIslemRequestDto
    {
        [Required(ErrorMessage = "Kanal işlem adı zorunludur")]
        [StringLength(100, ErrorMessage = "Kanal işlem adı en fazla 100 karakter olabilir")]
        public string KanalIslemAdi { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Aciklama { get; set; }

        [Required(ErrorMessage = "Sıra numarası zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Sıra numarası 1'den büyük olmalıdır")]
        public int Sira { get; set; }

        [Required(ErrorMessage = "Aktiflik durumu zorunludur")]
        public Aktiflik Aktiflik { get; set; }
    }
}
