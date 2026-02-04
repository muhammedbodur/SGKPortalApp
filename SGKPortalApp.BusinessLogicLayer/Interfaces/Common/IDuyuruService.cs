using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Dashboard;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Common
{
    public interface IDuyuruService
    {
        Task<ApiResponseDto<List<DuyuruResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<DuyuruResponseDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<DuyuruResponseDto>> CreateAsync(DuyuruCreateRequestDto request);
        Task<ApiResponseDto<DuyuruResponseDto>> UpdateAsync(int id, DuyuruUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);

        Task<ApiResponseDto<List<DuyuruResponseDto>>> GetSliderDuyurularAsync(int count = 5);
        Task<ApiResponseDto<List<DuyuruResponseDto>>> GetListeDuyurularAsync(int count = 10);
        Task<ApiResponseDto<List<DuyuruResponseDto>>> GetAktifDuyurularAsync();
    }
}
