using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Services.Auth.Interfaces;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Auth;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Auth;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Auth;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Auth
{
    public class LoginLogoutLogApiService : ILoginLogoutLogApiService
    {
        private readonly ILoginLogoutLogService _loginLogoutLogService;
        private readonly IToastService _toastService;
        private readonly ILogger<LoginLogoutLogApiService> _logger;

        public LoginLogoutLogApiService(
            ILoginLogoutLogService loginLogoutLogService,
            IToastService toastService,
            ILogger<LoginLogoutLogApiService> logger)
        {
            _loginLogoutLogService = loginLogoutLogService;
            _toastService = toastService;
            _logger = logger;
        }

        public async Task<LoginLogoutLogPagedResultDto?> GetLogsAsync(LoginLogoutLogFilterDto filter)
        {
            try
            {
                var result = await _loginLogoutLogService.GetLogsAsync(filter);
                if (!result.Success)
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Log'lar yüklenirken hata oluştu");
                    return null;
                }

                return result.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetLogsAsync failed");
                await _toastService.ShowErrorAsync("Log'lar yüklenirken bir hata oluştu");
                return null;
            }
        }

        public async Task<LoginLogoutLogResponseDto?> GetLogByIdAsync(int id)
        {
            try
            {
                var result = await _loginLogoutLogService.GetLogByIdAsync(id);
                if (!result.Success)
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Log yüklenirken hata oluştu");
                    return null;
                }

                return result.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetLogByIdAsync failed for ID: {Id}", id);
                await _toastService.ShowErrorAsync("Log yüklenirken bir hata oluştu");
                return null;
            }
        }

        public async Task<int> GetActiveSessionCountAsync()
        {
            try
            {
                var result = await _loginLogoutLogService.GetActiveSessionCountAsync();
                return result.Success ? result.Data : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveSessionCountAsync failed");
                return 0;
            }
        }

        public async Task<int> GetTodayLoginCountAsync()
        {
            try
            {
                var result = await _loginLogoutLogService.GetTodayLoginCountAsync();
                return result.Success ? result.Data : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTodayLoginCountAsync failed");
                return 0;
            }
        }
    }
}
