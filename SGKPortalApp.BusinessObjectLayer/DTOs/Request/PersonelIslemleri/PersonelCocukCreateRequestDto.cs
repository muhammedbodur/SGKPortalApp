using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    public class PersonelCocukCreateRequestDto
    {
        [Required(ErrorMessage = "Personel TC Kimlik No zorunludur")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 karakter olmalıdır")]
        public string PersonelTcKimlikNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Çocuk adı zorunludur")]
        [StringLength(200, ErrorMessage = "Çocuk adı en fazla 200 karakter olabilir")]
        public string CocukAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Doğum tarihi zorunludur")]
        public DateOnly CocukDogumTarihi { get; set; }

        public OgrenimDurumu OgrenimDurumu { get; set; }
    }
}
