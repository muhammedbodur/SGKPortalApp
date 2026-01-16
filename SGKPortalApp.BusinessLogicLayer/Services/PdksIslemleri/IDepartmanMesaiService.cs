using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri
{
    public interface IDepartmanMesaiService
    {
        Task<ApiResponseDto<DepartmanMesaiReportDto>> GetDepartmanMesaiReportAsync(DepartmanMesaiFilterRequestDto request);
    }
}
