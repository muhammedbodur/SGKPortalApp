using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Elasticsearch;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Elasticsearch
{
    public interface IElasticsearchAdminApiService
    {
        Task<ServiceResult<IndexStatusInfo>> GetStatusAsync();
        Task<ServiceResult<bool>> PingAsync();
        Task<ServiceResult<bool>> CreateIndexAsync();
        Task<ServiceResult<int>> FullReindexAsync();
        Task<ServiceResult<int>> IncrementalSyncAsync(int sinceHours = 24);
        Task<ServiceResult<long>> GetDocumentCountAsync();
        Task<ServiceResult<bool>> DeleteIndexAsync();
    }
}
