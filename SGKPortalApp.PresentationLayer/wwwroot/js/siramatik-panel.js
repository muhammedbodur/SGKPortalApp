/**
 * ============================================================
 * SIRA ÇAĞIRMA PANELİ - Sıramatik Modülü
 * Profesyonel JavaScript Implementation
 * ============================================================
 */

(function () {
    'use strict';

    // State Management
    let isPinned = false;
    let isVisible = false;

    // DOM Elements
    let panel = null;
    let toggleBtn = null;
    let pinIcon = null;
    let header = null;

    /**
     * Initialize Sıra Çağırma Panel
     */
    function initSiraCagirmaPanel() {
        // Get DOM elements
        panel = document.getElementById('siraCagirmaPanel');
        toggleBtn = document.getElementById('siraPanelToggle');
        pinIcon = document.getElementById('siraPanelPin');
        header = document.getElementById('siraPanelHeader');

        if (!panel || !toggleBtn) {
            console.warn('Sıra Çağırma Panel elements not found');
            return;
        }

        // Load saved state
        loadPanelState();

        // Attach event listeners
        attachEventListeners();

        // Apply initial state
        applyPanelState();

        console.log('✅ Sıra Çağırma Panel initialized');
    }

    /**
     * Attach Event Listeners
     */
    function attachEventListeners() {
        // Toggle button click
        toggleBtn.addEventListener('click', togglePanel);

        // Pin icon click
        if (pinIcon) {
            pinIcon.addEventListener('click', togglePin);
        }

        // Click outside to close (if not pinned)
        document.addEventListener('click', handleClickOutside);
    }

    /**
     * Toggle Panel Visibility
     */
    function togglePanel() {
        isVisible = !isVisible;
        applyPanelState();
        savePanelState();
    }

    /**
     * Toggle Pin State
     */
    function togglePin(e) {
        e.stopPropagation();
        isPinned = !isPinned;
        updatePinIcon();
        updateHeaderBackground();
        savePanelState();
    }

    /**
     * Handle Click Outside
     */
    function handleClickOutside(event) {
        if (!isVisible || isPinned) return;

        const isClickInsidePanel = panel.contains(event.target);
        const isClickOnToggle = toggleBtn.contains(event.target);

        if (!isClickInsidePanel && !isClickOnToggle) {
            isVisible = false;
            applyPanelState();
            savePanelState();
        }
    }

    /**
     * Apply Panel State (Show/Hide)
     */
    function applyPanelState() {
        if (isVisible) {
            panel.classList.add('show');
            toggleBtn.classList.add('hide');
        } else {
            panel.classList.remove('show');
            toggleBtn.classList.remove('hide');
        }
    }

    /**
     * Update Pin Icon
     */
    function updatePinIcon() {
        if (!pinIcon) return;

        const icon = pinIcon.querySelector('i');
        if (icon) {
            if (isPinned) {
                icon.classList.remove('bx-pin');
                icon.classList.add('bx-pin-off');
            } else {
                icon.classList.remove('bx-pin-off');
                icon.classList.add('bx-pin');
            }
        }
    }

    /**
     * Update Header Background
     */
    function updateHeaderBackground() {
        if (!header) return;

        if (isPinned) {
            header.style.background = 'linear-gradient(135deg, #696cff 0%, #5f61e6 100%)';
        } else {
            header.style.background = 'linear-gradient(135deg, #8b8dff 0%, #7f81f6 100%)';
        }
    }

    /**
     * Save Panel State to LocalStorage
     */
    function savePanelState() {
        try {
            localStorage.setItem('siraCagirmaPanel_isVisible', isVisible.toString());
            localStorage.setItem('siraCagirmaPanel_isPinned', isPinned.toString());
        } catch (e) {
            console.warn('LocalStorage save error:', e);
        }
    }

    /**
     * Load Panel State from LocalStorage
     */
    function loadPanelState() {
        try {
            const savedVisible = localStorage.getItem('siraCagirmaPanel_isVisible');
            const savedPinned = localStorage.getItem('siraCagirmaPanel_isPinned');

            if (savedVisible !== null) {
                isVisible = savedVisible === 'true';
            }

            if (savedPinned !== null) {
                isPinned = savedPinned === 'true';
            }

            updatePinIcon();
            updateHeaderBackground();
        } catch (e) {
            console.warn('LocalStorage load error:', e);
        }
    }

    /**
     * Public API
     */
    window.SiraCagirmaPanel = {
        show: function () {
            isVisible = true;
            applyPanelState();
            savePanelState();
        },
        hide: function () {
            isVisible = false;
            applyPanelState();
            savePanelState();
        },
        toggle: togglePanel,
        pin: function () {
            isPinned = true;
            updatePinIcon();
            updateHeaderBackground();
            savePanelState();
        },
        unpin: function () {
            isPinned = false;
            updatePinIcon();
            updateHeaderBackground();
            savePanelState();
        }
    };

    // Initialize when DOM is ready
    // Blazor Server için biraz bekle
    function tryInit() {
        const panel = document.getElementById('siraCagirmaPanel');
        if (panel) {
            initSiraCagirmaPanel();
        } else {
            // Element henüz render olmamış, tekrar dene
            setTimeout(tryInit, 100);
        }
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', tryInit);
    } else {
        tryInit();
    }

})();
