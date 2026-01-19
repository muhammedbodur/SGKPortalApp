using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common
{
    /// <summary>
    /// Nager.Date'ten resmi tatilleri senkronize etmek için kullanılan DTO
    /// </summary>
    public class ResmiTatilSyncRequestDto
    {
        [Required(ErrorMessage = "Yıl zorunludur")]
        [Range(2020, 2100, ErrorMessage = "Yıl 2020-2100 arasında olmalıdır")]
        public int Yil { get; set; }

        /// <summary>
        /// Mevcut kayıtları silip yeniden mi oluşturulsun?
        /// </summary>
        public bool MevcutlariSil { get; set; } = false;
    }
}
