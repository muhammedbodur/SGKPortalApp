using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri
{
    public interface ICardSyncComparisonService
    {
        Task<DeviceCardSyncReportDto> GenerateCardSyncReportAsync();
    }
}
