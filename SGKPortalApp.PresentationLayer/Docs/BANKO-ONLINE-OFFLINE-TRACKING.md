# 📱 Banko Online/Offline Takip Sistemi

## 🎯 Genel Bakış

Bankoların gerçek zamanlı bağlantı durumunu (online/offline) takip eden SignalR tabanlı sistem.

---

## 🏗️ Mimari

### Database Yapısı

```
HubBankoConnections
├── HubBankoConnectionId (PK)
├── BankoId (FK -> Bankolar) [UNIQUE]
├── ConnectionId (SignalR Connection ID)
├── ConnectionStatus (online/offline)
├── IslemZamani
├── KayitTarihi
└── DuzenlenmeTarihi
```

### Entity İlişkileri

```
Banko (1) ←→ (0..1) HubBankoConnection
```

---

## 📡 SignalR Hub Metodları

### Banko Grup Yönetimi

#### **JoinBankoGroup**
Banko'yu SignalR grubuna ekler ve bağlantıyı kaydeder.

```javascript
// Client
await manager.invoke("JoinBankoGroup", bankoId);
```

```csharp
// Server
public async Task JoinBankoGroup(int bankoId)
{
    await JoinGroupAsync($"Banko_{bankoId}");
    await _connectionService.RegisterBankoConnectionAsync(
        bankoId, 
        Context.ConnectionId, 
        ConnectionStatus.online
    );
}
```

#### **LeaveBankoGroup**
Banko'yu SignalR grubundan çıkarır.

```javascript
// Client
await manager.invoke("LeaveBankoGroup", bankoId);
```

#### **CheckBankoConnection**
Banko'nun bağlantı durumunu kontrol eder.

```javascript
// Client
const isOnline = await manager.invoke("CheckBankoConnection", bankoId);
console.log(`Banko ${bankoId} online: ${isOnline}`);
```

### Banko'ya Mesaj Gönderme

#### **SendMessageToBanko**
Belirli bir Banko'ya mesaj gönderir.

```csharp
// Server - Service Layer
await _hubContext.Clients.Group($"Banko_{bankoId}")
    .SendAsync("ReceiveMessage", new {
        message = "Yeni sıra var",
        timestamp = DateTime.Now
    });
```

#### **SendSiraToBanko**
Banko'ya sıra bildirimi gönderir.

```csharp
// Server
await _hubContext.Clients.Group($"Banko_{bankoId}")
    .SendAsync("ReceiveSiraNotification", new {
        siraNo = 101,
        islemTipi = "Emeklilik"
    });
```

#### **BroadcastToBankolar**
Tüm Bankolara mesaj gönderir.

```csharp
// Server
await _hubContext.Clients.All
    .SendAsync("ReceiveAnnouncement", new {
        message = "Sistem bakımı 10 dakika içinde başlayacak"
    });
```

---

## 💻 Client Tarafı Kullanım

### Banko Uygulaması Bağlantısı

```javascript
// SignalR Manager'ı başlat
const manager = initializeSignalR('/hubs/tv');

// Bağlantı event'lerini dinle
manager.addEventListener('connected', async () => {
    console.log('✅ Banko bağlandı');
    // Banko grubuna katıl
    await manager.invoke("JoinBankoGroup", bankoId);
});

manager.addEventListener('reconnected', async () => {
    console.log('✅ Banko yeniden bağlandı');
    // Banko grubuna tekrar katıl
    await manager.invoke("JoinBankoGroup", bankoId);
});

// Hub'ı başlat
await manager.initialize();

// Mesajları dinle
manager.on("ReceiveMessage", (data) => {
    console.log('📨 Mesaj alındı:', data);
    showNotification(data.message);
});

// Sıra bildirimlerini dinle
manager.on("ReceiveSiraNotification", (data) => {
    console.log('🔔 Yeni sıra:', data);
    playSiraSound();
    updateSiraDisplay(data);
});

// Duyuruları dinle
manager.on("ReceiveAnnouncement", (data) => {
    console.log('📢 Duyuru:', data);
    showAnnouncement(data.message);
});
```

---

## 🔧 Service Layer Kullanımı

### Banko Bağlantı Durumu Kontrolü

