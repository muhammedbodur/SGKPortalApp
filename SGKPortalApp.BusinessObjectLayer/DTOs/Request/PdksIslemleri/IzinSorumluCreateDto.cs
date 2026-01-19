using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri
{
    /// <summary>
    /// İzin Sorumlusu Oluşturma Request DTO
    /// </summary>
    public class IzinSorumluCreateDto
    {
        /// <summary>
        /// Sorumlu olunan Departman ID (Null ise tüm departmanlar için geçerli)
        /// </summary>
        public int? DepartmanId { get; set; }

        /// <summary>
        /// Sorumlu olunan Servis ID (Null ise departman içindeki tüm servisler için geçerli)
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
        public int OnaySeviyesi { get; set; } = 1;

        /// <summary>
        /// Açıklama/Not
        /// </summary>
        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Aciklama { get; set; }
    }
}
