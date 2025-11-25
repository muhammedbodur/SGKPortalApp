namespace SGKPortalApp.PresentationLayer.Models.ViewModels
{
    /// <summary>
    /// Breadcrumb (ekmek kırıntısı) navigasyon öğesi
    /// </summary>
    public class BreadcrumbItem
    {
        public string Text { get; set; } = string.Empty;
        public string? Url { get; set; }
    }
}
