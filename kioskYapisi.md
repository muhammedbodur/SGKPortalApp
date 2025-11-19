# 🏷️ Kiosk Yapısı – Yol Haritası
---
## 1. Hedefler

1. Vatandaşın ilk gördüğü **ana menü** (Ekran 2) için merkezi tanım ve sürükle-bırak yönetim arayüzü kurmak.
2. Her **departman / hizmet binası / kiosk** kombinasyonu için hangi menü öğelerinin gösterileceğini ve sırasını belirlemek.
3. Ana menüler ile mevcut `SIR_KanalAltIslemleri` arasındaki bağı tanımlayıp tekrar kullanılabilir hale getirmek.
4. Mevcut EF/DTO/Service pattern’ini bozmayacak şekilde yeni tabloları BusinessObjectLayer’a eklemek.

---

## 2. Mevcut Yapının Temizliği

1. `KioskGrup`, `KioskIslemGrup` ve bunlara bağlı repository/DTO/service katmanlarını doğrudan kaldır (deprecated işaretlemeden). Migration’ı kullanıcı alacağı için sadece kod/ref dosyalarını temizle.
2. Migration’a hazırlık için bu tabloların verilerini yedekleme ihtiyacını değerlendir (gerekirse script). Yeni yapıya geçerken hangi verilerin taşınacağına karar ver.
3. `KanalAltIslem` içindeki `KioskIslemGrupId` kolonunu yeni modeldeki karşılığı (`KioskKanalAltIslem`) devreye girdiğinde kaldırılacak şekilde planla.

---

## 3. Yeni Veri Modeli

### 3.1. Ana Menü Şablonu

| Tablo | Açıklama |
| --- | --- |
| **`SIR_KioskMenu`** | Ana menü şablonu başlığı. Alanlar: `KioskMenuId`, `MenuAdi`, `Aciklama`, `Aktiflik`. Kart görselleri/ikon gibi veriler backend’de tutulmayacak, masaüstü uygulaması kendi default setini kullanacak. |

### 3.2. Kiosk Tanımı ve Bina Eşlemesi

| Tablo | Açıklama |
| --- | --- |
| **`SIR_Kiosk`** | Hizmet binasına bağlı fiziksel veya mantıksal kiosk kaydı. Alanlar: `KioskId`, `HizmetBinasiId`, `KioskAdi`, `KioskMenuId`, `KioskIp`, `Aktiflik`. |
| **`SIR_KioskIslemleri`** | Kioska atanacak menü öğelerinin listesi. Alanlar: `KioskIslemId`, `KioskId`, `KioskMenuId` (veya ilgili şablon referansı), `MenuSira`, `Aktiflik`. `MenuSira` kioska özel sıralamayı sağlar. |

### 3.3. Menü → Alt Kanal Köprüsü

| Tablo | Açıklama |
| --- | --- |
| **`SIR_KioskKanalAltIslem`** | (Kiosk içindeki menü öğesi → KanalAltIslem) eşleştirmesi. Alanlar: `KioskKanalAltIslemId`, `KioskIslemId`, `KanalAltIslemId`, `Aktiflik`. Bu sayede Ekran 3’teki liste otomatik oluşur.

> Not: Tablo isimleri kullanıcı tarafından önerildi: `KioskMenu`, `Kiosk`, `KioskIslemleri`, `KioskKanalAltIslem`. EF tarafında sınıf isimlerini de buna göre belirleyeceğiz. Menüler için ayrı `KioskMenuOge` tablosu olmayacak; kart görselleri/ikonları masaüstü uygulamasının kendi konfigürasyonunda tutulacak.

---

## 4. Katmanlara Eklenmesi Gerekenler

1. **Entities (BusinessObjectLayer/Entities/SiramatikIslemleri)**
   - `KioskMenu`, `Kiosk`, `KioskIslemleri`, `KioskKanalAltIslem` sınıflarını AuditableEntity’den türet.
   - Navigation property’leri `[InverseProperty]` ile tanımla.

