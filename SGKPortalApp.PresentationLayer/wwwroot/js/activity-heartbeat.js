/**
 * Aktivite Heartbeat
 * Kullanıcının idle session timeout'ı için her X dakikada bir server'a ping atar
 * Son aktivite zamanını günceller
 */

let activityHeartbeatTimer = null;

/**
 * Aktivite heartbeat'i başlatır
 * @param {number} intervalMinutes - Heartbeat interval (dakika cinsinden)
 */
window.startActivityHeartbeat = function (intervalMinutes) {
    // Önceki timer varsa temizle
    if (activityHeartbeatTimer) {
        clearInterval(activityHeartbeatTimer);
    }

    console.log(`✅ Aktivite heartbeat başlatıldı: ${intervalMinutes} dakika interval`);

    // İlk ping'i hemen at
    sendActivityPing();

    // Periyodik heartbeat başlat
    const intervalMs = intervalMinutes * 60 * 1000; // Dakikayı milisaniyeye çevir
    activityHeartbeatTimer = setInterval(function () {
        sendActivityPing();
    }, intervalMs);
};

/**
 * Aktivite heartbeat'i durdurur
 */
window.stopActivityHeartbeat = function () {
    if (activityHeartbeatTimer) {
        clearInterval(activityHeartbeatTimer);
        activityHeartbeatTimer = null;
        console.log('⏹️ Aktivite heartbeat durduruldu');
    }
};

/**
 * Server'a aktivite ping'i gönderir
 */
function sendActivityPing() {
    // API endpoint'e POST request at
    fetch('/api/auth/ping-activity', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(response => {
            if (response.ok) {
                console.log('🔔 Aktivite ping başarılı');
            } else {
                console.warn('⚠️ Aktivite ping başarısız:', response.status);
            }
        })
        .catch(error => {
            console.error('❌ Aktivite ping hatası:', error);
        });
}
