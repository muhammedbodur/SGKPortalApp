using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.ZKTeco;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.ZKTeco
{
    public class CardSyncComparisonApiService : ICardSyncComparisonApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "pdks/card-sync-comparison";

        public CardSyncComparisonApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DeviceCardSyncReportDto> GenerateCardSyncReportAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<DeviceCardSyncReportDto>($"{BaseUrl}/generate-report");
                return response ?? new DeviceCardSyncReportDto();
            }
            catch
            {
                return new DeviceCardSyncReportDto();
            }
        }
    }
}
