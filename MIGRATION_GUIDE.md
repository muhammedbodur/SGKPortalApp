# 🚀 Migration Uygulama Rehberi

## 📋 Genel Bakış

Bu rehber, Personel-User refactoring migration'ını güvenli bir şekilde uygulamanız için adım adım talimatlar içerir.

---

## ⚠️ Önce Yedek Alın!

```sql
-- Veritabanı yedeği alın
BACKUP DATABASE [SGKPortalDB] 
TO DISK = 'C:\Backups\SGKPortalDB_BeforeUserMigration.bak'
WITH FORMAT, INIT, NAME = 'Before User Migration';
```

---

## 🔧 Yöntem 1: Otomatik Migration (ÖNERİLEN)

### Adım 1: Migration Oluştur

```powershell
cd d:\AspNetExamples\SGKPortalApp

dotnet ef migrations add RefactorUserPersonelRelationship `
    --project SGKPortalApp.DataAccessLayer `
    --startup-project SGKPortalApp.ApiLayer
```

### Adım 2: Migration Dosyasını Düzenle

Migration dosyası oluşturulduktan sonra (örn: `20251103_RefactorUserPersonelRelationship.cs`):

1. Dosyayı açın
2. `Up()` metoduna data migration SQL'ini ekleyin:

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // ... (EF Core tarafından oluşturulan kod)
    
    // 🆕 MEVCUT PERSONELLER İÇİN USER OLUŞTUR
    migrationBuilder.Sql(@"
        INSERT INTO [dbo].[CMN_Users] 
        (
            TcKimlikNo, PassWord, SessionID, AktifMi, 
            SonGirisTarihi, BasarisizGirisSayisi, HesapKilitTarihi,
            EklenmeTarihi, DuzenlenmeTarihi, SilindiMi,
            EkleyenKullanici, DuzenleyenKullanici
        )
        SELECT 
            p.TcKimlikNo,
            ISNULL(p.PassWord, p.TcKimlikNo) AS PassWord,
            p.SessionID,
            CASE WHEN p.PersonelAktiflikDurum = 1 THEN 1 ELSE 0 END AS AktifMi,
            NULL AS SonGirisTarihi,
            0 AS BasarisizGirisSayisi,
            NULL AS HesapKilitTarihi,
            p.EklenmeTarihi,
            p.DuzenlenmeTarihi,
            p.SilindiMi,
            p.EkleyenKullanici,
            p.DuzenleyenKullanici
        FROM [dbo].[PER_Personeller] p
        WHERE NOT EXISTS (
            SELECT 1 FROM [dbo].[CMN_Users] u WHERE u.TcKimlikNo = p.TcKimlikNo
        );
        
        PRINT 'User kayıtları oluşturuldu: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
    ");
    
    // ... (Personel tablosundan PassWord ve SessionID kolonlarını kaldırma kodu)
}
```

### Adım 3: Migration'ı Uygula

```powershell
dotnet ef database update `
    --project SGKPortalApp.DataAccessLayer `
    --startup-project SGKPortalApp.ApiLayer
```

### Adım 4: Kontrol Et

```sql
-- Kontrol sorguları
SELECT COUNT(*) AS ToplamPersonel FROM [dbo].[PER_Personeller];
SELECT COUNT(*) AS ToplamUser FROM [dbo].[CMN_Users];

-- User kaydı olmayan personeller (boş olmalı)
SELECT p.TcKimlikNo, p.AdSoyad
FROM [dbo].[PER_Personeller] p
LEFT JOIN [dbo].[CMN_Users] u ON p.TcKimlikNo = u.TcKimlikNo
WHERE u.TcKimlikNo IS NULL;
```

---

## 🔧 Yöntem 2: Manuel SQL Script

Eğer migration otomatik data migration yapmadıysa:

### Adım 1: Migration Uygula (Data Migration Olmadan)

```powershell
dotnet ef database update `
    --project SGKPortalApp.DataAccessLayer `
    --startup-project SGKPortalApp.ApiLayer