```csharp
public class SiraService
{
    private readonly IHubConnectionService _connectionService;
    private readonly IHubContext<SiramatikHub> _hubContext;
    
    public async Task<bool> IsBankoOnline(int bankoId)
    {
        return await _connectionService.IsBankoConnectedAsync(bankoId);
    }
    
    public async Task<Dictionary<int, string>> GetOnlineBankolar()
    {
        return await _connectionService.GetAllActiveBankoConnectionsAsync();
    }
}
```

### Banko'ya Sıra Gönderme

```csharp
public class SiraService
{
    private readonly IHubContext<SiramatikHub> _hubContext;
    private readonly IHubConnectionService _connectionService;
    
    public async Task SendSiraToBanko(int bankoId, SiraDto sira)
    {
        // Banko online mı kontrol et
        var isOnline = await _connectionService.IsBankoConnectedAsync(bankoId);
        
        if (isOnline)
        {
            // Banko'ya bildir
            await _hubContext.Clients.Group($"Banko_{bankoId}")
                .SendAsync("ReceiveSiraNotification", sira);
                
            _logger.LogInformation($"Sıra Banko'ya gönderildi: Banko#{bankoId}");
        }
        else
        {
            _logger.LogWarning($"Banko offline: Banko#{bankoId}");
            // Alternatif yöntem (SMS, email, vb.)
        }
    }
}
```

---

## 📊 BankoResponseDto

```csharp
public class BankoResponseDto
{
    public int BankoId { get; set; }
    public int BankoNo { get; set; }
    public string HizmetBinasiAdi { get; set; }
    public bool BankoMusaitMi { get; set; }
    
    // ✅ YENİ: Bağlantı durumu
    public bool IsConnected { get; set; }  // Online/Offline
    
    public PersonelAtamaDto? AtananPersonel { get; set; }
}
```

### API Response Örneği

```json
{
  "success": true,
  "data": [
    {
      "bankoId": 1,
      "bankoNo": 5,
      "hizmetBinasiAdi": "ALİAĞA SGM",
      "bankoMusaitMi": true,
      "isConnected": true,  // ✅ Online
      "atananPersonel": {
        "adSoyad": "Ahmet Yılmaz",
        "servisAdi": "Emeklilik"
      }
    },
    {
      "bankoId": 2,
      "bankoNo": 6,
      "hizmetBinasiAdi": "ALİAĞA SGM",
      "bankoMusaitMi": false,
      "isConnected": false,  // ❌ Offline
      "atananPersonel": null
    }
  ]
}
```

---

## 🎨 UI'da Gösterim

### Banko Listesi

```html
<div class="banko-card">
    <div class="banko-header">
        <h5>Banko #@banko.BankoNo</h5>
        <span class="badge bg-label-@(banko.IsConnected ? "success" : "secondary")">
            <i class="bx bx-wifi me-1"></i>
            @(banko.IsConnected ? "Online" : "Offline")
        </span>
    </div>
    <div class="banko-body">
        @if (banko.AtananPersonel != null)
        {
            <p>@banko.AtananPersonel.AdSoyad</p>
        }
    </div>
</div>
```

### Filtreleme

```csharp
// Sadece online bankoları göster
var onlineBankolar = bankolar.Where(b => b.IsConnected).ToList();

// Offline bankoları göster
var offlineBankolar = bankolar.Where(b => !b.IsConnected).ToList();
```

---

## 🔍 Bağlantı Durumu Takibi

### Otomatik Disconnection

```csharp
// SiramatikHub.cs
public override async Task OnDisconnectedAsync(Exception? exception)
{
    var connectionId = Context.ConnectionId;
    
    // Banko bağlantısını kaldır
    var bankoId = await _connectionService.GetBankoIdByConnectionIdAsync(connectionId);
    if (bankoId.HasValue)
    {
        await _connectionService.UnregisterBankoConnectionAsync(bankoId.Value, connectionId);
        _logger.LogInformation($"🔴 Banko bağlantısı koptu: Banko#{bankoId.Value}");
    }
    
    await base.OnDisconnectedAsync(exception);
}
```

### Ping/Pong Mekanizması

```javascript
// Client - Her 30 saniyede bir ping gönder
setInterval(async () => {
    try {
        await manager.invoke("Ping");
    } catch (error) {
        console.error('Ping hatası:', error);
        // Yeniden bağlan
        await manager.invoke("JoinBankoGroup", bankoId);
    }
}, 30000);
```

---

## 📈 Kullanım Senaryoları

