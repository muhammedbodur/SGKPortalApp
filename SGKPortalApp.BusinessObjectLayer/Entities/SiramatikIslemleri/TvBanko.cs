using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri
{
    /// <summary>
    /// Tv ve Banko arasındaki Many-to-Many ilişkiyi temsil eden junction table
    /// Bu sayede bir Tv birden fazla Banko'ya, bir Banko da birden fazla Tv'ye bağlanabilir
    /// </summary>
    public class TvBanko : AuditableEntity
    {
        [Key]
        public int TvBankoId { get; set; }

        // Foreign Key - Tv ile ilişki
        public int TvId { get; set; }
        [ForeignKey("TvId")]
        [InverseProperty("TvBankolar")]
        public required Tv Tv { get; set; }

        // Foreign Key - Banko ile ilişki
        public int BankoId { get; set; }
        [ForeignKey("BankoId")]
        [InverseProperty("TvBankolar")]
        public required Banko Banko { get; set; }

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}