using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    public class PersonelEgitimCreateDto
    {
        [StringLength(200)]
        public string? EgitimAdi { get; set; }

        public DateTime? BaslangicTarihi { get; set; }

        public DateTime? BitisTarihi { get; set; }
    }
}
