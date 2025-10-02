namespace SGKPortalApp.PresentationLayer.Services.StateServices
{
    /// <summary>
    /// Kullanıcı oturum durumu yönetimi
    /// </summary>
    public class UserStateService
    {
        private string? _userName;
        private string? _userRole;
        private bool _isAuthenticated;

        public event Action? OnChange;

        public string? UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                NotifyStateChanged();
            }
        }

        public string? UserRole
        {
            get => _userRole;
            set
            {
                _userRole = value;
                NotifyStateChanged();
            }
        }

        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            set
            {
                _isAuthenticated = value;
                NotifyStateChanged();
            }
        }

        public void SetUser(string userName, string userRole)
        {
            _userName = userName;
            _userRole = userRole;
            _isAuthenticated = true;
            NotifyStateChanged();
        }

        public void ClearUser()
        {
            _userName = null;
            _userRole = null;
            _isAuthenticated = false;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}