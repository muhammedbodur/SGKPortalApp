/**
 * PDKS Realtime Monitor - SignalR Client
 * ZKTeco cihazlarından gelen realtime event'leri dinler ve Blazor'a bildirir
 */

window.PdksRealtimeMonitor = {
    connection: null,
    dotNetRef: null,
    isInitialized: false,

    /**
     * SignalR bağlantısını başlat
     * @param {string} hubUrl - PdksHub URL'i (örn: https://localhost:9080/hubs/pdks)
     * @param {object} dotNetReference - Blazor component referansı
     */
    initialize: async function (hubUrl, dotNetReference) {
        if (this.isInitialized) {
            console.warn('PDKS Realtime Monitor zaten başlatılmış');
            return;
        }

        this.dotNetRef = dotNetReference;

        console.log('🔌 PDKS Realtime Monitor başlatılıyor:', hubUrl);

        try {
            // SignalR connection oluştur
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl(hubUrl, {
                    transport: signalR.HttpTransportType.WebSockets,
                    withCredentials: true  // Cookie ve Auth header'ları gönder
                })
                .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])  // Otomatik yeniden bağlanma
                .configureLogging(signalR.LogLevel.Information)
                .build();

            // Event handler'ları kaydet
            this.setupEventHandlers();

            // Bağlantıyı başlat
            await this.start();

            this.isInitialized = true;
            console.log('✅ PDKS Realtime Monitor başlatıldı');

        } catch (error) {
            console.error('❌ PDKS Realtime Monitor başlatma hatası:', error);
            this.notifyConnectionState(false);
            throw error;
        }
    },

    /**
     * Event handler'ları ayarla
     */
    setupEventHandlers: function () {
        // Realtime event geldiğinde
        this.connection.on('OnRealtimeEvent', (eventData) => {
            console.log('📥 Realtime event alındı:', eventData);
            this.handleRealtimeEvent(eventData);
        });

        // Bağlantı başarılı olduğunda
        this.connection.on('OnConnected', (data) => {
            console.log('✅ PdksHub bağlantısı kuruldu:', data);
            this.notifyConnectionState(true);
        });

        // Bağlantı kapandığında
        this.connection.onclose((error) => {
            console.log('🔴 PdksHub bağlantısı koptu');
            if (error) {
                console.error('Hata:', error);
            }
            this.notifyConnectionState(false);
        });

        // Yeniden bağlanırken
        this.connection.onreconnecting((error) => {
            console.log('🔄 PdksHub yeniden bağlanıyor...');
            if (error) {
                console.error('Hata:', error);
            }
            this.notifyConnectionState(false);
        });

        // Yeniden bağlandığında
        this.connection.onreconnected((connectionId) => {
            console.log('✅ PdksHub yeniden bağlandı. Connection ID:', connectionId);
            this.notifyConnectionState(true);
        });
    },

    /**
     * Bağlantıyı başlat
     */
    start: async function () {
        try {
            await this.connection.start();
            console.log('🚀 SignalR bağlantısı başlatıldı. State:', this.connection.state);
            this.notifyConnectionState(true);
        } catch (error) {
            console.error('❌ Bağlantı başlatma hatası:', error);
            this.notifyConnectionState(false);

            // 5 saniye sonra tekrar dene
            setTimeout(() => this.start(), 5000);
        }
    },

    /**
     * Realtime event'i işle ve Blazor'a bildir
     */
    handleRealtimeEvent: function (eventData) {
        try {
            // Blazor component'e event'i gönder
            if (this.dotNetRef) {
                this.dotNetRef.invokeMethodAsync('OnRealtimeEvent', eventData)
                    .then(() => {
                        console.log('✅ Event Blazor'a gönderildi');
                    })
                    .catch((error) => {
                        console.error('❌ Blazor invoke hatası:', error);
                    });
            }

            // Opsiyonel: Tarayıcıda bildirim göster
            if (Notification.permission === 'granted') {
                this.showNotification(eventData);
            }

            // Opsiyonel: Ses çal
            this.playNotificationSound();

        } catch (error) {
            console.error('❌ Event işleme hatası:', error);
        }
    },

    /**
     * Bağlantı durumunu Blazor'a bildir
     */
    notifyConnectionState: function (isConnected) {
        if (this.dotNetRef) {
            this.dotNetRef.invokeMethodAsync('OnConnectionStateChanged', isConnected)
                .catch((error) => {
                    console.error('❌ Connection state bildirim hatası:', error);
                });
        }
    },

    /**
     * Tarayıcı bildirimi göster
     */
    showNotification: function (eventData) {
        try {
            const verifyMethodMap = {
                0: 'Şifre',
                1: 'Parmak İzi',
                3: 'Yüz Tanıma',
                15: 'Kart'
            };

            const inOutModeMap = {
                0: 'Giriş',
                1: 'Çıkış',
                2: 'Mola Başlangıç',
                3: 'Mola Bitiş'
            };

            const title = `PDKS - ${inOutModeMap[eventData.inOutMode] || 'Bilinmeyen'}`;
            const body = `Sicil: ${eventData.enrollNumber}\nDoğrulama: ${verifyMethodMap[eventData.verifyMethod] || eventData.verifyMethod}\nCihaz: ${eventData.deviceIp}`;

            new Notification(title, {
                body: body,
                icon: '/img/logo.png',
                badge: '/img/badge.png',
                tag: 'pdks-realtime'
            });
        } catch (error) {
            console.error('Notification hatası:', error);
        }
    },

    /**
     * Bildirim sesi çal
     */
    playNotificationSound: function () {
        try {
            const audio = new Audio('/sounds/notification.mp3');
            audio.volume = 0.3;
            audio.play().catch((error) => {
                console.log('Ses çalma hatası (kullanıcı interaction gerekebilir):', error);
            });
        } catch (error) {
            console.error('Ses çalma hatası:', error);
        }
    },

    /**
     * Belirli bir cihazı dinlemeye başla
     */
    subscribeToDevice: async function (deviceIp) {
        if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
            try {
                await this.connection.invoke('SubscribeToDevice', deviceIp);
                console.log(`✅ Cihaz dinleme başlatıldı: ${deviceIp}`);
            } catch (error) {
                console.error(`❌ Cihaz dinleme hatası (${deviceIp}):`, error);
            }
        }
    },

    /**
     * Cihaz dinlemeyi durdur
     */
    unsubscribeFromDevice: async function (deviceIp) {
        if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
            try {
                await this.connection.invoke('UnsubscribeFromDevice', deviceIp);
                console.log(`✅ Cihaz dinleme durduruldu: ${deviceIp}`);
            } catch (error) {
                console.error(`❌ Cihaz dinleme durdurma hatası (${deviceIp}):`, error);
            }
        }
    },

    /**
     * Ping - Bağlantı testi
     */
    ping: async function () {
        if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
            try {
                const response = await this.connection.invoke('Ping');
                console.log('🏓 Ping response:', response);
                return response;
            } catch (error) {
                console.error('❌ Ping hatası:', error);
                return null;
            }
        }
    },

    /**
     * Bağlantıyı durdur
     */
    stop: async function () {
        if (this.connection) {
            try {
                await this.connection.stop();
                console.log('⏹️ PDKS Realtime Monitor durduruldu');
                this.isInitialized = false;
                this.notifyConnectionState(false);
            } catch (error) {
                console.error('❌ Durdurma hatası:', error);
            }
        }
    },

    /**
     * Tarayıcı bildirimi izni iste
     */
    requestNotificationPermission: async function () {
        if ('Notification' in window && Notification.permission === 'default') {
            try {
                const permission = await Notification.requestPermission();
                console.log('Notification permission:', permission);
                return permission === 'granted';
            } catch (error) {
                console.error('Notification permission hatası:', error);
                return false;
            }
        }
        return Notification.permission === 'granted';
    }
};

// Sayfa kapatılırken bağlantıyı temizle
window.addEventListener('beforeunload', () => {
    if (window.PdksRealtimeMonitor && window.PdksRealtimeMonitor.connection) {
        window.PdksRealtimeMonitor.stop();
    }
});
