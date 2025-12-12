using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    public class UnvanCreateRequestDto
    {
        [Required(ErrorMessage = "Unvan adı zorunludur")]
        [StringLength(150, ErrorMessage = "Unvan adı en fazla 150 karakter olabilir")]
        public string UnvanAdi { get; set; } = string.Empty;
        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}