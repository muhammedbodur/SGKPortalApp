// TV Display JavaScript Functions
console.log('📺 tv-display.js yüklendi');

window.tvDisplay = {
    // TV ID (SignalR bağlantısı için)
    _tvId: null,

    // Overlay kuyruğu
    _overlayQueue: [],
    _isShowingOverlay: false,

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

    // ⭐ Sıra çağırma overlay popup göster (kuyruk sistemi ile)
    showSiraCalledOverlay: function (siraNo, bankoNo, katTipi, bankoTipi) {
        // Kuyruğa ekle
        this._overlayQueue.push({ siraNo, bankoNo, katTipi, bankoTipi });
        console.log('📺 Overlay kuyruğa eklendi: Sıra#' + siraNo + ' -> Banko#' + bankoNo + ' (Kuyruk: ' + this._overlayQueue.length + ')');

        // Eğer şu an overlay gösterilmiyorsa, kuyruğu işlemeye başla
        if (!this._isShowingOverlay) {
            this._processOverlayQueue();
        }
    },

    // Overlay kuyruğunu işle
    _processOverlayQueue: function () {
        if (this._overlayQueue.length === 0) {
            this._isShowingOverlay = false;
            return;
        }

        this._isShowingOverlay = true;
        const item = this._overlayQueue.shift();
        
        console.log('📺 Overlay gösteriliyor: Sıra#' + item.siraNo + ' -> Banko#' + item.bankoNo + ' (Kalan: ' + this._overlayQueue.length + ')');

        // Mevcut overlay varsa kaldır
        const existingOverlay = document.getElementById('siraCalledOverlay');
        if (existingOverlay) {
            existingOverlay.remove();
        }

        // Overlay HTML oluştur
        const overlay = document.createElement('div');
        overlay.id = 'siraCalledOverlay';
        overlay.innerHTML = `
            <div class="sira-called-content">
                <div class="sira-called-icon">
                    <i class="bx bx-bell bx-tada"></i>
                </div>
                <div class="sira-called-title">SIRA ÇAĞRILDI</div>
                <div class="sira-called-number">${item.siraNo}</div>
                <div class="sira-called-banko">
                    <span class="banko-label">${item.bankoTipi}</span>
                    <span class="banko-number">${item.bankoNo}</span>
                </div>
                <div class="sira-called-kat">${item.katTipi || ''}</div>
            </div>
        `;
        
        document.body.appendChild(overlay);

        // Ses çal
        this.playSound();

        // 4 saniye sonra kapat ve sonraki overlay'i göster
        const self = this;
        setTimeout(() => {
            overlay.classList.add('fade-out');
            setTimeout(() => {
                overlay.remove();
                // Sonraki overlay'i işle
                self._processOverlayQueue();
            }, 500);
        }, 4000);
    },

    // ⭐ Listeye yeni sıra ekle (en üste) ve en alttakini kaldır
    addSiraToList: function (siraNo, bankoNo, katTipi) {
        const container = document.querySelector('.sira-cards-container');
        if (!container) {
            console.warn('Sıra kartları container bulunamadı');
            return;
        }

        // Aynı sıra zaten listede varsa, önce onu kaldır (en üste taşımak için)
        const existingCard = container.querySelector(`[data-sira="${siraNo}"]`);
        if (existingCard) {
            existingCard.remove();
            console.log('📺 Mevcut sıra kaldırıldı: Sıra#' + siraNo);
        }

        const cards = container.querySelectorAll('.sira-card');
        const maxRows = parseInt(container.dataset.maxRows) || 6;

        // Yeni kart oluştur (mevcut HTML yapısına uygun)
        const newCard = document.createElement('div');
        newCard.className = 'banko-card sira-card new-sira';
        newCard.dataset.sira = siraNo;
        newCard.innerHTML = `
            <div class="banko-info">
                <div class="banko-label">BANKO</div>
                <div class="banko-no">${bankoNo}</div>
                <div class="banko-kat">${katTipi || ''}</div>
            </div>
            <div class="sira-info">
                <div class="sira-label">SIRA NO</div>
                <div class="sira-no">${siraNo}</div>
            </div>
        `;

        // En üste ekle (animasyonlu)
        const currentCards = container.querySelectorAll('.sira-card');
        if (currentCards.length > 0) {
            container.insertBefore(newCard, currentCards[0]);
        } else {
            container.appendChild(newCard);
        }

        // Animasyon için
        setTimeout(() => {
            newCard.classList.remove('new-sira');
        }, 2000);

        // Satır sayısı aşıldıysa en alttakini kaldır
        const updatedCards = container.querySelectorAll('.sira-card');
        if (updatedCards.length > maxRows) {
            const lastCard = updatedCards[updatedCards.length - 1];
            lastCard.classList.add('removing');
            setTimeout(() => {
                lastCard.remove();
            }, 300);
        }

        console.log('📺 Liste güncellendi: Sıra#' + siraNo + ' eklendi, toplam: ' + Math.min(updatedCards.length, maxRows));
    },

    // ⭐ Tüm listeyi güncelle (sıra çağırma paneli mantığı)
    updateSiraList: function (siralar) {
        const container = document.querySelector('.sira-cards-container');
        if (!container) {
            console.warn('Sıra kartları container bulunamadı');
            return;
        }

        const maxRows = parseInt(container.dataset.maxRows) || 6;
        const displaySiralar = siralar.slice(0, maxRows);

        // Mevcut listeyi temizle ve yeniden oluştur
        container.innerHTML = '';
        
        displaySiralar.forEach((sira, index) => {
            const card = document.createElement('div');
            card.className = 'banko-card sira-card' + (index === 0 ? ' new-sira' : '');
            card.dataset.sira = sira.siraNo;
            card.innerHTML = `
                <div class="banko-info">
                    <div class="banko-label">BANKO</div>
                    <div class="banko-no">${sira.bankoNo}</div>
                    <div class="banko-kat">${sira.katTipi || ''}</div>
                </div>
                <div class="sira-info">
                    <div class="sira-label">SIRA NO</div>
                    <div class="sira-no">${sira.siraNo}</div>
                </div>
            `;
            container.appendChild(card);
        });

        // İlk kartın animasyonunu kaldır
        setTimeout(() => {
            const firstCard = container.querySelector('.new-sira');
            if (firstCard) {
                firstCard.classList.remove('new-sira');
            }
        }, 2000);

        console.log('📺 Liste güncellendi: ' + displaySiralar.length + ' sıra gösteriliyor');
    },

    // ⭐ Listeyi backend'den yenile (senkronizasyon için)
    refreshList: async function () {
        if (!this._tvId) {
            console.warn('TV ID bulunamadı, liste yenilenemedi');
            return;
        }

        try {
            const response = await fetch(`/api/tv/${this._tvId}/siralar`);
            if (!response.ok) {
                console.error('Liste yenileme hatası:', response.status);
                return;
            }

            const result = await response.json();
            if (!result.success || !result.data) {
                console.warn('Liste verisi alınamadı');
                return;
            }

            const container = document.querySelector('.sira-cards-container');
            if (!container) return;

            const maxRows = parseInt(container.dataset.maxRows) || 6;
            const siralar = result.data.slice(0, maxRows);

            // Mevcut listeyi temizle ve yeniden oluştur
            container.innerHTML = '';
            
            siralar.forEach(sira => {
                const card = document.createElement('div');
                card.className = 'banko-card sira-card';
                card.dataset.sira = sira.siraNo;
                card.innerHTML = `
                    <div class="banko-info">
                        <div class="banko-label">BANKO</div>
                        <div class="banko-no">${sira.bankoNo}</div>
                        <div class="banko-kat">${sira.katTipi || ''}</div>
                    </div>
                    <div class="sira-info">
                        <div class="sira-label">SIRA NO</div>
                        <div class="sira-no">${sira.siraNo}</div>
                    </div>
                `;
                container.appendChild(card);
            });

            console.log('📺 Liste senkronize edildi: ' + siralar.length + ' sıra');
        } catch (error) {
            console.error('Liste yenileme hatası:', error);
        }
    },

    // Periyodik senkronizasyon başlat (60 saniyede bir)
    startPeriodicSync: function (intervalSeconds = 60) {
        setInterval(() => {
            this.refreshList();
        }, intervalSeconds * 1000);
        console.log('📺 Periyodik senkronizasyon başlatıldı: ' + intervalSeconds + ' saniye');
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
        // Sıra güncelleme event'i (eski)
        connection.on("receiveSiraUpdate", function (data) {
            console.log("🔔 Yeni sıra çağrıldı (receiveSiraUpdate):", data);

            // Overlay popup göster (3 saniye)
            window.tvDisplay.showSiraCalledOverlay(data.siraNo, data.bankoNo, data.katTipi || '', data.bankoTipi || 'BANKO');

            // Tüm listeyi güncelle (sıra çağırma paneli mantığı)
            if (data.siralar && Array.isArray(data.siralar)) {
                window.tvDisplay.updateSiraList(data.siralar);
            }
        });

        // ⭐ Yeni TV sıra güncelleme event'i
        connection.on("TvSiraGuncellendi", function (data) {
            console.log("📺 TV Sıra Güncellendi:", data);

            // Overlay popup göster (3 saniye)
            window.tvDisplay.showSiraCalledOverlay(data.siraNo, data.bankoNo, data.katTipi || '', data.bankoTipi || 'BANKO');

            // Tüm listeyi güncelle (sıra çağırma paneli mantığı)
            if (data.siralar && Array.isArray(data.siralar)) {
                window.tvDisplay.updateSiraList(data.siralar);
            }
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
