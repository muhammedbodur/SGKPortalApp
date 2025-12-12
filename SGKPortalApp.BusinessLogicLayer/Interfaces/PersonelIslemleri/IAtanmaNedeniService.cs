using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri
{
    public interface IAtanmaNedeniService
    {
        Task<ApiResponseDto<List<AtanmaNedeniResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<AtanmaNedeniResponseDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<AtanmaNedeniResponseDto>> CreateAsync(AtanmaNedeniCreateRequestDto request);
        Task<ApiResponseDto<AtanmaNedeniResponseDto>> UpdateAsync(int id, AtanmaNedeniUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);
        Task<ApiResponseDto<int>> GetPersonelCountAsync(int atanmaNedeniId);
    }
}
