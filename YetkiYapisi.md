# Yetki Yapısı (SGKPortalApp)

Bu doküman; SGKPortalApp içerisindeki **Yetki / PersonelYetki / ModulControllerIslem** temelli yetkilendirme yapısının nasıl kurgulanacağını, `/personel` ve alt sayfaları için örnek **permission key** setini ve UI seviyesinde **Gör / Düzenle / Sil** davranışlarını tanımlar.

> Not: Projede `.gitignore` içinde `*.md` ignore edildiği için bu dosyanın takip edilebilmesi adına repo kök `.gitignore` içine `!YetkiYapisi.md` istisnası eklenmiştir.

## Hedef Özellikler (Unutma Listesi)
- **DB-Driven Permission Catalog**: Yetki tanımı (permission key) DB’de tutulur; yeni bir key eklemek için kod değişikliği gerektirmez.
- **Dinamik ve hiyerarşik key standardı**: Sayfa → tab/section → field → action seviyesine kadar inebilir; inheritance + override destekler.
- **UI davranışı standardı**: `None` = gizle, `View` = text/readonly, `Edit` = input, `Delete` = destructive aksiyon.
- **Server-side enforcement**: UI gizlese bile server, yetkisiz alan/buton/endpoint çağrısını engeller; yetkisiz alan update’de korunur.
- **Performanslı değerlendirme**: Render sırasında DB’ye tekrar tekrar gitmez; kullanıcı bazlı permission snapshot memory’den değerlendirilir.
- **Ölçeklenebilir cache**: Cache tüm kullanıcılar için değil, yalnızca aktif oturumlar/circuit’ler için oluşur; TTL/eviction ile bellek kontrol edilir.
- **Anında yansıma (relogin olmadan)**: Yetki değişince `PermissionStamp` artar + `PermissionsChanged` push ile UI cache/state temizlenip yeniden yüklenir.
- **Hard refresh dayanıklılığı**: Hard refresh yeni circuit oluşturur; permission snapshot yeniden yüklenir.
- **İleriye dönük ABAC uyumu**: Gerekirse departman/hizmet binası gibi koşullarla genişletilebilir.

 Bu dokümanın hedefi:
 - Yetkilerin hem **sayfa/endpoint** seviyesinde hem de **tab/section/field** seviyesinde yönetilebilmesi
 - UI davranışı (render/edit) ile API davranışının (update/patch) **tutarlı ve güvenli** olması
 - Yetkiler değiştiğinde kullanıcıya **anında yansıması** (cache/claim stratejisi)

---

## 1) Mevcut Yapı (Entity’ler)

### 1.1 Yetki (PER_Yetkiler)
- Amaç: Sistem içindeki yetki tanımlarını tutar.
- Hiyerarşi destekler: `UstYetkiId` ile **Ana / Orta / Alt** yetkiler.
- Önemli alanlar:
  - `YetkiId`
  - `YetkiTuru` (`AnaYetki`, `OrtaYetki`, `AltYetki`)
  - `YetkiAdi` (Permission Key olarak kullanılabilir)
  - `Aciklama`
  - `ControllerAdi`, `ActionAdi` (opsiyonel; ileri seviye mapping için)

### 1.2 PersonelYetki (PER_PersonelYetkileri)
- Amaç: Bir personele belirli bir yetkinin hangi seviyede verildiğini tutar.
- Önemli alanlar:
  - `TcKimlikNo`
  - `YetkiId`
  - `ModulControllerIslemId`
  - `YetkiTipleri` (Gör/Görme/Düzenle)

> DB tarafında aynı personele aynı `YetkiId` için tek kayıt olacak şekilde unique index var.

### 1.3 Modul / ModulController / ModulControllerIslem
- Amaç: Yetkileri “modül → controller → işlem” hiyerarşisinde gruplayabilmek.
- En granular seviye: `ModulControllerIslem`.

---

## 2) Yetki Seviyeleri (YetkiTipleri)

