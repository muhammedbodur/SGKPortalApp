-- =============================================
-- Script: Eksik ModulController Kayıtlarını Ekleme
-- Tarih: 2025-12-19
-- Açıklama: Siramatik ve Personel modüllerine ait alt controller'ları ekler
-- =============================================

USE [SGKPortalDB]
GO

BEGIN TRANSACTION;

DECLARE @SiraModulId INT;
DECLARE @PersonelModulId INT;

-- ModulId'leri al
SELECT @SiraModulId = ModulId FROM PER_Moduller WHERE ModulKodu = 'SIRA' AND SilindiMi = 0;
SELECT @PersonelModulId = ModulId FROM PER_Moduller WHERE ModulKodu = 'PER' AND SilindiMi = 0;

-- Kontrol: Modüller var mı?
IF @SiraModulId IS NULL
BEGIN
    RAISERROR('HATA: SIRA modülü bulunamadı! Önce PER_Moduller tablosuna SIRA modülünü ekleyin.', 16, 1);
    ROLLBACK TRANSACTION;
    RETURN;
END

IF @PersonelModulId IS NULL
BEGIN
    RAISERROR('HATA: PER modülü bulunamadı! Önce PER_Moduller tablosuna PER modülünü ekleyin.', 16, 1);
    ROLLBACK TRANSACTION;
    RETURN;
END

PRINT '✅ Modüller bulundu:';
PRINT '   - SIRA ModulId: ' + CAST(@SiraModulId AS NVARCHAR(10));
PRINT '   - PER ModulId: ' + CAST(@PersonelModulId AS NVARCHAR(10));
PRINT '';

-- =============================================
-- SIRAMATIK MODÜLÜNDEKİ ALT CONTROLLER'LARI EKLE
-- =============================================
PRINT '🔵 Siramatik Modülü Controller''ları ekleniyor...';
PRINT '';

-- 1. BANKO
IF NOT EXISTS (SELECT 1 FROM PER_ModulControllers WHERE ModulId = @SiraModulId AND ModulControllerAdi = 'Banko' AND SilindiMi = 0)
BEGIN
    INSERT INTO PER_ModulControllers (ModulId, ModulControllerAdi, Aktiflik, EklenmeTarihi, DuzenlenmeTarihi, SilindiMi)
    VALUES (@SiraModulId, 'Banko', 1, GETDATE(), GETDATE(), 0);
    PRINT '   ✅ Banko controller eklendi';
END
ELSE
    PRINT '   ⏭️  Banko controller zaten mevcut';

-- 2. KANAL
IF NOT EXISTS (SELECT 1 FROM PER_ModulControllers WHERE ModulId = @SiraModulId AND ModulControllerAdi = 'Kanal' AND SilindiMi = 0)
BEGIN
    INSERT INTO PER_ModulControllers (ModulId, ModulControllerAdi, Aktiflik, EklenmeTarihi, DuzenlenmeTarihi, SilindiMi)
    VALUES (@SiraModulId, 'Kanal', 1, GETDATE(), GETDATE(), 0);
    PRINT '   ✅ Kanal controller eklendi';
END
ELSE
    PRINT '   ⏭️  Kanal controller zaten mevcut';

-- 3. KANALALT
IF NOT EXISTS (SELECT 1 FROM PER_ModulControllers WHERE ModulId = @SiraModulId AND ModulControllerAdi = 'KanalAlt' AND SilindiMi = 0)
BEGIN
    INSERT INTO PER_ModulControllers (ModulId, ModulControllerAdi, Aktiflik, EklenmeTarihi, DuzenlenmeTarihi, SilindiMi)
    VALUES (@SiraModulId, 'KanalAlt', 1, GETDATE(), GETDATE(), 0);
    PRINT '   ✅ KanalAlt controller eklendi';
END
ELSE
    PRINT '   ⏭️  KanalAlt controller zaten mevcut';

-- 4. KANALISLEM
IF NOT EXISTS (SELECT 1 FROM PER_ModulControllers WHERE ModulId = @SiraModulId AND ModulControllerAdi = 'KanalIslem' AND SilindiMi = 0)
BEGIN
    INSERT INTO PER_ModulControllers (ModulId, ModulControllerAdi, Aktiflik, EklenmeTarihi, DuzenlenmeTarihi, SilindiMi)
    VALUES (@SiraModulId, 'KanalIslem', 1, GETDATE(), GETDATE(), 0);
    PRINT '   ✅ KanalIslem controller eklendi';
