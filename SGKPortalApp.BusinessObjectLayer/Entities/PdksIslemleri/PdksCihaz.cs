using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.PdksIslemleri
{
    public class PdksCihaz : BaseEntity
    {
        [Key]
        public int PdksCihazId { get; set; }

        [Required]
        public int DepartmanId { get; set; }

        [ForeignKey("DepartmanId")]
        public Departman Departman { get; set; } = null!;

        [Required]
        public string CihazIP { get; set; } = string.Empty;

        public int CihazPort { get; set; } = 4370;
        public int Durum { get; set; } = 0;
        public string? Aciklama { get; set; }
        public int Aktiflik { get; set; } = 1;
        public DateTime IslemZamani { get; set; } = DateTime.Now;
        public int IslemSayisi { get; set; } = 0;
        public int IslemBasari { get; set; } = 0;
        public string? IslemDurum { get; set; }
        public DateTime KontrolZamani { get; set; } = DateTime.Now;
        public int KontrolSayisi { get; set; } = 0;
        public int KontrolBasari { get; set; } = 0;
        public string? KontrolDurum { get; set; }
    }
}
