# 🚀 Migration Uygulama Komutu

## ✅ Hazırlık Tamamlandı!

Migration dosyaları oluşturuldu:
- ✅ `mig_9.cs` - Migration kodu
- ✅ `mig_9.Designer.cs` - Designer dosyası

---

## 📝 Migration'ı Uygulamak İçin:

### Komut:

```powershell
dotnet ef database update --project SGKPortalApp.DataAccessLayer --startup-project SGKPortalApp.ApiLayer
```

VEYA kısa hali:

```powershell
dotnet ef database update
```

---

## 🔍 Migration Ne Yapacak?

1. ✅ **CMN_Users** tablosunu oluşturacak
2. ✅ **Mevcut tüm personeller** için User kayıtları oluşturacak
3. ✅ PassWord ve SessionID verilerini kopyalayacak
4. ✅ Personel tablosundan **PassWord** ve **SessionID** kolonlarını kaldıracak
5. ✅ Index'leri oluşturacak
6. ✅ HubConnection ilişkisini User'a yönlendirecek
7. ✅ Kontrol sorguları çalıştıracak

---

## 📊 Beklenen Çıktı:

```
Applying migration 'mig_9'.
🚀 User kayıtları oluşturuluyor...
✅ Oluşturulan User kayıt sayısı: 150

📊 KONTROL SONUÇLARI:
─────────────────────────────────────────
Toplam Personel: 150
Toplam User    : 150
Fark           : 0
✅ Tüm personellerin User kaydı mevcut!

🎉 Migration tamamlandı!
Done.
```

---

## ⚠️ Sorun Yaşarsanız:

### Hata: "Migration already applied"

Migration zaten uygulanmış. Kontrol edin:

```powershell
dotnet ef migrations list
```

### Hata: "Build failed"

Projeyi derleyin:

```powershell
dotnet build
```

### Hata: "Connection string not found"

`appsettings.Shared.json` dosyasında connection string'i kontrol edin.

---

## 🔄 Geri Almak İsterseniz:

```powershell
# Önceki migration'ın adını bulun
dotnet ef migrations list

# Önceki migration'a geri dön
dotnet ef database update <PreviousMigrationName>
```

---

## ✅ Migration Sonrası Kontrol:

```sql
-- User tablosu oluşturuldu mu?
SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CMN_Users';

-- Kaç User kaydı var?
SELECT COUNT(*) FROM [dbo].[CMN_Users];

-- Kaç Personel var?
SELECT COUNT(*) FROM [dbo].[PER_Personeller];

-- User kaydı olmayan personel var mı? (boş olmalı)
SELECT p.TcKimlikNo, p.AdSoyad
FROM [dbo].[PER_Personeller] p
LEFT JOIN [dbo].[CMN_Users] u ON p.TcKimlikNo = u.TcKimlikNo
WHERE u.TcKimlikNo IS NULL;

-- Personel tablosunda PassWord kolonu var mı? (olmamalı)
SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'PER_Personeller' AND COLUMN_NAME = 'PassWord';
```

---

**🎉 Hazırsınız! Komutu çalıştırabilirsiniz.**
