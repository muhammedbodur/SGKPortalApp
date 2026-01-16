using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.Common.Results;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri
{
    public interface ISgmMesaiService
    {
        Task<IResult<SgmMesaiReportDto>> GetSgmMesaiReportAsync(SgmMesaiFilterRequestDto request);
    }
}
