namespace SGKPortalApp.PresentationLayer.Services.StateServices
{
    /// <summary>
    /// Global uygulama durumu yönetimi
    /// </summary>
    public class AppStateService
    {
        private bool _isLoading;
        private string _currentModule = string.Empty;

        public event Action? OnChange;

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyStateChanged();
                }
            }
        }

        public string CurrentModule
        {
            get => _currentModule;
            set
            {
                if (_currentModule != value)
                {
                    _currentModule = value;
                    NotifyStateChanged();
                }
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}