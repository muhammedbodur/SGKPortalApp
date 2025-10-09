// Blazor Reconnection Management
(function () {
    // Reconnection durumunu takip eden değişkenler
    let reconnectState = {
        currentRetry: 0,
        maxRetries: 8,
        isReconnecting: false,
        countdownTimer: null
    };

    // Blazor'ın varsayılan reconnection UI'sını devre dışı bırak
    window.addEventListener('DOMContentLoaded', function () {
        const originalModal = document.getElementById('components-reconnect-modal');
        if (originalModal) {
            originalModal.style.display = 'none !important';
            originalModal.remove();
        }
    });

    // Kendi reconnection yönetimimizi oluştur
    function showCustomReconnectModal(state = 'connecting') {
        const modal = document.getElementById('custom-blazor-reconnect-modal');
        if (!modal) return;

        modal.querySelectorAll('[class^="reconnect-state-"]').forEach(el => {
            el.style.display = 'none';
        });

        const stateElement = modal.querySelector(`.reconnect-state-${state}`);
        if (stateElement) {
            stateElement.style.display = 'block';
        }

        modal.style.display = 'block';
        console.log(`🔄 Reconnection modal gösteriliyor: ${state}`);
    }

    function hideCustomReconnectModal() {
        const modal = document.getElementById('custom-blazor-reconnect-modal');
        if (modal) {
            modal.style.display = 'none';
            if (reconnectState.countdownTimer) {
                clearInterval(reconnectState.countdownTimer);
                reconnectState.countdownTimer = null;
            }
            console.log('✅ Reconnection modal gizlendi');
        }
    }

    // Sadece geri sayım timer'ı
    function startRetryCountdown() {
        const timerContainer = document.getElementById('reconnect-timer-container');
        const nextRetryElement = document.getElementById('next-retry-time');

        if (!nextRetryElement || !timerContainer) return;

        if (reconnectState.currentRetry <= 1) {
            timerContainer.style.display = 'none';
            return;
        }

        timerContainer.style.display = 'block';

        if (reconnectState.countdownTimer) {
            clearInterval(reconnectState.countdownTimer);
        }

        const delay = Math.min(1000 * Math.pow(2, reconnectState.currentRetry - 1), 30000);
        let remainingTime = Math.floor(delay / 1000);

        nextRetryElement.textContent = `Sonraki deneme: ${remainingTime} saniye`;

        reconnectState.countdownTimer = setInterval(() => {
            remainingTime--;
            if (remainingTime > 0) {
                nextRetryElement.textContent = `Sonraki deneme: ${remainingTime} saniye`;
            } else {
                nextRetryElement.textContent = 'Şimdi deneniyor...';
                clearInterval(reconnectState.countdownTimer);
                reconnectState.countdownTimer = null;
            }
        }, 1000);
    }

    // Blazor bağlantı olaylarını dinle
    window.addEventListener('DOMContentLoaded', function () {
        let blazorLoadCheck = setInterval(() => {
            if (window.Blazor) {
                clearInterval(blazorLoadCheck);

                const originalStart = window.Blazor.start;
                window.Blazor.start = function (options) {
                    const customOptions = {
                        ...options,
                        reconnectionOptions: {
                            maxRetries: reconnectState.maxRetries,
                            retryIntervalMilliseconds: retryCount => {
                                reconnectState.currentRetry = retryCount + 1;
                                reconnectState.nextRetryDelay = Math.min(1000 * Math.pow(2, retryCount), 30000);
                                return reconnectState.nextRetryDelay;
                            }
                        },
                        reconnectionHandler: {
                            onConnectionDown: () => {
                                console.log('🔴 Blazor bağlantısı koptu - yeniden bağlanılıyor');
                                reconnectState.isReconnecting = true;
                                reconnectState.currentRetry = 0;
                                showCustomReconnectModal('connecting');
                            },
                            onConnectionUp: () => {
                                console.log('🟢 Blazor bağlantısı yeniden kuruldu');
                                reconnectState.isReconnecting = false;
                                reconnectState.currentRetry = 0;
                                hideCustomReconnectModal();
                            },
                            onReconnectionFailed: () => {
                                console.log('🔴 Blazor reconnection başarısız - maksimum deneme sayısına ulaşıldı');
                                reconnectState.isReconnecting = false;

                                const maxRetriesElement = document.getElementById('max-retries-reached');
                                if (maxRetriesElement) {
                                    maxRetriesElement.textContent = `${reconnectState.maxRetries} deneme`;
                                }

                                showCustomReconnectModal('failed');
                            }
                        }
                    };

                    return originalStart ? originalStart.call(this, customOptions) : Promise.resolve();
                };
            }
        }, 100);
    });

    // MutationObserver ile Blazor'ın yarattığı modal'ları yakalayalım
    document.addEventListener('DOMContentLoaded', function () {
        const observer = new MutationObserver(function (mutations) {
            mutations.forEach(function (mutation) {
                mutation.addedNodes.forEach(function (node) {
                    if (node.nodeType === 1) {
                        if (node.id === 'components-reconnect-modal' ||
                            node.classList?.contains('components-reconnect-modal')) {
                            console.log('🚫 Blazor varsayılan modal yakalandı ve gizlendi');
                            node.style.display = 'none';

                            if (!reconnectState.isReconnecting) {
                                reconnectState.isReconnecting = true;
                                reconnectState.currentRetry = 0;
                                showCustomReconnectModal('connecting');
                            }
                        }
                    }
                });
            });
        });

        observer.observe(document.body, {
            childList: true,
            subtree: true
        });
    });
})();
