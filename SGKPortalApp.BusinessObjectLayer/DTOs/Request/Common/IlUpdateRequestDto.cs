using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common
{
    /// <summary>
    /// İl güncelleme request DTO
    /// </summary>
    public class IlUpdateRequestDto
    {
        /// <summary>
        /// İl adı
        /// </summary>
        [Required(ErrorMessage = "İl adı zorunludur")]
        [StringLength(50, ErrorMessage = "İl adı en fazla 50 karakter olabilir")]
        public string IlAdi { get; set; } = string.Empty;
    }
}
