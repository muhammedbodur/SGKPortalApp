using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Base;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices
{
    public class PersonelApiService : BaseApiService, IPersonelApiService
    {
        public PersonelApiService(IHttpClientFactory httpClientFactory, ILogger<PersonelApiService> logger)
            : base(httpClientFactory, logger)
        {
        }

        public async Task<List<PersonelResponseDto>> GetAllAsync()
        {
            return await GetAsync<List<PersonelResponseDto>>("personel") ?? new List<PersonelResponseDto>();
        }

        public async Task<PersonelResponseDto?> GetByIdAsync(int id)
        {
            return await GetAsync<PersonelResponseDto>($"personel/{id}");
        }

        public async Task<PersonelResponseDto?> CreateAsync(PersonelCreateRequestDto dto)
        {
            return await PostAsync<PersonelCreateRequestDto, PersonelResponseDto>("personel", dto);
        }

        public async Task<PersonelResponseDto?> UpdateAsync(int id, PersonelUpdateRequestDto dto)
        {
            return await PutAsync<PersonelUpdateRequestDto, PersonelResponseDto>($"personel/{id}", dto);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await DeleteAsync($"personel/{id}");
        }
    }
}