```

### Adım 2: Manuel SQL Script Çalıştır

```powershell
# SQL Server Management Studio'da veya sqlcmd ile:
sqlcmd -S localhost -d SGKPortalDB -i "DataMigrationScripts\MigratePersonelToUser.sql"
```

VEYA SSMS'de:
1. `DataMigrationScripts\MigratePersonelToUser.sql` dosyasını açın
2. F5 ile çalıştırın
3. Çıktıyı kontrol edin

---

## 🔄 Rollback (Geri Alma)

Eğer bir sorun olursa:

### Adım 1: Verileri Geri Kopyala

```powershell
sqlcmd -S localhost -d SGKPortalDB -i "DataMigrationScripts\RollbackUserMigration.sql"
```

### Adım 2: Migration'ı Geri Al

```powershell
# Önceki migration'ın adını bulun
dotnet ef migrations list `
    --project SGKPortalApp.DataAccessLayer `
    --startup-project SGKPortalApp.ApiLayer

# Önceki migration'a geri dön
dotnet ef database update <PreviousMigrationName> `
    --project SGKPortalApp.DataAccessLayer `
    --startup-project SGKPortalApp.ApiLayer
```

### Adım 3: Migration Dosyasını Sil

```powershell
dotnet ef migrations remove `
    --project SGKPortalApp.DataAccessLayer `
    --startup-project SGKPortalApp.ApiLayer
```

---

## ✅ Kontrol Listesi

Migration sonrası kontrol edin:

- [ ] Tüm personellerin User kaydı oluşturuldu mu?
- [ ] PassWord değerleri kopyalandı mı?
- [ ] SessionID değerleri kopyalandı mı?
- [ ] AktifMi değerleri doğru mu?
- [ ] Personel tablosundan PassWord ve SessionID kolonları kaldırıldı mı?
- [ ] Foreign Key ilişkileri doğru mu?
- [ ] HubConnection ilişkisi User'a yönlendirildi mi?
- [ ] Index'ler oluşturuldu mu?

---

## 🧪 Test Senaryoları

### Test 1: Login İşlemi

```csharp
// Mevcut bir personel ile login deneyin
var loginRequest = new LoginRequestDto
{
    TcKimlikNo = "12345678901",
    Password = "12345678901" // veya mevcut şifre
};

var result = await authService.LoginAsync(loginRequest);
// Başarılı olmalı
```

### Test 2: Yeni Personel Ekleme

```csharp
// Yeni personel ekleyin
var personelRequest = new PersonelCreateRequestDto
{
    TcKimlikNo = "98765432109",
    AdSoyad = "Test Personel",
    Email = "test@example.com",
    // ...
};

var result = await personelService.CreateAsync(personelRequest);
// User otomatik oluşturulmalı
```

### Test 3: User Kontrolü

```sql
-- Yeni eklenen personelin User kaydı var mı?
SELECT * FROM [dbo].[CMN_Users] WHERE TcKimlikNo = '98765432109';
-- 1 kayıt dönmeli
```

---

## 📊 Beklenen Sonuçlar

### Başarılı Migration Çıktısı

```
✅ CMN_Users tablosu mevcut. Data migration başlıyor...

Oluşturulan User kayıt sayısı: 150

📊 KONTROL SONUÇLARI:
─────────────────────────────────────────
Toplam Personel: 150
Toplam User    : 150
Fark           : 0

Aktif Personel : 145
Aktif User     : 145

✅ Tüm personellerin User kaydı mevcut!

🎉 Data Migration tamamlandı!
```

### Hatalı Durum

Eğer User kaydı olmayan personeller varsa:

```
⚠️  UYARI: User kaydı olmayan personeller bulundu!

TcKimlikNo   AdSoyad          Email
-----------  ---------------  ------------------
12345678901  Ahmet Yılmaz     ahmet@example.com
```

Bu durumda:
1. Script'i tekrar çalıştırın
2. Veya manuel olarak User oluşturun

---

## 🆘 Sorun Giderme

### Sorun 1: "PassWord kolonu bulunamadı"

**Çözüm:** Migration henüz uygulanmamış. Önce migration'ı uygulayın.

### Sorun 2: "Foreign Key constraint hatası"

**Çözüm:** Önce User kayıtlarını oluşturun, sonra Personel'den kolonları kaldırın.

### Sorun 3: "Duplicate key hatası"

**Çözüm:** User kaydı zaten var. Script'teki `WHERE NOT EXISTS` kontrolü çalışıyor mu kontrol edin.

### Sorun 4: "NULL PassWord"

**Çözüm:** Script'te `ISNULL(p.PassWord, p.TcKimlikNo)` kullanılıyor, sorun olmamalı.

---

## 📞 Destek

Sorun yaşarsanız:
1. Migration çıktısını kaydedin
2. Hata mesajlarını not edin
3. Veritabanı yedeğini kontrol edin
4. Gerekirse rollback yapın

---

**🎉 Başarılar!**
