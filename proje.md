# 🏛️ SGK SIRAMATİK SİSTEMİ - KAPSAMLI PROJE TANIMI

**Versiyon:** 1.0  
**Tarih:** 03 Kasım 2025  
**Proje:** SGK İzmir İl Müdürlüğü Sıramatik Otomasyon Sistemi  
**Teknoloji Stack:** .NET 9, Blazor Server, SignalR, SQL Server Express

---

## 📋 İÇİNDEKİLER

1. [Proje Özeti](#1-proje-özeti)
2. [Teknoloji Stack](#2-teknoloji-stack)
3. [Organizasyon Yapısı](#3-organizasyon-yapısı)
4. [Veritabanı Yapısı](#4-veritabanı-yapısı)
5. [İş Akışları](#5-iş-akışları)
6. [Yetkilendirme ve Güvenlik](#6-yetkilendirme-ve-güvenlik)
7. [Real-Time Sistem](#7-real-time-sistem)
8. [Raporlama ve Analytics](#8-raporlama-ve-analytics)
9. [Kullanıcı Arayüzleri](#9-kullanıcı-arayüzleri)
10. [Sistem Gereksinimleri](#10-sistem-gereksinimleri)

---

## 1. PROJE ÖZETİ

### 1.1. Amaç
SGK İzmir İl Müdürlüğü ve bağlı SGM'lerde (Sosyal Güvenlik Merkezleri) vatandaş hizmetlerinin dijital sıra sistemi ile yönetilmesi. Sistem, vatandaşların sıra alması, personellerin sıra çağırması ve yönetimin istatistik ve raporları görüntülemesi için gerçek zamanlı bir platform sağlar.

### 1.2. Kapsam
- **20 SGM** (Menemen, Karşıyaka, Bornova, Konak, vb.)
- **Yaklaşık 500 cihaz** (Kiosk, Banko, TV ekranları)
- **Günlük 50000+ sıra** işlemi
- **7/24 uptime** gereksinimi

### 1.3. Temel Özellikler
✅ Gerçek zamanlı sıra yönetimi  
✅ Kanal bazlı yetkilendirme sistemi  
✅ Uzman/Yardımcı Uzman seviye kontrolü  
✅ TV ekranlarında canlı görüntüleme  
✅ Heartbeat bazlı online tracking  
✅ Kapsamlı raporlama ve analytics  
✅ SignalR ile sıfır gecikmeli iletişim  

---

## 2. TEKNOLOJİ STACK

### 2.1. Backend
| Teknoloji | Versiyon | Kullanım Amacı |
|-----------|----------|----------------|
| .NET | 9.0 | Ana framework |
| ASP.NET Core | 9.0 | Web API ve Blazor Server |
| Entity Framework Core | 9.0 | ORM (Object-Relational Mapping) |
| SignalR | 9.0 | Real-time communication |
| AutoMapper | 13.0 | DTO-Entity mapping |
| FluentValidation | 11.0 | İş kuralları validasyonu |

### 2.2. Frontend
| Teknoloji | Versiyon | Kullanım Amacı |
|-----------|----------|----------------|
| Blazor Server | 9.0 | UI Framework |
| Bootstrap 5 | 5.3 | CSS Framework |
| SignalR Client | 9.0 | Real-time updates |
| JavaScript Interop | Native | Browser API erişimi |

### 2.3. Database
| Teknoloji | Versiyon | Kullanım Amacı |
|-----------|----------|----------------|
| SQL Server Express | 2022 | Ana veritabanı |
| SQL Server Management Studio | 20.x | Veritabanı yönetimi |

### 2.4. Mimari Pattern'ler
- **4 Katmanlı Mimari**: Presentation → Business → Data Access → Business Objects
- **Repository Pattern**: Data access abstraction
- **Unit of Work Pattern**: Transaction management
- **DTO Pattern**: Veri transferi
- **SignalR Hub Pattern**: Real-time messaging

---

## 3. ORGANİZASYON YAPISI

### 3.1. Hiyerarşik Yapı

```
İZMİR İL MÜDÜRLÜĞÜ
├── İl Müdürlüğü (Departman)
│   ├── İdari ve Mali İşler Servisi
│   ├── İnsan Kaynakları Servisi
│   ├── Hukuk Servisi
│   └── Bilgi İşlem Servisi
│
├── Menemen SGM (Departman)
│   ├── Emeklilik Servisi
│   ├── İşveren Servisi
│   ├── Sigortalı Tescil Servisi
│   └── Tahsis Servisi
│
├── Karşıyaka SGM (Departman)
│   ├── Emeklilik Servisi
│   ├── İşveren Servisi
│   └── ...
│
└── ... (18+ SGM daha)
```

### 3.2. Fiziksel Yapı

```
SGM (Departman)
├── A Binası (HizmetBinasi)
│   ├── 1. Kat
│   │   ├── Kiosk 1
│   │   ├── Banko 1, 2, 3
│   │   └── TV Ekranı 1
│   └── 2. Kat
│       ├── Kiosk 2
│       ├── Banko 4, 5, 6
│       └── TV Ekranı 2
│
└── B Binası (HizmetBinasi)
    └── ...
```

### 3.3. Roller ve Yetkiler

| Rol | Sorumluluklar | Erişim Seviyesi |
|-----|---------------|-----------------|
| **Bilgi İşlem** | Sistem ayarları, bina/kanal tanımları, global yetkilendirme | Super Admin |
| **İnsan Kaynakları** | Departman/servis tanımları, personel atamaları | Admin |
| **Müdür Yardımcısı** | SGM bazlı işlem yönetimi, raporlama | SGM Admin |
| **Supervisor** | Servis bazlı kanal planlaması, yetkili kişi ataması | SGM Manager |
| **Yetkili Kişi (Şef)** | Personel-kanal eşleştirmesi, banko ataması, günlük operasyon | Servis Manager |
| **Personel** | Sıra çağırma, işlem yapma | User |

---

## 4. VERİTABANI YAPISI

### 4.1. Tablo Grupları

#### A) Ortak Tablolar (CMN_ Prefix)

**CMN_Iller**
- IlId (PK, int, Identity)
- IlAdi (nvarchar(50))
- Aktiflik (Enum: Aktif/Pasif)
- Audit Kolonları (EklenmeTarihi, DuzenlenmeTarihi, SilindiMi)

**CMN_Ilceler**
- IlceId (PK, int, Identity)
- IlId (FK)
- IlceAdi (nvarchar(100))
- Aktiflik
- Audit Kolonları

**CMN_HizmetBinalari**
- HizmetBinasiId (PK, int, Identity)
- HizmetBinasiAdi (nvarchar(200))
- IlId (FK)
- IlceId (FK)
- Adres (nvarchar(500))
- Aktiflik
- Audit Kolonları

**CMN_Users**
- TcKimlikNo (PK, nvarchar(11))
- KullaniciAdi (nvarchar(50), Unique)
- Email (nvarchar(100), Unique)
- PasswordHash (nvarchar(max))
- TelefonNo (nvarchar(20))
- Rol (Enum: BirimAmiri, Supervisor, YetkiliKisi, Personel)
- Aktiflik
- Audit Kolonları

**CMN_DatabaseLogs**
- LogId (PK, int, Identity)
- Tablo (nvarchar(100))
- IslemTipi (Enum: Insert, Update, Delete)
- EskiDeger (nvarchar(max), JSON)
- YeniDeger (nvarchar(max), JSON)
- KullaniciTcKimlikNo (nvarchar(11))
- IslemZamani (datetime2)

**CMN_LoginLogoutLogs**
- LogId (PK, int, Identity)
- TcKimlikNo (nvarchar(11))
- IslemTipi (Enum: Login, Logout)
- IslemZamani (datetime2)
- IpAdresi (nvarchar(50))
- BrowserInfo (nvarchar(200))

#### B) Personel Tabloları (PER_ Prefix)

**PER_Personeller**
- TcKimlikNo (PK, nvarchar(11))
- SicilNo (int, Unique)
- AdSoyad (nvarchar(200))
- Email (nvarchar(100))
- CepTelefonu (nvarchar(20))
- DepartmanId (FK)
- ServisId (FK)
- UnvanId (FK)
- HizmetBinasiId (FK)
- PersonelAktiflik (Enum: Aktif, Pasif, İzinli)
- Audit Kolonları

**PER_Departmanlar**
- DepartmanId (PK, int, Identity)
- DepartmanAdi (nvarchar(200), Unique)
- DepartmanTip (Enum: IlMudurlugu, SGM)
- DepartmanAktiflik
- Audit Kolonları

**PER_Servisler**
- ServisId (PK, int, Identity)
- DepartmanId (FK)
- ServisAdi (nvarchar(200))
- ServisAktiflik
- Audit Kolonları

**PER_Unvanlar**
- UnvanId (PK, int, Identity)
- UnvanAdi (nvarchar(100))
- UnvanSeviye (int) - Hiyerarşik sıra
- Aktiflik
- Audit Kolonları

**PER_PersonelCocuklari**
- PersonelCocukId (PK, int, Identity)
- TcKimlikNo (FK)
- CocukAdi (nvarchar(200))
- DogumTarihi (datetime2)
- Cinsiyet (Enum: Erkek, Kiz)
- Audit Kolonları

#### C) Sıramatik Tabloları (SIR_ Prefix)

**SIR_Kanallar**
- KanalId (PK, int, Identity)
- KanalAdi (nvarchar(200))
- BaslangicNumarasi (int) - Örn: 1001
- BitisNumarasi (int) - Örn: 1999
- KanalSirasi (int) - Kiosk'ta gösterim sırası
- Aktiflik
- Audit Kolonları

**SIR_KanallarAlt**
- KanalAltId (PK, int, Identity)
- KanalId (FK)
- KanalAltAdi (nvarchar(200))
- KanalAltSirasi (int) - Alt kanal gösterim sırası
- Aktiflik
- Audit Kolonları

**SIR_KanalPersonelleri**
- KanalPersonelId (PK, int, Identity)
- KanalAltId (FK)
- TcKimlikNo (FK - Personel)
- YetkinlikSeviyesi (Enum: YardimciUzman, Uzman)
- Aktiflik
- Audit Kolonları
- **UNIQUE INDEX**: (KanalAltId, TcKimlikNo) - Bir personel aynı kanala tek kayıtla yetkili

**SIR_Bankolar**
- BankoId (PK, int, Identity)
- BankoAdi (nvarchar(100))
- HizmetBinasiId (FK)
- AtanmisTcKimlikNo (FK - Personel, Nullable) - O anda kim çalışıyor
- AktifSiraId (FK - Siralar, Nullable) - Şu anda hangi sıra işlemde
- BankoDurum (Enum: Aktif, Pasif, Bakimda)
- Aktiflik
- Audit Kolonları

**SIR_Tvler**
- TvId (PK, int, Identity)
- TvAdi (nvarchar(100))
- HizmetBinasiId (FK)
- TvDurum (Enum: Aktif, Pasif)
- Aktiflik
- Audit Kolonları

**SIR_TvBankolari**
- TvBankoId (PK, int, Identity)
- TvId (FK)
- BankoId (FK)
- Audit Kolonları
- **UNIQUE INDEX**: (TvId, BankoId)

**SIR_Siralar**
- SiraId (PK, int, Identity) - Internal unique ID
- SiraNo (int) - Vatandaşın gördüğü numara (1105, 2045, vb.)
- KanalAltId (FK)
- HizmetBinasiId (FK)
- SiraDurum (Enum: Beklemede, Cagrildi, Islemde, Tamamlandi, Iptal)
- CagiranBankoId (FK, Nullable)
- SiraAlisZamani (datetime2)
- CagrilmaZamani (datetime2, Nullable)
- IslemBaslamaZamani (datetime2, Nullable)
- IslemBitisZamani (datetime2, Nullable)
- IptalNedeni (nvarchar(500), Nullable)
- Audit Kolonları
- **INDEX**: (HizmetBinasiId, SiraDurum, SiraAlisZamani) - Performans için kritik

**SIR_HubConnections**
- HubConnectionId (PK, int, Identity)
- TcKimlikNo (nvarchar(11), Unique) - Aktif kullanıcı
- ConnectionId (nvarchar(100)) - SignalR connection ID
- ConnectionStatus (Enum: Connected, Disconnected, Away)
- LastHeartbeat (datetime2) - Son heartbeat zamanı
- OnlineStartTime (datetime2) - Online olmaya başlama
- TotalOnlineSeconds (int) - Bugünkü toplam online süresi
- IslemZamani (datetime2) - Son aktivite
- Audit Kolonları
- **INDEX**: (TcKimlikNo, ConnectionStatus)

**SIR_HubTvConnections**
- HubTvConnectionId (PK, int, Identity)
- TvId (FK)
- ConnectionId (nvarchar(100))
- ConnectionStatus (Enum: Connected, Disconnected)
- IslemZamani (datetime2)
- Audit Kolonları

**SIR_OnlineHistory**
- HistoryId (PK, int, Identity)
- TcKimlikNo (nvarchar(11))
- SessionStartTime (datetime2) - Session başlangıç
- SessionEndTime (datetime2, Nullable) - Session bitiş
- DurationSeconds (int) - Session süresi
- HizmetBinasiId (FK)
- BankoId (FK, Nullable)
- Tarih (date) - Raporlama için
- **INDEX**: (TcKimlikNo, Tarih)

**SIR_SistemAyarlari**
- AyarId (PK, int, Identity)
- AyarAdi (nvarchar(100), Unique)
- AyarDegeri (nvarchar(500))
- AyarTipi (Enum: String, Integer, Boolean, JSON)
- Aciklama (nvarchar(max))
- Audit Kolonları

Örnek Kayıtlar:
- `UzmanEsikDegeri` = `5` (Uzmanın önünde kaç sıra olunca yrd. uzman devreye girer)
- `HeartbeatInterval` = `30` (Saniye cinsinden)
- `CalismaBaslangic` = `08:30`
- `CalismaBitis` = `17:30`
- `SiraNumaraSifirlamaZamani` = `08:30`

#### D) PDKS Tabloları (PDK_ Prefix)

*Not: PDKS (Personel Devam Kontrol Sistemi) modülü şimdilik kapsam dışı, ileride eklenebilir.*

#### E) Eshot Tabloları (ESH_ Prefix)

*Not: Eshot (Ulaşım) modülü şimdilik kapsam dışı, ileride eklenebilir.*

### 4.2. Enum Tanımları

```
Aktiflik: Aktif, Pasif
PersonelAktiflik: Aktif, Pasif, Izinli
BankoDurum: Aktif, Pasif, Bakimda
SiraDurum: Beklemede, Cagrildi, Islemde, Tamamlandi, Iptal
YetkinlikSeviyesi: YardimciUzman (1), Uzman (2)
ConnectionStatus: Connected, Disconnected, Away
DepartmanTip: IlMudurlugu, SGM
KullaniciRol: BirimAmiri, Supervisor, YetkiliKisi, Personel
IslemTipi: Insert, Update, Delete
LoginIslemTipi: Login, Logout
```

### 4.3. Audit Kolonları (Tüm Tablolarda)

```
EklenmeTarihi (datetime2, DEFAULT GETDATE())
DuzenlenmeTarihi (datetime2, DEFAULT GETDATE())
EkleyenKullanici (nvarchar(11), Nullable)
DuzenleyenKullanici (nvarchar(11), Nullable)
SilindiMi (bit, DEFAULT 0)
SilinmeTarihi (datetime2, Nullable)
SilenKullanici (nvarchar(11), Nullable)
```

### 4.4. Indexes ve Constraints

**Performance Critical Indexes:**
```
IX_SIR_Siralar_Bina_Durum_Zaman: (HizmetBinasiId, SiraDurum, SiraAlisZamani)
IX_SIR_KanalPersonelleri_Kanal_Personel: (KanalAltId, TcKimlikNo)
IX_SIR_HubConnections_Tc_Status: (TcKimlikNo, ConnectionStatus)
IX_SIR_OnlineHistory_Tc_Tarih: (TcKimlikNo, Tarih)
IX_PER_Personeller_Departman: (DepartmanId, PersonelAktiflik)
```

**Unique Constraints:**
```
UQ_CMN_Users_Email
UQ_CMN_Users_KullaniciAdi
UQ_PER_Personeller_SicilNo
UQ_SIR_KanalPersonelleri_Kanal_Personel
UQ_SIR_TvBankolari_Tv_Banko
```

**Foreign Key Cascade Rules:**
- Silme işlemleri: `ON DELETE RESTRICT` (Veri bütünlüğü için)
- Güncelleme işlemleri: `ON UPDATE CASCADE`

---

## 5. İŞ AKIŞLARI

### 5.1. Sıra Alma İş Akışı (Kiosk)

```
ADIM 1: Vatandaş Kiosk'a Gelir
─────────────────────────────
- Ekrana dokunur
- Sistem kanalları gösterir (SIR_Kanallar tablosundan)

ADIM 2: Kanal Seçimi
────────────────────
- Vatandaş kanal seçer (Örn: "Emeklilik İşlemleri")
- Sistem alt kanalları gösterir (SIR_KanallarAlt tablosundan)

ADIM 3: Alt Kanal Seçimi
─────────────────────────
- Vatandaş alt kanal seçer (Örn: "Yaşlılık Aylığı")
- Sistem sıra numarası üretir:
  * Kanal numara aralığından next numarayı al
  * Örn: Emeklilik 1001-1999, son verilen 1104 ise → 1105 ver

ADIM 4: Sıra Kaydı Oluştur
───────────────────────────
INSERT INTO SIR_Siralar:
- SiraNo: 1105
- KanalAltId: (Seçilen alt kanal)
- HizmetBinasiId: (Kiosk'un bulunduğu bina)
- SiraDurum: Beklemede
- SiraAlisZamani: GETDATE()

ADIM 5: Fiş Yazdır
──────────────────
- Fiş içeriği:
  * Sıra No: 1105
  * İşlem: Yaşlılık Aylığı Başvurusu
  * Tarih/Saat: 03.11.2025 09:15
  * Hangi kata gitmelisiniz: 2. Kat - Emeklilik Servisi

ADIM 6: SignalR Broadcast
──────────────────────────
Event: "OnQueueCreated"
Hedef: Kanal bazlı gruplar
- "Kanal-{KanalAltId}-Uzman" grubuna
- "Kanal-{KanalAltId}-YardimciUzman" grubuna (eğer uzman yoksa)
- "Bina-{BinaId}-TV" grubuna (TV ekranları için)

Payload:
{
  SiraId: 123456,
  SiraNo: 1105,
  KanalAltId: 5,
  KanalAltAdi: "Yaşlılık Aylığı",
  BinaId: 1,
  Zaman: "2025-11-03T09:15:00"
}
```

### 5.2. Personel Login İş Akışı

```
ADIM 1: Giriş Sayfası
─────────────────────
- Personel TC Kimlik No ve Şifre girer
- Sistem CMN_Users tablosunda doğrular

ADIM 2: Yetki ve Bina Kontrolü
───────────────────────────────
- Personelin hangi departman/serviste çalıştığını al (PER_Personeller)
- Personelin hangi binada görevli olduğunu al (HizmetBinasiId)
- Personelin kanal yetkilerini al (SIR_KanalPersonelleri)

ADIM 3: SignalR Bağlantısı Başlat
──────────────────────────────────
- Hub OnConnectedAsync tetiklenir
- Connection ID al
- Hub Connections tablosuna kaydet:

INSERT/UPDATE SIR_HubConnections:
- TcKimlikNo: (Personel TC)
- ConnectionId: (SignalR connection ID)
- ConnectionStatus: Connected
- LastHeartbeat: GETDATE()
- OnlineStartTime: GETDATE()
- IslemZamani: GETDATE()

ADIM 4: SignalR Group Membership
─────────────────────────────────
Personelin yetkili olduğu her kanal için:

SELECT KanalAltId, YetkinlikSeviyesi 
FROM SIR_KanalPersonelleri 
WHERE TcKimlikNo = @tc AND Aktiflik = 'Aktif'

Her kanal için gruba ekle:
- Groups.Add("Kanal-{KanalAltId}-{YetkinlikSeviyesi}")
- Groups.Add("Bina-{BinaId}")

Örnek:
Ahmet (Uzman - Yaşlılık, Uzman - Emeklilik):
→ "Kanal-5-Uzman"
→ "Kanal-8-Uzman"
→ "Bina-1"

ADIM 5: İlk Sıra Listesini Yükle
─────────────────────────────────
Backend'den bekleyen sıraları getir:
- Personelin yetkili olduğu kanallar
- Bina bazlı filtreleme
- Yetkinlik seviyesi önceliklendirmesi

ADIM 6: Heartbeat Timer Başlat
───────────────────────────────
Client-side (Blazor):
- Her 30 saniyede bir Hub.Invoke("Heartbeat")
- Kullanıcı habersiz, arka planda
```

### 5.3. Sıra Çağırma İş Akışı (Banko)

```
ADIM 1: "Sonraki Sıra" Butonuna Bas
────────────────────────────────────
Personel (Ahmet) butona basar

ADIM 2: Client → SignalR Hub
─────────────────────────────
await HubConnection.InvokeAsync("CallNextQueue", bankoId)

ADIM 3: Hub → Business Service
───────────────────────────────
Hub method tetiklenir:
- User TcKimlikNo al
- Yetki kontrolü yap
- Service çağır: CallNextQueueAsync(bankoId, tcKimlikNo)

ADIM 4: Business Logic
──────────────────────
A) Personelin yetkilerini ve seviyelerini al:
   - Uzman olduğu kanallar: [5, 8]
   - Yrd. Uzman olduğu kanallar: [3]

B) Bekleyen sıraları al (Öncelik sırasıyla):
   
   Query:
   SELECT s.*, kp.YetkinlikSeviyesi
   FROM SIR_Siralar s
   JOIN SIR_KanalPersonelleri kp ON s.KanalAltId = kp.KanalAltId
   WHERE kp.TcKimlikNo = @tc
     AND s.HizmetBinasiId = @binaId
     AND s.SiraDurum = 'Beklemede'
   ORDER BY 
     kp.YetkinlikSeviyesi DESC,  -- Önce uzman (2), sonra yrd (1)
     s.SiraNo ASC                -- Sonra en küçük numara
   
   Sonuç:
   1105 (Yaşlılık, Uzman)   ← EN ÖNCELİKLİ
   1109 (Emeklilik, Uzman)
   1112 (Emeklilik, Uzman)
   1108 (Malullük, YrdUzman)

C) İlk sırayı al: 1105

ADIM 5: Transaction Başlat
───────────────────────────
BEGIN TRANSACTION

D) Sıra durumu kontrolü:
   SELECT * FROM SIR_Siralar WITH (UPDLOCK, ROWLOCK)
   WHERE SiraId = 1105
   
   IF Durum != 'Beklemede':
     ROLLBACK
     RETURN Error("Sıra artık müsait değil")

E) Banko durumu kontrolü:
   SELECT * FROM SIR_Bankolar WHERE BankoId = @bankoId
   
   IF AktifSiraId IS NOT NULL:
     ROLLBACK
     RETURN Error("Zaten aktif sıranız var")

F) Güncelleme yap:
   UPDATE SIR_Siralar SET
     SiraDurum = 'Cagrildi',
     CagiranBankoId = @bankoId,
     CagrilmaZamani = GETDATE()
   WHERE SiraId = 1105
   
   UPDATE SIR_Bankolar SET
     AktifSiraId = 1105,
     BankoDurum = 'Aktif'
   WHERE BankoId = @bankoId

COMMIT TRANSACTION

ADIM 6: SignalR Broadcast
──────────────────────────
Event: "OnQueueCalled"

Hedef 1: Kanal grubuna (diğer personeller için)
await Clients.Group("Kanal-5-Uzman")
  .SendAsync("OnQueueCalled", {
    SiraId: 1105,
    SiraNo: 1105,
    BankoId: 3,
    BankoAdi: "3 Nolu Banko",
    Zaman: "2025-11-03T09:20:00"
  })

Hedef 2: TV ekranlarına
await Clients.Group("Bina-1-TV")
  .SendAsync("OnQueueCalledForDisplay", {
    SiraNo: 1105,
    BankoAdi: "3 NOLU BANKO",
    KanalAltAdi: "Yaşlılık Aylığı",
    Zaman: "09:20"
  })

ADIM 7: Client-Side Event Handler
──────────────────────────────────
A) Ahmet'in ekranı (çağıran):
   - 1105 listeden "Aktif Sıra" bölümüne taşınır
   - Toast: "Sıra 1105 çağrıldı"
   - Butonlar: [Tamamla] [Sonraki] [İptal]

B) Mehmet'in ekranı (diğer personel):
   - 1105 listeden SİLİNİR
   - (Çünkü başkası çağırdı)

C) TV ekranları:
   - En üste eklenir: "1105 → 3 NOLU BANKO"
   - Ses çalar: "Bin yüz beş numaralı sıra, üç nolu bankoya"
   - 10 sıra gösterilir, fazlası kaybolur
```

### 5.4. İşlem Tamamlama İş Akışı

```
SENARYO A: "Tamamla" Butonu
────────────────────────────
Ahmet "Tamamla" butonuna basar:

1. Hub method: CompleteQueue(siraId, bankoId)

2. Business Logic:
   UPDATE SIR_Siralar SET
     SiraDurum = 'Tamamlandi',
     IslemBitisZamani = GETDATE()
   WHERE SiraId = @siraId
   
   UPDATE SIR_Bankolar SET
     AktifSiraId = NULL,
     BankoDurum = 'Aktif'
   WHERE BankoId = @bankoId

3. SignalR Broadcast:
   Event: "OnQueueCompleted"
   Hedef: Sadece Ahmet'e (Caller)
   
4. UI Güncelleme:
   - Aktif sıra bölümü temizlenir
   - Banko "Boş" durumuna döner
   - "Sonraki Sıra" butonu tekrar aktif


SENARYO B: "Sonraki" Butonu
────────────────────────────
Ahmet "Sonraki" butonuna basar:

1. Hub method: CompleteAndCallNextQueue(bankoId)

2. Business Logic (Transaction içinde):
   A) Mevcut sırayı tamamla (yukarıdaki gibi)
   B) Sonraki sırayı çağır (CallNextQueue logic'i)
   
3. SignalR Broadcast:
   - "OnQueueCompleted" (mevcut sıra için)
   - "OnQueueCalled" (yeni sıra için)

4. UI Güncelleme:
   - Eski sıra tamamlandı
   - Yeni sıra aktif oldu
   - Tek işlemde geçiş!


SENARYO C: "İptal" Butonu
──────────────────────────
Ahmet "İptal" butonuna basar:
(Vatandaş gelmedi veya vazgeçti)

1. Modal açılır: "İptal nedeni girin"

2. Hub method: CancelQueue(siraId, iptalNedeni)

3. Business Logic:
   UPDATE SIR_Siralar SET
     SiraDurum = 'Iptal',
     IptalNedeni = @neden,
     IslemBitisZamani = GETDATE()
   WHERE SiraId = @siraId
   
   UPDATE SIR_Bankolar SET
     AktifSiraId = NULL
   WHERE BankoId = @bankoId

4. Log Kaydet:
   INSERT INTO CMN_DatabaseLogs
   (Tablo, IslemTipi, YeniDeger, KullaniciTcKimlikNo, IslemZamani)

5. UI Güncelleme:
   - Banko boş duruma döner
   - İptal edilen sıra log'a düşer
```

### 5.5. Logout / Mola İş Akışı

```
Personel Logout Yapar
─────────────────────
1. Blazor Component Dispose:
   await HubConnection.StopAsync()

2. Hub OnDisconnectedAsync:
   A) Online süresini hesapla:
      Duration = NOW - OnlineStartTime
   
   B) History'e kaydet:
      INSERT INTO SIR_OnlineHistory
      (TcKimlikNo, SessionStartTime, SessionEndTime, 
       DurationSeconds, HizmetBinasiId, BankoId, Tarih)
   
   C) Connection'ı güncelle:
      UPDATE SIR_HubConnections SET
        ConnectionStatus = 'Disconnected',
        IslemZamani = GETDATE()
      WHERE TcKimlikNo = @tc
   
   D) Eğer bankoda sıra varsa:
      UPDATE SIR_Bankolar SET
        AtanmisTcKimlikNo = NULL,
        BankoDurum = 'Pasif'
      WHERE AtanmisTcKimlikNo = @tc

3. SignalR Groups'tan çıkar:
   - Tüm kanal gruplarından
   - Bina grubundan

4. Login Log:
   INSERT INTO CMN_LoginLogoutLogs
   (TcKimlikNo, IslemTipi, IslemZamani, IpAdresi)
   VALUES (@tc, 'Logout', GETDATE(), @ip)
```

### 5.6. Heartbeat İş Akışı

```
Her 30 Saniyede (Client-Side)
──────────────────────────────
1. Blazor Timer tetiklenir

2. Hub method çağır:
   await HubConnection.InvokeAsync("Heartbeat")

3. Hub Heartbeat Method:
   UPDATE SIR_HubConnections SET
     LastHeartbeat = GETDATE(),
     ConnectionStatus = 'Connected'
   WHERE TcKimlikNo = @tc

4. Hiçbir broadcast yok (silent operation)


Online/Offline Kontrolü (Backend Job - Her 1 dakikada)
─────────────────────────────────────────────────────
1. Tüm connection'ları kontrol et:
   
   UPDATE SIR_HubConnections SET
     ConnectionStatus = CASE
       WHEN DATEDIFF(SECOND, LastHeartbeat, GETDATE()) < 60 THEN 'Connected'
       WHEN DATEDIFF(SECOND, LastHeartbeat, GETDATE()) < 300 THEN 'Away'
       ELSE 'Disconnected'
     END

2. Disconnected olanları temizle:
   - 10+ dakika heartbeat yok
   - Session'ı kapat
   - History'e kaydet


Online Personel Listesi (Query)
────────────────────────────────
SELECT p.TcKimlikNo, p.AdSoyad, 
       hc.ConnectionStatus, hc.LastHeartbeat,
       DATEDIFF(SECOND, hc.OnlineStartTime, GETDATE()) AS OnlineSeconds
FROM PER_Personeller p
JOIN SIR_HubConnections hc ON p.TcKimlikNo = hc.TcKimlikNo
WHERE hc.ConnectionStatus IN ('Connected', 'Away')
ORDER BY OnlineSeconds DESC
```

---

## 6. YETKİLENDİRME VE GÜVENLİK

### 6.1. Kanal Bazlı Yetkilendirme

**Yetkilendirme Matrisi:**
```
Personel → Kanal → Yetkinlik Seviyesi

Örnek:
Ahmet:
  - Yaşlılık Aylığı: Uzman
  - Emeklilik Hesap: Uzman
  - Malullük: Yardımcı Uzman

Mehmet:
  - Yaşlılık Aylığı: Uzman
  - Emeklilik Hesap: Uzman
  - Malullük: YOK
```

**Yetki Kontrolü (Her İşlemde):**
```
Method: IsAuthorizedForChannel(tcKimlikNo, kanalAltId)

Query:
SELECT COUNT(*) FROM SIR_KanalPersonelleri
WHERE TcKimlikNo = @tc 
  AND KanalAltId = @kanal
  AND Aktiflik = 'Aktif'

Return: True/False
```

### 6.2. Uzman/Yardımcı Uzman Sıralama

**Sıra Dağıtım Algoritması:**

```
FONKSIYON: GetQueuesByPriority(tcKimlikNo, binaId)

1. Personelin yetkilerini al:
   SELECT KanalAltId, YetkinlikSeviyesi
   FROM SIR_KanalPersonelleri
   WHERE TcKimlikNo = @tc

2. Her kanal için sıraları çek:
   
   FOR EACH kanal IN yetkiler:
     
     IF kanal.YetkinlikSeviyesi == 'Uzman':
       → Uzman sıralarına ekle (öncelikli)
     
     ELSE IF kanal.YetkinlikSeviyesi == 'YardimciUzman':
       → Bu kanalda başka uzman var mı kontrol et
       
       uzmanSayisi = SELECT COUNT(*) 
                     FROM SIR_KanalPersonelleri kp
                     JOIN SIR_HubConnections hc ON kp.TcKimlikNo = hc.TcKimlikNo
                     WHERE kp.KanalAltId = @kanal
                       AND kp.YetkinlikSeviyesi = 'Uzman'
                       AND hc.ConnectionStatus = 'Connected'
       
       IF uzmanSayisi == 0:
         → Yrd. uzman sıralarına ekle
       ELSE:
         → Uzmanların kuyruk uzunluğuna bak
         uzmanKuyrukUzunlugu = ... (hesapla)
         
         IF uzmanKuyrukUzunlugu > EsikDegeri:
           → Yrd. uzman sıralarına da ekle (iş yükü dengesi)

3. Sıraları birleştir ve sırala:
   ORDER BY YetkinlikSeviyesi DESC, SiraNo ASC
   
   Sonuç:
   [Uzman sıraları önce]
   [Yrd. Uzman sıraları sonra]

RETURN sıralar
```

**Eşik Değeri Sistemi:**
```
SIR_SistemAyarlari tablosundan:
UzmanEsikDegeri = 5

Anlamı:
Uzmanın önünde 5+ sıra varsa,
yrd. uzman da aynı kanaldaki sıraları görebilir.
```

### 6.3. Authentication & Authorization

**JWT Token Bazlı (Cookie Authentication):**
```
Login Başarılı:
1. Claims oluştur:
   - TcKimlikNo
   - AdSoyad
   - Email
   - Rol
   - DepartmanId
   - ServisId
   - HizmetBinasiId

2. Cookie yaz:
   - HttpOnly: true
   - Secure: true (HTTPS)
   - SameSite: Strict
   - Expiry: 8 saat

3. Her request'te:
   - Cookie validate et
   - Claims bilgilerini al
   - Yetki kontrolü yap
```

**Role-Based Access Control (RBAC):**
```
Rol Hiyerarşisi:
BirimAmiri > Supervisor > YetkiliKisi > Personel

Sayfa Yetkileri:
/admin → Sadece BirimAmiri
/supervisor → BirimAmiri, Supervisor
/banko → Tüm roller
/kiosk → Public (yetki gerekmez)
/tv → Public
```

### 6.4. Güvenlik Önlemleri

**A) SQL Injection Koruması:**
- Entity Framework Parameterized Queries
- Hiçbir yerde raw SQL kullanılmaz (gerekmedikçe)
- Stored Procedure'ler için parametre validasyonu

**B) XSS Koruması:**
- Blazor otomatik encoding
- JavaScript Interop'ta sanitization
- User input validation

**C) CSRF Koruması:**
- Blazor built-in anti-forgery token
- SignalR connection token

**D) Rate Limiting:**
- IP bazlı: 100 request/dakika
- User bazlı: 1000 request/saat
- Hub method bazlı throttling

**E) Logging & Audit:**
- Tüm CRUD işlemleri log'lanır (CMN_DatabaseLogs)
- Login/Logout kayıtları (CMN_LoginLogoutLogs)
- Critical işlemler email alert

---

## 7. REAL-TIME SİSTEM

### 7.1. SignalR Hub Mimarisi

**Hub Sınıfı: SiramatikHub**

**Methods:**
```
[Authorize]
public class SiramatikHub : Hub
{
  // Heartbeat
  Task Heartbeat()
  
  // Sıra işlemleri
  Task CallNextQueue(int bankoId)
  Task CompleteQueue(int siraId, int bankoId)
  Task CompleteAndCallNextQueue(int bankoId)
  Task CancelQueue(int siraId, string iptalNedeni)
  
  // Connection management
  Task OnConnectedAsync()
  Task OnDisconnectedAsync(Exception exception)
}
```

**Client Events:**
```
// Personel ekranları için
OnQueueCreated(QueueCreatedEvent)
OnQueueCalled(QueueCalledEvent)
OnQueueCompleted(QueueCompletedEvent)
OnQueueCancelled(QueueCancelledEvent)

// TV ekranları için
OnQueueCalledForDisplay(TvDisplayEvent)

// Status updates
OnPersonelStatusChanged(PersonelStatusEvent)
```

### 7.2. SignalR Groups Organizasyonu

**Group Naming Convention:**
```
Personeller için:
- "Kanal-{KanalAltId}-Uzman"
- "Kanal-{KanalAltId}-YardimciUzman"
- "Bina-{BinaId}"

TV'ler için:
- "Bina-{BinaId}-TV"

Admin/Raporlama için:
- "Admin-{DepartmanId}"
```

**Group Membership Yönetimi:**
```
Login (OnConnectedAsync):
→ Personelin yetkilerine göre gruplara ekle
→ Groups.AddToGroupAsync(connectionId, groupName)

Logout (OnDisconnectedAsync):
→ Otomatik gruplardan çıkar (SignalR built-in)

Yetki Değişikliği:
→ Eski gruplardan çıkar
→ Yeni gruplara ekle
→ Yeniden login gerekebilir
```

### 7.3. Connection Scalability

**Tek Sunucu Yapısı:**
- Maksimum 500 aktif bağlantı
- Blazor Server + SignalR optimize edilmiş
- Memory: 8GB yeterli
- CPU: 4 core yeterli

**Connection Pooling:**
- Default Blazor Server settings
- Long-polling fallback (WebSocket desteklemeyen tarayıcılar için)

**Reconnection Strategy:**
```
HubConnectionBuilder:
- WithAutomaticReconnect([0s, 2s, 5s, 10s])
- KeepAliveInterval: 15 saniye
- ServerTimeout: 30 saniye
```

---

## 8. RAPORLAMA VE ANALYTICS

### 8.1. Gerçek Zamanlı Dashboard

**Admin Dashboard (Her SGM için):**
```
KPIs:
- Şu anda bekleyen sıra sayısı
- Online personel sayısı
- Bugün verilen toplam sıra
- Bugün tamamlanan sıra
- Ortalama bekleme süresi (dakika)
- Ortalama işlem süresi (dakika)
- İptal edilen sıra sayısı

Grafikler:
- Saatlik sıra dağılımı (çizgi grafik)
- Kanal bazlı dağılım (pasta grafik)
- Personel performansı (bar chart)
```

### 8.2. Raporlar

**A) Günlük Sıra Raporu**
```
Parametreler: Tarih, BinaId

Kolonlar:
- Sıra No
- Kanal/Alt Kanal
- Sıra Alış Zamanı
- Çağrılma Zamanı
- Tamamlanma Zamanı
- Bekleme Süresi (dk)
- İşlem Süresi (dk)
- Çağıran Personel
- Durum

Export: Excel, PDF
```

**B) Personel Performans Raporu**
```
Parametreler: Tarih Aralığı, DepartmanId

Kolonlar:
- Personel Adı
- Online Süresi (saat)
- Tamamlanan Sıra Sayısı
- Ortalama İşlem Süresi
- İptal Edilen Sıra
- Verimlilik Skoru

Hesaplama:
VerimlilikSkoru = (TamamlananSira * 100) / (OnlineSuresi / OrtalamaIslemSuresi)
```

**C) Kanal Kullanım Raporu**
```
Parametreler: Tarih Aralığı, BinaId

Kolonlar:
- Kanal Adı
- Alt Kanal Adı
- Toplam Sıra
- Tamamlanan Sıra
- Ortalama Bekleme
- Ortalama İşlem Süresi
- Yoğunluk Saatleri

Grafik: Zaman bazlı dağılım (heatmap)
```

**D) Online/Offline Raporu**
```
Parametreler: Tarih, DepartmanId

SIR_OnlineHistory tablosundan:
- Her personel için session listesi
- Toplam online süresi
- Mola süreleri
- En uzun session
- Ortalama session süresi
```

### 8.3. İstatistikler

**Veritabanı Query'leri:**

```sql
-- Bugünkü özet
SELECT 
  COUNT(*) AS ToplamSira,
  SUM(CASE WHEN SiraDurum = 'Tamamlandi' THEN 1 ELSE 0 END) AS Tamamlanan,
  SUM(CASE WHEN SiraDurum = 'Iptal' THEN 1 ELSE 0 END) AS Iptal,
  AVG(DATEDIFF(MINUTE, SiraAlisZamani, CagrilmaZamani)) AS OrtBekleme,
  AVG(DATEDIFF(MINUTE, CagrilmaZamani, IslemBitisZamani)) AS OrtIslem
FROM SIR_Siralar
WHERE CAST(SiraAlisZamani AS DATE) = CAST(GETDATE() AS DATE)
  AND HizmetBinasiId = @binaId

-- En çok sıra alan personel (Top 10)
SELECT TOP 10
  p.AdSoyad,
  COUNT(*) AS SiraSayisi,
  AVG(DATEDIFF(MINUTE, s.CagrilmaZamani, s.IslemBitisZamani)) AS OrtIslemSuresi
FROM SIR_Siralar s
JOIN SIR_Bankolar b ON s.CagiranBankoId = b.BankoId
JOIN PER_Personeller p ON b.AtanmisTcKimlikNo = p.TcKimlikNo
WHERE CAST(s.SiraAlisZamani AS DATE) = CAST(GETDATE() AS DATE)
  AND s.SiraDurum = 'Tamamlandi'
GROUP BY p.TcKimlikNo, p.AdSoyad
ORDER BY SiraSayisi DESC

-- Saatlik yoğunluk
SELECT 
  DATEPART(HOUR, SiraAlisZamani) AS Saat,
  COUNT(*) AS SiraSayisi
FROM SIR_Siralar
WHERE CAST(SiraAlisZamani AS DATE) = CAST(GETDATE() AS DATE)
GROUP BY DATEPART(HOUR, SiraAlisZamani)
ORDER BY Saat
```

---

## 9. KULLANICI ARAYÜZLER

### 9.1. Kiosk Arayüzü (Blazor Component)

**Sayfa:** /kiosk

**Layout:**
```
┌──────────────────────────────────────┐
│  SGK İZMİR - MENEMEN SGM             │
├──────────────────────────────────────┤
│                                      │
│  🏛️ HOŞGELDİNİZ                     │
│                                      │
│  Lütfen işlem türünüzü seçiniz:     │
│                                      │
│  ┌────────────────────┐              │
│  │ 📋 EMEKLİLİK       │              │
│  │    İŞLEMLERİ       │              │
│  └────────────────────┘              │
│                                      │
│  ┌────────────────────┐              │
│  │ 💼 İŞVEREN          │              │
│  │    İŞLEMLERİ       │              │
│  └────────────────────┘              │
│                                      │
│  ┌────────────────────┐              │
│  │ 👤 SİGORTALI        │              │
│  │    İŞLEMLERİ       │              │
│  └────────────────────┘              │
│                                      │
└──────────────────────────────────────┘

(Kanal seçilince alt kanallar gösterilir)

┌──────────────────────────────────────┐
│  ← GERİ                              │
├──────────────────────────────────────┤
│  EMEKLİLİK İŞLEMLERİ                 │
│                                      │
│  ┌────────────────────┐              │
│  │ Yaşlılık Aylığı    │              │
│  │ Başvurusu          │              │
│  └────────────────────┘              │
│                                      │
│  ┌────────────────────┐              │
│  │ Malullük           │              │
│  │ Başvurusu          │              │
│  └────────────────────┘              │
│                                      │
│  ┌────────────────────┐              │
│  │ Aylık Bağlama      │              │
│  │ Durumu Sorgulama   │              │
│  └────────────────────┘              │
│                                      │
└──────────────────────────────────────┘

(Alt kanal seçilince sıra verilir ve fiş yazdırılır)
```

**Özellikler:**
- Tam ekran mod
- Touch-friendly (büyük butonlar)
- Timeout: 30 saniye (işlem yapılmazsa ana ekrana dön)
- Ses feedback
- Çoklu dil desteği (TR/EN/AR) - opsiyonel

### 9.2. Banko Arayüzü (Blazor Page)

**Sayfa:** /banko

**Layout:**
```
┌────────────────────────────────────────────────────────┐
│  BANKO 3 - AHMET BEY              🟢 Online  [Logout] │
├────────────────────────────────────────────────────────┤
│                                                        │
│  🔴 AKTİF SIRA                                         │
│  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━  │
│                                                        │
│      📢 Sıra No: 1105                                 │
│      👤 İşlem: Yaşlılık Aylığı Başvurusu              │
│      ⏱️  Süre: 03:45                                   │
│                                                        │
│      [  ✅ TAMAMLA  ]  [  ➡️ SONRAKİ  ]  [  ❌ İPTAL  ] │
│                                                        │
├────────────────────────────────────────────────────────┤
│  📋 BEKLİYOR (İlk 10 Sıra)                            │
│  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━  │
│                                                        │
│  ⭐ ÖNCELİKLİ (Uzman)                                  │
│  🔵 1109 - Yaşlılık Aylığı        08:35               │
│  🔵 1111 - Emeklilik Hesap        08:42               │
│  🔵 1112 - Emeklilik Hesap        08:45               │
│                                                        │
│  ─────────────────────────────────────────────────    │
│                                                        │
│  ⚪ DİĞER (Yrd. Uzman)                                │
│  🟡 1108 - Malullük               08:30               │
│                                                        │
│  [ 🔄 YENİLE ]                                        │
│                                                        │
└────────────────────────────────────────────────────────┘
```

**Özellikler:**
- Real-time liste güncelleme (SignalR)
- Ses bildirimi (yeni sıra geldiğinde)
- Sıra sayısı badge
- Renk kodlaması (uzman/yrd.uzman)
- Responsive design (tablet/desktop)

### 9.3. TV Ekranı Arayüzü (Blazor Page)

**Sayfa:** /tv

**Layout:**
```
┌─────────────────────────────────────────────────────────────┐
│  SGK İZMİR - MENEMEN SGM                    14:35:22       │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  🔴 SON ÇAĞRILAN                                            │
│  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━  │
│                                                             │
│           📢  1105  →  3 NOLU BANKO                        │
│                                                             │
├─────────────────────────────────────────────────────────────┤
│  📋 SON ÇAĞRILANLAR                                         │
│  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━  │
│                                                             │
│  1104  →  Banko 2             14:32                        │
│  1103  →  Banko 1             14:30                        │
│  1102  →  Banko 3             14:28                        │
│  1101  →  Banko 4             14:26                        │
│  1100  →  Banko 2             14:24                        │
│  1099  →  Banko 1             14:22                        │
│  1098  →  Banko 3             14:20                        │
│  1097  →  Banko 2             14:18                        │
│  1096  →  Banko 1             14:16                        │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

**Özellikler:**
- Auto-scroll animasyonu (yeni sıra gelince)
- Ses çalma (Text-to-Speech): "1105 numaralı sıra, 3 nolu bankoya"
- Tam ekran mod
- Sadece ilişkili bankoları göster (TV-Banko eşleşmesi)
- Screensaver (5 dakika aktivite yoksa)

### 9.4. Admin Dashboard (Blazor Page)

**Sayfa:** /admin/dashboard

**Layout:**
```
┌─────────────────────────────────────────────────────┐
│  MENEMEN SGM - Canlı İzleme                         │
├─────────────────────────────────────────────────────┤
│                                                     │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐         │
│  │ Bekleyen │  │ Tamamlan │  │  Online  │         │
│  │   Sıra   │  │    an    │  │ Personel │         │
│  │    12    │  │    45    │  │    8     │         │
│  └──────────┘  └──────────┘  └──────────┘         │
│                                                     │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐         │
│  │  Ortalama│  │ Ortalama │  │  İptal   │         │
│  │ Bekleme  │  │  İşlem   │  │  Edilen  │         │
│  │  8 dk    │  │  11 dk   │  │    3     │         │
│  └──────────┘  └──────────┘  └──────────┘         │
│                                                     │
├─────────────────────────────────────────────────────┤
│  📊 SAATLİK DAĞILIM                                 │
│  [Çizgi grafik: Saat bazlı sıra sayısı]            │
│                                                     │
├─────────────────────────────────────────────────────┤
│  👥 PERSONEL DURUMU                                 │
│                                                     │
│  🟢 Ahmet Yılmaz (Banko 3) - Aktif - 1105          │
│  🟢 Mehmet Demir (Banko 5) - Boş                   │
│  🔴 Ayşe Kaya (Molada) - Son görülme: 10 dk önce  │
│  🟢 Fatma Çelik (Banko 1) - Aktif - 1109          │
│                                                     │
├─────────────────────────────────────────────────────┤
│  📋 KANAL DAĞILIMI                                  │
│  [Pasta grafik: Kanal bazlı sıra sayıları]         │
│                                                     │
└─────────────────────────────────────────────────────┘
```

**Özellikler:**
- 5 saniyede bir otomatik refresh (SignalR)
- Export (Excel/PDF)
- Tarih filtresi
- Departman/Bina filtresi
- Drill-down detay görüntüleme

### 9.5. Yetkilendirme Sayfası (Blazor Page)

**Sayfa:** /admin/yetkilendirme

**Layout:**
```
┌──────────────────────────────────────────────────────┐
│  PERSONEL-KANAL EŞLEŞTİRMESİ                         │
├──────────────────────────────────────────────────────┤
│                                                      │
│  Personel Seç:                                       │
│  [Ahmet Yılmaz ▼]                                    │
│                                                      │
│  ┌────────────────────────────────────────────────┐ │
│  │ KanalAdi         │ AltKanal          │ Seviye  │ │
│  ├──────────────────┼───────────────────┼─────────┤ │
│  │ ☑ Emeklilik     │ ☑ Yaşlılık        │ [Uzman▼]│ │
│  │                  │ ☑ Malullük        │ [Yrd.▼] │ │
│  │                  │ ☐ Aylık Sorgulama │         │ │
│  │                  │                   │         │ │
│  │ ☑ İşveren       │ ☑ İşe Giriş       │ [Uzman▼]│ │
│  │                  │ ☐ İşten Çıkış     │         │ │
│  │                  │                   │         │ │
│  │ ☐ Sigortalı     │ ...               │         │ │
│  └────────────────────────────────────────────────┘ │
│                                                      │
│  [💾 KAYDET]  [❌ İPTAL]                             │
│                                                      │
└──────────────────────────────────────────────────────┘
```

**İşlevler:**
- Personel seç (dropdown/arama)
- Kanal ağacı (tree view)
- Toplu seçim (tüm alt kanallar)
- Seviye değiştirme (uzman/yrd.uzman)
- Değişiklik log'lama
- Onay mekanizması (yetkili kişi onayı)

---

## 10. SİSTEM GEREKSİNİMLERİ

### 10.1. Sunucu Gereksinimleri

**Tek Merkezi Sunucu:**
```
İşletim Sistemi: Windows Server 2022 veya Ubuntu 22.04 LTS
CPU: 4 Core (Intel Xeon veya AMD EPYC)
RAM: 8 GB (16 GB önerilir)
Disk: 128 GB SSD (IOPS kritik)
Network: 1 Gbps Ethernet
```

**SQL Server Express:**
```
Versiyon: SQL Server 2022 Express
Limitler: 10 GB veritabanı (yeterli)
Memory: Max 1 GB (Express limiti)
Backup: Günlük otomatik yedekleme
```

**IIS / Kestrel:**
```
Web Server: IIS 10.0 veya Kestrel
HTTPS: SSL/TLS 1.3 (Let's Encrypt)
Port: 443 (HTTPS), 80 (HTTP redirect)
```

### 10.2. İstemci Gereksinimleri

**Kiosk:**
```
Donanım: Touch screen PC (Windows 10+)
Ekran: 21-24" dokunmatik
Yazıcı: Termal yazıcı (fiş için)
Browser: Edge/Chrome (WebView2)
```

**Banko:**
```
Donanım: Standart PC/Laptop (Windows 10+)
Ekran: 15-17" (FHD önerilir)
Browser: Edge/Chrome/Firefox (güncel)
Network: LAN/WiFi (min 10 Mbps)
```

**TV Ekranı:**
```
Donanım: Mini PC (Intel NUC vb.) + TV/Monitör
TV: 32-55" LED (1080p minimum)
Browser: Chrome (kiosk mode)
Network: LAN/WiFi (min 5 Mbps)
```

### 10.3. Network Gereksinimleri

**Bandwidth:**
```
Kiosk: 1 Mbps (düşük kullanım)
Banko: 2 Mbps (real-time updates)
TV: 1 Mbps (görüntüleme only)

Toplam (500 cihaz): ~750 Mbps
Sunucu internet: 100 Mbps (yeterli)
```

**Latency:**
```
İstemci → Sunucu: < 50ms (LAN)
SignalR heartbeat: Her 30 saniye
Timeout: 30 saniye (bağlantı kopması)
```

### 10.4. Güvenlik Gereksinimleri

**Firewall:**
```
Port 443 (HTTPS): İzin ver (tüm istemciler)
Port 80 (HTTP): Redirect to 443
Port 1433 (SQL): Sadece local (dışarıya kapalı)
Port 5000-5001 (Kestrel): Opsiyonel
```

**Antivirus:**
```
Windows Defender veya kurumsal AV
Real-time protection: Aktif
SQL Server klasörü: Exception (performans)
```

**Backup:**
```
Veritabanı: Günlük full backup (gece 02:00)
Transaction Log: Her 6 saatte bir
Retention: 30 gün
Offsite: Haftalık (bulut yedekleme)
```

---

## 11. DEPLOYMENT VE BAKIM

### 11.1. Kurulum Adımları

```
1. Sunucu Hazırlığı:
   - Windows Server 2022 kur
   - SQL Server Express 2022 kur
   - IIS kur (veya Kestrel standalone)
   - .NET 9 Runtime kur

2. Veritabanı Kurulumu:
   - SQL Server Management Studio ile bağlan
   - Yeni veritabanı oluştur: SGKSiramatikDB
   - Migration script'lerini çalıştır (EF Core migrations)
   - Seed data'yı yükle (İl/İlçe, vb.)

3. Uygulama Deploy:
   - Publish profili: Release
   - Target: Folder
   - Framework: net9.0
   - Self-contained: false (framework-dependent)
   - Dosyaları sunucuya kopyala
   - IIS'te site oluştur veya Kestrel başlat

4. SSL Sertifikası:
   - Let's Encrypt ile ücretsiz SSL
   - Certbot kullan (otomatik renewal)
   - HTTPS binding ekle (IIS)

5. Test:
   - Kiosk'tan sıra al
   - Banko'dan çağır
   - TV'de görüntüle
   - Admin panel kontrol et
   - Raporları test et

6. Go-Live:
   - Kullanıcı eğitimi
   - Pilot çalışma (1 SGM)
   - Kademeli açılış (diğer SGM'ler)
```

### 11.2. Bakım ve İzleme

**Günlük:**
```
- Sistem loglarını kontrol et (hata var mı?)
- Veritabanı boyutunu izle (10 GB'a yaklaşıyor mu?)
- Online personel sayısını kontrol et
- Performans metrikleri (CPU, RAM, Disk)
```

**Haftalık:**
```
- Backup'ların başarılı olduğunu doğrula
- Yavaş query'leri analiz et (SQL Profiler)
- Index'leri optimize et (gerekirse)
- Eski logları temizle (90+ gün)
```

**Aylık:**
```
- Güvenlik güncellemeleri (Windows Update)
- .NET Runtime güncelleme (minor versions)
- Antivirus definition update
- Disaster recovery testi
```

---

## 12. GELİŞTİRME ROADMAP

### Faz 1: MVP (3 Ay)
- ✅ Temel sıramatik işlevleri (kiosk, banko, TV)
- ✅ Kanal yetkilendirmesi
- ✅ SignalR real-time
- ✅ Temel raporlar

### Faz 2: Optimizasyon (2 Ay)
- ⏳ Uzman/Yrd.Uzman önceliklendirmesi
- ⏳ Heartbeat ve online tracking
- ⏳ Gelişmiş raporlama
- ⏳ Performance tuning

### Faz 3: Ek Modüller (4 Ay)
- 📅 PDKS entegrasyonu
- 📅 Eshot entegrasyonu
- 📅 Mobil uygulama (personel için)
- 📅 SMS/Email bildirimleri

### Faz 4: AI/ML (Gelecek)
- 🔮 Sıra tahmini (makine öğrenmesi)
- 🔮 Personel öneri sistemi
- 🔮 Kapasite planlaması
- 🔮 Chatbot entegrasyonu

---

## 13. SONUÇ

Bu döküman, SGK İzmir İl Müdürlüğü Sıramatik Sistemi'nin eksiksiz teknik tanımını içermektedir. Sistem:

✅ **Gerçek zamanlı** sıra yönetimi sağlar  
✅ **Ölçeklenebilir** mimari ile 20 SGM'yi destekler  
✅ **Güvenli** ve denetlenebilir bir yapıya sahiptir  
✅ **Kullanıcı dostu** arayüzler sunar  
✅ **Kapsamlı raporlama** ve analytics içerir  

**Teknik Stack:**
- .NET 9 + Blazor Server
- SignalR (Real-time)
- SQL Server Express
- Entity Framework Core
- Bootstrap 5

**Temel Prensipler:**
1. Kanal bazlı yetkilendirme
2. Uzman/Yardımcı Uzman seviye kontrolü
3. Ortak sıra havuzu sistemi
4. Bina bazlı izolasyon
5. Real-time senkronizasyon

Sistem, günde 5000+ sıra işlemi yapmaya ve 500 eşzamanlı kullanıcıya hizmet vermeye hazırdır.

---

**Hazırlayan:** [İsim]  
**Onaylayan:** [Yönetici]  
**Revizyon:** 1.0  
**Son Güncelleme:** 03 Kasım 2025

**İletişim:**  
Email: teknik@sgk.gov.tr  
Telefon: +90 XXX XXX XX XX

---

## EKLER

### EK-A: Tablo İlişki Diyagramı
```
[Buraya Entity Relationship Diagram (ERD) eklenecek]
```

### EK-B: SignalR Event Flow Diyagramı
```
[Buraya SignalR mesaj akış şeması eklenecek]
```

### EK-C: Kullanıcı Rolleri ve Yetkiler Matrisi
```
[Buraya yetki matrisi tablosu eklenecek]
```

### EK-D: API Endpoint Listesi
```
[Buraya REST API endpoint dökümanı eklenecek]
```

---

**NOT:** Bu döküman "canlı" bir dokümandır ve proje ilerledikçe güncellenmelidir. Her güncelleme için revizyon numarası artırılmalı ve değişiklik logu tutulmalıdır.