END
ELSE
    PRINT '   ⏭️  KanalIslem controller zaten mevcut';

-- 5. KANALALTISLEM
IF NOT EXISTS (SELECT 1 FROM PER_ModulControllers WHERE ModulId = @SiraModulId AND ModulControllerAdi = 'KanalAltIslem' AND SilindiMi = 0)
BEGIN
    INSERT INTO PER_ModulControllers (ModulId, ModulControllerAdi, Aktiflik, EklenmeTarihi, DuzenlenmeTarihi, SilindiMi)
    VALUES (@SiraModulId, 'KanalAltIslem', 1, GETDATE(), GETDATE(), 0);
    PRINT '   ✅ KanalAltIslem controller eklendi';
END
ELSE
    PRINT '   ⏭️  KanalAltIslem controller zaten mevcut';

-- 6. KIOSK
IF NOT EXISTS (SELECT 1 FROM PER_ModulControllers WHERE ModulId = @SiraModulId AND ModulControllerAdi = 'Kiosk' AND SilindiMi = 0)
BEGIN
    INSERT INTO PER_ModulControllers (ModulId, ModulControllerAdi, Aktiflik, EklenmeTarihi, DuzenlenmeTarihi, SilindiMi)
    VALUES (@SiraModulId, 'Kiosk', 1, GETDATE(), GETDATE(), 0);
    PRINT '   ✅ Kiosk controller eklendi';
END
ELSE
    PRINT '   ⏭️  Kiosk controller zaten mevcut';

-- 7. KIOSKMENU
IF NOT EXISTS (SELECT 1 FROM PER_ModulControllers WHERE ModulId = @SiraModulId AND ModulControllerAdi = 'KioskMenu' AND SilindiMi = 0)
BEGIN
    INSERT INTO PER_ModulControllers (ModulId, ModulControllerAdi, Aktiflik, EklenmeTarihi, DuzenlenmeTarihi, SilindiMi)
    VALUES (@SiraModulId, 'KioskMenu', 1, GETDATE(), GETDATE(), 0);
    PRINT '   ✅ KioskMenu controller eklendi';
END
ELSE
    PRINT '   ⏭️  KioskMenu controller zaten mevcut';

-- 8. KIOSKMENUATAMA
IF NOT EXISTS (SELECT 1 FROM PER_ModulControllers WHERE ModulId = @SiraModulId AND ModulControllerAdi = 'KioskMenuAtama' AND SilindiMi = 0)
BEGIN
    INSERT INTO PER_ModulControllers (ModulId, ModulControllerAdi, Aktiflik, EklenmeTarihi, DuzenlenmeTarihi, SilindiMi)
    VALUES (@SiraModulId, 'KioskMenuAtama', 1, GETDATE(), GETDATE(), 0);
    PRINT '   ✅ KioskMenuAtama controller eklendi';
END
ELSE
    PRINT '   ⏭️  KioskMenuAtama controller zaten mevcut';

-- 9. KIOSKMENUISLEM
IF NOT EXISTS (SELECT 1 FROM PER_ModulControllers WHERE ModulId = @SiraModulId AND ModulControllerAdi = 'KioskMenuIslem' AND SilindiMi = 0)
BEGIN
    INSERT INTO PER_ModulControllers (ModulId, ModulControllerAdi, Aktiflik, EklenmeTarihi, DuzenlenmeTarihi, SilindiMi)
    VALUES (@SiraModulId, 'KioskMenuIslem', 1, GETDATE(), GETDATE(), 0);
    PRINT '   ✅ KioskMenuIslem controller eklendi';
END
ELSE
    PRINT '   ⏭️  KioskMenuIslem controller zaten mevcut';

-- 10. TV
IF NOT EXISTS (SELECT 1 FROM PER_ModulControllers WHERE ModulId = @SiraModulId AND ModulControllerAdi = 'Tv' AND SilindiMi = 0)
BEGIN
    INSERT INTO PER_ModulControllers (ModulId, ModulControllerAdi, Aktiflik, EklenmeTarihi, DuzenlenmeTarihi, SilindiMi)
    VALUES (@SiraModulId, 'Tv', 1, GETDATE(), GETDATE(), 0);
    PRINT '   ✅ Tv controller eklendi';
