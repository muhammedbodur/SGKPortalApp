using SGKPortalApp.BusinessObjectLayer.DTOs.Response;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.ZKTeco;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.ZKTeco
{
    public class ZKTecoDeviceApiService : IZKTecoDeviceApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseRoute = "api/Device";

        public ZKTecoDeviceApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<List<Device>>> GetAllAsync()
        {
            try
            {
                var devices = await _httpClient.GetFromJsonAsync<List<Device>>(BaseRoute);
                return ApiResponse<List<Device>>.SuccessResponse(devices ?? new List<Device>());
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<List<Device>>.ErrorResponse($"API isteği başarısız: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<Device>>> GetActiveAsync()
        {
            try
            {
                var devices = await _httpClient.GetFromJsonAsync<List<Device>>($"{BaseRoute}/active");
                return ApiResponse<List<Device>>.SuccessResponse(devices ?? new List<Device>());
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<List<Device>>.ErrorResponse($"API isteği başarısız: {ex.Message}");
            }
        }

        public async Task<ApiResponse<Device>> GetByIdAsync(int id)
        {
            try
            {
                var device = await _httpClient.GetFromJsonAsync<Device>($"{BaseRoute}/{id}");
                if (device == null)
                    return ApiResponse<Device>.ErrorResponse("Cihaz bulunamadı");

                return ApiResponse<Device>.SuccessResponse(device);
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<Device>.ErrorResponse($"API isteği başarısız: {ex.Message}");
            }
        }
    }
}
