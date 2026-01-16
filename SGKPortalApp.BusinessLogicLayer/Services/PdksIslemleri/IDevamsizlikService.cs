using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri
{
    public interface IDevamsizlikService
    {
        Task<ApiResponseDto<List<DevamsizlikListDto>>> GetDevamsizlikListAsync(DevamsizlikFilterDto filter);
        Task<ApiResponseDto<int>> CreateDevamsizlikAsync(DevamsizlikCreateDto request);
        Task<ApiResponseDto<bool>> OnaylaDevamsizlikAsync(int id, int onaylayanSicilNo);
        Task<ApiResponseDto<bool>> DeleteDevamsizlikAsync(int id);
    }
}
