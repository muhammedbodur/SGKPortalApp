using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    public class PersonelYetkiCreateRequestDto
    {
        [Required(ErrorMessage = "TcKimlikNo zorunludur")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 karakter olmalıdır")]
        public string TcKimlikNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "ModulControllerIslemId zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir ModulControllerIslemId giriniz")]
        public int ModulControllerIslemId { get; set; }

        [Required(ErrorMessage = "YetkiSeviyesi zorunludur")]
        public YetkiSeviyesi YetkiSeviyesi { get; set; } = YetkiSeviyesi.View;
    }
}
