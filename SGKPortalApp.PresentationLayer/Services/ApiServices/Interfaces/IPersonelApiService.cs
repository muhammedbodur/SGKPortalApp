using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces
{
    public interface IPersonelApiService
    {
        Task<List<PersonelResponseDto>> GetAllAsync();
        Task<PersonelResponseDto?> GetByIdAsync(int id);
        Task<PersonelResponseDto?> CreateAsync(PersonelCreateRequestDto dto);
        Task<PersonelResponseDto?> UpdateAsync(int id, PersonelUpdateRequestDto dto);
        Task<bool> DeleteAsync(int id);
    }
}