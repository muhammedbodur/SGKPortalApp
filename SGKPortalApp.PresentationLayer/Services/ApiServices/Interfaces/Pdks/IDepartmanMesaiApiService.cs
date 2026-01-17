using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Pdks
{
    public interface IDepartmanMesaiApiService
    {
        Task<ServiceResult<DepartmanMesaiReportDto>> GetRaporAsync(DepartmanMesaiFilterRequestDto request);
    }
}
