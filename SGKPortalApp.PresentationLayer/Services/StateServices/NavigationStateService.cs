using SGKPortalApp.PresentationLayer.Models.ViewModels;

namespace SGKPortalApp.PresentationLayer.Services.StateServices
{
    /// <summary>
    /// Navigasyon durumu ve breadcrumb yönetimi
    /// </summary>
    public class NavigationStateService
    {
        private readonly List<BreadcrumbItem> _breadcrumbs = new();

        public event Action? OnChange;

        public IReadOnlyList<BreadcrumbItem> Breadcrumbs => _breadcrumbs.AsReadOnly();

        public void SetBreadcrumbs(params BreadcrumbItem[] items)
        {
            _breadcrumbs.Clear();
            _breadcrumbs.AddRange(items);
            NotifyStateChanged();
        }

        public void AddBreadcrumb(string text, string? url = null)
        {
            _breadcrumbs.Add(new BreadcrumbItem { Text = text, Url = url });
            NotifyStateChanged();
        }

        public void ClearBreadcrumbs()
        {
            _breadcrumbs.Clear();
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}