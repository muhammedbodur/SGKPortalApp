using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    public class PersonelYetkiUpdateRequestDto
    {
        [Required(ErrorMessage = "ModulControllerIslemId zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir ModulControllerIslemId giriniz")]
        public int ModulControllerIslemId { get; set; }

        [Required(ErrorMessage = "YetkiSeviyesi zorunludur")]
        public YetkiSeviyesi YetkiSeviyesi { get; set; } = YetkiSeviyesi.View;
    }
}
