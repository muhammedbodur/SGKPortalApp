# 📡 SignalR Hub Kullanım Kılavuzu

## 📋 İçindekiler

1. [Genel Bakış](#genel-bakış)
2. [Mimari Yapı](#mimari-yapı)
3. [Kurulum](#kurulum)
4. [Kullanım Örnekleri](#kullanım-örnekleri)
5. [API Referansı](#api-referansı)
6. [Best Practices](#best-practices)
7. [Sorun Giderme](#sorun-giderme)

---

## 🎯 Genel Bakış

SGK Portal projesi için profesyonel SignalR Hub altyapısı. Gerçek zamanlı iletişim, TV ekranı yönetimi, sıra çağırma ve bildirim sistemleri için kullanılır.

### ✨ Özellikler

- ✅ **BaseHub** - Tüm Hub'lar için ortak temel sınıf
- ✅ **SiramatikHub** - TV ekranları ve sıra yönetimi
- ✅ **HubConnectionService** - Bağlantı durumu yönetimi
- ✅ **Otomatik bağlantı takibi** - TV'lerin online/offline durumu
- ✅ **Grup yönetimi** - TV bazlı mesajlaşma
- ✅ **Admin bildirimleri** - Merkezi bildirim sistemi
- ✅ **Ping/Pong** - Bağlantı canlılığı kontrolü

---

## 🏗️ Mimari Yapı

### Klasör Yapısı

```
Services/
└── Hubs/
    ├── Base/
    │   └── BaseHub.cs                    # Temel Hub sınıfı
    ├── Interfaces/
    │   └── IHubConnectionService.cs      # Bağlantı servisi interface
    ├── Concrete/
    │   └── HubConnectionService.cs       # Bağlantı servisi implementation
    └── SiramatikHub.cs                   # Sıramatik Hub
```

### Katman İlişkileri

```
┌─────────────────────────────────────────┐
│         Client (JavaScript)              │
│  - signalr-connection-manager.js        │
└──────────────┬──────────────────────────┘
               │ SignalR Connection
               ↓
┌─────────────────────────────────────────┐
│         SiramatikHub                     │
│  - JoinTvGroup()                        │
│  - SendSiraUpdate()                     │
│  - SendDuyuruUpdate()                   │
└──────────────┬──────────────────────────┘
               │
               ↓
┌─────────────────────────────────────────┐
│      IHubConnectionService               │
│  - RegisterTvConnectionAsync()          │
│  - UnregisterTvConnectionAsync()        │
│  - IsTvConnectedAsync()                 │
└──────────────┬──────────────────────────┘
               │
               ↓
┌─────────────────────────────────────────┐
│         Database (EF Core)               │
│  - HubTvConnection Table                │
└─────────────────────────────────────────┘
```

---

## 🚀 Kurulum

### 1. NuGet Paketleri

```xml
<PackageReference Include="Microsoft.AspNetCore.SignalR" Version="1.1.0" />
```

### 2. Program.cs Konfigürasyonu

```csharp
// SignalR Servisleri
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.KeepAliveInterval = TimeSpan.FromSeconds(10);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
    options.MaximumReceiveMessageSize = 32 * 1024; // 32KB
});

// Hub Connection Service
builder.Services.AddScoped<IHubConnectionService, HubConnectionService>();

// Hub Endpoint
app.MapHub<SiramatikHub>("/hubs/tv");
```

### 3. _Host.cshtml Script Referansları

```html
<!-- SignalR Library -->
<script src="/lib/microsoft/signalr/dist/browser/signalr.js"></script>

<!-- SignalR Connection Manager -->
<script src="~/js/signalr-connection-manager.js"></script>
```

---

## 📖 Kullanım Örnekleri

### 1. TV Ekranı Bağlantısı

#### Client (JavaScript)

```javascript
// Global manager'ı başlat
const manager = initializeSignalR('/hubs/tv');

// Bağlantı event'lerini dinle
manager.addEventListener('connected', async () => {
    console.log('✅ TV bağlandı');
    // TV grubuna katıl
    await manager.invoke("JoinTvGroup", tvId);
});

// Hub'ı başlat
await manager.initialize();

// Sıra güncellemelerini dinle
manager.on("ReceiveSiraUpdate", (data) => {
    console.log('Yeni sıra:', data);
    playSiraSound();
    highlightNewSira(data.bankoId);
});

// Duyuru güncellemelerini dinle
manager.on("ReceiveDuyuruUpdate", (duyuru) => {
    document.getElementById('duyuruText').textContent = duyuru;
});
```

#### Server (C#)

```csharp
// Hub'dan TV'ye sıra gönder
await _hubContext.Clients.Group($"TV_{tvId}")
    .SendAsync("ReceiveSiraUpdate", new {
        bankoId = 1,
        bankoNo = 5,
        siraNo = 101,
        katTipi = "Zemin Kat"
    });
```

### 2. Sıra Çağırma Sistemi

#### Service Layer'dan Hub Çağırma

```csharp
public class SiraService
{
    private readonly IHubContext<SiramatikHub> _hubContext;
    
    public async Task CagirSira(int tvId, SiraDto sira)
    {
        // Veritabanı işlemleri...
        
        // TV'ye bildir
        await _hubContext.Clients.Group($"TV_{tvId}")
            .SendAsync("ReceiveSiraUpdate", sira);
            
        // Admin'lere bildir
        await _hubContext.Clients.Group("Admins")
            .SendAsync("ReceiveAdminNotification", new {
                message = $"Sıra çağrıldı: {sira.SiraNo}",
                type = "success"
            });
    }
}
```

### 3. Admin Panel Bildirimleri

#### Client (JavaScript)

```javascript
// Admin grubuna katıl
await manager.invoke("JoinAdminGroup");

// Admin bildirimlerini dinle
manager.on("ReceiveAdminNotification", (notification) => {
    showToast(notification.message, notification.type);
});
```

#### Server (C#)

```csharp
// Tüm admin'lere bildirim gönder
await _hubContext.Clients.Group("Admins")
    .SendAsync("ReceiveAdminNotification", new {
        message = "Yeni TV bağlandı",
        type = "info",
        timestamp = DateTime.Now
    });
```

### 4. Duyuru Yönetimi

#### Belirli Bir TV'ye Duyuru

```csharp
public async Task SendDuyuruToTv(int tvId, string duyuru)
{
    await _hubContext.Clients.Group($"TV_{tvId}")
        .SendAsync("ReceiveDuyuruUpdate", duyuru);
}
```

#### Tüm TV'lere Duyuru

```csharp
public async Task BroadcastDuyuru(string duyuru)
{
    await _hubContext.Clients.All
        .SendAsync("ReceiveDuyuruUpdate", duyuru);
}
```

### 5. Bağlantı Durumu Kontrolü

#### Service'den Kontrol

```csharp
public class TvService
{
    private readonly IHubConnectionService _connectionService;
    
    public async Task<bool> IsTvOnline(int tvId)
    {
        return await _connectionService.IsTvConnectedAsync(tvId);
    }
    
    public async Task<Dictionary<int, string>> GetAllOnlineTvs()
    {
        return await _connectionService.GetAllActiveConnectionsAsync();
    }
}
```

### 6. Ping/Pong - Bağlantı Canlılığı

#### Client (JavaScript)

```javascript
// Her 30 saniyede bir ping gönder
setInterval(async () => {
    try {
        await manager.invoke("Ping");
    } catch (error) {
        console.error('Ping hatası:', error);
    }
}, 30000);

// Pong yanıtını dinle
manager.on("Pong", (data) => {
    console.log('Pong alındı:', data.timestamp);
});
```

---

## 📚 API Referansı

### BaseHub Metodları

| Metod | Açıklama | Parametreler |
|-------|----------|--------------|
| `SendToGroupAsync` | Gruba mesaj gönder | `groupName`, `method`, `data` |
| `SendToUserAsync` | Kullanıcıya mesaj gönder | `userId`, `method`, `data` |
| `BroadcastAsync` | Herkese mesaj gönder | `method`, `data` |
| `SendToCallerAsync` | Çağırana mesaj gönder | `method`, `data` |
| `SendToOthersAsync` | Diğerlerine mesaj gönder | `method`, `data` |
| `JoinGroupAsync` | Gruba katıl | `groupName` |
| `LeaveGroupAsync` | Gruptan ayrıl | `groupName` |
| `GetConnectionInfo` | Bağlantı bilgilerini al | - |

### SiramatikHub Metodları

#### TV Group Management

| Metod | Açıklama | Parametreler | Dönüş |
|-------|----------|--------------|-------|
| `JoinTvGroup` | TV grubuna katıl | `tvId` | `Task` |
| `LeaveTvGroup` | TV grubundan ayrıl | `tvId` | `Task` |

#### Sıra Çağırma

| Metod | Açıklama | Parametreler | Dönüş |
|-------|----------|--------------|-------|
| `SendSiraUpdateToTv` | Belirli TV'ye sıra gönder | `tvId`, `siraData` | `Task` |
| `BroadcastSiraUpdate` | Tüm TV'lere sıra gönder | `siraData` | `Task` |

#### Duyuru Yönetimi

| Metod | Açıklama | Parametreler | Dönüş |
|-------|----------|--------------|-------|
| `SendDuyuruToTv` | Belirli TV'ye duyuru | `tvId`, `duyuru` | `Task` |
| `BroadcastDuyuru` | Tüm TV'lere duyuru | `duyuru` | `Task` |

#### Banko Yönetimi

| Metod | Açıklama | Parametreler | Dönüş |
|-------|----------|--------------|-------|
| `SendBankoUpdate` | Banko güncellemesi gönder | `tvId`, `bankoData` | `Task` |

#### Ping/Pong

| Metod | Açıklama | Parametreler | Dönüş |
|-------|----------|--------------|-------|
| `Ping` | Ping gönder | - | `Task` |
| `CheckTvConnection` | TV bağlantısını kontrol et | `tvId` | `Task<bool>` |

#### Admin

| Metod | Açıklama | Parametreler | Dönüş |
|-------|----------|--------------|-------|
| `JoinAdminGroup` | Admin grubuna katıl | - | `Task` |
| `LeaveAdminGroup` | Admin grubundan ayrıl | - | `Task` |
| `SendAdminNotification` | Admin bildirimi gönder | `message`, `type` | `Task` |

### IHubConnectionService Metodları

| Metod | Açıklama | Parametreler | Dönüş |
|-------|----------|--------------|-------|
| `RegisterTvConnectionAsync` | TV bağlantısını kaydet | `tvId`, `connectionId`, `status` | `Task<bool>` |
| `UnregisterTvConnectionAsync` | TV bağlantısını kaldır | `tvId`, `connectionId` | `Task<bool>` |
| `IsTvConnectedAsync` | TV bağlı mı kontrol et | `tvId` | `Task<bool>` |
| `GetTvIdByConnectionIdAsync` | ConnectionId'den TV ID al | `connectionId` | `Task<int?>` |
| `GetConnectionIdByTvIdAsync` | TV ID'den ConnectionId al | `tvId` | `Task<string?>` |
| `GetAllActiveConnectionsAsync` | Tüm aktif bağlantıları al | - | `Task<Dictionary<int, string>>` |
| `UpdateConnectionStatusAsync` | Bağlantı durumunu güncelle | `connectionId`, `status` | `Task<bool>` |

---

## 🎯 Best Practices

### 1. Dependency Injection

```csharp
public class MyService
{
    private readonly IHubContext<SiramatikHub> _hubContext;
    private readonly IHubConnectionService _connectionService;
    
    public MyService(
        IHubContext<SiramatikHub> hubContext,
        IHubConnectionService connectionService)
    {
        _hubContext = hubContext;
        _connectionService = connectionService;
    }
}
```

### 2. Error Handling

```csharp
try
{
    await _hubContext.Clients.Group($"TV_{tvId}")
        .SendAsync("ReceiveSiraUpdate", data);
}
catch (Exception ex)
{
    _logger.LogError(ex, $"Sıra güncellemesi gönderilemedi: TV#{tvId}");
    // Fallback mekanizması
}
```

### 3. Grup İsimlendirme

```csharp
// ✅ İyi
$"TV_{tvId}"
$"Banko_{bankoId}"
"Admins"

// ❌ Kötü
"tv1"
"group_123"
"users"
```

### 4. Mesaj Boyutu

```csharp
// ✅ İyi - Küçük ve öz veri
await _hubContext.Clients.Group($"TV_{tvId}")
    .SendAsync("ReceiveSiraUpdate", new { 
        bankoId = 1, 
        siraNo = 101 
    });

// ❌ Kötü - Gereksiz büyük veri
await _hubContext.Clients.Group($"TV_{tvId}")
    .SendAsync("ReceiveSiraUpdate", entireDatabaseObject);
```

### 5. Bağlantı Kontrolü

```csharp
// Her işlem öncesi kontrol et
if (await _connectionService.IsTvConnectedAsync(tvId))
{
    await _hubContext.Clients.Group($"TV_{tvId}")
        .SendAsync("ReceiveSiraUpdate", data);
}
else
{
    _logger.LogWarning($"TV offline: {tvId}");
}
```

---

## 🐛 Sorun Giderme

### Bağlantı Kurulamıyor

**Sorun:** `Failed to start the connection`

**Çözüm:**
1. Hub endpoint'ini kontrol edin: `/hubs/tv`
2. CORS ayarlarını kontrol edin
3. SignalR servisinin kayıtlı olduğundan emin olun

```csharp
// Program.cs
builder.Services.AddSignalR();
app.MapHub<SiramatikHub>("/hubs/tv");
```

### Mesajlar Alınamıyor

**Sorun:** Event handler çalışmıyor

**Çözüm:**
1. Event ismini kontrol edin (case-sensitive)
2. Gruba katıldığınızdan emin olun
3. Bağlantının aktif olduğunu kontrol edin

```javascript
// Doğru event ismi
manager.on("ReceiveSiraUpdate", handler); // ✅

// Yanlış event ismi
manager.on("receivesiraupdate", handler); // ❌
```

### TV "Bağlı Değil" Görünüyor

**Sorun:** `IsConnected = false`

**Çözüm:**
1. `JoinTvGroup` çağrıldığından emin olun
2. `HubTvConnection` tablosunu kontrol edin
3. Console loglarını inceleyin

```javascript
// TV grubuna katıl
await manager.invoke("JoinTvGroup", tvId);
```

### Bağlantı Sık Sık Kopuyor

**Sorun:** Frequent disconnections

**Çözüm:**
1. KeepAlive interval'i artırın
2. Timeout sürelerini uzatın
3. Network bağlantısını kontrol edin

```csharp
builder.Services.AddSignalR(options =>
{
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
});
```

### Memory Leak

**Sorun:** Memory kullanımı artıyor

**Çözüm:**
1. Event listener'ları temizleyin
2. Bağlantıyı düzgün kapatın
3. Dispose pattern kullanın

```javascript
// Sayfa kapatılırken
window.addEventListener('beforeunload', () => {
    manager.stop();
});
```

---

## 📊 Performans İpuçları

### 1. Grup Kullanımı

```csharp
// ✅ Verimli - Sadece ilgili TV'ye gönder
await _hubContext.Clients.Group($"TV_{tvId}")
    .SendAsync("ReceiveSiraUpdate", data);

// ❌ Verimsiz - Herkese gönder
await _hubContext.Clients.All
    .SendAsync("ReceiveSiraUpdate", data);
```

### 2. Mesaj Sıklığı

```csharp
// ✅ İyi - Throttling kullan
private DateTime _lastSent = DateTime.MinValue;

public async Task SendUpdate(object data)
{
    if ((DateTime.Now - _lastSent).TotalSeconds < 1)
        return; // 1 saniyede bir gönder
        
    _lastSent = DateTime.Now;
    await _hubContext.Clients.All.SendAsync("Update", data);
}
```

### 3. Batch İşlemler

```csharp
// ✅ İyi - Toplu gönder
var updates = GetAllUpdates();
await _hubContext.Clients.All.SendAsync("BatchUpdate", updates);

// ❌ Kötü - Tek tek gönder
foreach (var update in updates)
{
    await _hubContext.Clients.All.SendAsync("Update", update);
}
```

---

## 📞 Destek

Sorun yaşarsanız:
1. Console loglarını kontrol edin
2. Network sekmesinde WebSocket bağlantısını inceleyin
3. `manager.getInfo()` ile durum bilgisini alın
4. Database'de `HubTvConnection` tablosunu kontrol edin

---

## 📝 Changelog

### v1.0.0 (24 Kasım 2025)
- ✅ BaseHub temel sınıfı oluşturuldu
- ✅ SiramatikHub implement edildi
- ✅ HubConnectionService eklendi
- ✅ TV bağlantı yönetimi
- ✅ Sıra çağırma sistemi
- ✅ Duyuru yönetimi
- ✅ Admin bildirimleri
- ✅ Ping/Pong mekanizması

---

**Son Güncelleme:** 24 Kasım 2025  
**Versiyon:** 1.0.0  
**Yazar:** SGK Portal Development Team
