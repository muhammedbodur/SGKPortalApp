using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri
{
    public class Banko : AuditableEntity
    {
        [Key]
        public int BankoId { get; set; }

        [Required]
        public int HizmetBinasiId { get; set; }
        [ForeignKey("HizmetBinasiId")]
        [InverseProperty("Bankolar")]
        public required HizmetBinasi HizmetBinasi { get; set; }

        public int BankoNo { get; set; }
        public BankoTipi BankoTipi { get; set; }
        public KatTipi KatTipi { get; set; }
        public Aktiflik BankoAktiflik { get; set; }

        [InverseProperty("Banko")]
        public ICollection<BankoKullanici>? BankoKullanicilari { get; set; } = new List<BankoKullanici>();

        [InverseProperty("Banko")]
        public ICollection<TvBanko>? TvBankolar { get; set; } = new List<TvBanko>();
    }
}