### 1. Sıra Çağırma Sistemi

```csharp
// Sıra çağrıldığında
public async Task CagirSira(int bankoId, int siraNo)
{
    // TV'ye bildir
    await _hubContext.Clients.Group($"TV_{tvId}")
        .SendAsync("ReceiveSiraUpdate", new { bankoId, siraNo });
    
    // Banko'ya bildir
    var isOnline = await _connectionService.IsBankoConnectedAsync(bankoId);
    if (isOnline)
    {
        await _hubContext.Clients.Group($"Banko_{bankoId}")
            .SendAsync("ReceiveSiraNotification", new { siraNo });
    }
}
```

### 2. Banko Müsaitlik Durumu

```csharp
// Banko müsait olduğunda
public async Task SetBankoMusait(int bankoId, bool musaitMi)
{
    // Database güncelle
    await UpdateBankoMusaitlik(bankoId, musaitMi);
    
    // Tüm TV'lere bildir
    await _hubContext.Clients.All
        .SendAsync("ReceiveBankoUpdate", new {
            bankoId,
            musaitMi,
            isConnected = await _connectionService.IsBankoConnectedAsync(bankoId)
        });
}
```

### 3. Sistem Duyuruları

```csharp
// Tüm Bankolara duyuru
public async Task SendSystemAnnouncement(string message)
{
    await _hubContext.Clients.All
        .SendAsync("ReceiveAnnouncement", new {
            message,
            type = "system",
            timestamp = DateTime.Now
        });
}
```

---

## 🎯 Avantajlar

### TV Sistemi İle Karşılaştırma

| Özellik | TV | Banko |
|---------|-----|-------|
| **Bağlantı Takibi** | ✅ | ✅ |
| **Grup Yönetimi** | ✅ TV_{id} | ✅ Banko_{id} |
| **Sıra Bildirimi** | ✅ Görüntüleme | ✅ İşlem yapma |
| **Duyuru Alma** | ✅ | ✅ |
| **Müsaitlik Durumu** | ❌ | ✅ |
| **Personel Bilgisi** | ❌ | ✅ |

### Kullanım Alanları

1. **Sıra Yönetimi**
   - Banko'ya sıra bildirimi
   - Otomatik sıra dağıtımı
   - Müsait banko bulma

2. **Personel Takibi**
   - Hangi personel hangi bankoda
   - Online/Offline durumu
   - Çalışma saatleri

3. **Sistem Bildirimleri**
   - Acil duyurular
   - Sistem bakımı
   - Güncelleme bildirimleri

4. **Raporlama**
   - Online banko sayısı
   - Bağlantı süresi
   - Kullanım istatistikleri

---

## 🔧 Sorun Giderme

### Banko "Offline" Görünüyor

**Çözüm:**
1. `JoinBankoGroup` çağrıldığından emin olun
2. `HubBankoConnections` tablosunu kontrol edin
3. Console loglarını inceleyin

```sql
-- Database kontrolü
SELECT * FROM HubBankoConnections WHERE BankoId = 1;
```

### Mesajlar Banko'ya Ulaşmıyor

**Çözüm:**
1. Banko'nun online olduğunu kontrol edin
2. Grup ismini kontrol edin: `Banko_{bankoId}`
3. Event listener'ın eklendiğinden emin olun

```javascript
// Event listener kontrolü
manager.on("ReceiveSiraNotification", (data) => {
    console.log('Event alındı:', data);
});
```

---

## 📝 Migration

```bash
# Migration oluştur
dotnet ef migrations add AddHubBankoConnection --project SGKPortalApp.DataAccessLayer --startup-project SGKPortalApp.PresentationLayer

# Migration uygula
dotnet ef database update --project SGKPortalApp.DataAccessLayer --startup-project SGKPortalApp.PresentationLayer
```

---

## 🎉 Sonuç

Artık hem TV'lerin hem de Bankoların online/offline durumunu gerçek zamanlı takip edebilirsiniz!

- ✅ **TV'ler** - Sıra görüntüleme ekranları
- ✅ **Bankolar** - Personel çalışma istasyonları
- ✅ **Gerçek zamanlı** - SignalR ile anlık iletişim
- ✅ **Merkezi yönetim** - Tek bir Hub üzerinden

---

**Son Güncelleme:** 24 Kasım 2025  
**Versiyon:** 1.0.0
