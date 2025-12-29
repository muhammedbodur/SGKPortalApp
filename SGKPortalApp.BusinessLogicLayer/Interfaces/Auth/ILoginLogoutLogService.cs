using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Auth;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Auth;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Auth
{
    public interface ILoginLogoutLogService
    {
        Task<ApiResponseDto<LoginLogoutLogPagedResultDto>> GetLogsAsync(LoginLogoutLogFilterDto filter);
        Task<ApiResponseDto<LoginLogoutLogResponseDto>> GetLogByIdAsync(int id);
        Task<ApiResponseDto<int>> GetActiveSessionCountAsync();
        Task<ApiResponseDto<int>> GetTodayLoginCountAsync();
    }
}
