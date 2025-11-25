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
        [ForeignKey(nameof(HizmetBinasiId))]
        [InverseProperty("Bankolar")]
        public required HizmetBinasi HizmetBinasi { get; set; }

        [Required]
        [Range(1, 999)]
        public int BankoNo { get; set; }

        [Required]
        public BankoTipi BankoTipi { get; set; } = BankoTipi.Normal;

        [Required]
        public KatTipi KatTipi { get; set; } = KatTipi.zemin;

        [Required]
        public Aktiflik BankoAktiflik { get; set; } = Aktiflik.Aktif;

        [StringLength(500)]
        public string? BankoAciklama { get; set; }

        public int BankoSira { get; set; } = 0;

        // Navigation Properties
        [InverseProperty("Banko")]
        public ICollection<BankoKullanici>? BankoKullanicilari { get; set; } = new List<BankoKullanici>();

        [InverseProperty("Banko")]
        public ICollection<TvBanko>? TvBankolar { get; set; } = new List<TvBanko>();

        [InverseProperty("Banko")]
        public ICollection<BankoHareket>? BankoHareketleri { get; set; } = new List<BankoHareket>();

        [InverseProperty("Banko")]
        public HubBankoConnection? HubBankoConnection { get; set; }
    }
}