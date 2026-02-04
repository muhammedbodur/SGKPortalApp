using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    /// <summary>
    /// Duyuru (Haber) için çok resim desteği
    /// </summary>
    public class DuyuruResim : AuditableEntity
    {
        [Key]
        public int DuyuruResimId { get; set; }

        [Required]
        public int DuyuruId { get; set; }

        [Required]
        [StringLength(500)]
        public string ResimUrl { get; set; } = string.Empty;

        /// <summary>
        /// Vitrin (slider) fotoğrafı mı? Dashboard slider'da bu gösterilir.
        /// </summary>
        public bool IsVitrin { get; set; } = false;

        /// <summary>
        /// Görsellerin sırası
        /// </summary>
        public int Sira { get; set; } = 1;

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;

        // Navigation
        [ForeignKey(nameof(DuyuruId))]
        public Duyuru? Duyuru { get; set; }
    }
}
