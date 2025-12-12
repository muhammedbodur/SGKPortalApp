using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel
{
    public interface IAtanmaNedeniApiService
    {
        Task<ApiResponseDto<List<AtanmaNedeniResponseDto>>?> GetAllAsync();
        Task<ApiResponseDto<AtanmaNedeniResponseDto>?> GetByIdAsync(int id);
        Task<ApiResponseDto<AtanmaNedeniResponseDto>?> CreateAsync(AtanmaNedeniCreateRequestDto dto);
        Task<ApiResponseDto<AtanmaNedeniResponseDto>?> UpdateAsync(int id, AtanmaNedeniUpdateRequestDto dto);
        Task<ApiResponseDto<bool>?> DeleteAsync(int id);
        Task<ApiResponseDto<int>?> GetPersonelCountAsync(int atanmaNedeniId);
    }
}
