using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri
{
    /// <summary>
    /// Personel Aktif Durum Güncelleme Request DTO
    /// </summary>
    public class PersonelAktifDurumUpdateDto
    {
        [Required]
        [StringLength(11)]
        public string TcKimlikNo { get; set; } = string.Empty;

        [Required]
        public bool Aktif { get; set; }
    }
}
