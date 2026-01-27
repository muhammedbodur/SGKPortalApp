// ======================================================
// Select2 + Blazor Server (Sneat Compatible)
// Duplicate-proof, Production Ready
// ======================================================

window.Select2Blazor = {
    // 🔐 Global tracking Set (Blazor re-render'dan etkilenmez)
    _initializedSelects: new Set(),

    initializeAllSelects: function () {

        const selects = document.querySelectorAll(
            'select:not([data-no-select2])'
        );

        let initCount = 0;
        let skipCount = 0;

        selects.forEach(element => {
            // Unique identifier oluştur (name veya id)
            const identifier = element.name || element.id || element.getAttribute('data-val-required');
            
            if (!identifier) {
                console.warn('⚠️ Select without name/id, skipping:', element);
                return;
            }

            // 🔒 GLOBAL SET İLE KONTROL (Blazor re-render safe)
            if (this._initializedSelects.has(identifier)) {
                skipCount++;
                console.log(`⏭️ Skip (already init): ${identifier}`);
                return;
            }

            initCount++;
            console.log(`✅ Init: ${identifier}`);

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

            // 🔐 GLOBAL SET'E EKLE (Blazor re-render safe)
            this._initializedSelects.add(identifier);

            // 🔄 Mevcut değeri koru
            if (currentValue && currentValue !== '0' && currentValue !== '') {
                $(element).val(currentValue).trigger('change.select2');
            }

            // 🔗 Blazor ile senkronizasyon
            $(element).on('select2:select', function (e) {
                element.value = e.params.data.id;
                element.dispatchEvent(
                    new Event('change', { bubbles: true })
                );
            });

            $(element).on('select2:clear', function () {
                element.value = '0';
                element.dispatchEvent(
                    new Event('change', { bubbles: true })
                );
            });
        });

        console.log(`📊 Select2 Init Summary: ${initCount} initialized, ${skipCount} skipped, ${this._initializedSelects.size} total tracked`);
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
