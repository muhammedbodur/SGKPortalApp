# 🏷️ Kiosk Yapısı – Güncel Durum ve Dokümantasyon
---
## 1. Genel Bakış

✅ **Tamamlanan Hedefler:**
1. Vatandaşın ilk gördüğü **ana menü** için merkezi tanım ve yönetim arayüzü kuruldu.
2. Her **hizmet binası / kiosk** kombinasyonu için hangi menülerin gösterileceği ve sırası belirleniyor.
3. Ana menüler ile mevcut `SIR_KanalAltIslemleri` arasındaki bağ `KioskMenuIslem` tablosu ile sağlandı.
4. Mevcut EF/DTO/Service pattern'i korunarak yeni tablolar BusinessObjectLayer'a eklendi.
5. Otomatik sıra yönetimi tüm modüllerde (KioskMenu, KioskMenuIslem, KanalIslem) uygulandı.

---

## 2. ✅ Tamamlanan Yapı Değişiklikleri

1. ✅ Eski `KioskGrup`, `KioskIslemGrup` tabloları kaldırıldı.
2. ✅ Yeni yapı tamamen devreye alındı ve çalışır durumda.
3. ✅ Tüm entity'ler, DTO'lar, repository'ler ve servisler oluşturuldu.
4. ✅ UI katmanı tamamlandı ve aktif olarak kullanılıyor.

---

## 3. ✅ Mevcut Veri Modeli (Uygulanmış)

### 3.1. Ana Menü Tanımı

| Tablo | Açıklama | Alanlar |
| --- | --- | --- |
| **`SIR_KioskMenuTanim`** | Ana menü tanımı. | `KioskMenuId`, `MenuAdi`, `Aciklama`, `MenuSira` (otomatik), `Aktiflik`, `EklenmeTarihi`, `DuzenlenmeTarihi`, `SilindiMi` |

**Özellikler:**
- ✅ `MenuSira` otomatik hesaplanıyor (max + 1)
- ✅ Soft delete desteği
- ✅ Aktif/Pasif durum yönetimi

### 3.2. Kiosk Tanımı

| Tablo | Açıklama | Alanlar |
| --- | --- | --- |
| **`SIR_KioskTanim`** | Hizmet binasına bağlı kiosk cihazı. | `KioskId`, `HizmetBinasiId`, `KioskAdi`, `KioskKodu`, `KioskIp`, `Konum`, `Aktiflik`, `EklenmeTarihi`, `DuzenlenmeTarihi`, `SilindiMi` |

**Özellikler:**
- ✅ Hizmet binası bazlı filtreleme
- ✅ IP ve konum bilgisi
- ✅ Unique kiosk kodu

### 3.3. Menü → Alt Kanal Eşleştirmesi

| Tablo | Açıklama | Alanlar |
| --- | --- | --- |
| **`SIR_KioskMenuIslemleri`** | Menüye atanan alt kanal işlemleri. | `KioskMenuIslemId`, `KioskMenuId`, `KanalAltId`, `MenuSira` (otomatik), `Aktiflik`, `EklenmeTarihi`, `DuzenlenmeTarihi`, `SilindiMi` |

**Özellikler:**
- ✅ `MenuSira` otomatik hesaplanıyor (menü bazında max + 1)
- ✅ Duplicate kontrolü (aynı alt kanal aynı menüye tekrar eklenemez)
- ✅ Navigation properties: `KioskMenu`, `KanalAlt`

### 3.4. Kiosk → Menü Ataması

| Tablo | Açıklama | Alanlar |
| --- | --- | --- |
| **`SIR_KioskMenuAtama`** | Kiosk cihazlarına menü ataması. | `KioskMenuAtamaId`, `KioskId`, `KioskMenuId`, `AtamaTarihi`, `Aktiflik`, `EklenmeTarihi`, `DuzenlenmeTarihi`, `SilindiMi` |

**Özellikler:**
- ✅ Bir kiosk'a birden fazla menü atanabilir
- ✅ Aynı menü aynı kiosk'a tekrar atanamaz (unique constraint)
- ✅ Aktif/Pasif toggle özelliği
- ✅ Card grid UI ile görselleştirilmiş yönetim

---

## 4. ✅ Katman Yapısı (Tamamlanmış)

### 4.1. Entities (BusinessObjectLayer/Entities/SiramatikIslemleri)
✅ **Tamamlandı:**
- `KioskMenu` - AuditableEntity'den türetildi
- `Kiosk` - AuditableEntity'den türetildi
- `KioskMenuIslem` - AuditableEntity'den türetildi
- `KioskMenuAtama` - AuditableEntity'den türetildi
- Navigation property'ler `[InverseProperty]` ile tanımlandı
- `MenuSira` alanları eklendi ve otomatik yönetiliyor

