# 🏦 BANKO YAPISI - ÖZET DOKÜMANTASYON

**Tarih:** 03 Kasım 2025  
**Proje:** SGK İzmir İl Müdürlüğü Sıramatik Sistemi  

---

## 1. BANKO NEDİR?

**Banko = Kişiye Ait Çalışma Noktası**

- Fiziksel bir masa/hizmet noktası
- İçindeki personel değişken
- Bankoya gelen sıralar = O anda oturan personelin yetkileri
- Bir personel aynı anda sadece 1 bankoda
- Bir bankoda aynı anda sadece 1 personel

---

## 2. VERİTABANI TABLOLARI

### SIR_Bankolar (Master)
- BankoId, HizmetBinasiId, KatTipi, BankoNo
- BankoTipi (Normal, Oncelikli, Engelli, SefMasasi)
- BankoAktiflik, BankoAciklama, BankoSira
- **Unique:** (HizmetBinasiId, BankoNo)

### SIR_BankoKullanicilari (Atama)
- BankoKullaniciId, BankoId, TcKimlikNo
- AtanmaTarihi
- **Unique:** BankoId (bir bankoda 1 personel)
- **Unique:** TcKimlikNo (bir personel 1 bankoda)

### SIR_BankoHareketleri (Log)
- BankoHareketId, BankoId, PersonelTcKimlikNo
- SiraId, SiraNo, KanalId, KanalAltId
- IslemBaslamaZamani, IslemBitisZamani, IslemSuresiSaniye

---

## 3. BANKO NUMARALAMA

**Bina Bazlı Sürekli Numaralama:**

```
Menemen SGM A Binası:
├─ Zemin Kat: 1-10
├─ 1. Kat: 11-20
└─ 2. Kat: 21-30

Menemen SGM B Binası:
└─ Zemin Kat: 1-10 (yeni binada tekrar 1'den)

Menderes SGM Binası:
├─ Zemin Kat: 1-10
├─ 1. Kat: 1-5
└─ 2. Kat: 1-15
```

---

## 4. SIRA YÖNLENDİRME

### Sira Tablosuna Eklenen Alanlar:
- YonlendirildiMi (bool)
- YonlendirmeBankoId (hangi bankodan)
- YonlendirenPersonelTc (kim yönlendirdi)
- HedefBankoId (nereye)
- YonlendirmeZamani
- YonlendirmeNedeni
- YonlendirmeTipi (BaskaBanko, Sef, UzmanPersonel)

### Yönlendirme Senaryosu:
1. Ahmet (5 nolu banko) 1105'i çağırdı
2. Evrak eksik, 8 nolu bankoya (Mehmet - Uzman) yönlendirdi
3. Sıra durumu: İşlemde → Beklemede
4. Mehmet'in ekranında 1105 ÖNCELİKLİ görünür
5. Mehmet "Sonraki Sıra" dediğinde 1105 gelir

---

## 5. SIRA ÇAĞIRMA ÖNCELİĞİ

```
ÖNCELİK 1: Yönlendirilmiş Sıralar
└─ En eski yönlendirme önce

ÖNCELİK 2: Normal Sıralar
├─ Uzman olduğu işlemler önce
└─ En küçük sıra numarası önce
```

---

## 6. İŞ KURALLARI

### Banko Atama:
- Personelin eski ataması varsa önce silinir
- Hedef banko boş olmalı
- Personel ve banko aktif olmalı

### Yönlendirme:
- Sıra "İşlemde" olmalı
- Yönlendiren personel o sırayı çağırmış olmalı
- Hedef banko aktif ve personelli olmalı
- Tekrar yönlendirme yapılamaz

### Personel Çıkış:
- BankoKullanici kaydı silinir
- Banko boşalır

---

## 7. UI YAPISI

### Banko Yönetimi:
- Kat bazlı gruplu gösterim
- Boş/Dolu/Pasif renk kodları
- Tıklayarak personel atama

