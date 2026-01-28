// ======================================================
// Select2 + Blazor Server (Sneat Compatible)
// Duplicate-proof, Production Ready
// ======================================================

window.Select2Blazor = {
    // 🔐 Global tracking Map: identifier -> element reference
    _initializedSelects: new Map(),

    // 🔄 Tek bir select'i yeniden initialize et (clear sonrası duplicate fix için)
    _reinitialize: function (element) {
        const identifier = element.name || element.id || element.getAttribute('data-val-required');
        if (!identifier) return;

        // Map'ten kaldır
        this._initializedSelects.delete(identifier);

        // Select2'yi destroy et
        try {
            $(element).select2('destroy');
        } catch (e) { /* ignore */ }

        // Duplicate container'ları temizle
        const nextSibling = element.nextElementSibling;
        if (nextSibling && nextSibling.classList.contains('select2-container')) {
            nextSibling.remove();
        }

        // Yeniden initialize et
        const currentValue = element.value;

        $(element).select2({
            placeholder: element.getAttribute('data-placeholder') || 'Seçiniz',
            allowClear: true,
            width: '100%',
            minimumResultsForSearch: 0,
            language: {
                noResults: () => "Sonuç bulunamadı",
                searching: () => "Aranıyor..."
            }
        });

        // Map'e geri ekle
        this._initializedSelects.set(identifier, element);

        // Event handler'ları yeniden bağla
        this._bindEvents(element);

        // Değeri koru
        if (currentValue && currentValue !== '0' && currentValue !== '') {
            $(element).val(currentValue).trigger('change.select2');
        }
    },

    // 🔗 Event handler'ları bağla (tekrar kullanılabilir)
    _bindEvents: function (element) {
        const self = this;

        $(element).off('select2:select select2:clear');

        $(element).on('select2:select', function (e) {
            element.value = e.params.data.id;
            element.dispatchEvent(new Event('change', { bubbles: true }));
        });

        $(element).on('select2:clear', function () {
            element.value = '0';
            element.dispatchEvent(new Event('change', { bubbles: true }));

            // 🔧 50ms sonra reinitialize et (duplicate fix)
            setTimeout(function () {
                self._reinitialize(element);
            }, 50);
        });
    },

    // 🧹 Orphan Select2 container'ları temizle
    cleanupOrphanedSelect2: function () {
        // Artık DOM'da olmayan element'leri Map'ten kaldır
        for (const [identifier, elementRef] of this._initializedSelects.entries()) {
            if (!document.contains(elementRef)) {
                this._initializedSelects.delete(identifier);
            }
        }

        // Orphan select2-container'ları temizle (select element'i olmayan)
        document.querySelectorAll('.select2-container').forEach(container => {
            const prevSibling = container.previousElementSibling;
            if (!prevSibling || prevSibling.tagName !== 'SELECT') {
                container.remove();
            }
        });
    },

    initializeAllSelects: function () {
        // Önce orphan'ları temizle
        this.cleanupOrphanedSelect2();

        const selects = document.querySelectorAll(
            'select:not([data-no-select2])'
        );

        let initCount = 0;
        let skipCount = 0;

        selects.forEach(element => {
            // Unique identifier oluştur (name veya id)
            const identifier = element.name || element.id || element.getAttribute('data-val-required');

            if (!identifier) {
                return; // Sessizce skip et
            }

            // Eğer bu element zaten initialize edilmişse skip et
            // Ama aynı identifier'a sahip FARKLI bir element varsa (Blazor re-render), yeniden init et
            const existingElement = this._initializedSelects.get(identifier);
            if (existingElement === element) {
                skipCount++;
                return;
            }

            // Eğer eski element varsa ve DOM'da hala varsa destroy et
            if (existingElement && document.contains(existingElement)) {
                try {
                    $(existingElement).select2('destroy');
                } catch (e) { /* ignore */ }
            }

            initCount++;

            const currentValue = element.value;

            $(element).select2({
                placeholder: element.getAttribute('data-placeholder') || 'Seçiniz',
                allowClear: true,
                width: '100%',
                minimumResultsForSearch: 0,
                language: {
                    noResults: () => "Sonuç bulunamadı",
                    searching: () => "Aranıyor..."
                }
            });

            // 🔐 Map'e ekle (element referansı ile)
            this._initializedSelects.set(identifier, element);

            // 🔄 Mevcut değeri koru
            if (currentValue && currentValue !== '0' && currentValue !== '') {
                $(element).val(currentValue).trigger('change.select2');
            }

            // 🔗 Event handler'ları bağla (reinitialize ile duplicate fix dahil)
            this._bindEvents(element);
        });

        if (initCount > 0) {
            console.log(`📊 Select2: ${initCount} init, ${skipCount} skip`);
        }
    }
};

// ======================================================
// GLOBAL AUTO INIT (Blazor-safe)
// ======================================================
(function () {

    function safeInit() {
        clearTimeout(window.__select2InitTimer);
        window.__select2InitTimer = setTimeout(() => {
            window.Select2Blazor.initializeAllSelects();
        }, 150);
    }

    // İlk yükleme
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', safeInit);
    } else {
        safeInit();
    }

    // Blazor navigation (enhanced routing)
    document.addEventListener('enhancedload', safeInit);

    // 🔍 Yeni select'leri yakala (AGRESİF OLMAYAN)
    const observer = new MutationObserver(mutations => {

        let foundNewSelect = false;

        for (const m of mutations) {
            for (const node of m.addedNodes) {

                if (node.nodeType !== 1) continue;

                // Select2'nin kendi DOM'u → ignore
                if (node.classList?.contains('select2-container')) continue;

                if (
                    node.tagName === 'SELECT' ||
                    node.querySelector?.('select')
                ) {
                    foundNewSelect = true;
                    break;
                }
            }
            if (foundNewSelect) break;
        }

        if (foundNewSelect) safeInit();
    });

    observer.observe(document.body, {
        childList: true,
        subtree: true
    });

})();
