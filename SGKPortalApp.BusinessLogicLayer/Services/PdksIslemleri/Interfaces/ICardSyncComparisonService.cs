using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri.Interfaces
{
    public interface ICardSyncComparisonService
    {
        Task<DeviceCardSyncReportDto> GenerateCardSyncReportAsync();
    }
}
