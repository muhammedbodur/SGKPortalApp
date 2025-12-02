using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Siramatik
{
    /// <summary>
    /// Sıra Yönlendirme API Servisi - HTTP Client ile API çağrıları
    /// </summary>
    public class SiraYonlendirmeApiService : ISiraYonlendirmeApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "siramatik/sira-yonlendirme";

        public SiraYonlendirmeApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponseDto<bool>> YonlendirSiraAsync(SiraYonlendirmeDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/yonlendir", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
                    return result ?? ApiResponseDto<bool>.ErrorResult("Yanıt alınamadı");
                }

                var errorResult = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
                return errorResult ?? ApiResponseDto<bool>.ErrorResult("Yönlendirme işlemi başarısız oldu");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Hatası - YonlendirSiraAsync: {ex.Message}");
                return ApiResponseDto<bool>.ErrorResult($"API çağrısı sırasında hata oluştu: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<int>> GetYonlendirilmisSiraCountAsync(int bankoId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<int>>($"{BaseUrl}/yonlendirilmis-sira-count/{bankoId}");
                return response ?? ApiResponseDto<int>.ErrorResult("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Hatası - GetYonlendirilmisSiraCountAsync: {ex.Message}");
                return ApiResponseDto<int>.ErrorResult($"API çağrısı sırasında hata oluştu: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<YonlendirmeSecenekleriResponseDto>> GetYonlendirmeSecenekleriAsync(int siraId, int kaynakBankoId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<YonlendirmeSecenekleriResponseDto>>(
                    $"{BaseUrl}/secenekler/{siraId}/{kaynakBankoId}");
                return response ?? ApiResponseDto<YonlendirmeSecenekleriResponseDto>.ErrorResult("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Hatası - GetYonlendirmeSecenekleriAsync: {ex.Message}");
                return ApiResponseDto<YonlendirmeSecenekleriResponseDto>.ErrorResult($"API çağrısı sırasında hata oluştu: {ex.Message}");
            }
        }
    }
}
