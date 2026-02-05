using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.PresentationLayer.Models.FormModels.Common
{
    public class GununMenusuFormModel
    {
        [Required(ErrorMessage = "Tarih zorunludur")]
        public DateTime Tarih { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Menü içeriği zorunludur")]
        public string Icerik { get; set; } = string.Empty;

        [Required]
        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}
