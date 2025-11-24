# 🏠 SGK Portal Dashboard Yapısı - Güncel Plan
---

## 📸 Referans Tasarım

![Dashboard Referans](dashboard-reference.png)

*Yukarıdaki tasarım referans alınarak Sneat template ile uyumlu modern dashboard oluşturulacak.*

---

## 1. Genel Bakış

**Amaç:** Kullanıcıların ilk giriş yaptığında göreceği bilgilendirici ve interaktif ana sayfa.

**Tasarım Prensibi:** 
- ✅ Sneat template ile %100 uyumlu
- ✅ Modern card-based layout
- ✅ Responsive (mobil uyumlu)
- ✅ Dinamik içerik yönetimi

**Hedef Kitle:** Tüm SGK Portal kullanıcıları

---

## 2. Dashboard Yapısı

### 2.1. Üst Bölüm - Hoş Geldiniz
```html
<div class='row mb-4'>
  <div class='col-12'>
    <div class='card bg-primary text-white'>
      <div class='card-body'>
        <h4 class='text-white mb-1'>👋 Hoş Geldiniz, {{KullaniciAdi}}!</h4>
        <p class='mb-0'>SGK Portal sistemine hoş geldiniz. Sol menüden işlemlerinize başlayabilirsiniz.</p>
      </div>
    </div>
  </div>
</div>
```

### 2.2. İstatistik Kartları (4 Kolon - Zorunlu)
```html
<div class='row mb-4'>
  <div class='col-xl-3 col-md-6 mb-4'>
    <div class='card'>
      <div class='card-body'>
        <div class='d-flex align-items-center'>
          <div class='avatar flex-shrink-0 me-3'>
            <span class='avatar-initial rounded bg-label-primary'>
              <i class='bx bx-user bx-sm'></i>
            </span>
          </div>
          <div>
            <small class='text-muted d-block'>Toplam Personel</small>
            <h4 class='mb-0'>1,234</h4>
            <small class='text-success'><i class='bx bx-up-arrow-alt'></i> +15 bu ay</small>
          </div>
        </div>
      </div>
    </div>
  </div>
  <!-- Diğer 3 kart benzer yapıda -->
</div>
```

**Kartlar:**
1. 👤 **Toplam Personel** - Personel tablosundan
2. 💻 **Aktif Bankolar** - Banko tablosundan (Aktiflik = Aktif)
3. 📅 **Bekleyen İzinler** - İzin modülünden (gelecekte)
4. 🚌 **Eshot Kullanımı** - Eshot modülünden (gelecekte)

---

## 3. Ana İçerik (2 Kolon Layout)

### 3.1. Sol Kolon (col-xl-8)

#### A) Duyuru Slider (Büyük Kart)
```html
<div class='card mb-4'>
  <div class='card-body p-0'>
    <div id='duyuruSlider' class='carousel slide' data-bs-ride='carousel'>
      <div class='carousel-inner'>
        <div class='carousel-item active'>
          <img src='duyuru1.jpg' class='d-block w-100' style='height: 400px; object-fit: cover;'>
          <div class='carousel-caption'>
            <span class='badge bg-danger mb-2'>DUYURU</span>
            <h5>Konak İlçe Tanım ve Orman Müdürlüğünden Ziyaret</h5>
            <p><i class='bx bx-calendar'></i> 28.08.2025</p>
          </div>
        </div>
      </div>
      <button class='carousel-control-prev'>
        <span class='carousel-control-prev-icon'></span>
      </button>
      <button class='carousel-control-next'>
        <span class='carousel-control-next-icon'></span>
      </button>
    </div>
  </div>
  <div class='card-footer text-end'>
    <a href='/common/duyuru' class='btn btn-sm btn-primary'>Tüm Duyurular</a>
  </div>
</div>
```

#### B) İki Alt Kart (Row)

