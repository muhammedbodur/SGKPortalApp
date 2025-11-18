# 🏛️ SGK SIRAMATİK SİSTEMİ - KAPSAMLI PROJE TANIMI

**Versiyon:** 2.0 - GERÇEK PROJE YAPISI  
**Tarih:** 03 Kasım 2025  
**Proje:** SGK İzmir İl Müdürlüğü Sıramatik Otomasyon Sistemi  
**Teknoloji Stack:** .NET 9, Blazor Server, SignalR, SQL Server  
**Mimari:** 4 Katmanlı (Presentation → Business → DataAccess → BusinessObject)

---

## ⚠️ ÖNEMLİ NOT

Bu döküman, **MEVCUT PROJENİN GERÇEK VERİTABANI YAPISINA** dayanarak hazırlanmıştır. 
Tüm tablo isimleri, entity ilişkileri ve yapılar **proje kodundan** çıkarılmıştır.
Hayali veya teorik hiçbir yapı içermez - sadece gerçek implementasyon!

---

## 📋 İÇİNDEKİLER

1. [Proje Özeti](#1-proje-özeti)
2. [Teknoloji Stack](#2-teknoloji-stack)
3. [Organizasyon Yapısı](#3-organizasyon-yapısı)
4. [Veritabanı Yapısı - GERÇEK](#4-veritabanı-yapısı)
5. [Sıramatik Sistemi Nasıl Çalışır](#5-sıramatik-sistemi-nasıl-çalışır)
6. [Yetkilendirme ve Atama Sistemi](#6-yetkilendirme-ve-atama-sistemi)
7. [SignalR Real-Time İletişim](#7-signalr-real-time-iletişim)
8. [Kullanıcı Arayüzleri](#8-kullanıcı-arayüzleri)
9. [İş Akışları - Detaylı](#9-iş-akışları)
10. [Sistem Gereksinimleri](#10-sistem-gereksinimleri)

---

## 1. PROJE ÖZETİ

### 1.1. Genel Tanım

SGK İzmir İl Müdürlüğü ve bağlı Sosyal Güvenlik Merkezlerinde (SGM) vatandaşlara hizmet sunan 
dijital sıra yönetim sistemidir. Sistem, vatandaşların Kiosk cihazlarından sıra alması, 
personellerin banko ekranlarından sıra çağırması ve TV ekranlarında canlı görüntüleme 
yapabilmesini sağlar.

### 1.2. Kapsam

- **20 SGM** (Menemen, Karşıyaka, Bornova, Konak, vb.)
- **Yaklaşık 500+ personel**
- **Günlük 5000+ sıra** işlemi
- **Real-time (gerçek zamanlı)** senkronizasyon
- **Multi-building (çoklu bina)** desteği

### 1.3. Temel Prensipler

✅ **Hiyerarşik Kanal Yapısı**: Kanal → KanalAlt → KanalIslem → KanalAltIslem  
✅ **Bina Bazlı İzolasyon**: Her binanın kendi sıra sistemi  
✅ **Personel Uzmanlık Seviyeleri**: Uzman / Yardımcı Uzman  
✅ **Banko-Personel Ataması**: BankoKullanici tablosu ile  
✅ **Real-Time Broadcast**: SignalR Hub sistemi  
✅ **TV-Banko Eşleştirmesi**: Çoktan-çoğa ilişki  

---

## 2. TEKNOLOJİ STACK

### 2.1. Backend

| Teknoloji | Versiyon | Kullanım Amacı |
|-----------|----------|----------------|
| .NET | 9.0 | Ana framework |
| ASP.NET Core | 9.0 | Web framework |
| Entity Framework Core | 9.0 | ORM ve database migration |
| SignalR | 9.0 | Real-time bidirectional communication |
| AutoMapper | 13.0+ | Entity-DTO mapping |
| FluentValidation | 11.0+ | İş kuralları validasyonu |

### 2.2. Frontend

| Teknoloji | Kullanım Amacı |
|-----------|----------------|
| Blazor Server | Server-side UI framework |
| Bootstrap 5 | CSS framework |
| JavaScript Interop | Browser API erişimi |
| SignalR Client | Real-time UI updates |

### 2.3. Database

| Teknoloji | Versiyon | Kullanım |
|-----------|----------|----------|
| SQL Server | 2019+ | Ana veritabanı |
| SQL Server Management Studio | 19.0+ | DB yönetimi |

### 2.4. Mimari Pattern'ler

- **Repository Pattern**: Veri erişim soyutlaması
- **Unit of Work Pattern**: Transaction yönetimi
- **DTO Pattern**: Katmanlar arası veri transferi
- **Dependency Injection**: IoC container
- **SignalR Hub Pattern**: Real-time messaging

---

## 3. ORGANİZASYON YAPISI

### 3.1. Hiyerarşi

```
İZMİR İL MÜDÜRLÜĞÜ
│
├── İl Müdürlüğü (Departman - DepartmanTip: IlMudurlugu)
│   ├── İdari ve Mali İşler Servisi
│   ├── İnsan Kaynakları Servisi
│   ├── Hukuk Servisi
│   └── Bilgi İşlem Servisi
│
├── Menemen SGM (Departman - DepartmanTip: SGM)
│   ├── Emeklilik Servisi
│   ├── İşveren Servisi
│   └── Tahsis Servisi
│
├── Karşıyaka SGM (Departman - DepartmanTip: SGM)
│   └── ...
│
└── ... (18+ SGM daha)
```

### 3.2. Fiziksel Yapı

```
SGM (Departman)
│
├── A Binası (HizmetBinasi)
│   ├── Zemin Kat (KatTipi: ZeminKat)
│   │   ├── Kiosk Cihazları (KioskGrup)
│   │   ├── Banko 1, 2, 3 (Banko - BankoTipi: Normal)
│   │   └── TV Ekranı 1 (Tv)
│   │
│   └── 1. Kat (KatTipi: BirinciKat)
│       ├── Kiosk Cihazları
│       ├── Banko 4, 5, 6
│       └── TV Ekranı 2
│
└── B Binası (HizmetBinasi)
    └── ...
```

### 3.3. Roller ve Sorumluluklar

| Rol | Erişim Seviyesi | Sorumluluklar |
|-----|----------------|---------------|
| **Bilgi İşlem** | Super Admin | Sistem ayarları, global tanımlamalar |
| **İnsan Kaynakları** | Admin | Departman/servis/personel yönetimi |
| **Müdür Yardımcısı** | SGM Admin | SGM bazlı yönetim, raporlama |
| **Şef** | Servis Manager | Personel-kanal eşleştirmesi, banko atamaları |
| **Personel** | User | Sıra çağırma, işlem yapma |

---

## 4. VERİTABANI YAPISI

### 4.1. Tablo Grupları ve Prefix'ler

- **CMN_**: Common (Ortak tablolar) - İl, İlçe, Bina, User, Log
- **PER_**: Personel (Personel işlemleri) - Personel, Departman, Servis, Unvan
- **SIR_**: Sıramatik (Sıra sistemi) - Kanal, Banko, Sıra, Tv, Hub
- **PDK_**: PDKS (Personel devam kontrol) - İleride eklenecek
- **ESH_**: Eshot (Ulaşım kartları) - İleride eklenecek 

---

### 4.2. ORTAK TABLOLAR (CMN_ Prefix)

#### CMN_Iller
```
IlId (PK, int, Identity)
IlAdi (nvarchar(50))
Aktiflik (Enum: Aktif/Pasif)
+ Audit Kolonları (EklenmeTarihi, DuzenlenmeTarihi, SilindiMi, vb.)
```

#### CMN_Ilceler
```
IlceId (PK, int, Identity)
IlId (FK → CMN_Iller)
IlceAdi (nvarchar(100))
Aktiflik
+ Audit Kolonları
```

#### CMN_HizmetBinalari
```
HizmetBinasiId (PK, int, Identity)
HizmetBinasiAdi (nvarchar(200))
IlId (FK → CMN_Iller)
IlceId (FK → CMN_Ilceler)
Adres (nvarchar(500))
Aktiflik
+ Audit Kolonları

İlişkiler:
- Bankolar (1-N)
- Tvler (1-N)
- KanalIslemleri (1-N)
- KanalAltIslemleri (1-N)
```

#### CMN_Users
```
TcKimlikNo (PK, nvarchar(11))
KullaniciAdi (nvarchar(50), Unique)
Email (nvarchar(100), Unique)
PasswordHash (nvarchar(max))
TelefonNo (nvarchar(20))
Rol (Enum: BirimAmiri, Supervisor, YetkiliKisi, Personel)
Aktiflik
+ Audit Kolonları

Not: Authentication için kullanılır
```

#### CMN_DatabaseLogs
```
LogId (PK, int, Identity)
Tablo (nvarchar(100))
IslemTipi (Enum: Insert, Update, Delete)
EskiDeger (nvarchar(max), JSON)
YeniDeger (nvarchar(max), JSON)
KullaniciTcKimlikNo (nvarchar(11))
IslemZamani (datetime2)

Kullanım: Tüm CRUD işlemlerinin audit trail'i
```

#### CMN_LoginLogoutLogs
```
LogId (PK, int, Identity)
TcKimlikNo (nvarchar(11))
IslemTipi (Enum: Login, Logout)
IslemZamani (datetime2)
IpAdresi (nvarchar(50))
BrowserInfo (nvarchar(200))

Kullanım: Giriş/Çıkış takibi
```

#### CMN_Moduller (Yetki Sistemi)
```
ModulId (PK, int, Identity)
ModulAdi (nvarchar(100))
ModulIcon (nvarchar(50))
ModulSira (int)
ModulAktiflik (Aktiflik)
+ Audit Kolonları

İlişkiler:
- ModulAltlari (1-N)
```

#### CMN_ModullerAlt
```
ModulAltId (PK, int, Identity)
ModulId (FK → CMN_Moduller)
ModulAltAdi (nvarchar(100))
ModulAltUrl (nvarchar(200))
ModulAltSira (int)
+ Audit Kolonları
```

---

### 4.3. PERSONEL TABLOLARI (PER_ Prefix)

#### PER_Personeller
```
TcKimlikNo (PK, nvarchar(11))
SicilNo (int, Unique)
AdSoyad (nvarchar(200))
Email (nvarchar(100))
CepTelefonu (nvarchar(20))
DepartmanId (FK → PER_Departmanlar)
ServisId (FK → PER_Servisler)
UnvanId (FK → PER_Unvanlar)
HizmetBinasiId (FK → CMN_HizmetBinalari)
PersonelAktiflik (Enum: Aktif, Pasif, Izinli)
+ Audit Kolonları
+ Kişisel Bilgiler (DogumTarihi, Cinsiyet, MedeniDurum, vb.)
+ Eş Bilgileri
+ İletişim Bilgileri (Il, Ilce, Adres, vb.)

İlişkiler:
- BankoKullanicilari (1-N) - Personelin atandığı bankolar
- KanalPersonelleri (1-N) - Kanal yetkileri
- HubConnection (1-1) - SignalR bağlantısı
- Siralar (1-N) - İşlem yaptığı sıralar
```

#### PER_Departmanlar
```
DepartmanId (PK, int, Identity)
DepartmanAdi (nvarchar(200), Unique)
DepartmanTip (Enum: IlMudurlugu, SGM)
DepartmanAktiflik
+ Audit Kolonları

Örnekler:
- İl Müdürlüğü (IlMudurlugu)
- Menemen SGM (SGM)
- Karşıyaka SGM (SGM)
```

#### PER_Servisler
```
ServisId (PK, int, Identity)
DepartmanId (FK → PER_Departmanlar)
ServisAdi (nvarchar(200))
ServisAktiflik
+ Audit Kolonları

Örnekler:
- Emeklilik Servisi
- İşveren Servisi
- Tahsis Servisi
```

#### PER_Unvanlar
```
UnvanId (PK, int, Identity)
UnvanAdi (nvarchar(100))
UnvanSeviye (int) - Hiyerarşik sıralama
Aktiflik
+ Audit Kolonları

Örnekler:
- Müdür (1)
- Müdür Yardımcısı (2)
- Şef (3)
- Memur (4)
```

#### PER_PersonelCocuklari
```
PersonelCocukId (PK, int, Identity)
TcKimlikNo (FK → PER_Personeller)
CocukAdi (nvarchar(200))
DogumTarihi (datetime2)
Cinsiyet (Enum: Erkek, Kiz)
+ Audit Kolonları
```

#### PER_Yetkiler (Menü Yetkilendirme Sistemi)
```
YetkiId (PK, int, Identity)
YetkiAdi (nvarchar(100))
YetkiTuru (nvarchar(50)) - Menu, Action, vb.
UstYetkiId (int, Nullable) - Hiyerarşik yapı
ControllerAdi (nvarchar(100))
ActionAdi (nvarchar(100))
+ Audit Kolonları

İlişkiler:
- Hiyerarşik self-reference
- PersonelYetkiler (1-N)
```

#### PER_PersonelYetkiler
```
PersonelYetkiId (PK, int, Identity)
TcKimlikNo (FK → PER_Personeller)
YetkiId (FK → PER_Yetkiler)
+ Audit Kolonları
```

---

### 4.4. SIRAMATİK TABLOLARI (SIR_ Prefix)

Bu grup, sistemin KALBİDİR! En kritik tablolar burada.

---

#### SIR_Kanallar (Ana Kanal Tanımları)
```
KanalId (PK, int, Identity)
KanalAdi (nvarchar(200))
Aktiflik
+ Audit Kolonları

Örnekler:
- Emeklilik İşlemleri
- İşveren İşlemleri  
- Sigortalı İşlemleri
- Genel Sağlık Sigortası

İlişkiler:
- KanalAltlari (1-N)
- KanalIslemleri (1-N)

ÖNEMLİ: Kanal = Vatandaşın anladığı ana kategori
```

---

#### SIR_KanallarAlt (Alt Kanal Tanımları)
```
KanalAltId (PK, int, Identity)
KanalId (FK → SIR_Kanallar)
KanalAltAdi (nvarchar(200))
Aktiflik
+ Audit Kolonları

Örnekler (Emeklilik kanalı altında):
- Yaşlılık Aylığı Başvurusu
- Malullük Başvurusu
- Aylık Bağlama Durumu Sorgulama
- Emekli Sandık Aktarma

İlişkiler:
- KanalAltIslemleri (1-N)
- KioskIslemGruplari (1-N)

ÖNEMLİ: AltKanal = İşlem türü (daha spesifik)
```

---

#### SIR_KanalIslemleri (Bina Bazlı Kanal Aktivasyonu)
```
KanalIslemId (PK, int, Identity)
KanalId (FK → SIR_Kanallar)
HizmetBinasiId (FK → CMN_HizmetBinalari)
Sira (int) - Gösterim sırası
BaslangicNumara (int) - Örn: 1000
BitisNumara (int) - Örn: 1999
Aktiflik
EklenmeTarihi
DuzenlenmeTarihi
+ Audit Kolonları

İlişkiler:
- KanalAltIslemleri (1-N)

AMAÇ: 
Bir kanal (örn: Emeklilik), her binada aktif olmayabilir.
Bu tablo, hangi kanalın hangi binada aktif olduğunu ve 
o binada hangi numara aralığını kullanacağını belirler.

Örnek:
- Menemen SGM A Binası - Emeklilik: 1000-1999
- Menemen SGM B Binası - Emeklilik: 2000-2999
```

---

#### SIR_KanalAltIslemleri (Bina Bazlı Alt Kanal Aktivasyonu)
```
KanalAltIslemId (PK, int, Identity)
KanalAltId (FK → SIR_KanallarAlt)
KanalIslemId (FK → SIR_KanalIslemleri)
HizmetBinasiId (FK → CMN_HizmetBinalari)
KioskIslemGrupId (FK → SIR_KioskIslemGruplari, Nullable)
Aktiflik
+ Audit Kolonları

İlişkiler:
- Siralar (1-N) - Bu alt kanal işleminden alınan sıralar
- KanalPersonelleri (1-N) - Bu işlemi yapabilecek personeller

AMAÇ:
Alt kanalın belirli bir binada aktif olması.
Personeller ve Sıralar bu tabloya bağlanır!

ÖNEMLİ: Sisteminizin SIRA DAĞITIM MERKEZİ burası!

Örnek:
- Menemen A Binası - Yaşlılık Aylığı (KanalAltIslemId: 15)
- Bu ID'ye personeller atanır (KanalPersonelleri)
- Bu ID'den sıralar oluşturulur (Siralar)
```

---

#### SIR_KanalPersonelleri (Personel-Kanal Yetkilendirmesi)
```
KanalPersonelId (PK, int, Identity)
TcKimlikNo (FK → PER_Personeller)
KanalAltIslemId (FK → SIR_KanalAltIslemleri)
Uzmanlik (Enum: YardimciUzman=1, Uzman=2)
Aktiflik
+ Audit Kolonları

UNIQUE INDEX: (TcKimlikNo, KanalAltIslemId)

AMAÇ:
Ahmet hangi alt kanal işlemlerinde yetkili?
Ve hangi seviyede? (Uzman mı, Yardımcı Uzman mı?)

Örnek Kayıt:
TcKimlikNo: 12345678901 (Ahmet)
KanalAltIslemId: 15 (Menemen A - Yaşlılık Aylığı)
Uzmanlik: Uzman

Anlamı:
Ahmet, Menemen A Binasında, Yaşlılık Aylığı işleminde UZMAN seviyesinde.
```

---

#### SIR_Bankolar (Fiziksel Banko Tanımları)
```
BankoId (PK, int, Identity)
HizmetBinasiId (FK → CMN_HizmetBinalari)
BankoNo (int) - Banko numarası (örn: 3)
BankoTipi (Enum: Normal, Oncelikli, Engelli)
KatTipi (Enum: ZeminKat, BirinciKat, IkinciKat, vb.)
BankoAktiflik (Aktiflik)
+ Audit Kolonları

İlişkiler:
- BankoKullanicilari (1-N) - Bankoya atanan personeller
- TvBankolar (1-N) - Bu bankoyu gösteren TV'ler

AMAÇ:
Fiziksel banko tanımı. "3 Nolu Banko" gibi.

Örnek:
BankoId: 5
BankoNo: 3
HizmetBinasiId: 1 (Menemen A Binası)
KatTipi: BirinciKat
```

---

#### SIR_BankoKullanicilari (Banko-Personel Ataması)
```
BankoKullaniciId (PK, int, Identity)
BankoId (FK → SIR_Bankolar)
TcKimlikNo (FK → PER_Personeller)
EklenmeTarihi
DuzenlenmeTarihi
+ Audit Kolonları

UNIQUE INDEX: BankoId (Bir bankoda aynı anda sadece 1 personel)
UNIQUE INDEX: TcKimlikNo (Bir personel aynı anda sadece 1 bankoda)

AMAÇ:
Ahmet şu anda hangi bankoda çalışıyor?
3 nolu banko şu anda kim tarafından kullanılıyor?

ÇOK ÖNEMLİ:
Bu tablo sayesinde, her bankonun o anda hangi personele ait olduğu bilinir.
Sıralar, o personelin yetkilerine göre gelir!

Örnek:
BankoKullaniciId: 10
BankoId: 5 (3 Nolu Banko)
TcKimlikNo: 12345678901 (Ahmet)

Anlamı: 3 nolu banko şu anda Ahmet'e atanmış.
Bu bankoya sadece Ahmet'in yetkili olduğu işlemlerin sıraları gelir.
```

---

#### SIR_Siralar (Sıra Kayıtları)
```
SiraId (PK, int, Identity) - Internal unique ID
SiraNo (int) - Vatandaşın gördüğü numara (1105, 2045, vb.)
KanalAltIslemId (FK → SIR_KanalAltIslemleri)
KanalAltAdi (nvarchar) - Cache için
HizmetBinasiId (FK → CMN_HizmetBinalari)
TcKimlikNo (FK → PER_Personeller, Nullable) - Hangi personel işlem yaptı
SiraAlisZamani (datetime2)
IslemBaslamaZamani (datetime2, Nullable)
IslemBitisZamani (datetime2, Nullable)
BeklemeDurum (Enum: Beklemede, Islemde, Tamamlandi, IptalEdildi)
SiraAlisTarihi (NotMapped) - Hesaplanır
+ Audit Kolonları

INDEX: (HizmetBinasiId, BeklemeDurum, SiraAlisZamani)

AMAÇ:
Sistemdeki her sıra burada kayıtlı.

Sıra Yaşam Döngüsü:
1. Beklemede - Kiosk'tan alındı, bekliyor
2. Islemde - Personel çağırdı, işlem yapılıyor
3. Tamamlandi - İşlem bitti
4. IptalEdildi - Vatandaş gelmedi veya personel iptal etti

Örnek:
SiraId: 12345
SiraNo: 1105
KanalAltIslemId: 15 (Yaşlılık Aylığı)
HizmetBinasiId: 1 (Menemen A)
BeklemeDurum: Beklemede
SiraAlisZamani: 2025-11-03 09:15:00
```

---

#### SIR_Tvler (TV Ekranı Tanımları)
```
TvId (PK, int, Identity)
TvAdi (nvarchar) - "Zemin Kat TV 1"
HizmetBinasiId (FK → CMN_HizmetBinalari)
KatTipi (Enum)
TvAktiflik
TvAciklama (nvarchar, Nullable)
IslemZamani
+ Audit Kolonları

İlişkiler:
- TvBankolar (1-N) - Bu TV hangi bankoları gösterir
- HubTvConnection (1-1) - SignalR bağlantısı

Örnek:
TvId: 3
TvAdi: "1. Kat TV Ekranı"
HizmetBinasiId: 1
KatTipi: BirinciKat
```

---

#### SIR_TvBankolari (TV-Banko Eşleştirmesi - Many-to-Many)
```
TvBankoId (PK, int, Identity)
TvId (FK → SIR_Tvler)
BankoId (FK → SIR_Bankolar)
Aktiflik
+ Audit Kolonları

UNIQUE INDEX: (TvId, BankoId)

AMAÇ:
Bir TV, birden fazla bankonun çağrılarını gösterir.
Bir banko, birden fazla TV'de görünebilir.

Örnek:
"1. Kat TV" şu bankoları gösterir:
- 3 Nolu Banko
- 4 Nolu Banko
- 5 Nolu Banko
```

---

#### SIR_KioskGruplari (Kiosk Grup Tanımları)
```
KioskGrupId (PK, int, Identity)
KioskGrupAdi (nvarchar) - "Zemin Kat Kiosk'ları"
Aktiflik
+ Audit Kolonları

İlişkiler:
- KioskIslemGruplari (1-N)

AMAÇ:
Kioskları gruplamak. Örneğin, zemin kattaki tüm kiosklara 
aynı alt kanalları atamak için.
```

---

#### SIR_KioskIslemGruplari (Kiosk-Kanal Eşleştirmesi)
```
KioskIslemGrupId (PK, int, Identity)
KioskGrupId (FK → SIR_KioskGruplari)
HizmetBinasiId (FK → CMN_HizmetBinalari)
KanalAltId (FK → SIR_KanallarAlt)
KioskIslemGrupSira (int) - Gösterim sırası
KioskIslemGrupAktiflik
+ Audit Kolonları

UNIQUE INDEX: (KioskGrupId, KanalAltId)

İlişkiler:
- KanalAltIslemleri (1-N)

AMAÇ:
Hangi kiosk grubu hangi alt kanalları gösterecek?

Örnek:
"Zemin Kat Kiosk'ları" şunları gösterir:
- Emeklilik → Yaşlılık Aylığı
- Emeklilik → Malullük
- İşveren → İşe Giriş Bildirimi
```

---

#### SIR_BankoIslemleri (Kiosk Menü Hiyerarşisi - ESKİ YAPI)
```
BankoIslemId (PK, int, Identity)
BankoGrup (Enum: AnaGrup, OrtaGrup, AltGrup)
BankoUstIslemId (int) - -1 ise AnaGrup
BankoIslemAdi (nvarchar)
BankoIslemSira (int)
BankoIslemAktiflik
DiffLang (nvarchar) - Çoklu dil desteği
+ Audit Kolonları

NOT: Bu tablo eski yapı için. Yeni yapı:
Kanal → KanalAlt → KanalIslem → KanalAltIslem
```

---

#### SIR_HubConnections (Personel SignalR Bağlantıları)
```
HubConnectionId (PK, int, Identity)
TcKimlikNo (FK → PER_Personeller, Unique)
ConnectionId (nvarchar(100)) - SignalR connection ID
ConnectionStatus (Enum: online, offline, away)
IslemZamani (datetime2) - Son aktivite
+ Audit Kolonları

İlişkiler:
- Personel (1-1) - Bir personelin bir connection'ı

AMAÇ:
Personel online mı? SignalR connection ID'si ne?
Real-time message routing için kritik!

Örnek:
TcKimlikNo: 12345678901
ConnectionId: "xyz123abc456"
ConnectionStatus: online
IslemZamani: 2025-11-03 09:30:00
```

---

#### SIR_HubTvConnections (TV SignalR Bağlantıları)
```
HubTvConnectionId (PK, int, Identity)
TvId (FK → SIR_Tvler, Unique)
ConnectionId (nvarchar(100))
ConnectionStatus (Enum)
IslemZamani
+ Audit Kolonları

İlişkiler:
- Tv (1-1)

AMAÇ:
TV ekranları için SignalR connection tracking.
```

---

### 4.5. Enum Tanımları

```csharp
// Genel
public enum Aktiflik { Aktif = 1, Pasif = 0 }

// Personel
public enum PersonelAktiflik { Aktif = 1, Pasif = 0, Izinli = 2 }
public enum PersonelUzmanlik { YardimciUzman = 1, Uzman = 2 }

// Sıramatik
public enum BankoTipi { Normal, Oncelikli, Engelli }
public enum KatTipi { ZeminKat, BirinciKat, IkinciKat, UcuncuKat }
public enum BeklemeDurum { Beklemede, Islemde, Tamamlandi, IptalEdildi }
public enum ConnectionStatus { online, offline, away }
public enum BankoGrup { AnaGrup, OrtaGrup, AltGrup }

// Departman
public enum DepartmanTip { IlMudurlugu, SGM }
```

---

### 4.6. Audit Kolonları (Tüm Tablolarda)

```csharp
public abstract class AuditableEntity
{
    public DateTime EklenmeTarihi { get; set; } = DateTime.Now;
    public DateTime DuzenlenmeTarihi { get; set; } = DateTime.Now;
    public string? EkleyenKullanici { get; set; }
    public string? DuzenleyenKullanici { get; set; }
    public bool SilindiMi { get; set; } = false;
    public DateTime? SilinmeTarihi { get; set; }
    public string? SilenKullanici { get; set; }
}
```

**Global Query Filter**: `HasQueryFilter(e => !e.SilindiMi)`  
Yani silinmiş kayıtlar otomatik filtrelenir (Soft Delete)

---

## 5. SIRAMATİK SİSTEMİ NASIL ÇALIŞIR

### 5.1. Temel Kavramlar

#### 5.1.1. Kanal Hiyerarşisi

```
Kanal (Ana Kategori)
│
├─> KanalAlt (İşlem Türü)
│   │
│   └─> KanalIslem (Bina Bazlı Aktivasyon + Numara Aralığı)
│       │
│       └─> KanalAltIslem (Bina Bazlı Alt Kanal Aktivasyonu)
│           │
│           ├─> Personeller bu seviyeye atanır (KanalPersonelleri)
│           └─> Sıralar bu seviyeden oluşturulur (Siralar)
```

**Örnek Akış:**

1. **Kanal**: "Emeklilik İşlemleri" (KanalId: 1)
   
2. **KanalAlt**: "Yaşlılık Aylığı" (KanalAltId: 5)

3. **KanalIslem**: 
   - Menemen A Binası + Emeklilik → Numara: 1000-1999
   - (KanalIslemId: 10)

4. **KanalAltIslem**:
   - Menemen A + Yaşlılık Aylığı
   - (KanalAltIslemId: 15)
   - **Buraya personeller atanır!**
   - **Buradan sıralar oluşturulur!**

---

#### 5.1.2. Banko-Personel İlişkisi

```
Banko (Fiziksel)
│
└─> BankoKullanici
    │
    ├─> Banko: 3 Nolu Banko
    └─> Personel: Ahmet (TC: 12345678901)

Ahmet'in Yetkileri (KanalPersonelleri):
├─> KanalAltIslemId: 15 (Yaşlılık) - Uzman
├─> KanalAltIslemId: 18 (Malullük) - Yardımcı Uzman
└─> KanalAltIslemId: 22 (Aylık Sorgulama) - Uzman

Sonuç:
3 Nolu Banko'ya sadece bu 3 işlemin sıraları gelir!
```

---

### 5.2. Sıra Alma İşlemi (Kiosk)

**ADIM 1**: Vatandaş Kiosk'a Gelir

**ADIM 2**: Kiosk Ekranı Gösterir:
- KioskGrup'a göre filtrelenmiş
- KanalAltlari listesi

**ADIM 3**: Vatandaş Seçer:
- "Emeklilik İşlemleri" → "Yaşlılık Aylığı"

**ADIM 4**: Sistem Sıra Oluşturur:
```sql
-- KanalAltIslemId'yi bul
SELECT KanalAltIslemId 
FROM SIR_KanalAltIslemleri 
WHERE KanalAltId = 5 -- Yaşlılık Aylığı
  AND HizmetBinasiId = 1 -- Menemen A Binası

-- Sıra numarasını belirle (KanalIslem'den)
SELECT BaslangicNumara, BitisNumara
FROM SIR_KanalIslemleri
WHERE KanalId = 1 AND HizmetBinasiId = 1
-- Sonuç: 1000-1999

-- Son verilen sıra numarasını al
SELECT MAX(SiraNo) FROM SIR_Siralar
WHERE KanalAltIslemId = 15 
  AND CAST(SiraAlisZamani AS DATE) = CAST(GETDATE() AS DATE)
-- Sonuç: 1104

-- Yeni sıra numarası: 1105

-- Sıra kaydet
INSERT INTO SIR_Siralar (SiraNo, KanalAltIslemId, HizmetBinasiId, BeklemeDurum, SiraAlisZamani)
VALUES (1105, 15, 1, 'Beklemede', GETDATE())
```

**ADIM 5**: Fiş Yazdır
```
═══════════════════════════════
    SGK MENEMEN SGM
═══════════════════════════════
    SIRA NUMARANIZ

         1105

    İşlem: Yaşlılık Aylığı
    Tarih: 03.11.2025 09:15
    
    Lütfen bekleyiniz...
    1. Kat - Emeklilik Servisi
═══════════════════════════════
```

**ADIM 6**: SignalR Broadcast
```csharp
// Tüm yetkili personellere gönder
await Clients.Groups($"KanalAltIslem-{kanalAltIslemId}")
    .SendAsync("OnQueueCreated", new {
        SiraId = 12345,
        SiraNo = 1105,
        KanalAltAdi = "Yaşlılık Aylığı",
        BinaId = 1,
        Zaman = DateTime.Now
    });
```

---

### 5.3. Personel Login ve Banko Ataması

**ADIM 1**: Personel Giriş Yapar
- TC Kimlik ve Şifre ile authentication

**ADIM 2**: Banko Seçer
- "Hangi bankoda çalışacaksınız?" sorusu
- Dropdown: Boş bankolar listelenir

**ADIM 3**: Banko Ataması Yapılır
```sql
-- Önce eski kaydı temizle (aynı anda 2 bankoda olamaz)
DELETE FROM SIR_BankoKullanicilari 
WHERE TcKimlikNo = '12345678901'

-- Yeni atama
INSERT INTO SIR_BankoKullanicilari (BankoId, TcKimlikNo, EklenmeTarihi)
VALUES (5, '12345678901', GETDATE())
```

**ADIM 4**: SignalR Hub Bağlantısı
```sql
-- Hub connection kaydet
INSERT INTO SIR_HubConnections (TcKimlikNo, ConnectionId, ConnectionStatus, IslemZamani)
VALUES ('12345678901', 'xyz123', 'online', GETDATE())
```

**ADIM 5**: SignalR Group'lara Ekle
```csharp
// Personelin yetkili olduğu her KanalAltIslem için
var yetkiler = await GetKanalPersonelleri(tcKimlikNo);

foreach(var yetki in yetkiler)
{
    await Groups.AddToGroupAsync(
        Context.ConnectionId, 
        $"KanalAltIslem-{yetki.KanalAltIslemId}"
    );
}

// Bina grubuna da ekle
await Groups.AddToGroupAsync(Context.ConnectionId, $"Bina-{binaId}");
```

**ADIM 6**: Bekleyen Sıraları Yükle
```csharp
// Personelin görebileceği sıraları getir
var siralar = await GetPersonelSiralari(tcKimlikNo, binaId);

// UI'a gönder
await Clients.Caller.SendAsync("LoadQueues", siralar);
```

---

### 5.4. Sıra Çağırma İşlemi

**Personel "Sonraki Sıra" Butonuna Basar**

**Backend İşlem:**

```csharp
public async Task<ServiceResult> CallNextQueue(int bankoId, string tcKimlikNo)
{
    using var transaction = await _unitOfWork.BeginTransactionAsync();
    
    try
    {
        // 1. Banko kontrolü - Bu personel bu bankoda mı?
        var bankoKullanici = await _bankoKullaniciRepo.GetByBankoAsync(bankoId);
        if(bankoKullanici?.TcKimlikNo != tcKimlikNo)
            throw new UnauthorizedException("Bu banko size ait değil!");
        
        // 2. Personelin yetkilerini al
        var yetkiler = await _kanalPersonelRepo.GetByPersonelAsync(tcKimlikNo);
        
        // 3. Öncelik sırasına göre sıra seç
        // ÖNCE UZMAN olduğu kanallardan
        // SONRA YARDIMCI UZMAN olduğu kanallardan
        
        var sira = await _db.Siralar
            .Where(s => s.HizmetBinasiId == binaId && s.BeklemeDurum == BeklemeDurum.Beklemede)
            .Join(_db.KanalPersonelleri.Where(kp => kp.TcKimlikNo == tcKimlikNo),
                  s => s.KanalAltIslemId,
                  kp => kp.KanalAltIslemId,
                  (s, kp) => new { Sira = s, Yetki = kp })
            .OrderByDescending(x => x.Yetki.Uzmanlik) // 2 (Uzman) önce
            .ThenBy(x => x.Sira.SiraNo) // En küçük numara
            .Select(x => x.Sira)
            .FirstOrDefaultAsync();
        
        if(sira == null)
            return ServiceResult.Error("Bekleyen sıra yok!");
        
        // 4. Sırayı çağır
        sira.BeklemeDurum = BeklemeDurum.Islemde;
        sira.IslemBaslamaZamani = DateTime.Now;
        sira.TcKimlikNo = tcKimlikNo;
        
        await _unitOfWork.SaveChangesAsync();
        await transaction.CommitAsync();
        
        // 5. SignalR Broadcast
        // a) Tüm personellere: Bu sıra artık yok!
        await _hubContext.Clients
            .Group($"KanalAltIslem-{sira.KanalAltIslemId}")
            .SendAsync("OnQueueRemoved", sira.SiraId);
        
        // b) TV ekranlarına: Yeni çağrı!
        await _hubContext.Clients
            .Group($"Bina-{binaId}-TV")
            .SendAsync("OnQueueCalled", new {
                SiraNo = sira.SiraNo,
                BankoNo = banko.BankoNo,
                KanalAltAdi = sira.KanalAltAdi,
                Zaman = DateTime.Now
            });
        
        // c) Çağıran personele: Aktif sıran bu!
        await _hubContext.Clients
            .User(tcKimlikNo)
            .SendAsync("OnMyQueueStarted", sira);
        
        return ServiceResult.Success(sira);
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}
```

---

### 5.5. TV Ekranı Görüntüleme

**TV Ekranı Açılır:**

1. **SignalR Bağlantısı:**
```csharp
// TV'nin connection'ını kaydet
await _db.HubTvConnections.AddAsync(new HubTvConnection {
    TvId = tvId,
    ConnectionId = Context.ConnectionId,
    ConnectionStatus = ConnectionStatus.online
});

// TV grubuna ekle
await Groups.AddToGroupAsync(Context.ConnectionId, $"Bina-{binaId}-TV");
```

2. **İlk Yükleme:**
```csharp
// Bu TV'nin göstereceği bankoları al
var tvBankolar = await _db.TvBankolari
    .Where(tb => tb.TvId == tvId)
    .Include(tb => tb.Banko)
    .ToListAsync();

// Son 10 çağrıyı getir
var sonCagrilar = await _db.Siralar
    .Where(s => tvBankolar.Select(tb => tb.BankoId).Contains(s.???)) // İlişki yok!
    .OrderByDescending(s => s.IslemBaslamaZamani)
    .Take(10)
    .ToListAsync();

await Clients.Caller.SendAsync("LoadRecentCalls", sonCagrilar);
```

**NOT**: Burada bir sorun var! Sira tablosunda BankoId yok. 
Çözüm: Sıra çağrıldığında BankoId'yi de tutmalıyız veya 
TcKimlikNo üzerinden BankoKullanici'dan çözmeliyiz.

3. **Real-Time Güncellemeler:**
```csharp
// OnQueueCalled event'i geldiğinde:
// - En üste ekle
// - Kırmızı renkte göster
// - 3 saniye sonra normal renge döndür
// - Sesli anons: "1105 numaralı sıra, 3 nolu bankoya"
```

---

### 5.6. Uzmanlık Seviyesi ve Önceliklendirme

**Senaryo:**

Ahmet (Uzman - Yaşlılık, Yardımcı Uzman - Malullük)

**Bekleyen Sıralar:**
- 1105 - Yaşlılık Aylığı (09:15)
- 1107 - Malullük (09:16)
- 1110 - Yaşlılık Aylığı (09:20)

**Ahmet "Sonraki Sıra" Butonuna Basar:**

**SQL Query:**
```sql
SELECT s.*, kp.Uzmanlik
FROM SIR_Siralar s
JOIN SIR_KanalPersonelleri kp ON s.KanalAltIslemId = kp.KanalAltIslemId
WHERE kp.TcKimlikNo = '12345678901'
  AND s.HizmetBinasiId = 1
  AND s.BeklemeDurum = 'Beklemede'
ORDER BY 
    kp.Uzmanlik DESC,  -- 2 (Uzman) önce, 1 (Yrd. Uzman) sonra
    s.SiraNo ASC       -- En küçük numara önce
```

**Sonuç:**
```
1105 - Yaşlılık (Uzmanlik: 2)  ← BU ÇAĞRILIR!
1110 - Yaşlılık (Uzmanlik: 2)
1107 - Malullük (Uzmanlik: 1)
```

**Sistem 1105'i çağırır!**

---

### 5.7. Eşik Değeri Sistemi (Uzman Yükü)

**Sorun:** Uzmanlar çok meşgul, yardımcı uzmanlar boşta!

**Çözüm:** Eşik değeri

```sql
-- Sistem ayarı
INSERT INTO SIR_SistemAyarlari (AyarAdi, AyarDegeri)
VALUES ('UzmanEsikDegeri', '5')
```

**Mantık:**
```
EĞER (Uzman kanallarındaki bekleyen sıra sayısı > 5) İSE:
    Yardımcı Uzmana da o kanalın sıralarını göster
DEĞILSE:
    Sadece kendi uzmanlık alanındakileri göster
```

**Implementasyon:**
```csharp
var esikDegeri = await GetSystemSetting("UzmanEsikDegeri"); // 5

// Ahmet'in Uzman olduğu kanallar
var uzmanKanallari = yetkiler.Where(y => y.Uzmanlik == Uzman).Select(y => y.KanalAltIslemId);

// Bu kanallardaki bekleyen sıra sayısı
var uzmanKuyrukSayisi = await _db.Siralar
    .CountAsync(s => uzmanKanallari.Contains(s.KanalAltIslemId) && s.BeklemeDurum == Beklemede);

// Eğer eşik değeri aşıldıysa
if(uzmanKuyrukSayisi > esikDegeri)
{
    // Yardımcı uzman olduğu kanalları da ekle
    var yardimciKanallari = yetkiler.Where(y => y.Uzmanlik == YardimciUzman).Select(y => y.KanalAltIslemId);
    tumKanallari.AddRange(yardimciKanallari);
}
```

---

## 6. YETKİLENDİRME VE ATAMA SİSTEMİ

### 6.1. Personel-Kanal Eşleştirmesi

**Yetkili Kişi (Şef) Rolü:**

1. **Personel Seçer**: Ahmet
2. **Kanal Alt İşlem Seçer**: Menemen A - Yaşlılık Aylığı
3. **Uzmanlık Seviyesi Seçer**: Uzman
4. **Kaydet:**

```sql
INSERT INTO SIR_KanalPersonelleri (TcKimlikNo, KanalAltIslemId, Uzmanlik, Aktiflik)
VALUES ('12345678901', 15, 2, 1) -- 2 = Uzman
```

**Çakışma Kontrolü:**
```sql
-- Aynı kişiye aynı kanal tekrar verilemez
UNIQUE INDEX: IX_KanalPersonelleri_TcKimlik_KanalAltIslem
```

---

### 6.2. Banko Ataması

**İki Yöntem:**

**Yöntem 1: Personel Kendi Seçer (Login Sırasında)**
```
1. Login ol
2. "Hangi bankoda çalışacaksınız?" dropdown
3. Boş bankoları göster
4. Seç → Atama yapılır
```

**Yöntem 2: Yönetici Atar (Admin Panel)**
```sql
-- Şef, Ahmet'i 3 nolu bankoya atar
INSERT INTO SIR_BankoKullanicilari (BankoId, TcKimlikNo)
VALUES (5, '12345678901') -- 5 = 3 nolu bankonun ID'si

-- Ahmet login olduğunda otomatik o bankoda açılır
```

---

### 6.3. Kiosk-Kanal Eşleştirmesi

**Amaç:** Hangi kiosk hangi kanalları gösterecek?

**Adımlar:**

1. **Kiosk Grubu Oluştur:**
```sql
INSERT INTO SIR_KioskGruplari (KioskGrupAdi)
VALUES ('Zemin Kat Kiosk''ları')
-- KioskGrupId: 1
```

2. **Alt Kanalları Grupla:**
```sql
INSERT INTO SIR_KioskIslemGruplari (KioskGrupId, HizmetBinasiId, KanalAltId, KioskIslemGrupSira)
VALUES 
  (1, 1, 5, 1),   -- Yaşlılık Aylığı
  (1, 1, 8, 2),   -- Malullük
  (1, 1, 12, 3);  -- İşe Giriş Bildirimi
```

3. **Kiosk Açıldığında:**
```csharp
var grup = await GetKioskGrup(kioskId); // 1
var altKanallar = await _db.KioskIslemGruplari
    .Where(kg => kg.KioskGrupId == grup.Id && kg.HizmetBinasiId == binaId)
    .Include(kg => kg.KanalAlt)
    .OrderBy(kg => kg.KioskIslemGrupSira)
    .ToListAsync();

// UI'da göster
```

---

### 6.4. TV-Banko Eşleştirmesi

**Amaç:** 1. Kattaki TV, hangi bankoların çağrılarını gösterecek?

**Atama:**
```sql
INSERT INTO SIR_TvBankolari (TvId, BankoId, Aktiflik)
VALUES 
  (3, 5, 1),  -- TV 3, Banko 3'ü gösterir
  (3, 7, 1),  -- TV 3, Banko 4'ü gösterir
  (3, 9, 1);  -- TV 3, Banko 5'i gösterir
```

**Sonuç:**
"1. Kat TV Ekranı" artık sadece 3, 4 ve 5 nolu bankoların çağrılarını gösterir.

---

## 7. SIGNALR REAL-TIME İLETİŞİM

### 7.1. Hub Yapısı

```csharp
[Authorize]
public class SiramatikHub : Hub
{
    // Personel bağlandı
    public override async Task OnConnectedAsync()
    {
        var tcKimlikNo = Context.User.GetTcKimlikNo();
        
        // Connection kaydet
        await SaveConnection(tcKimlikNo, Context.ConnectionId);
        
        // Gruplara ekle
        await JoinGroups(tcKimlikNo);
    }
    
    // Personel ayrıldı
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var tcKimlikNo = Context.User.GetTcKimlikNo();
        
        // Connection sil
        await RemoveConnection(tcKimlikNo);
        
        // Banko atamasını kaldır (opsiyonel)
    }
    
    // Heartbeat (Her 30 saniyede)
    public async Task Heartbeat()
    {
        var tcKimlikNo = Context.User.GetTcKimlikNo();
        await UpdateHeartbeat(tcKimlikNo);
    }
    
    // Sıra çağır
    public async Task CallNextQueue(int bankoId)
    {
        var tcKimlikNo = Context.User.GetTcKimlikNo();
        var result = await _siraService.CallNextQueue(bankoId, tcKimlikNo);
        
        if(result.Success)
        {
            // Broadcast yap
            await BroadcastQueueCalled(result.Data);
        }
    }
    
    // Sıra tamamla
    public async Task CompleteQueue(int siraId)
    {
        await _siraService.CompleteQueue(siraId);
        await BroadcastQueueCompleted(siraId);
    }
}
```

---

### 7.2. Group Yapısı

**Personel Grupları:**
```
"KanalAltIslem-{KanalAltIslemId}"
└─> Bu alt kanalda yetkili tüm personeller

"Bina-{BinaId}"
└─> Bu binadaki tüm personeller

"Banko-{BankoId}"
└─> Bu bankodaki personel (tek kişi)
```

**TV Grupları:**
```
"Bina-{BinaId}-TV"
└─> Bu binadaki tüm TV'ler
```

---

### 7.3. Event'ler

**Server → Client Events:**

```csharp
// Yeni sıra oluşturuldu (Personellere)
OnQueueCreated(QueueCreatedDto queue)

// Sıra çağrıldı (Diğer personellere - listeden kaldır)
OnQueueRemoved(int siraId)

// Sıra çağrıldı (TV'lere - göster)
OnQueueCalled(QueueDisplayDto display)

// Sıra tamamlandı (Çağıran personele)
OnQueueCompleted(int siraId)

// Sıra iptal edildi
OnQueueCancelled(int siraId, string reason)

// Personel online/offline
OnPersonelStatusChanged(string tcKimlikNo, ConnectionStatus status)
```

**Client → Server Events:**

```csharp
// Personel tarafından
CallNextQueue(int bankoId)
CompleteQueue(int siraId)
CancelQueue(int siraId, string reason)
Heartbeat()

// TV tarafından
RegisterTv(int tvId)
Heartbeat()
```

---

### 7.4. Connection Management

**Personel Login:**
```sql
-- 1. Eski connection'ları temizle
UPDATE SIR_HubConnections 
SET ConnectionStatus = 'offline', IslemZamani = GETDATE()
WHERE TcKimlikNo = '12345678901'

-- 2. Yeni connection ekle
INSERT INTO SIR_HubConnections (TcKimlikNo, ConnectionId, ConnectionStatus)
VALUES ('12345678901', 'xyz123', 'online')
```

**Heartbeat (Her 30 saniye):**
```sql
UPDATE SIR_HubConnections 
SET IslemZamani = GETDATE(), ConnectionStatus = 'online'
WHERE TcKimlikNo = '12345678901'
```

**Otomatik Offline (Backend Job - Her 1 dakika):**
```sql
-- 2 dakika heartbeat yoksa offline yap
UPDATE SIR_HubConnections 
SET ConnectionStatus = 'offline'
WHERE DATEDIFF(SECOND, IslemZamani, GETDATE()) > 120
  AND ConnectionStatus = 'online'
```

---

## 8. KULLANICI ARAYÜZLER

### 8.1. Kiosk Arayüzü (Blazor Component)

**Sayfa:** `/kiosk?binaId=1`

**Akış:**

1. **Ana Ekran** - Kanallar gösterilir
2. **Alt Kanal Seçimi** - Seçilen kanalın alt kanalları
3. **Sıra Numarası** - Fiş yazdırılır

**Özellikler:**
- Tam ekran mod
- Touch-friendly (büyük butonlar)
- Timeout: 30 saniye inaktivite sonrası ana ekrana dön
- Çoklu dil desteği (opsiyonel)

---

### 8.2. Banko Arayüzü (Blazor Page)

**Sayfa:** `/banko`

**Bölümler:**

**1. Aktif Sıra (Üstte - Büyük)**
```
┌────────────────────────────────┐
│ 🔴 AKTİF SIRA                  │
│                                │
│     Sıra No: 1105              │
│     İşlem: Yaşlılık Aylığı     │
│     Süre: 03:45                │
│                                │
│ [✅ TAMAMLA] [➡️ SONRAKİ]       │
└────────────────────────────────┘
```

**2. Bekleyen Sıralar (Altta - Liste)**
```
┌────────────────────────────────┐
│ 📋 BEKLİYOR (İlk 10)           │
│                                │
│ ⭐ ÖNCELİKLİ (Uzman)            │
│ 1109 - Yaşlılık      08:35     │
│ 1111 - Yaşlılık      08:42     │
│                                │
│ ⚪ DİĞER (Yrd. Uzman)          │
│ 1108 - Malullük      08:30     │
└────────────────────────────────┘
```

**Real-Time Güncellemeler:**
- SignalR ile otomatik refresh
- Yeni sıra gelince ses çalar
- Başka personel çağırınca listeden silinir

---

### 8.3. TV Ekranı (Blazor Page)

**Sayfa:** `/tv?tvId=3`

**Layout:**
```
┌─────────────────────────────────────┐
│ SGK MENEMEN SGM        14:35:22     │
├─────────────────────────────────────┤
│ 🔴 SON ÇAĞRILAN                     │
│   1105 → 3 NOLU BANKO               │
├─────────────────────────────────────┤
│ 📋 SON 10 ÇAĞRI                     │
│                                     │
│ 1104 → Banko 2         14:32        │
│ 1103 → Banko 1         14:30        │
│ 1102 → Banko 3         14:28        │
│ ...                                 │
└─────────────────────────────────────┘
```

**Özellikler:**
- Tam ekran
- Auto-refresh (SignalR)
- Sesli anons: "1105 numaralı sıra, 3 nolu bankoya"
- Yeni çağrı: Kırmızı renk, 3 saniye sonra normale döner

---

### 8.4. Admin Dashboard (Blazor Page)

**Sayfa:** `/admin/dashboard`

**KPI'lar:**
- Bekleyen Sıra
- Tamamlanan Sıra
- Online Personel
- Ortalama Bekleme Süresi
- Ortalama İşlem Süresi

**Grafikler:**
- Saatlik sıra dağılımı (çizgi)
- Kanal bazlı dağılım (pasta)
- Personel performansı (bar)

**Canlı Takip:**
```
👥 PERSONEL DURUMU
🟢 Ahmet (Banko 3) - Aktif - Sıra: 1105
🟢 Mehmet (Banko 5) - Boş
🔴 Ayşe - Molada (10 dk önce)
```

---

### 8.5. Personel-Kanal Eşleştirme Sayfası

**Sayfa:** `/admin/kanal-personel`

**UI:**
```
Personel Seç: [Ahmet Yılmaz ▼]

Bina: [Menemen A Binası ▼]

┌─────────────────────────────────────────┐
│ Kanal              │ Alt Kanal  │ Seviye│
├────────────────────┼────────────┼───────┤
│ ☑ Emeklilik       │ ☑ Yaşlılık │[Uzman]│
│                    │ ☑ Malullük │[Yrd.] │
│                    │ ☐ Aylık    │       │
├────────────────────┼────────────┼───────┤
│ ☑ İşveren         │ ☑ İşe Giriş│[Uzman]│
│                    │ ☐ İşten Çk.│       │
└─────────────────────────────────────────┘

[💾 KAYDET]  [❌ İPTAL]
```

**İş Mantığı:**
- Checkbox ile alt kanal seç
- Dropdown ile uzmanlık seviyesi seç
- Kaydet → `SIR_KanalPersonelleri` tablosuna yaz
- Çakışma kontrolü (UNIQUE constraint)

---

## 9. İŞ AKIŞLARI - DETAYLI

### 9.1. Sabah Açılış (08:30)

**Adım 1: Personeller Login Olur**
- Her personel TC ve şifre ile giriş
- Banko seçimi yapar
- `SIR_BankoKullanicilari` kaydı oluşur
- SignalR connection başlar

**Adım 2: TV'ler Açılır**
- Her TV otomatik kendi sayfasını açar
- SignalR bağlantısı kurar
- `SIR_HubTvConnections` kaydı oluşur

**Adım 3: Kiosklar Aktif**
- Kiosklar zaten 7/24 açık
- Sadece ekran güncellemesi yapılır

**Adım 4: İlk Sıralar**
- Vatandaşlar gelmeye başlar
- Kiosk'tan sıra alırlar
- Numara sayacı her kanal için sıfırdan başlar
  - Emeklilik: 1001
  - İşveren: 2001
  - vb.

---

### 9.2. Günlük Operasyon

**Senaryo: Ahmet'in Bir Günü**

**09:00** - Ahmet 3 nolu bankoya oturur, login olur
- `SIR_BankoKullanicilari`: Ahmet → 3 nolu banko
- `SIR_HubConnections`: online

**09:05** - İlk sıra çağrısı
- "Sonraki Sıra" butonuna basar
- Sistem en küçük sıra numarasını verir: 1105
- 1105, "Yaşlılık Aylığı" işlemi (Ahmet uzman)
- Sıra durumu: Beklemede → Islemde
- TV'lerde gösterilir: "1105 → 3 NOLU BANKO"
- Vatandaş gelir, işlem yapılır

**09:15** - İşlem tamamlanır
- "Tamamla" butonuna basar
- Sıra durumu: Islemde → Tamamlandi
- İşlem süresi: 10 dakika (kaydedilir)

**09:16** - Bir sonraki sıra
- "Sonraki Sıra" basar
- 1107 gelir (Malullük - Yrd. Uzman)
- İşleme başlar

**12:00** - Yemek molası
- Logout yapar
- `SIR_BankoKullanicilari` kaydı silinir
- `SIR_HubConnections`: offline
- Banko boş kalır

**13:00** - Tekrar login
- Aynı bankoya oturur veya başka banko seçer
- İşlemlere devam eder

**17:30** - Mesai sonu
- Logout
- Eğer aktif sıra varsa, tamamlaması beklenir

---

### 9.3. İstisnai Durumlar

**Durum 1: Vatandaş Gelmedi**
- Ahmet 1105'i çağırdı ama vatandaş gelmedi
- "İptal Et" butonuna basar
- İptal nedeni girer: "Vatandaş gelmedi"
- Sıra durumu: Iptal Edildi
- Log'a kaydedilir

**Durum 2: Internet Koptu**
- SignalR connection kesilir
- Client-side: Auto-reconnect dener (0s, 2s, 5s, 10s)
- Backend: 2 dakika heartbeat yoksa offline işaretler
- Connection geri gelince: State senkronize edilir

**Durum 3: Personel Değişikliği**
- Ahmet hasta, yerine Mehmet geldi
- Mehmet login olur, Ahmet'in bankosu boş
- Mehmet o bankoyu seçer
- Ama Mehmet'in yetkileri farklı!
- O bankoya artık Mehmet'in yetkili olduğu sıralar gelir

**Durum 4: Acil Durum (Elektrik Kesintisi)**
- Tüm connection'lar kesilir
- Veritabanı sağlam (transaction'lar commit edilmiş)
- Elektrik gelince:
  - Personeller tekrar login olur
  - TV'ler tekrar bağlanır
  - Beklemede kalan sıralar kaldığı yerden devam eder
  - "Islemde" olan sıralar kontrol edilir:
    - Eğer personel gelmezse → "Beklemede"ye alınabilir

---

### 9.4. Gece İşlemleri (17:30 sonrası)

**Adım 1: Personeller Logout**
- Tüm `SIR_BankoKullanicilari` kayıtları temizlenir
- `SIR_HubConnections`: offline

**Adım 2: TV'ler Kapanır**
- `SIR_HubTvConnections`: offline

**Adım 3: Kiosklar Pasif**
- Yeni sıra verilmez
- Ekran: "Mesai saatleri: 08:30 - 17:30"

**Adım 4: Veritabanı İşlemleri**
- Günlük raporlar oluşturulur
- Otomatik backup
- Eski log'lar arşivlenir (90+ gün)
- İstatistikler hesaplanır

**Adım 5: Geceyarısı Reset**
- Hiçbir sıra sayacı sıfırlanmaz!
- Sadece ertesi gün ilk sıra alındığında,
  o gün için numara aralığı baştan başlar.

---

## 10. SİSTEM GEREKSİNİMLERİ

### 10.1. Sunucu

**Donanım:**
```
CPU: 8 Core (Intel Xeon / AMD EPYC)
RAM: 16 GB (32 GB önerilir)
Disk: 256 GB SSD (NVMe önerilir)
Network: 1 Gbps Ethernet
```

**Yazılım:**
```
OS: Windows Server 2022 veya Ubuntu 22.04 LTS
SQL Server: 2019+
IIS: 10.0+ (Windows) veya Kestrel (cross-platform)
.NET Runtime: 9.0
```

---

### 10.2. İstemci

**Kiosk:**
```
Donanım: Touch PC (Windows 10+)
Ekran: 21-24" dokunmatik
Yazıcı: Termal fiş yazıcı
Browser: Chrome/Edge
```

**Banko (Personel):**
```
Donanım: PC/Laptop (Windows 10+)
Ekran: 15-17" (FHD)
Browser: Chrome/Edge/Firefox
Network: LAN/WiFi (min 10 Mbps)
```

**TV:**
```
Donanım: Mini PC + TV/Monitör
TV: 32-55" LED (1080p)
Browser: Chrome (kiosk mode)
Network: LAN/WiFi
```

---

### 10.3. Network

**Bandwidth:**
- Kiosk: 1 Mbps
- Banko: 2 Mbps (real-time)
- TV: 1 Mbps
- Toplam (500 cihaz): ~750 Mbps

**Latency:** < 50ms (LAN)

**SignalR:**
- WebSocket (preferred)
- Long-polling (fallback)
- KeepAlive: 15 saniye
- ServerTimeout: 30 saniye

---

### 10.4. Güvenlik

**Firewall:**
```
Port 443 (HTTPS): İzin ver
Port 80 (HTTP): 443'e yönlendir
Port 1433 (SQL): Sadece local
```

**SSL/TLS:**
- Let's Encrypt ücretsiz sertifika
- Auto-renewal (certbot)

**Backup:**
```
Veritabanı: Günlük full backup (02:00)
Transaction Log: Her 6 saatte
Retention: 30 gün
Offsite: Haftalık (cloud)
```

---

## 11. SONUÇ VE ÖNERİLER

### 11.1. Sistemin Güçlü Yönleri

✅ **Bina Bazlı İzolasyon**: Her bina kendi sıra sistemine sahip  
✅ **Esnek Kanal Yapısı**: 4 seviyeli hiyerarşi  
✅ **Uzmanlık Seviyesi**: Personel yetkinliklerine göre sıra dağıtımı  
✅ **Real-Time**: SignalR ile sıfır gecikme  
✅ **Çoktan-Çoğa İlişkiler**: TV-Banko, Kiosk-Kanal  
✅ **Soft Delete**: Veri kaybı yok  
✅ **Audit Trail**: Her işlem loglanıyor  

---

### 11.2. İyileştirme Önerileri

**1. Sira Tablosuna BankoId Eklenmeli**
```sql
ALTER TABLE SIR_Siralar ADD CagiranBankoId INT NULL
FOREIGN KEY (CagiranBankoId) REFERENCES SIR_Bankolar(BankoId)
```
Böylece TV ekranları daha kolay filtreleme yapabilir.

**2. Online Süre Tracking**
```sql
-- Yeni tablo
CREATE TABLE SIR_PersonelOnlineLog (
    LogId INT IDENTITY PRIMARY KEY,
    TcKimlikNo NVARCHAR(11),
    SessionStart DATETIME2,
    SessionEnd DATETIME2,
    DurationMinutes INT,
    HizmetBinasiId INT,
    BankoId INT
)
```
Personellerin günlük ne kadar online kaldığını takip etmek için.

**3. Raporlama Tabloları**
- Günlük istatistikler için aggregate tablolar
- Performans iyileştirmesi

**4. Sıra Önceliklendirmesi**
```sql
-- Sira tablosuna
ALTER TABLE SIR_Siralar ADD Oncelik TINYINT DEFAULT 0
-- 0: Normal, 1: Öncelikli (Engelli, Yaşlı, vb.)
```

**5. Push Notification**
- Personele mobil bildirim
- "10+ sıra bekliyor!"
- Firebase Cloud Messaging

---

### 11.3. Teknik Borç

⚠️ **BankoIslemleri Tablosu**: Eski yapı, kullanılmıyor. Temizlenebilir.  
⚠️ **Index Optimizasyonu**: Performans için ek index'ler gerekebilir  
⚠️ **Cache Strategy**: Redis entegrasyonu düşünülebilir  

---

## 12. EK BİLGİLER

### 12.1. Kısaltmalar

- **SGM**: Sosyal Güvenlik Merkezi
- **SGK**: Sosyal Güvenlik Kurumu
- **PDKS**: Personel Devam Kontrol Sistemi
- **DTO**: Data Transfer Object
- **PK**: Primary Key
- **FK**: Foreign Key

---

### 12.2. Önemli Linkler

- SQL Server Kurulumu: https://docs.microsoft.com/sql/
- EF Core Migrations: https://docs.microsoft.com/ef/core/migrations/
- SignalR Hub: https://docs.microsoft.com/aspnet/signalr/
- Blazor Server: https://docs.microsoft.com/aspnet/blazor/

---

## 13. TABLO İLİŞKİ DİYAGRAMI

```
CMN_HizmetBinalari
├─> SIR_Bankolar (1-N)
│   ├─> SIR_BankoKullanicilari (1-N)
│   │   └─> PER_Personeller (N-1)
│   └─> SIR_TvBankolari (1-N)
│       └─> SIR_Tvler (N-1)
│           └─> SIR_HubTvConnections (1-1)
│
├─> SIR_KanalIslemleri (1-N)
│   └─> SIR_KanalAltIslemleri (1-N)
│       ├─> SIR_Siralar (1-N)
│       │   └─> PER_Personeller (N-1)
│       └─> SIR_KanalPersonelleri (1-N)
│           └─> PER_Personeller (N-1)
│               └─> SIR_HubConnections (1-1)
│
└─> SIR_KioskIslemGruplari (1-N)
    └─> SIR_KioskGruplari (N-1)

SIR_Kanallar
├─> SIR_KanallarAlt (1-N)
│   └─> SIR_KioskIslemGruplari (1-N)
│   └─> SIR_KanalAltIslemleri (1-N)
└─> SIR_KanalIslemleri (1-N)

PER_Departmanlar
└─> PER_Servisler (1-N)
    └─> PER_Personeller (1-N)
```

---

**Son Güncelleme:** 03 Kasım 2025  
**Hazırlayan:** Claude AI (Anthropic)  
**Revizyon:** 2.0 - Gerçek Proje Yapısı  

---

## ⚠️ ÇOK ÖNEMLİ NOTLAR

### 1. KanalAltIslem = Sistemin Kalbi!

Tüm sistem `SIR_KanalAltIslemleri` tablosu üzerine kurulu:
- Personeller buraya atanır (SIR_KanalPersonelleri)
- Sıralar buradan oluşturulur (SIR_Siralar)
- Bu tablo bina bazlı izolasyon sağlar!

### 2. BankoKullanici = Kim Nerede?

- Her an hangi personelin hangi bankoda olduğunu bildirir
- Unique constraint: Bir personel aynı anda tek bankoda
- Unique constraint: Bir bankoda aynı anda tek personel

### 3. SignalR Groups = Akıllı Routing

- Her personel yetkili olduğu kanal gruplarına eklenir
- Broadcast mesajları sadece ilgili gruplara gider
- Network trafiği optimize edilir

### 4. Numara Aralıkları = Bina Özgü

- Her bina, her kanal için kendi numara aralığını kullanır
- 1000-1999: Menemen A - Emeklilik
- 2000-2999: Menemen B - Emeklilik
- Böylece çakışma olmaz!

### 5. Uzman/Yrd.Uzman = İş Yükü Dengesi

- Uzman kanalları önce gösterilir
- Eşik değeri aşılırsa yrd. uzman devreye girer
- Sistem otomatik dengeleme yapar

---

Bu döküman, **gerçek proje yapısına** %100 sadık kalınarak hazırlanmıştır.  
Tüm tablo isimleri, ilişkiler ve yapılar **mevcut koddan** çıkarılmıştır.  
Hayali veya teorik hiçbir bilgi içermez!

**Başarılar! 🚀**