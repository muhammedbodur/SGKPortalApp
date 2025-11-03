# 🔄 Personel-User Refactoring Özeti (FINAL)

## 📋 Genel Bakış

Bu refactoring işlemi, **Personel** ve **User** tablolarını ayırarak, **statik** ve **dinamik** verilerin doğru yerde tutulmasını sağlamıştır.

### 🎯 Amaç
- **Personel Tablosu:** Kimlik, kadro, unvan, adres, **email**, **telefon** gibi **statik** (değişmeyen/nadir değişen) bilgiler
- **User Tablosu:** Şifre, oturum, aktiflik, giriş başarısızlık sayısı gibi **dinamik** (sık değişen) bilgiler
- **One-to-One İlişki:** Her personelin bir User kaydı olacak (TcKimlikNo üzerinden)

### ⚡ Önemli Değişiklik
**Email, CepTelefonu, NickName gibi alanlar Personel tablosunda kalıyor!**
- Bu alanlar nadir değişir (statik)
- User tablosunda SADECE sık değişen veriler (şifre, oturum, kilitleme, vb.)

---

## ✅ Yapılan Değişiklikler

### 1. **Entity Değişiklikleri**

#### 📄 User Entity (`User.cs`)
**SADECE Dinamik Alanlar:**
- `PassWord` - Şifre (Personel'den taşındı)
- `SessionID` - Oturum ID (Personel'den taşındı)
- `AktifMi` - Kullanıcı aktif mi?
- `SonGirisTarihi` - Son giriş tarihi
- `BasarisizGirisSayisi` - Başarısız giriş denemesi sayısı
- `HesapKilitTarihi` - Hesap kilitlenme tarihi

**İlişkiler:**
```csharp
// Personel ile One-to-One
[ForeignKey(nameof(TcKimlikNo))]
public Personel? Personel { get; set; }

// HubConnection ile One-to-One (SignalR)
public HubConnection? HubConnection { get; set; }
```

**❌ User'da OLMAYAN Alanlar (Personel'de kalıyor):**
- Email → Personel.Email
- CepTelefonu → Personel.CepTelefonu
- NickName → Personel.NickName

#### 📄 Personel Entity (`Personel.cs`)
**Kaldırılan Alanlar:**
- ❌ `PassWord` → User'a taşındı
- ❌ `SessionID` → User'a taşındı

**Kalan Statik Alanlar:**
- ✅ `Email` - Email adresi (statik)
- ✅ `CepTelefonu` - Telefon numarası (statik)
- ✅ `NickName` - Kullanıcı adı (statik)
- ✅ `PersonelAktiflikDurum` - Kadro durumu (statik)

**Eklenen İlişki:**
```csharp
// User ile One-to-One
[InverseProperty("Personel")]
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
// SADECE dinamik alanlar için property tanımları
builder.Property(u => u.PassWord)
    .IsRequired()
    .HasMaxLength(255);

builder.Property(u => u.SessionID)
    .HasMaxLength(100);

builder.Property(u => u.AktifMi)
    .IsRequired()
    .HasDefaultValue(true);

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

// Index: Sadece TcKimlikNo ve SessionID
builder.HasIndex(u => u.TcKimlikNo).IsUnique();
builder.HasIndex(u => u.SessionID);
```

**❌ Kaldırılan Index'ler:**
- Email index (Personel'de kalıyor)
- KullaniciAdi index (Personel'de NickName olarak kalıyor)

---

### 3. **Service Değişiklikleri**

#### 📄 PersonelService (`PersonelService.cs`)
**CreateAsync Metodu:**
```csharp
// Personel oluşturulurken otomatik olarak User kaydı da oluşturulur
var user = new User
{
    TcKimlikNo = personel.TcKimlikNo,
    PassWord = personel.TcKimlikNo, // Varsayılan şifre
    AktifMi = personel.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif,
    BasarisizGirisSayisi = 0
};
await userRepo.AddAsync(user);
```

**NOT:** Email, CepTelefonu zaten Personel'de, User'a kopyalanmıyor!

#### 📄 AuthService (`AuthService.cs`)
**LoginAsync Metodu:**
```csharp
// User tablosundan login yapılıyor
var user = await _context.Users
    .Include(u => u.Personel)
        .ThenInclude(p => p.Departman)
    .Include(u => u.Personel)
        .ThenInclude(p => p.Servis)
    .Include(u => u.Personel)
        .ThenInclude(p => p.HizmetBinasi)
    .FirstOrDefaultAsync(u => u.TcKimlikNo == request.TcKimlikNo);

// Email Personel'den alınıyor
Email = user.Personel.Email
```

**Güvenlik Kontrolleri:**
- ✅ Hesap aktif mi kontrolü
- ✅ Hesap kilitli mi kontrolü
- ✅ 5 başarısız denemeden sonra otomatik kilitleme
- ✅ Başarılı girişte sayacı sıfırlama
- ✅ Son giriş tarihi güncelleme

---

### 4. **Repository Değişiklikleri**

#### 📄 IUserRepository
**Kaldırılan Metodlar:**
- ❌ `GetByKullaniciAdiAsync()` - Artık Personel.NickName kullanılacak
- ❌ `GetByEmailAsync()` - Artık Personel.Email kullanılacak
- ❌ `GetDropdownAsync()` - Personel'den alınacak

**Kalan Metodlar:**
- ✅ `GetByTcKimlikNoAsync()`
- ✅ `GetActiveUsersAsync()`
- ✅ `GetLockedUsersAsync()`
- ✅ `UpdateLastLoginAsync()`
- ✅ `IncrementFailedLoginAsync()`
- ✅ `ResetFailedLoginAsync()`
- ✅ `LockUserAsync()`
- ✅ `UnlockUserAsync()`

---

### 5. **DTOs**

#### 📄 UserResponseDto
```csharp
public class UserResponseDto
{
    public string TcKimlikNo { get; set; }
    
    // Dinamik Veriler (User tablosundan)
    public bool AktifMi { get; set; }
    public DateTime? SonGirisTarihi { get; set; }
    public int BasarisizGirisSayisi { get; set; }
    public DateTime? HesapKilitTarihi { get; set; }
    
    // Personel Bilgileri (İlişkili - Statik veriler)
    public string? PersonelAdSoyad { get; set; }
    public string? Email { get; set; }           // Personel'den
    public string? CepTelefonu { get; set; }     // Personel'den
    public int? SicilNo { get; set; }
    public string? DepartmanAdi { get; set; }
    public string? ServisAdi { get; set; }
}
```

#### 📄 UserUpdateRequestDto
```csharp
public class UserUpdateRequestDto
{
    public bool AktifMi { get; set; }  // SADECE bu alan güncellenebilir
}
```

**NOT:** Email, CepTelefonu güncellemesi Personel üzerinden yapılır!

#### 📄 UserCreateRequestDto
**❌ KALDIRILDI** - User artık PersonelService tarafından otomatik oluşturuluyor!

---

### 6. **UserService Değişiklikleri**

**Kaldırılan Metodlar:**
- ❌ `CreateAsync()` - User artık PersonelService tarafından oluşturuluyor
- ❌ `GetByKullaniciAdiAsync()` - Personel'den yapılacak

**Kalan Metodlar:**
- ✅ `GetByTcKimlikNoAsync()`
- ✅ `GetAllAsync()`
- ✅ `GetActiveUsersAsync()`
- ✅ `GetLockedUsersAsync()`
- ✅ `UpdateAsync()` - Sadece AktifMi güncellenebilir
- ✅ `DeleteAsync()`
- ✅ `ChangePasswordAsync()`
- ✅ `ResetPasswordAsync()`
- ✅ `LockUserAsync()`
- ✅ `UnlockUserAsync()`
- ✅ `ActivateUserAsync()`
- ✅ `DeactivateUserAsync()`
- ✅ `ClearSessionAsync()`
- ✅ `GetBySessionIdAsync()`

---

## 🗄️ Database Migration

### Data Migration Script
**Lokasyon:** `DataMigrationScripts/MigratePersonelToUser.sql`

```sql
-- SADECE DİNAMİK ALANLAR User tablosuna kopyalanıyor
INSERT INTO [dbo].[CMN_Users] 
(
    TcKimlikNo, 
    PassWord,      -- Personel'den kopyalanıyor
    SessionID,     -- Personel'den kopyalanıyor
    AktifMi,       -- PersonelAktiflikDurum'dan hesaplanıyor
    SonGirisTarihi,
    BasarisizGirisSayisi,
    HesapKilitTarihi,
    ...
)
SELECT 
    p.TcKimlikNo,
    p.PassWord,
    p.SessionID,
    CASE WHEN p.PersonelAktiflikDurum = 1 THEN 1 ELSE 0 END AS AktifMi,
    NULL AS SonGirisTarihi,
    0 AS BasarisizGirisSayisi,
    NULL AS HesapKilitTarihi,
    ...
FROM [dbo].[PER_Personeller] p
```

**NOT:** Email, CepTelefonu, NickName kopyalanmıyor - zaten Personel'de!

---

## 🔗 İlişki Diyagramı

```
┌─────────────────┐
│    Personel     │ (Statik Veriler)
│─────────────────│
│ TcKimlikNo (PK) │
│ SicilNo         │
│ AdSoyad         │
│ Email           │◄─── STATİK (User'da değil!)
│ CepTelefonu     │◄─── STATİK (User'da değil!)
│ NickName        │◄─── STATİK (User'da değil!)
│ DepartmanId     │
│ ServisId        │
│ ...             │
└────────┬────────┘
         │ 1:1
         │
┌────────▼────────┐
│      User       │ (SADECE Dinamik Veriler)
│─────────────────│
│ TcKimlikNo (PK) │◄─── Foreign Key
│ PassWord        │◄─── DİNAMİK
│ SessionID       │◄─── DİNAMİK
│ AktifMi         │◄─── DİNAMİK
│ SonGirisTarihi  │◄─── DİNAMİK
│ BasarisizGiris  │◄─── DİNAMİK
│ HesapKilitTarihi│◄─── DİNAMİK
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
2. Personel entity oluşturulur (Email, CepTelefonu dahil)
3. User entity otomatik oluşturulur (SADECE dinamik alanlar)
   - PassWord = TcKimlikNo (varsayılan)
   - AktifMi = PersonelAktiflikDurum'a göre
4. Her ikisi de veritabanına kaydedilir
```

### Login İşlemi
```
1. AuthService.LoginAsync() çağrılır
2. User tablosundan TcKimlikNo ile arama yapılır
3. Personel bilgileri Include ile yüklenir
4. Email, CepTelefonu Personel'den alınır (User'da yok!)
5. Güvenlik kontrolleri yapılır
6. Başarılı girişte User tablosu güncellenir
```

### Email/Telefon Güncelleme
```
1. PersonelService.UpdateAsync() çağrılır
2. Personel.Email veya Personel.CepTelefonu güncellenir
3. User tablosu etkilenmez (Email User'da yok!)
```

### Şifre Değiştirme
```
1. UserService.ChangePasswordAsync() çağrılır
2. User.PassWord güncellenir
3. Personel tablosu etkilenmez
```

---

## 📋 Statik vs Dinamik Alanlar

### Personel Tablosu (Statik)
✅ TcKimlikNo
✅ SicilNo
✅ AdSoyad
✅ **Email**
✅ **CepTelefonu**
✅ **NickName**
✅ DepartmanId
✅ ServisId
✅ UnvanId
✅ HizmetBinasiId
✅ DogumTarihi
✅ Cinsiyet
✅ MedeniDurumu
✅ Adres
✅ PersonelAktiflikDurum (Kadro durumu)
✅ ...

### User Tablosu (Dinamik)
✅ TcKimlikNo (FK)
✅ **PassWord**
✅ **SessionID**
✅ **AktifMi**
✅ **SonGirisTarihi**
✅ **BasarisizGirisSayisi**
✅ **HesapKilitTarihi**

---

## ⚠️ Önemli Notlar

### Cascade Delete
- **Personel silindiğinde → User otomatik silinir**
- **User silindiğinde → HubConnection otomatik silinir**

### Varsayılan Değerler
- **Şifre:** TC Kimlik No
- **AktifMi:** Personel.PersonelAktiflikDurum'a göre

### Email/Telefon Güncellemesi
- **Personel.Email** ve **Personel.CepTelefonu** PersonelService üzerinden güncellenir
- User tablosunda bu alanlar YOK!

### Login
- TC Kimlik No ile giriş yapılır
- Email Personel tablosundan alınır
- Şifre User tablosunda kontrol edilir

---

## 🎉 SONUÇ

**Statik ve Dinamik Veriler Doğru Yerde:**
- ✅ Email, CepTelefonu, NickName → **Personel** (Statik)
- ✅ PassWord, SessionID, AktifMi, SonGirisTarihi → **User** (Dinamik)
- ✅ One-to-One ilişki kuruldu
- ✅ Cascade Delete aktif
- ✅ Güvenlik kontrolleri eklendi

**Migration'ı oluşturup uygulayabilirsiniz!** 🚀
