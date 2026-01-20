using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.PdksIslemleri
{
    /// <summary>
    /// İzin ve Mazeret türü tanımları
    /// Veritabanında izin türlerini yönetmek için kullanılır
    /// </summary>
    [Table("PDKS_IzinMazeretTuruTanim")]
    public class IzinMazeretTuruTanim : AuditableEntity
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        [Key]
        public int IzinMazeretTuruId { get; set; }

        /// <summary>
        /// İzin/Mazeret türü adı
        /// Örn: "Yıllık İzin", "Mazeret", "Saatlik İzin"
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string TuruAdi { get; set; } = string.Empty;

        /// <summary>
        /// Kısa kod (opsiyonel)
        /// Örn: "YILLIK", "MAZERET", "SAATLIK"
        /// </summary>
        [MaxLength(50)]
        public string? KisaKod { get; set; }

        /// <summary>
        /// Açıklama
        /// </summary>
        [MaxLength(500)]
        public string? Aciklama { get; set; }

        /// <summary>
        /// 1. Onaycı gerekli mi?
        /// </summary>
        [Required]
        public bool BirinciOnayciGerekli { get; set; } = true;

        /// <summary>
        /// 2. Onaycı gerekli mi?
        /// </summary>
        [Required]
        public bool IkinciOnayciGerekli { get; set; } = true;

        /// <summary>
        /// Planlı izin mi? (Başlangıç-Bitiş tarihi var)
        /// false ise Mazeret (Tek tarih + Saat dilimi)
        /// </summary>
        [Required]
        public bool PlanliIzinMi { get; set; } = true;

        /// <summary>
        /// Sıralama
        /// </summary>
        [Required]
        public int Sira { get; set; } = 0;

        /// <summary>
        /// Aktif mi?
        /// </summary>
        [Required]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Renk kodu (UI'da gösterim için)
        /// Örn: "#007bff", "#28a745"
        /// </summary>
        [MaxLength(20)]
        public string? RenkKodu { get; set; }
    }
}
