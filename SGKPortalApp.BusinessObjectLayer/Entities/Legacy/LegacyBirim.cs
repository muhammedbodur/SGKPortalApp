using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Legacy
{
    [Table("birim")]
    public class LegacyBirim
    {
        [Key]
        [Column("KOD")]
        public int Kod { get; set; } // DepartmanId

        [Column("BIRIMAD")]
        [StringLength(55)]
        public string BirimAd { get; set; } = string.Empty; // DepartmanAdi

        [Column("KISAAD")]
        [StringLength(25)]
        public string? KisaAd { get; set; } // DepartmanAdiKisa

        [Column("CALISMA_BASLANGIC")]
        public TimeSpan? CalismaBaslangic { get; set; }

        [Column("CALISMA_BITIS")]
        public TimeSpan? CalismaBitis { get; set; }

        [Column("BAGLI_ILCELER")]
        [StringLength(100)]
        public string? BagliIlceler { get; set; }

        [Column("BAGLI_NUFUS")]
        [StringLength(9)]
        public string? BagliNufus { get; set; }
    }
}
