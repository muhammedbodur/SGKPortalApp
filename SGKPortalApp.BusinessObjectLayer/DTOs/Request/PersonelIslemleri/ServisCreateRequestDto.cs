using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    public class ServisCreateRequestDto
    {
        [Required(ErrorMessage = "Servis adı zorunludur")]
        [StringLength(150, ErrorMessage = "Servis adı en fazla 150 karakter olabilir")]
        public string ServisAdi { get; set; } = string.Empty;
        public Aktiflik ServisAktiflik { get; set; } = Aktiflik.Aktif;
    }
}