using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri
{
    public class Tv : AuditableEntity
    {
        [Key]
        public int TvId { get; set; }

        public int HizmetBinasiId { get; set; }
        [ForeignKey(nameof(HizmetBinasiId))]
        [InverseProperty("Tvler")]
        public required HizmetBinasi HizmetBinasi { get; set; }

        public KatTipi KatTipi { get; set; }
        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;

        [StringLength(500)]
        public string? Aciklama { get; set; }

        public DateTime IslemZamani { get; set; } = DateTime.Now;
        public HubTvConnection? HubTvConnection { get; set; }

        [InverseProperty("Tv")]
        public ICollection<TvBanko>? TvBankolar { get; set; } = new List<TvBanko>();
    }
}