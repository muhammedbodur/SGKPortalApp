using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Legacy
{
    [Table("servisler")]
    public class LegacyServis
    {
        [Key]
        [Column("servisid")]
        public int ServisId { get; set; }

        [Column("servisadi")]
        [StringLength(255)]
        public string ServisAdi { get; set; } = string.Empty;
    }
}
