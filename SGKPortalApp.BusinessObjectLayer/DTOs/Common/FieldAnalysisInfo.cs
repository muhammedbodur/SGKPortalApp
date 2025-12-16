namespace SGKPortalApp.BusinessObjectLayer.DTOs.Common
{
    /// <summary>
    /// Tek bir field'ın analiz bilgisi
    /// </summary>
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
}
