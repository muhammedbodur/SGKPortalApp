using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    /// <summary>
    /// Departman güncelleme request DTO
    /// </summary>
    public class DepartmanUpdateRequestDto
    {
        /// <summary>
        /// Departman adı
        /// </summary>
        [Required(ErrorMessage = "Departman adı zorunludur")]
        [StringLength(150, ErrorMessage = "Departman adı en fazla 150 karakter olabilir")]
        public string DepartmanAdi { get; set; } = string.Empty;

        /// <summary>
        /// Departman kısa adı
        /// </summary>
        [StringLength(50, ErrorMessage = "Departman kısa adı en fazla 50 karakter olabilir")]
        public string? DepartmanAdiKisa { get; set; }

        /// <summary>
        /// Departman aktiflik durumu
        /// </summary>
        [Required(ErrorMessage = "Aktiflik durumu zorunludur")]
        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}