Projede `YetkiTipleri` enum’u şu an:
- `None` = Yetkisiz (UI’de gösterme, update’de koru)
- `View` = Gör (render edilebilir)
- `Edit` = Düzenle (kullanıcı değer değiştirebilir)
- `Delete` = Sil (destructive aksiyon)

Bu enum pratikte “bir resource üzerinde maksimum izin seviyesi” gibi kullanılabilir.

### 2.1 Delete (Sil) için önerilen kural
Delete aksiyonlarını **ayrı permission key** olarak tanımlayın (örn: `PER.PERSONEL.DELETE`).

Önerilen değerlendirme:
 - Bir buton/endpoint “Sil” ise: ilgili key için `YetkiTipleri.Delete` gerekir.
 - “Edit” yetkisi otomatik olarak “Delete” anlamına gelmemelidir.

---

## 3) UI Davranış Kuralları (Gör / Düzenle / Sil)

### 3.1 Liste ve Detay Sayfaları
- Kullanıcı `View` yetkisine sahipse:
  - İlgili alan/kolon gösterilir.
- Kullanıcı `View` yetkisine sahip değilse:
  - Alan/kolon hiç render edilmez.

### 3.1.1 Buton / Aksiyon (Action) bazlı yetki (Delete dahil)
Butonlar (örn. **Sil**, **Düzenle**, **Excel’e Aktar**) birer “aksiyon”dur ve her biri için ayrı permission key tanımlanabilir.

Önerilen kural:
 - Butonun görünmesi (UI): ilgili key için en az ilgili seviye yetki olmalı
 - Butonun çalışması (API): ilgili endpoint/handler da aynı key ile server-side kontrol edilmeli

Örnekler:
 - “Sil” butonu:
   - Key: `PER.PERSONEL.DELETE`
   - Gerekli seviye: `YetkiTipleri.Delete`
 - “Düzenle” butonu:
   - Key: `PER.PERSONEL.EDIT`
   - Gerekli seviye: `YetkiTipleri.Edit`
 - “Detay” butonu:
   - Key: `PER.PERSONEL.DETAIL`
   - Gerekli seviye: `YetkiTipleri.View`

### 3.2 Düzenleme (Manage) Sayfası – Input Seviyesi
- Kullanıcı `Edit` yetkisine sahipse:
  - Input aktif (normal edit)
- Kullanıcı sadece `View` yetkisine sahipse:
  - Input yerine düz metin/readonly gösterim
- Kullanıcı `None` ise:
  - Alan tamamen gizlenir.

Örnek (senaryon):
 - Sayfa: `https://localhost:8080/personel/manage/16406457430`
 - Alan: `Personel.Adres`
   - Yetki: `PER.PERSONEL.FIELD.ADRES.EDIT` = `Edit` ise input
   - Yetki: `...` = `View` ise text
   - Yetki: `...` = `None` ise hiç render edilmez

### 3.3 “Yetkisiz alanlar bozulmadan güncellensin” kuralı
Bu kuralı sağlamak için önerilen yaklaşım:
- UI’de yetkisiz alanları kullanıcı değiştiremeyecek şekilde göster (readonly/text/hidden)
- API’ye update gönderirken:
  - “Yetkili alanlar” gönderilir
  - “Yetkisiz alanlar” ya hiç gönderilmez (patch modeli) ya da server tarafında eski değer korunur

Kritik güvenlik notu:
 - UI’nin alanı gizlemesi tek başına yeterli değildir.
 - Kullanıcı devtools ile request payload’ını manipüle edebilir.
 - Bu nedenle **server-side** olarak “hangi alanlar güncellenebilir?” kontrolü yapılmalıdır.

Pratik seçenekler:
- **Seçenek A (önerilen):** Update DTO’larını “patch” gibi tasarlayıp sadece değişen alanları gönder.
- **Seçenek B:** Server-side update’te “null/empty gelen alanları overwrite etme” gibi koruma kuralları.

