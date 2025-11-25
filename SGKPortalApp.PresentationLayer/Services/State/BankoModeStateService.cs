namespace SGKPortalApp.PresentationLayer.Services.State
{
    /// <summary>
    /// Banko modu state yönetimi
    /// Singleton olarak çalışır, tüm uygulama boyunca banko modu durumunu tutar
    /// </summary>
    public class BankoModeStateService
    {
        private bool _isInBankoMode = false;
        private int? _activeBankoId = null;
        private string? _personelTcKimlikNo = null;

        /// <summary>
        /// Banko modu aktif mi?
        /// </summary>
        public bool IsInBankoMode => _isInBankoMode;

        /// <summary>
        /// Aktif banko ID
        /// </summary>
        public int? ActiveBankoId => _activeBankoId;

        /// <summary>
        /// Banko modundaki personel TC Kimlik No
        /// </summary>
        public string? PersonelTcKimlikNo => _personelTcKimlikNo;

        /// <summary>
        /// Banko modu değişiklik event'i
        /// </summary>
        public event Action? OnBankoModeChanged;

        /// <summary>
        /// Banko modunu aktif et
        /// </summary>
        public void ActivateBankoMode(int bankoId, string tcKimlikNo)
        {
            _isInBankoMode = true;
            _activeBankoId = bankoId;
            _personelTcKimlikNo = tcKimlikNo;
            OnBankoModeChanged?.Invoke();
        }

        /// <summary>
        /// Banko modunu deaktif et
        /// </summary>
        public void DeactivateBankoMode()
        {
            _isInBankoMode = false;
            _activeBankoId = null;
            _personelTcKimlikNo = null;
            OnBankoModeChanged?.Invoke();
        }

        /// <summary>
        /// Bu personel banko modunda mı?
        /// </summary>
        public bool IsPersonelInBankoMode(string tcKimlikNo)
        {
            return _isInBankoMode && _personelTcKimlikNo == tcKimlikNo;
        }

        /// <summary>
        /// İzin verilen URL mi? (Banko modunda)
        /// </summary>
        public bool IsUrlAllowedInBankoMode(string url)
        {
            if (!_isInBankoMode) return true;

            // Banko modunda sadece bu URL'lere izin var
            var allowedUrls = new[]
            {
                "/",
                "/siramatik/banko/",
                "/account/logout",
                "/_framework/",
                "/_content/",
                "/lib/",
                "/js/",
                "/css/",
                "/portal/assets/"
            };

            return allowedUrls.Any(allowed => url.StartsWith(allowed, StringComparison.OrdinalIgnoreCase));
        }
    }
}
