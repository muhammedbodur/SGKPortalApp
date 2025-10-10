using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Base;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Personel
{
    public class PersonelApiService : BaseApiService, IPersonelApiService
    {
        public PersonelApiService(HttpClient httpClient, ILogger<PersonelApiService> logger)
            : base(httpClient, logger)
        {
        }

        public async Task<List<PersonelResponseDto>> GetAllAsync()
        {
            return await GetAsync<List<PersonelResponseDto>>("api/personel") ?? new List<PersonelResponseDto>();
        }

        public async Task<PersonelResponseDto?> GetByTcKimlikNoAsync(string tcKimlikNo)
        {
            return await GetAsync<PersonelResponseDto>($"api/personel/{tcKimlikNo}");
        }

        public async Task<PersonelResponseDto?> CreateAsync(PersonelCreateRequestDto dto)
        {
            return await PostAsync<PersonelCreateRequestDto, PersonelResponseDto>("api/personel", dto);
        }

        public async Task<PersonelResponseDto?> UpdateAsync(string tcKimlikNo, PersonelUpdateRequestDto dto)
        {
            return await PutAsync<PersonelUpdateRequestDto, PersonelResponseDto>($"api/personel/{tcKimlikNo}", dto);
        }

        public async Task<bool> DeleteAsync(string tcKimlikNo)
        {
            return await DeleteAsync($"api/personel/{tcKimlikNo}");
        }

        // Toplu kayıt metodları (Transaction)
        public async Task<ApiResponseDto<PersonelResponseDto>?> CreateCompleteAsync(PersonelCompleteRequestDto dto)
        {
            return await PostAsync<PersonelCompleteRequestDto, ApiResponseDto<PersonelResponseDto>>("api/personel/complete", dto);
        }

        public async Task<ApiResponseDto<PersonelResponseDto>?> UpdateCompleteAsync(string tcKimlikNo, PersonelCompleteRequestDto dto)
        {
            return await PutAsync<PersonelCompleteRequestDto, ApiResponseDto<PersonelResponseDto>>($"api/personel/{tcKimlikNo}/complete", dto);
        }
    }
}