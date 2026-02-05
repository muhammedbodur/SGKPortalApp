using System;
using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common
{
    public class HaberCreateRequestDto
    {
        [Required(ErrorMessage = "Başlık zorunludur")]
        [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir")]
        public string Baslik { get; set; } = string.Empty;

        [Required(ErrorMessage = "İçerik zorunludur")]
        public string Icerik { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sıra zorunludur")]
        public int Sira { get; set; } = 1;

        [Required(ErrorMessage = "Yayın tarihi zorunludur")]
        public DateTime YayinTarihi { get; set; } = DateTime.Now;

        public DateTime? BitisTarihi { get; set; }

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}
