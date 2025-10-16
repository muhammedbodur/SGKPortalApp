// Sıra Çağırma Paneli
var SiraCagirmaPanel = (function () {
    var isPinned = false;
    var panelHeader = null;
    var panel = null;
    var button = null;
    var body = document.body;
    var dotNetHelper = null;

    function init(dotNetReference) {
        dotNetHelper = dotNetReference;
        
        // Elementleri bul
        panelHeader = document.getElementById('panel-header');
        panel = document.getElementById('template-customizer');
        button = document.getElementById('callPanelToggleCustomizerButton');

        if (!panel || !button || !panelHeader) {
            console.warn('SiraCagirmaPanel: Gerekli elementler bulunamadı', {
                panel: !!panel,
                button: !!button,
                panelHeader: !!panelHeader
            });
            return;
        }

        // LocalStorage'dan durumu yükle
        loadState();

        // Click-outside detection
        document.addEventListener('click', handleClickOutside);

        console.log('SiraCagirmaPanel initialized');
    }

    function handleClickOutside(event) {
        if (!panel || !button) return;

        // Eğer tıklanan yer panelin dışı ve toggle butonu değilse ve panel sabitlenmemişse
        if (!panel.contains(event.target) && !button.contains(event.target) && !isPinned) {
            closePanel();
        }
    }

    function openPanel() {
        if (!panel || !button) return;

        panel.classList.add('show');
        button.classList.add('hide');
        body.classList.add('layout-shifted');

        saveState(true);
    }

    function closePanel() {
        if (!panel || !button) return;

        panel.classList.remove('show');
        button.classList.remove('hide');
        body.classList.remove('layout-shifted');

        saveState(false);

        // Blazor'a bildir
        if (dotNetHelper) {
            dotNetHelper.invokeMethodAsync('CloseFromJS');
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
        localStorage.setItem('isPinned', isPinned.toString());
        updatePanelHeaderBackground();
    }

    function loadState() {
        var isPinnedFromStorage = localStorage.getItem('isPinned');
        var isPanelVisibleFromStorage = localStorage.getItem('isPanelVisible');

        if (isPinnedFromStorage === 'true') {
            isPinned = true;
        } else {
            isPinned = false;
        }

        updatePanelHeaderBackground();

        if (isPanelVisibleFromStorage === 'true') {
            panel.classList.add('show');
            button.classList.add('hide');
            body.classList.add('layout-shifted');
        } else {
            panel.classList.remove('show');
            button.classList.remove('hide');
            body.classList.remove('layout-shifted');
        }
    }

    function saveState(isVisible) {
        localStorage.setItem('isPanelVisible', isVisible.toString());
    }

    function updatePanelHeaderBackground() {
        if (panelHeader) {
            if (isPinned) {
                panelHeader.style.backgroundColor = '#696bff';
            } else {
                panelHeader.style.backgroundColor = '#d2e2fc';
            }
        }
    }

    return {
        init: init,
        openPanel: openPanel,
        closePanel: closePanel,
        togglePanel: togglePanel,
        setPin: setPin
    };
})();
