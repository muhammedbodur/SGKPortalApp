// Select2 Blazor Integration
window.Select2Blazor = {
    // Select2'yi initialize et ve değeri set et
    initialize: function (elementId, value) {
        const element = document.getElementById(elementId);
        if (!element) return;

        // Zaten initialize edilmişse destroy et
        if ($(element).hasClass('select2-hidden-accessible')) {
            $(element).select2('destroy');
        }

        // Select2'yi initialize et
        $(element).select2({
            placeholder: element.getAttribute('data-placeholder') || 'Seçiniz',
            allowClear: true,
            width: '100%',
            language: {
                noResults: function () { return "Sonuç bulunamadı"; },
                searching: function () { return "Aranıyor..."; }
            }
        });

        // Değeri set et
        if (value && value !== '0' && value !== 0) {
            $(element).val(value).trigger('change.select2');
        }

        // Blazor'a değişikliği bildir
        $(element).off('select2:select select2:clear');
        $(element).on('select2:select', function (e) {
            element.value = e.params.data.id;
            element.dispatchEvent(new Event('change', { bubbles: true }));
        });

        $(element).on('select2:clear', function () {
            element.value = '0';
            element.dispatchEvent(new Event('change', { bubbles: true }));
        });
    },

    // Tüm select2'leri initialize et
    initializeAll: function () {
        const selects = document.querySelectorAll('.select2:not(.select2-hidden-accessible)');
        selects.forEach(element => {
            const currentValue = element.value;

            $(element).select2({
                placeholder: element.getAttribute('data-placeholder') || 'Seçiniz',
                allowClear: true,
                width: '100%',
                language: {
                    noResults: function () { return "Sonuç bulunamadı"; },
                    searching: function () { return "Aranıyor..."; }
                }
            });

            if (currentValue && currentValue !== '0') {
                $(element).val(currentValue).trigger('change.select2');
            }

            $(element).off('select2:select select2:clear');
            $(element).on('select2:select', function (e) {
                element.value = e.params.data.id;
                element.dispatchEvent(new Event('change', { bubbles: true }));
            });

            $(element).on('select2:clear', function () {
                element.value = '0';
                element.dispatchEvent(new Event('change', { bubbles: true }));
            });
        });
    },

    // Select2'yi destroy et
    destroy: function (elementId) {
        const element = document.getElementById(elementId);
        if (element && $(element).hasClass('select2-hidden-accessible')) {
            $(element).select2('destroy');
        }
    },

    // Değeri güncelle
    updateValue: function (elementId, value) {
        const element = document.getElementById(elementId);
        if (element && $(element).hasClass('select2-hidden-accessible')) {
            $(element).val(value).trigger('change.select2');
        }
    }
};
