using System.Collections.Generic;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    public class HaberListeResponseDto
    {
        public List<HaberResponseDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
    }
}
