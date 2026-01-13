namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    /// <summary>
    /// Field analiz sonucu
    /// </summary>
    public class FieldAnalysisResult
    {
        public string PageKey { get; set; } = string.Empty;
        public string DtoTypeName { get; set; } = string.Empty;
        public int TotalFields { get; set; }
        public int ProtectedFields { get; set; }
        public int AvailableFields { get; set; }
        public List<FieldAnalysisInfo> Fields { get; set; } = new();
    }
}