**B1) SGK Duyurular (col-md-6)**
```html
<div class='col-md-6 mb-4'>
  <div class='card h-100'>
    <div class='card-header'>
      <h5 class='mb-0'><i class='bx bx-news'></i> SGK Duyurular</h5>
    </div>
    <div class='card-body'>
      <div class='list-group list-group-flush'>
        <a href='#' class='list-group-item list-group-item-action'>
          <div class='d-flex w-100 justify-content-between'>
            <h6 class='mb-1'>Anlaşma Yapılana Özel Hostanelene İlşkin...</h6>
            <small><i class='bx bx-calendar'></i> 28.08.2025</small>
          </div>
        </a>
        <!-- Diğer duyurular -->
      </div>
    </div>
    <div class='card-footer text-end'>
      <a href='/common/duyuru' class='btn btn-sm btn-outline-primary'>Tüm Duyurular</a>
    </div>
  </div>
</div>
```

**B2) Sık Kullanılan Programlar (col-md-6)**
```html
<div class='col-md-6 mb-4'>
  <div class='card h-100'>
    <div class='card-header'>
      <h5 class='mb-0'><i class='bx bx-grid-alt'></i> Sık Kullanılan Programlar</h5>
    </div>
    <div class='card-body'>
      <div class='row g-3'>
        @if (!programlar.Any())
        {
          <div class='col-12 text-center py-5'>
            <i class='bx bx-folder-open display-1 text-muted'></i>
            <p class='text-muted'>Henüz program eklenmemiş</p>
          </div>
        }
        else
        {
          @foreach (var program in programlar)
          {
            <div class='col-6'>
              <a href='@program.Url' target='_blank' class='text-decoration-none'>
                <div class='card bg-label-@program.RenkKodu'>
                  <div class='card-body text-center'>
                    <i class='bx @program.IkonClass bx-lg'></i>
                    <p class='mb-0 mt-2 small'>@program.ProgramAdi</p>
                  </div>
                </div>
              </a>
            </div>
          }
        }
      </div>
    </div>
  </div>
</div>
```

### 3.2. Sağ Kolon (col-xl-4)

#### A) Günün Menüsü
```html
<div class='card mb-4'>
  <div class='card-header'>
    <h5 class='mb-0'><i class='bx bx-food-menu'></i> Günün Menüsü</h5>
  </div>
  <div class='card-body'>
    <div class='d-flex align-items-center mb-3'>
      <i class='bx bx-calendar text-primary me-2'></i>
      <span class='fw-semibold'>20 Kasım 2025</span>
    </div>
    <div class='menu-content'>
      @if (menu != null)
      {
        <p>@menu.Icerik</p>
      }
      else
      {
        <p class='text-muted'>Bugün için menü bilgisi yok</p>
      }
    </div>
  </div>
</div>
```

#### B) Önemli Linkler
```html
<div class='card mb-4'>
  <div class='card-header'>
    <h5 class='mb-0'><i class='bx bx-link-external'></i> Önemli Linkler</h5>
  </div>
  <div class='card-body'>
    <ul class='list-unstyled mb-0'>
      @foreach (var link in linkler)
      {
        <li class='mb-2'>
          <a href='@link.Url' target='_blank' class='text-decoration-none'>
            <i class='bx bx-chevron-right'></i> @link.LinkAdi
          </a>
        </li>
      }
    </ul>
  </div>
</div>
```

#### C) Bugün Doğanlar
```html
<div class='card'>
  <div class='card-header'>
    <h5 class='mb-0'><i class='bx bx-cake'></i> Bugün Doğanlar</h5>
  </div>
  <div class='card-body'>
    @if (!dogumlular.Any())
    {
      <p class='text-muted text-center'>Bugün doğum günü olan yok</p>
    }
    else
    {
      @foreach (var personel in dogumlular)
      {
        <div class='d-flex align-items-center mb-3'>
          <div class='avatar me-3'>
            <span class='avatar-initial rounded-circle bg-label-warning'>
              <i class='bx bx-cake'></i>
            </span>
          </div>
          <div>
            <h6 class='mb-0'>@personel.AdSoyad</h6>
            <small class='text-muted'>@personel.DepartmanAdi</small>
          </div>
        </div>
      }
    }
  </div>
</div>
```

