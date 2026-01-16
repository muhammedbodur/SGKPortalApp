using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using System;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri
{
    /// <summary>
    /// Devamsızlık/Mazeret Oluşturma Request DTO
    /// </summary>
    public class DevamsizlikCreateDto
    {
        [Required]
        [StringLength(11)]
        public string TcKimlikNo { get; set; } = string.Empty;

        [Required]
        public IzinMazeretTuru Turu { get; set; }

        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public DateTime? MazeretTarihi { get; set; }

        public SaatDilimi? SaatDilimi { get; set; }

        [StringLength(500)]
        public string? Aciklama { get; set; }

        public int? OnaylayanSicilNo { get; set; }
    }
}
