using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    /// <summary>
    /// Servis güncelleme request DTO
    /// </summary>
    public class ServisUpdateRequestDto
    {
        /// <summary>
        /// Servis adı
        /// </summary>
        [Required(ErrorMessage = "Servis adı zorunludur")]
        [StringLength(150, ErrorMessage = "Servis adı en fazla 150 karakter olabilir")]
        public string ServisAdi { get; set; } = string.Empty;

        /// <summary>
        /// Servis aktiflik durumu
        /// </summary>
        [Required(ErrorMessage = "Aktiflik durumu zorunludur")]
        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}