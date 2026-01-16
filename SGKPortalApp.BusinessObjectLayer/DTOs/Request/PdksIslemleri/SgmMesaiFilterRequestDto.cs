using System;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri
{
    /// <summary>
    /// SGM Mesai Raporu Filtre Request DTO
    /// </summary>
    public class SgmMesaiFilterRequestDto
    {
        [Required]
        public int SgmId { get; set; }

        public int? ServisId { get; set; }

        [Required]
        public DateTime BaslangicTarihi { get; set; }

        [Required]
        public DateTime BitisTarihi { get; set; }
    }
}
