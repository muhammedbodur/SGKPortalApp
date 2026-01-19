// ═══════════════════════════════════════════════════════
// RESMİ TATİL TAKVİMİ - FULLCALENDAR INITIALIZATION
// ═══════════════════════════════════════════════════════

let resmiTatilCalendar = null;

/**
 * Resmi Tatil takvimini başlatır
 * @param {string} eventsJson - JSON formatında event listesi
 * @param {number} year - Gösterilecek yıl
 */
window.initResmiTatilCalendar = function (eventsJson, year) {
    try {
        const events = JSON.parse(eventsJson);
        const calendarEl = document.getElementById('resmiTatilCalendar');

        if (!calendarEl) {
            console.error('Calendar element bulunamadı: #resmiTatilCalendar');
            return;
        }

        // Mevcut takvimi temizle
        if (resmiTatilCalendar) {
            resmiTatilCalendar.destroy();
        }

        // FullCalendar başlat
        resmiTatilCalendar = new FullCalendar.Calendar(calendarEl, {
            // Görünüm ayarları
            initialView: 'dayGridMonth',
            initialDate: `${year}-01-01`,
            
            // Header toolbar
            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: 'dayGridMonth,dayGridWeek,listMonth'
            },

            // Yerelleştirme
            locale: 'tr',
            buttonText: {
                today: 'Bugün',
                month: 'Ay',
                week: 'Hafta',
                list: 'Liste'
            },

            // Event'ler
            events: events,

            // Event tıklama
            eventClick: function (info) {
                const eventId = parseInt(info.event.id);
                
                // Blazor'a event ID'yi gönder
                if (window.DotNet) {
                    DotNet.invokeMethodAsync('SGKPortalApp.PresentationLayer', 'OnEventClick', eventId)
                        .catch(err => console.error('OnEventClick hatası:', err));
                }
            },

            // Event görünümü
            eventContent: function (arg) {
                const props = arg.event.extendedProps;
                
                let html = '<div class="fc-event-main-frame">';
                html += '<div class="fc-event-title-container">';
                html += '<div class="fc-event-title fc-sticky">';
                
                // Yarım gün ise özel işaret
                if (props.yariGun) {
                    html += '<i class="bx bx-time-five me-1"></i>';
                }
                
                html += arg.event.title;
                html += '</div>';
                html += '</div>';
                html += '</div>';

                return { html: html };
            },

            // Tooltip (event üzerine gelince)
            eventMouseEnter: function (info) {
                const props = info.event.extendedProps;
                
                let tooltipContent = `
                    <div class="p-2">
                        <strong>${info.event.title}</strong><br>
                        <small class="text-muted">${props.tatilTipi}</small><br>
                `;
                
                if (props.yariGun) {
                    tooltipContent += '<small><i class="bx bx-time-five"></i> Yarım Gün</small><br>';
                }
                
                if (props.aciklama) {
                    tooltipContent += `<small>${props.aciklama}</small><br>`;
                }
                
                if (props.otomatikSenkronize) {
                    tooltipContent += '<small class="text-info"><i class="bx bx-sync"></i> Otomatik</small>';
                }
                
                tooltipContent += '</div>';

                // Bootstrap tooltip kullan
                $(info.el).tooltip({
                    title: tooltipContent,
                    html: true,
                    placement: 'top',
                    trigger: 'hover',
                    container: 'body'
                });
            },

            // Takvim yüklendi
            loading: function (isLoading) {
                if (isLoading) {
                    console.log('Takvim yükleniyor...');
                } else {
                    console.log('Takvim yüklendi');
                }
            },

            // Responsive ayarlar
            height: 'auto',
            contentHeight: 'auto',
            aspectRatio: 1.8,

            // Hafta sonu vurgulama
            dayCellDidMount: function (info) {
                if (info.date.getDay() === 0 || info.date.getDay() === 6) {
                    info.el.style.backgroundColor = '#f8f9fa';
                }
            }
        });

        // Takvimi render et
        resmiTatilCalendar.render();
        console.log(`✅ Resmi Tatil takvimi başlatıldı (${year})`);

    } catch (error) {
        console.error('Takvim başlatma hatası:', error);
    }
};

/**
 * Takvimi yeniden yükler
 * @param {string} eventsJson - Yeni event listesi
 */
window.refreshResmiTatilCalendar = function (eventsJson) {
    try {
        if (!resmiTatilCalendar) {
            console.warn('Takvim henüz başlatılmamış');
            return;
        }

        const events = JSON.parse(eventsJson);
        
        // Tüm event'leri temizle
        resmiTatilCalendar.removeAllEvents();
        
        // Yeni event'leri ekle
        resmiTatilCalendar.addEventSource(events);
        
        console.log('✅ Takvim yenilendi');
    } catch (error) {
        console.error('Takvim yenileme hatası:', error);
    }
};

/**
 * Takvimi temizler
 */
window.destroyResmiTatilCalendar = function () {
    if (resmiTatilCalendar) {
        resmiTatilCalendar.destroy();
        resmiTatilCalendar = null;
        console.log('✅ Takvim temizlendi');
    }
};

// ═══════════════════════════════════════════════════════
// DASHBOARD WIDGET CALENDAR
// ═══════════════════════════════════════════════════════

let resmiTatilWidgetCalendar = null;

/**
 * Dashboard widget için mini takvim başlatır
 * @param {string} eventsJson - JSON formatında event listesi
 * @param {number} year - Gösterilecek yıl
 */
window.initResmiTatilWidgetCalendar = function (eventsJson, year) {
    try {
        const events = JSON.parse(eventsJson);
        const calendarEl = document.getElementById('resmiTatilWidgetCalendar');

        if (!calendarEl) {
            console.error('Widget calendar element bulunamadı');
            return;
        }

        // Mevcut takvimi temizle
        if (resmiTatilWidgetCalendar) {
            resmiTatilWidgetCalendar.destroy();
        }

        // FullCalendar başlat (compact mode)
        resmiTatilWidgetCalendar = new FullCalendar.Calendar(calendarEl, {
            // Görünüm ayarları
            initialView: 'dayGridMonth',
            initialDate: `${year}-01-01`,

            // Compact header
            headerToolbar: {
                left: 'prev,next',
                center: 'title',
                right: ''
            },

            // Yerelleştirme
            locale: 'tr',

            // Event'ler
            events: events,

            // Event görünümü (compact)
            eventContent: function (arg) {
                return { html: '<div class="fc-event-title">' + arg.event.title + '</div>' };
            },

            // Click -> Detay sayfasına git
            eventClick: function (info) {
                window.location.href = '/common/resmitatil';
            },

            // Responsive ayarlar
            height: 'auto',
            contentHeight: 'auto',

            // Hafta sonu vurgulama
            dayCellDidMount: function (info) {
                if (info.date.getDay() === 0 || info.date.getDay() === 6) {
                    info.el.style.backgroundColor = '#f8f9fa';
                }
            }
        });

        // Takvimi render et
        resmiTatilWidgetCalendar.render();
        console.log(`✅ Widget takvimi başlatıldı (${year})`);

    } catch (error) {
        console.error('Widget takvim başlatma hatası:', error);
    }
};
