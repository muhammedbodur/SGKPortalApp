using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    /// <summary>
    /// Unvan güncelleme request DTO
    /// </summary>
    public class UnvanUpdateRequestDto
    {
        /// <summary>
        /// Unvan adı
        /// </summary>
        [Required(ErrorMessage = "Unvan adı zorunludur")]
        [StringLength(150, ErrorMessage = "Unvan adı en fazla 150 karakter olabilir")]
        public string UnvanAdi { get; set; } = string.Empty;

        /// <summary>
        /// Unvan aktiflik durumu
        /// </summary>
        [Required(ErrorMessage = "Aktiflik durumu zorunludur")]
        public Aktiflik UnvanAktiflik { get; set; } = Aktiflik.Aktif;
    }
}