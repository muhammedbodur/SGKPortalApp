using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    /// <summary>
    /// Haber Görselleri Entity
    /// Bir habere ait birden fazla görseli saklar
    /// </summary>
    public class HaberGorsel : AuditableEntity
    {
        [Key]
        public int HaberGorselId { get; set; }

        [Required]
        public int HaberId { get; set; }

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
        [ForeignKey("HaberId")]
        public virtual Haber? Haber { get; set; }
    }
}
