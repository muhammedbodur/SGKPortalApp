using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri
{
    public interface IPersonelYetkiService
    {
        Task<ApiResponseDto<PersonelYetkiResponseDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<List<PersonelYetkiResponseDto>>> GetByTcKimlikNoAsync(string tcKimlikNo);
        Task<ApiResponseDto<List<PersonelYetkiResponseDto>>> GetByModulControllerIslemIdAsync(int modulControllerIslemId);

        Task<ApiResponseDto<PersonelYetkiResponseDto>> CreateAsync(PersonelYetkiCreateRequestDto request);
        Task<ApiResponseDto<PersonelYetkiResponseDto>> UpdateAsync(int id, PersonelYetkiUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);
    }
}
