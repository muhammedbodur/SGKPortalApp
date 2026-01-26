using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    /// <summary>
    /// Departman ve HizmetBinasi arasındaki many-to-many ilişkiyi temsil eden junction tablosu.
    /// Bir binada birden fazla departman olabilir, bir departman birden fazla binada olabilir.
    /// </summary>
    public class DepartmanHizmetBinasi : AuditableEntity
    {
        [Key]
        public int DepartmanHizmetBinasiId { get; set; }

        public int DepartmanId { get; set; }
        [ForeignKey(nameof(DepartmanId))]
        public Departman? Departman { get; set; }

        public int HizmetBinasiId { get; set; }
        [ForeignKey(nameof(HizmetBinasiId))]
        public HizmetBinasi? HizmetBinasi { get; set; }

        /// <summary>
        /// Bu departmanın bu binadaki ana/varsayılan bina mı olduğunu belirtir
        /// </summary>
        public bool AnaBina { get; set; } = false;

        // Navigation Properties - Sıramatik İlişkileri
        [InverseProperty("DepartmanHizmetBinasi")]
        public ICollection<KanalIslem>? KanalIslemleri { get; set; } = new List<KanalIslem>();

        [InverseProperty("DepartmanHizmetBinasi")]
        public ICollection<KanalAltIslem>? KanalAltIslemleri { get; set; } = new List<KanalAltIslem>();

        [InverseProperty("DepartmanHizmetBinasi")]
        public ICollection<Banko>? Bankolar { get; set; } = new List<Banko>();

        [InverseProperty("DepartmanHizmetBinasi")]
        public ICollection<Tv>? Tvler { get; set; } = new List<Tv>();

        [InverseProperty("DepartmanHizmetBinasi")]
        public ICollection<Kiosk>? Kiosklar { get; set; } = new List<Kiosk>();

        [InverseProperty("DepartmanHizmetBinasi")]
        public ICollection<Sira>? Siralar { get; set; } = new List<Sira>();

        [InverseProperty("DepartmanHizmetBinasi")]
        public ICollection<BankoKullanici>? BankoKullanicilari { get; set; } = new List<BankoKullanici>();

        /// <summary>
        /// Bu departman-hizmet binası kombinasyonundaki personeller
        /// </summary>
        [InverseProperty("DepartmanHizmetBinasi")]
        public ICollection<Personel>? Personeller { get; set; } = new List<Personel>();
    }
}
