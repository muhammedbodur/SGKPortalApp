using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri
{
    public class KanalPersonel : AuditableEntity
    {
        [Key]
        public int KanalPersonelId { get; set; }

        public string TcKimlikNo { get; set; } = string.Empty;
        [ForeignKey("TcKimlikNo")]
        [InverseProperty("KanalPersonelleri")]
        public Personel? Personel { get; set; }

        public int KanalAltIslemId { get; set; }
        [ForeignKey("KanalAltIslemId")]
        [InverseProperty("KanalPersonelleri")]
        public KanalAltIslem? KanalAltIslem { get; set; }

        public PersonelUzmanlik Uzmanlik { get; set; }
        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}