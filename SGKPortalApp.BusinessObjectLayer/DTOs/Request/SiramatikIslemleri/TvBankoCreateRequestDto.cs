using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri
{
    public class TvBankoCreateRequestDto
    {
        [Required(ErrorMessage = "TV seçimi zorunludur")]
        public int TvId { get; set; }

        [Required(ErrorMessage = "Banko seçimi zorunludur")]
        public int BankoId { get; set; }
    }
}
