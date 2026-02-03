using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    public class PersonelEngelCreateDto
    {
        public EngelDerecesi EngelDerecesi { get; set; }

        [StringLength(500)]
        public string? Neden1 { get; set; }

        [StringLength(500)]
        public string? Neden2 { get; set; }

        [StringLength(500)]
        public string? Neden3 { get; set; }
    }
}
