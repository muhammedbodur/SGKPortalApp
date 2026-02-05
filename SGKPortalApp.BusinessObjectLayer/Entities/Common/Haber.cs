using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    /// <summary>
    /// Haber Entity
    /// Tablo adı: CMN_Haberler (Configuration'da tanımlanacak)
    /// </summary>
    public class Haber : AuditableEntity
    {
        [Key]
        public int HaberId { get; set; }

        [Required]
        [StringLength(200)]
        public string Baslik { get; set; } = string.Empty;

        [Required]
        public string Icerik { get; set; } = string.Empty;

        [StringLength(500)]
        public string? GorselUrl { get; set; }

        [Required]
        public int Sira { get; set; }

        [Required]
        public DateTime YayinTarihi { get; set; } = DateTime.Now;

        public DateTime? BitisTarihi { get; set; }

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}
