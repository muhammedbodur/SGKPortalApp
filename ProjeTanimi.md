# Proje Ayarları ve Klasör Yapısı

## 🎯 Ana Proje Bilgileri

### SGPPortalApp (Ana Proje)
- **Durum**: Aktif geliştirme
- **Amaç**: Yeni SGK Portal uygulaması
- **Ana Klasör**: `[Buraya SGPPortalApp'in tam yolunu yazın]`
- **Teknoloji**: .NET Core, ASP.NET MVC
- **Not**: Bu proje üzerinde aktif olarak çalışılacak

### SocialSecurityInstitution (Referans Proje)
- **Durum**: Referans/Şablon
- **Amaç**: Mimari ve yapı referansı
- **Ana Klasör**: `[Buraya SocialSecurityInstitution'ın tam yolunu yazın]`
- **Kullanım**: Mimari yapı, katman organizasyonu, design pattern'ler için referans
- **Not**: Kod kopyalanmayacak, sadece yapı/mimari referans alınacak

## 📁 Varsayılan Çalışma Dizini

**Aksi belirtilmedikçe tüm işlemler SGPPortalApp projesi üzerinde yapılacaktır.**

## 🔄 Proje Arası İlişki

```
SocialSecurityInstitution (Eski)
    ↓ (Mimari Referans)
SGPPortalApp (Yeni)
```

### Referans Alınacak Yapılar:
- 4-Katmanlı mimari (Presentation, Business, Data Access, Business Object)
- Repository Pattern implementasyonu
- Service katmanı organizasyonu
- DTO ve Entity yapıları
- AutoMapper kullanımı
- SignalR implementasyonu
- Middleware yapıları

## 📝 Kullanım Notları

### Komut Örnekleri:
- "Kanal controller'ı oluştur" → SGPPortalApp'te oluşturulur
- "SocialSecurityInstitution'daki kanal yapısını göster" → Referans proje incelenir
- "Eski projedeki repository pattern'i referans al" → SocialSecurityInstitution'dan referans alınır

### Önemli:
Eğer **SocialSecurityInstitution** projesinde işlem yapmak isterseniz, açıkça belirtmeniz gerekir:
- ❌ "Repository pattern'i göster" (SGPPortalApp'te aranır)
- ✅ "SocialSecurityInstitution'daki repository pattern'i göster" (Doğru proje)

---

**Son Güncelleme**: 02 Ekim 2025
**Varsayılan Proje**: SGPPortalApp
**Referans Proje**: SocialSecurityInstitution