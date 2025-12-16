using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri
{
    /// <summary>
    /// Personel bazlı yetki ataması - Her personel için ModulControllerIslem bazında yetki seviyesi
    /// </summary>
    public class PersonelYetki : AuditableEntity
    {
        [Key]
        public int PersonelYetkiId { get; set; }

        /// <summary>
        /// Personel TC Kimlik No
        /// </summary>
        public required string TcKimlikNo { get; set; }

        [ForeignKey(nameof(TcKimlikNo))]
        [InverseProperty("PersonelYetkileri")]
        public required Personel Personel { get; set; }

        /// <summary>
        /// Yetki tanımı (ModulControllerIslem) - Hangi işlem için yetki verildiği
        /// </summary>
        public int ModulControllerIslemId { get; set; }

        [ForeignKey(nameof(ModulControllerIslemId))]
        [InverseProperty("PersonelYetkileri")]
        public required ModulControllerIslem ModulControllerIslem { get; set; }

        /// <summary>
        /// Yetki seviyesi - None (erişim yok), View (görüntüle), Edit (düzenle)
        /// </summary>
        public YetkiSeviyesi YetkiSeviyesi { get; set; } = YetkiSeviyesi.None;
    }
}