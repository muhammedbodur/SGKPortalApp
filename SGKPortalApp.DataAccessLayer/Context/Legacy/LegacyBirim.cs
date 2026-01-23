using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.DataAccessLayer.Context.Legacy
{
    [Table("birim")]
    public class LegacyBirim
    {
        [Key]
        [Column("KOD")]
        public int Kod { get; set; } // DepartmanId

        [Column("BIRIMAD")]
        [StringLength(50)]
        public string BirimAd { get; set; } = string.Empty; // DepartmanAdi

        [Column("KISAAD")]
        [StringLength(10)]
        public string? KisaAd { get; set; } // DepartmanAdiKisa

        [Column("CALISMA_BASLANGIC")]
        public TimeSpan? CalismaBaslangic { get; set; }

        [Column("CALISMA_BITIS")]
        public TimeSpan? CalismaBitis { get; set; }

        [Column("BAGLI_ILCELER")]
        [StringLength(200)]
        public string? BagliIlceler { get; set; }

        [Column("BAGLI_NUFUS")]
        public int? BagliNufus { get; set; }
    }
}
