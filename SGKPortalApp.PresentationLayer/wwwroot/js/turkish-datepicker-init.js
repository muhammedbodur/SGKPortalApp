/**
 * Turkish DatePicker Auto-Initializer
 * Tüm type="date" ve type="datetime-local" input'ları otomatik olarak
 * Türkiye formatında (dd.MM.yyyy) çalışacak şekilde Flatpickr ile değiştirir.
 * 
 * Özellikler:
 * - Türkçe dil desteği
 * - dd.MM.yyyy formatı (görüntüleme)
 * - yyyy-MM-dd formatı (DB'ye kayıt için)
 * - Sneat template uyumlu görünüm
 * - Otomatik initialization (hiçbir sayfada kod değişikliği gerekmez)
 */

(function () {
    'use strict';

    // Türkçe dil ayarları
    const Turkish = {
        weekdays: {
            shorthand: ['Paz', 'Pzt', 'Sal', 'Çar', 'Per', 'Cum', 'Cmt'],
            longhand: ['Pazar', 'Pazartesi', 'Salı', 'Çarşamba', 'Perşembe', 'Cuma', 'Cumartesi']
        },
        months: {
            shorthand: ['Oca', 'Şub', 'Mar', 'Nis', 'May', 'Haz', 'Tem', 'Ağu', 'Eyl', 'Eki', 'Kas', 'Ara'],
            longhand: ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran', 'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık']
        },
        firstDayOfWeek: 1,
        ordinal: function () {
            return '.';
        },
        rangeSeparator: ' - ',
        weekAbbreviation: 'Hf',
        scrollTitle: 'Artırmak için kaydırın',
        toggleTitle: 'Aç/Kapa',
        amPM: ['AM', 'PM'],
        yearAriaLabel: 'Yıl',
        time_24hr: true
    };

    // Flatpickr varsa Türkçe dil ekle
    if (typeof flatpickr !== 'undefined') {
        if (flatpickr.l10ns) {
            flatpickr.l10ns.tr = Turkish;
        }
        // Global olarak Türkçe'yi default yap
        flatpickr.localize(Turkish);
    }

    // Date input'ları Flatpickr ile değiştir
    function initializeDatePickers() {
        // type="date" input'ları
        const dateInputs = document.querySelectorAll('input[type="date"]:not(.flatpickr-input)');
        dateInputs.forEach(input => {
            // Mevcut değeri al (yyyy-MM-dd formatında olabilir)
            const currentValue = input.value;

            // Flatpickr ile değiştir
            const fp = flatpickr(input, {
                dateFormat: 'Y-m-d', // DB formatı (yyyy-MM-dd) - Blazor için
                altInput: true, // Alternatif input göster
                altFormat: 'd.m.Y', // Kullanıcıya gösterilen format (dd.MM.yyyy)
                allowInput: true,
                clickOpens: true,
                disableMobile: true, // Mobilde native picker kullanma
                onChange: function (selectedDates, dateStr, instance) {
                    // Blazor için change event tetikle
                    const event = new Event('change', { bubbles: true });
                    input.dispatchEvent(event);
                },
                onReady: function (selectedDates, dateStr, instance) {
                    // Alt input'a Sneat form-control class'ı ekle
                    const altInput = instance.altInput;
                    if (altInput && !altInput.classList.contains('form-control')) {
                        altInput.classList.add('form-control');
                    }
                }
            });

            // Mevcut değeri set et
            if (currentValue) {
                fp.setDate(currentValue, false);
            }

            // Blazor'ın @bind için input event'i de tetikle
            input.addEventListener('blur', function () {
                const event = new Event('input', { bubbles: true });
                input.dispatchEvent(event);
            });
        });

        // type="datetime-local" input'ları
        const dateTimeInputs = document.querySelectorAll('input[type="datetime-local"]:not(.flatpickr-input)');
        dateTimeInputs.forEach(input => {
            const currentValue = input.value;

            const fp = flatpickr(input, {
                enableTime: true,
                time_24hr: true,
                dateFormat: 'Y-m-d H:i', // DB formatı (yyyy-MM-dd HH:mm) - Blazor için
                altInput: true, // Alternatif input göster
                altFormat: 'd.m.Y H:i', // Kullanıcıya gösterilen format (dd.MM.yyyy HH:mm)
                allowInput: true,
                clickOpens: true,
                disableMobile: true,
                onChange: function (selectedDates, dateStr, instance) {
                    const event = new Event('change', { bubbles: true });
                    input.dispatchEvent(event);
                },
                onReady: function (selectedDates, dateStr, instance) {
                    const altInput = instance.altInput;
                    if (altInput && !altInput.classList.contains('form-control')) {
                        altInput.classList.add('form-control');
                    }
                }
            });

            if (currentValue) {
                fp.setDate(currentValue, false);
            }

            input.addEventListener('blur', function () {
                const event = new Event('input', { bubbles: true });
                input.dispatchEvent(event);
            });
        });
    }

    // Sayfa yüklendiğinde initialize et
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initializeDatePickers);
    } else {
        initializeDatePickers();
    }

    // Blazor render sonrası yeniden initialize et (MutationObserver ile)
    const observer = new MutationObserver(function (mutations) {
        let shouldReinit = false;
        mutations.forEach(function (mutation) {
            mutation.addedNodes.forEach(function (node) {
                if (node.nodeType === 1) { // Element node
                    if (node.matches && node.matches('input[type="date"], input[type="datetime-local"]')) {
                        shouldReinit = true;
                    } else if (node.querySelectorAll) {
                        const inputs = node.querySelectorAll('input[type="date"], input[type="datetime-local"]');
                        if (inputs.length > 0) {
                            shouldReinit = true;
                        }
                    }
                }
            });
        });

        if (shouldReinit) {
            setTimeout(initializeDatePickers, 100);
        }
    });

    // Body'yi gözlemle
    observer.observe(document.body, {
        childList: true,
        subtree: true
    });

    // Global fonksiyon olarak da expose et
    window.initializeTurkishDatePickers = initializeDatePickers;

    console.log('✅ Turkish DatePicker Auto-Initializer yüklendi');
})();