---

## 4. Veri Modeli

### 4.1. Duyurular
```csharp
public class Duyuru : AuditableEntity
{
    [Key]
    public int DuyuruId { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Baslik { get; set; }
    
    [Required]
    public string Icerik { get; set; }
    
    [StringLength(500)]
    public string? GorselUrl { get; set; }
    
    [Required]
    public int Sira { get; set; }
    
    [Required]
    public DateTime YayinTarihi { get; set; }
    
    public DateTime? BitisTarihi { get; set; }
    
    public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
}
```

### 4.2. Sık Kullanılan Programlar
```csharp
public class SikKullanilanProgram : AuditableEntity
{
    [Key]
    public int ProgramId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string ProgramAdi { get; set; }
    
    [Required]
    [StringLength(500)]
    public string Url { get; set; }
    
    [Required]
    [StringLength(50)]
    public string IkonClass { get; set; } // bx-desktop, bx-file, etc.
    
    [Required]
    [StringLength(20)]
    public string RenkKodu { get; set; } // primary, danger, info, warning
    
    [Required]
    public int Sira { get; set; }
    
    public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
}
```

### 4.3. Önemli Linkler
```csharp
public class OnemliLink : AuditableEntity
{
    [Key]
    public int LinkId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string LinkAdi { get; set; }
    
    [Required]
    [StringLength(500)]
    public string Url { get; set; }
    
    [Required]
    public int Sira { get; set; }
    
    public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
}
```

### 4.4. Günün Menüsü
```csharp
public class GununMenusu : AuditableEntity
{
    [Key]
    public int MenuId { get; set; }
    
    [Required]
    public DateTime Tarih { get; set; }
    
    [Required]
    public string Icerik { get; set; }
    
    public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
}
```

---

## 5. Yönetim Paneli

### 5.1. Navigation Menu
```
Ortak Tanımlar
├── 📢 Duyurular (/common/duyuru)
├── 🔗 Sık Kullanılan Programlar (/common/sik-kullanilan-program)
├── 🌐 Önemli Linkler (/common/onemli-link)
└── 🍽️ Günün Menüsü (/common/gunun-menusu)
```

### 5.2. Yönetim Sayfaları

**Her modül için:**
- ✅ **Index.razor** - Card grid layout, sıralı listeleme
- ✅ **Manage.razor** - Create/Update formu
- ✅ Otomatik sıra yönetimi (max + 1)
- ✅ Aktif/Pasif toggle
- ✅ Resim upload (duyurular için)
- ✅ WYSIWYG editor (içerik için)

---

## 6. API Endpoints

```
Dashboard:
GET /api/Dashboard/stats          - İstatistikler
GET /api/Dashboard/birthdays      - Bugün doğanlar

Duyuru:
GET /api/Duyuru                    - Tüm duyurular
GET /api/Duyuru/active             - Aktif duyurular (slider için)
POST /api/Duyuru                   - Yeni duyuru
PUT /api/Duyuru/{id}               - Güncelle
DELETE /api/Duyuru/{id}            - Sil

Sık Kullanılan Program:
GET /api/SikKullanilanProgram      - Tüm programlar
POST /api/SikKullanilanProgram     - Yeni program
PUT /api/SikKullanilanProgram/{id} - Güncelle
DELETE /api/SikKullanilanProgram/{id} - Sil

Önemli Link:
GET /api/OnemliLink                - Tüm linkler
POST /api/OnemliLink               - Yeni link
PUT /api/OnemliLink/{id}           - Güncelle
DELETE /api/OnemliLink/{id}        - Sil

Günün Menüsü:
GET /api/GununMenusu/today         - Bugünün menüsü
GET /api/GununMenusu/date/{tarih}  - Belirli tarih
POST /api/GununMenusu              - Yeni menü
PUT /api/GununMenusu/{id}          - Güncelle
DELETE /api/GununMenusu/{id}       - Sil
```

