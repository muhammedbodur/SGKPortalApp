using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common
{
    public class ModulControllerUpdateRequestDto
    {
        [Required(ErrorMessage = "Controller adı zorunludur")]
        [StringLength(100, ErrorMessage = "Controller adı en fazla 100 karakter olabilir")]
        public string ModulControllerAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Modül seçimi zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir modül seçiniz")]
        public int ModulId { get; set; }

        /// <summary>
        /// Üst controller (parent) - Hiyerarşik yapı için
        /// NULL ise root controller'dır
        /// </summary>
        public int? UstModulControllerId { get; set; }
    }
}
