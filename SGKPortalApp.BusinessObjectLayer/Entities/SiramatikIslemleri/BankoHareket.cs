using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri
{
    public class BankoHareket : AuditableEntity
    {
        [Key]
        public long BankoHareketId { get; set; }  // bigint - çok büyüyecek

        // ═══════════════════════════════════════════════════════
        // BANKO BİLGİSİ
        // ═══════════════════════════════════════════════════════

        [Required]
        public int BankoId { get; set; }
        [ForeignKey("BankoId")]
        [InverseProperty("BankoHareketleri")]
        public required Banko Banko { get; set; }

        // ═══════════════════════════════════════════════════════
        // PERSONEL BİLGİSİ (Kim çağırdı/işlem yaptı)
        // ═══════════════════════════════════════════════════════

        [Required]
        [StringLength(11)]
        public string PersonelTcKimlikNo { get; set; } = string.Empty;
        [ForeignKey("PersonelTcKimlikNo")]
        public required Personel Personel { get; set; }

        // ═══════════════════════════════════════════════════════
        // SIRA BİLGİSİ
        // ═══════════════════════════════════════════════════════

        [Required]
        public int SiraId { get; set; }
        [ForeignKey("SiraId")]
        public required Sira Sira { get; set; }

        [Required]
        public int SiraNo { get; set; }  // Vatandaşın gördüğü numara (1105, 2045 vb)

        // ═══════════════════════════════════════════════════════
        // İŞLEM BİLGİSİ (Hangi kanal/alt kanal işlemi - Bina Bazlı)
        // ═══════════════════════════════════════════════════════

        [Required]
        public int KanalIslemId { get; set; }
        [ForeignKey("KanalIslemId")]
        public required KanalIslem KanalIslem { get; set; }

        [Required]
        public int KanalAltIslemId { get; set; }
        [ForeignKey("KanalAltIslemId")]
        public required KanalAltIslem KanalAltIslem { get; set; }

        // ═══════════════════════════════════════════════════════
        // ZAMAN BİLGİLERİ
        // ═══════════════════════════════════════════════════════

        [Required]
        public DateTime IslemBaslamaZamani { get; set; }

        public DateTime? IslemBitisZamani { get; set; }  // NULL = hala işlemde

        // ═══════════════════════════════════════════════════════
        // HESAPLANAN ALAN (Raporlama için)
        // ═══════════════════════════════════════════════════════

        public int? IslemSuresiSaniye { get; set; }  // IslemBitis - IslemBaslama
    }
}
