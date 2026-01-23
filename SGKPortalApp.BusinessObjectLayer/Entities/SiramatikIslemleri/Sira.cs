using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri
{
    public class Sira : AuditableEntity
    {
        [Key]
        public int SiraId { get; set; }

        public int SiraNo { get; set; }

        public int KanalAltIslemId { get; set; }
        [ForeignKey("KanalAltIslemId")]
        [InverseProperty("Siralar")]
        public required KanalAltIslem KanalAltIslem { get; set; }

        public string KanalAltAdi { get; set; } = string.Empty;

        /// <summary>
        /// Departman-Bina kombinasyonu (Junction table referansı)
        /// Sıra doğru departman-bina kombinasyonunda alınmalı
        /// </summary>
        public int DepartmanHizmetBinasiId { get; set; }
        [ForeignKey(nameof(DepartmanHizmetBinasiId))]
        public required DepartmanHizmetBinasi DepartmanHizmetBinasi { get; set; }

        public string? TcKimlikNo { get; set; }
        [ForeignKey("TcKimlikNo")]
        public Personel? Personel { get; set; }

        public DateTime SiraAlisZamani { get; set; } = DateTime.Now;
        public DateTime? IslemBaslamaZamani { get; set; }
        public DateTime? IslemBitisZamani { get; set; }

        public BeklemeDurum BeklemeDurum { get; set; } = BeklemeDurum.Beklemede;

        [NotMapped]
        public DateTime SiraAlisTarihi { get; set; } = DateTime.Now.Date;

        // ═══════════════════════════════════════════════════════
        // YÖNLENDİRME BİLGİLERİ
        // ═══════════════════════════════════════════════════════

        public bool YonlendirildiMi { get; set; } = false;

        public int? YonlendirenBankoId { get; set; }
        [ForeignKey("YonlendirenBankoId")]
        public Banko? YonlendirenBanko { get; set; }

        [StringLength(11)]
        public string? YonlendirenPersonelTc { get; set; }
        [ForeignKey("YonlendirenPersonelTc")]
        public Personel? YonlendirenPersonel { get; set; }

        public int? HedefBankoId { get; set; }
        [ForeignKey("HedefBankoId")]
        public Banko? HedefBanko { get; set; }

        public DateTime? YonlendirmeZamani { get; set; }

        [StringLength(500)]
        public string? YonlendirmeNedeni { get; set; }

        public YonlendirmeTipi? YonlendirmeTipi { get; set; }

        // Navigation Property
        [InverseProperty("Sira")]
        public ICollection<BankoHareket>? BankoHareketleri { get; set; } = new List<BankoHareket>();
    }
}