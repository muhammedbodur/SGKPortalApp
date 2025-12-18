using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.YetkiIslemleri
{
    /// <summary>
    /// Sayfa action konfigürasyonu oluşturma DTO
    /// </summary>
    public class PageActionConfigCreateRequestDto
    {
        [Required(ErrorMessage = "Sayfa key zorunludur")]
        [StringLength(200)]
        public string PageKey { get; set; } = string.Empty;

        [Required(ErrorMessage = "En az bir action seçilmelidir")]
        public List<string> Actions { get; set; } = new();

        /// <summary>
        /// Hangi rollere/kullanıcılara atanacak (opsiyonel)
        /// </summary>
        public List<int>? RoleIds { get; set; }
    }
}
