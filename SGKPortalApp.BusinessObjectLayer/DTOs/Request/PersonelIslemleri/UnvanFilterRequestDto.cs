using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    /// <summary>
    /// Unvan filtreleme ve arama request DTO
    /// </summary>
    public class UnvanFilterRequestDto
    {
        /// <summary>
        /// Arama terimi (Unvan adı)
        /// </summary>
        [StringLength(150)]
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Aktiflik durumu filtresi
        /// </summary>
        public Aktiflik? Aktiflik { get; set; }

        /// <summary>
        /// Sayfa numarası (Pagination)
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Sayfa numarası 1'den büyük olmalıdır")]
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Sayfa başına kayıt sayısı
        /// </summary>
        [Range(5, 100, ErrorMessage = "Sayfa boyutu 5-100 arasında olmalıdır")]
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Sıralama alanı
        /// </summary>
        [StringLength(50)]
        public string OrderBy { get; set; } = "UnvanAdi";

        /// <summary>
        /// Sıralama yönü (asc/desc)
        /// </summary>
        [RegularExpression("^(asc|desc)$", ErrorMessage = "Sıralama yönü 'asc' veya 'desc' olmalıdır")]
        public string OrderDirection { get; set; } = "asc";
    }
}