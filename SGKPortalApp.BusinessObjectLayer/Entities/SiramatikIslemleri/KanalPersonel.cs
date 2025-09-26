using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
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
        public required Personel Personel { get; set; }

        public int KanalAltIslemId { get; set; }
        [ForeignKey("KanalAltIslemId")]
        [InverseProperty("KanalPersonelleri")]
        public required KanalAltIslem KanalAltIslem { get; set; }

        public PersonelUzmanlik Uzmanlik { get; set; }
        public Aktiflik KanalAltIslemPersonelAktiflik { get; set; } = Aktiflik.Aktif;
    }
}