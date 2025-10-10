using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel
{
    public interface IPersonelCocukApiService
    {
        Task<ApiResponseDto<List<PersonelCocukResponseDto>>?> GetAllAsync();
        Task<ApiResponseDto<List<PersonelCocukResponseDto>>?> GetByPersonelTcKimlikNoAsync(string tcKimlikNo);
        Task<ApiResponseDto<PersonelCocukResponseDto>?> GetByIdAsync(int id);
        Task<ApiResponseDto<PersonelCocukResponseDto>?> CreateAsync(PersonelCocukCreateRequestDto dto);
        Task<ApiResponseDto<PersonelCocukResponseDto>?> UpdateAsync(int id, PersonelCocukUpdateRequestDto dto);
        Task<ApiResponseDto<bool>?> DeleteAsync(int id);
    }
}
