using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri
{
    /// <summary>
    /// İzin/Mazeret talebi onay/red işlemi DTO
    /// 1. veya 2. Onayci tarafından kullanılır
    /// </summary>
    public class IzinMazeretTalepOnayRequestDto
    {
        /// <summary>
        /// Onay durumu (Onaylandi, Reddedildi)
        /// </summary>
        [Required(ErrorMessage = "Onay durumu zorunludur")]
        public OnayDurumu OnayDurumu { get; set; }

        /// <summary>
        /// Onayci açıklaması (Red/Onay nedeni)
        /// </summary>
        [StringLength(500)]
        public string? Aciklama { get; set; }

        /// <summary>
        /// Hangi onayci (1 veya 2)
        /// </summary>
        [Required(ErrorMessage = "Onayci seviyesi zorunludur")]
        [Range(1, 2, ErrorMessage = "Onayci seviyesi 1 veya 2 olmalıdır")]
        public int OnayciSeviyesi { get; set; }
    }
}
