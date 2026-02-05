using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.PresentationLayer.Models.FormModels.Common
{
    public class SikKullanilanProgramFormModel
    {
        [Required(ErrorMessage = "Program adı zorunludur")]
        [StringLength(100)]
        public string ProgramAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "URL zorunludur")]
        [StringLength(500)]
        public string Url { get; set; } = string.Empty;

        [Required(ErrorMessage = "İkon class zorunludur")]
        [StringLength(50)]
        public string IkonClass { get; set; } = "bx-desktop";

        [Required(ErrorMessage = "Renk kodu zorunludur")]
        [StringLength(20)]
        public string RenkKodu { get; set; } = "primary";

        [Range(1, int.MaxValue, ErrorMessage = "Sıra 1'den büyük olmalıdır")]
        public int Sira { get; set; } = 1;

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}
