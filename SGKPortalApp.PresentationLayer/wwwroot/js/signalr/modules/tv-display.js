// TV Display JavaScript Functions
console.log('📺 tv-display.js yüklendi');

window.tvDisplay = {
    // Saat ve Tarih
    startClock: function () {
        function updateTime() {
            const today = new Date();
            let h = today.getHours();
            let m = today.getMinutes();
            let s = today.getSeconds();
            const day = today.toLocaleDateString("tr-TR", { weekday: "long", year: "numeric", month: "long", day: "numeric" });

            h = h < 10 ? "0" + h : h;
            m = m < 10 ? "0" + m : m;
            s = s < 10 ? "0" + s : s;

            const saatEl = document.getElementById("saat");
            const tarihEl = document.getElementById("tarih");
            
            if (saatEl) saatEl.innerHTML = h + ":" + m + ":" + s;
            if (tarihEl) tarihEl.innerHTML = day;
        }

        updateTime();
        setInterval(updateTime, 1000);
    },

    // Ses çalma
    playSound: function () {
        const sound = document.getElementById('dingDongSound');
        if (sound) {
            if (sound.readyState >= 2) {
                sound.currentTime = 0;
                sound.play().catch(e => console.log('Ses çalma hatası:', e));
            } else {
                sound.load();
                sound.addEventListener('canplay', function () {
                    sound.play().catch(e => console.log('Ses çalma hatası:', e));
                }, { once: true });
            }
        }
    },

    // Yeni sıra animasyonu
    highlightSira: function (bankoId) {
        const card = document.getElementById('banko_' + bankoId);
        if (card) {
            card.classList.add('new-sira');
            setTimeout(() => {
                card.classList.remove('new-sira');
            }, 2000);
        }
    },

    // Video başlat
    startVideo: function () {
        const video = document.getElementById('tvVideo');
        if (video) {
            video.play().catch(e => console.log('Video autoplay hatası:', e));
        }
    },

    // ConnectionType'ı TvMode'a güncelle
    updateConnectionTypeToTvMode: function () {
        console.log('🔄 ConnectionType TvMode olarak güncelleniyor...');
        
        if (!window.signalRManager || !window.signalRManager.connection) {
            console.error('❌ SignalR Manager bulunamadı!');
            return;
        }

        const connection = window.signalRManager.connection;
        
        if (connection.state === signalR.HubConnectionState.Connected) {
            connection.invoke("UpdateConnectionType", "TvMode")
                .then(() => {
                    console.log('✅ ConnectionType TvMode olarak güncellendi');
                })
                .catch(err => {
                    console.error('❌ ConnectionType güncelleme hatası:', err);
                });
        } else {
            console.warn('⚠️ SignalR henüz bağlı değil, ConnectionType güncellenemedi');
        }
    },

    // SignalR bağlantısı kur
    initializeSignalR: function (tvId) {
        console.log('📺 TV Display SignalR başlatılıyor, TV#' + tvId);

        // Global SignalR Manager'ı kullan (zaten _Host.cshtml'de başlatılmış)
        if (!window.signalRManager || !window.signalRManager.connection) {
            console.error('❌ Global SignalR Manager bulunamadı! Sayfa yenileniyor...');
            setTimeout(() => location.reload(), 2000);
            return;
        }

        const connection = window.signalRManager.connection;

        // Bağlantı zaten kurulu, sadece TV grubuna katıl
        if (connection.state === signalR.HubConnectionState.Connected) {
            console.log('✅ SignalR zaten bağlı, TV grubuna katılıyor...');
            connection.invoke("JoinTvGroup", tvId)
                .then(() => {
                    console.log('✅ TV grubuna katıldı: TV#' + tvId);
                })
                .catch(err => {
                    console.error('❌ TV grubuna katılma hatası:', err);
                });
        } else {
            // Bağlantı henüz kurulmamış, kurulmasını bekle
            console.log('⏳ SignalR bağlantısı bekleniyor...');
            const checkInterval = setInterval(() => {
                if (connection.state === signalR.HubConnectionState.Connected) {
                    clearInterval(checkInterval);
                    console.log('✅ SignalR bağlandı, TV grubuna katılıyor...');
                    connection.invoke("JoinTvGroup", tvId)
                        .then(() => {
                            console.log('✅ TV grubuna katıldı: TV#' + tvId);
                        })
                        .catch(err => {
                            console.error('❌ TV grubuna katılma hatası:', err);
                        });
                }
            }, 500);

            // 10 saniye sonra timeout
            setTimeout(() => {
                clearInterval(checkInterval);
                if (connection.state !== signalR.HubConnectionState.Connected) {
                    console.error('❌ SignalR bağlantısı kurulamadı, sayfa yenileniyor...');
                    location.reload();
                }
            }, 10000);
        }

        // Yeniden bağlantı event'i
        connection.onreconnected(() => {
            console.log('✅ TV ekranı yeniden bağlandı, TV grubuna tekrar katılıyor...');
            connection.invoke("JoinTvGroup", tvId)
                .catch(err => console.error('❌ Yeniden bağlantıda TV grubuna katılma hatası:', err));
        });

        // ⭐ Event adları: camelCase (SignalREvents.cs ile uyumlu)
        // Sıra güncelleme event'i
        connection.on("receiveSiraUpdate", function (data) {
            console.log("🔔 Yeni sıra çağrıldı:", data);
            
            // Ses çal
            window.tvDisplay.playSound();

            // Yeni sırayı vurgula
            if (data.bankoId) {
                window.tvDisplay.highlightSira(data.bankoId);
            }

            // Sayfayı yenile
            setTimeout(() => {
                location.reload();
            }, 2000);
        });

        // Duyuru güncelleme event'i
        connection.on("receiveDuyuruUpdate", function (duyuru) {
            console.log("📢 Duyuru güncellendi:", duyuru);
            const duyuruText = document.getElementById('duyuruText');
            if (duyuruText) {
                duyuruText.textContent = duyuru;
            }
        });

        // Tam ekran modu için F11 tuşu
        document.addEventListener('keydown', function (e) {
            if (e.key === 'F11') {
                e.preventDefault();
                if (!document.fullscreenElement) {
                    document.documentElement.requestFullscreen();
                    document.body.classList.add('fullscreen');
                } else {
                    document.exitFullscreen();
                    document.body.classList.remove('fullscreen');
                }
            }
        });

        console.log('Ekran çözünürlüğü:', window.screen.width + 'x' + window.screen.height);
        console.log('Pencere boyutu:', window.innerWidth + 'x' + window.innerHeight);
    }
};
