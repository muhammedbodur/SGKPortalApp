using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    public class PersonelCezaCreateDto
    {
        [StringLength(500)]
        public string? Sebep { get; set; }

        [StringLength(200)]
        public string? AltBendi { get; set; }

        public DateTime? CezaTarihi { get; set; }
    }
}
