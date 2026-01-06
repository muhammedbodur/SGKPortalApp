using SGKPortalApp.BusinessObjectLayer.DTOs.Response;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.ZKTeco;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.ZKTeco
{
    public class ZKTecoUserApiService : IZKTecoUserApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseRoute = "api/ZKTecoUser";

        public ZKTecoUserApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<List<ZKTecoUserDto>>> GetAllAsync()
        {
            try
            {
                var users = await _httpClient.GetFromJsonAsync<List<ZKTecoUserDto>>(BaseRoute);
                return ApiResponse<List<ZKTecoUserDto>>.SuccessResponse(users ?? new List<ZKTecoUserDto>());
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<List<ZKTecoUserDto>>.ErrorResponse($"API isteği başarısız: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ZKTecoUserDto>> GetByIdAsync(int id)
        {
            try
            {
                var user = await _httpClient.GetFromJsonAsync<ZKTecoUserDto>($"{BaseRoute}/{id}");
                if (user == null)
                    return ApiResponse<ZKTecoUserDto>.ErrorResponse("Kullanıcı bulunamadı");

                return ApiResponse<ZKTecoUserDto>.SuccessResponse(user);
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<ZKTecoUserDto>.ErrorResponse($"API isteği başarısız: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<ZKTecoUserDto>>> GetByDeviceIdAsync(int deviceId)
        {
            try
            {
                var users = await _httpClient.GetFromJsonAsync<List<ZKTecoUserDto>>($"{BaseRoute}/device/{deviceId}");
                return ApiResponse<List<ZKTecoUserDto>>.SuccessResponse(users ?? new List<ZKTecoUserDto>());
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<List<ZKTecoUserDto>>.ErrorResponse($"API isteği başarısız: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<ZKTecoApiUserDto>>> GetUsersFromDeviceAsync(int deviceId)
        {
            try
            {
                var users = await _httpClient.GetFromJsonAsync<List<ZKTecoApiUserDto>>($"{BaseRoute}/device/{deviceId}/from-device");
                return ApiResponse<List<ZKTecoApiUserDto>>.SuccessResponse(users ?? new List<ZKTecoApiUserDto>());
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<List<ZKTecoApiUserDto>>.ErrorResponse($"API isteği başarısız: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> SyncUsersFromDeviceToDbAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{BaseRoute}/device/{deviceId}/sync-from-device", null);
                if (response.IsSuccessStatusCode)
                {
                    return ApiResponse<bool>.SuccessResponse(true);
                }

                return ApiResponse<bool>.ErrorResponse("Senkronizasyon başarısız");
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<bool>.ErrorResponse($"API isteği başarısız: {ex.Message}");
            }
        }
    }
}
