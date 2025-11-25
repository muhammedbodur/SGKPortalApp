// Banko Modu Yönetimi
window.bankoMode = {
    connection: null,

    // SignalR bağlantısını al (MainLayout'tan)
    getConnection: function () {
        if (!this.connection) {
            console.error('❌ SignalR bağlantısı bulunamadı!');
            return null;
        }
        return this.connection;
    },

    // Bağlantıyı set et (MainLayout'tan çağrılır)
    setConnection: function (connection) {
        this.connection = connection;
        console.log('✅ Banko modu SignalR bağlantısı ayarlandı');
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

    // Event handler'ları kur
    setupEventHandlers: function (dotNetHelper) {
        const connection = this.getConnection();
        if (!connection) return;

        // Banko modu aktif oldu
        connection.on("BankoModeActivated", (data) => {
            console.log('✅ BankoModeActivated:', data);
            if (dotNetHelper) {
                dotNetHelper.invokeMethodAsync('OnBankoModeActivated', data.bankoId);
            }
        });

        // Banko modu deaktif oldu
        connection.on("BankoModeDeactivated", (data) => {
            console.log('✅ BankoModeDeactivated');
            if (dotNetHelper) {
                dotNetHelper.invokeMethodAsync('OnBankoModeDeactivated');
            }
        });

        // Banko modu hatası
        connection.on("BankoModeError", (data) => {
            console.error('❌ BankoModeError:', data);
            alert(data.error || 'Banko modu hatası!');
        });

        console.log('✅ Banko modu event handlerlari kuruldu');
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
