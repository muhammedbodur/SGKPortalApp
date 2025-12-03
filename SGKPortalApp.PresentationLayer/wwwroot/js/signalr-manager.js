// ═══════════════════════════════════════════════════════════════
// SignalR Manager - ForceLogout Event Handler
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
                }
            }, 1000);
            
            return;
        }
        
        this.setupForceLogoutListener();
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
    }
};
