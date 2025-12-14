using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri
{
    public interface IYetkiService
    {
        Task<ApiResponseDto<List<YetkiResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<YetkiResponseDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<YetkiResponseDto>> CreateAsync(YetkiCreateRequestDto request);
        Task<ApiResponseDto<YetkiResponseDto>> UpdateAsync(int id, YetkiUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);

        Task<ApiResponseDto<List<YetkiResponseDto>>> GetRootAsync();
        Task<ApiResponseDto<List<YetkiResponseDto>>> GetChildrenAsync(int ustYetkiId);
        Task<ApiResponseDto<List<DropdownItemDto>>> GetDropdownAsync();
    }
}
