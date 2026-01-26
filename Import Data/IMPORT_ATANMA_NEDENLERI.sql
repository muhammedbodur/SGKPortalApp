-- ============================================================================
-- ATANMA NEDENLERİ IMPORT (MySQL Legacy Mapping)
-- MySQL'deki atanma tablosundan gelen değerleri SQL Server'a mapping
-- ============================================================================
USE SGKPortalDB;
GO

PRINT '📦 Atanma Nedenleri import ediliyor...';
GO

-- MySQL atanma tablosu değerleri:
-- 1  = Eş Durumu
-- 2  = Sağlık Durumu
-- 3  = Kurum İçi Nakil
-- 4  = Kurumlar Arası Nakil
-- 5  = KPSS İlk Atama
-- 6  = Atamaya Esas
-- 7  = Hizmetin Gereği
-- 8  = Görevde Yükselme
-- 9  = 3713 Sayılı Terörle Mücadele Yasası
-- 10 = Becayiş
-- 12 = Diğer
-- 13 = 2828 Sayılı Sosyal Hizmetler Kanunnuna Göre
-- 14 = E-KPSS
-- 15 = Denetmenlik Yeterlik Sınavı Sonucu Atama
-- 16 = Geçici Görevlendirme
-- 17 = Unvan Değişikliği
-- 18 = Engellilik Durumu
-- 19 = Açıktan Atama
-- 20 = Yeniden Atama

IF NOT EXISTS (SELECT 1 FROM PER_AtanmaNedenleri WHERE LegacyKod = 1)
    INSERT INTO PER_AtanmaNedenleri (AtanmaNedeni, LegacyKod, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi)
    VALUES (N'Eş Durumu', 1, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_AtanmaNedenleri WHERE LegacyKod = 2)
    INSERT INTO PER_AtanmaNedenleri (AtanmaNedeni, LegacyKod, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi)
    VALUES (N'Sağlık Durumu', 2, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_AtanmaNedenleri WHERE LegacyKod = 3)
    INSERT INTO PER_AtanmaNedenleri (AtanmaNedeni, LegacyKod, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi)
    VALUES (N'Kurum İçi Nakil', 3, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_AtanmaNedenleri WHERE LegacyKod = 4)
    INSERT INTO PER_AtanmaNedenleri (AtanmaNedeni, LegacyKod, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi)
    VALUES (N'Kurumlar Arası Nakil', 4, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_AtanmaNedenleri WHERE LegacyKod = 5)
    INSERT INTO PER_AtanmaNedenleri (AtanmaNedeni, LegacyKod, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi)
    VALUES (N'KPSS İlk Atama', 5, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_AtanmaNedenleri WHERE LegacyKod = 6)
    INSERT INTO PER_AtanmaNedenleri (AtanmaNedeni, LegacyKod, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi)
    VALUES (N'Atamaya Esas', 6, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_AtanmaNedenleri WHERE LegacyKod = 7)
    INSERT INTO PER_AtanmaNedenleri (AtanmaNedeni, LegacyKod, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi)
    VALUES (N'Hizmetin Gereği', 7, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_AtanmaNedenleri WHERE LegacyKod = 8)
    INSERT INTO PER_AtanmaNedenleri (AtanmaNedeni, LegacyKod, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi)
    VALUES (N'Görevde Yükselme', 8, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_AtanmaNedenleri WHERE LegacyKod = 9)
    INSERT INTO PER_AtanmaNedenleri (AtanmaNedeni, LegacyKod, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi)
    VALUES (N'3713 Sayılı Terörle Mücadele Yasası', 9, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_AtanmaNedenleri WHERE LegacyKod = 10)
    INSERT INTO PER_AtanmaNedenleri (AtanmaNedeni, LegacyKod, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi)
    VALUES (N'Becayiş', 10, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_AtanmaNedenleri WHERE LegacyKod = 12)
    INSERT INTO PER_AtanmaNedenleri (AtanmaNedeni, LegacyKod, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi)
    VALUES (N'Diğer', 12, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_AtanmaNedenleri WHERE LegacyKod = 13)
    INSERT INTO PER_AtanmaNedenleri (AtanmaNedeni, LegacyKod, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi)
    VALUES (N'2828 Sayılı Sosyal Hizmetler Kanunnuna Göre', 13, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_AtanmaNedenleri WHERE LegacyKod = 14)
    INSERT INTO PER_AtanmaNedenleri (AtanmaNedeni, LegacyKod, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi)
    VALUES (N'E-KPSS', 14, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_AtanmaNedenleri WHERE LegacyKod = 15)
    INSERT INTO PER_AtanmaNedenleri (AtanmaNedeni, LegacyKod, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi)
    VALUES (N'Denetmenlik Yeterlik Sınavı Sonucu Atama', 15, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_AtanmaNedenleri WHERE LegacyKod = 16)
    INSERT INTO PER_AtanmaNedenleri (AtanmaNedeni, LegacyKod, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi)
    VALUES (N'Geçici Görevlendirme', 16, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_AtanmaNedenleri WHERE LegacyKod = 17)
    INSERT INTO PER_AtanmaNedenleri (AtanmaNedeni, LegacyKod, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi)
    VALUES (N'Unvan Değişikliği', 17, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_AtanmaNedenleri WHERE LegacyKod = 18)
    INSERT INTO PER_AtanmaNedenleri (AtanmaNedeni, LegacyKod, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi)
    VALUES (N'Engellilik Durumu', 18, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_AtanmaNedenleri WHERE LegacyKod = 19)
    INSERT INTO PER_AtanmaNedenleri (AtanmaNedeni, LegacyKod, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi)
    VALUES (N'Açıktan Atama', 19, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_AtanmaNedenleri WHERE LegacyKod = 20)
    INSERT INTO PER_AtanmaNedenleri (AtanmaNedeni, LegacyKod, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi)
    VALUES (N'Yeniden Atama', 20, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

GO

PRINT '✅ 19 Atanma Nedeni import edildi';
GO
