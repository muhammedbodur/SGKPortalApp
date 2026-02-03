using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    public class PersonelHizmetCreateDto
    {
        [StringLength(200)]
        public string? Departman { get; set; }

        [StringLength(200)]
        public string? Servis { get; set; }

        public DateTime? BaslamaTarihi { get; set; }

        public DateTime? AyrilmaTarihi { get; set; }

        [StringLength(500)]
        public string? Sebep { get; set; }
    }
}
