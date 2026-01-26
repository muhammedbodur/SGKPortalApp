using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Legacy
{
    [Table("atanma")]
    public class LegacyAtanma
    {
        [Key]
        [Column("atanma_id")]
        public int AtanmaId { get; set; }

        [Column("atanma_nedeni")]
        [StringLength(50)]
        public string? AtanmaNedeni { get; set; }
    }
}
