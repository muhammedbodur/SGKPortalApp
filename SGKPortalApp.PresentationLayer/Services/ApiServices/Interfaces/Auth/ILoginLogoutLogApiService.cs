using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Auth;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Auth;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Auth
{
    public interface ILoginLogoutLogApiService
    {
        Task<LoginLogoutLogPagedResultDto?> GetLogsAsync(LoginLogoutLogFilterDto filter);
        Task<LoginLogoutLogResponseDto?> GetLogByIdAsync(int id);
        Task<int> GetActiveSessionCountAsync();
        Task<int> GetTodayLoginCountAsync();
    }
}
