using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.DataAccessLayer.Context.Legacy
{
    [Table("servisler")]
    public class LegacyServis
    {
        [Key]
        [Column("servisid")]
        public int ServisId { get; set; }

        [Column("servisadi")]
        [StringLength(50)]
        public string ServisAdi { get; set; } = string.Empty;
    }
}
