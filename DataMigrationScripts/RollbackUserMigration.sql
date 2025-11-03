-- =============================================
-- ROLLBACK Script: User → Personel
-- User tablosundaki verileri Personel'e geri kopyala
-- 
-- UYARI: Bu script'i sadece migration geri alınacaksa kullanın!
-- =============================================

PRINT '⚠️  ROLLBACK işlemi başlıyor...';
PRINT '';

-- 1. Önce kontrol: User tablosu var mı?
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CMN_Users' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    PRINT '❌ HATA: CMN_Users tablosu bulunamadı! Rollback yapılacak bir şey yok.';
    RETURN;
END

-- 2. PassWord ve SessionID kolonları Personel'de var mı kontrol et
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.PER_Personeller') AND name = 'PassWord')
BEGIN
    PRINT '⚠️  PassWord kolonu Personel tablosunda yok. Ekleniyor...';
    
    ALTER TABLE [dbo].[PER_Personeller]
    ADD PassWord NVARCHAR(255) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.PER_Personeller') AND name = 'SessionID')
BEGIN
    PRINT '⚠️  SessionID kolonu Personel tablosunda yok. Ekleniyor...';
    
    ALTER TABLE [dbo].[PER_Personeller]
    ADD SessionID NVARCHAR(100) NULL;
END

PRINT '';
PRINT '📋 Kolonlar hazır. Veri kopyalama başlıyor...';
PRINT '';

-- 3. User tablosundaki verileri Personel'e geri kopyala
UPDATE p
SET 
    p.PassWord = u.PassWord,
    p.SessionID = u.SessionID
FROM [dbo].[PER_Personeller] p
INNER JOIN [dbo].[CMN_Users] u ON p.TcKimlikNo = u.TcKimlikNo;

DECLARE @UpdatedCount INT = @@ROWCOUNT;
PRINT 'Güncellenen Personel kayıt sayısı: ' + CAST(@UpdatedCount AS VARCHAR(10));

-- 4. Kontrol: Tüm personellerin verileri kopyalandı mı?
DECLARE @MissingData INT = (
    SELECT COUNT(*)
    FROM [dbo].[PER_Personeller] p
    LEFT JOIN [dbo].[CMN_Users] u ON p.TcKimlikNo = u.TcKimlikNo
    WHERE u.TcKimlikNo IS NULL
);

IF @MissingData > 0
BEGIN
    PRINT '';
    PRINT '⚠️  UYARI: ' + CAST(@MissingData AS VARCHAR(10)) + ' personelin User kaydı bulunamadı!';
    
    SELECT 
        p.TcKimlikNo,
        p.AdSoyad,
        p.Email
    FROM [dbo].[PER_Personeller] p
    LEFT JOIN [dbo].[CMN_Users] u ON p.TcKimlikNo = u.TcKimlikNo
    WHERE u.TcKimlikNo IS NULL;
END
ELSE
BEGIN
    PRINT '✅ Tüm personellerin verileri başarıyla kopyalandı!';
END

PRINT '';
PRINT '🎉 Rollback tamamlandı!';
PRINT '';
PRINT '⚠️  NOT: User tablosunu silmek için migration''ı geri alın:';
PRINT '   dotnet ef database update <PreviousMigrationName>';
