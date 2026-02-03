namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    /// <summary>
    /// DTO tipi bilgisi
    /// </summary>
    public class DtoTypeInfo
    {
        public string TypeName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Namespace { get; set; }
    }
}