---

## 7. TV Görünümü (Fullscreen - Layout: null)

### 7.1. TV Ekranı Tasarımı
**Route:** `/tv/display/{tvId}` (Layout: null)

```html
<!DOCTYPE html>
<html lang="tr">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>SGK Portal TV</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.2.3/dist/css/bootstrap.min.css" rel="stylesheet">
    <style>
        body {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            margin: 0;
            padding: 0;
            overflow: hidden;
        }
        
        .tv-container {
            height: 100vh;
            display: flex;
            flex-direction: column;
        }
        
        .tv-header {
            background: rgba(255, 255, 255, 0.95);
            padding: 20px;
            box-shadow: 0 4px 6px rgba(0,0,0,0.1);
        }
        
        .tv-logo {
            max-height: 80px;
        }
        
        .tv-title {
            font-size: 3rem;
            font-weight: bold;
            color: #2c3e50;
        }
        
        .tv-content {
            flex: 1;
            display: flex;
            gap: 20px;
            padding: 20px;
        }
        
        .sira-panel {
            flex: 1;
            background: rgba(255, 255, 255, 0.95);
            border-radius: 20px;
            padding: 30px;
            overflow-y: auto;
        }
        
        .banko-card {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border-radius: 15px;
            padding: 20px;
            margin-bottom: 15px;
            display: flex;
            align-items: center;
            box-shadow: 0 8px 16px rgba(0,0,0,0.2);
        }
        
        .banko-info {
            flex: 1;
            text-align: center;
        }
        
        .banko-no {
            font-size: 4vw;
            font-weight: bold;
        }
        
        .banko-kat {
            font-size: 1.5vw;
            opacity: 0.9;
        }
        
        .sira-no {
            font-size: 6vw;
            font-weight: bold;
            text-align: center;
            flex: 1;
        }
        
        .duyuru-panel {
            flex: 1;
            display: flex;
            flex-direction: column;
            gap: 20px;
        }
        
        .duyuru-card {
            background: rgba(255, 255, 255, 0.95);
            border-radius: 20px;
            padding: 30px;
            flex: 1;
        }
        
        .duyuru-header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 15px;
            border-radius: 10px;
            margin-bottom: 20px;
        }
        
        .marquee-text {
            font-size: 2rem;
            font-weight: bold;
            color: #2c3e50;
        }
        
        .video-container {
            border-radius: 15px;
            overflow: hidden;
        }
        
        .clock-card {
            background: rgba(255, 255, 255, 0.95);
            border-radius: 20px;
            padding: 30px;
            text-align: center;
        }
        
        .clock-time {
            font-size: 4rem;
            font-weight: bold;
            color: #667eea;
        }
        
        .clock-date {
            font-size: 2rem;
            color: #764ba2;
        }
    </style>
</head>
<body>
    <div class="tv-container">
        <!-- Header -->
        <div class="tv-header d-flex justify-content-between align-items-center">
            <img src="/img/logos/sgk_logo.svg" alt="SGK" class="tv-logo">
            <h1 class="tv-title">İzmir Sosyal Güvenlik İl Müdürlüğü</h1>
            <div class="text-end">
                <small class="text-muted">TV ID: {{TvId}}</small>
            </div>
        </div>
        
        <!-- Content -->
        <div class="tv-content">
            <!-- Sol Panel - Sıralar -->
            <div class="sira-panel">
                <div id="siraListe">
                    <!-- SignalR ile dinamik yüklenecek -->
                </div>
            </div>
            
            <!-- Sağ Panel - Duyuru & Saat -->
            <div class="duyuru-panel">
                <!-- Duyuru -->
                <div class="duyuru-card">
                    <div class="duyuru-header">
                        <h3 class="m-0">DUYURU</h3>
                    </div>
                    <marquee behavior="scroll" direction="left" scrollamount="3">
                        <p class="marquee-text" id="duyuruText">Duyuru yükleniyor...</p>
                    </marquee>
                    <div class="video-container">
                        <video id="tvVideo" class="w-100" autoplay loop muted>
                            <source src="" type="video/mp4">
                        </video>
                    </div>
                </div>
                
                <!-- Saat -->
                <div class="clock-card">
                    <div class="clock-time" id="saat">00:00:00</div>
                    <div class="clock-date" id="tarih">-</div>
                </div>
            </div>
        </div>
    </div>
    
    <audio id="dingDongSound" src="/sounds/dingdong.mp3" preload="auto"></audio>
    
    <script src="/lib/microsoft/signalr/dist/browser/signalr.js"></script>
    <script src="/js/tv-display.js"></script>
</body>
</html>
```

