namespace SGKPortalApp.BusinessObjectLayer.DTOs.Elasticsearch
{
    public class ReindexResponse
    {
        public int IndexedCount { get; set; }
        public string? Message { get; set; }
    }
}
