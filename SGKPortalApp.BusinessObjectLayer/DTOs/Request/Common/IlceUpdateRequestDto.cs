using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common
{
    /// <summary>
    /// İlçe güncelleme request DTO
    /// </summary>
    public class IlceUpdateRequestDto
    {
        /// <summary>
        /// İl ID
        /// </summary>
        [Required(ErrorMessage = "İl seçimi zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir il seçiniz")]
        public int IlId { get; set; }

        /// <summary>
        /// İlçe adı
        /// </summary>
        [Required(ErrorMessage = "İlçe adı zorunludur")]
        [StringLength(50, ErrorMessage = "İlçe adı en fazla 50 karakter olabilir")]
        public string IlceAdi { get; set; } = string.Empty;
    }
}