### Personel Ekranı:
- Aktif sıra bilgisi
- Yönlendirme butonu
- Hedef banko/şef seçimi

### Login Ekranı:
- Boş bankolar listesi
- Banko seçimi

---

## 8. BANKO CRUD - SNEAT TEMPLATE UYUMLU TASARIM

### 8.1. Sayfa Yapısı (Kat Bazlı Accordion + Table)

**URL:** `https://localhost:8080/siramatik/banko-yonetimi`
**Tasarım Pattern:** Mevcut modüllerle (Kanal İşlem, Personel Atama) uyumlu
**Özellikler:**
- Bootstrap 5 Accordion (Kat bazlı)
- Responsive Table
- Dropdown menü (İşlemler)
- Modal'lar (Ekle, Düzenle, Ata)
- Sneat template class'ları

**Ana Bileşenler:**
- Header (Başlık + Yeni Banko Ekle butonu)
- Filtre Card (Hizmet Binası seçimi + İstatistikler)
- Kat Bazlı Accordion (Her kat için ayrı accordion item)
- Table (Her katta banko listesi)
- Dropdown menü (Personel Ata, Düzenle, Aktif/Pasif, Sil)

**Görünüm:**
- Zemin Kat (Açık - default)
  - Table: Banko No | Tip | Durum | Personel | Açıklama | İşlemler
- 1. Kat (Kapalı)
- 2. Kat (Kapalı)

### 8.2. Yeni Banko Ekleme Modal (Bootstrap Modal)

**Alanlar:**
- Hizmet Binası (Dropdown - Required)
- Kat (Dropdown - ZeminKat, BirinciKat, IkinciKat, UcuncuKat)
- Banko Numarası (Number Input - 1-999 arası)
- Banko Tipi (Dropdown - Normal, Oncelikli, Engelli, SefMasasi)
- Açıklama (Text Input - Optional)

**Validasyon:**
- DataAnnotationsValidator
- BankoNo unique kontrolü (aynı binada)

**Butonlar:**
- İptal (btn-secondary)
- Kaydet (btn-primary + spinner)

### 8.3. Banko Düzenleme Modal

**Alanlar:**
- Banko Numarası (Readonly - Değiştirilemez)
- Kat (Readonly - Değiştirilemez)
- Banko Tipi (Dropdown - Değiştirilebilir)
- Açıklama (Text Input)
- Durum (Checkbox - Aktif/Pasif)

**Butonlar:**
- Sil (btn-danger)
- İptal (btn-secondary)
- Güncelle (btn-primary)

### 8.4. Personel Atama Modal (Modal-lg)

**Özellikler:**
- Arama input (Real-time filtering)
- Servis filtresi (Dropdown)
- Sadece aktif personeller checkbox
- List group (Scrollable - max-height: 400px)
- Avatar gösterimi
- Seçili personel highlight (active class)

**Personel Item:**
- Avatar (İlk harf)
- Ad Soyad (Strong)
- Servis - Ünvan (Small, muted)
- Check icon (Seçiliyse)

**Butonlar:**
- İptal (btn-secondary)
- Ata (btn-primary + disabled if no selection)

### 8.5. Personel Çıkarma Onayı (Confirmation Modal)

**İçerik:**
- Uyarı icon (⚠️)
- Personel adı + Banko numarası
- Sonuç açıklaması (Alert warning)
  - Banko boşalacak
  - Personel çıkış yapmış sayılacak
  - Artık bu bankodan sıra gelmeyecek

**Butonlar:**
- İptal (btn-secondary)
- Evet, Çıkar (btn-danger + spinner)

---

## 9. API ENDPOINT'LERİ

### 9.1. Banko CRUD

```
GET    /api/banko                          - Tüm bankoları listele
GET    /api/banko/{id}                     - Banko detayı
GET    /api/banko/bina/{binaId}            - Bina bazlı bankolar (düz liste)
GET    /api/banko/bina/{binaId}/grouped    - Bina bazlı kat gruplu
POST   /api/banko                          - Yeni banko ekle
PUT    /api/banko/{id}                     - Banko güncelle
DELETE /api/banko/{id}                     - Banko sil (soft delete)
```

