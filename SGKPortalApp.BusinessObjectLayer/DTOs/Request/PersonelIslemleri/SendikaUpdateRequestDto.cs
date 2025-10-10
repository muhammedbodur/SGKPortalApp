using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    public class SendikaUpdateRequestDto
    {
        [Required(ErrorMessage = "Sendika adı zorunludur")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Sendika adı 2-200 karakter arasında olmalıdır")]
        public string SendikaAdi { get; set; } = string.Empty;

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}
