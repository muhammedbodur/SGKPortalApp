using System.Collections.Concurrent;

namespace SGKPortalApp.PresentationLayer.Services.State
{
    /// <summary>
    /// Banko modu state yönetimi
    /// Singleton olarak çalışır, kullanıcı bazlı banko modu durumunu tutar
    /// ⚠️ AsyncLocal kullanarak thread-safe context isolation sağlar
    /// </summary>
    public class BankoModeStateService
    {
        // Kullanıcı bazlı state (TcKimlikNo -> BankoId)
        private readonly ConcurrentDictionary<string, int> _userBankoStates = new();

        // Event'ler kullanıcı bazlı (TcKimlikNo -> Event)
        private readonly ConcurrentDictionary<string, Action?> _userEvents = new();

        // ⭐ AsyncLocal: Her async context için ayrı değer (thread-safe, race condition yok!)
        private static readonly AsyncLocal<string?> _currentUserTc = new();

        /// <summary>
        /// Mevcut kullanıcıyı set et (Her request başında çağrılmalı)
        /// ⭐ AsyncLocal kullanarak context-specific value set eder
        /// </summary>
        public void SetCurrentUser(string tcKimlikNo)
        {
            _currentUserTc.Value = tcKimlikNo;
        }

        /// <summary>
        /// Banko modu aktif mi? (Mevcut kullanıcı için)
        /// </summary>
        public bool IsInBankoMode => !string.IsNullOrEmpty(_currentUserTc.Value) && _userBankoStates.ContainsKey(_currentUserTc.Value);

        /// <summary>
        /// Aktif banko ID (Mevcut kullanıcı için)
        /// </summary>
        public int? ActiveBankoId => !string.IsNullOrEmpty(_currentUserTc.Value) && _userBankoStates.TryGetValue(_currentUserTc.Value, out var bankoId) ? bankoId : null;

        /// <summary>
        /// Banko modundaki personel TC Kimlik No (Mevcut kullanıcı için)
        /// </summary>
        public string? PersonelTcKimlikNo => IsInBankoMode ? _currentUserTc.Value : null;

        /// <summary>
        /// Banko modu değişiklik event'i (Mevcut kullanıcı için)
        /// ⚠️ AsyncLocal context'inden TcKimlikNo alır
        /// </summary>
        public event Action? OnBankoModeChanged
        {
            add
            {
                var tc = _currentUserTc.Value;
                if (!string.IsNullOrEmpty(tc))
                {
                    _userEvents.AddOrUpdate(tc, value, (_, existing) => existing + value);
                }
            }
            remove
            {
                var tc = _currentUserTc.Value;
                if (!string.IsNullOrEmpty(tc) && _userEvents.TryGetValue(tc, out var existing))
                {
                    _userEvents[tc] = existing - value;
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
        /// Banko modunu deaktif et (Mevcut kullanıcı için)
        /// ⚠️ AsyncLocal context'inden TcKimlikNo alır
        /// </summary>
        public void DeactivateBankoMode()
        {
            var tc = _currentUserTc.Value;
            if (!string.IsNullOrEmpty(tc))
            {
                _userBankoStates.TryRemove(tc, out _);
                NotifyUser(tc);
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
