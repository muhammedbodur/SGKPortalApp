// ═══════════════════════════════════════════════════════
// TAKVİM - FULLCALENDAR INITIALIZATION
// ═══════════════════════════════════════════════════════

let takvimCalendar = null;

/**
 * Takvim sayfasını başlatır (full page)
 * @param {string} eventsJson - JSON formatında event listesi
 * @param {number} year - Gösterilecek yıl
 */
window.initTakvimCalendar = function (eventsJson, year) {
    try {
        console.log('📅 initTakvimCalendar çağrıldı, yıl:', year);

        // FullCalendar yüklü mü kontrol et (Sneat window.Calendar olarak export ediyor)
        if (typeof Calendar === 'undefined') {
            console.error('❌ FullCalendar kütüphanesi yüklenmemiş!');
            return;
        }

        const events = JSON.parse(eventsJson);
        console.log('📊 Event sayısı:', events.length);

        const calendarEl = document.getElementById('takvimCalendar');

        if (!calendarEl) {
            console.error('❌ Calendar element bulunamadı: #takvimCalendar');
            return;
        }

        console.log('✅ Calendar element bulundu');

        // Mevcut takvimi temizle
        if (takvimCalendar) {
            console.log('♻️ Mevcut takvim temizleniyor...');
            takvimCalendar.destroy();
        }

        // FullCalendar başlat
        console.log('🔧 FullCalendar oluşturuluyor...');
        takvimCalendar = new Calendar(calendarEl, {
            // Plugin'ler (Sneat FullCalendar'dan)
            plugins: [dayGridPlugin, interactionPlugin, listPlugin, timegridPlugin],
            
            // Görünüm ayarları
            initialView: 'dayGridMonth',
            initialDate: new Date(year, new Date().getMonth(), 1), // Seçilen yılın şu anki ayına başla
            
            // Header toolbar
            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: 'dayGridMonth,timeGridWeek,timeGridDay,listMonth'
            },
            
            // Navigasyon
            navLinks: true,
            editable: false,
            dragScroll: false,

            // Yerelleştirme (Türkçe)
            locale: 'tr',
            buttonText: {
                today: 'Bugün',
                month: 'Ay',
                week: 'Hafta',
                day: 'Gün',
                list: 'Liste'
            },
            allDayText: 'Tüm gün',
            moreLinkText: 'daha fazla',
            noEventsText: 'Gösterilecek etkinlik yok',
            weekText: 'Hf',
            firstDay: 1,
            
            // Format ayarları
            dayHeaderFormat: { weekday: 'long' },
            titleFormat: { year: 'numeric', month: 'long' },

            // Event'ler
            events: events,

            // Event tıklama
            eventClick: function (info) {
                try {
                    const eventType = info.event.extendedProps?.eventType;
                    const eventId = info.event.id;

                    // Event tipine göre işlem yap
                    if (eventType === 'tatil') {
                        // Tatil edit sayfasına git
                        const tatilId = parseInt(eventId.replace('tatil-', ''));
                        window.location.href = `/common/takvim/manage/${tatilId}`;
                    } else if (eventType === 'mesai') {
                        // Mesai detaylarını göster (tooltip veya modal)
                        console.log('Mesai detayı:', info.event.extendedProps);
                    } else if (eventType === 'izin' || eventType === 'mazeret') {
                        // İzin/Mazeret detay sayfasına git
                        console.log('İzin/Mazeret detayı:', info.event.extendedProps);
                    }
                } catch (err) {
                    console.error('Event click hatası:', err);
                }
            },

            // Event görünümü
            eventContent: function (arg) {
                // Sneat teması ile uyumlu event rendering
                return {
                    html: '<div class="fc-event-title-container"><div class="fc-event-title fc-sticky">' + arg.event.title + '</div></div>'
                };
            },

            // Tooltip (event üzerine gelince)
            eventMouseEnter: function (info) {
                try {
                    const props = info.event.extendedProps;
                    let tooltipContent = '<div class="p-2">';
                    tooltipContent += `<strong>${info.event.title}</strong><br>`;

                    // Event tipine göre içerik
                    if (props && props.eventType === 'tatil') {
                        tooltipContent += `<small class="text-muted">${props.tatilTipi || ''}</small><br>`;
                        if (props.aciklama) {
                            tooltipContent += `<small>${props.aciklama}</small><br>`;
                        }
                    } else if (props && props.eventType === 'mesai') {
                        tooltipContent += `<small>Giriş: ${props.girisSaati || '?'}</small><br>`;
                        tooltipContent += `<small>Çıkış: ${props.cikisSaati || '?'}</small><br>`;
                        //if (props.mesaiSuresi) {
                        //    tooltipContent += `<small>Mesai Süresi: ${props.mesaiSuresi}</small><br>`;
                        //}
                        if (props.gecKalma) {
                            tooltipContent += '<small class="text-danger">⚠️ Geç Kalma</small><br>';
                        }
                        if (props.detay) {
                            tooltipContent += `<small>${props.detay}</small><br>`;
                        }
                    } else if (props && props.eventType === 'izin') {
                        tooltipContent += `<small>${props.tur || ''}</small><br>`;
                        tooltipContent += `<small>Durum: ${props.onayDurumu || 'Beklemede'}</small><br>`;
                    } else if (props && props.eventType === 'mazeret') {
                        tooltipContent += `<small>${props.tur || ''}</small><br>`;
                        if (props.saatDilimi) {
                            tooltipContent += `<small>Saat: ${props.saatDilimi}</small><br>`;
                        }
                        tooltipContent += `<small>Durum: ${props.onayDurumu || 'Beklemede'}</small><br>`;
                    }

                    tooltipContent += '</div>';

                    // Bootstrap tooltip kullan (jQuery ve Bootstrap yüklüyse)
                    if (typeof $ !== 'undefined' && $.fn.tooltip) {
                        $(info.el).tooltip({
                            title: tooltipContent,
                            html: true,
                            placement: 'top',
                            trigger: 'hover',
                            container: 'body'
                        });
                    }
                } catch (err) {
                    console.error('Tooltip oluşturma hatası:', err);
                }
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
        takvimCalendar.render();
        console.log(`✅ Takvim başlatıldı (${year})`);

    } catch (error) {
        console.error('Takvim başlatma hatası:', error);
    }
};

/**
 * Takvimi yeniden yükler
 * @param {string} eventsJson - Yeni event listesi
 */
window.refreshTakvimCalendar = function (eventsJson) {
    try {
        if (!takvimCalendar) {
            console.warn('Takvim henüz başlatılmamış');
            return;
        }

        const events = JSON.parse(eventsJson);
        
        // Tüm event'leri temizle
        takvimCalendar.removeAllEvents();
        
        // Yeni event'leri ekle
        takvimCalendar.addEventSource(events);
        
        console.log('✅ Takvim yenilendi');
    } catch (error) {
        console.error('Takvim yenileme hatası:', error);
    }
};

/**
 * Takvimi temizler
 */
window.destroyTakvimCalendar = function () {
    if (takvimCalendar) {
        takvimCalendar.destroy();
        takvimCalendar = null;
        console.log('✅ Takvim temizlendi');
    }
};

// ═══════════════════════════════════════════════════════
// DASHBOARD WIDGET CALENDAR
// ═══════════════════════════════════════════════════════

let takvimWidgetCalendar = null;

/**
 * Dashboard widget için mini takvim başlatır
 * @param {string} eventsJson - JSON formatında event listesi
 * @param {number} year - Gösterilecek yıl
 */
window.initTakvimWidgetCalendar = function (eventsJson, year) {
    try {
        const events = JSON.parse(eventsJson);
        const calendarEl = document.getElementById('takvimWidgetCalendar');

        if (!calendarEl) {
            console.error('Widget calendar element bulunamadı');
            return;
        }

        // Mevcut takvimi temizle
        if (takvimWidgetCalendar) {
            takvimWidgetCalendar.destroy();
        }

        // FullCalendar başlat (compact mode)
        takvimWidgetCalendar = new Calendar(calendarEl, {
            // Plugin'ler
            plugins: [dayGridPlugin, interactionPlugin],

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
            buttonText: {
                today: 'Bugün',
                prev: 'Önceki',
                next: 'Sonraki'
            },
            allDayText: 'Tüm gün',
            firstDay: 1,

            // Event'ler
            events: events,

            // Event görünümü (compact)
            eventContent: function (arg) {
                return { html: '<div class="fc-event-title">' + arg.event.title + '</div>' };
            },

            // Click -> Detay sayfasına git
            eventClick: function (info) {
                window.location.href = '/common/takvim';
            },

            // Responsive ayarlar - Widget için kompakt
            height: 500, // Sabit yükseklik
            aspectRatio: 1.35, // En/Boy oranı

            // Hafta sonu vurgulama
            dayCellDidMount: function (info) {
                if (info.date.getDay() === 0 || info.date.getDay() === 6) {
                    info.el.style.backgroundColor = '#f8f9fa';
                }
            }
        });

        // Takvimi render et
        takvimWidgetCalendar.render();
        console.log(`✅ Widget takvimi başlatıldı (${year})`);

    } catch (error) {
        console.error('Widget takvim başlatma hatası:', error);
    }
};