END
ELSE
    PRINT '   ⏭️  Tv controller zaten mevcut';

-- 11. PERSONELATAMA
IF NOT EXISTS (SELECT 1 FROM PER_ModulControllers WHERE ModulId = @SiraModulId AND ModulControllerAdi = 'PersonelAtama' AND SilindiMi = 0)
BEGIN
    INSERT INTO PER_ModulControllers (ModulId, ModulControllerAdi, Aktiflik, EklenmeTarihi, DuzenlenmeTarihi, SilindiMi)
    VALUES (@SiraModulId, 'PersonelAtama', 1, GETDATE(), GETDATE(), 0);
    PRINT '   ✅ PersonelAtama controller eklendi';
END
ELSE
    PRINT '   ⏭️  PersonelAtama controller zaten mevcut';

-- 12. SIGNALRLOGS
IF NOT EXISTS (SELECT 1 FROM PER_ModulControllers WHERE ModulId = @SiraModulId AND ModulControllerAdi = 'SignalRLogs' AND SilindiMi = 0)
BEGIN
    INSERT INTO PER_ModulControllers (ModulId, ModulControllerAdi, Aktiflik, EklenmeTarihi, DuzenlenmeTarihi, SilindiMi)
    VALUES (@SiraModulId, 'SignalRLogs', 1, GETDATE(), GETDATE(), 0);
    PRINT '   ✅ SignalRLogs controller eklendi';
END
ELSE
    PRINT '   ⏭️  SignalRLogs controller zaten mevcut';

-- 13. KIOSKSIMULATOR
IF NOT EXISTS (SELECT 1 FROM PER_ModulControllers WHERE ModulId = @SiraModulId AND ModulControllerAdi = 'KioskSimulator' AND SilindiMi = 0)
BEGIN
    INSERT INTO PER_ModulControllers (ModulId, ModulControllerAdi, Aktiflik, EklenmeTarihi, DuzenlenmeTarihi, SilindiMi)
    VALUES (@SiraModulId, 'KioskSimulator', 1, GETDATE(), GETDATE(), 0);
    PRINT '   ✅ KioskSimulator controller eklendi';
END
ELSE
    PRINT '   ⏭️  KioskSimulator controller zaten mevcut';

PRINT '';
PRINT '✅ Siramatik modülü tamamlandı!';
PRINT '';

-- =============================================
-- PERSONEL MODÜLÜNDEKİ ALT CONTROLLER'LARI EKLE
-- =============================================
PRINT '🔵 Personel Modülü Controller''ları ekleniyor...';
PRINT '';

-- 1. PERSONEL (Ana Personel)
IF NOT EXISTS (SELECT 1 FROM PER_ModulControllers WHERE ModulId = @PersonelModulId AND ModulControllerAdi = 'Personel' AND SilindiMi = 0)
BEGIN
    INSERT INTO PER_ModulControllers (ModulId, ModulControllerAdi, Aktiflik, EklenmeTarihi, DuzenlenmeTarihi, SilindiMi)
    VALUES (@PersonelModulId, 'Personel', 1, GETDATE(), GETDATE(), 0);
    PRINT '   ✅ Personel controller eklendi';
END
ELSE
    PRINT '   ⏭️  Personel controller zaten mevcut';

-- 2. ATANMANEDENI
IF NOT EXISTS (SELECT 1 FROM PER_ModulControllers WHERE ModulId = @PersonelModulId AND ModulControllerAdi = 'AtanmaNedeni' AND SilindiMi = 0)
BEGIN
    INSERT INTO PER_ModulControllers (ModulId, ModulControllerAdi, Aktiflik, EklenmeTarihi, DuzenlenmeTarihi, SilindiMi)
    VALUES (@PersonelModulId, 'AtanmaNedeni', 1, GETDATE(), GETDATE(), 0);
    PRINT '   ✅ AtanmaNedeni controller eklendi';
END
ELSE
    PRINT '   ⏭️  AtanmaNedeni controller zaten mevcut';

-- 3. DEPARTMAN
IF NOT EXISTS (SELECT 1 FROM PER_ModulControllers WHERE ModulId = @PersonelModulId AND ModulControllerAdi = 'Departman' AND SilindiMi = 0)
BEGIN
    INSERT INTO PER_ModulControllers (ModulId, ModulControllerAdi, Aktiflik, EklenmeTarihi, DuzenlenmeTarihi, SilindiMi)
    VALUES (@PersonelModulId, 'Departman', 1, GETDATE(), GETDATE(), 0);
    PRINT '   ✅ Departman controller eklendi';
