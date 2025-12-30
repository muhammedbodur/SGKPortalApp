// ═══════════════════════════════════════════════════════════════
// SignalR Manager - ForceLogout & BankoModeExited Event Handlers
// ═══════════════════════════════════════════════════════════════

window.signalRManager = {
    dotNetHelper: null,
    connection: null,

    // MainLayout'tan çağrılır
    registerForceLogoutHandler: function (dotNetHelper) {
        this.dotNetHelper = dotNetHelper;

        // SignalR connection'ı bul
        this.connection = window.blazorSignalR?.connection;

        if (!this.connection) {
            console.warn('⚠️ SignalR connection bulunamadı, yeniden deneniyor...');

            // 1 saniye sonra tekrar dene
            setTimeout(() => {
                this.connection = window.blazorSignalR?.connection;
                if (this.connection) {
                    this.setupForceLogoutListener();
                    this.setupBankoModeExitedListener();
                }
            }, 1000);

            return;
        }

        this.setupForceLogoutListener();
        this.setupBankoModeExitedListener();
    },

    setupForceLogoutListener: function () {
        if (!this.connection) {
            console.error('❌ SignalR connection yok!');
            return;
        }

        // ⭐ Event adı: camelCase (SignalREvents.cs ile uyumlu)
        this.connection.on('forceLogout', (message) => {
            console.warn('🚨 forceLogout event alındı:', message);

            // Alert göster
            alert(message || 'Oturumunuz sonlandırıldı. Lütfen tekrar giriş yapın.');

            // C# tarafına bildir
            if (this.dotNetHelper) {
                this.dotNetHelper.invokeMethodAsync('HandleForceLogout', message);
            } else {
                // Fallback: Doğrudan login sayfasına yönlendir
                window.location.href = '/auth/login';
            }
        });

        console.log('✅ forceLogout event listener kaydedildi');
    },

    setupBankoModeExitedListener: function () {
        if (!this.connection) {
            console.error('❌ SignalR connection yok!');
            return;
        }

        // ⭐ Orphan cleanup tarafından gönderilen event
        this.connection.on('BankoModeExited', (data) => {
            console.warn('🧹 BankoModeExited event alındı:', data);

            // LocalStorage'daki banko mode state'i temizle
            localStorage.removeItem('bankoMode');
            localStorage.removeItem('activeBankoId');
            localStorage.removeItem('bankoModeStartTime');

            // UI'ı yenile (eğer C# helper varsa)
            if (this.dotNetHelper) {
                this.dotNetHelper.invokeMethodAsync('HandleBankoModeExited', data);
            }

            // Sayfa yenile (banko mode panel'i kaybolsun)
            window.location.reload();
        });

        console.log('✅ BankoModeExited event listener kaydedildi');
    }
};
