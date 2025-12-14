using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    public class PersonelYetkiUpdateRequestDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir YetkiId giriniz")]
        public int YetkiId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir ModulControllerIslemId giriniz")]
        public int ModulControllerIslemId { get; set; }

        [Required(ErrorMessage = "YetkiTipleri zorunludur")]
        public YetkiTipleri YetkiTipleri { get; set; } = YetkiTipleri.View;
    }
}
