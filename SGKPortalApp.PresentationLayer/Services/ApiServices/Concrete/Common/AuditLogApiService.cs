using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.AuditLog;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.AuditLog;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Common
{
    /// <summary>
    /// Audit log API servisi implementation
    /// API Layer'a HTTP istekleri gönderir
    /// </summary>
    public class AuditLogApiService : IAuditLogApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuditLogApiService> _logger;

        public AuditLogApiService(HttpClient httpClient, ILogger<AuditLogApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<AuditLogPagedResultDto?> GetLogsAsync(AuditLogFilterDto filter)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("auditlog/search", filter);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetLogsAsync failed: {Error}", errorContent);
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<AuditLogPagedResultDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetLogsAsync exception");
                return null;
            }
        }

        public async Task<AuditLogDetailDto?> GetLogDetailAsync(int logId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"auditlog/{logId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetLogDetailAsync failed: {Error}", errorContent);
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<AuditLogDetailDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetLogDetailAsync exception");
                return null;
            }
        }

        public async Task<List<AuditLogDto>?> GetTransactionLogsAsync(Guid transactionId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"auditlog/transaction/{transactionId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetTransactionLogsAsync failed: {Error}", errorContent);
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<List<AuditLogDto>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTransactionLogsAsync exception");
                return null;
            }
        }

        public async Task<List<AuditLogDto>?> GetUserRecentLogsAsync(string tcKimlikNo, int count = 50)
        {
            try
            {
                var response = await _httpClient.GetAsync($"auditlog/user/{tcKimlikNo}?count={count}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetUserRecentLogsAsync failed: {Error}", errorContent);
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<List<AuditLogDto>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUserRecentLogsAsync exception");
                return null;
            }
        }

        public async Task<List<AuditLogDto>?> GetEntityHistoryAsync(string tableName, string entityId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"auditlog/entity/{tableName}/{entityId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetEntityHistoryAsync failed: {Error}", errorContent);
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<List<AuditLogDto>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetEntityHistoryAsync exception");
                return null;
            }
        }
    }
}