END
ELSE
    PRINT '   ⏭️  Departman controller zaten mevcut';

-- 4. SENDIKA
IF NOT EXISTS (SELECT 1 FROM PER_ModulControllers WHERE ModulId = @PersonelModulId AND ModulControllerAdi = 'Sendika' AND SilindiMi = 0)
BEGIN
    INSERT INTO PER_ModulControllers (ModulId, ModulControllerAdi, Aktiflik, EklenmeTarihi, DuzenlenmeTarihi, SilindiMi)
    VALUES (@PersonelModulId, 'Sendika', 1, GETDATE(), GETDATE(), 0);
    PRINT '   ✅ Sendika controller eklendi';
END
ELSE
    PRINT '   ⏭️  Sendika controller zaten mevcut';

-- 5. SERVIS
IF NOT EXISTS (SELECT 1 FROM PER_ModulControllers WHERE ModulId = @PersonelModulId AND ModulControllerAdi = 'Servis' AND SilindiMi = 0)
BEGIN
    INSERT INTO PER_ModulControllers (ModulId, ModulControllerAdi, Aktiflik, EklenmeTarihi, DuzenlenmeTarihi, SilindiMi)
    VALUES (@PersonelModulId, 'Servis', 1, GETDATE(), GETDATE(), 0);
    PRINT '   ✅ Servis controller eklendi';
END
ELSE
    PRINT '   ⏭️  Servis controller zaten mevcut';

-- 6. UNVAN
IF NOT EXISTS (SELECT 1 FROM PER_ModulControllers WHERE ModulId = @PersonelModulId AND ModulControllerAdi = 'Unvan' AND SilindiMi = 0)
BEGIN
    INSERT INTO PER_ModulControllers (ModulId, ModulControllerAdi, Aktiflik, EklenmeTarihi, DuzenlenmeTarihi, SilindiMi)
    VALUES (@PersonelModulId, 'Unvan', 1, GETDATE(), GETDATE(), 0);
    PRINT '   ✅ Unvan controller eklendi';
END
ELSE
    PRINT '   ⏭️  Unvan controller zaten mevcut';

PRINT '';
PRINT '✅ Personel modülü tamamlandı!';
PRINT '';

-- =============================================
-- SONUÇLARI GÖSTER
-- =============================================
PRINT '📊 Son Durum:';
PRINT '';

DECLARE @SiraControllerCount INT;
DECLARE @PersonelControllerCount INT;

SELECT @SiraControllerCount = COUNT(*) FROM PER_ModulControllers WHERE ModulId = @SiraModulId AND SilindiMi = 0;
SELECT @PersonelControllerCount = COUNT(*) FROM PER_ModulControllers WHERE ModulId = @PersonelModulId AND SilindiMi = 0;

PRINT '   📌 Siramatik Modülü: ' + CAST(@SiraControllerCount AS NVARCHAR(10)) + ' controller';
PRINT '   📌 Personel Modülü: ' + CAST(@PersonelControllerCount AS NVARCHAR(10)) + ' controller';
PRINT '';

-- Siramatik controller listesi
PRINT '   Siramatik Controller''ları:';
SELECT '      - ' + ModulControllerAdi AS [Controller Adı]
FROM PER_ModulControllers
WHERE ModulId = @SiraModulId AND SilindiMi = 0
ORDER BY ModulControllerAdi;

PRINT '';

-- Personel controller listesi
PRINT '   Personel Controller''ları:';
SELECT '      - ' + ModulControllerAdi AS [Controller Adı]
FROM PER_ModulControllers
WHERE ModulId = @PersonelModulId AND SilindiMi = 0
ORDER BY ModulControllerAdi;

PRINT '';
PRINT '🎉 İşlem başarıyla tamamlandı!';
PRINT '';
PRINT '⚠️  NOT: Bu transaction henüz commit edilmedi.';
PRINT '   ✅ Sonuçları kontrol edin ve sorun yoksa COMMIT TRANSACTION çalıştırın.';
PRINT '   ❌ Sorun varsa ROLLBACK TRANSACTION çalıştırın.';

-- Transaction'ı commit etmek için aşağıdaki satırın yorumunu kaldırın:
-- COMMIT TRANSACTION;
