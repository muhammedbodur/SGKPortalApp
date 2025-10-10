using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    public class PersonelHizmetCreateRequestDto
    {
        [Required(ErrorMessage = "Personel TC Kimlik No zorunludur")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 karakter olmalıdır")]
        public string TcKimlikNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Departman seçimi zorunludur")]
        public int DepartmanId { get; set; }

        [Required(ErrorMessage = "Servis seçimi zorunludur")]
        public int ServisId { get; set; }

        [Required(ErrorMessage = "Göreve başlama tarihi zorunludur")]
        public DateTime GorevBaslamaTarihi { get; set; }

        public DateTime? GorevAyrilmaTarihi { get; set; }

        [StringLength(500, ErrorMessage = "Sebep en fazla 500 karakter olabilir")]
        public string? Sebep { get; set; }
    }
}