Profesyonel öneri (field-level enforcement):
 - Persisted entity’yi DB’den yükle
 - Gelen DTO’yu entity’ye map ederken sadece **Edit yetkisi olan alanları** uygula
 - `View` veya `None` olan alanlar için:
   - Request’ten gelse bile ignore et
   - DB’deki değer korunur

Bu sayede `None` olan alan:
 - UI’de hiç görünmez
 - Update çağrısında **aynı kalır**

---

## 4) Permission Key Standardı

### 4.1 Naming Convention
Önerilen format:
- `{MODULE}.{RESOURCE}.{ACTION}`
- veya daha granular:
  - `{MODULE}.{RESOURCE}.{SECTION}.{FIELD}.{ACTION}`

Tavsiye edilen hiyerarşik path yaklaşımı:
 - En üst seviye: `PER.PERSONEL`
 - Sayfa/feature: `PER.PERSONEL.MANAGE`
 - Tab/section: `PER.PERSONEL.MANAGE.TAB.ILETISIM`
 - Alan: `PER.PERSONEL.MANAGE.FIELD.EMAIL`
 - Aksiyon: `.VIEW` / `.EDIT` / `.DELETE`

Örnekler:
 - `PER.PERSONEL.MANAGE.VIEW`
 - `PER.PERSONEL.MANAGE.EDIT`
 - `PER.PERSONEL.MANAGE.FIELD.EMAIL.VIEW`
 - `PER.PERSONEL.MANAGE.FIELD.EMAIL.EDIT`
 - `PER.PERSONEL.DELETE.DELETE`

Bu dokümanda kısaltma:
- `PER` = Personel işlemleri modülü

---

## 5) /personel Modülü – Örnek Yetkiler

### 5.1 Personel Liste (GET /personel)
- `PER.PERSONEL.LIST` (View)
- `PER.PERSONEL.EXPORT.EXCEL` (View)
- `PER.PERSONEL.EXPORT.PDF` (View)

UI örnekleri:
- “Yeni Personel Ekle” butonu: `PER.PERSONEL.CREATE`
- “Detay” butonu: `PER.PERSONEL.DETAIL`
- “Düzenle” butonu: `PER.PERSONEL.EDIT`
- “Sil” butonu: `PER.PERSONEL.DELETE` (Delete)

### 5.2 Personel Detay (GET /personel/detail/{tc})
- `PER.PERSONEL.DETAIL` (View)
- “Düzenle” butonu için: `PER.PERSONEL.EDIT`

### 5.3 Personel Manage (GET/POST /personel/manage)
- `PER.PERSONEL.CREATE` (Edit)
- `PER.PERSONEL.EDIT` (Edit)

#### 5.3.1 Tab bazlı önerilen alt yetkiler
**Personel Tab**
- `PER.PERSONEL.TAB.PERSONEL.VIEW`
- `PER.PERSONEL.TAB.PERSONEL.EDIT`

**İletişim Tab**
- `PER.PERSONEL.TAB.ILETISIM.VIEW`
- `PER.PERSONEL.TAB.ILETISIM.EDIT`

**Kişisel Tab**
- `PER.PERSONEL.TAB.KISISEL.VIEW`
- `PER.PERSONEL.TAB.KISISEL.EDIT`

**Özlük Tab**
- `PER.PERSONEL.TAB.OZLUK.VIEW`
- `PER.PERSONEL.TAB.OZLUK.EDIT`

**Eş/Çocuk Tab**
- `PER.PERSONEL.TAB.ES_COCUK.VIEW`
- `PER.PERSONEL.TAB.ES_COCUK.EDIT`

**Hizmet Tab**
- `PER.PERSONEL.TAB.HIZMET.VIEW`
- `PER.PERSONEL.TAB.HIZMET.EDIT`

**Eğitim Tab**
- `PER.PERSONEL.TAB.EGITIM.VIEW`
- `PER.PERSONEL.TAB.EGITIM.EDIT`

**Yetki Tab**
- `PER.PERSONEL.TAB.YETKI.VIEW`
- `PER.PERSONEL.TAB.YETKI.EDIT`