### 4.2. DTO'lar
✅ **Request DTO'lar:**
- `KioskMenuCreateRequestDto`, `KioskMenuUpdateRequestDto`
- `KioskCreateRequestDto`, `KioskUpdateRequestDto`
- `KioskMenuIslemCreateRequestDto`, `KioskMenuIslemUpdateRequestDto`
- `KioskMenuAtamaCreateRequestDto`, `KioskMenuAtamaUpdateRequestDto`

✅ **Response DTO'lar:**
- `KioskMenuResponseDto`
- `KioskResponseDto`
- `KioskMenuIslemResponseDto`
- `KioskMenuAtamaResponseDto`
- `KioskSummaryDto` (ayrı dosyada)

### 4.3. Repositories
✅ **Interface ve Implementation:**
- `IKioskMenuRepository` / `KioskMenuRepository`
  - `GetActiveAsync()`, `GetWithKiosksAsync()`, `ExistsByNameAsync()`, `GetMaxSiraAsync()`
- `IKioskRepository` / `KioskRepository`
  - `GetByHizmetBinasiAsync()`, `GetWithDetailsAsync()`, `ExistsByKodAsync()`
- `IKioskMenuIslemRepository` / `KioskMenuIslemRepository`
  - `GetByKioskMenuAsync()`, `ExistsByMenuAndSiraAsync()`, `GetMaxSiraByMenuAsync()`
- `IKioskMenuAtamaRepository` / `KioskMenuAtamaRepository`
  - `GetByKioskAsync()`, `GetByKioskAndMenuAsync()`, `GetWithDetailsAsync()`

### 4.4. Services
✅ **Business Logic Layer:**
- `IKioskMenuService` / `KioskMenuService`
  - CRUD operasyonları, otomatik sıra hesaplama
- `IKioskService` / `KioskService`
  - CRUD operasyonları, hizmet binası bazlı filtreleme
- `IKioskMenuIslemService` / `KioskMenuIslemService`
  - CRUD operasyonları, otomatik sıra hesaplama, duplicate kontrolü
- `IKioskMenuAtamaService` / `KioskMenuAtamaService`
  - CRUD operasyonları, duplicate kontrolü, aktif/pasif toggle

### 4.5. Presentation Layer
✅ **Sayfalar:**
- `/siramatik/kiosk-menu` - Menü tanımlama (Index, Manage)
- `/siramatik/kiosk` - Kiosk tanımlama (Index, Manage)
- `/siramatik/kiosk-menu-islem` - Menü işlem ataması (Index, Manage)
- `/siramatik/kiosk-menu-atama` - Kiosk menü ataması (Index, Manage)

✅ **UI Özellikleri:**
- Card grid layout (responsive)
- Modal form'lar
- Dropdown filtreleme (hizmet binası, kiosk, menü)
- Aktif/Pasif toggle
- Refresh butonları
- İstatistik göstergeleri
- Toast bildirimleri

---

## 5. ✅ İş Akışı (Mevcut Uygulama)

### 5.1. Kiosk Menü Tanımı Oluşturma
1. `/siramatik/kiosk-menu` sayfasından "Yeni Menü" butonu ile form açılır
2. Menü adı ve açıklama girilir
3. `MenuSira` otomatik hesaplanır (kullanıcı değiştirebilir)
4. Aktif/Pasif durumu seçilir
5. Kaydet → Menü oluşturulur

### 5.2. Menüye Alt Kanal İşlemi Ekleme
1. `/siramatik/kiosk-menu-islem` sayfasından menü seçilir
2. "Yeni İşlem Ekle" butonu ile form açılır
3. Alt kanal dropdown'dan seçilir (alfabetik sıralı)
4. `MenuSira` otomatik hesaplanır (menü bazında)
5. Duplicate kontrolü yapılır
6. Kaydet → İşlem menüye eklenir

### 5.3. Kiosk Tanımı
1. `/siramatik/kiosk` sayfasından "Yeni Kiosk" butonu ile form açılır
2. Hizmet binası seçilir (dropdown)
3. Kiosk adı, kodu, IP ve konum bilgileri girilir
4. Aktif/Pasif durumu seçilir
5. Kaydet → Kiosk oluşturulur

### 5.4. Kiosk'a Menü Atama
1. `/siramatik/kiosk-menu-atama` sayfası açılır
2. Hizmet binası seçilir → Kiosk'lar otomatik yüklenir
3. Kiosk seçilir → O kiosk'un mevcut atamaları gösterilir
4. "Yeni Atama" butonu ile form açılır
5. Menü seçilir (dropdown)
6. Duplicate kontrolü yapılır (aynı menü tekrar atanamaz)
7. Kaydet → Atama oluşturulur
8. Card grid'de aktif/pasif toggle yapılabilir

### 5.5. Kiosk Masaüstü Uygulaması (Planlanan)
1. API'den kiosk bilgileri ve atanan menüler çekilir
2. Her menü için `KioskMenuIslem` kayıtları çekilir
3. Vatandaş menü seçtiğinde ilgili alt kanal işlemleri gösterilir
4. Sıralama `MenuSira` alanına göre yapılır

