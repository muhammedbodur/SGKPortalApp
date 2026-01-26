-- ============================================================================
-- SENDİKALAR FULL IMPORT (PHP Legacy Mapping)
-- PHP'deki tüm sendika değerlerini SQL Server'a mapping
-- ============================================================================
USE SGKPortalDB;
GO

PRINT '📦 Sendikalar (Full) import ediliyor...';
GO

-- PHP sendika mapping (LegacyKod):
-- 0 = Belirtilmemiş
-- 1 = Büro Emekçileri Sendikası
-- 2 = Büro Memur Sen
-- 3 = Türk Büro Sen
-- 4 = Türkiye Demokratik Ofis Çal. Sen.
-- 5 = Büro Hak Sen
-- 6 = Birleşik Büro-İş
-- 7 = Tüm Büro-Sen
-- 8 = Ufuk Büro Sen
-- 9 = Savdes-Sen
-- 99 = Üye Değil

-- NOT: SendikaId SQL Server tarafından otomatik atanır (IDENTITY)
-- LegacyKod ile MySQL değerleri mapping yapılır

-- Özel değerler
IF NOT EXISTS (SELECT 1 FROM PER_Sendikalar WHERE LegacyKod = 0) 
    INSERT INTO PER_Sendikalar (SendikaAdi, LegacyKod, Aktiflik, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi) 
    VALUES (N'-BELİRTİLMEMİŞ-', 0, 0, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_Sendikalar WHERE LegacyKod = 99) 
    INSERT INTO PER_Sendikalar (SendikaAdi, LegacyKod, Aktiflik, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi) 
    VALUES (N'-ÜYE DEĞİL-', 99, 0, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

-- Gerçek sendikalar (PHP'den gelen değerler)
IF NOT EXISTS (SELECT 1 FROM PER_Sendikalar WHERE LegacyKod = 1) 
    INSERT INTO PER_Sendikalar (SendikaAdi, LegacyKod, Aktiflik, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi) 
    VALUES (N'Büro Emekçileri Sendikası', 1, 0, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_Sendikalar WHERE LegacyKod = 2) 
    INSERT INTO PER_Sendikalar (SendikaAdi, LegacyKod, Aktiflik, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi) 
    VALUES (N'Büro Memur Sen', 2, 0, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_Sendikalar WHERE LegacyKod = 3) 
    INSERT INTO PER_Sendikalar (SendikaAdi, LegacyKod, Aktiflik, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi) 
    VALUES (N'Türk Büro Sen', 3, 0, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_Sendikalar WHERE LegacyKod = 4) 
    INSERT INTO PER_Sendikalar (SendikaAdi, LegacyKod, Aktiflik, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi) 
    VALUES (N'Türkiye Demokratik Ofis Çal. Sen.', 4, 0, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_Sendikalar WHERE LegacyKod = 5) 
    INSERT INTO PER_Sendikalar (SendikaAdi, LegacyKod, Aktiflik, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi) 
    VALUES (N'Büro Hak Sen', 5, 0, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_Sendikalar WHERE LegacyKod = 6) 
    INSERT INTO PER_Sendikalar (SendikaAdi, LegacyKod, Aktiflik, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi) 
    VALUES (N'Birleşik Büro-İş', 6, 0, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_Sendikalar WHERE LegacyKod = 7) 
    INSERT INTO PER_Sendikalar (SendikaAdi, LegacyKod, Aktiflik, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi) 
    VALUES (N'Tüm Büro-Sen', 7, 0, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_Sendikalar WHERE LegacyKod = 8) 
    INSERT INTO PER_Sendikalar (SendikaAdi, LegacyKod, Aktiflik, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi) 
    VALUES (N'Ufuk Büro Sen', 8, 0, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

IF NOT EXISTS (SELECT 1 FROM PER_Sendikalar WHERE LegacyKod = 9) 
    INSERT INTO PER_Sendikalar (SendikaAdi, LegacyKod, Aktiflik, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi) 
    VALUES (N'Savdes-Sen', 9, 0, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

GO

PRINT '✅ 11 Sendika (0, 1-9, 99) import edildi';
GO
