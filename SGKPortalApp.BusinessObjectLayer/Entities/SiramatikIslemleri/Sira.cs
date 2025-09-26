using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri
{
    public class Sira
    {
        [Key]
        public int SiraId { get; set; }

        public int SiraNo { get; set; }

        public int KanalAltIslemId { get; set; }
        [ForeignKey("KanalAltIslemId")]
        [InverseProperty("Siralar")]
        public required KanalAltIslem KanalAltIslem { get; set; }

        public string KanalAltAdi { get; set; } = string.Empty;

        public int HizmetBinasiId { get; set; }
        [ForeignKey("HizmetBinasiId")]
        public required HizmetBinasi HizmetBinasi { get; set; }

        public string? TcKimlikNo { get; set; }
        [ForeignKey("TcKimlikNo")]
        public Personel? Personel { get; set; }

        public DateTime SiraAlisZamani { get; set; } = DateTime.Now;
        public DateTime? IslemBaslamaZamani { get; set; }
        public DateTime? IslemBitisZamani { get; set; }

        public BeklemeDurum BeklemeDurum { get; set; } = BeklemeDurum.Beklemede;

        [NotMapped]
        public DateTime SiraAlisTarihi { get; set; } = DateTime.Now.Date;
    }
}