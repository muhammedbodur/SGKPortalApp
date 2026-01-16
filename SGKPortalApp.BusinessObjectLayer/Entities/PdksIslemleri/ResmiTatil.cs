using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.PdksIslemleri
{
    /// <summary>
    /// Resmi Tatil Günleri
    /// PDKS.Net4.8: tbResmiTatilller uyumlu
    /// </summary>
    [Table("ResmiTatil", Schema = "PdksIslemleri")]
    public class ResmiTatil : BaseEntity
    {
        [Key]
        public int ResmiTatilId { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime Tarih { get; set; }

        [Required]
        [StringLength(200)]
        public string Aciklama { get; set; } = string.Empty;

        public bool Aktif { get; set; } = true;
    }
}