### 7.2. Yönetim Sayfaları (Mevcut Yapıyla Aynı)

**Önemli:** Tüm yönetim sayfaları **mevcut KioskMenu, Kiosk, vb. sayfalarla birebir aynı** yapıda olacak.

**Sayfalar:**
```
/common/duyuru
├── Index.razor          - KioskMenu/Index.razor ile aynı yapı
└── Manage.razor         - KioskMenu/Manage.razor ile aynı yapı

/common/sik-kullanilan-program
├── Index.razor          - KioskMenu/Index.razor ile aynı yapı
└── Manage.razor         - KioskMenu/Manage.razor ile aynı yapı

/common/onemli-link
├── Index.razor          - KioskMenu/Index.razor ile aynı yapı
└── Manage.razor         - KioskMenu/Manage.razor ile aynı yapı

/common/gunun-menusu
├── Index.razor          - KioskMenu/Index.razor ile aynı yapı
└── Manage.razor         - KioskMenu/Manage.razor ile aynı yapı
```

**Ortak Özellikler (Mevcut Sayfalardan):**
```
✅ Card grid layout (3 kolon)
✅ Header (başlık + açıklama + Yeni buton)
✅ İstatistik kartları (Toplam, Aktif, Pasif)
✅ Refresh butonu
✅ Aktif/Pasif toggle (dropdown menü)
✅ Düzenle/Sil butonları (dropdown menü)
✅ Modal form (Manage sayfası)
✅ Toast notifications
✅ Loading states
✅ Empty states
```

**Örnek: Duyuru Index.razor**
```razor
@* KioskMenu/Index.razor ile AYNI yapı *@
@page "/common/duyuru"
@attribute [Authorize]

<PageTitle>Duyurular - SGK Portal</PageTitle>

<div class="container-xxl flex-grow-1 container-p-y">
    <!-- Header - KioskMenu ile aynı -->
    <div class="d-flex justify-content-between align-items-center mb-4">
        <div>
            <h4 class="fw-bold mb-1">
                <i class="bx bx-news me-2"></i>Duyurular
            </h4>
            <p class="text-muted mb-0">Dashboard ve TV ekranları için duyuru yönetimi</p>
        </div>
        <div>
            <button class="btn btn-icon btn-outline-primary me-2" @onclick="RefreshAsync">
                <i class="bx bx-refresh"></i>
            </button>
            <a href="/common/duyuru/manage" class="btn btn-primary">
                <i class="bx bx-plus me-1"></i>Yeni Duyuru
            </a>
        </div>
    </div>

    <!-- İstatistikler - KioskMenu ile aynı -->
    <div class="row mb-4">
        <div class="col-xl-3 col-sm-6 mb-4">
            <div class="card">
                <div class="card-body">
                    <div class="d-flex align-items-start justify-content-between">
                        <div class="avatar flex-shrink-0 me-3">
                            <span class="avatar-initial rounded bg-label-primary">
                                <i class="bx bx-news bx-sm"></i>
                            </span>
                        </div>
                        <div class="dropdown">
                            <!-- ... -->
                        </div>
                    </div>
                    <span class="fw-semibold d-block mb-1">Toplam</span>
                    <h3 class="card-title mb-2">@toplamDuyuru</h3>
                </div>
            </div>
        </div>
        <!-- Aktif, Pasif, Filtrelenen kartları -->
    </div>

    <!-- Card Grid - KioskMenu ile aynı -->
    <div class="row">
        @foreach (var duyuru in duyurular)
        {
            <div class="col-xl-4 col-md-6 mb-4">
                <div class="card h-100">
                    <div class="card-body">
                        <!-- KioskMenu card yapısı ile aynı -->
                    </div>
                </div>
            </div>
        }
    </div>
</div>
```