**Ceza Tab**
- `PER.PERSONEL.TAB.CEZA.VIEW`
- `PER.PERSONEL.TAB.CEZA.EDIT`

**Engel Tab**
- `PER.PERSONEL.TAB.ENGEL.VIEW`
- `PER.PERSONEL.TAB.ENGEL.EDIT`

**Fotoğraf Tab**
- `PER.PERSONEL.TAB.FOTOGRAF.VIEW`
- `PER.PERSONEL.TAB.FOTOGRAF.EDIT`

> İstersen daha da granular: field bazlı key’ler. (Örn: `PER.PERSONEL.FIELD.EMAIL.EDIT`)

---

## 6) Departman Sayfaları – Örnek Yetkiler

### 6.1 Departman Liste (GET /personel/departman)
- `PER.DEPARTMAN.LIST` (View)
- `PER.DEPARTMAN.EXPORT.EXCEL` (View)
- `PER.DEPARTMAN.EXPORT.PDF` (View)

Butonlar:
- Yeni: `PER.DEPARTMAN.CREATE`
- Düzenle: `PER.DEPARTMAN.EDIT`
- Aktif/Pasif: `PER.DEPARTMAN.TOGGLE_ACTIVE`
- Sil: `PER.DEPARTMAN.DELETE`

### 6.2 Departman Manage (GET/POST /personel/departman/manage)
- `PER.DEPARTMAN.CREATE` (Edit)
- `PER.DEPARTMAN.EDIT` (Edit)
- Sil alanı: `PER.DEPARTMAN.DELETE`

---

## 7) Yeni Yetki Nasıl Eklenir? (Önerilen Akış)

### 7.1 DB’ye Yetki Tanımı (Yetki)
1. `PER_Yetkiler` tablosuna yeni kayıt ekle:
   - `YetkiAdi`: yeni permission key
   - `YetkiTuru`: hiyerarşide uygun seviye
   - `UstYetkiId`: bağlı olduğu üst yetki
   - `Aciklama`: anlamlı açıklama

### 7.2 Modul/Controller/Islem ekleme
1. `PER_Moduller` → (örn. `Personel`)
2. `PER_ModulControllers` → (örn. `PersonelController`, `DepartmanController`)
3. `PER_ModulControllerIslemleri` → (örn. `List`, `Manage`, `Delete`)

> Not: Şu an API controller’larınızda `ControllerAdi/ActionAdi` alanları aktif kullanılmıyor. İsterseniz bunu ileride gerçek endpoint mapping’e de bağlayabiliriz.

### 7.3 Personele Yetki Atama (PersonelYetki)
1. `PER_PersonelYetkileri` tablosuna kayıt:
   - `TcKimlikNo`
   - `YetkiId`
   - `ModulControllerIslemId`
   - `YetkiTipleri`: None/View/Edit

---

## 8) Test Senaryoları (Hızlı Checklist)

### 8.1 Liste sayfası
- Kullanıcıda `PER.PERSONEL.LIST` yok → `/personel` menüsü görünmesin veya sayfa erişimi engellensin.
- `LIST` var ama `DELETE` yok → “Sil” butonu görünmesin.
- `LIST` var ama `EDIT` yok → “Düzenle” butonu görünmesin.

### 8.2 Manage sayfası
- `EDIT` yok ama `VIEW` var → input yerine metin görünsün.
- `VIEW` yok → tab/alan gizlensin.

---

## 9) Uygulama Notu (Kod Tarafı)

Bu doküman yalnızca yapı/standardı tanımlar. Bunu kodda işletmek için önerilen minimum teknik bileşenler:
- `PermissionService` (TcKimlikNo → permission cache)
- Razor için helper/component:
  - `PermissionView` (render/hide)
  - `PermissionField` (edit/text/hide)
- API tarafında kritik endpoint’lere `[Authorize]` + kontrol (opsiyonel, mimariye göre)

---

