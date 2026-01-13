using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.ZKTeco;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.ZKTeco
{
    public class SpecialCardApiService : ISpecialCardApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "pdks/special-cards";

        public SpecialCardApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponseDto<List<SpecialCardResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<List<SpecialCardResponseDto>>>(BaseUrl);
                return response ?? ApiResponseDto<List<SpecialCardResponseDto>>.ErrorResult("Yanıt alınamadı");
            }
            catch
            {
                return ApiResponseDto<List<SpecialCardResponseDto>>.ErrorResult("Bağlantı hatası");
            }
        }

        public async Task<ApiResponseDto<SpecialCardResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<SpecialCardResponseDto>>($"{BaseUrl}/{id}");
                return response ?? ApiResponseDto<SpecialCardResponseDto>.ErrorResult("Yanıt alınamadı");
            }
            catch
            {
                return ApiResponseDto<SpecialCardResponseDto>.ErrorResult("Bağlantı hatası");
            }
        }

        public async Task<ApiResponseDto<SpecialCardResponseDto>> GetByCardNumberAsync(long cardNumber)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<SpecialCardResponseDto>>($"{BaseUrl}/card-number/{cardNumber}");
                return response ?? ApiResponseDto<SpecialCardResponseDto>.ErrorResult("Yanıt alınamadı");
            }
            catch
            {
                return ApiResponseDto<SpecialCardResponseDto>.ErrorResult("Bağlantı hatası");
            }
        }

        public async Task<ApiResponseDto<SpecialCardResponseDto>> GetByEnrollNumberAsync(string enrollNumber)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<SpecialCardResponseDto>>($"{BaseUrl}/enroll-number/{enrollNumber}");
                return response ?? ApiResponseDto<SpecialCardResponseDto>.ErrorResult("Yanıt alınamadı");
            }
            catch
            {
                return ApiResponseDto<SpecialCardResponseDto>.ErrorResult("Bağlantı hatası");
            }
        }

        public async Task<ApiResponseDto<List<SpecialCardResponseDto>>> GetByCardTypeAsync(CardType cardType)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<List<SpecialCardResponseDto>>>($"{BaseUrl}/type/{cardType}");
                return response ?? ApiResponseDto<List<SpecialCardResponseDto>>.ErrorResult("Yanıt alınamadı");
            }
            catch
            {
                return ApiResponseDto<List<SpecialCardResponseDto>>.ErrorResult("Bağlantı hatası");
            }
        }

        public async Task<ApiResponseDto<SpecialCardResponseDto>> CreateAsync(SpecialCardCreateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, request);
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<SpecialCardResponseDto>>()
                    ?? ApiResponseDto<SpecialCardResponseDto>.ErrorResult("Yanıt alınamadı");
            }
            catch
            {
                return ApiResponseDto<SpecialCardResponseDto>.ErrorResult("Bağlantı hatası");
            }
        }

        public async Task<ApiResponseDto<SpecialCardResponseDto>> UpdateAsync(int id, SpecialCardUpdateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request);
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<SpecialCardResponseDto>>()
                    ?? ApiResponseDto<SpecialCardResponseDto>.ErrorResult("Yanıt alınamadı");
            }
            catch
            {
                return ApiResponseDto<SpecialCardResponseDto>.ErrorResult("Bağlantı hatası");
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>()
                    ?? ApiResponseDto<bool>.ErrorResult("Yanıt alınamadı");
            }
            catch
            {
                return ApiResponseDto<bool>.ErrorResult("Bağlantı hatası");
            }
        }

        // ========== Device Operations ==========

        public async Task<ApiResponseDto<CardSyncResultDto>> SendCardToDeviceAsync(int cardId, int deviceId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{BaseUrl}/{cardId}/send-to-device/{deviceId}", null);
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<CardSyncResultDto>>()
                    ?? ApiResponseDto<CardSyncResultDto>.ErrorResult("Yanıt alınamadı");
            }
            catch
            {
                return ApiResponseDto<CardSyncResultDto>.ErrorResult("Bağlantı hatası");
            }
        }

        public async Task<ApiResponseDto<CardSyncResultDto>> SendCardToAllDevicesAsync(int cardId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{BaseUrl}/{cardId}/send-to-all-devices", null);
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<CardSyncResultDto>>()
                    ?? ApiResponseDto<CardSyncResultDto>.ErrorResult("Yanıt alınamadı");
            }
            catch
            {
                return ApiResponseDto<CardSyncResultDto>.ErrorResult("Bağlantı hatası");
            }
        }

        public async Task<ApiResponseDto<CardSyncResultDto>> DeleteCardFromDeviceAsync(int cardId, int deviceId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{cardId}/delete-from-device/{deviceId}");
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<CardSyncResultDto>>()
                    ?? ApiResponseDto<CardSyncResultDto>.ErrorResult("Yanıt alınamadı");
            }
            catch
            {
                return ApiResponseDto<CardSyncResultDto>.ErrorResult("Bağlantı hatası");
            }
        }

        public async Task<ApiResponseDto<CardSyncResultDto>> DeleteCardFromAllDevicesAsync(int cardId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{cardId}/delete-from-all-devices");
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<CardSyncResultDto>>()
                    ?? ApiResponseDto<CardSyncResultDto>.ErrorResult("Yanıt alınamadı");
            }
            catch
            {
                return ApiResponseDto<CardSyncResultDto>.ErrorResult("Bağlantı hatası");
            }
        }
    }
}
