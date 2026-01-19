using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.PdksIslemleri
{
    /// <summary>
    /// İzin ve Mazeret talepleri - Tek tabloda birleştirilmiş yapı
    /// İzin: Planlı izinler (Yıllık, Ücretsiz vb.) - Tarih aralığı
    /// Mazeret: Geriye dönük mazeretler (Geç kalma, Erken çıkış vb.) - Saatlik
    /// Tablo adı: IzinMazeretTalep (Configuration'da tanımlanacak)
    /// </summary>
    public class IzinMazeretTalep : AuditableEntity
    {
        [Key]
        public int IzinMazeretTalepId { get; set; }

        // ═══════════════════════════════════════════════════════
        // PERSONEL İLİŞKİSİ
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Talep sahibi personel TC Kimlik No
        /// </summary>
        [Required]
        [StringLength(11)]
        public string TcKimlikNo { get; set; } = string.Empty;

        [ForeignKey(nameof(TcKimlikNo))]
        [InverseProperty("IzinMazeretTalepleri")]
        public Personel? Personel { get; set; }

        // ═══════════════════════════════════════════════════════
        // TALEP BİLGİLERİ
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// İzin/Mazeret türü (Yıllık, Mazeret, Hastalık vb.)
        /// </summary>
        [Required]
        public IzinMazeretTuru Turu { get; set; }

        /// <summary>
        /// Açıklama/Sebep
        /// </summary>
        [StringLength(500)]
        public string? Aciklama { get; set; }

        // ═══════════════════════════════════════════════════════
        // İZİN (PLANLI) - Tarih Aralığı
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Başlangıç tarihi (İzin için zorunlu, Mazeret için null)
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? BaslangicTarihi { get; set; }

        /// <summary>
        /// Bitiş tarihi (İzin için zorunlu, Mazeret için null)
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? BitisTarihi { get; set; }

        /// <summary>
        /// Toplam gün sayısı (hesaplanmış, İzin için)
        /// </summary>
        public int? ToplamGun { get; set; }

        // ═══════════════════════════════════════════════════════
        // MAZERET (GERİYE DÖNÜK) - Saatlik
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Mazeret tarihi (Mazeret için zorunlu, İzin için null)
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? MazeretTarihi { get; set; }

        /// <summary>
        /// Saat dilimi (ör: "08:00-09:00", Mazeret için)
        /// </summary>
        [StringLength(50)]
        public string? SaatDilimi { get; set; }

        // ═══════════════════════════════════════════════════════
        // BİRİNCİ ONAYCI (YÖNETİCİ/MÜDÜR)
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// 1. Onayci TC Kimlik No (Yönetici/Müdür)
        /// </summary>
        [StringLength(11)]
        public string? BirinciOnayciTcKimlikNo { get; set; }

        /// <summary>
        /// 1. Onay durumu
        /// </summary>
        public OnayDurumu BirinciOnayDurumu { get; set; } = OnayDurumu.Beklemede;

        /// <summary>
        /// 1. Onay tarihi
        /// </summary>
        public DateTime? BirinciOnayTarihi { get; set; }

        /// <summary>
        /// 1. Onayci açıklaması (Red/Onay nedeni)
        /// </summary>
        [StringLength(500)]
        public string? BirinciOnayAciklama { get; set; }

        // ═══════════════════════════════════════════════════════
        // İKİNCİ ONAYCI (ÜST YÖNETİM)
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// 2. Onayci TC Kimlik No (Üst Yönetim)
        /// </summary>
        [StringLength(11)]
        public string? IkinciOnayciTcKimlikNo { get; set; }

        /// <summary>
        /// 2. Onay durumu
        /// </summary>
        public OnayDurumu IkinciOnayDurumu { get; set; } = OnayDurumu.Beklemede;

        /// <summary>
        /// 2. Onay tarihi
        /// </summary>
        public DateTime? IkinciOnayTarihi { get; set; }

        /// <summary>
        /// 2. Onayci açıklaması (Red/Onay nedeni)
        /// </summary>
        [StringLength(500)]
        public string? IkinciOnayAciklama { get; set; }

        // ═══════════════════════════════════════════════════════
        // EK BİLGİLER
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Talep oluşturma tarihi (default: şu an)
        /// </summary>
        public DateTime TalepTarihi { get; set; } = DateTime.Now;

        /// <summary>
        /// Talep aktif mi? (İptal edilmiş talepler için false)
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Belge eki (ör: Rapor, Evlilik cüzdanı vb.)
        /// Dosya yolu veya base64 string
        /// </summary>
        [StringLength(1000)]
        public string? BelgeEki { get; set; }
    }
}