## 10) Yetkiler Değiştiğinde Anında Yansıma (Gerçek Zamanlı Senaryo)

İhtiyaç: Bir kullanıcının yetkileri admin tarafından değiştirildiğinde kullanıcı sayfayı refresh etmeden (veya en azından **bir sonraki request’te**) yeni yetkilerin uygulanması.

### 10.1 Temel ilke: “Yetki kaynağı server, UI sadece yansıma”
 - UI buton/alan gizleyebilir ama **asıl karar** server-side alınır.
 - Bu nedenle “anında yansıma” iki katmandır:
   - UI’nin anında güncellenmesi (kullanıcı deneyimi)
   - API’nin anında enforce etmesi (güvenlik)

### 10.2 Önerilen profesyonel çözüm: Permission Stamp (Versiyon) + Cache Invalidation
Önerilen yaklaşım:
 - Her kullanıcı için `PermissionVersion`/`PermissionStamp` benzeri bir değer tut (int/Guid/DateTime ticks)
 - Kullanıcının yetkileri değiştiğinde bu değer güncellenir
 - `PermissionService` cache kullanıyorsa cache anahtarı şu şekilde kurgulanır:
   - `TcKimlikNo + PermissionStamp`
 - Böylece stamp değişince eski cache otomatik “boşa düşer” ve bir sonraki request’te güncel yetkiler yüklenir.

Performans hedefi (önemli):
 - UI/component render sürecinde DB’ye tekrar tekrar gitmeyin.
 - Yetkileri “kullanıcı bazlı snapshot” olarak bir defa yükleyin ve memory’den değerlendirin.

Önerilen cache modeli:
 - Cache value: `Dictionary<string /*PermissionKey*/, YetkiTipleri>`
 - Cache key: `perm:{TcKimlikNo}:{PermissionStamp}`
 - TTL: 5-30 dakika (proje ölçeğine göre) + stamp değişince otomatik invalidate

3-5K kullanıcı ölçeği için notlar (tek sunucu):
 - Cache’i “tüm kullanıcılar” için değil, **aktif oturumlar** için tutarsınız.
 - Cache **on-demand** oluşur (kullanıcı login olur / ilk permission check olur → yüklenir).
 - Cache entry’lerine `Size` verip `MemoryCache`’te global bir `SizeLimit` ile bellek sınırı koyabilirsiniz.
 - TTL + eviction ile uzun süre aktif olmayan kullanıcıların snapshot’ları bellekten düşer.
 - Aynı kullanıcı birden fazla sekme/circuit açsa bile cache anahtarı `TcKimlikNo` bazlı olduğu için snapshot paylaşılabilir.

Daha profesyonel optimizasyon (opsiyonel, ihtiyaç olursa):
 - Permission key string’lerini her snapshot’ta tekrar tekrar tutmak yerine:
   - “Catalog cache”: `PermissionKey -> YetkiId` map’ini global cache’te tut
   - User snapshot: `Dictionary<int /*YetkiId*/, YetkiTipleri>` veya `HashSet<int>` tut
 - Bu yaklaşım bellek kullanımını ciddi düşürür ve lookup’ı hızlandırır.

`PermissionStamp` nasıl güncel kalacak?
 - Yetki yönetimi işlemleri uygulama üzerinden yapılacaksa (önerilir):
   - Yetki kaydı update edilir
   - Stamp artırılır
   - Kullanıcının cache’i temizlenir (veya yeni stamp ile yeni key’e geçilir)
 - Yetkiler doğrudan SQL ile DB’den değiştirilirse:
   - Stamp de güncellenmelidir, aksi halde cache eski kalabilir
   - Alternatif: kısa TTL ile “en geç X dakikada” güncellenir (anlık olmaz)

“Daha önce olmayan yeni bir permission key” eklendiğinde anında çalışma şartı:
 - Yeni key’in DB’ye eklenmesi tek başına yetmez; ilgili kullanıcıya atama yapıldıysa **Stamp de artırılmalıdır**.
 - Stamp artınca snapshot yeniden yükleneceği için yeni key otomatik olarak sisteme dahil olur.

