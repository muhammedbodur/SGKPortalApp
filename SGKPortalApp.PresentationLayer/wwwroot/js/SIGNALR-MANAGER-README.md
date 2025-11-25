# SignalR Connection Manager - Kullanım Kılavuzu

## 📋 Genel Bakış

`signalr-connection-manager.js` tüm proje için merkezi SignalR bağlantı yönetimi sağlar.

### ✨ Özellikler

- ✅ **Otomatik yeniden bağlanma** - Bağlantı koptuğunda otomatik olarak yeniden bağlanır
- ✅ **10 saniyede bir kontrol** - Bağlantı durumunu periyodik olarak kontrol eder
- ✅ **Event-driven mimari** - Custom event'ler ile esnek kullanım
- ✅ **Merkezi yönetim** - Tüm projede tek bir bağlantı instance'ı
- ✅ **Detaylı loglama** - Console'da renkli ve açıklayıcı loglar
- ✅ **Otomatik temizlik** - Sayfa kapatılırken bağlantıyı düzgün kapatır

---

## 🚀 Kurulum

### 1. Script Referansları

`_Host.cshtml` veya layout dosyanıza ekleyin:

```html
<!-- SignalR Library -->
<script src="/lib/microsoft/signalr/dist/browser/signalr.js"></script>

<!-- SignalR Connection Manager -->
<script src="~/js/signalr-connection-manager.js"></script>
```

### 2. Manager'ı Başlatma

```javascript
// Global manager'ı başlat
const manager = initializeSignalR('/hubs/tv');

// Manager'ı başlat ve bağlan
await manager.initialize();
```

---

## 📖 Kullanım Örnekleri

### Temel Kullanım

```javascript
// Manager'ı başlat
const manager = initializeSignalR('/hubs/tv');

// Bağlantıyı başlat
manager.initialize().then(() => {
    console.log('Bağlantı hazır!');
    
    // Hub metodunu dinle
    manager.on("ReceiveSiraUpdate", (data) => {
        console.log('Yeni sıra:', data);
    });
    
    // Hub metodunu çağır
    manager.invoke("JoinTvGroup", 1);
});
```

### Event Listeners

```javascript
// Bağlantı kurulduğunda
manager.addEventListener('connected', () => {
    console.log('✅ Bağlantı kuruldu');
});

// Yeniden bağlanıldığında
manager.addEventListener('reconnected', (connectionId) => {
    console.log('✅ Yeniden bağlandı:', connectionId);
});

// Durum kontrolü yapıldığında
manager.addEventListener('statusChecked', (state) => {
    console.log('Durum:', state);
});
```

### Hub Event'lerini Dinleme

```javascript
// Sıra güncelleme
manager.on("ReceiveSiraUpdate", (data) => {
    console.log('Yeni sıra çağrıldı:', data);
    // Ses çal, animasyon göster, vb.
});

// Duyuru güncelleme
manager.on("ReceiveDuyuruUpdate", (duyuru) => {
    console.log('Duyuru güncellendi:', duyuru);
    document.getElementById('duyuruText').textContent = duyuru;
});

// Custom event
manager.on("CustomEvent", (data) => {
    console.log('Custom event:', data);
});
```

### Hub Metodlarını Çağırma

```javascript
// Gruba katıl
await manager.invoke("JoinTvGroup", tvId);

// Mesaj gönder
await manager.invoke("SendMessage", "Merhaba");

// Veri al
const result = await manager.invoke("GetData", 123);
console.log('Sonuç:', result);
```

### Bağlantı Durumu Kontrolü

```javascript
// Bağlı mı?
if (manager.isConnected()) {
    console.log('Bağlantı aktif');
}

// Durum bilgisi
const info = manager.getInfo();
console.log('Durum:', info.state);
console.log('Connection ID:', info.connectionId);
console.log('Hub URL:', info.hubUrl);
```

---

## 🎯 TV Ekranı Örneği

```javascript
// TV ekranı için SignalR kurulumu
const manager = initializeSignalR('/hubs/tv');

// Bağlantı event'leri
manager.addEventListener('connected', async () => {
    console.log('✅ TV ekranı bağlandı');
    await manager.invoke("JoinTvGroup", @TvId);
});

manager.addEventListener('reconnected', async () => {
    console.log('✅ TV ekranı yeniden bağlandı');
    await manager.invoke("JoinTvGroup", @TvId);
});

// Hub event'lerini dinle
manager.initialize().then(() => {
    // Sıra çağrıldığında
    manager.on("ReceiveSiraUpdate", (data) => {
        playSiraSound();
        highlightNewSira(data.bankoId);
        setTimeout(() => location.reload(), 2000);
    });
    
    // Duyuru güncellendiğinde
    manager.on("ReceiveDuyuruUpdate", (duyuru) => {
        document.getElementById('duyuruText').textContent = duyuru;
    });
    
    // TV grubuna katıl
    manager.invoke("JoinTvGroup", @TvId);
});
```

---

## 🎯 Admin Panel Örneği

```javascript
// Admin panel için SignalR kurulumu
const manager = initializeSignalR('/hubs/admin');

manager.initialize().then(() => {
    // Kullanıcı aktivitesi
    manager.on("UserActivity", (activity) => {
        updateActivityLog(activity);
    });
    
    // Sistem bildirimi
    manager.on("SystemNotification", (notification) => {
        showToast(notification.message, notification.type);
    });
    
    // Admin grubuna katıl
    manager.invoke("JoinAdminGroup");
});
```

