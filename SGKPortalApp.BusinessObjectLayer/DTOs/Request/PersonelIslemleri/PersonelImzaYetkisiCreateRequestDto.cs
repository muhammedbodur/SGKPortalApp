using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    public class PersonelImzaYetkisiCreateRequestDto
    {
        [Required(ErrorMessage = "Personel TC Kimlik No zorunludur")]
        [StringLength(11, MinimumLength = 11)]
        public string TcKimlikNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Departman seçimi zorunludur")]
        public int DepartmanId { get; set; }

        [Required(ErrorMessage = "Servis seçimi zorunludur")]
        public int ServisId { get; set; }

        [StringLength(200)]
        public string? GorevDegisimSebebi { get; set; }

        [Required(ErrorMessage = "İmza yetkisi başlama tarihi zorunludur")]
        public DateTime ImzaYetkisiBaslamaTarihi { get; set; }

        public DateTime? ImzaYetkisiBitisTarihi { get; set; }

        [StringLength(500)]
        public string? Aciklama { get; set; }
    }
}
