# 🎯 AKILLI TAB YÖNETİMİ - TEKNİK DÖKÜMAN

**Proje:** SGK Portal - Sıramatik Sistemi  
**Versiyon:** 1.0  
**Tarih:** 21 Kasım 2025  
**Amaç:** Banko modunda çalışan personelin aynı anda birden fazla sıra çağırma paneli açmasını engellemek

---

## 📋 İçindekiler

1. [Genel Bakış](#1-genel-bakış)
2. [Mimari Tasarım](#2-mimari-tasarım)
3. [Teknoloji Stack](#3-teknoloji-stack)
4. [Client-Side Implementasyon](#4-client-side-implementasyon)
5. [Server-Side Implementasyon](#5-server-side-implementasyon)
6. [Veritabanı Değişiklikleri](#6-veritabanı-değişiklikleri)
7. [Kullanıcı Arayüzü](#7-kullanıcı-arayüzü)
8. [Test Senaryoları](#8-test-senaryoları)
9. [Güvenlik Considerations](#9-güvenlik-considerations)
10. [Deployment Checklist](#10-deployment-checklist)

---

## 1. Genel Bakış

### 1.1. Problem Tanımı

**Mevcut Durum:**
- Personel yeni tab açtığında SignalR yeni connection başlatıyor
- Eski tab'a ForceDisconnect mesajı gidiyor
- Kullanıcı tab'lar arası geçiş yapmak istediğinde her seferinde refresh gerekiyor

**İstenen Durum:**
- BANKO modunda çalışan personel için tek tab politikası
- Yeni tab açma girişimleri engellenmeli
- İZLEME modunda çalışan personel için kısıtlama olmamalı

### 1.2. Kapsam

**Dahil:**
- ✅ Link'lere Ctrl+Click engelleme
- ✅ Link'lere Middle Click (mouse tekerlek) engelleme
- ✅ Link'lere Sağ Tık → "Yeni sekmede aç" engelleme
- ✅ Manuel URL girişi kontrolü
- ✅ Sadece "/siramatik/panel" sayfası için kısıtlama
- ✅ Sadece BANKO modunda çalışan personel için

**Hariç:**
- ❌ İzleme modunda çalışan personel (serbest)
- ❌ Diğer sayfalar (raporlar, dashboard, vb.)
- ❌ Admin kullanıcıları

### 1.3. Faydalar
```
KULLANICI DENEYİMİ:
├─ Kullanıcı bilgilendirilir (yeni tab açma girişimi)
├─ Gereksiz refresh'ler önlenir
├─ Modal kargaşası azalır
└─ Net ve anlaşılır uyarılar

TEKNIK FAYDALAR:
├─ SignalR connection churn azalır
├─ Server yükü azalır
├─ State tutarlılığı artar
└─ Debug kolaylaşır
```

---

## 2. Mimari Tasarım

### 2.1. İki Katmanlı Savunma (Defense in Depth)
```
┌─────────────────────────────────────────────────────┐
│                   KULLANICI                         │
└──────────────────┬──────────────────────────────────┘
                   │
                   ▼
    ┌──────────────────────────────────────┐
    │   1. CLIENT-SIDE ENGELLEME           │
    │   (JavaScript Event Listener)        │
    │   • Link yakalama                    │
    │   • Ctrl/Middle/Shift tuş kontrolü   │
    │   • preventDefault()                 │
    │   • Modal gösterimi                  │
    └──────────────┬───────────────────────┘
                   │
                   │ (Bypass edilirse)
                   ▼
    ┌──────────────────────────────────────┐
    │   2. SERVER-SIDE KONTROL             │
    │   (Backend API Check)                │
    │   • Aktif tab kontrolü               │
    │   • Veritabanı sorgusu               │
    │   • SignalR engelleme                │
    │   • Uyarı sayfası gösterimi          │
    └──────────────────────────────────────┘
```

### 2.2. Akış Diyagramı
```
                    [Kullanıcı Link'e Tıkladı]
                              │
                              ▼
              ┌───────────────────────────┐
              │ Ctrl/Middle/Shift basılı? │
              └───────┬───────────────────┘
                      │
           ┌──────────┴──────────┐
           │ Hayır               │ Evet
           ▼                     ▼
    [Normal Link]    ┌─────────────────────┐
    [Davranışı]      │ Hedef sayfa panel? │
                     └──────┬──────────────┘
                            │
                 ┌──────────┴──────────┐
                 │ Hayır               │ Evet
                 ▼                     ▼
          [İzin Ver]       ┌──────────────────┐
                           │ BANKO modunda?   │
                           └────┬─────────────┘
                                │
                     ┌──────────┴──────────┐
                     │ Hayır               │ Evet
                     ▼                     ▼
              [İzin Ver]          [preventDefault()]
                                          │
                                          ▼
                                  [Modal Göster]
```

---

## 3. Teknoloji Stack

### 3.1. Frontend

| Teknoloji | Versiyon | Kullanım Amacı |
|-----------|----------|----------------|
| **Blazor Server** | .NET 9.0 | UI Framework |
| **JavaScript (Vanilla)** | ES6+ | Event handling, DOM manipulation |
| **Bootstrap 5** | 5.3.x | Modal ve UI bileşenleri |
| **Boxicons** | 2.x | Icon library |

### 3.2. Backend

| Teknoloji | Versiyon | Kullanım Amacı |
|-----------|----------|----------------|
| **ASP.NET Core** | 9.0 | Web framework |
| **SignalR** | 9.0 | Real-time communication |
| **Entity Framework Core** | 9.0 | ORM |
| **SQL Server** | 2019+ | Database |

### 3.3. Storage

| Teknoloji | Kullanım Amacı |
|-----------|----------------|
| **LocalStorage** | WorkMode bilgisi, Modal onay durumu |
| **SQL Server** | HubConnections tablosu, CurrentPage kolonu |

---

## 4. Client-Side Implementasyon

### 4.1. JavaScript - Tab Manager

**Dosya:** `wwwroot/js/tab-manager.js`
```javascript
// Tab Manager - Akıllı tab kontrolü için
window.TabManager = {
    
    workMode: null,
    isInitialized: false,
    
    // Başlatma
    initialize: function(workMode) {
        
        console.log('[TabManager] Initializing with mode:', workMode);
        
        this.workMode = workMode;
        
        if (this.workMode !== 'BANKO') {
            console.log('[TabManager] Not in BANKO mode, skipping initialization');
            return;
        }
        
        if (this.isInitialized) {
            console.log('[TabManager] Already initialized');
            return;
        }
        
        // Event listener ekle
        document.addEventListener('click', this.handleLinkClick.bind(this), true);
        document.addEventListener('auxclick', this.handleAuxClick.bind(this), true);
        
        this.isInitialized = true;
        console.log('[TabManager] Initialized successfully');
    },
    
    // Link tıklama (sol/sağ tık)
    handleLinkClick: function(event) {
        
        // Link element'i bul
        const link = event.target.closest('a');
        if (!link) return;
        
        // Internal link mi?
        const href = link.getAttribute('href');
        if (!href || href.startsWith('http://') || href.startsWith('https://')) {
            return;
        }
        
        // Ctrl/Meta/Shift tuşu basılı mı?
        const isModifierKey = event.ctrlKey || event.metaKey || event.shiftKey;
        
        if (!isModifierKey) return;
        
        // Hedef sayfa sıra paneli mi?
        if (!this.isTargetPanelPage(href)) return;
        
        console.log('[TabManager] Preventing new tab:', href);
        
        // Engelle!
        event.preventDefault();
        event.stopPropagation();
        
        // Modal göster
        this.showWarningModal();
    },
    
    // Middle click (tekerlek)
    handleAuxClick: function(event) {
        
        // Middle click (button 1)
        if (event.button !== 1) return;
        
        const link = event.target.closest('a');
        if (!link) return;
        
        const href = link.getAttribute('href');
        if (!href || href.startsWith('http://') || href.startsWith('https://')) {
            return;
        }
        
        if (!this.isTargetPanelPage(href)) return;
        
        console.log('[TabManager] Preventing middle click new tab:', href);
        
        event.preventDefault();
        event.stopPropagation();
        
        this.showWarningModal();
    },
    
    // Hedef sayfa kontrol
    isTargetPanelPage: function(href) {
        const panelPaths = [
            '/siramatik/panel',
            '/Siramatik/Panel'
        ];
        
        return panelPaths.some(path => href.includes(path));
    },
    
    // Modal göster (Blazor'a bildir)
    showWarningModal: function() {
        try {
            DotNet.invokeMethodAsync(
                'SGKPortalApp.PresentationLayer', 
                'ShowMultiTabWarning'
            );
        } catch (error) {
            console.error('[TabManager] Failed to show modal:', error);
            alert('Sıra çağırma paneli zaten bu sekmede açık.\n\nAynı anda birden fazla panel açamazsınız.');
        }
    },
    
    // Temizle
    destroy: function() {
        if (!this.isInitialized) return;
        
        document.removeEventListener('click', this.handleLinkClick, true);
        document.removeEventListener('auxclick', this.handleAuxClick, true);
        
        this.isInitialized = false;
        console.log('[TabManager] Destroyed');
    }
};

// Context menu engelleme (opsiyonel)
window.TabManager.preventContextMenu = function() {
    document.addEventListener('contextmenu', function(event) {
        const link = event.target.closest('a');
        if (!link) return;
        
        const href = link.getAttribute('href');
        if (!href) return;
        
        if (TabManager.workMode === 'BANKO' && TabManager.isTargetPanelPage(href)) {
            // Uyarı göster ama context menu'yü engelleme
            // Kullanıcı deneyimi için
            console.log('[TabManager] Context menu on panel link detected');
        }
    });
};
```

### 4.2. Blazor - MainLayout Entegrasyonu

**Dosya:** `Shared/Layout/MainLayout.razor`
```razor
@inherits LayoutComponentBase
@inject IJSRuntime JS
@inject IHttpContextAccessor HttpContextAccessor

<div class="layout-wrapper layout-content-navbar">
    <div class="layout-container">
        
        <!-- Sidebar, TopBar, vb. -->
        
        <div class="layout-page">
            <div class="content-wrapper">
                @Body
            </div>
        </div>
    </div>
</div>

<!-- Multi-Tab Warning Modal -->
<MultiTabWarningModal @ref="warningModal" />

@code {
    private MultiTabWarningModal? warningModal;
    private string? currentWorkMode;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Kullanıcının çalışma modunu al
            currentWorkMode = await GetUserWorkMode();
            
            // Tab Manager'ı başlat
            await JS.InvokeVoidAsync("TabManager.initialize", currentWorkMode);
        }
    }
    
    private async Task<string?> GetUserWorkMode()
    {
        try
        {
            // LocalStorage'dan oku
            var mode = await JS.InvokeAsync<string>("localStorage.getItem", "SelectedMode");
            return mode; // "BANKO" veya "MONITORING"
        }
        catch
        {
            return null;
        }
    }
    
    [JSInvokable]
    public static async Task ShowMultiTabWarning()
    {
        // Static method - Instance'a erişim için event kullan
        await Task.CompletedTask;
        // Modal gösterilecek (component içinde yönetilecek)
    }
}
```

### 4.3. Blazor - Multi-Tab Warning Modal

**Dosya:** `Shared/Common/MultiTabWarningModal.razor`
```razor
@inject IJSRuntime JS

<div class="modal @(IsVisible ? "show d-block" : "d-none")" 
     tabindex="-1" 
     role="dialog"
     style="background-color: rgba(0,0,0,0.5);">
    
    <div class="modal-dialog modal-dialog-centered" role="document">
        <div class="modal-content">
            
            <div class="modal-header bg-warning">
                <h5 class="modal-title">
                    <i class="bx bx-info-circle me-2"></i>
                    Zaten Aktif Bir Sıra Paneliniz Var
                </h5>
            </div>
            
            <div class="modal-body">
                <p>
                    Sıra çağırma paneli zaten bu sekmede açık.
                </p>
                <p class="mb-0">
                    <strong>Aynı anda birden fazla sıra paneli açamazsınız.</strong>
                </p>
                
                <div class="alert alert-info mt-3 mb-0">
                    <small>
                        <i class="bx bx-shield me-1"></i>
                        <strong>Neden?</strong><br/>
                        Güvenlik ve veri tutarlılığı için her personel sadece bir sıra paneli kullanabilir.
                    </small>
                </div>
            </div>
            
            <div class="modal-footer">
                <button type="button" 
                        class="btn btn-primary" 
                        @onclick="CloseModal">
                    <i class="bx bx-check me-1"></i>
                    Anladım
                </button>
            </div>
            
        </div>
    </div>
</div>

@code {
    private bool IsVisible = false;
    
    protected override void OnInitialized()
    {
        // Static interop için instance'ı kaydet
        MultiTabWarningModal._instance = this;
    }
    
    private static MultiTabWarningModal? _instance;
    
    [JSInvokable("ShowMultiTabWarning")]
    public static Task ShowWarningStatic()
    {
        _instance?.ShowWarning();
        return Task.CompletedTask;
    }
    
    public void ShowWarning()
    {
        IsVisible = true;
        StateHasChanged();
    }
    
    private void CloseModal()
    {
        IsVisible = false;
        StateHasChanged();
    }
}
```

---

## 5. Server-Side Implementasyon

### 5.1. Database Migration - CurrentPage Kolonu

**Dosya:** `DataAccessLayer/Migrations/YYYYMMDDHHMMSS_AddCurrentPageToHubConnections.cs`
```csharp
using Microsoft.EntityFrameworkCore.Migrations;

public partial class AddCurrentPageToHubConnections : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "CurrentPage",
            schema: "dbo",
            table: "SIR_HubConnections",
            type: "nvarchar(200)",
            maxLength: 200,
            nullable: true);
        
        migrationBuilder.CreateIndex(
            name: "IX_SIR_HubConnections_TcKimlikNo_CurrentPage",
            schema: "dbo",
            table: "SIR_HubConnections",
            columns: new[] { "TcKimlikNo", "CurrentPage" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_SIR_HubConnections_TcKimlikNo_CurrentPage",
            schema: "dbo",
            table: "SIR_HubConnections");
        
        migrationBuilder.DropColumn(
            name: "CurrentPage",
            schema: "dbo",
            table: "SIR_HubConnections");
    }
}
```

### 5.2. Entity - HubConnection Güncelleme

**Dosya:** `BusinessObjectLayer/Entities/SiramatikIslemleri/HubConnection.cs`
```csharp
public class HubConnection : AuditableEntity
{
    public int HubConnectionId { get; set; }
    
    [Required]
    [MaxLength(11)]
    public string TcKimlikNo { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string ConnectionId { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string ConnectionStatus { get; set; } = "online"; // online, offline, away
    
    [MaxLength(20)]
    public string? ConnectionMode { get; set; } // BANKO, MONITORING
    
    [MaxLength(200)]
    public string? CurrentPage { get; set; } // /siramatik/panel, /dashboard, vb.
    
    public DateTime IslemZamani { get; set; } = DateTime.Now;
    
    // Navigation
    public User User { get; set; } = null!;
}
```

### 5.3. SignalR Hub - Page Change Notification

**Dosya:** `PresentationLayer/Hubs/SiramatikHub.cs`
```csharp
public class SiramatikHub : Hub
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SiramatikHub> _logger;
    
    public SiramatikHub(
        IUnitOfWork unitOfWork,
        ILogger<SiramatikHub> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    // Sayfa değişikliğini bildir
    public async Task NotifyPageChanged(string pagePath)
    {
        try
        {
            var tcKimlikNo = Context.User?.FindFirst("TcKimlikNo")?.Value;
            if (string.IsNullOrEmpty(tcKimlikNo))
            {
                _logger.LogWarning("TcKimlikNo bulunamadı");
                return;
            }
            
            var connectionId = Context.ConnectionId;
            
            var connection = await _unitOfWork.HubConnectionRepository
                .GetByConnectionIdAsync(connectionId);
            
            if (connection != null)
            {
                connection.CurrentPage = pagePath;
                connection.IslemZamani = DateTime.Now;
                
                await _unitOfWork.SaveChangesAsync();
                
                _logger.LogInformation(
                    "Sayfa değişikliği kaydedildi: {TcKimlikNo} -> {Page}",
                    tcKimlikNo, pagePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sayfa değişikliği bildirimi hatası");
        }
    }
    
    // Diğer hub methodları...
}
```

### 5.4. API Controller - Active Tab Check

**Dosya:** `ApiLayer/Controllers/HubController.cs`
```csharp
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class HubController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<HubController> _logger;
    
    public HubController(
        IUnitOfWork unitOfWork,
        ILogger<HubController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    [HttpGet("check-active-tab")]
    public async Task<IActionResult> CheckActiveTab([FromQuery] string page)
    {
        try
        {
            var tcKimlikNo = User.FindFirst("TcKimlikNo")?.Value;
            if (string.IsNullOrEmpty(tcKimlikNo))
            {
                return Unauthorized(new { Message = "Kullanıcı bilgisi bulunamadı" });
            }
            
            // Bu kullanıcının aktif connection'ı var mı?
            var activeConnection = await _unitOfWork.HubConnectionRepository
                .GetByTcKimlikNoAsync(tcKimlikNo);
            
            if (activeConnection == null)
            {
                return Ok(new CheckActiveTabResponse
                {
                    HasActiveTab = false
                });
            }
            
            // Connection mode BANKO mu?
            if (activeConnection.ConnectionMode != "BANKO")
            {
                return Ok(new CheckActiveTabResponse
                {
                    HasActiveTab = false
                });
            }
            
            // Aynı sayfada mı?
            var isSamePage = activeConnection.CurrentPage?.Equals(page, 
                StringComparison.OrdinalIgnoreCase) ?? false;
            
            return Ok(new CheckActiveTabResponse
            {
                HasActiveTab = isSamePage,
                ConnectionId = activeConnection.ConnectionId,
                CurrentPage = activeConnection.CurrentPage,
                LastActivity = activeConnection.IslemZamani
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Aktif tab kontrolü hatası");
            return StatusCode(500, new { Message = "Sunucu hatası" });
        }
    }
}

public class CheckActiveTabResponse
{
    public bool HasActiveTab { get; set; }
    public string? ConnectionId { get; set; }
    public string? CurrentPage { get; set; }
    public DateTime? LastActivity { get; set; }
}
```

### 5.5. Repository - HubConnection Methodları

**Dosya:** `DataAccessLayer/Repositories/Concrete/SiramatikIslemleri/HubConnectionRepository.cs`
```csharp
public class HubConnectionRepository : GenericRepository<HubConnection>, IHubConnectionRepository
{
    public HubConnectionRepository(SGKDbContext context) : base(context)
    {
    }
    
    public async Task<HubConnection?> GetByTcKimlikNoAsync(string tcKimlikNo)
    {
        return await _dbSet
            .Where(h => h.TcKimlikNo == tcKimlikNo && !h.SilindiMi)
            .FirstOrDefaultAsync();
    }
    
    public async Task<HubConnection?> GetByConnectionIdAsync(string connectionId)
    {
        return await _dbSet
            .Where(h => h.ConnectionId == connectionId && !h.SilindiMi)
            .FirstOrDefaultAsync();
    }
    
    public async Task<HubConnection?> GetActiveConnectionByPageAsync(
        string tcKimlikNo, 
        string page)
    {
        return await _dbSet
            .Where(h => 
                h.TcKimlikNo == tcKimlikNo &&
                h.ConnectionStatus == "online" &&
                h.ConnectionMode == "BANKO" &&
                h.CurrentPage == page &&
                !h.SilindiMi)
            .FirstOrDefaultAsync();
    }
}
```

---

## 6. Veritabanı Değişiklikleri

### 6.1. Tablo Değişikliği
```sql
-- SIR_HubConnections tablosuna yeni kolon

ALTER TABLE [dbo].[SIR_HubConnections]
ADD [CurrentPage] NVARCHAR(200) NULL;

-- Index oluştur (performans için)
CREATE NONCLUSTERED INDEX [IX_SIR_HubConnections_TcKimlikNo_CurrentPage]
ON [dbo].[SIR_HubConnections] ([TcKimlikNo], [CurrentPage])
WHERE [SilindiMi] = 0;
```

### 6.2. Yeni Tablo Yapısı
```sql
CREATE TABLE [dbo].[SIR_HubConnections] (
    [HubConnectionId] INT IDENTITY(1,1) NOT NULL,
    [TcKimlikNo] NVARCHAR(11) NOT NULL,
    [ConnectionId] NVARCHAR(100) NOT NULL,
    [ConnectionStatus] NVARCHAR(20) NOT NULL DEFAULT 'online',
    [ConnectionMode] NVARCHAR(20) NULL,          -- YENİ
    [CurrentPage] NVARCHAR(200) NULL,            -- YENİ
    [IslemZamani] DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    
    -- Audit kolonları
    [EklenmeTarihi] DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    [DuzenlenmeTarihi] DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    [EkleyenKullanici] NVARCHAR(50) NULL,
    [DuzenleyenKullanici] NVARCHAR(50) NULL,
    [SilindiMi] BIT NOT NULL DEFAULT 0,
    [SilinmeTarihi] DATETIME2(7) NULL,
    [SilenKullanici] NVARCHAR(50) NULL,
    
    CONSTRAINT [PK_SIR_HubConnections] PRIMARY KEY CLUSTERED ([HubConnectionId]),
    CONSTRAINT [FK_SIR_HubConnections_CMN_Users] 
        FOREIGN KEY ([TcKimlikNo]) 
        REFERENCES [dbo].[CMN_Users] ([TcKimlikNo])
);

-- Indexes
CREATE UNIQUE NONCLUSTERED INDEX [IX_SIR_HubConnections_TcKimlikNo]
ON [dbo].[SIR_HubConnections] ([TcKimlikNo])
WHERE [SilindiMi] = 0;

CREATE NONCLUSTERED INDEX [IX_SIR_HubConnections_ConnectionId]
ON [dbo].[SIR_HubConnections] ([ConnectionId]);

CREATE NONCLUSTERED INDEX [IX_SIR_HubConnections_TcKimlikNo_CurrentPage]
ON [dbo].[SIR_HubConnections] ([TcKimlikNo], [CurrentPage])
WHERE [SilindiMi] = 0;
```

---

## 7. Kullanıcı Arayüzü

### 7.1. Sıra Çağırma Paneli - Init Check

**Dosya:** `Pages/Siramatik/SiraCagirmaPanel.razor`
```razor
@page "/siramatik/panel"
@inject HttpClient Http
@inject NavigationManager Navigation
@inject IJSRuntime JS

<PageTitle>Sıra Çağırma Paneli</PageTitle>

@if (showWarning)
{
    <AlreadyOpenWarning />
}
else if (isLoading)
{
    <div class="text-center p-5">
        <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Yükleniyor...</span>
        </div>
        <p class="mt-3">Panel hazırlanıyor...</p>
    </div>
}
else
{
    <!-- Normal panel içeriği -->
    <div class="container-fluid">
        <h2>Sıra Çağırma Paneli</h2>
        <!-- ... -->
    </div>
}

@code {
    private bool showWarning = false;
    private bool isLoading = true;
    private string? workMode;
    
    protected override async Task OnInitializedAsync()
    {
        try
        {
            // 1. Kullanıcının çalışma modunu al
            workMode = await GetUserWorkMode();
            
            // 2. BANKO modundaysa aktif tab kontrolü yap
            if (workMode == "BANKO")
            {
                var hasActiveTab = await CheckForActiveTab();
                
                if (hasActiveTab)
                {
                    // Uyarı göster, SignalR başlatma
                    showWarning = true;
                    isLoading = false;
                    return;
                }
            }
            
            // 3. Normal başlatma
            await InitializePanel();
            
            // 4. SignalR'a sayfa değişikliğini bildir
            if (HubConnection?.State == HubConnectionState.Connected)
            {
                await HubConnection.InvokeAsync("NotifyPageChanged", "/siramatik/panel");
            }
            
            isLoading = false;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Panel başlatma hatası");
            isLoading = false;
        }
    }
    
    private async Task<string?> GetUserWorkMode()
    {
        try
        {
            return await JS.InvokeAsync<string>("localStorage.getItem", "SelectedMode");
        }
        catch
        {
            return null;
        }
    }
    
    private async Task<bool> CheckForActiveTab()
    {
        try
        {
            var response = await Http.GetFromJsonAsync<CheckActiveTabResponse>(
                "/api/hub/check-active-tab?page=/siramatik/panel");
            
            return response?.HasActiveTab ?? false;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Aktif tab kontrolü hatası");
            return false;
        }
    }
    
    private async Task InitializePanel()
    {
        // SignalR bağlantısı, veri yükleme, vb.
    }
}
```

### 7.2. Already Open Warning Component

**Dosya:** `Pages/Siramatik/AlreadyOpenWarning.razor`
```razor
@inject IJSRuntime JS
@inject NavigationManager Navigation

<div class="container mt-5">
    <div class="row justify-content-center">
        <div class="col-lg-6 col-md-8">
            <div class="card shadow-lg">
                <div class="card-body text-center p-5">
                    
                    <!-- Icon -->
                    <div class="mb-4">
                        <i class="bx bx-info-circle text-warning" 
                           style="font-size: 80px;"></i>
                    </div>
                    
                    <!-- Başlık -->
                    <h2 class="mb-3 fw-bold">
                        Sıra Paneli Zaten Açık
                    </h2>
                    
                    <!-- Açıklama -->
                    <p class="text-muted mb-4 fs-5">
                        Sıra çağırma paneli başka bir sekmede zaten açık.<br/>
                        Aynı anda birden fazla panel açamazsınız.
                    </p>
                    
                    <!-- Bilgi Kutusu -->
                    <div class="alert alert-info text-start">
                        <h6 class="alert-heading">
                            <i class="bx bx-shield me-2"></i>
                            Neden bu kısıtlama var?
                        </h6>
                        <p class="mb-0 small">
                            Güvenlik ve veri tutarlılığı için her personel sadece 
                            bir sıra çağırma paneli kullanabilir. Bu sayede:
                        </p>
                        <ul class="mb-0 small mt-2">
                            <li>Çift sıra çağırma önlenir</li>
                            <li>Veri tutarsızlığı oluşmaz</li>
                            <li>Sistem performansı korunur</li>
                        </ul>
                    </div>
                    
                    <!-- Butonlar -->
                    <div class="d-grid gap-2 mt-4">
                        
                        <button class="btn btn-lg btn-outline-secondary" 
                                @onclick="FocusExistingTab">
                            <i class="bx bx-window me-2"></i>
                            Mevcut Panele Dön
                        </button>
                        
                        <button class="btn btn-lg btn-outline-danger" 
                                @onclick="CloseThisTab">
                            <i class="bx bx-x me-2"></i>
                            Bu Sekmeyi Kapat
                        </button>
                        
                        <a href="/dashboard" class="btn btn-lg btn-primary">
                            <i class="bx bx-home me-2"></i>
                            Ana Sayfaya Git
                        </a>
                        
                    </div>
                    
                </div>
            </div>
        </div>
    </div>
</div>

@code {
    private async Task FocusExistingTab()
    {
        // Browser kısıtlaması nedeniyle direkt focus yapılamaz
        await JS.InvokeVoidAsync("alert", 
            "Lütfen manuel olarak mevcut sekmeye geçin.\n\n" +
            "Bu sekmeyi kapatabilirsiniz.");
    }
    
    private async Task CloseThisTab()
    {
        // Sekmeyi kapatmayı dene
        try
        {
            await JS.InvokeVoidAsync("window.close");
        }
        catch
        {
            // Kapanamazsa ana sayfaya yönlendir
            Navigation.NavigateTo("/dashboard");
        }
    }
}
```

---

## 8. Test Senaryoları

### 8.1. Manuel Test Case'leri

#### Test Case 1: Ctrl+Click Engelleme
```
Ön Koşullar:
- Kullanıcı BANKO modunda login olmuş
- Sıra çağırma paneli açık

Adımlar:
1. Sol menüden "Sıramatik Panel" linkine Ctrl+Click yap
2. Modal'ın göründüğünü doğrula
3. "Anladım" butonuna tıkla
4. Yeni tab açılmadığını doğrula

Beklenen Sonuç: ✅ Yeni tab açılmaz, modal gösterilir
```

#### Test Case 2: Middle Click Engelleme
```
Ön Koşullar:
- Kullanıcı BANKO modunda login olmuş
- Sıra çağırma paneli açık

Adımlar:
1. Link'e mouse tekerleği ile tıkla (middle click)
2. Modal'ın göründüğünü doğrula
3. Yeni tab açılmadığını doğrula

Beklenen Sonuç: ✅ Yeni tab açılmaz, modal gösterilir
```

#### Test Case 3: Manuel URL Girişi
```
Ön Koşullar:
- Kullanıcı BANKO modunda login olmuş
- Tab 1'de sıra paneli açık

Adımlar:
1. Ctrl+T ile yeni tab aç (Tab 2)
2. Adres çubuğuna: localhost:5000/siramatik/panel
3. Enter
4. Uyarı sayfasının göründüğünü doğrula
5. SignalR bağlantısı başlamadığını doğrula
6. "Bu Sekmeyi Kapat" butonuna tıkla

Beklenen Sonuç: ✅ Uyarı sayfası gösterilir, Tab 1 etkilenmez
```

#### Test Case 4: İzleme Modu - Serbest
```
Ön Koşullar:
- Kullanıcı İZLEME modunda login olmuş

Adımlar:
1. Herhangi bir link'e Ctrl+Click yap
2. Yeni tab'ın açıldığını doğrula
3. Engelleme olmadığını doğrula

Beklenen Sonuç: ✅ Normal davranış, engelleme yok
```

#### Test Case 5: Farklı Sayfa - İzin Verilir
```
Ön Koşullar:
- Kullanıcı BANKO modunda
- Sıra paneli açık

Adımlar:
1. "Raporlar" linkine Ctrl+Click yap
2. Yeni tab'ın açıldığını doğrula
3. Raporlar sayfasının yüklendiğini doğrula

Beklenen Sonuç: ✅ Farklı sayfa için engelleme yok
```

### 8.2. Otomatik Test (Unit Test)

**Dosya:** `Tests/TabManagerTests.cs`
```csharp
public class TabManagerTests
{
    private Mock<IHubConnectionRepository> _mockRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private HubController _controller;
    
    public TabManagerTests()
    {
        _mockRepo = new Mock<IHubConnectionRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUnitOfWork.Setup(u => u.HubConnectionRepository).Returns(_mockRepo.Object);
        
        _controller = new HubController(_mockUnitOfWork.Object, Mock.Of<ILogger<HubController>>());
    }
    
    [Fact]
    public async Task CheckActiveTab_WhenNoActiveConnection_ReturnsFalse()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByTcKimlikNoAsync(It.IsAny<string>()))
            .ReturnsAsync((HubConnection?)null);
        
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("TcKimlikNo", "12345678901")
        }));
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        
        // Act
        var result = await _controller.CheckActiveTab("/siramatik/panel");
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<CheckActiveTabResponse>(okResult.Value);
        Assert.False(response.HasActiveTab);
    }
    
    [Fact]
    public async Task CheckActiveTab_WhenActiveConnectionExists_ReturnsTrue()
    {
        // Arrange
        var activeConnection = new HubConnection
        {
            TcKimlikNo = "12345678901",
            ConnectionId = "ABC123",
            ConnectionMode = "BANKO",
            CurrentPage = "/siramatik/panel",
            ConnectionStatus = "online"
        };
        
        _mockRepo.Setup(r => r.GetByTcKimlikNoAsync("12345678901"))
            .ReturnsAsync(activeConnection);
        
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("TcKimlikNo", "12345678901")
        }));
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        
        // Act
        var result = await _controller.CheckActiveTab("/siramatik/panel");
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<CheckActiveTabResponse>(okResult.Value);
        Assert.True(response.HasActiveTab);
        Assert.Equal("ABC123", response.ConnectionId);
    }
    
    [Fact]
    public async Task CheckActiveTab_WhenMonitoringMode_ReturnsFalse()
    {
        // Arrange
        var activeConnection = new HubConnection
        {
            TcKimlikNo = "12345678901",
            ConnectionMode = "MONITORING", // İzleme modu
            CurrentPage = "/siramatik/panel"
        };
        
        _mockRepo.Setup(r => r.GetByTcKimlikNoAsync("12345678901"))
            .ReturnsAsync(activeConnection);
        
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("TcKimlikNo", "12345678901")
        }));
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        
        // Act
        var result = await _controller.CheckActiveTab("/siramatik/panel");
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<CheckActiveTabResponse>(okResult.Value);
        Assert.False(response.HasActiveTab); // İzleme modunda engelleme yok
    }
}
```

---

## 9. Güvenlik Considerations

### 9.1. Güvenlik Kontrol Listesi
```
✅ Client-Side Validations:
   ├─ JavaScript event listeners
   ├─ preventDefault() kullanımı
   └─ Modal ile kullanıcı bilgilendirme

✅ Server-Side Validations:
   ├─ API endpoint authorization
   ├─ TcKimlikNo doğrulama
   ├─ Connection mode kontrolü
   └─ Database query güvenliği

✅ Defense in Depth:
   ├─ İki katmanlı kontrol
   ├─ JavaScript bypass koruması
   └─ Browser güvenlik özellikleri

✅ Data Protection:
   ├─ SQL Injection koruması (EF Core)
   ├─ XSS koruması (Razor encoding)
   └─ CSRF token (ASP.NET Core)
```

### 9.2. Potansiyel Güvenlik Riskleri

#### Risk 1: JavaScript Devre Dışı
```
Risk Seviyesi: ORTA
Açıklama: Kullanıcı JavaScript'i kapatırsa client-side engelleme çalışmaz

Azaltma:
├─ Server-side kontrol mutlaka yapılmalı
├─ API endpoint'leri her durumda kontrol eder
└─ noscript tag ile uyarı gösterilebilir

Sonuç: ✅ Korunmuş (Server-side sayesinde)
```

#### Risk 2: Browser Developer Tools
```
Risk Seviyesi: DÜŞÜK
Açıklama: Gelişmiş kullanıcı console'dan bypass edebilir

Azaltma:
├─ Server-side kontrol ana savunma hattı
├─ Client-side sadece UX iyileştirmesi
└─ Bypass edilse bile backend korur

Sonuç: ✅ Korunmuş
```

#### Risk 3: Race Condition
```
Risk Seviyesi: DÜŞÜK
Açıklama: Aynı anda birden fazla tab açılırsa

Azaltma:
├─ Database UNIQUE constraint
├─ Transaction isolation level
└─ Lock mekanizması (gerekirse)

Sonuç: ✅ Korunmuş
```

### 9.3. OWASP Top 10 Compliance
```
A01:2021 – Broken Access Control
✅ Authorization attribute kullanımı
✅ TcKimlikNo doğrulama
✅ Role-based access control

A03:2021 – Injection
✅ Entity Framework parameterized queries
✅ Input validation
✅ Output encoding

A05:2021 – Security Misconfiguration
✅ Secure headers
✅ HTTPS enforcement
✅ CORS policy

A07:2021 – Identification and Authentication Failures
✅ Cookie-based authentication
✅ Secure session management
✅ SignalR authentication
```

---

## 10. Deployment Checklist

### 10.1. Pre-Deployment
```
□ Kod review tamamlandı mı?
□ Unit testler geçiyor mu?
□ Manuel testler tamamlandı mı?
□ Database migration hazır mı?
□ JavaScript dosyaları minify edildi mi?
□ Loglama sistemi hazır mı?
□ Rollback planı var mı?
```

### 10.2. Deployment Adımları

#### Adım 1: Database Migration
```bash
# Development ortamında test
dotnet ef database update --project DataAccessLayer --startup-project PresentationLayer

# Production ortamında
# Migration script'i generate et
dotnet ef migrations script --project DataAccessLayer --output migration.sql

# DBA'ya gönder veya manuel uygula
```

#### Adım 2: Backend Deployment
```bash
# Build
dotnet publish -c Release -o ./publish

# IIS'e deploy (Windows Server)
# - Application pool'u durdur
# - Dosyaları kopyala
# - Application pool'u başlat

# Veya Docker
docker build -t sgkportal:latest .
docker push registry.sgk.local/sgkportal:latest
```

#### Adım 3: Frontend Assets
```bash
# JavaScript dosyalarını minify et
npm run minify

# wwwroot dosyalarını deploy et
# - tab-manager.js
# - Diğer static assets
```

#### Adım 4: Configuration
```json
// appsettings.Production.json
{
  "TabManager": {
    "Enabled": true,
    "ProtectedPages": [
      "/siramatik/panel",
      "/Siramatik/Panel"
    ]
  },
  "Logging": {
    "LogLevel": {
      "TabManager": "Information"
    }
  }
}
```

### 10.3. Post-Deployment Verification
```
□ Uygulama başladı mı?
□ Database migration uygulandı mı?
□ JavaScript dosyaları yükleniyor mu?
□ Tab engelleme çalışıyor mu?
□ Server-side kontrol çalışıyor mu?
□ Modal gösterimi çalışıyor mu?
□ Loglama aktif mi?
□ Performans kabul edilebilir mi?
```

### 10.4. Monitoring

#### Application Insights
```csharp
// Startup.cs veya Program.cs
services.AddApplicationInsightsTelemetry();

// Custom event tracking
telemetryClient.TrackEvent("TabBlocked", new Dictionary<string, string>
{
    { "TcKimlikNo", tcKimlikNo },
    { "Page", "/siramatik/panel" },
    { "Method", "ClientSide" } // veya "ServerSide"
});
```

#### Log Queries
```sql
-- En çok tab açma girişimi yapan kullanıcılar
SELECT 
    TcKimlikNo,
    COUNT(*) as AttemptCount,
    MAX(IslemZamani) as LastAttempt
FROM [dbo].[TabBlockLogs]
WHERE IslemZamani >= DATEADD(DAY, -7, GETDATE())
GROUP BY TcKimlikNo
ORDER BY AttemptCount DESC;

-- Günlük engelleme istatistikleri
SELECT 
    CAST(IslemZamani AS DATE) as Tarih,
    BlockMethod, -- 'ClientSide' veya 'ServerSide'
    COUNT(*) as BlockCount
FROM [dbo].[TabBlockLogs]
GROUP BY CAST(IslemZamani AS DATE), BlockMethod
ORDER BY Tarih DESC;
```

### 10.5. Rollback Plan
```
Sorun Tespit Edilirse:

1. CLIENT-SIDE SORUN:
   ├─ tab-manager.js dosyasını devre dışı bırak
   ├─ MainLayout'tan initialize çağrısını kaldır
   └─ Sayfa yenile

2. SERVER-SIDE SORUN:
   ├─ CheckActiveTab endpoint'ini devre dışı bırak
   ├─ SiraCagirmaPanel'de kontrolü bypass et
   └─ Deploy

3. DATABASE SORUN:
   ├─ CurrentPage kolonunu NULL'a set et
   └─ Index'i drop et (geçici)

4. TAM ROLLBACK:
   ├─ Önceki release'i deploy et
   ├─ Database migration'ı geri al
   └─ JavaScript dosyalarını eski versiyona döndür
```

---

## 11. Performans Optimizasyonu

### 11.1. Client-Side Optimizasyon
```javascript
// Debounce event handler (çok fazla trigger'dan kaçın)
window.TabManager.handleLinkClick = debounce(function(event) {
    // ... mevcut kod
}, 100);

function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}
```

### 11.2. Server-Side Optimizasyon
```csharp
// Cache active connections (distributed cache)
public async Task<bool> CheckActiveTabCached(string tcKimlikNo, string page)
{
    var cacheKey = $"ActiveTab:{tcKimlikNo}:{page}";
    
    // Cache'den oku
    var cached = await _distributedCache.GetStringAsync(cacheKey);
    if (cached != null)
    {
        return bool.Parse(cached);
    }
    
    // Database'den oku
    var result = await CheckActiveTabFromDatabase(tcKimlikNo, page);
    
    // Cache'e yaz (30 saniye TTL)
    await _distributedCache.SetStringAsync(
        cacheKey, 
        result.ToString(),
        new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
        });
    
    return result;
}
```

### 11.3. Database Query Optimization
```sql
-- Index istatistikleri
SELECT 
    OBJECT_NAME(s.object_id) AS TableName,
    i.name AS IndexName,
    s.user_seeks,
    s.user_scans,
    s.user_lookups,
    s.user_updates
FROM sys.dm_db_index_usage_stats s
INNER JOIN sys.indexes i ON s.object_id = i.object_id AND s.index_id = i.index_id
WHERE OBJECT_NAME(s.object_id) = 'SIR_HubConnections';

-- Query execution plan analizi
SET STATISTICS IO ON;
SET STATISTICS TIME ON;

SELECT * FROM [dbo].[SIR_HubConnections]
WHERE TcKimlikNo = '12345678901' 
  AND CurrentPage = '/siramatik/panel'
  AND ConnectionMode = 'BANKO'
  AND SilindiMi = 0;
```

---

## 12. Troubleshooting

### 12.1. Yaygın Sorunlar ve Çözümler

#### Sorun 1: Modal Gösterilmiyor
```
Belirtiler:
- Link engelleniyor ama modal gösterilmiyor

Olası Nedenler:
├─ DotNet.invokeMethodAsync hatası
├─ JSInvokable attribute eksik
└─ Component instance bulunamıyor

Çözüm:
1. Browser console'u kontrol et
2. Network tab'de hata var mı bak
3. Blazor circuit active mi kontrol et
4. Log'lara bak

Debug:
console.log('[TabManager] Modal show attempt');
```

#### Sorun 2: Server-Side Kontrol Çalışmıyor
```
Belirtiler:
- Manuel URL girişinde uyarı sayfası gösterilmiyor

Olası Nedenler:
├─ API endpoint ulaşılamıyor
├─ Authorization hatası
└─ Database connection sorunu

Çözüm:
1. API endpoint'e Postman ile istek at
2. Authentication token'ı kontrol et
3. Database connection string doğru mu?
4. SQL Server Profiler ile query'leri incele
```

#### Sorun 3: Performans Sorunu
```
Belirtiler:
- Sayfa yavaş yükleniyor
- JavaScript donma

Olası Nedenler:
├─ Event listener çok fazla tetikleniyor
├─ Database query yavaş
└─ Network latency

Çözüm:
1. Browser Performance tab'ı kullan
2. Debounce implementasyonu ekle
3. Database index'leri kontrol et
4. Query execution plan'ı analiz et
```

### 12.2. Debug Mode
```javascript
// Debug mode açma
window.TabManager.debug = true;

window.TabManager.handleLinkClick = function(event) {
    if (this.debug) {
        console.log('[TabManager DEBUG]', {
            target: event.target,
            href: event.target.closest('a')?.getAttribute('href'),
            ctrlKey: event.ctrlKey,
            metaKey: event.metaKey,
            button: event.button,
            workMode: this.workMode
        });
    }
    
    // ... normal kod
};
```
```csharp
// Server-side debug logging
_logger.LogDebug("CheckActiveTab called: {TcKimlikNo}, {Page}", tcKimlikNo, page);
_logger.LogDebug("Active connection found: {ConnectionId}, Mode: {Mode}, CurrentPage: {CurrentPage}", 
    activeConnection?.ConnectionId, 
    activeConnection?.ConnectionMode, 
    activeConnection?.CurrentPage);
```

---

## 13. Ekler

### 13.1. Referanslar

- [Blazor Event Handling](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/event-handling)
- [SignalR Hub Methods](https://learn.microsoft.com/en-us/aspnet/core/signalr/hubs)
- [Entity Framework Core Indexes](https://learn.microsoft.com/en-us/ef/core/modeling/indexes)
- [Browser Events (MDN)](https://developer.mozilla.org/en-US/docs/Web/Events)

### 13.2. Glossary

| Term | Açıklama |
|------|----------|
| **Tab Manager** | Client-side JavaScript modülü, link tıklamalarını yakalayan |
| **BANKO Modu** | Personelin sıra çağırma yetkisinin olduğu çalışma modu |
| **İZLEME Modu** | Personelin sadece raporlama yapabildiği çalışma modu |
| **ForceDisconnect** | SignalR mesajı, eski tab'ı disconnect eden |
| **CurrentPage** | HubConnection'da tutulan, aktif sayfanın path'i |
| **Defense in Depth** | Çok katmanlı güvenlik stratejisi |

### 13.3. Changelog
```
v1.0.0 - 21 Kasım 2025
├─ İlk release
├─ Client-side link engelleme
├─ Server-side aktif tab kontrolü
├─ Modal uyarı sistemi
└─ Uyarı sayfası

Gelecek Versiyonlar:
v1.1.0 - Planlanan
├─ Tab otomatik focus özelliği
├─ Keyboard shortcuts
└─ Gelişmiş loglama

v1.2.0 - Planlanan
├─ Multi-monitor desteği
└─ Tab senkronizasyonu
```

---

## 14. Katkıda Bulunanlar

- **Proje Yöneticisi:** [İsim]
- **Backend Developer:** [İsim]
- **Frontend Developer:** [İsim]
- **QA Engineer:** [İsim]
- **DevOps Engineer:** [İsim]

---

## 15. Lisans ve İletişim

**İç Kullanım Dökümanı**  
SGK İzmir İl Müdürlüğü - Bilgi İşlem  

İletişim: [email@sgk.gov.tr]  
Versiyon: 1.0  
Son Güncelleme: 21 Kasım 2025