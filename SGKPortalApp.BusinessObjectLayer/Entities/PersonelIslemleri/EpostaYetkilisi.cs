using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri
{
    /// <summary>
    /// Eposta Yetkilisi - Özel yetki tanımları
    /// PDKS.Net4.8: tbEpostaYetkilisi uyumlu
    /// Tip: "st" = Sosyal Tesis, "eshot" = ESHOT Kart, "ik" = İK Yetkisi
    /// </summary>
    [Table("EpostaYetkilisi", Schema = "PersonelIslemleri")]
    public class EpostaYetkilisi : BaseEntity
    {
        [Key]
        public int EpostaYetkilisiId { get; set; }

        [Required]
        public int SicilNo { get; set; }

        [ForeignKey(nameof(SicilNo))]
        public Personel? Personel { get; set; }

        [Required]
        [StringLength(50)]
        public string Tip { get; set; } = string.Empty; // st, eshot, ik

        public bool Aktif { get; set; } = true;
    }
}
