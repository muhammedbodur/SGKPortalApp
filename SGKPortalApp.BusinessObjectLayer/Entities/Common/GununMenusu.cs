using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    /// <summary>
    /// Günün Menüsü Entity
    /// Tablo adı: CMN_GununMenusu (Configuration'da tanımlanacak)
    /// </summary>
    public class GununMenusu : AuditableEntity
    {
        [Key]
        public int MenuId { get; set; }

        [Required]
        public DateTime Tarih { get; set; } = DateTime.Today;

        [Required]
        public string Icerik { get; set; } = string.Empty;

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}
