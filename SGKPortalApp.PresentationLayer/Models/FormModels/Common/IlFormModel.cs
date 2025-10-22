using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.PresentationLayer.Models.FormModels.Common
{
    public class IlFormModel
    {
        [Required(ErrorMessage = "İl adı zorunludur")]
        [StringLength(50, ErrorMessage = "İl adı en fazla 50 karakter olabilir")]
        public string IlAdi { get; set; } = string.Empty;
    }
}
