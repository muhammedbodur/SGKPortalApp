using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel
{
    public interface IYetkiApiService
    {
        Task<ServiceResult<List<YetkiResponseDto>>> GetAllAsync();
        Task<ServiceResult<YetkiResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<YetkiResponseDto>> CreateAsync(YetkiCreateRequestDto request);
        Task<ServiceResult<YetkiResponseDto>> UpdateAsync(int id, YetkiUpdateRequestDto request);
        Task<ServiceResult<bool>> DeleteAsync(int id);

        Task<ServiceResult<List<YetkiResponseDto>>> GetRootAsync();
        Task<ServiceResult<List<YetkiResponseDto>>> GetChildrenAsync(int ustYetkiId);
        Task<ServiceResult<List<DropdownItemDto>>> GetDropdownAsync();
    }
}