2. **DTO’lar**
   - Request (Create/Update) DTO’ları `DTOs/Request/SiramatikIslemleri` altına ekle.
   - Response DTO’ları `DTOs/Response/SiramatikIslemleri` altına ekle (liste ve detay varyantları).

3. **Repositories**
   - Her entity için `I...Repository` interface ve `...Repository` concrete (GenericRepository’den türeyen) oluştur.
   - Özel sorgu ihtiyaçları: departman/bina bazlı menü listesi, kioska göre aktif menü öğeleri vb.

4. **Services**
   - `IKioskMenuService`, `IKioskManagementService` gibi arabirimler; BusinessLogicLayer’da uygulamaları.
   - Servisler repository’leri DI üzerinden kullanmalı.

5. **Presentation Layer**
   - Sol nav: “Kiosk Tanımları”, “Kiosk İşlemleri”, “Bina Bazlı Menü İçerikleri” sayfaları.
   - UI bileşenleri: Kart grid (Alt Kanal Yönetimi ekranına benzer), modal form’lar, sürükle-bırak sıralama opsiyonu.
   - `Pages` klasöründe her ekran için `.razor` + `.razor.cs` (code-behind) yapısı korunacak; mevcut component/service injection pattern’iyle uyumlu kalınacak.

---

## 5. İş Akışı

1. **Kiosk Menü Tanımı Oluşturma**
   - `KioskMenu` sadece layout/grid ve hangi kart kodlarının kullanılacağını belirler; kart içeriği masaüstü uygulamasındaki konfigürasyondan okunur.
   - Backend, masaüstü uygulamasının “kart kodu” listesine referans verir; görsel ve ikon tarafı masaüstü uygulamasında güncellenir.

2. **Kiosk Tanımı**
   - Departman + Hizmet Binasi için kiosk kaydı açılır; hangi menü şablonunu kullanacağı seçilir.
   - İsteğe göre cihaz bilgileri (IP, kiosk kodu) girilir.

3. **Kiosk İşlemleri Yönetimi**
   - `KioskIslemleri` kayıtlarında seçilen menü şablonundaki öğeler kioska atanır, `MenuSira` değeri kioska özel tutulur.

4. **Menü → Alt Kanal Eşleştirmesi**
   - `KioskKanalAltIslem` ekranında ilgili `KioskIslem` kaydı seçilir; hangi `KanalAltIslemleri` açacağı belirlenir. Eşleştirme yapılırken `KanalAltIslem.HizmetBinasiId` ile kioskun bağlı olduğu bina eşleşmesi doğrulanır.
   - Ekstra display metni/sırası tutmaya gerek yoktur; sunum logic’i mevcut kanal adlarını kullanır.

5. **Kiosk Masaüstü Uygulaması**
   - API’den `Kiosk` ve `KioskIslemleri` verilerini çekip Ekran 2’yi oluşturur.
   - Vatandaş butona bastığında `KioskKanalAltIslem` kayıtlarına göre Ekran 3 listesi render edilir.

---

## 6. Migration & Geçiş Planı

1. Yeni tablolar için migration hazırlarken mevcut kiosk tablolarını kaldır.
2. Gerekirse eski tablolardan veri taşımak için script yaz (örneğin KioskGrup → KioskMenu dönüşümü).
3. API/Service katmanında yeni endpoint’ler eklenene kadar eski endpoint’leri kapatma; iki yapı paralel çalışabilir.
4. Masaüstü kiosk uygulaması yeni API’yi tüketmeye hazır olduğunda eski tablo referansları temizlenir.

---

## 7. Açık Konular

1. `LayoutJson` yapısı nasıl olacak? (Örn. 3x4 grid vs responsive). Tasarım onayı bekleniyor.
2. Çoklu dil desteği: `DisplayText` alanlarını culture bazlı hale getirmek gerekiyor mu?
3. Offline mod senaryosu: Masaüstü uygulaması veriyi ne sıklıkta cache’leyecek?

---

Bu plan onaylandığında entity/DTO/repository/service dosyaları oluşturularak development’a geçilebilir.
