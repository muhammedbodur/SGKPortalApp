using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri
{
    public class KanalIslem : AuditableEntity
    {
        [Key]
        public int KanalIslemId { get; set; }

        public int KanalId { get; set; }
        [ForeignKey("KanalId")]
        [InverseProperty("KanalIslemleri")]
        public required Kanal Kanal { get; set; }

        [Required]
        [StringLength(100)]
        public string KanalIslemAdi { get; set; } = string.Empty;

        public int Sira { get; set; }

        public int HizmetBinasiId { get; set; }
        [ForeignKey("HizmetBinasiId")]
        public required HizmetBinasi HizmetBinasi { get; set; }

        [Range(0, 9999, ErrorMessage = "BaslangicNumara 0 ile 9999 arasında olmalıdır.")]
        public int BaslangicNumara { get; set; }

        [Range(0, 9999, ErrorMessage = "BitisNumara 0 ile 9999 arasında olmalıdır.")]
        public int BitisNumara { get; set; }

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;

        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;
        public DateTime DuzenlenmeTarihi { get; set; } = DateTime.Now;

        [InverseProperty("KanalIslem")]
        public ICollection<KanalAltIslem>? KanalAltIslemleri { get; set; } = new List<KanalAltIslem>();
    }
}