using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common
{
    public class IlceCreateRequestDto
    {
        [Required(ErrorMessage = "İl seçimi zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir il seçiniz")]
        public int IlId { get; set; }

        [Required(ErrorMessage = "İlçe adı zorunludur")]
        [StringLength(50, ErrorMessage = "İlçe adı en fazla 50 karakter olabilir")]
        public string IlceAdi { get; set; } = string.Empty;
    }
}
