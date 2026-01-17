using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.PdksIslemleri
{
    /// <summary>
    /// İzin/Mazeret Talepleri için Sorumlu Atama
    /// Departman ve/veya Servis bazında izin onay sorumlusu tanımlanır
    /// İki seviye onay yapısı vardır: 1.Onayci (Yönetici) ve 2.Onayci (Üst Yönetim)
    /// </summary>
    [Table("IzinSorumlu", Schema = "PdksIslemleri")]
    public class IzinSorumlu : AuditableEntity
    {
        [Key]
        public int IzinSorumluId { get; set; }

        // ═══════════════════════════════════════════════════════
        // KAPSAM TANIMLAMA (Departman/Servis)
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Sorumlu olunan Departman ID
        /// Null ise tüm departmanlar için geçerli
        /// </summary>
        public int? DepartmanId { get; set; }

        [ForeignKey(nameof(DepartmanId))]
        public Departman? Departman { get; set; }

        /// <summary>
        /// Sorumlu olunan Servis ID
        /// Null ise departman içindeki tüm servisler için geçerli
        /// </summary>
        public int? ServisId { get; set; }

        [ForeignKey(nameof(ServisId))]
        public Servis? Servis { get; set; }

        // ═══════════════════════════════════════════════════════
        // SORUMLU PERSONEL BİLGİSİ
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Sorumlu personelin TC Kimlik No
        /// </summary>
        [Required]
        [StringLength(11)]
        public string SorumluPersonelTcKimlikNo { get; set; } = string.Empty;

        [ForeignKey(nameof(SorumluPersonelTcKimlikNo))]
        [InverseProperty("IzinSorumluluklar")]
        public Personel? SorumluPersonel { get; set; }

        /// <summary>
        /// Onay seviyesi: 1 = Birinci Onayci (Yönetici/Müdür), 2 = İkinci Onayci (Üst Yönetim)
        /// </summary>
        [Required]
        public int OnaySeviyes { get; set; } = 1;

        // ═══════════════════════════════════════════════════════
        // DURUM BİLGİSİ
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Sorumluluk aktif mi?
        /// Personel departman değiştirdiğinde eski sorumluluğu pasif yapılır
        /// </summary>
        public bool Aktif { get; set; } = true;

        /// <summary>
        /// Açıklama/Not (opsiyonel)
        /// </summary>
        [StringLength(500)]
        public string? Aciklama { get; set; }
    }
}
