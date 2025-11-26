var SiraCagirmaPanel = (function () {
    var isPinned = false;
    var panelHeader = null;
    var panel = null;
    var button = null;
    var body = document.body;
    var dotNetHelper = null;
    var isInitialized = false;
    var initRetryCount = 0;
    var maxRetries = 10;

    function init(dotNetReference) {
        console.log('🔄 SiraCagirmaPanel.init (Deneme: ' + (initRetryCount + 1) + ')');

        dotNetHelper = dotNetReference;

        panelHeader = document.getElementById('panel-header');
        panel = document.getElementById('template-customizer');
        button = document.getElementById('callPanelToggleCustomizerButton');

        if (!panel || !button || !panelHeader) {
            console.warn('⚠️ Elementler henüz yüklenmedi');

            if (initRetryCount < maxRetries) {
                initRetryCount++;
                setTimeout(function () {
                    init(dotNetReference);
                }, 200);
            }
            return;
        }

        console.log('✅ Elementler bulundu');
        initRetryCount = 0;

        if (!isInitialized) {
            document.addEventListener('click', handleClickOutside);
            isInitialized = true;
        }

        loadState();
        console.log('✅ Panel başlatıldı');
    }

    function handleClickOutside(event) {
        if (!panel || !button || isPinned) return;

        if (!panel.contains(event.target) && !button.contains(event.target)) {
            closePanel();
        }
    }

    function openPanel() {
        if (!panel || !button) return;

        panel.classList.add('show');
        button.classList.add('hide');
        body.classList.add('layout-shifted');

        saveState(true);
        notifyBlazor(true, isPinned);
    }

    function closePanel() {
        if (!panel || !button) return;

        panel.classList.remove('show');
        button.classList.remove('hide');
        body.classList.remove('layout-shifted');

        saveState(false);

        if (dotNetHelper) {
            try {
                dotNetHelper.invokeMethodAsync('CloseFromJS');
            } catch (e) {
                console.error('❌ CloseFromJS error:', e);
            }
        }
    }

    function togglePanel() {
        if (!panel) return;

        if (panel.classList.contains('show')) {
            closePanel();
        } else {
            openPanel();
        }
    }

    function setPin(pinned) {
        isPinned = pinned;
        localStorage.setItem('callPanelIsPinned', isPinned.toString());
        saveState(panel.classList.contains('show'));
        console.log('📌 Pin:', isPinned);
    }

    function loadState() {
        try {
            var storedPinned = localStorage.getItem('callPanelIsPinned');
            var storedVisible = localStorage.getItem('callPanelIsVisible');

            isPinned = storedPinned === 'true';
            var shouldBeVisible = storedVisible === 'true';

            if (panel && button && body) {
                if (shouldBeVisible) {
                    panel.classList.add('show');
                    button.classList.add('hide');
                    body.classList.add('layout-shifted');
                } else {
                    panel.classList.remove('show');
                    button.classList.remove('hide');
                    body.classList.remove('layout-shifted');
                }

                console.log('✅ Durum yüklendi:', { isPinned: isPinned, isVisible: shouldBeVisible });
            }
        } catch (e) {
            console.error('❌ loadState error:', e);
        }
    }

    function saveState(isVisible) {
        try {
            localStorage.setItem('callPanelIsVisible', isVisible.toString());
            localStorage.setItem('callPanelIsPinned', isPinned.toString());
            notifyBlazor(isVisible, isPinned);
            console.log('💾 Kaydedildi:', { isVisible: isVisible, isPinned: isPinned });
        } catch (e) {
            console.error('❌ saveState error:', e);
        }
    }

    function notifyBlazor(isVisible, isPinned) {
        if (dotNetHelper) {
            try {
                dotNetHelper.invokeMethodAsync('UpdateStateFromJS', isVisible, isPinned);
            } catch (e) {
                // Silent fail - Blazor hazır olmayabilir
            }
        }
    }

    function destroy() {
        if (isInitialized) {
            document.removeEventListener('click', handleClickOutside);
            isInitialized = false;
        }
    }

    // Test fonksiyonları
    window.testCallPanel = {
        getState: function () {
            return {
                isPinned: isPinned,
                isVisible: panel ? panel.classList.contains('show') : false,
                localStorage: {
                    pinned: localStorage.getItem('callPanelIsPinned'),
                    visible: localStorage.getItem('callPanelIsVisible')
                }
            };
        },
        reset: function () {
            localStorage.removeItem('callPanelIsPinned');
            localStorage.removeItem('callPanelIsVisible');
            console.log('🗑️ Temizlendi');
        }
    };

    return {
        init: init,
        openPanel: openPanel,
        closePanel: closePanel,
        togglePanel: togglePanel,
        setPin: setPin,
        destroy: destroy
    };
})();

console.log('✅ SiraCagirmaPanel yüklendi');