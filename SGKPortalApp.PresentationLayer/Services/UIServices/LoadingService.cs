namespace SGKPortalApp.PresentationLayer.Services.UIServices
{
    public class LoadingService : ILoadingService
    {
        private bool _isLoading;
        private string? _loadingMessage;

        public event Action<bool>? OnLoadingChanged;

        public bool IsLoading => _isLoading;
        public string? LoadingMessage => _loadingMessage;

        public void Show(string? message = null)
        {
            _isLoading = true;
            _loadingMessage = message ?? "Yükleniyor...";
            OnLoadingChanged?.Invoke(true);
        }

        public void Hide()
        {
            _isLoading = false;
            _loadingMessage = null;
            OnLoadingChanged?.Invoke(false);
        }
    }
}