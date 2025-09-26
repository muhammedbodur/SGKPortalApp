using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri
{
    public class BankoKullanici : BaseEntity
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

        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;
        public DateTime DuzenlenmeTarihi { get; set; } = DateTime.Now;
    }
}