Pratik DB modeli (minimum):
 - `PER_Personel` veya ayrı bir tabloda: `TcKimlikNo`, `PermissionStamp`

### 10.3 UI’de anında yansıtma (opsiyonel ama önerilir)
Eğer gerçek zamanlı UI güncellemesi istiyorsanız:
 - Yetki değiştiğinde server, ilgili kullanıcıya SignalR ile `PermissionsChanged` event’i push eder
 - Client bu eventi alınca:
   - Permission cache’i temizler
   - Yetkileri yeniden çeker (örn. `/auth/me/permissions`)
   - Sayfayı/komponentleri yeniden render eder

Blazor Server için pratik not:
 - UI tarafında yetkiler `scoped` bir state service içinde tutuluyorsa, `PermissionsChanged` event’i geldiğinde bu state resetlenmeli ve yeniden yüklenmelidir.
 - Yetkiler düşürüldüyse, kullanıcı mevcut route’da kalmamalı (redirect/forbidden).

Yetki düşürme senaryosu için:
 - Kullanıcı bulunduğu sayfaya artık erişemiyorsa:
   - UI redirect (örn. `/forbidden`)
   - Gerekirse “force logout”

### 10.4 Token/JWT kullanılıyorsa dikkat
Eğer yetkiler token içine gömülüyse:
 - Token süresi bitene kadar “eski yetkiler” client’ta kalabilir
 - Bu yüzden:
   - Token’ı kısa ömürlü yapın veya
   - Token içinde `PermissionStamp` taşıyıp her request’te stamp’i server’da doğrulayın veya
   - Token’ı sadece kimlik için kullanıp yetkiyi her request’te server-side çözün (önerilir)

---

## 11) Dinamik ve Geleceğe Dönük Yetkilendirme Modeli (Öneri)

Hedef: İleride ihtiyaç oldukça **yeni sayfalar**, **yeni tab/alanlar**, hatta **domain içi aksiyonlar** (örn: Onayla, Yayınla, İptal) eklendiğinde sistemin bozulmaması.

### 11.1 Model: RBAC + Resource Hierarchy (gerekirse ABAC ile genişletilebilir)
Önerilen katmanlar:
 - **Resource (kaynak)**: Yetki verilen şey (sayfa, endpoint, tab, field, aksiyon)
 - **Action (aksiyon)**: View/Edit/Delete (veya custom)
 - **Assignment (atama)**: Personel → ResourceKey → YetkiTipi

Bu projede `YetkiAdi` zaten ResourceKey olarak kullanılabilir.

### 11.2 Hiyerarşi ve “inheritance” (üst yetki altı kapsayabilsin)
İki yaklaşım:
 - **DB hiyerarşisi ile**: `UstYetkiId` üzerinden parent-child
 - **String prefix ile**: `PER.PERSONEL.MANAGE` parent, `PER.PERSONEL.MANAGE.FIELD.EMAIL` child

Değerlendirme kuralı (öneri):
 - Önce en spesifik key aranır (field)
 - Yoksa parent’a doğru çıkılır (tab → sayfa → modül)
 - Bulunan ilk kaydın `YetkiTipleri` seviyesi uygulanır
 - Default: deny (`None`)

Bu sayede:
 - `PER.PERSONEL.MANAGE.EDIT = Edit` verilirse, tüm alanlar editlenebilir
 - Sadece bir alanı kısıtlamak istersen:
   - `PER.PERSONEL.MANAGE.FIELD.MAAS.VIEW = View` gibi daha spesifik override tanımlanır

### 11.3 “Key patlaması” riskini yönetme
Alan bazına indikçe key sayısı artar. Bunu yönetmek için:
 - Varsayılanı tab/page seviyesinde ver
 - Sadece istisna alanlarda field-level key oluştur
 - Admin ekranında (Yetki Tab) hiyerarşiyi tree olarak göster (parent → child)

