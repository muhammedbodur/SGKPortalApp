using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Base;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Personel
{
    public class AtanmaNedeniApiService : BaseApiService, IAtanmaNedeniApiService
    {
        public AtanmaNedeniApiService(HttpClient httpClient, ILogger<AtanmaNedeniApiService> logger)
            : base(httpClient, logger)
        {
        }

        public async Task<ApiResponseDto<List<AtanmaNedeniResponseDto>>?> GetAllAsync()
        {
            return await GetAsync<ApiResponseDto<List<AtanmaNedeniResponseDto>>>("api/atanma-nedeni");
        }

        public async Task<ApiResponseDto<AtanmaNedeniResponseDto>?> GetByIdAsync(int id)
        {
            return await GetAsync<ApiResponseDto<AtanmaNedeniResponseDto>>($"api/atanma-nedeni/{id}");
        }

        public async Task<ApiResponseDto<AtanmaNedeniResponseDto>?> CreateAsync(AtanmaNedeniCreateRequestDto dto)
        {
            return await PostAsync<AtanmaNedeniCreateRequestDto, ApiResponseDto<AtanmaNedeniResponseDto>>("api/atanma-nedeni", dto);
        }

        public async Task<ApiResponseDto<AtanmaNedeniResponseDto>?> UpdateAsync(int id, AtanmaNedeniUpdateRequestDto dto)
        {
            return await PutAsync<AtanmaNedeniUpdateRequestDto, ApiResponseDto<AtanmaNedeniResponseDto>>($"api/atanma-nedeni/{id}", dto);
        }

        public async Task<ApiResponseDto<bool>?> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/atanma-nedeni/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
            }
            return null;
        }
    }
}
