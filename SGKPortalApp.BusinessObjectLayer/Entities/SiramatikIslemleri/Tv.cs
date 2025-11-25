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

        public string TvAdi { get; set; } = string.Empty;

        public int HizmetBinasiId { get; set; }
        [ForeignKey(nameof(HizmetBinasiId))]
        [InverseProperty("Tvler")]
        public HizmetBinasi? HizmetBinasi { get; set; }

        public KatTipi KatTipi { get; set; }
        public Aktiflik TvAktiflik { get; set; } = Aktiflik.Aktif;

        [StringLength(500)]
        public string? TvAciklama { get; set; }

        public DateTime IslemZamani { get; set; } = DateTime.Now;

        // User ile One-to-One ilişki (TV için otomatik oluşturulan kullanıcı)
        [StringLength(11)]
        public string? TcKimlikNo { get; set; }
        [ForeignKey(nameof(TcKimlikNo))]
        [InverseProperty("Tv")]
        public User? User { get; set; }

        // SignalR bağlantı bilgileri (Birden fazla kullanıcı aynı TV'yi izleyebilir)
        [InverseProperty("Tv")]
        public ICollection<HubTvConnection> HubTvConnections { get; set; } = new List<HubTvConnection>();

        [InverseProperty("Tv")]
        public ICollection<TvBanko>? TvBankolar { get; set; } = new List<TvBanko>();
    }
}