/**
 * SignalR Bağlantı Yöneticisi
 * Tüm proje için merkezi SignalR bağlantı yönetimi
 */

// Eğer zaten tanımlıysa, tekrar tanımlama
if (typeof SignalRConnectionManager !== 'undefined') {
    console.warn('⚠️ SignalRConnectionManager zaten tanımlı, tekrar yükleme atlanıyor');
} else {

const TAB_SESSION_STORAGE_KEY = "portal.tabSessionId";

function ensureTabSessionId() {
    let tabId = sessionStorage.getItem(TAB_SESSION_STORAGE_KEY);
    if (!tabId) {
        tabId = crypto.randomUUID();
        sessionStorage.setItem(TAB_SESSION_STORAGE_KEY, tabId);
    }
    window.currentTabSessionId = tabId;
    return tabId;
}

class SignalRConnectionManager {
    constructor(hubUrl, reconnectIntervals = [0, 2000, 5000, 10000, 30000]) {
        this.hubUrl = hubUrl;
        this.reconnectIntervals = reconnectIntervals;
        this.connection = null;
        this.connectionCheckInterval = null;
        this.checkIntervalMs = 10000; // 10 saniye
        this.isInitialized = false;
        this.eventHandlers = {};
    }

    /**
     * Bağlantıyı başlat
     */
    async initialize() {
        if (this.isInitialized) {
            console.warn('SignalR bağlantısı zaten başlatılmış');
            return;
        }

        try {
            const tabId = ensureTabSessionId();
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl(`${this.hubUrl}?tabSessionId=${encodeURIComponent(tabId)}`, {
                    accessTokenFactory: () => null,
                    transport: signalR.HttpTransportType.WebSockets,
                })
                .withAutomaticReconnect(this.reconnectIntervals)
                .configureLogging(signalR.LogLevel.Information)
                .build();

            this.setupEventHandlers();
            await this.start();
            this.startConnectionCheck();
            this.isInitialized = true;

            console.log('✅ SignalR bağlantı yöneticisi başlatıldı');
        } catch (error) {
            console.error('❌ SignalR başlatma hatası:', error);
            throw error;
        }
    }

    /**
     * Event handler'ları ayarla
     */
    setupEventHandlers() {
        // Bağlantı kapandığında
        this.connection.onclose((error) => {
            console.log('🔴 SignalR bağlantısı koptu');
            if (error) {
                console.error('Hata:', error);
            }
            this.scheduleReconnect();
        });

        // Yeniden bağlanıldığında
        this.connection.onreconnecting((error) => {
            console.log('🔄 SignalR yeniden bağlanıyor...');
            if (error) {
                console.error('Hata:', error);
            }
        });

        // Yeniden bağlantı başarılı
        this.connection.onreconnected((connectionId) => {
            console.log('✅ SignalR yeniden bağlandı. Connection ID:', connectionId);
            this.triggerEvent('reconnected', connectionId);
        });
    }

    /**
     * Bağlantıyı başlat
     */
    async start() {
        try {
            await this.connection.start();
            console.log('✅ SignalR bağlantısı kuruldu. State:', this.getConnectionState());
            this.triggerEvent('connected');
            return true;
        } catch (error) {
            console.error('❌ SignalR bağlantı hatası:', error);
            this.scheduleReconnect();
            return false;
        }
    }

    /**
     * Yeniden bağlanmayı planla
     */
    scheduleReconnect() {
        setTimeout(async () => {
            console.log('🔄 Yeniden bağlanma denemesi...');
            await this.start();
        }, 5000);
    }

    /**
     * Periyodik bağlantı kontrolü başlat
     */
    startConnectionCheck() {
        if (this.connectionCheckInterval) {
            clearInterval(this.connectionCheckInterval);
        }

        this.connectionCheckInterval = setInterval(() => {
            this.checkConnection();
        }, this.checkIntervalMs);

        console.log(`⏱️ Bağlantı kontrolü başlatıldı (${this.checkIntervalMs / 1000} saniye)`);
    }

    /**
     * Bağlantı durumunu kontrol et
     */
    async checkConnection() {
        const state = this.getConnectionState();
        console.log('🔍 Bağlantı durumu:', state);

        if (state === 'Disconnected') {
            console.log('⚠️ Bağlantı kopuk, yeniden bağlanılıyor...');
            await this.start();
        }

        // ⭐ Banko heartbeat gönder (her zaman, server tarafında kontrol edilir)
        if (state === 'Connected') {
            try {
                await this.connection.invoke('BankoHeartbeat');
                console.log('💓 Banko heartbeat gönderildi');
            } catch (error) {
                console.warn('⚠️ Banko heartbeat hatası:', error);
            }
        }

        this.triggerEvent('statusChecked', state);
    }

    /**
     * Bağlantı durumunu al
     */
    getConnectionState() {
        if (!this.connection) return 'NotInitialized';

        const stateMap = {
            0: 'Disconnected',
            1: 'Connected',
            2: 'Connecting',
            4: 'Reconnecting'
        };

        return stateMap[this.connection.state] || 'Unknown';
    }

    /**
     * Bağlı mı kontrol et
     */
    isConnected() {
        return this.connection && this.connection.state === signalR.HubConnectionState.Connected;
    }

    /**
     * Hub metodunu dinle
     */
    on(methodName, handler) {
        if (!this.connection) {
            console.error('Bağlantı henüz başlatılmadı');
            return;
        }

        this.connection.on(methodName, handler);
        console.log(`📡 Event listener eklendi: ${methodName}`);
    }

    /**
     * Hub metodunu çağır
     */
    async invoke(methodName, ...args) {
        if (!this.isConnected()) {
            console.error('Bağlantı aktif değil');
            return null;
        }

        try {
            const result = await this.connection.invoke(methodName, ...args);
            console.log(`📤 Metod çağrıldı: ${methodName}`, args);
            return result;
        } catch (error) {
            console.error(`❌ Metod çağırma hatası (${methodName}):`, error);
            throw error;
        }
    }

    /**
     * Custom event handler ekle
     */
    addEventListener(eventName, handler) {
        if (!this.eventHandlers[eventName]) {
            this.eventHandlers[eventName] = [];
        }
        this.eventHandlers[eventName].push(handler);
    }

    /**
     * Custom event tetikle
     */
    triggerEvent(eventName, data) {
        if (this.eventHandlers[eventName]) {
            this.eventHandlers[eventName].forEach(handler => {
                try {
                    handler(data);
                } catch (error) {
                    console.error(`Event handler hatası (${eventName}):`, error);
                }
            });
        }
    }

    /**
     * Bağlantıyı kapat
     */
    async stop() {
        if (this.connectionCheckInterval) {
            clearInterval(this.connectionCheckInterval);
            this.connectionCheckInterval = null;
        }

        if (this.connection) {
            try {
                await this.connection.stop();
                console.log('🛑 SignalR bağlantısı kapatıldı');
            } catch (error) {
                console.error('Bağlantı kapatma hatası:', error);
            }
        }

        this.isInitialized = false;
    }

    /**
     * Bağlantı bilgilerini göster
     */
    getInfo() {
        return {
            state: this.getConnectionState(),
            isConnected: this.isConnected(),
            connectionId: this.connection?.connectionId,
            hubUrl: this.hubUrl,
            checkInterval: this.checkIntervalMs
        };
    }
}

// Global instance
window.signalRManager = null;

/**
 * SignalR Manager'ı başlat
 */
function initializeSignalR(hubUrl = '/hubs/tv') {
    if (window.signalRManager) {
        console.warn('SignalR Manager zaten başlatılmış');
        return window.signalRManager;
    }

    window.signalRManager = new SignalRConnectionManager(hubUrl);
    
    // Sayfa kapatılırken temizle
    window.addEventListener('beforeunload', () => {
        if (window.signalRManager) {
            window.signalRManager.stop();
        }
    });

    return window.signalRManager;
}

} // SignalRConnectionManager tanımlama sonu

// Export
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { SignalRConnectionManager, initializeSignalR };
}
