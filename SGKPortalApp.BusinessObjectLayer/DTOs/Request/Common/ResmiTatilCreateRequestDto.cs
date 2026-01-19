using System;
using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common
{
    public class ResmiTatilCreateRequestDto
    {
        [Required(ErrorMessage = "Tatil adı zorunludur")]
        [StringLength(100, ErrorMessage = "Tatil adı en fazla 100 karakter olabilir")]
        public string TatilAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tatil tarihi zorunludur")]
        public DateTime Tarih { get; set; }

        [Required(ErrorMessage = "Tatil tipi zorunludur")]
        public TatilTipi TatilTipi { get; set; }

        public bool YariGun { get; set; } = false;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Aciklama { get; set; }
    }
}
