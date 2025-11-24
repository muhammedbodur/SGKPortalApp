using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    [Table("COM_SikKullanilanProgramlar")]
    public class SikKullanilanProgram : AuditableEntity
    {
        [Key]
        public int ProgramId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProgramAdi { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Url { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string IkonClass { get; set; } = "bx-desktop";

        [Required]
        [StringLength(20)]
        public string RenkKodu { get; set; } = "primary";

        [Required]
        public int Sira { get; set; }

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}
