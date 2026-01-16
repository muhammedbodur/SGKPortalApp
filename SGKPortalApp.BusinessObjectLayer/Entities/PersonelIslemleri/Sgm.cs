using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri
{
    /// <summary>
    /// SGM (Sosyal Güvenlik Merkezi) - Organizasyonel Birim
    /// PDKS.Net4.8: tbSgm uyumlu
    /// </summary>
    [Table("Sgm", Schema = "PersonelIslemleri")]
    public class Sgm : BaseEntity
    {
        [Key]
        public int SgmId { get; set; }

        [Required]
        [StringLength(200)]
        public string SgmAdi { get; set; } = string.Empty;

        [StringLength(100)]
        public string? SgmKodu { get; set; }

        public bool Aktif { get; set; } = true;

        // Navigation Properties
        [InverseProperty("Sgm")]
        public ICollection<Departman>? Departmanlar { get; set; } = new List<Departman>();

        [InverseProperty("Sgm")]
        public ICollection<Servis>? Servisler { get; set; } = new List<Servis>();
    }
}
