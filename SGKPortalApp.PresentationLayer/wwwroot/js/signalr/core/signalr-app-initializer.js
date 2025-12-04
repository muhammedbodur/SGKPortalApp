/**
 * SignalR Uygulama Başlatıcı
 * Tüm SignalR bağlantı yönetimini merkezi olarak başlatır ve yönetir
 * 
 * Özellikler:
 * - Tek seferlik başlatma (SPA navigation'da tekrar başlatmaz)
 * - ForceLogout event yönetimi
 * - Banko mode entegrasyonu
 * - Otomatik yeniden bağlanma
 * - Connection state monitoring
 */

(function() {
    'use strict';

    // ============================================
    // GUARD: SPA navigation'da tekrar başlatmayı engelle
    // NOT: Full page refresh'te yeniden başlatılmalı (sessionStorage korunur)
    // ============================================
    if (window.signalRAppInitialized) {
        console.log('⚠️ SignalR App zaten başlatılmış (SPA navigation), tekrar başlatma atlanıyor');
        return;
    }
    
    window.signalRAppInitialized = true;
    console.log('🚀 SignalR App Initializer başlatılıyor...');

    // ============================================
    // CONFIGURATION
    // ============================================
    const CONFIG = {
        // ⭐ ApiLayer Hub URL'ini kullan (PresentationLayer değil!)
        // appsettings.json'dan gelen API URL'i kullanır (dinamik)
        hubUrl: window.appConfig?.apiUrl ? `${window.appConfig.apiUrl}/hubs/siramatik` : 'https://localhost:9080/hubs/siramatik',
        loginRedirectUrl: '/Account/Login',
        enableDebugLogs: true
    };

    // ============================================
    // UTILITY FUNCTIONS
    // ============================================
    const Logger = {
        info: (msg, ...args) => CONFIG.enableDebugLogs && console.log(`ℹ️ ${msg}`, ...args),
        success: (msg, ...args) => console.log(`✅ ${msg}`, ...args),
        warn: (msg, ...args) => console.warn(`⚠️ ${msg}`, ...args),
        error: (msg, ...args) => console.error(`❌ ${msg}`, ...args)
    };

    // ============================================
    // FORCE LOGOUT HANDLER
    // ============================================
    class ForceLogoutHandler {
        constructor() {
            this.isAttached = false;
            this.isProcessing = false;
        }

        attach(connection) {
            if (!connection || this.isAttached) {
                return;
            }

            this.isAttached = true;

            // ⭐ Event adı: camelCase (SignalREvents.cs ile uyumlu)
            connection.on("forceLogout", (message) => {
                Logger.warn("forceLogout event alındı:", message);
                this.handleForceLogout(message);
            });

            Logger.success("forceLogout handler bağlandı");
        }

        handleForceLogout(message) {
            // Zaten işlem yapılıyorsa, tekrar yapma (loop önleme)
            if (this.isProcessing) {
                Logger.warn("ForceLogout zaten işleniyor, tekrar atlanıyor");
                return;
            }

            this.isProcessing = true;

            // Kullanıcıya bilgi ver
            alert(message || "Oturumunuz sonlandırıldı. Lütfen tekrar giriş yapın.");

            // LocalStorage temizle (sessionStorage'ı koru - tabSessionId için)
            this.clearLocalStorage();

            // Cookie'leri temizle
            this.clearCookies();

            // Login sayfasına yönlendir
            this.redirectToLogin();
        }

        clearLocalStorage() {
            try {
                localStorage.clear();
                Logger.info("LocalStorage temizlendi");
            } catch (e) {
                Logger.error("LocalStorage temizleme hatası:", e);
            }
        }

        clearCookies() {
            try {
                document.cookie.split(";").forEach((cookie) => {
                    const cookieName = cookie.split("=")[0].trim();
                    document.cookie = `${cookieName}=;expires=Thu, 01 Jan 1970 00:00:00 UTC;path=/`;
                });
                Logger.info("Cookies temizlendi");
            } catch (e) {
                Logger.error("Cookie temizleme hatası:", e);
            }
        }

        redirectToLogin() {
            Logger.info("Login sayfasına yönlendiriliyor...");
            window.location.href = CONFIG.loginRedirectUrl;
        }
    }

    // ============================================
    // BANKO MODE INTEGRATION
    // ============================================
    class BankoModeIntegration {
        setConnection(connection) {
            if (window.bankoMode && typeof window.bankoMode.setConnection === 'function') {
                window.bankoMode.setConnection(connection);
                Logger.success("Banko mode bağlantısı ayarlandı");
            } else {
                Logger.warn("window.bankoMode bulunamadı veya setConnection metodu yok");
            }
        }
    }

    // ============================================
    // SIGNALR APP MANAGER
    // ============================================
    class SignalRAppManager {
        constructor(config) {
            this.config = config;
            this.manager = null;
            this.forceLogoutHandler = new ForceLogoutHandler();
            this.bankoModeIntegration = new BankoModeIntegration();
        }

        async initialize() {
            try {
                Logger.info("SignalR Manager oluşturuluyor...");
                
                // SignalR Connection Manager'ı başlat
                this.manager = initializeSignalR(this.config.hubUrl);
                
                // Event listener'ları ekle
                this.setupEventListeners();
                
                // Bağlantıyı başlat
                await this.manager.initialize();
                
                Logger.success("SignalR App başarıyla başlatıldı");
                
                // Global erişim için expose et
                window.signalRApp = this;
                
                return true;
            } catch (error) {
                Logger.error("SignalR App başlatma hatası:", error);
                throw error;
            }
        }

        setupEventListeners() {
            // Connected event
            this.manager.addEventListener('connected', () => {
                Logger.success("SignalR bağlantısı kuruldu");
                this.onConnected();
            });

            // Reconnected event
            this.manager.addEventListener('reconnected', () => {
                Logger.success("SignalR yeniden bağlandı");
                this.onReconnected();
            });

            // Status checked event
            this.manager.addEventListener('statusChecked', (state) => {
                if (state === 'Disconnected') {
                    Logger.warn("SignalR bağlantısı koptu");
                }
            });

            Logger.info("Event listener'lar ayarlandı");
        }

        onConnected() {
            const connection = this.manager.connection;
            
            // ForceLogout handler'ı bağla
            this.forceLogoutHandler.attach(connection);
            
            // Banko mode'a bağlantıyı set et
            this.bankoModeIntegration.setConnection(connection);
        }

        onReconnected() {
            const connection = this.manager.connection;
            
            // ForceLogout handler'ı bağla (tekrar bağlanma durumunda)
            this.forceLogoutHandler.attach(connection);
            
            // Banko mode'a bağlantıyı set et
            this.bankoModeIntegration.setConnection(connection);
        }

        // Public API
        getManager() {
            return this.manager;
        }

        getConnection() {
            return this.manager?.connection;
        }

        isConnected() {
            return this.manager?.isConnected() || false;
        }

        async invoke(methodName, ...args) {
            if (!this.manager) {
                Logger.error("SignalR Manager henüz başlatılmadı");
                return null;
            }
            return await this.manager.invoke(methodName, ...args);
        }

        on(eventName, handler) {
            if (!this.manager?.connection) {
                Logger.error("SignalR Connection henüz başlatılmadı");
                return;
            }
            this.manager.connection.on(eventName, handler);
        }
    }

    // ============================================
    // AUTO-START
    // ============================================
    (async function autoStart() {
        try {
            // Login sayfasında SignalR başlatma (sonsuz loop'u önler)
            if (window.location.pathname.toLowerCase().includes('/auth/login') ||
                window.location.pathname.toLowerCase().includes('/account/login')) {
                Logger.info("Login sayfası algılandı, SignalR başlatılmıyor");
                return;
            }

            // Page lifecycle bilgisini logla
            if (window.pageLifecycle) {
                const lifecycle = window.pageLifecycle.getInfo();
                if (lifecycle.isRefresh) {
                    Logger.info("🔄 Sayfa refresh algılandı");
                } else if (lifecycle.isNewTab) {
                    Logger.info("🆕 Yeni tab algılandı");
                } else {
                    Logger.info("📍 Normal navigation algılandı");
                }
            }

            const app = new SignalRAppManager(CONFIG);
            await app.initialize();
            
            Logger.success("🎉 SignalR App hazır!");
            
        } catch (error) {
            Logger.error("SignalR App auto-start hatası:", error);
        }
    })();

})();
