using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common
{
    public class GununMenusuFilterRequestDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [StringLength(200)]
        public string? SearchTerm { get; set; }

        public Aktiflik? Aktiflik { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Sayfa numarası 1'den büyük olmalıdır")]
        public int PageNumber { get; set; } = 1;

        [Range(5, 200, ErrorMessage = "Sayfa boyutu 5-200 arasında olmalıdır")]
        public int PageSize { get; set; } = 20;

        [StringLength(50)]
        public string OrderBy { get; set; } = "Tarih";

        [RegularExpression("^(asc|desc)$", ErrorMessage = "Sıralama yönü 'asc' veya 'desc' olmalıdır")]
        public string OrderDirection { get; set; } = "desc";
    }
}
