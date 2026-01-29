using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri
{
    /// <summary>
    /// Personel aktiflik durumu hareket kaydı
    /// Her durum değişikliği bu tabloda tarihle birlikte saklanır
    /// </summary>
    public class PersonelAktiflikDurumHareket : AuditableEntity
    {
        [Key]
        public int PersonelAktiflikDurumHareketId { get; set; }

        [Required]
        [StringLength(11)]
        public string TcKimlikNo { get; set; } = string.Empty;

        [ForeignKey(nameof(TcKimlikNo))]
        [InverseProperty("PersonelAktiflikDurumHareketleri")]
        public Personel? Personel { get; set; }

        /// <summary>
        /// Önceki durum
        /// </summary>
        public PersonelAktiflikDurum? OncekiDurum { get; set; }

        /// <summary>
        /// Yeni durum
        /// </summary>
        [Required]
        public PersonelAktiflikDurum YeniDurum { get; set; }

        /// <summary>
        /// Durum değişiklik tarihi
        /// </summary>
        [Required]
        public DateTime DegisiklikTarihi { get; set; }

        /// <summary>
        /// Geçerlilik başlangıç tarihi (durum ne zamandan itibaren geçerli)
        /// </summary>
        public DateTime? GecerlilikBaslangicTarihi { get; set; }

        /// <summary>
        /// Geçerlilik bitiş tarihi (durum ne zamana kadar geçerli - varsa)
        /// </summary>
        public DateTime? GecerlilikBitisTarihi { get; set; }

        /// <summary>
        /// Durum değişikliği nedeni/açıklaması
        /// </summary>
        [StringLength(500)]
        public string? Aciklama { get; set; }

        /// <summary>
        /// İlgili belge/karar numarası
        /// </summary>
        [StringLength(100)]
        public string? BelgeNo { get; set; }
    }
}
