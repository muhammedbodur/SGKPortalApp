# GitHub ile .NET Core 9 Blazor Server Deployment Rehberi

Bu rehber, .NET Core 9 Blazor Server uygulamanızı GitHub üzerinden Windows Server'a nasıl deploy edeceğinizi adım adım anlatmaktadır.

## İçindekiler
- [1. GitHub Repository Oluşturma](#1-github-repository-oluşturma)
- [2. Lokalde Git Kurulumu ve İlk Push](#2-lokalde-git-kurulumu-ve-ilk-push)
- [3. Sunucuda Git Kurulumu](#3-sunucuda-git-kurulumu)
- [4. Sunucuda Otomatik Deployment Script](#4-sunucuda-otomatik-deployment-script)
- [5. Lokalde Pratik Push Script](#5-lokalde-pratik-push-script)
- [6. Sunucuda Zamanlanmış Otomatik Deployment](#6-sunucuda-zamanlanmış-otomatik-deployment)
- [7. Sunucuda Manuel Deployment için Kısayol](#7-sunucuda-manuel-deployment-için-kısayol)
- [8. Tam İş Akışı](#8-tam-iş-akışı)
- [9. Appsettings Yönetimi](#9-appsettings-yönetimi)
- [10. Sorun Giderme](#10-sorun-giderme)

---

## 1. GitHub Repository Oluşturma

### GitHub'da:
1. [github.com](https://github.com)'a giriş yapın
2. Sağ üst köşede `+` → `New repository`
3. Repository adı: `MyBlazorApp` (örnek)
4. Private/Public seçin
5. `Create repository` butonuna tıklayın

---

## 2. Lokalde Git Kurulumu ve İlk Push

### Visual Studio'da projenizin bulunduğu klasörde:

```bash
# Git başlat
git init

# .gitignore oluştur (önemli!)
# Visual Studio'da: Solution'a sağ tık → Add → .gitignore
# Veya manuel oluşturun
```

### .gitignore içeriği (önemli dosyalar):

```gitignore
## Build results
[Dd]ebug/
[Rr]elease/
[Bb]in/
[Oo]bj/
publish/

## Visual Studio
.vs/
*.user
*.suo
*.userosscache

## Sensitive files
appsettings.Development.json
appsettings.Production.json
*.pfx
*.key
```

### Git komutları:

```bash
# İlk commit
git add .
git commit -m "İlk commit"

# GitHub'a bağlan
git remote add origin https://github.com/kullaniciadi/MyBlazorApp.git

# Main branch oluştur ve gönder
git branch -M main
git push -u origin main
```

---

## 3. Sunucuda Git Kurulumu

### Windows Server'da:

#### 1. Git'i indirin ve kurun:
- [https://git-scm.com/download/win](https://git-scm.com/download/win)
- Varsayılan ayarlarla kurulum yapın

#### 2. Deployment klasörü oluşturun:

```powershell
# PowerShell (Administrator olarak)
New-Item -ItemType Directory -Path "C:\GitDeploy"
cd C:\GitDeploy
```

#### 3. Repository'yi klonlayın:

```bash
# HTTPS ile (daha kolay)
git clone https://github.com/kullaniciadi/MyBlazorApp.git

# Veya SSH ile (daha güvenli)
git clone git@github.com:kullaniciadi/MyBlazorApp.git
```

### GitHub kimlik doğrulama:

```bash
# Personal Access Token oluşturun (GitHub'da)
# Settings → Developer settings → Personal access tokens → Tokens (classic)
# repo yetkisini verin

# Git credential manager ayarı
git config --global credential.helper wincred
# İlk git pull'da kullanıcı adı ve token soracak
```

---

## 4. Sunucuda Otomatik Deployment Script

### C:\Scripts\deploy.ps1 oluşturun:

```powershell
# Deployment Script
param(
    [string]$ProjectPath = "C:\GitDeploy\MyBlazorApp",
    [string]$ApiPublishPath = "C:\inetpub\wwwroot\api",
    [string]$PresentationPublishPath = "C:\inetpub\wwwroot\presentation"
)

# Renkli çıktı için
function Write-ColorOutput($ForegroundColor) {
    $fc = $host.UI.RawUI.ForegroundColor
    $host.UI.RawUI.ForegroundColor = $ForegroundColor
    if ($args) {
        Write-Output $args
    }
    $host.UI.RawUI.ForegroundColor = $fc
}

Write-ColorOutput Green "========================================="
Write-ColorOutput Green "  Deployment Basladi"
Write-ColorOutput Green "========================================="

# 1. Son değişiklikleri çek
Write-ColorOutput Yellow "`n[1/5] Git Pull yapiliyor..."
Set-Location $ProjectPath
git pull origin main

if ($LASTEXITCODE -ne 0) {
    Write-ColorOutput Red "Git pull HATASI! Deployment durduruluyor."
    exit 1
}

# 2. NuGet paketlerini restore et
Write-ColorOutput Yellow "`n[2/5] NuGet paketleri yukleniyor..."
dotnet restore

# 3. API Publish
Write-ColorOutput Yellow "`n[3/5] API publish ediliyor..."
$apiProject = Get-ChildItem -Path $ProjectPath -Filter "*API*.csproj" -Recurse | Select-Object -First 1

if ($apiProject) {
    # IIS'i durdur (dosyaların kilitlenmemesi için)
    Stop-WebAppPool -Name "ApiAppPool" -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
    
    dotnet publish $apiProject.FullName -c Release -o $ApiPublishPath --no-restore
    
    if ($LASTEXITCODE -eq 0) {
        Write-ColorOutput Green "  ✓ API başarıyla publish edildi"
    } else {
        Write-ColorOutput Red "  ✗ API publish HATASI!"
        exit 1
    }
    
    Start-WebAppPool -Name "ApiAppPool"
} else {
    Write-ColorOutput Red "API projesi bulunamadi!"
}

# 4. Presentation Publish
Write-ColorOutput Yellow "`n[4/5] Presentation publish ediliyor..."
$presentationProject = Get-ChildItem -Path $ProjectPath -Filter "*Presentation*.csproj" -Recurse | Select-Object -First 1

if ($presentationProject) {
    Stop-WebAppPool -Name "PresentationAppPool" -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
    
    dotnet publish $presentationProject.FullName -c Release -o $PresentationPublishPath --no-restore
    
    if ($LASTEXITCODE -eq 0) {
        Write-ColorOutput Green "  ✓ Presentation başarıyla publish edildi"
    } else {
        Write-ColorOutput Red "  ✗ Presentation publish HATASI!"
        exit 1
    }
    
    Start-WebAppPool -Name "PresentationAppPool"
} else {
    Write-ColorOutput Red "Presentation projesi bulunamadi!"
}

# 5. IIS Recycle
Write-ColorOutput Yellow "`n[5/5] IIS yeniden baslatiliyor..."
iisreset /restart

# Log kaydet
$logPath = "C:\Scripts\deploy_log.txt"
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
Add-Content -Path $logPath -Value "$timestamp - Deployment tamamlandi"

Write-ColorOutput Green "`n========================================="
Write-ColorOutput Green "  Deployment BAŞARIYLA tamamlandi!"
Write-ColorOutput Green "========================================="
```

### Script'i çalıştırılabilir yapın:

```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

---

## 5. Lokalde Pratik Push Script

### Lokal makinenizde deploy.bat oluşturun:

```batch
@echo off
echo ========================================
echo   GitHub'a Gonderiliyor
echo ========================================

REM Değişiklikleri ekle
git add .

REM Commit mesajı iste
set /p message="Commit mesaji girin: "
if "%message%"=="" set message="Guncelleme"

git commit -m "%message%"

REM Push et
echo.
echo GitHub'a gonderiliyor...
git push origin main

if %errorlevel% equ 0 (
    echo.
    echo ========================================
    echo   BASARILI! Kod GitHub'a gonderildi
    echo ========================================
    echo.
    echo Sunucuda deployment icin:
    echo 1. Sunucuya RDP ile baglanin
    echo 2. PowerShell'i ac
    echo 3. C:\Scripts\deploy.ps1 calistirin
    echo.
) else (
    echo.
    echo HATA! Push basarisiz oldu.
    echo.
)

pause
```

---

## 6. Sunucuda Zamanlanmış Otomatik Deployment

### Windows Task Scheduler ile otomatik deployment (İsteğe Bağlı):

```powershell
# Task oluştur
$action = New-ScheduledTaskAction -Execute "PowerShell.exe" -Argument "-File C:\Scripts\deploy.ps1"
$trigger = New-ScheduledTaskTrigger -Daily -At 3am  # Her gece saat 3'te
$principal = New-ScheduledTaskPrincipal -UserId "SYSTEM" -LogonType ServiceAccount -RunLevel Highest
$settings = New-ScheduledTaskSettingsSet -StartWhenAvailable

Register-ScheduledTask -TaskName "AutoDeploy" -Action $action -Trigger $trigger -Principal $principal -Settings $settings
```

---

## 7. Sunucuda Manuel Deployment için Kısayol

### Masaüstüne kısayol oluşturun:

**deploy_shortcut.bat:**

```batch
@echo off
powershell.exe -ExecutionPolicy Bypass -File C:\Scripts\deploy.ps1
pause
```

---

## 8. Tam İş Akışı

### Lokalde (Geliştirme)

```bash
# 1. Kod değişikliği yap
# 2. Test et

# 3. GitHub'a gönder
git add .
git commit -m "Yeni özellik eklendi"
git push origin main
```

**Veya sadece `deploy.bat`'ı çalıştırın.**

### Sunucuda (Deployment)

#### Seçenek A - Manuel:
1. Masaüstündeki `deploy_shortcut.bat`'a çift tıkla
2. İşlemin bitmesini bekle

#### Seçenek B - PowerShell:
```powershell
C:\Scripts\deploy.ps1
```

#### Seçenek C - Otomatik:
- Hiçbir şey yapma, Task Scheduler her gece otomatik deploy eder

---

## 9. Appsettings Yönetimi

### Farklı ortamlar için farklı ayarlar:

**appsettings.Development.json** (Lokal - Git'e eklenmez)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyDb;..."
  },
  "ApiBaseUrl": "https://localhost:5001"
}
```

**appsettings.Production.json** (Sunucu - Git'e eklenmez)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=ProductionServer;Database=MyDb;..."
  },
  "ApiBaseUrl": "https://api.sunucunuz.com"
}
```

### Sunucuda appsettings.Production.json'u manuel oluşturun:

```powershell
# Publish sonrası dosyayı kopyala
Copy-Item "C:\Config\appsettings.Production.json" -Destination "C:\inetpub\wwwroot\api\" -Force
Copy-Item "C:\Config\appsettings.Production.json" -Destination "C:\inetpub\wwwroot\presentation\" -Force
```

### Deployment scriptine ekleyin:

```powershell
# appsettings.Production.json'u kopyala
if (Test-Path "C:\Config\appsettings.Production.json") {
    Copy-Item "C:\Config\appsettings.Production.json" -Destination $ApiPublishPath -Force
    Copy-Item "C:\Config\appsettings.Production.json" -Destination $PresentationPublishPath -Force
    Write-ColorOutput Green "  ✓ Production ayarlari kopyalandi"
}
```

---

## 10. Sorun Giderme

### Git pull hata veriyorsa:

```bash
# Credential sorunları
git config --global credential.helper manager-core

# Local değişiklikleri sıfırla
git reset --hard origin/main
git pull
```

### Publish hataları:

```powershell
# Build önbelleğini temizle
dotnet clean
dotnet build -c Release
```

### Log kontrolü:

```powershell
# Deployment logları
Get-Content C:\Scripts\deploy_log.txt -Tail 50

# IIS logları
Get-Content C:\inetpub\logs\LogFiles\W3SVC1\*.log -Tail 50
```

---

## Özet İş Akışı

```
[LOKAL]                [GITHUB]              [SUNUCU]
  ↓                        ↓                      ↓
Kod yaz  →  git push  →  Repository  →  git pull  →  Publish  →  IIS
  ↓                        ↓                      ↓
Test et      deploy.bat    Depola        deploy.ps1   Çalışır
```

---

## Sonuç

Bu sistemi kurduktan sonra:

1. **Lokalde:** Sadece `deploy.bat` çalıştır
2. **Sunucuda:** Sadece `deploy_shortcut.bat`'a çift tıkla

**Herhangi bir adımda sorun yaşarsanız, deployment loglarını kontrol edin ve gerekirse script parametrelerini projenize göre düzenleyin.**

---

**Son Güncelleme:** 2025
**Yazar:** Deployment Rehberi
**Versiyon:** 1.0