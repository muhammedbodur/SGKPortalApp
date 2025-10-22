using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.PresentationLayer.Models.FormModels.PersonelIslemleri
{
    public class SendikaFormModel
    {
        [Required(ErrorMessage = "Sendika adı zorunludur")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Sendika adı 2-200 karakter arasında olmalıdır")]
        public string SendikaAdi { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
