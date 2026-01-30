export function initSearchShortcut(dotnetHelper) {
    // CTRL + K veya CMD + K ile arama modal'ını aç
    document.addEventListener('keydown', function (e) {
        // CTRL + K (Windows/Linux) veya CMD + K (Mac)
        if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
            e.preventDefault();
            dotnetHelper.invokeMethodAsync('OpenSearchModal');
        }
        
        // ESC ile modal'ı kapat (modal açıkken)
        if (e.key === 'Escape') {
            const modal = document.querySelector('.modal.show');
            if (modal) {
                e.preventDefault();
                // Modal'ın close butonuna tıkla
                const closeBtn = modal.querySelector('.btn-close');
                if (closeBtn) {
                    closeBtn.click();
                }
            }
        }
    });
}