### 11.4 ABAC (koşullu yetki) ihtiyacı doğarsa
Örnek ihtiyaçlar:
 - Kullanıcı sadece kendi departmanındaki personeli düzenleyebilsin
 - Sadece belirli hizmet binası kapsamındaki kayıtlara erişebilsin

Bu durumda permission key’e ek olarak:
 - “scope/condition” alanı (DepartmanId, HizmetBinasiId gibi)
 - veya policy engine (request context + user claims)

Bu dokümandaki yapı RBAC ile başlar; ABAC ihtiyacı geldiğinde aynı ResourceKey standardı bozulmadan genişletilebilir.

---

## 12) Kod Değiştirmeden Yeni Yetki Ekleyebilme (DB-Driven Permissions)

Hedef: Daha önce sistemde tanımı olmayan bir alan/aksiyon için (örn. `FormModel.TcKimlikNo`) sonradan **sadece DB’ye yeni permission key ekleyerek** yetkilendirme yapabilmek.

Bu hedefe ulaşmak için kritik önkoşul:
 - Uygulama; yetkileri “hardcoded if/else” ile değil, **string key + seviye** mantığıyla, merkezi bir `PermissionService` üzerinden değerlendirmelidir.

### 12.1 Konvansiyon (Convention) ile Key üretimi
Sayfa ve field için key’ler kodda tek tek yazılmak yerine, **konvansiyonla üretilebilir**.

Önerilen base key:
 - `{MODULE}.{RESOURCE}.{PAGE}.FIELD.{FIELD}`

Aksiyonlar:
 - `{BaseKey}.VIEW`
 - `{BaseKey}.EDIT`

Örnek (senaryon):
 - Sayfa: `/personel/manage/{tc}`
 - PAGE: `MANAGE`
 - FIELD: `TC_KIMLIK_NO`
 - Key’ler:
   - `PER.PERSONEL.MANAGE.FIELD.TC_KIMLIK_NO.VIEW`
   - `PER.PERSONEL.MANAGE.FIELD.TC_KIMLIK_NO.EDIT`

Alan adı normalize kuralı (öneri, tek standard seçilmeli):
 - `TcKimlikNo` → `TC_KIMLIK_NO` (PascalCase → UPPER_SNAKE)

### 12.2 “Key yoksa ne olacak?” (Default + Inheritance)
Konvansiyon tabanlı sistemlerde şu karar net olmalı:
 - Eğer **spesifik field key DB’de yoksa**, parent key’e fallback yapılır.

Örnek fallback sırası (öneri):
 - `PER.PERSONEL.MANAGE.FIELD.TC_KIMLIK_NO.EDIT`
 - `PER.PERSONEL.MANAGE.EDIT`
 - `PER.PERSONEL.EDIT`
 - Default: `None`

Bu sayede:
 - Bugün `TcKimlikNo` için spesifik kayıt yoksa, sayfanın genel edit/view yetkisi geçerli olur.
 - Yarın DB’ye `...FIELD.TC_KIMLIK_NO...` key’lerini eklersen, sistem otomatik olarak o daha spesifik kaydı bulur ve uygular.

### 12.3 UI tarafında kod değişmeden “sonradan” yetki ekleyebilmenin şartı
UI tarafında iki model vardır:
 - **Model A (tam dinamik)**: Form alanları bir metadata kaynağından üretilir (form generator). Bu modelde yeni field permission key’leri hiçbir UI kodu değişmeden uygulanabilir.
 - **Model B (klasik Razor/manuel form)**: Input’lar elle yazılmıştır. Bu modelde de “sonradan DB’den yetki ekleme” mümkündür ama bunun için sayfadaki alan render’larının bir defa **generic bir helper/tag-helper** üzerinden geçmesi gerekir.

Öneri (Model B için):
 - `PermissionFieldFor(asp-for="FormModel.TcKimlikNo")` gibi bir helper/tag-helper
 - Helper;
   - sayfadan `PAGE` bilgisini (route/controller/action) alır
   - `asp-for` ifadesinden field adını alır
   - key’i konvansiyonla üretir
   - `None/View/Edit` sonucuna göre input/text/hide davranışını uygular

