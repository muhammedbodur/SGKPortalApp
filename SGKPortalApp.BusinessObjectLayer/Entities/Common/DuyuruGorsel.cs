using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    /// <summary>
    /// Duyuru Görselleri Entity
    /// Bir duyuruya ait birden fazla görseli saklar
    /// </summary>
    public class DuyuruGorsel : AuditableEntity
    {
        [Key]
        public int DuyuruGorselId { get; set; }

        [Required]
        public int DuyuruId { get; set; }

        [Required]
        [StringLength(500)]
        public string GorselUrl { get; set; } = string.Empty;

        /// <summary>
        /// Görsel sırası (1 = vitrin fotoğrafı)
        /// </summary>
        [Required]
        public int Sira { get; set; } = 1;

        /// <summary>
        /// Vitrin fotoğrafı mı? (Slider'da gösterilecek)
        /// </summary>
        public bool VitrinFoto { get; set; } = false;

        [StringLength(200)]
        public string? Aciklama { get; set; }

        // Navigation property
        [ForeignKey("DuyuruId")]
        public virtual Duyuru? Duyuru { get; set; }
    }
}
