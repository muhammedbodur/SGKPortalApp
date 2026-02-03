using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    public class PersonelYetkiCreateDto
    {
        public int DepartmanId { get; set; }

        public int ServisId { get; set; }

        [StringLength(500)]
        public string? Sebep { get; set; }

        public DateTime? BaslamaTarihi { get; set; }

        public DateTime? BitisTarihi { get; set; }
    }
}
