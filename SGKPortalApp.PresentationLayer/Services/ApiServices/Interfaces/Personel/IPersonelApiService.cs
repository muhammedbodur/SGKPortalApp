using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel
{
    public interface IPersonelApiService
    {
        Task<List<PersonelResponseDto>> GetAllAsync();
        Task<PersonelResponseDto?> GetByTcKimlikNoAsync(string tcKimlikNo);
        Task<PersonelResponseDto?> CreateAsync(PersonelCreateRequestDto dto);
        Task<PersonelResponseDto?> UpdateAsync(string tcKimlikNo, PersonelUpdateRequestDto dto);
        Task<bool> DeleteAsync(string tcKimlikNo);
    }
}