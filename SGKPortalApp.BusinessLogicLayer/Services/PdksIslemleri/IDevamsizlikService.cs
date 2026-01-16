using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.Common.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri
{
    public interface IDevamsizlikService
    {
        Task<IResult<List<DevamsizlikListDto>>> GetDevamsizlikListAsync(DevamsizlikFilterDto filter);
        Task<IResult<int>> CreateDevamsizlikAsync(DevamsizlikCreateDto request);
        Task<IResult<bool>> OnaylaDevamsizlikAsync(int id, int onaylayanSicilNo);
        Task<IResult<bool>> DeleteDevamsizlikAsync(int id);
    }
}
