using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri
{
    public class BankoIslem : AuditableEntity
    {
        /*
         Bu tablo admin tarafından girilerek oluşturulacak, AnaGrup ise direkt devam edilecek, 
         OrtaGrup ise Anagrup a bağlanacak, AltGrup ise OrtaGrup a bağlanacak ve bu şekilde bir yapı oluşturulacak
         Yapı olarak yetkiler yapısına benzeyecek.
         AnaGrup taki satırlar Kioskta ilk görünecekler, OrtaGrup takiler seçilen AnaGrup a göre gelecek, AltGrup takiler
         ise yine seçilen OrtaGrup a göre gelecek.
        */
        [Key]
        public int BankoIslemId { get; set; }

        /* AnaGrup, OrtaGrup, AltGrup olarak 3 farklı grup verilebilir, BankoGrupları ndan beslenir. */
        [Required]
        public BankoGrup BankoGrup { get; set; }

        /* AnaGrup değilse UstIslemId yi eklemek mecburi(-1 demek AnaGrup demektir, kiosk ta ilk görünür), 
        bu veri yine seviyesine göre eklenecek */
        public int BankoUstIslemId { get; set; } = -1;

        [Required]
        [StringLength(200)]
        public string? BankoIslemAdi { get; set; }

        public int BankoIslemSira { get; set; }

        public Aktiflik BankoIslemAktiflik { get; set; }

        [StringLength(10)]
        public string? DiffLang { get; set; }
    }
}