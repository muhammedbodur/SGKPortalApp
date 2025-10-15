/**
 * ============================================================
 * SIRA ÇAĞIRMA PANELİ - Sıramatik Modülü
 * Click Outside to Close - Blazor Interop
 * ============================================================
 */

(function () {
    'use strict';

    let dotNetHelper = null;
    let panel = null;

    /**
     * Initialize - Called from Blazor
     */
    function init(dotNetRef) {
        dotNetHelper = dotNetRef;
        panel = document.getElementById('siraCagirmaPanel');

        if (!panel) {
            console.warn('Sıra Çağırma Panel not found');
            return;
        }

        // Click outside to close
        document.addEventListener('click', handleClickOutside);
        
        console.log('✅ Sıra Çağırma Panel - Click Outside initialized');
    }

    /**
     * Handle Click Outside
     */
    function handleClickOutside(event) {
        if (!panel || !dotNetHelper) return;

        const isClickInsidePanel = panel.contains(event.target);
        const toggleBtn = document.querySelector('.sira-panel-toggle');
        const isClickOnToggle = toggleBtn && toggleBtn.contains(event.target);

        if (!isClickInsidePanel && !isClickOnToggle) {
            // Blazor'a kapat komutu gönder
            dotNetHelper.invokeMethodAsync('CloseFromJS');
        }
    }

    /**
     * Public API
     */
    window.SiraCagirmaPanel = {
        init: init
    };

})();
