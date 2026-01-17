using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri
{
    /// <summary>
    /// İzin Sorumlusu Güncelleme Request DTO
    /// </summary>
    public class IzinSorumluUpdateDto
    {
        [Required]
        public int IzinSorumluId { get; set; }

        /// <summary>
        /// Sorumlu olunan Departman ID
        /// </summary>
        public int? DepartmanId { get; set; }

        /// <summary>
        /// Sorumlu olunan Servis ID
        /// </summary>
        public int? ServisId { get; set; }

        /// <summary>
        /// Sorumlu personelin TC Kimlik No
        /// </summary>
        [Required(ErrorMessage = "Sorumlu personel seçilmelidir")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 karakter olmalıdır")]
        public string SorumluPersonelTcKimlikNo { get; set; } = string.Empty;

        /// <summary>
        /// Onay seviyesi: 1 = Birinci Onayci, 2 = İkinci Onayci
        /// </summary>
        [Required(ErrorMessage = "Onay seviyesi seçilmelidir")]
        [Range(1, 2, ErrorMessage = "Onay seviyesi 1 veya 2 olmalıdır")]
        public int OnaySeviyes { get; set; }

        /// <summary>
        /// Aktif/Pasif durum
        /// </summary>
        public bool Aktif { get; set; } = true;

        /// <summary>
        /// Açıklama/Not
        /// </summary>
        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Aciklama { get; set; }
    }
}
