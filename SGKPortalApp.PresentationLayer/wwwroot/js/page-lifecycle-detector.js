/**
 * Sayfa Yaşam Döngüsü Algılayıcı
 * Refresh, yeni tab, browser kapatma gibi durumları algılar
 */

(function() {
    'use strict';

    // ============================================
    // PAGE LIFECYCLE DETECTOR
    // ============================================
    class PageLifecycleDetector {
        constructor() {
            this.isRefresh = false;
            this.isNewTab = false;
            this.isClosing = false;
            this.detectNavigationType();
            this.setupEventListeners();
        }

        /**
         * Sayfa yüklenme türünü algıla
         */
        detectNavigationType() {
            // Performance Navigation API v2 (Modern browsers)
            if (performance.navigation) {
                const navType = performance.navigation.type;
                
                switch (navType) {
                    case 0: // TYPE_NAVIGATE - Normal navigation
                        this.isNewTab = !sessionStorage.getItem('portal.hasVisited');
                        console.log('📍 Navigation Type: Normal (Yeni tab veya link)');
                        break;
                    case 1: // TYPE_RELOAD - Refresh
                        this.isRefresh = true;
                        console.log('🔄 Navigation Type: Refresh (F5 veya Ctrl+R)');
                        break;
                    case 2: // TYPE_BACK_FORWARD - Back/Forward button
                        console.log('⬅️ Navigation Type: Back/Forward');
                        break;
                    default:
                        console.log('❓ Navigation Type: Unknown');
                }
            }
            
            // Performance API v2 (Daha modern)
            if (performance.getEntriesByType) {
                const navEntries = performance.getEntriesByType('navigation');
                if (navEntries.length > 0) {
                    const navEntry = navEntries[0];
                    if (navEntry.type === 'reload') {
                        this.isRefresh = true;
                        console.log('🔄 Navigation Entry Type: Reload');
                    } else if (navEntry.type === 'navigate') {
                        console.log('📍 Navigation Entry Type: Navigate');
                    } else if (navEntry.type === 'back_forward') {
                        console.log('⬅️ Navigation Entry Type: Back/Forward');
                    }
                }
            }

            // SessionStorage ile yeni tab kontrolü
            if (!sessionStorage.getItem('portal.hasVisited')) {
                sessionStorage.setItem('portal.hasVisited', 'true');
                if (!this.isRefresh) {
                    this.isNewTab = true;
                    console.log('🆕 Yeni tab algılandı');
                }
            }
        }

        /**
         * Event listener'ları kur
         */
        setupEventListeners() {
            // beforeunload - Sayfa kapatılmadan önce
            window.addEventListener('beforeunload', (event) => {
                this.isClosing = true;
                console.log('⚠️ beforeunload: Sayfa kapatılıyor veya refresh ediliyor');
                
                // Banko modunda uyarı göster (opsiyonel)
                if (window.signalRApp?.isInBankoMode?.()) {
                    // Modern browsers için return value önemsiz, sadece event.returnValue set etmek yeterli
                    event.preventDefault();
                    event.returnValue = '';
                    return '';
                }
            });

            // unload - Sayfa tamamen kapandığında
            window.addEventListener('unload', () => {
                console.log('🚪 unload: Sayfa kapatıldı');
                
                // Son işlemler (örn: localStorage'a durum kaydet)
                if (this.isClosing && !this.isRefresh) {
                    localStorage.setItem('portal.lastExit', new Date().toISOString());
                }
            });

            // pagehide - Sayfa gizlendiğinde (mobil için önemli)
            window.addEventListener('pagehide', (event) => {
                console.log('👋 pagehide: Sayfa gizlendi, persisted:', event.persisted);
            });

            // visibilitychange - Sayfa görünürlüğü değiştiğinde
            document.addEventListener('visibilitychange', () => {
                if (document.hidden) {
                    console.log('🙈 Sayfa gizlendi (tab değişti veya minimize edildi)');
                } else {
                    console.log('👁️ Sayfa görünür hale geldi');
                }
            });
        }

        /**
         * Refresh mi kontrol et
         */
        isPageRefresh() {
            return this.isRefresh;
        }

        /**
         * Yeni tab mı kontrol et
         */
        isNewTabOpen() {
            return this.isNewTab;
        }

        /**
         * Sayfa kapatılıyor mu kontrol et
         */
        isPageClosing() {
            return this.isClosing;
        }

        /**
         * Bilgileri al
         */
        getInfo() {
            return {
                isRefresh: this.isRefresh,
                isNewTab: this.isNewTab,
                isClosing: this.isClosing,
                navigationType: performance.navigation?.type,
                navigationEntryType: performance.getEntriesByType?.('navigation')?.[0]?.type
            };
        }
    }

    // ============================================
    // GLOBAL INSTANCE
    // ============================================
    window.pageLifecycle = new PageLifecycleDetector();
    
    // Debug için bilgileri göster
    console.log('📊 Page Lifecycle Info:', window.pageLifecycle.getInfo());

})();
