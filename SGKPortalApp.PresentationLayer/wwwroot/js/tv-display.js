// TV Display JavaScript Functions

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

    // SignalR bağlantısı kur
    initializeSignalR: function (tvId) {
        if (typeof signalR === 'undefined') {
            console.error('❌ SignalR kütüphanesi yüklenmemiş!');
            return;
        }

        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/hubs/tv")
            .withAutomaticReconnect()
            .build();

        // Bağlantı kurulduğunda
        connection.start()
            .then(() => {
                console.log('✅ TV ekranı SignalR bağlantısı kuruldu');
                return connection.invoke("JoinTvGroup", tvId);
            })
            .then(() => {
                console.log('✅ TV grubuna katıldı: TV#' + tvId);
            })
            .catch(err => {
                console.error('❌ SignalR bağlantı hatası:', err);
            });

        // Yeniden bağlantı
        connection.onreconnected(() => {
            console.log('✅ TV ekranı yeniden bağlandı');
            connection.invoke("JoinTvGroup", tvId);
        });

        // ForceLogout event handler (TV User sadece 1 tab)
        connection.on("ForceLogout", function (message) {
            console.warn("⚠️ ForceLogout alındı:", message);
            
            // Kullanıcıya bilgi ver
            alert(message);
            
            // LocalStorage ve SessionStorage temizle
            localStorage.clear();
            sessionStorage.clear();
            
            // Cookie'leri temizle
            document.cookie.split(";").forEach((c) => {
                document.cookie = c.replace(/^ +/, "").replace(/=.*/, "=;expires=" + new Date().toUTCString() + ";path=/");
            });
            
            // Login sayfasına yönlendir
            window.location.href = "/Account/Login";
        });

        // Sıra güncelleme event'i
        connection.on("ReceiveSiraUpdate", function (data) {
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
        connection.on("ReceiveDuyuruUpdate", function (duyuru) {
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