**NOT:** UI'da **hiçbir değişiklik yok**, sadece içerik farklı.

---

## 8. Geliştirme Planı

### Faz 1: Backend (3 gün)
- [ ] Entity'leri oluştur (Duyuru, SikKullanilanProgram, OnemliLink, GununMenusu)
- [ ] DTO'ları hazırla
- [ ] Repository'leri implement et
- [ ] Service'leri yaz
- [ ] Controller'ları oluştur
- [ ] Migration'ları uygula

### Faz 2: Dashboard UI - Sneat Template (2 gün)
- [ ] Ana dashboard layout (/dashboard)
- [ ] İstatistik kartları (4 adet)
- [ ] Duyuru slider component
- [ ] Sık Kullanılan Programlar (boş başlangıç)
- [ ] Önemli Linkler widget
- [ ] Günün Menüsü widget
- [ ] Bugün Doğanlar widget
- [ ] Responsive tasarım

### Faz 3: Yönetim Paneli - Sneat Template (3 gün)
- [ ] Duyuru yönetimi (Index, Manage)
- [ ] Program yönetimi (Index, Manage)
- [ ] Link yönetimi (Index, Manage)
- [ ] Menü yönetimi (Index, Manage)
- [ ] TV Ayarları (Index, Manage)
- [ ] Resim upload sistemi
- [ ] Video upload sistemi

### Faz 4: TV Görünümü - Fullscreen (2 gün)
- [ ] TV Display sayfası (Layout: null)
- [ ] SignalR entegrasyonu
- [ ] Sıra gösterimi (real-time)
- [ ] Duyuru marquee
- [ ] Video player
- [ ] Saat/Tarih widget
- [ ] Ses efekti (ding-dong)
- [ ] Auto-refresh mekanizması

### Faz 5: Test ve İyileştirme (2 gün)
- [ ] Tüm modülleri test et
- [ ] TV ekranı test (farklı çözünürlükler)
- [ ] SignalR bağlantı testi
- [ ] Responsive test
- [ ] Loading state'leri
- [ ] Error handling
- [ ] Performance optimizasyonu

**Toplam Süre:** 12 iş günü (2.5 hafta)

---

## 9. Önemli Notlar

### TV Görünümü Özellikleri
✅ **Layout: null** - Sidebar/header yok
✅ **Fullscreen** - 100vh
✅ **Büyük fontlar** - 4-6vw
✅ **Yüksek kontrast** - Gradient background
✅ **SignalR** - Real-time sıra güncellemeleri
✅ **Auto-play** - Video otomatik başlar
✅ **Ses efekti** - Yeni sıra geldiğinde
✅ **Responsive** - Farklı TV çözünürlükleri

### Yönetim Sayfaları Özellikleri
✅ **Sneat Template** - %100 uyumlu
✅ **Card Grid Layout** - Mevcut yapıyla aynı
✅ **Modal Forms** - Create/Update
✅ **Toast Notifications** - Başarı/Hata
✅ **Dropdown Filters** - Hizmet binası, vb.
✅ **Aktif/Pasif Toggle** - Tek tıkla
✅ **Refresh Buttons** - Manuel yenileme

---

**Son Güncelleme:** 21 Kasım 2025  
**Durum:** ✅ Onaylandı - Geliştirmeye Hazır  
**Tasarım:** 
- Dashboard & Yönetim: Sneat Template
- TV Görünümü: Fullscreen (Layout: null)
