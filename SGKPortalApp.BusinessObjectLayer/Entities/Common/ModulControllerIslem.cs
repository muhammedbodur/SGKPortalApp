using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    /// <summary>
    /// Modül-Controller-İşlem hiyerarşisinde en granular seviye.
    /// Sayfa, alan, buton veya bölüm seviyesinde yetki tanımı yapılabilir.
    /// </summary>
    public class ModulControllerIslem : AuditableEntity
    {
        [Key]
        public int ModulControllerIslemId { get; set; }

        /// <summary>
        /// İşlem adı (örn: LIST, MANAGE, DELETE, FIELD_EMAIL, ACTION_SAVE)
        /// </summary>
        [Required]
        [StringLength(100)]
        public required string ModulControllerIslemAdi { get; set; }

        /// <summary>
        /// İşlem tipi: Grup, Page, Tab, Buton, Field, Input
        /// </summary>
        public YetkiIslemTipi IslemTipi { get; set; } = YetkiIslemTipi.Page;

        /// <summary>
        /// Sayfa route pattern'i (sadece IslemTipi=Page için)
        /// Örn: /personel, /personel/manage, /siramatik/banko/list
        /// </summary>
        [StringLength(200)]
        public string? Route { get; set; }

        /// <summary>
        /// Permission key - Benzersiz yetki anahtarı
        /// Örn: PER.PERSONEL.LIST, PER.PERSONEL.MANAGE.FIELD.EMAIL, PER.PERSONEL.MANAGE.ACTION.DELETE
        /// </summary>
        [Required]
        [StringLength(200)]
        public string PermissionKey { get; set; } = string.Empty;

        /// <summary>
        /// Bu işlem için gereken minimum yetki seviyesi
        /// </summary>
        public YetkiSeviyesi MinYetkiSeviyesi { get; set; } = YetkiSeviyesi.View;

        /// <summary>
        /// Sayfa tipi: Public (login gerekmez), Authenticated (login yeterli), Protected (yetki gerekli)
        /// Sadece IslemTipi=Page için anlamlı
        /// </summary>
        public SayfaTipi SayfaTipi { get; set; } = SayfaTipi.Protected;

        /// <summary>
        /// Üst işlem ID'si - Hiyerarşik yapı için (örn: MANAGE sayfasının altındaki FIELD_EMAIL)
        /// </summary>
        public int? UstIslemId { get; set; }

        [ForeignKey("UstIslemId")]
        [InverseProperty("AltIslemler")]
        public ModulControllerIslem? UstIslem { get; set; }

        [InverseProperty("UstIslem")]
        public ICollection<ModulControllerIslem> AltIslemler { get; set; } = new List<ModulControllerIslem>();

        /// <summary>
        /// Açıklama - UI'da tooltip olarak gösterilebilir
        /// </summary>
        [StringLength(500)]
        public string? Aciklama { get; set; }

        public int ModulControllerId { get; set; }
        [ForeignKey("ModulControllerId")]
        [InverseProperty("ModulControllerIslemler")]
        public ModulController? ModulController { get; set; }

        /// <summary>
        /// Bu işlem için atanan personel yetkileri
        /// </summary>
        [InverseProperty("ModulControllerIslem")]
        public ICollection<PersonelYetki> PersonelYetkileri { get; set; } = new List<PersonelYetki>();
    }
}