(function () {
    'use strict';

    var initialized = false;
    var isTemporarilyExpanded = false;

    var blazorLoadCheckInterval = setInterval(function () {
        if (typeof DotNet !== 'undefined') {
            clearInterval(blazorLoadCheckInterval);
            setTimeout(function () {
                if (!initialized) {
                    initializeSneat();
                }
            }, 300);
        }
    }, 100);

    setTimeout(function () {
        clearInterval(blazorLoadCheckInterval);
        if (typeof DotNet === 'undefined' && !initialized) {
            console.warn('Blazor yüklenemedi, yine de Sneat başlatılıyor...');
            initializeSneat();
        }
    }, 10000);

    function initializeSneat() {
        if (initialized) {
            return;
        }

        var menuElement = document.getElementById('layout-menu');

        if (!menuElement) {
            console.error('#layout-menu bulunamadı!');
            return;
        }

        try {
            if (typeof Menu !== 'undefined' && !window.sneatMenuInstance) {
                window.sneatMenuInstance = new Menu(menuElement, {
                    orientation: 'vertical',
                    closeChildren: false,
                    showDropdownOnHover: true
                });
            }

            if (typeof PerfectScrollbar !== 'undefined') {
                var menuInner = document.querySelector('.menu-inner');
                if (menuInner && !menuInner.classList.contains('ps')) {
                    new PerfectScrollbar(menuInner, {
                        wheelPropagation: false,
                        suppressScrollX: true
                    });
                }
            }

            if (window.Helpers) {
                window.Helpers.initSidebarToggle();
            }

            removeMainJsToggleEvents();
            bindMenuToggleEvents();
            setupMenuHoverWithHelpers();

            initialized = true;

        } catch (error) {
            console.error('Sneat başlatma hatası:', error);
        }
    }

    function removeMainJsToggleEvents() {
        var toggleButtons = document.querySelectorAll('.layout-menu-toggle');
        toggleButtons.forEach(function (button) {
            var newButton = button.cloneNode(true);
            button.parentNode.replaceChild(newButton, button);
        });
    }

    function bindMenuToggleEvents() {
        var toggleButtons = document.querySelectorAll('.layout-menu-toggle');
        toggleButtons.forEach(function (button) {
            button.addEventListener('click', handleToggleClick);
        });
    }

    function handleToggleClick(e) {
        e.preventDefault();
        e.stopPropagation();

        if (!window.Helpers) return;

        var chevron = document.querySelector('.bx-chevron-left');
        var layoutWrapper = document.querySelector('.layout-wrapper');

        if (isTemporarilyExpanded) {
            // Geçici açık → Kalıcı açık: SOL'a bak
            isTemporarilyExpanded = false;
            chevron.style.transform = 'rotate(0deg)';

            if (layoutWrapper) {
                layoutWrapper.classList.remove('layout-menu-hover');
            }

            if (typeof templateName !== 'undefined' && window.config && window.config.enableMenuLocalStorage) {
                try {
                    localStorage.setItem(
                        'templateCustomizer-' + templateName + '--LayoutCollapsed',
                        'false'
                    );
                } catch (err) {
                    console.warn('LocalStorage hatası:', err);
                }
            }

            console.log('Menu toggle: Geçici → Kalıcı AÇIK, chevron sola');
            return;
        }

        // Normal toggle
        window.Helpers.toggleCollapsed();

        var isCollapsed = window.Helpers.isCollapsed();

        // Chevron'u güncelle
        if (isCollapsed) {
            chevron.style.transform = 'rotate(180deg)'; // Kapalı → Sağa
        } else {
            chevron.style.transform = 'rotate(0deg)'; // Açık → Sola
        }

        if (typeof templateName !== 'undefined' && window.config && window.config.enableMenuLocalStorage) {
            try {
                localStorage.setItem(
                    'templateCustomizer-' + templateName + '--LayoutCollapsed',
                    String(isCollapsed)
                );
            } catch (err) {
                console.warn('LocalStorage hatası:', err);
            }
        }

        console.log('Menu toggle: Collapsed =', isCollapsed, ', Chevron:', isCollapsed ? 'sağ' : 'sol');
    }

    function setupMenuHoverWithHelpers() {
        var menuElement = document.getElementById('layout-menu');
        var toggleBtn = document.querySelector('.layout-menu-toggle');
        var chevron = document.querySelector('.bx-chevron-left');
        var layoutWrapper = document.querySelector('.layout-wrapper');
        var hoverTimeout = null;

        if (!menuElement || !window.Helpers || !layoutWrapper || !chevron) return;

        // Chevron'u başlangıçta doğru yöne çevir
        function updateChevron() {
            var isCollapsed = window.Helpers.isCollapsed();
            if (isCollapsed) {
                chevron.style.transform = 'rotate(180deg)'; // Kapalı → Sağa
            } else {
                chevron.style.transform = 'rotate(0deg)'; // Açık → Sola
            }
            console.log('Chevron güncellendi. Collapsed:', isCollapsed);
        }

        // Mouse menüye girdiğinde
        menuElement.addEventListener('mouseenter', function () {
            if (!window.Helpers.isSmallScreen() && window.Helpers.isCollapsed()) {
                hoverTimeout = setTimeout(function () {
                    window.Helpers.setCollapsed(false, false);
                    isTemporarilyExpanded = true;

                    // Hover ile açıldığında SAĞ'a bak (geçici)
                    chevron.style.transform = 'rotate(180deg)';

                    layoutWrapper.classList.add('layout-menu-hover');
                    console.log('Menu hover: Geçici açıldı, chevron sağda');
                }, 300);
            }

            if (toggleBtn && !window.Helpers.isSmallScreen()) {
                toggleBtn.classList.add('d-block');
            }
        });

        // Mouse menüden çıktığında
        menuElement.addEventListener('mouseleave', function () {
            if (hoverTimeout) {
                clearTimeout(hoverTimeout);
                hoverTimeout = null;
            }

            if (isTemporarilyExpanded) {
                window.Helpers.setCollapsed(true, false);
                isTemporarilyExpanded = false;

                // Kapandığında yine SAĞ'a bak
                chevron.style.transform = 'rotate(180deg)';

                layoutWrapper.classList.remove('layout-menu-hover');
                console.log('Menu hover: Geçici kapandı');
            }

            if (toggleBtn) {
                toggleBtn.classList.remove('d-block');
            }
        });

        // İlk yükleme
        updateChevron();

        console.log('✓ Menu hover davranışı bağlandı');
    }

    window.initSneatMenu = function () {
        if (!initialized) {
            initializeSneat();
        }
    };

    window.initPerfectScrollbar = function () {
        if (typeof PerfectScrollbar === 'undefined') return;

        var menuInner = document.querySelector('.menu-inner');
        if (menuInner && !menuInner.classList.contains('ps')) {
            new PerfectScrollbar(menuInner, {
                wheelPropagation: false,
                suppressScrollX: true
            });
        }
    };

})();