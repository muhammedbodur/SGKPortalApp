using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common
{
    public class GununMenusuUpdateRequestDto
    {
        [Required(ErrorMessage = "Tarih zorunludur")]
        public DateTime Tarih { get; set; }

        [Required(ErrorMessage = "Menü içeriği zorunludur")]
        public string Icerik { get; set; } = string.Empty;

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}
