using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    public class PersonelCocukCreateDto
    {
        [Required]
        [StringLength(200)]
        public string Isim { get; set; } = string.Empty;

        public DateTime? DogumTarihi { get; set; }

        [StringLength(100)]
        public string? Egitim { get; set; }
    }
}
