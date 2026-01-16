using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.PdksIslemleri
{
    /// <summary>
    /// Görevli Personel (Dışarı görevlendirme)
    /// PDKS.Net4.8: tbGorevli uyumlu
    /// </summary>
    [Table("Gorevli", Schema = "PdksIslemleri")]
    public class Gorevli : BaseEntity
    {
        [Key]
        public int GorevliId { get; set; }

        [Required]
        [StringLength(11)]
        public string TcKimlikNo { get; set; } = string.Empty;

        [ForeignKey(nameof(TcKimlikNo))]
        public Personel? Personel { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime BaslangicTarihi { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime BitisTarihi { get; set; }

        [Required]
        [StringLength(500)]
        public string GorevYeri { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Aciklama { get; set; }

        public bool Aktif { get; set; } = true;
    }
}
