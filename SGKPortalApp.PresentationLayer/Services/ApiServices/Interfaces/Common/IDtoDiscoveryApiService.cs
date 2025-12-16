using SGKPortalApp.BusinessObjectLayer.DTOs.Common;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common
{
    /// <summary>
    /// DTO Discovery API Service - Backend DtoDiscoveryController'a HTTP çağrıları yapar
    /// DTO'lar BusinessObjectLayer.DTOs.Common.DtoDiscoveryDtos'da tanımlı
    /// </summary>
    public interface IDtoDiscoveryApiService
    {
        /// <summary>
        /// Tüm *RequestDto sınıflarını listeler
        /// </summary>
        Task<ServiceResult<List<DtoTypeInfo>>> GetAllDtoTypesAsync();

        /// <summary>
        /// Belirtilen DTO'nun tüm property'lerini döner
        /// </summary>
        Task<ServiceResult<List<DtoPropertyInfo>>> GetDtoPropertiesAsync(string dtoTypeName);

        /// <summary>
        /// Field Analysis - DTO field'ları + korumalı field'lar
        /// </summary>
        Task<ServiceResult<FieldAnalysisResult>> GetFieldAnalysisAsync(string pageKey, string dtoTypeName);
    }
}
