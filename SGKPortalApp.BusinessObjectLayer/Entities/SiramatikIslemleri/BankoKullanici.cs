using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri
{
    public class BankoKullanici : AuditableEntity
    {
        [Key]
        public int BankoKullaniciId { get; set; }

        public int BankoId { get; set; }
        [ForeignKey("BankoId")]
        [InverseProperty("BankoKullanicilari")]
        public required Banko Banko { get; set; }

        public string TcKimlikNo { get; set; } = string.Empty;
        [ForeignKey("TcKimlikNo")]
        [InverseProperty("BankoKullanicilari")]
        public required Personel Personel { get; set; }

        // ⭐ YENİ: HizmetBinasi referansı eklendi
        // Bu sayede personel, banko ve hizmet binası tutarlılığı database seviyesinde garanti edilir
        [Required]
        public int HizmetBinasiId { get; set; }
        [ForeignKey("HizmetBinasiId")]
        public required HizmetBinasi HizmetBinasi { get; set; }

        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;
        public DateTime DuzenlenmeTarihi { get; set; } = DateTime.Now;
    }
}