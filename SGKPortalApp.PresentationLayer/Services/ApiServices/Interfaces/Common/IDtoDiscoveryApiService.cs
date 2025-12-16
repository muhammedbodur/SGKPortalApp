using SGKPortalApp.BusinessObjectLayer.DTOs.Common;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common
{
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

    #region Response Models

    public class DtoTypeInfo
    {
        public string TypeName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Namespace { get; set; }
    }

    public class DtoPropertyInfo
    {
        public string PropertyName { get; set; } = string.Empty;
        public string PropertyType { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public int? MaxLength { get; set; }
    }

    public class FieldAnalysisResult
    {
        public string PageKey { get; set; } = string.Empty;
        public string DtoTypeName { get; set; } = string.Empty;
        public int TotalFields { get; set; }
        public int ProtectedFields { get; set; }
        public int AvailableFields { get; set; }
        public List<FieldAnalysisInfo> Fields { get; set; } = new();
    }

    public class FieldAnalysisInfo
    {
        public string FieldName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string PropertyType { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public bool IsProtected { get; set; }
        public bool CanAddPermission { get; set; }
        public string? ExistingPermissionKey { get; set; }
    }

    #endregion
}
