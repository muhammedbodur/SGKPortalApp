using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Auth;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Auth;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Auth
{
    public interface ILoginLogoutLogService
    {
        Task<ApiResponseDto<LoginLogoutLogPagedResultDto>> GetLogsAsync(LoginLogoutLogFilterDto filter);
        Task<ApiResponseDto<LoginLogoutLogResponseDto>> GetLogByIdAsync(int id);
        Task<ApiResponseDto<int>> GetActiveSessionCountAsync();
        Task<ApiResponseDto<int>> GetTodayLoginCountAsync();

        // SessionID bazında logout tracking
        Task<ApiResponseDto<bool>> UpdateLogoutTimeBySessionIdAsync(string sessionId);

        // TcKimlikNo bazında logout tracking
        Task<ApiResponseDto<bool>> UpdateLogoutTimeByTcKimlikNoAsync(string tcKimlikNo);

        // Cleanup işlemleri (Background Service için)
        Task<ApiResponseDto<int>> CleanupOrphanSessionsAsync();
    }
}
