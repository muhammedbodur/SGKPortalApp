using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Legacy
{
    [Table("unvanlar")]
    public class LegacyUnvan
    {
        [Key]
        [Column("id")]
        public int Id { get; set; } // UnvanId

        [Column("unvan")]
        [StringLength(150)]
        public string Unvan { get; set; } = string.Empty; // UnvanAdi
    }
}
