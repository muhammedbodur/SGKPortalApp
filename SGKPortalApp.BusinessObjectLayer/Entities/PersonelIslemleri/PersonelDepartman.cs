using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri
{
    public class PersonelDepartman : AuditableEntity
    {
        [Key]
        public int PersonelDepartmanId { get; set; }

        [Required]
        [StringLength(11)]
        public string TcKimlikNo { get; set; } = string.Empty;
        [ForeignKey("TcKimlikNo")]
        public required Personel Personel { get; set; }

        [Required]
        public int DepartmanId { get; set; }
        [ForeignKey("DepartmanId")]
        public required Departman Departman { get; set; }

        public DateTime BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;

        [StringLength(500)]
        public string? Aciklama { get; set; }
    }
}