using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Siramatik
{
    /// <summary>
    /// Sıra Çağırma API Servisi - HTTP Client ile API çağrıları
    /// </summary>
    public class SiraCagirmaApiService : ISiraCagirmaApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "siramatik/sira-cagirma";

        public SiraCagirmaApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<SiraCagirmaResponseDto>> GetBekleyenSiralarAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<SiraCagirmaResponseDto>>($"{BaseUrl}/bekleyen-siralar");
                return response ?? new List<SiraCagirmaResponseDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Hatası - GetBekleyenSiralarAsync: {ex.Message}");
                return new List<SiraCagirmaResponseDto>();
            }
        }

        public async Task<List<SiraCagirmaResponseDto>> GetPersonelBekleyenSiralarAsync(string tcKimlikNo)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<SiraCagirmaResponseDto>>($"{BaseUrl}/personel-bekleyen-siralar/{tcKimlikNo}");
                return response ?? new List<SiraCagirmaResponseDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Hatası - GetPersonelBekleyenSiralarAsync: {ex.Message}");
                return new List<SiraCagirmaResponseDto>();
            }
        }

        public async Task<SiraCagirmaResponseDto?> SiradakiCagirAsync(int siraId, string personelTcKimlikNo, int? bankoId = null, string? bankoNo = null, int? firstCallableSiraId = null)
        {
            try
            {
                var queryParams = new List<string> { $"personelTcKimlikNo={personelTcKimlikNo}" };
                
                if (bankoId.HasValue)
                    queryParams.Add($"bankoId={bankoId.Value}");
                
                if (!string.IsNullOrEmpty(bankoNo))
                    queryParams.Add($"bankoNo={Uri.EscapeDataString(bankoNo)}");
                
                if (firstCallableSiraId.HasValue)
                    queryParams.Add($"firstCallableSiraId={firstCallableSiraId.Value}");

                var query = string.Join("&", queryParams);
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/siradaki-cagir/{siraId}?{query}", new { });
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<SiraCagirmaResponseDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Hatası - SiradakiCagirAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> SiraTamamlaAsync(int siraId)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/tamamla/{siraId}", new { });
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Hatası - SiraTamamlaAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SiraIptalAsync(int siraId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/iptal/{siraId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Hatası - SiraIptalAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<List<SiraCagirmaResponseDto>> GetBankoPanelSiralarAsync(string tcKimlikNo)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<SiraCagirmaResponseDto>>($"{BaseUrl}/banko-panel/{tcKimlikNo}");
                return response ?? new List<SiraCagirmaResponseDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Hatası - GetBankoPanelSiralarAsync: {ex.Message}");
                return new List<SiraCagirmaResponseDto>();
            }
        }
    }
}