Bu altyapı bir kez kurulduktan sonra:
 - Yarın yeni bir field için DB’ye key eklersen, UI o key’i otomatik keşfeder.

### 12.4 API tarafı (asıl güvenlik): “DB’den eklenen yeni field yetkisi” nasıl enforce edilir?
Server-side update’lerde (Manage POST/PUT/PATCH) önerilen yaklaşım:
 - Entity DB’den yüklenir
 - DTO’dan gelen alanlar tek tek değerlendirilir
 - Her alan için konvansiyonla key üretilir
 - `Edit` yoksa o alan ignore edilir ve DB değeri korunur

Bu sayede:
 - Kullanıcı payload’a sonradan `TcKimlikNo` ekleyip gönderse bile, `EDIT` yetkisi yoksa güncelleme gerçekleşmez.

### 12.5 Operasyonel akış (sadece DB ile yeni yetki ekleme)
1. `PER_Yetkiler` tablosuna yeni kayıt:
   - `YetkiAdi`: örn. `PER.PERSONEL.MANAGE.FIELD.TC_KIMLIK_NO.EDIT`
   - `YetkiTuru/UstYetkiId`: hiyerarşiye uygun
2. İlgili personele `PER_PersonelYetkileri` üzerinden yetki ata
3. `PermissionStamp` güncelle (bölüm 10) → anında yansıma

---

## 13) Blazor Server Notları (SPA Navigation + Hard Refresh + Circuit)

Bu proje Blazor Server’dır:
 - `AddServerSideBlazor(...)` kullanıyor
 - `MapBlazorHub(...)` aktif
 - `_Host.cshtml` içinde `~/_framework/blazor.server.js` yükleniyor

### 13.1 Single Page davranışı ne demek?
 - Blazor `Router` ile route değişimlerinde tam sayfa yenilemeden (hard refresh) component’ler arası geçiş yapar.
 - Bu nedenle “kullanıcının yetkileri değişti” gibi durumlar, sayfa yenilenmeden de UI’ye yansıtılmak istenebilir.

### 13.2 Circuit ve cache konusu
Blazor Server’da her tarayıcı sekmesi tipik olarak ayrı bir “circuit” oluşturur.

Önerilen kural:
 - “Kullanıcı yetkileri” gibi verileri scoped bir state service içinde tutuyorsanız, bu state circuit ömrü boyunca yaşayabilir.
 - Yetki değişikliği olduğunda bu state’i invalidate etmezseniz kullanıcı UI’de eski izinleri görmeye devam edebilir.

Bu nedenle bölüm 10’daki `PermissionStamp` yaklaşımı Blazor Server için özellikle önemlidir.

### 13.3 Hard refresh olunca ne olur?
 - Tarayıcı hard refresh yaptığında `_Host` tekrar render edilir.
 - Yeni circuit oluşur ve servisler yeniden resolve edilir.
 - Bu, yetkilerin “yeniden yüklenmesi” için doğal bir fırsattır.

Ancak hedef “refresh olmadan” da yansıtmaksa 10.3 (push) yaklaşımına ihtiyaç vardır.

### 13.4 Blazor Server’da “anında yansıma” için önerilen minimum
 - Server-side enforcement her zaman aktif kalır (API update/endpoint kontrolü)
 - UI deneyimi için:
   - `PermissionsChanged` bildirimi (SignalR) alındığında permission cache/state temizlenir
   - Gerekirse ilgili sayfa yeniden render edilir
   - Kullanıcının artık erişemediği route’larda forbidden/redirect uygulanır

Not:
 - Authentication cookie içindeki claim’lere permission set’i gömmek yerine (güncellenmesi zor), permission’ı DB/cached servis üzerinden çözmek ve `PermissionStamp` ile invalidate etmek daha esnektir.
