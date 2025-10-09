using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common
{
    /// <summary>
    /// İl filtreleme ve arama request DTO
    /// </summary>
    public class IlFilterRequestDto
    {
        /// <summary>
        /// Arama terimi (İl adı)
        /// </summary>
        [StringLength(50)]
        public string? SearchTerm { get; set; }

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
        /// Sıralama alanı (IlAdi, EklenmeTarihi, IlceSayisi)
        /// </summary>
        [StringLength(50)]
        public string OrderBy { get; set; } = "IlAdi";

        /// <summary>
        /// Sıralama yönü (asc/desc)
        /// </summary>
        [RegularExpression("^(asc|desc)$", ErrorMessage = "Sıralama yönü 'asc' veya 'desc' olmalıdır")]
        public string OrderDirection { get; set; } = "asc";
    }
}
