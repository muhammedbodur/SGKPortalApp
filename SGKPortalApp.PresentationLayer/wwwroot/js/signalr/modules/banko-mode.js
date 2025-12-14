// Banko Modu Yönetimi
window.bankoMode = {
    connection: null,
    dotNetHelper: null,
    eventHandlersSetup: false,

    // SignalR bağlantısını al (MainLayout'tan)
    getConnection: function () {
        if (!this.connection) {
            console.error('❌ SignalR bağlantısı bulunamadı!');
            return null;
        }
        return this.connection;
    },

    // Bağlantıyı set et (signalr-app-initializer'dan çağrılır)
    setConnection: function (connection) {
        this.connection = connection;
        console.log('✅ Banko modu SignalR bağlantısı ayarlandı');
        
        // ⭐ Eğer dotNetHelper zaten set edilmişse, event handler'ları kur
        if (this.dotNetHelper && !this.eventHandlersSetup) {
            console.log('🔄 Connection geldi, event handler\'ları kuruluyor...');
            this._setupEventHandlersInternal();
        }
    },

    // Aktif tab'ın ConnectionId'sini al
    getCurrentConnectionId: function () {
        const connection = this.getConnection();
        if (!connection) {
            console.error('❌ SignalR bağlantısı bulunamadı!');
            return null;
        }
        return connection.connectionId;
    },

    // Banko moduna geç
    enter: async function (bankoId) {
        const connection = this.getConnection();
        if (!connection) {
            alert('SignalR bağlantısı kurulamadı!');
            return false;
        }

        try {
            console.log(`🏦 Banko moduna geçiliyor: Banko#${bankoId}`);
            await connection.invoke("EnterBankoMode", bankoId);
            console.log(`✅ Banko#${bankoId} moduna girildi`);
            return true;
        } catch (err) {
            console.error('❌ Banko moduna giriş hatası:', err);
            alert(err.message || 'Banko moduna geçilemedi!');
            return false;
        }
    },

    // Banko modundan çık
    exit: async function () {
        const connection = this.getConnection();
        if (!connection) {
            alert('SignalR bağlantısı kurulamadı!');
            return false;
        }

        try {
            console.log('🚪 Banko modundan çıkılıyor...');
            await connection.invoke("ExitBankoMode");
            console.log('✅ Banko modundan çıkıldı');
            return true;
        } catch (err) {
            console.error('❌ Banko modundan çıkış hatası:', err);
            alert(err.message || 'Banko modundan çıkılamadı!');
            return false;
        }
    },

    // Event handler'ları kur (MainLayout.OnAfterRenderAsync'den çağrılır)
    // ⭐ Event adları: camelCase formatında (SignalREvents.cs ile uyumlu)
    setupEventHandlers: function (dotNetHelper) {
        this.dotNetHelper = dotNetHelper;
        console.log('📝 dotNetHelper kaydedildi');
        
        // Connection henüz hazır değilse, setConnection çağrıldığında kurulacak
        if (!this.connection) {
            console.log('⏳ Connection henüz hazır değil, event handler\'lar connection geldiğinde kurulacak');
            return;
        }
        
        this._setupEventHandlersInternal();
    },
    
    // Internal: Event handler'ları gerçekten kur
    _setupEventHandlersInternal: function () {
        if (this.eventHandlersSetup) {
            console.log('⚠️ Event handler\'lar zaten kurulmuş');
            return;
        }
        
        const connection = this.connection;
        const dotNetHelper = this.dotNetHelper;
        
        if (!connection || !dotNetHelper) {
            console.error('❌ Connection veya dotNetHelper eksik!');
            return;
        }

        // Banko modu aktif oldu
        connection.on("bankoModeActivated", (data) => {
            console.log('✅ bankoModeActivated:', data);
            if (this.dotNetHelper) {
                this.dotNetHelper.invokeMethodAsync('OnBankoModeActivated', data.bankoId)
                    .then(() => {
                        console.log('✅ C# OnBankoModeActivated tamamlandı - UI Blazor tarafından güncellenecek');
                    })
                    .catch(err => {
                        console.error('❌ OnBankoModeActivated çağrısı başarısız:', err);
                    });
            }
        });

        // Banko modu deaktif oldu
        connection.on("bankoModeDeactivated", (data) => {
            console.log('✅ bankoModeDeactivated');
            if (this.dotNetHelper) {
                this.dotNetHelper.invokeMethodAsync('OnBankoModeDeactivated')
                    .then(() => {
                        console.log('✅ C# OnBankoModeDeactivated tamamlandı - UI Blazor tarafından güncellenecek');
                    })
                    .catch(err => {
                        console.error('❌ OnBankoModeDeactivated çağrısı başarısız:', err);
                    });
            }
        });

        // Banko modu hatası
        connection.on("bankoModeError", (data) => {
            console.error('❌ bankoModeError:', data);
            alert(data.error || 'Banko modu hatası!');
        });

        // Force logout
        connection.on("forceLogout", (message) => {
            console.warn('🚨 forceLogout:', message);
            alert(message || 'Oturumunuz sonlandırıldı!');
            window.location.href = '/auth/login';
        });

        // Permissions changed
        connection.on("permissionsChanged", (data) => {
            console.log('🔑 permissionsChanged:', data);
            if (this.dotNetHelper) {
                this.dotNetHelper.invokeMethodAsync('OnPermissionsChanged')
                    .catch(err => {
                        console.error('❌ OnPermissionsChanged çağrısı başarısız:', err);
                    });
            }
        });

        // ⭐ Sıra listesi güncelleme (Kiosk'tan yeni sıra geldiğinde) - ESKİ
        connection.on("siraListUpdate", (payload) => {
            console.log('📥 siraListUpdate alındı:', payload);
            
            // SiraCagirmaPanel varsa güncelle
            if (typeof SiraCagirmaPanel !== 'undefined' && typeof SiraCagirmaPanel.handleSiraUpdate === 'function') {
                SiraCagirmaPanel.handleSiraUpdate(payload);
            } else {
                console.warn('⚠️ SiraCagirmaPanel bulunamadı veya handleSiraUpdate metodu yok');
            }
        });

        // ⭐ Banko Panel Sıra Güncellemesi (Kiosk sıra alma veya yönlendirme sonrası)
        // Her personele kendi güncel sıra listesi gönderilir
        connection.on("BankoPanelSiraGuncellemesi", (payload) => {
            console.log('📥 BankoPanelSiraGuncellemesi alındı:', payload);
            
            // SiraCagirmaPanel varsa güncelle
            if (typeof SiraCagirmaPanel !== 'undefined' && typeof SiraCagirmaPanel.handleBankoPanelGuncellemesi === 'function') {
                SiraCagirmaPanel.handleBankoPanelGuncellemesi(payload);
            } else {
                console.warn('⚠️ SiraCagirmaPanel bulunamadı veya handleBankoPanelGuncellemesi metodu yok');
            }
        });

        this.eventHandlersSetup = true;
        console.log('✅ Banko modu event handlerlari kuruldu (camelCase)');
    },

    // Sayfa yüklendiğinde banko modu kontrolü
    checkBankoModeOnLoad: function () {
        const bankoModeData = localStorage.getItem('bankoMode');
        if (bankoModeData) {
            try {
                const data = JSON.parse(bankoModeData);
                if (data.active) {
                    console.log('⚠️ Banko modu aktif, URL kontrolü yapılıyor...');
                    
                    const currentUrl = window.location.pathname;
                    const allowedUrls = [
                        '/',
                        '/siramatik/banko/',
                        '/account/logout'
                    ];
                    
                    const isAllowed = allowedUrls.some(url => currentUrl.startsWith(url));
                    
                    if (!isAllowed) {
                        console.warn('❌ Bu sayfaya banko modunda erişim yok!');
                        window.location.href = `/siramatik/banko/${data.bankoId}`;
                    }
                }
            } catch (e) {
                console.error('Banko modu kontrolü hatası:', e);
            }
        }
    }
};

// Sayfa yüklendiğinde kontrol et
window.addEventListener('DOMContentLoaded', () => {
    window.bankoMode.checkBankoModeOnLoad();
});

// Tab'lar arası mesajlaşma (Diğer tab'ları kapatmak için)
// NOT: Artık kullanılmıyor - C# tarafında yönetiliyor
// window.addEventListener('message', (event) => {
//     if (event.data.type === 'CLOSE_ALL_TABS') {
//         console.warn('⚠️ Banko moduna geçildi, bu tab kapatılıyor...');
//         window.close();
//     }
// });
