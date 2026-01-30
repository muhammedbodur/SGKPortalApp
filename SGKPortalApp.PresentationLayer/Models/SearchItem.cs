namespace SGKPortalApp.PresentationLayer.Models
{
    public class SearchItem
    {
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? PermissionKey { get; set; }
    }
}
