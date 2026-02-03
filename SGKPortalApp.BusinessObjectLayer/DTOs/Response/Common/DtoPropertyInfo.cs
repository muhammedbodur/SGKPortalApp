namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    /// <summary>
    /// DTO property bilgisi
    /// </summary>
    public class DtoPropertyInfo
    {
        public string PropertyName { get; set; } = string.Empty;
        public string PropertyType { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public int? MaxLength { get; set; }
    }
}
