-- =================================================================
-- FIX: BankoKullanici - Personel HizmetBinasi Tutarsızlığı
-- =================================================================
-- SORUN: Personel.HizmetBinasiId != BankoKullanici.HizmetBinasiId
--        olan kayıtlar var. Bu kayıtlar eski kodun çalıştığı dönemde
--        oluşmuş ve personel hizmet binası değiştiğinde temizlenmemiş.
--
-- ÇÖZÜM: Bu tutarsız kayıtları soft delete ile temizle
-- =================================================================

BEGIN TRANSACTION;

-- 1. Önce tutarsız kayıtları listele (kontrol için)
PRINT '========================================';
PRINT '1. TUTARSIZ KAYITLARI LİSTELEME';
PRINT '========================================';

SELECT
    bk.BankoKullaniciId,
    bk.TcKimlikNo,
    p.AdSoyad AS PersonelAdi,
    p.HizmetBinasiId AS PersonelHizmetBinasiId,
    ph.HizmetBinasiAdi AS PersonelHizmetBinasi,
    bk.HizmetBinasiId AS BankoKullaniciHizmetBinasiId,
    bkh.HizmetBinasiAdi AS BankoKullaniciHizmetBinasi,
    b.BankoId,
    b.BankoNo,
    b.HizmetBinasiId AS BankoHizmetBinasiId,
    bh.HizmetBinasiAdi AS BankoHizmetBinasi,
    bk.SilindiMi,
    bk.EklenmeTarihi,
    bk.DuzenlenmeTarihi
FROM SIR_BankoKullanicilari bk
INNER JOIN PER_Personeller p ON bk.TcKimlikNo = p.TcKimlikNo
INNER JOIN CMN_HizmetBinalari ph ON p.HizmetBinasiId = ph.HizmetBinasiId
INNER JOIN CMN_HizmetBinalari bkh ON bk.HizmetBinasiId = bkh.HizmetBinasiId
INNER JOIN SIR_Bankolar b ON bk.BankoId = b.BankoId
INNER JOIN CMN_HizmetBinalari bh ON b.HizmetBinasiId = bh.HizmetBinasiId
WHERE p.HizmetBinasiId != bk.HizmetBinasiId
  AND bk.SilindiMi = 0  -- Sadece aktif kayıtlar
ORDER BY bk.EklenmeTarihi DESC;

PRINT '';
PRINT '========================================';
PRINT '2. TUTARSIZ KAYITLARI SOFT DELETE YAPMA';
PRINT '========================================';

-- 2. Tutarsız kayıtları soft delete yap
DECLARE @AffectedRows INT;

UPDATE bk
SET
    bk.SilindiMi = 1,
    bk.SilinmeTarihi = GETDATE(),
    bk.SilenKullanici = 'SYSTEM_DATA_FIX',
    bk.DuzenlenmeTarihi = GETDATE()
FROM SIR_BankoKullanicilari bk
INNER JOIN PER_Personeller p ON bk.TcKimlikNo = p.TcKimlikNo
WHERE p.HizmetBinasiId != bk.HizmetBinasiId
  AND bk.SilindiMi = 0;  -- Sadece aktif kayıtlar

SET @AffectedRows = @@ROWCOUNT;

PRINT CONCAT('Toplam ', @AffectedRows, ' kayıt soft delete yapıldı.');
PRINT '';

-- 3. İlgili User kayıtlarındaki banko modu bilgilerini temizle
PRINT '========================================';
PRINT '3. İLGİLİ USER KAYITLARINI TEMİZLEME';
PRINT '========================================';

DECLARE @UserAffectedRows INT;

UPDATE u
SET
    u.BankoModuAktif = 0,
    u.AktifBankoId = NULL,
    u.BankoModuBaslangic = NULL
FROM CMN_Users u
INNER JOIN PER_Personeller p ON u.TcKimlikNo = p.TcKimlikNo
LEFT JOIN SIR_BankoKullanicilari bk ON u.TcKimlikNo = bk.TcKimlikNo AND bk.SilindiMi = 0
WHERE u.BankoModuAktif = 1
  AND bk.BankoKullaniciId IS NULL;  -- BankoKullanici kaydı yok veya silinmiş

SET @UserAffectedRows = @@ROWCOUNT;

PRINT CONCAT('Toplam ', @UserAffectedRows, ' kullanıcı banko modundan çıkarıldı.');
PRINT '';

-- 4. Doğrulama: Artık tutarsız kayıt kalmamalı
PRINT '========================================';
PRINT '4. DOĞRULAMA';
PRINT '========================================';

DECLARE @RemainingInconsistencies INT;

SELECT @RemainingInconsistencies = COUNT(*)
FROM SIR_BankoKullanicilari bk
INNER JOIN PER_Personeller p ON bk.TcKimlikNo = p.TcKimlikNo
WHERE p.HizmetBinasiId != bk.HizmetBinasiId
  AND bk.SilindiMi = 0;

IF @RemainingInconsistencies = 0
BEGIN
    PRINT '✓ BAŞARILI: Artık tutarsız kayıt kalmadı!';
    PRINT '';
    PRINT 'Özet:';
    PRINT CONCAT('  - ', @AffectedRows, ' BankoKullanici kaydı temizlendi');
    PRINT CONCAT('  - ', @UserAffectedRows, ' User kaydı güncellendi');
    PRINT '';
    PRINT 'Transaction commit ediliyor...';
    COMMIT TRANSACTION;
    PRINT '✓ Transaction başarıyla commit edildi.';
END
ELSE
BEGIN
    PRINT CONCAT('✗ UYARI: Hala ', @RemainingInconsistencies, ' tutarsız kayıt var!');
    PRINT 'Transaction rollback ediliyor...';
    ROLLBACK TRANSACTION;
    PRINT '✗ Transaction rollback edildi. Lütfen logları inceleyin.';
END

PRINT '';
PRINT '========================================';
PRINT 'İŞLEM TAMAMLANDI';
PRINT '========================================';