---

## 📊 Bağlantı Durumları

| Durum | Açıklama |
|-------|----------|
| `Disconnected` | Bağlantı kopuk |
| `Connected` | Bağlantı aktif |
| `Connecting` | Bağlanıyor |
| `Reconnecting` | Yeniden bağlanıyor |
| `NotInitialized` | Henüz başlatılmadı |

---

## 🔧 Yapılandırma

### Yeniden Bağlanma Aralıkları

```javascript
// Custom yeniden bağlanma aralıkları (ms)
const manager = new SignalRConnectionManager(
    '/hubs/tv',
    [0, 1000, 3000, 5000, 10000, 30000]
);
```

### Kontrol Aralığı

```javascript
// Manager'ı başlat
const manager = initializeSignalR('/hubs/tv');

// Kontrol aralığını değiştir (varsayılan: 10000ms)
manager.checkIntervalMs = 5000; // 5 saniye

await manager.initialize();
```

---

## 🐛 Hata Ayıklama

### Console Logları

Manager otomatik olarak detaylı loglar üretir:

```
✅ SignalR bağlantı yöneticisi başlatıldı
✅ SignalR bağlantısı kuruldu. State: Connected
⏱️ Bağlantı kontrolü başlatıldı (10 saniye)
🔍 Bağlantı durumu: Connected
📡 Event listener eklendi: ReceiveSiraUpdate
📤 Metod çağrıldı: JoinTvGroup [1]
```

### Bağlantı Bilgilerini Görüntüleme

```javascript
const info = manager.getInfo();
console.table(info);
```

### Manuel Bağlantı Kontrolü

```javascript
// Anında kontrol yap
await manager.checkConnection();
```

---

## ⚠️ Önemli Notlar

1. **Tek Instance**: Tüm projede tek bir manager instance'ı kullanın
2. **Otomatik Temizlik**: Sayfa kapatılırken otomatik olarak temizlenir
3. **Event Handlers**: Event handler'ları initialize'dan sonra ekleyin
4. **Async/Await**: invoke metodları async'tir, await kullanın
5. **Error Handling**: invoke çağrılarında try-catch kullanın

---

## 📝 Best Practices

### ✅ Yapılması Gerekenler

```javascript
// Manager'ı global olarak kullan
const manager = window.signalRManager || initializeSignalR('/hubs/tv');

// Event handler'ları initialize'dan sonra ekle
await manager.initialize();
manager.on("MyEvent", handler);

// Async metodları await ile çağır
await manager.invoke("MyMethod", param);
```

### ❌ Yapılmaması Gerekenler

```javascript
// Her sayfada yeni manager oluşturma
const manager1 = new SignalRConnectionManager('/hubs/tv');
const manager2 = new SignalRConnectionManager('/hubs/tv'); // ❌

// Initialize etmeden kullanma
manager.on("MyEvent", handler); // ❌ Önce initialize et

// Async metodları await olmadan çağırma
manager.invoke("MyMethod"); // ❌ await ekle
```

---

## 🆘 Sorun Giderme

### Bağlantı Kurulamıyor

```javascript
// Hub URL'ini kontrol et
console.log('Hub URL:', manager.hubUrl);

// Bağlantı durumunu kontrol et
console.log('Durum:', manager.getConnectionState());

// Manuel başlatma dene
await manager.start();
```

### Event'ler Çalışmıyor

```javascript
// Event listener'ın eklendiğinden emin ol
manager.on("MyEvent", (data) => {
    console.log('Event alındı:', data);
});

// Bağlantının aktif olduğunu kontrol et
if (!manager.isConnected()) {
    console.error('Bağlantı aktif değil!');
}
```

---

## 📚 API Referansı

### Metodlar

| Metod | Açıklama | Dönüş |
|-------|----------|-------|
| `initialize()` | Manager'ı başlat | `Promise<void>` |
| `start()` | Bağlantıyı başlat | `Promise<boolean>` |
| `stop()` | Bağlantıyı kapat | `Promise<void>` |
| `on(method, handler)` | Hub event'ini dinle | `void` |
| `invoke(method, ...args)` | Hub metodunu çağır | `Promise<any>` |
| `isConnected()` | Bağlı mı kontrol et | `boolean` |
| `getConnectionState()` | Durum bilgisi al | `string` |
| `getInfo()` | Detaylı bilgi al | `object` |
| `addEventListener(event, handler)` | Custom event dinle | `void` |
| `checkConnection()` | Manuel kontrol yap | `Promise<void>` |

### Custom Events

| Event | Tetiklenme | Parametre |
|-------|-----------|-----------|
| `connected` | Bağlantı kurulduğunda | - |
| `reconnected` | Yeniden bağlanıldığında | `connectionId` |
| `statusChecked` | Durum kontrolü yapıldığında | `state` |

---

## 📞 Destek

Sorun yaşarsanız:
1. Console loglarını kontrol edin
2. `manager.getInfo()` ile durum bilgisini alın
3. Network sekmesinde WebSocket bağlantısını kontrol edin

---

**Son Güncelleme:** 24 Kasım 2025
**Versiyon:** 1.0.0
