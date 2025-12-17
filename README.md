# SGK Portal App

## URL ve Permission Key Convention

Bu dokümantasyon, projede kullanılan URL ve Permission Key standartlarını tanımlar. **Tüm geliştiriciler bu kurallara uymak zorundadır.**

---

## 1. URL Convention

### Format
```
/{klasör1}-{klasör2}-{...}/{action?}/{id?}
```

### Kurallar
- Klasörler arası **tire (-)** ile birleştirilir
- Action (manage, create, detail) ayrı segment olarak kalır
- ID parametresi en sonda yer alır
- Tüm URL'ler **küçük harf** olmalıdır

### Örnekler
| Klasör Yolu | URL |
|-------------|-----|
| `Pages/Personel/Index.razor` | `/personel` |
| `Pages/Personel/Manage.razor` | `/personel/manage/{tc}` |
| `Pages/Personel/Departman/Index.razor` | `/personel-departman` |
| `Pages/Personel/Departman/Manage.razor` | `/personel-departman/manage/{id}` |
| `Pages/Yetki/Modul/Index.razor` | `/yetki-modul` |
| `Pages/Yetki/Controller/Index.razor` | `/yetki-controller` |
| `Pages/Yetki/Islem/Index.razor` | `/yetki-islem` |
| `Pages/Yetki/Atama/Index.razor` | `/yetki-atama` |
| `Pages/Siramatik/Banko/Index.razor` | `/siramatik-banko` |
| `Pages/Siramatik/Banko/Manage.razor` | `/siramatik-banko/manage/{id}` |
| `Pages/Siramatik/Kiosk/Menu/Index.razor` | `/siramatik-kiosk-menu` |

---

## 2. Permission Key Convention

### Format
```
{MODUL_KODU}.{URL_TIRESIZ}.{ACTION}
```

### Kurallar
- **Modül Kodu:** DB'de tanımlı 3 harfli kod (PER, YET, SIR, COM)
- **URL_TIRESIZ:** URL'deki tüm segmentler (action hariç) birleştirilir, tireler kaldırılır, büyük harfe çevrilir
- **Action:** Razor dosya adı (INDEX, MANAGE, CREATE, DETAIL)

### Örnekler
| URL | Permission Key |
|-----|----------------|
| `/personel` | `PER.PERSONEL.INDEX` |
| `/personel/manage/{tc}` | `PER.PERSONEL.MANAGE` |
| `/personel-departman` | `PER.PERSONELDEPARTMAN.INDEX` |
| `/personel-departman/manage/{id}` | `PER.PERSONELDEPARTMAN.MANAGE` |
| `/yetki-modul` | `YET.YETKIMODUL.INDEX` |
| `/yetki-controller` | `YET.YETKICONTROLLER.INDEX` |
| `/yetki-islem` | `YET.YETKIISLEM.INDEX` |
| `/yetki-atama` | `YET.YETKIATAMA.INDEX` |
| `/siramatik-banko` | `SIR.SIRAMATIKBANKO.INDEX` |
| `/siramatik-banko/manage/{id}` | `SIR.SIRAMATIKBANKO.MANAGE` |
| `/siramatik-kiosk-menu` | `SIR.SIRAMATIKKIOSKMENU.INDEX` |

---

## 3. Modül Kodları

| Klasör | Modül Kodu | Açıklama |
|--------|------------|----------|
| Personel | PER | Personel işlemleri |
| Yetki | YET | Yetki yönetimi |
| Siramatik | SIR | Sıramatik sistemi |
| Common | COM | Ortak modüller |
| Pdks | PDKS | Personel devam kontrol |
| Eshot | ESHOT | ESHOT entegrasyonu |

---

## 4. Alt Seviye Permission'lar

### FormField (Form Alanları)
```
{MODUL_KODU}.{CONTROLLER}.{ACTION}.FORMFIELD.{ALAN_ADI}
```
Örnek: `PER.PERSONEL.MANAGE.FORMFIELD.EMAIL`

### Buton
```
{MODUL_KODU}.{CONTROLLER}.{ACTION}.BUTON.{BUTON_ADI}
```
Örnek: `PER.PERSONEL.INDEX.BUTON.EXPORT`

### Tab
```
{MODUL_KODU}.{CONTROLLER}.{ACTION}.TAB.{TAB_ADI}
```
Örnek: `PER.PERSONEL.MANAGE.TAB.YETKI`

---

## 5. Sayfa Oluşturma Checklist

Yeni bir sayfa oluştururken:

1. [ ] URL convention'a uygun `@page` direktifi ekle
2. [ ] `@inherits SGKPortalApp.PresentationLayer.Components.Base.FieldPermissionPageBase` ekle
3. [ ] Code-behind'da `PagePermissionKey` property'sini override et
4. [ ] Razor'da `@if (!CanViewPage)` kontrolü ekle
5. [ ] DB'de `PER_ModulControllerIslemleri` tablosuna kayıt ekle

### Örnek Razor
```razor
@page "/yetki-atama"
@inherits SGKPortalApp.PresentationLayer.Components.Base.FieldPermissionPageBase

<div class="container-xxl flex-grow-1 container-p-y">
    @if (!CanViewPage)
    {
        <div class="alert alert-danger">
            Bu sayfayı görüntüleme yetkiniz bulunmuyor.
        </div>
    }
    else
    {
        <!-- Sayfa içeriği -->
    }
</div>
```

### Örnek Code-Behind
```csharp
public partial class Index
{
    /// <summary>
    /// Permission Key: YET.ATAMA.INDEX
    /// Route: /yetki-atama
    /// Convention: {MODUL_KODU}.{CONTROLLER}.{ACTION}
    /// </summary>
    protected override string PagePermissionKey => "YET.ATAMA.INDEX";
}
```

---

## 6. Önemli Notlar

⚠️ **DİKKAT:** Bu convention'a uymayan sayfalar code review'da reddedilecektir.

- URL'de `/` yerine `-` kullanın (klasörler arası)
- Permission key'de tire kullanmayın, birleştirin (KIOSKMENU, YETKIATAMA değil KIOSK-MENU)
- Modül kodu DB'de `PER_Moduller` tablosunda tanımlı olmalıdır
- Her sayfa için `PER_ModulControllerIslemleri` tablosuna kayıt eklenmeli

---

*Son güncelleme: 2025-12-17*
