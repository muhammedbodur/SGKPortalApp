-- =============================================
-- Data Migration: Personel → User
-- Mevcut Personel kayıtları için User oluştur
-- 
-- KULLANIM:
-- Migration uygulandıktan SONRA bu script'i çalıştırın
-- VEYA migration dosyasının Up() metoduna ekleyin
-- =============================================

-- 0. Önce kontrol: User tablosu var mı?
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CMN_Users' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    PRINT '❌ HATA: CMN_Users tablosu bulunamadı! Önce migration''ı uygulayın.';
    RETURN;
END

PRINT '✅ CMN_Users tablosu mevcut. Data migration başlıyor...';
PRINT '';

-- 1. Mevcut personeller için User kayıtları oluştur (SADECE DİNAMİK ALANLAR)
INSERT INTO [dbo].[CMN_Users] 
(
    TcKimlikNo, 
    PassWord, 
    SessionID,
    AktifMi, 
    SonGirisTarihi,
    BasarisizGirisSayisi, 
    HesapKilitTarihi,
    EklenmeTarihi, 
    DuzenlenmeTarihi, 
    SilindiMi,
    EkleyenKullanici,
    DuzenleyenKullanici
)
SELECT 
    p.TcKimlikNo,
    ISNULL(p.PassWord, p.TcKimlikNo) AS PassWord,     -- Eğer NULL ise TC Kimlik No kullan
    p.SessionID,                                       -- Mevcut session'ı kopyala
    CASE 
        WHEN p.PersonelAktiflikDurum = 1 THEN 1        -- Aktif
        ELSE 0                                         -- Pasif
    END AS AktifMi,
    NULL AS SonGirisTarihi,                           -- İlk giriş henüz yapılmadı
    0 AS BasarisizGirisSayisi,
    NULL AS HesapKilitTarihi,
    p.EklenmeTarihi,
    p.DuzenlenmeTarihi,
    p.SilindiMi,
    p.EkleyenKullanici,
    p.DuzenleyenKullanici
FROM [dbo].[PER_Personeller] p
WHERE NOT EXISTS (
    SELECT 1 
    FROM [dbo].[CMN_Users] u 
    WHERE u.TcKimlikNo = p.TcKimlikNo
);

-- 2. Oluşturulan kayıt sayısını göster
DECLARE @CreatedCount INT = @@ROWCOUNT;
PRINT 'Oluşturulan User kayıt sayısı: ' + CAST(@CreatedCount AS VARCHAR(10));

-- 3. Kontrol: Her personelin User kaydı var mı?
PRINT '';
PRINT '📊 KONTROL SONUÇLARI:';
PRINT '─────────────────────────────────────────';

DECLARE @ToplamPersonel INT = (SELECT COUNT(*) FROM [dbo].[PER_Personeller]);
DECLARE @ToplamUser INT = (SELECT COUNT(*) FROM [dbo].[CMN_Users]);
DECLARE @AktifPersonel INT = (SELECT COUNT(*) FROM [dbo].[PER_Personeller] WHERE SilindiMi = 0);
DECLARE @AktifUser INT = (SELECT COUNT(*) FROM [dbo].[CMN_Users] WHERE SilindiMi = 0);

PRINT 'Toplam Personel: ' + CAST(@ToplamPersonel AS VARCHAR(10));
PRINT 'Toplam User    : ' + CAST(@ToplamUser AS VARCHAR(10));
PRINT 'Fark           : ' + CAST(@ToplamPersonel - @ToplamUser AS VARCHAR(10));
PRINT '';
PRINT 'Aktif Personel : ' + CAST(@AktifPersonel AS VARCHAR(10));
PRINT 'Aktif User     : ' + CAST(@AktifUser AS VARCHAR(10));
PRINT '';

-- 4. User kaydı olmayan personelleri listele (olmamalı!)
IF EXISTS (
    SELECT 1 
    FROM [dbo].[PER_Personeller] p
    WHERE NOT EXISTS (
        SELECT 1 
        FROM [dbo].[CMN_Users] u 
        WHERE u.TcKimlikNo = p.TcKimlikNo
    )
)
BEGIN
    PRINT '⚠️  UYARI: User kaydı olmayan personeller bulundu!';
    PRINT '';
    
    SELECT 
        p.TcKimlikNo,
        p.AdSoyad,
        p.Email,
        p.PersonelAktiflikDurum,
        p.SilindiMi
    FROM [dbo].[PER_Personeller] p
    WHERE NOT EXISTS (
        SELECT 1 
        FROM [dbo].[CMN_Users] u 
        WHERE u.TcKimlikNo = p.TcKimlikNo
    );
END
ELSE
BEGIN
    PRINT '✅ Tüm personellerin User kaydı mevcut!';
END

PRINT '';
PRINT '🎉 Data Migration tamamlandı!';
