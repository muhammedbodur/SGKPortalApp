using System.Collections.Concurrent;

namespace SGKPortalApp.ApiLayer.Services.State
{
    /// <summary>
    /// Banko modu state yönetimi
    /// Singleton olarak çalışır, kullanıcı bazlı banko modu durumunu tutar
    /// </summary>
    public class BankoModeStateService
    {
        // Kullanıcı bazlı state (TcKimlikNo -> BankoId)
        private readonly ConcurrentDictionary<string, int> _userBankoStates = new();
        
        // Event'ler kullanıcı bazlı (TcKimlikNo -> Event)
        private readonly ConcurrentDictionary<string, Action?> _userEvents = new();

        // Mevcut kullanıcı context'i (Scoped olarak set edilmeli)
        private string? _currentUserTc;

        /// <summary>
        /// Mevcut kullanıcıyı set et (Her request başında çağrılmalı)
        /// </summary>
        public void SetCurrentUser(string tcKimlikNo)
        {
            _currentUserTc = tcKimlikNo;
        }

        /// <summary>
        /// Banko modu aktif mi? (Mevcut kullanıcı için)
        /// </summary>
        public bool IsInBankoMode => !string.IsNullOrEmpty(_currentUserTc) && _userBankoStates.ContainsKey(_currentUserTc);

        /// <summary>
        /// Aktif banko ID (Mevcut kullanıcı için)
        /// </summary>
        public int? ActiveBankoId => !string.IsNullOrEmpty(_currentUserTc) && _userBankoStates.TryGetValue(_currentUserTc, out var bankoId) ? bankoId : null;

        /// <summary>
        /// Banko modundaki personel TC Kimlik No (Mevcut kullanıcı için)
        /// </summary>
        public string? PersonelTcKimlikNo => IsInBankoMode ? _currentUserTc : null;

        /// <summary>
        /// Banko modu değişiklik event'i (Mevcut kullanıcı için)
        /// </summary>
        public event Action? OnBankoModeChanged
        {
            add
            {
                if (!string.IsNullOrEmpty(_currentUserTc))
                {
                    _userEvents.AddOrUpdate(_currentUserTc, value, (_, existing) => existing + value);
                }
            }
            remove
            {
                if (!string.IsNullOrEmpty(_currentUserTc) && _userEvents.TryGetValue(_currentUserTc, out var existing))
                {
                    _userEvents[_currentUserTc] = existing - value;
                }
            }
        }

        /// <summary>
        /// Banko modunu aktif et
        /// </summary>
        public void ActivateBankoMode(int bankoId, string tcKimlikNo)
        {
            _userBankoStates[tcKimlikNo] = bankoId;
            NotifyUser(tcKimlikNo);
        }

        /// <summary>
        /// Banko modunu deaktif et
        /// </summary>
        public void DeactivateBankoMode()
        {
            if (!string.IsNullOrEmpty(_currentUserTc))
            {
                _userBankoStates.TryRemove(_currentUserTc, out _);
                NotifyUser(_currentUserTc);
            }
        }

        /// <summary>
        /// Belirli bir kullanıcının banko modunu deaktif et
        /// </summary>
        public void DeactivateBankoMode(string tcKimlikNo)
        {
            _userBankoStates.TryRemove(tcKimlikNo, out _);
            NotifyUser(tcKimlikNo);
        }

        /// <summary>
        /// Bu personel banko modunda mı?
        /// </summary>
        public bool IsPersonelInBankoMode(string tcKimlikNo)
        {
            return _userBankoStates.ContainsKey(tcKimlikNo);
        }

        /// <summary>
        /// Belirli bir kullanıcının aktif banko ID'sini getir
        /// </summary>
        public int? GetUserActiveBankoId(string tcKimlikNo)
        {
            return _userBankoStates.TryGetValue(tcKimlikNo, out var bankoId) ? bankoId : null;
        }

        private void NotifyUser(string tcKimlikNo)
        {
            if (_userEvents.TryGetValue(tcKimlikNo, out var handler))
            {
                handler?.Invoke();
            }
        }

        /// <summary>
        /// İzin verilen URL mi? (Banko modunda)
        /// </summary>
        public bool IsUrlAllowedInBankoMode(string url)
        {
            if (!IsInBankoMode) return true;

            // Banko modunda sadece bu URL'lere izin var
            var allowedUrls = new[]
            {
                "/",
                "/siramatik/banko/",
                "/account/logout",
                "/auth/logout",
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
