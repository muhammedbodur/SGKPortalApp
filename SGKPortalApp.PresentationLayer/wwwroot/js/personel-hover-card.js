// Simple Follow Cursor Tooltip
(function() {
    'use strict';
    
    let currentTooltip = null;
    let currentRow = null;
    let showTimer = null;
    let hideTimer = null;
    let lastMouseEvent = null;

    function updateTooltipPosition(e) {
        if (!currentTooltip) return;
        
        const offset = 15;
        const margin = 10;
        
        // Tooltip boyutlarını al
        const rect = currentTooltip.getBoundingClientRect();
        const tooltipWidth = rect.width;
        const tooltipHeight = rect.height;
        
        // Viewport boyutları
        const viewportWidth = window.innerWidth;
        const viewportHeight = window.innerHeight;
        
        // Varsayılan pozisyon: sağ alt (mouse'un sağında ve altında)
        let x = e.clientX + offset;
        let y = e.clientY + offset;
        
        // Sağ kenara taşıyorsa, sol tarafa koy
        if (x + tooltipWidth + margin > viewportWidth) {
            x = e.clientX - tooltipWidth - offset;
        }
        
        // Alt kenara taşıyorsa (görev çubuğu), üst tarafa koy
        if (y + tooltipHeight + margin > viewportHeight) {
            y = e.clientY - tooltipHeight - offset;
        }
        
        // Sol kenara taşmasını engelle
        if (x < margin) {
            x = margin;
        }
        
        // Üst kenara taşmasını engelle
        if (y < margin) {
            y = margin;
        }
        
        currentTooltip.style.left = x + 'px';
        currentTooltip.style.top = y + 'px';
    }

    function showTooltip(row) {
        const personelId = row.getAttribute('data-personel-row');
        if (!personelId) return;

        const tooltip = document.querySelector(`.personel-tooltip[data-tooltip-for="${personelId}"]`);
        if (!tooltip) return;

        // Önceki tooltip'i gizle
        if (currentTooltip && currentTooltip !== tooltip) {
            currentTooltip.classList.remove('show');
        }

        currentTooltip = tooltip;
        currentRow = row;
        
        // Önce göster ki boyutları hesaplayabilelim
        tooltip.classList.add('show');
        
        // İlk pozisyonu ayarla (lastMouseEvent zaten mevcut)
        if (lastMouseEvent) {
            updateTooltipPosition(lastMouseEvent);
        }
    }

    function hideTooltip() {
        if (currentTooltip) {
            currentTooltip.classList.remove('show');
            currentTooltip = null;
            currentRow = null;
        }
    }

    // Mouse move - tooltip'i takip et ve son pozisyonu sakla
    document.addEventListener('mousemove', function(e) {
        lastMouseEvent = e;

        // İşlemler sütununda ise tooltip'i gizle
        const isInActionsColumn = e.target.closest('.actions-column') ||
                                   e.target.closest('button') ||
                                   e.target.closest('.dropdown') ||
                                   e.target.closest('.btn-group') ||
                                   e.target.closest('[data-no-tooltip]');
        if (isInActionsColumn && currentTooltip) {
            clearTimeout(showTimer);
            hideTooltip();
            return;
        }

        updateTooltipPosition(e);
    });

    // TR satırına mouseenter
    document.addEventListener('mouseenter', function(e) {
        const row = e.target.closest('tr.personel-row-trigger');
        if (!row) return;

        // İşlemler sütununda ise tooltip gösterme
        const isInActionsColumn = e.target.closest('.actions-column') ||
                                   e.target.closest('button') ||
                                   e.target.closest('.dropdown') ||
                                   e.target.closest('.btn-group') ||
                                   e.target.closest('[data-no-tooltip]');
        if (isInActionsColumn) {
            clearTimeout(showTimer);
            hideTooltip();
            return;
        }

        // Timer'ları temizle
        clearTimeout(showTimer);
        clearTimeout(hideTimer);

        // 1 saniye bekle
        showTimer = setTimeout(function() {
            showTooltip(row);
        }, 1000);
    }, true);

    // TR satırından mouseleave
    document.addEventListener('mouseleave', function(e) {
        const row = e.target.closest('tr.personel-row-trigger');
        if (!row) return;

        // Show timer'ı iptal et
        clearTimeout(showTimer);

        // Hemen gizle
        hideTimer = setTimeout(function() {
            hideTooltip();
        }, 100);
    }, true);

})();