---

## 6. ✅ Migration Geçmişi

### Uygulanan Migration'lar:
1. ✅ `AddKioskEntities` - İlk kiosk tabloları oluşturuldu
2. ✅ `UpdateKioskMenuAtamaUniqueConstraint` - Unique constraint düzeltildi
3. ✅ `AddMenuSiraToKioskMenu` - MenuSira alanı eklendi

### Database Yapısı:
- ✅ Tüm tablolar `SIR_` prefix'i ile oluşturuldu
- ✅ Foreign key'ler tanımlandı
- ✅ Unique constraint'ler eklendi
- ✅ Soft delete filter'ları aktif
- ✅ Audit alanları (EklenmeTarihi, DuzenlenmeTarihi, SilindiMi) tüm tablolarda mevcut

---

## 7. 🎯 Özellikler ve İyileştirmeler

### Otomatik Sıra Yönetimi
✅ **Tüm modüllerde uygulandı:**
- `KioskMenu.MenuSira` - Global sıra (max + 1)
- `KioskMenuIslem.MenuSira` - Menü bazında sıra (max + 1)
- `KanalIslem.Sira` - Kanal + Hizmet binası bazında sıra (max + 1)

**Mantık:**
- Sıra girilmezse (0) → Otomatik hesaplanır
- Sıra girilirse → Kullanıcının girdiği değer kullanılır
- Duplicate kontrolü yapılır

### UI/UX İyileştirmeleri
✅ **Tamamlandı:**
- Card grid layout (responsive, 3 kolon)
- Dropdown otomatik sıralama (MenuSira, alfabetik)
- Aktif/Pasif toggle (tek tıkla)
- Refresh butonları
- İstatistik göstergeleri (toplam, aktif, pasif, filtrelenen)
- Toast bildirimleri (başarı, hata)
- Loading state'leri
- Empty state mesajları

### Validasyon ve Kontroller
✅ **Uygulandı:**
- Required field kontrolü
- Duplicate kontrolü (menü adı, kiosk kodu, atamalar)
- ID mismatch kontrolü (update işlemlerinde)
- Navigation property null kontrolü
- Inner exception logging

---

## 8. 📋 Sonraki Adımlar

### Kısa Vadeli
1. ⏳ Kiosk masaüstü uygulaması API entegrasyonu
2. ⏳ BankoIslem ve Banko için otomatik sıra yönetimi (isteğe bağlı)
3. ⏳ Toplu işlem özellikleri (çoklu atama, kopyalama)

### Orta Vadeli
1. ⏳ Sürükle-bırak sıralama UI'ı
2. ⏳ Menü önizleme özelliği
3. ⏳ Raporlama ve istatistikler

### Uzun Vadeli
1. ⏳ Çoklu dil desteği
2. ⏳ Offline mod desteği (masaüstü uygulama)
3. ⏳ Kiosk kullanım analitiği

---

## 9. 📊 Teknik Detaylar

### API Endpoints
```
GET    /api/KioskMenu
GET    /api/KioskMenu/{id}
POST   /api/KioskMenu
PUT    /api/KioskMenu/{id}
DELETE /api/KioskMenu/{id}

GET    /api/Kiosk
GET    /api/Kiosk/{id}
GET    /api/Kiosk/byhizmetbinasi/{hizmetBinasiId}
POST   /api/Kiosk
PUT    /api/Kiosk/{id}
DELETE /api/Kiosk/{id}

GET    /api/KioskMenuIslem/bymenu/{kioskMenuId}
POST   /api/KioskMenuIslem
PUT    /api/KioskMenuIslem/{id}
DELETE /api/KioskMenuIslem/{id}

GET    /api/KioskMenuAtama/bykiosk/{kioskId}
POST   /api/KioskMenuAtama
PUT    /api/KioskMenuAtama/{id}
DELETE /api/KioskMenuAtama/{id}
```

### Navigation Menu
```
Sıramatik İşlemleri
├── Kiosk Menü Tanımları (/siramatik/kiosk-menu)
├── Kiosk Tanımları (/siramatik/kiosk)
├── Menü Alt Kanal İşlemleri (/siramatik/kiosk-menu-islem)
└── Kiosk Menü Ataması (/siramatik/kiosk-menu-atama)
```

### Önemli Notlar
- ✅ `IslemAdi` alanı kaldırıldı, yerine `KanalAltAdi` kullanılıyor
- ✅ Menü seçimi manuel yapılıyor (otomatik seçim kaldırıldı)
- ✅ Tüm dropdown'lar `MenuSira` ve alfabetik sıraya göre listeleniyor
- ✅ Repository'lerde `Include(kmi => kmi.KioskMenu)` eklendi
- ✅ Tüm modüllerde inner exception logging aktif

---

**Son Güncelleme:** 20 Kasım 2025  
**Durum:** ✅ Aktif ve Çalışır Durumda  
**Versiyon:** 1.0
