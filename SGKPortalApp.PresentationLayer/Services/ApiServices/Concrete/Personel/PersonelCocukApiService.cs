using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Base;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Personel
{
    public class PersonelCocukApiService : BaseApiService, IPersonelCocukApiService
    {
        public PersonelCocukApiService(HttpClient httpClient, ILogger<PersonelCocukApiService> logger)
            : base(httpClient, logger)
        {
        }

        public async Task<ApiResponseDto<List<PersonelCocukResponseDto>>?> GetAllAsync()
        {
            return await GetAsync<ApiResponseDto<List<PersonelCocukResponseDto>>>("personel-cocuk");
        }

        public async Task<ApiResponseDto<List<PersonelCocukResponseDto>>?> GetByPersonelTcKimlikNoAsync(string tcKimlikNo)
        {
            return await GetAsync<ApiResponseDto<List<PersonelCocukResponseDto>>>($"personel-cocuk/personel/{tcKimlikNo}");
        }

        public async Task<ApiResponseDto<PersonelCocukResponseDto>?> GetByIdAsync(int id)
        {
            return await GetAsync<ApiResponseDto<PersonelCocukResponseDto>>($"personel-cocuk/{id}");
        }

        public async Task<ApiResponseDto<PersonelCocukResponseDto>?> CreateAsync(PersonelCocukCreateRequestDto dto)
        {
            return await PostAsync<PersonelCocukCreateRequestDto, ApiResponseDto<PersonelCocukResponseDto>>("personel-cocuk", dto);
        }

        public async Task<ApiResponseDto<PersonelCocukResponseDto>?> UpdateAsync(int id, PersonelCocukUpdateRequestDto dto)
        {
            return await PutAsync<PersonelCocukUpdateRequestDto, ApiResponseDto<PersonelCocukResponseDto>>($"personel-cocuk/{id}", dto);
        }

        public async Task<ApiResponseDto<bool>?> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"personel-cocuk/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
            }
            return null;
        }
    }
}
