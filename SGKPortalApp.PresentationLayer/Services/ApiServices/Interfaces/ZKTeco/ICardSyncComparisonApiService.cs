using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.ZKTeco
{
    public interface ICardSyncComparisonApiService
    {
        Task<DeviceCardSyncReportDto> GenerateCardSyncReportAsync();
    }
}
