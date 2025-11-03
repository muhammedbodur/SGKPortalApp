# 🔐 Şifre Yönetimi Dokümantasyonu

## 📋 Genel Bakış

SGK Portal uygulamasında şifre yönetimi **User** tablosu üzerinden yapılmaktadır. Şifreler şu anda düz metin olarak saklanmaktadır (ileride hash'lenecek).

---

## 🔑 Şifre İşlemleri

### 1️⃣ Şifre Değiştirme (Change Password)

Kullanıcı kendi şifresini değiştirebilir. **Eski şifre gereklidir.**

#### API Endpoint:
```http
POST /api/user/{tcKimlikNo}/change-password
Content-Type: application/json

{
  "oldPassword": "12345678901",
  "newPassword": "YeniSifre123",
  "confirmPassword": "YeniSifre123"
}
```

#### Servis Metodu:
```csharp
await _userService.ChangePasswordAsync(tcKimlikNo, oldPassword, newPassword);
```

#### İş Akışı:
1. ✅ Kullanıcı TC Kimlik No ile bulunur
2. ✅ Eski şifre kontrol edilir
3. ✅ Yeni şifre atanır
4. ✅ Veritabanına kaydedilir
5. ✅ Log kaydı oluşturulur

#### Validasyonlar:
- ✅ Eski şifre zorunlu (min 1 karakter)
- ✅ Yeni şifre zorunlu (min 6 karakter)
- ✅ Şifre tekrarı eşleşmeli

---

### 2️⃣ Şifre Sıfırlama (Reset Password)

Yönetici kullanıcının şifresini **TC Kimlik No'ya** sıfırlayabilir. **Eski şifre gerekmez.**

#### API Endpoint:
```http
POST /api/user/{tcKimlikNo}/reset-password
```

#### Servis Metodu:
```csharp
await _userService.ResetPasswordAsync(tcKimlikNo);
```

#### İş Akışı:
1. ✅ Kullanıcı TC Kimlik No ile bulunur
2. ✅ Şifre TC Kimlik No'ya sıfırlanır
3. ✅ Başarısız giriş sayısı sıfırlanır
4. ✅ Veritabanına kaydedilir
5. ✅ Log kaydı oluşturulur

#### Varsayılan Şifre:
```
Şifre = TC Kimlik No
Örnek: 12345678901
```

---

### 3️⃣ AuthService Reset Password

AuthService'te de şifre sıfırlama var (farklı DTO kullanıyor):

#### API Endpoint:
```http
POST /api/auth/reset-password
Content-Type: application/json

{
  "tcKimlikNo": "12345678901",
  "newPassword": "YeniSifre123"
}
```

#### Servis Metodu:
```csharp
await _authService.ResetPasswordAsync(request);
```

#### Fark:
- UserService: Şifreyi TC Kimlik No'ya sıfırlar
- AuthService: Belirtilen yeni şifreyi atar

---

## 📊 Veri Akışı

### Şifre Değiştirme Akışı:
```
┌─────────────┐
│   Kullanıcı │
└──────┬──────┘
       │ 1. Eski + Yeni Şifre
       ▼
┌─────────────────┐
│ UserController  │
└──────┬──────────┘
       │ 2. ChangePasswordAsync()
       ▼
┌─────────────────┐
│  UserService    │
└──────┬──────────┘
       │ 3. Eski şifre kontrolü
       │ 4. Yeni şifre ataması
       ▼
┌─────────────────┐
│  UserRepository │
└──────┬──────────┘
       │ 5. Update()
       ▼
┌─────────────────┐
│   User Tablosu  │
│  (PassWord)     │
└─────────────────┘
```

### Şifre Sıfırlama Akışı:
```
┌─────────────┐
│  Yönetici   │
└──────┬──────┘
       │ 1. TC Kimlik No
       ▼
┌─────────────────┐
│ UserController  │
└──────┬──────────┘
       │ 2. ResetPasswordAsync()
       ▼
┌─────────────────┐
│  UserService    │
└──────┬──────────┘
       │ 3. PassWord = TcKimlikNo
       │ 4. BasarisizGirisSayisi = 0
       ▼
┌─────────────────┐
│  UserRepository │
└──────┬──────────┘
       │ 5. Update()
       ▼
┌─────────────────┐
│   User Tablosu  │
│  (PassWord)     │
└─────────────────┘
```

---

## 🔒 Güvenlik Özellikleri

### Mevcut:
- ✅ Eski şifre kontrolü (değiştirme için)
- ✅ Şifre tekrarı validasyonu
- ✅ Minimum şifre uzunluğu (6 karakter)
- ✅ Başarısız giriş sayısı takibi
- ✅ Hesap kilitleme (5 başarısız denemede)
- ✅ Log kayıtları

### Gelecek İyileştirmeler:
- [ ] Şifre hashleme (BCrypt/PBKDF2)
- [ ] Şifre karmaşıklık kuralları
  - [ ] En az 1 büyük harf
  - [ ] En az 1 küçük harf
  - [ ] En az 1 rakam
  - [ ] En az 1 özel karakter
- [ ] Şifre geçmişi tutma (son 5 şifre)
- [ ] Şifre değiştirme zorunluluğu (90 günde bir)
- [ ] Email ile şifre sıfırlama linki
- [ ] 2FA (Two-Factor Authentication)

---

## 📝 DTOs

### ChangePasswordRequestDto
```csharp
public class ChangePasswordRequestDto
{
    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string OldPassword { get; set; }

    [Required]
    [StringLength(255, MinimumLength = 6)]
    public string NewPassword { get; set; }

    [Required]
    [Compare(nameof(NewPassword))]
    public string ConfirmPassword { get; set; }
}
```

### ResetPasswordRequestDto (AuthService)
```csharp
public class ResetPasswordRequestDto
{
    [Required]
    public string TcKimlikNo { get; set; }

    [Required]
    public string NewPassword { get; set; }
}
```

---

## 🧪 Test Senaryoları

### Test 1: Şifre Değiştirme (Başarılı)
```http
POST /api/user/12345678901/change-password
{
  "oldPassword": "12345678901",
  "newPassword": "YeniSifre123",
  "confirmPassword": "YeniSifre123"
}

Beklenen: 200 OK
```

### Test 2: Şifre Değiştirme (Eski Şifre Hatalı)
```http
POST /api/user/12345678901/change-password
{
  "oldPassword": "YanlisSifre",
  "newPassword": "YeniSifre123",
  "confirmPassword": "YeniSifre123"
}

Beklenen: 400 Bad Request - "Mevcut şifre hatalı"
```

### Test 3: Şifre Değiştirme (Şifre Tekrarı Eşleşmiyor)
```http
POST /api/user/12345678901/change-password
{
  "oldPassword": "12345678901",
  "newPassword": "YeniSifre123",
  "confirmPassword": "FarkliSifre"
}

Beklenen: 400 Bad Request - Validation Error
```

### Test 4: Şifre Sıfırlama (Yönetici)
```http
POST /api/user/12345678901/reset-password

Beklenen: 200 OK
Şifre: 12345678901 (TC Kimlik No)
```

### Test 5: Sıfırlanmış Şifre ile Giriş
```http
POST /api/auth/login
{
  "tcKimlikNo": "12345678901",
  "password": "12345678901"
}

Beklenen: 200 OK - Login başarılı
```

---

## 🎯 Kullanım Örnekleri

### Frontend (Blazor) - Şifre Değiştirme
```csharp
public async Task ChangePassword()
{
    var request = new ChangePasswordRequestDto
    {
        OldPassword = oldPassword,
        NewPassword = newPassword,
        ConfirmPassword = confirmPassword
    };

    var response = await Http.PostAsJsonAsync(
        $"api/user/{tcKimlikNo}/change-password", 
        request);

    if (response.IsSuccessStatusCode)
    {
        // Başarılı
        await ShowSuccessMessage("Şifreniz başarıyla değiştirildi");
    }
    else
    {
        // Hata
        var error = await response.Content.ReadAsStringAsync();
        await ShowErrorMessage(error);
    }
}
```

### Frontend (Blazor) - Şifre Sıfırlama (Yönetici)
```csharp
public async Task ResetUserPassword(string tcKimlikNo)
{
    var response = await Http.PostAsync(
        $"api/user/{tcKimlikNo}/reset-password", 
        null);

    if (response.IsSuccessStatusCode)
    {
        await ShowSuccessMessage($"Şifre TC Kimlik No'ya sıfırlandı: {tcKimlikNo}");
    }
}
```

---

## 📞 API Endpoints Özeti

| Method | Endpoint | Açıklama | Auth |
|--------|----------|----------|------|
| POST | `/api/user/{tcKimlikNo}/change-password` | Şifre değiştir | ✅ User |
| POST | `/api/user/{tcKimlikNo}/reset-password` | Şifre sıfırla | ✅ Admin |
| POST | `/api/auth/reset-password` | Şifre sıfırla (özel) | ✅ Admin |

---

## ⚠️ Önemli Notlar

1. **Şifreler Düz Metin:** Şu anda şifreler düz metin olarak saklanıyor. Üretim ortamında mutlaka hash'lenmelidir.

2. **Varsayılan Şifre:** Yeni kullanıcılar ve sıfırlanan şifreler TC Kimlik No'ya eşittir.

3. **Hesap Kilitleme:** 5 başarısız giriş denemesinde hesap otomatik kilitlenir.

4. **Yönetici Yetkisi:** Şifre sıfırlama işlemi sadece yöneticiler tarafından yapılmalıdır.

5. **Log Kayıtları:** Tüm şifre işlemleri loglanır.

---

## 🚀 Gelecek Geliştirmeler

### Öncelik 1 (Kritik):
- [ ] Şifre hashleme implementasyonu
- [ ] Şifre karmaşıklık kuralları
- [ ] Şifre geçmişi

### Öncelik 2 (Önemli):
- [ ] Email ile şifre sıfırlama
- [ ] Şifre değiştirme zorunluluğu
- [ ] 2FA

### Öncelik 3 (İyileştirme):
- [ ] Şifre gücü göstergesi (UI)
- [ ] Şifre önerileri
- [ ] Güvenlik soruları

---

**🔐 Güvenli Şifreleme!**
