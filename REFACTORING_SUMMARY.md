# 🔄 Personel-User Refactoring Özeti

## 📋 Genel Bakış

Bu refactoring işlemi, **Personel** ve **User** tablolarını ayırarak, statik ve dinamik verilerin doğru yerde tutulmasını sağlamıştır.

### 🎯 Amaç
- **Personel Tablosu:** Kimlik, kadro, unvan, adres gibi **statik** (değişmeyen) bilgiler
- **User Tablosu:** Şifre, oturum, aktiflik, giriş başarısızlık sayısı gibi **dinamik** (sık değişen) bilgiler
- **One-to-One İlişki:** Her personelin bir User kaydı olacak (TcKimlikNo üzerinden)

---

## ✅ Yapılan Değişiklikler

### 1. **Entity Değişiklikleri**

#### 📄 User Entity (`User.cs`)
**Yeni Alanlar:**
- `KullaniciAdi` - Login için kullanıcı adı
- `Email` - Email adresi
- `TelefonNo` - Telefon numarası
- `PassWord` - Şifre (Personel'den taşındı)
- `SessionID` - Oturum ID (Personel'den taşındı)
- `AktifMi` - Kullanıcı aktif mi?
- `SonGirisTarihi` - Son giriş tarihi
- `BasarisizGirisSayisi` - Başarısız giriş denemesi sayısı
- `HesapKilitTarihi` - Hesap kilitlenme tarihi

**İlişkiler:**
```csharp
// Personel ile One-to-One
public Personel? Personel { get; set; }

// HubConnection ile One-to-One (SignalR)
public HubConnection? HubConnection { get; set; }
```

#### 📄 Personel Entity (`Personel.cs`)
**Kaldırılan Alanlar:**
- ❌ `PassWord` → User'a taşındı
- ❌ `SessionID` → User'a taşındı
- ❌ `HubConnection` → User ile ilişkilendirildi

**Eklenen İlişki:**
```csharp
// User ile One-to-One
public User? User { get; set; }
```

#### 📄 HubConnection Entity (`HubConnection.cs`)
**Değişiklik:**
```csharp
// Önceki: Personel ile ilişkili
// Şimdi: User ile ilişkili
[ForeignKey(nameof(TcKimlikNo))]
public User? User { get; set; }
```

---

### 2. **Configuration Değişiklikleri**

#### 📄 UserConfiguration (`UserConfiguration.cs`)
```csharp
// Personel ile One-to-One ilişki
builder.HasOne(u => u.Personel)
    .WithOne(p => p.User)
    .HasForeignKey<User>(u => u.TcKimlikNo)
    .OnDelete(DeleteBehavior.Cascade);

// HubConnection ile One-to-One ilişki
builder.HasOne(u => u.HubConnection)
    .WithOne(h => h.User)
    .HasForeignKey<HubConnection>(h => h.TcKimlikNo)
    .OnDelete(DeleteBehavior.Cascade);
```

**Önemli:** Cascade Delete aktif - Personel silindiğinde User da silinir.

---

### 3. **Service Değişiklikleri**

#### 📄 PersonelService (`PersonelService.cs`)
**CreateAsync Metodu Güncellendi:**
```csharp
// Personel oluşturulurken otomatik olarak User kaydı da oluşturulur
var user = new User
{
    TcKimlikNo = personel.TcKimlikNo,
    KullaniciAdi = personel.TcKimlikNo,
    Email = personel.Email,
    TelefonNo = personel.CepTelefonu,
    PassWord = personel.TcKimlikNo, // Varsayılan şifre
    AktifMi = personel.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif
};
await userRepo.AddAsync(user);
```

#### 📄 AuthService (`AuthService.cs`)
**LoginAsync Metodu Refactor Edildi:**
```csharp
// Artık User tablosundan login yapılıyor
var user = await _context.Users
    .Include(u => u.Personel)
        .ThenInclude(p => p.Departman)
    .Include(u => u.Personel)
        .ThenInclude(p => p.Servis)
    .Include(u => u.Personel)
        .ThenInclude(p => p.HizmetBinasi)
    .FirstOrDefaultAsync(u => u.TcKimlikNo == request.TcKimlikNo);
```

**Yeni Güvenlik Kontrolleri:**
- ✅ Hesap aktif mi kontrolü
- ✅ Hesap kilitli mi kontrolü
- ✅ 5 başarısız denemeden sonra otomatik kilitleme
- ✅ Başarılı girişte sayacı sıfırlama
- ✅ Son giriş tarihi güncelleme

---

### 4. **Yeni Servisler**

#### 📄 UserService (`UserService.cs`)
**CRUD İşlemleri:**
- `GetByTcKimlikNoAsync()`
- `GetByKullaniciAdiAsync()`
- `GetAllAsync()`
- `GetActiveUsersAsync()`
- `GetLockedUsersAsync()`
- `CreateAsync()`
- `UpdateAsync()`
- `DeleteAsync()`

**Şifre İşlemleri:**
- `ChangePasswordAsync()` - Şifre değiştirme
- `ResetPasswordAsync()` - Şifre sıfırlama (TC Kimlik No'ya)

**Hesap Yönetimi:**
- `LockUserAsync()` - Kullanıcıyı kilitle
- `UnlockUserAsync()` - Kullanıcı kilidini aç
- `ActivateUserAsync()` - Kullanıcıyı aktif et
- `DeactivateUserAsync()` - Kullanıcıyı pasif et

**Oturum Yönetimi:**
- `ClearSessionAsync()` - Oturumu temizle
- `GetBySessionIdAsync()` - Session ID ile kullanıcı getir

---

### 5. **DTOs**

#### 📄 UserResponseDto
```csharp
public class UserResponseDto
{
    public string TcKimlikNo { get; set; }
    public string KullaniciAdi { get; set; }
    public string Email { get; set; }
    public string? TelefonNo { get; set; }
    public bool AktifMi { get; set; }
    public DateTime? SonGirisTarihi { get; set; }
    public int BasarisizGirisSayisi { get; set; }
    public DateTime? HesapKilitTarihi { get; set; }
    
    // İlişkili Personel Bilgileri
    public string? PersonelAdSoyad { get; set; }
    public int? SicilNo { get; set; }
    public string? DepartmanAdi { get; set; }
    public string? ServisAdi { get; set; }
}
```

#### 📄 UserCreateRequestDto
```csharp
public class UserCreateRequestDto
{
    [Required]
    [StringLength(11, MinimumLength = 11)]
    public string TcKimlikNo { get; set; }
    
    [Required]
    [StringLength(50)]
    public string KullaniciAdi { get; set; }
    
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; }
    
    [StringLength(20)]
    public string? TelefonNo { get; set; }
    
    [Required]
    [StringLength(255, MinimumLength = 6)]
    public string PassWord { get; set; }
    
    public bool AktifMi { get; set; } = true;
}
```

#### 📄 UserUpdateRequestDto
```csharp
public class UserUpdateRequestDto
{
    [Required]
    [StringLength(50)]
    public string KullaniciAdi { get; set; }
    
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; }
    
    [StringLength(20)]
    public string? TelefonNo { get; set; }
    
    public bool AktifMi { get; set; }
}
```

---

## 🗄️ Database Migration

### Migration Oluşturma
```powershell
dotnet ef migrations add RefactorUserPersonelRelationship --project SGKPortalApp.DataAccessLayer --startup-project SGKPortalApp.ApiLayer
```

### Data Migration Script
**Lokasyon:** `DataMigrationScripts/MigratePersonelToUser.sql`

Bu script:
1. Mevcut tüm Personel kayıtları için User oluşturur
2. PassWord ve SessionID verilerini kopyalar
3. PersonelAktiflikDurum'a göre AktifMi alanını set eder
4. Kontrol sorguları çalıştırır

### Migration Uygulama
```powershell
dotnet ef database update --project SGKPortalApp.DataAccessLayer --startup-project SGKPortalApp.ApiLayer
```

---

## 🔗 İlişki Diyagramı

```
┌─────────────────┐
│    Personel     │ (Statik Veriler)
│─────────────────│
│ TcKimlikNo (PK) │
│ SicilNo         │
│ AdSoyad         │
│ DepartmanId     │
│ ServisId        │
│ Email           │
│ CepTelefonu     │
│ ...             │
└────────┬────────┘
         │ 1:1
         │
┌────────▼────────┐
│      User       │ (Dinamik Veriler)
│─────────────────│
│ TcKimlikNo (PK) │◄─── Foreign Key
│ KullaniciAdi    │
│ Email           │
│ PassWord        │
│ SessionID       │
│ AktifMi         │
│ SonGirisTarihi  │
│ BasarisizGiris  │
│ HesapKilitTarihi│
└────────┬────────┘
         │ 1:1
         │
┌────────▼────────┐
│  HubConnection  │ (SignalR)
│─────────────────│
│ HubConnectionId │
│ TcKimlikNo (FK) │
│ ConnectionId    │
│ ConnectionStatus│
└─────────────────┘
```

---

## 📊 Veri Akışı

### Yeni Personel Ekleme
```
1. PersonelService.CreateAsync() çağrılır
2. Personel entity oluşturulur
3. User entity otomatik oluşturulur
   - KullaniciAdi = TcKimlikNo
   - PassWord = TcKimlikNo (varsayılan)
   - Email = Personel.Email
4. Her ikisi de veritabanına kaydedilir
```

### Login İşlemi
```
1. AuthService.LoginAsync() çağrılır
2. User tablosundan TcKimlikNo ile arama yapılır
3. Personel bilgileri Include ile yüklenir
4. Güvenlik kontrolleri:
   - Hesap aktif mi?
   - Hesap kilitli mi?
   - Şifre doğru mu?
5. Başarısız denemeler sayılır (5'te kilitleme)
6. Başarılı girişte:
   - SessionID oluşturulur
   - SonGirisTarihi güncellenir
   - BasarisizGirisSayisi sıfırlanır
```

### SignalR Bağlantısı
```
1. Kullanıcı bağlandığında HubConnection oluşturulur
2. User.TcKimlikNo ile ilişkilendirilir
3. ConnectionStatus güncellenir
4. Kullanıcı çıkış yaptığında bağlantı güncellenir
```

---

## ⚠️ Önemli Notlar

### Cascade Delete
- **Personel silindiğinde → User otomatik silinir**
- **User silindiğinde → HubConnection otomatik silinir**

### Varsayılan Değerler
- **Kullanıcı Adı:** TC Kimlik No
- **Şifre:** TC Kimlik No
- **AktifMi:** Personel.PersonelAktiflikDurum'a göre

### Güvenlik
- 5 başarısız giriş denemesinde hesap otomatik kilitlenir
- Kilitli hesaplar yönetici tarafından açılmalıdır
- Şifre sıfırlama TC Kimlik No'ya döner

### Performans
- User-Personel ilişkisi One-to-One olduğu için performans kaybı minimal
- Include kullanımı ile N+1 problemi önlenir
- Index'ler doğru tanımlandı (TcKimlikNo, Email, KullaniciAdi, SessionID)

---

## 🧪 Test Senaryoları

### 1. Yeni Personel Ekleme
```csharp
// Personel oluştur
var personelRequest = new PersonelCreateRequestDto
{
    TcKimlikNo = "12345678901",
    AdSoyad = "Test Personel",
    Email = "test@example.com",
    // ...
};

var result = await personelService.CreateAsync(personelRequest);

// User otomatik oluşturuldu mu kontrol et
var user = await userService.GetByTcKimlikNoAsync("12345678901");
Assert.NotNull(user);
Assert.Equal("12345678901", user.KullaniciAdi);
```

### 2. Login İşlemi
```csharp
// İlk giriş
var loginRequest = new LoginRequestDto
{
    TcKimlikNo = "12345678901",
    Password = "12345678901" // Varsayılan şifre
};

var result = await authService.LoginAsync(loginRequest);
Assert.True(result.Success);
Assert.NotNull(result.SessionId);
```

### 3. Başarısız Giriş Denemesi
```csharp
// 5 kez hatalı şifre
for (int i = 0; i < 5; i++)
{
    var result = await authService.LoginAsync(new LoginRequestDto
    {
        TcKimlikNo = "12345678901",
        Password = "wrongpassword"
    });
}

// Hesap kilitlendi mi?
var user = await userService.GetByTcKimlikNoAsync("12345678901");
Assert.False(user.AktifMi);
Assert.NotNull(user.HesapKilitTarihi);
```

### 4. Şifre Değiştirme
```csharp
var result = await userService.ChangePasswordAsync(
    "12345678901",
    "12345678901", // Eski şifre
    "NewPassword123" // Yeni şifre
);

Assert.True(result.Success);
```

---

## 📚 Sonraki Adımlar

### Yapılacaklar
1. ✅ Migration oluştur ve uygula
2. ✅ Data migration script'i çalıştır
3. ⏳ Unit testler yaz
4. ⏳ Integration testler yaz
5. ⏳ API endpoint'leri test et
6. ⏳ Frontend'i güncelle (User yönetim sayfası)
7. ⏳ Şifre hashleme ekle (BCrypt/PBKDF2)
8. ⏳ JWT token implementasyonu
9. ⏳ Rol bazlı yetkilendirme

### İyileştirmeler
- [ ] Şifre karmaşıklık kuralları
- [ ] Email doğrulama
- [ ] 2FA (Two-Factor Authentication)
- [ ] Şifre geçmişi tutma
- [ ] Oturum timeout ayarları
- [ ] IP bazlı kilitleme
- [ ] Audit log (kim ne zaman giriş yaptı)

---

## 👥 Katkıda Bulunanlar

- **Geliştirici:** Cascade AI Assistant
- **Tarih:** 3 Kasım 2025
- **Versiyon:** 1.0.0

---

## 📞 Destek

Sorularınız için:
- Email: muhammedbodur@gmail.com
- Proje: SGK Portal App

---

**🎉 Refactoring Tamamlandı!**
