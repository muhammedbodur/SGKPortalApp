namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Elasticsearch
{
    public class ReindexResponse
    {
        public int IndexedCount { get; set; }
        public string? Message { get; set; }
    }
}
