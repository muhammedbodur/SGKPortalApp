using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri
{
    public class PersonelYetki : BaseEntity
    {
        [Key]
        public int PersonelYetkiId { get; set; }

        public required string TcKimlikNo { get; set; }
        [ForeignKey(nameof(TcKimlikNo))]
        [InverseProperty("PersonelYetkileri")]
        public required Personel Personel { get; set; }

        public int YetkiId { get; set; }
        [ForeignKey(nameof(YetkiId))]
        [InverseProperty("PersonelYetkileri")]
        public required Yetki Yetki { get; set; }

        public int ModulControllerIslemId { get; set; }
        [ForeignKey(nameof(ModulControllerIslemId))]
        [InverseProperty("PersonelYetkileri")]
        public required ModulControllerIslem ModulControllerIslem { get; set; }

        // Yetki Tipi
        public YetkiTipleri YetkiTipleri { get; set; }
    }
}