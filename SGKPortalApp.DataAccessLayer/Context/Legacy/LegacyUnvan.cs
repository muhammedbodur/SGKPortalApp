using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.DataAccessLayer.Context.Legacy
{
    [Table("unvanlar")]
    public class LegacyUnvan
    {
        [Key]
        [Column("id")]
        public int Id { get; set; } // UnvanId

        [Column("unvan")]
        [StringLength(50)]
        public string Unvan { get; set; } = string.Empty; // UnvanAdi
    }
}
