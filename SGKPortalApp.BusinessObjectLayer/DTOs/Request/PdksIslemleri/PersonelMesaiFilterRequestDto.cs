using System;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri
{
    /// <summary>
    /// Personel Mesai Filtreleme Request DTO
    /// </summary>
    public class PersonelMesaiFilterRequestDto
    {
        [Required]
        [StringLength(11)]
        public string TcKimlikNo { get; set; } = string.Empty;

        [Required]
        public DateTime BaslangicTarihi { get; set; }

        [Required]
        public DateTime BitisTarihi { get; set; }

        public int? SgmId { get; set; }
        public int? ServisId { get; set; }
    }
}