### 9.2. Banko Atama

```
POST   /api/banko/{bankoId}/ata            - Personel bankoya ata
DELETE /api/banko/{bankoId}/cikar          - Personeli bankodan çıkar
GET    /api/banko/bos/{binaId}             - Boş bankoları listele
GET    /api/banko/personel/{tcKimlikNo}    - Personelin şu anki bankosu
```

### 9.3. Sıra Yönlendirme

```
POST   /api/sira/{siraId}/yonlendir        - Sırayı yönlendir
GET    /api/sira/yonlendirilen/{bankoId}   - Bankoya yönlendirilmiş sıralar
```

---

## 10. DTO YAPILARI

### 10.1. Request DTO'lar

```csharp
// Banko Oluşturma
public class BankoCreateRequestDto
{
    public int HizmetBinasiId { get; set; }
    public KatTipi KatTipi { get; set; }
    public int BankoNo { get; set; }
    public BankoTipi BankoTipi { get; set; }
    public string? BankoAciklama { get; set; }
}

// Banko Güncelleme
public class BankoUpdateRequestDto
{
    public BankoTipi BankoTipi { get; set; }
    public Aktiflik BankoAktiflik { get; set; }
    public string? BankoAciklama { get; set; }
}

// Personel Atama
public class BankoPersonelAtaDto
{
    public int BankoId { get; set; }
    public string TcKimlikNo { get; set; }
}

// Sıra Yönlendirme
public class SiraYonlendirmeDto
{
    public int SiraId { get; set; }
    public string YonlendirenPersonelTc { get; set; }
    public int YonlendirmeBankoId { get; set; }
    public int HedefBankoId { get; set; }
    public YonlendirmeTipi YonlendirmeTipi { get; set; }
    public string? YonlendirmeNedeni { get; set; }
}
```

### 10.2. Response DTO'lar

```csharp
// Banko Response
public class BankoResponseDto
{
    public int BankoId { get; set; }
    public int HizmetBinasiId { get; set; }
    public string HizmetBinasiAdi { get; set; }
    public KatTipi KatTipi { get; set; }
    public string KatTipiAdi { get; set; }
    public int BankoNo { get; set; }
    public BankoTipi BankoTipi { get; set; }
    public string BankoTipiAdi { get; set; }
    public Aktiflik BankoAktiflik { get; set; }
    public string? BankoAciklama { get; set; }
    
    // Atanmış personel bilgisi
    public PersonelAtamaDto? AtananPersonel { get; set; }
    public bool BankoMusaitMi { get; set; }
}

// Personel Atama Bilgisi
public class PersonelAtamaDto
{
    public string TcKimlikNo { get; set; }
    public string AdSoyad { get; set; }
    public string ServisAdi { get; set; }
    public DateTime AtanmaTarihi { get; set; }
}

// Kat Gruplu Response
public class BankoKatGrupluResponseDto
{
    public KatTipi KatTipi { get; set; }
    public string KatTipiAdi { get; set; }
    public List<BankoResponseDto> Bankolar { get; set; }
}
```

---

## 11. VALIDASYON KURALLARI

### Banko Oluşturma:
- BankoNo: 1-999 arası olmalı
- (HizmetBinasiId, BankoNo) unique olmalı
- HizmetBinasi aktif olmalı

### Banko Güncelleme:
- Banko mevcut olmalı
- Silinmemiş olmalı

### Personel Atama:
- Banko aktif olmalı
- Banko boş olmalı
- Personel aktif olmalı
- Personel başka bankoda olmamalı

### Sıra Yönlendirme:
- Sıra "İşlemde" olmalı
- Yönlendiren personel sırayı çağırmış olmalı
- Hedef banko aktif ve personelli olmalı
- Sıra daha önce yönlendirilmemiş olmalı
