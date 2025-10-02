using System.Net.Http.Json;
using System.Text.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Base
{
    public abstract class BaseApiService
    {
        protected readonly HttpClient _httpClient;
        protected readonly ILogger _logger;
        protected readonly JsonSerializerOptions _jsonOptions;

        protected BaseApiService(IHttpClientFactory httpClientFactory, ILogger logger)
        {
            _httpClient = httpClientFactory.CreateClient("SGKAPI");
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        protected async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                _logger.LogInformation($"GET: {endpoint}");
                return await _httpClient.GetFromJsonAsync<T>(endpoint, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GET hatası: {endpoint}");
                throw;
            }
        }

        protected async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                _logger.LogInformation($"POST: {endpoint}");
                var response = await _httpClient.PostAsJsonAsync(endpoint, data, _jsonOptions);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"POST hatası: {endpoint}");
                throw;
            }
        }

        protected async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                _logger.LogInformation($"PUT: {endpoint}");
                var response = await _httpClient.PutAsJsonAsync(endpoint, data, _jsonOptions);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"PUT hatası: {endpoint}");
                throw;
            }
        }

        protected async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                _logger.LogInformation($"DELETE: {endpoint}");
                var response = await _httpClient.DeleteAsync(endpoint);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DELETE hatası: {endpoint}");
                throw;
            }
        }
    }
}