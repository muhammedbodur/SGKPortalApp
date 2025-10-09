using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common
{
    /// <summary>
    /// İlçe filtreleme ve arama request DTO
    /// </summary>
    public class IlceFilterRequestDto
    {
        /// <summary>
        /// Arama terimi (İlçe adı)
        /// </summary>
        [StringLength(50)]
        public string? SearchTerm { get; set; }

        /// <summary>
        /// İl filtresi
        /// </summary>
        public int? IlId { get; set; }

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
        /// Sıralama alanı (IlceAdi, IlAdi, EklenmeTarihi)
        /// </summary>
        [StringLength(50)]
        public string OrderBy { get; set; } = "IlceAdi";

        /// <summary>
        /// Sıralama yönü (asc/desc)
        /// </summary>
        [RegularExpression("^(asc|desc)$", ErrorMessage = "Sıralama yönü 'asc' veya 'desc' olmalıdır")]
        public string OrderDirection { get; set; } = "asc";
    }
}
