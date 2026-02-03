using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Common
{
    /// <summary>
    /// DTO Discovery Service - Reflection ile DTO tiplerini ve property'lerini keşfeder
    /// </summary>
    public interface IDtoDiscoveryService
    {
        /// <summary>
        /// Tüm *RequestDto sınıflarını listeler (Reflection ile)
        /// </summary>
        ApiResponseDto<List<DtoTypeInfo>> GetAllDtoTypes();

        /// <summary>
        /// Belirtilen DTO'nun tüm property'lerini döner (Reflection ile)
        /// </summary>
        ApiResponseDto<List<DtoPropertyInfo>> GetDtoProperties(string dtoTypeName);

        /// <summary>
        /// Field Analysis - DTO'daki tüm field'ları + hangilerinin korumalı olduğunu döner
        /// </summary>
        Task<ApiResponseDto<FieldAnalysisResult>> GetFieldAnalysisAsync(string pageKey, string dtoTypeName);
    }
}
