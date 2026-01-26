# ============================================
# EF Core Migration Komutları - Standart Kullanım
# ============================================
# 
# KULLANIM:
# 1. Visual Studio Package Manager Console'da bu dosyayı çalıştır:
#    . .\migration-commands.ps1
# 
# 2. Kısa komutları kullan:
#    Add-Mig "mig_1"
#    Update-DB
#    Remove-Mig
# ============================================

# SGKDbContext için migration (SQL Server - Ana Veritabanı)
# ============================================

# Yeni migration oluştur
function Add-SGKMigration {
    param([string]$Name)
    dotnet ef migrations add $Name `
        --project SGKPortalApp.DataAccessLayer `
        --startup-project SGKPortalApp.ApiLayer `
        --context SGKDbContext
}

# Database'i güncelle
function Update-SGKDatabase {
    dotnet ef database update `
        --project SGKPortalApp.DataAccessLayer `
        --startup-project SGKPortalApp.ApiLayer `
        --context SGKDbContext
}

# Son migration'ı geri al
function Remove-SGKMigration {
    dotnet ef migrations remove `
        --project SGKPortalApp.DataAccessLayer `
        --startup-project SGKPortalApp.ApiLayer `
        --context SGKDbContext
}

# Migration listesini göster
function Get-SGKMigrations {
    dotnet ef migrations list `
        --project SGKPortalApp.DataAccessLayer `
        --startup-project SGKPortalApp.ApiLayer `
        --context SGKDbContext
}

# ============================================
# MysqlDbContext için migration (Legacy MySQL)
# ============================================
# NOT: Legacy MySQL veritabanı için migration YAPMA!
# Bu sadece okuma amaçlı kullanılıyor.

# ============================================
# KISA ALIASLAR (Package Manager Console için)
# ============================================

# Kısa ve kolay komutlar
function Add-Mig {
    param([Parameter(Mandatory=$true)][string]$Name)
    Add-Migration $Name -Context SGKDbContext
}

function Update-DB {
    Update-Database -Context SGKDbContext
}

function Remove-Mig {
    Remove-Migration -Context SGKDbContext
}

function Get-Migs {
    Get-Migration -Context SGKDbContext
}

# ============================================
# Kullanım Örnekleri
# ============================================

Write-Host ""
Write-Host "✅ Migration komutları yüklendi!" -ForegroundColor Green
Write-Host ""
Write-Host "📦 KISA KOMUTLAR (Package Manager Console):" -ForegroundColor Cyan
Write-Host "   Add-Mig 'mig_1'        - Yeni migration ekle" -ForegroundColor White
Write-Host "   Update-DB              - Database güncelle" -ForegroundColor White
Write-Host "   Remove-Mig             - Son migration'ı sil" -ForegroundColor White
Write-Host "   Get-Migs               - Migration'ları listele" -ForegroundColor White
Write-Host ""
Write-Host "🔧 UZUN KOMUTLAR (CLI için):" -ForegroundColor Cyan
Write-Host "   Add-SGKMigration 'mig_1'" -ForegroundColor White
Write-Host "   Update-SGKDatabase" -ForegroundColor White
Write-Host "   Remove-SGKMigration" -ForegroundColor White
Write-Host "   Get-SGKMigrations" -ForegroundColor White
Write-Host ""
