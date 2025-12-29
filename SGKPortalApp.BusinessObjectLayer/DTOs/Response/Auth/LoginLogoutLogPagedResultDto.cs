using System.Collections.Generic;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Auth
{
    public class LoginLogoutLogPagedResultDto
    {
        public List<LoginLogoutLogResponseDto> Logs { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
