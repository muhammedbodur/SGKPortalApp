-- ============================================================================
-- SENDİKALAR IMPORT
-- Personel için opsiyonel sendika bilgisi
-- ============================================================================
USE SGKPortalDB;
GO

PRINT '📦 Sendikalar import ediliyor...';
GO

SET IDENTITY_INSERT PER_Sendikalar ON;
GO

-- Default sendika (Sendikasız personeller için)
IF NOT EXISTS (SELECT 1 FROM PER_Sendikalar WHERE SendikaId = 1) INSERT INTO PER_Sendikalar (SendikaId, SendikaAdi, Aktiflik, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi) VALUES (1, N'-SENDİKASIZ-', 0, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());

-- Yaygın sendikalar
IF NOT EXISTS (SELECT 1 FROM PER_Sendikalar WHERE SendikaId = 2) INSERT INTO PER_Sendikalar (SendikaId, SendikaAdi, Aktiflik, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi) VALUES (2, N'TÜRK SAĞLIK SEN', 0, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());
IF NOT EXISTS (SELECT 1 FROM PER_Sendikalar WHERE SendikaId = 3) INSERT INTO PER_Sendikalar (SendikaId, SendikaAdi, Aktiflik, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi) VALUES (3, N'MEMUR SEN', 0, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());
IF NOT EXISTS (SELECT 1 FROM PER_Sendikalar WHERE SendikaId = 4) INSERT INTO PER_Sendikalar (SendikaId, SendikaAdi, Aktiflik, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi) VALUES (4, N'KAMU SEN', 0, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());
IF NOT EXISTS (SELECT 1 FROM PER_Sendikalar WHERE SendikaId = 5) INSERT INTO PER_Sendikalar (SendikaId, SendikaAdi, Aktiflik, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi) VALUES (5, N'HAK İŞ', 0, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());
IF NOT EXISTS (SELECT 1 FROM PER_Sendikalar WHERE SendikaId = 6) INSERT INTO PER_Sendikalar (SendikaId, SendikaAdi, Aktiflik, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi) VALUES (6, N'BİRLEŞİK KAMU İŞ', 0, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());
GO

SET IDENTITY_INSERT PER_Sendikalar OFF;
GO

PRINT '✅ 6 Sendika import edildi';
GO
