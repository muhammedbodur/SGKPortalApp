// Flatpickr Blazor Integration
window.FlatpickrBlazor = {
    // Flatpickr'ı initialize et
    initialize: function () {
        // Sadece henüz initialize edilmemiş elementleri bul
        const uninitializedPickers = document.querySelectorAll('.flatpickr-date:not(.flatpickr-input)');

        uninitializedPickers.forEach(element => {
            flatpickr(element, {
                dateFormat: 'Y-m-d',
                allowInput: true,
                locale: {
                    firstDayOfWeek: 1,
                    weekdays: {
                        shorthand: ['Paz', 'Pzt', 'Sal', 'Çar', 'Per', 'Cum', 'Cmt'],
                        longhand: ['Pazar', 'Pazartesi', 'Salı', 'Çarşamba', 'Perşembe', 'Cuma', 'Cumartesi']
                    },
                    months: {
                        shorthand: ['Oca', 'Şub', 'Mar', 'Nis', 'May', 'Haz', 'Tem', 'Ağu', 'Eyl', 'Eki', 'Kas', 'Ara'],
                        longhand: ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran', 'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık']
                    }
                },
                onChange: function (selectedDates, dateStr, instance) {
                    // Blazor'a değişikliği bildir
                    element.value = dateStr;
                    element.dispatchEvent(new Event('change', { bubbles: true }));
                }
            });
        });
    }
};

// Sayfa yüklendiğinde
document.addEventListener('DOMContentLoaded', function () {
    FlatpickrBlazor.initialize();
});

// Blazor render'dan sonra
if (window.Blazor) {
    Blazor.addEventListener('enhancedload', function () {
        FlatpickrBlazor.initialize();
    });
}

// MutationObserver ile sadece yeni eklenen elementleri yakala
let flatpickrTimeout;
const flatpickrObserver = new MutationObserver(() => {
    clearTimeout(flatpickrTimeout);
    flatpickrTimeout = setTimeout(() => {
        FlatpickrBlazor.initialize();
    }, 200);
});
flatpickrObserver.observe(document.body, { childList: true, subtree: true });
