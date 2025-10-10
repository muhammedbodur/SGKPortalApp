using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri
{
    public interface IPersonelCocukService
    {
        Task<ApiResponseDto<List<PersonelCocukResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<List<PersonelCocukResponseDto>>> GetByPersonelTcKimlikNoAsync(string tcKimlikNo);
        Task<ApiResponseDto<PersonelCocukResponseDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<PersonelCocukResponseDto>> CreateAsync(PersonelCocukCreateRequestDto request);
        Task<ApiResponseDto<PersonelCocukResponseDto>> UpdateAsync(int id, PersonelCocukUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);
    }
}
