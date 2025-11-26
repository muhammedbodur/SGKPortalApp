-- ═══════════════════════════════════════════════════════════════════════════════
-- BankoKullanici Tablosuna HizmetBinasiId Ekleme Migration
-- ═══════════════════════════════════════════════════════════════════════════════
-- Tarih: 2025-11-26
-- Amaç: Personel, Banko ve HizmetBinasi arasındaki tutarlılığı database seviyesinde garanti etmek
-- ═══════════════════════════════════════════════════════════════════════════════

BEGIN TRANSACTION;

-- ═══════════════════════════════════════════════════════════════════════════════
-- ADIM 1: Tutarsız Verileri Tespit Et ve Logla
-- ═══════════════════════════════════════════════════════════════════════════════
PRINT '🔍 Tutarsız BankoKullanici kayıtları kontrol ediliyor...';

SELECT 
    bk.BankoKullaniciId,
    bk.TcKimlikNo,
    p.AdSoyad AS PersonelAdi,
    p.HizmetBinasiId AS PersonelHizmetBinasiId,
    ph.HizmetBinasiAdi AS PersonelHizmetBinasiAdi,
    bk.BankoId,
    b.HizmetBinasiId AS BankoHizmetBinasiId,
    bh.HizmetBinasiAdi AS BankoHizmetBinasiAdi,
    CASE 
        WHEN p.HizmetBinasiId = b.HizmetBinasiId THEN 'TUTARLI ✅'
        ELSE 'TUTARSIZ ❌'
    END AS Durum
FROM dbo.SIR_BankoKullanicilari bk
INNER JOIN dbo.PER_Personeller p ON bk.TcKimlikNo = p.TcKimlikNo
INNER JOIN dbo.SIR_Bankolar b ON bk.BankoId = b.BankoId
INNER JOIN dbo.CMN_HizmetBinalari ph ON p.HizmetBinasiId = ph.HizmetBinasiId
INNER JOIN dbo.CMN_HizmetBinalari bh ON b.HizmetBinasiId = bh.HizmetBinasiId
WHERE bk.SilindiMi = 0
ORDER BY 
    CASE WHEN p.HizmetBinasiId = b.HizmetBinasiId THEN 1 ELSE 0 END,
    bk.BankoKullaniciId;

DECLARE @TutarsizKayitSayisi INT;
SELECT @TutarsizKayitSayisi = COUNT(*)
FROM dbo.SIR_BankoKullanicilari bk
INNER JOIN dbo.PER_Personeller p ON bk.TcKimlikNo = p.TcKimlikNo
INNER JOIN dbo.SIR_Bankolar b ON bk.BankoId = b.BankoId
WHERE bk.SilindiMi = 0 AND p.HizmetBinasiId != b.HizmetBinasiId;

PRINT '⚠️ Tutarsız kayıt sayısı: ' + CAST(@TutarsizKayitSayisi AS VARCHAR(10));

-- ═══════════════════════════════════════════════════════════════════════════════
-- ADIM 2: Tutarsız Kayıtları Sil (Soft Delete)
-- ═══════════════════════════════════════════════════════════════════════════════
PRINT '🗑️ Tutarsız kayıtlar siliniyor...';

UPDATE bk
SET 
    bk.SilindiMi = 1,
    bk.SilinmeTarihi = GETDATE()
FROM dbo.SIR_BankoKullanicilari bk
INNER JOIN dbo.PER_Personeller p ON bk.TcKimlikNo = p.TcKimlikNo
INNER JOIN dbo.SIR_Bankolar b ON bk.BankoId = b.BankoId
WHERE bk.SilindiMi = 0 AND p.HizmetBinasiId != b.HizmetBinasiId;

PRINT '✅ ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' tutarsız kayıt silindi.';

-- ═══════════════════════════════════════════════════════════════════════════════
-- ADIM 3: Eski Index'leri Kaldır
-- ═══════════════════════════════════════════════════════════════════════════════
PRINT '🔧 Eski index''ler kaldırılıyor...';

-- Eski TcKimlikNo unique index'i kaldır
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SIR_BankoKullanicilari_TcKimlikNo' AND object_id = OBJECT_ID('dbo.SIR_BankoKullanicilari'))
BEGIN
    DROP INDEX IX_SIR_BankoKullanicilari_TcKimlikNo ON dbo.SIR_BankoKullanicilari;
    PRINT '✅ IX_SIR_BankoKullanicilari_TcKimlikNo kaldırıldı';
END

-- BankoId unique index'ini non-unique yap
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SIR_BankoKullanicilari_BankoId' AND object_id = OBJECT_ID('dbo.SIR_BankoKullanicilari'))
BEGIN
    DROP INDEX IX_SIR_BankoKullanicilari_BankoId ON dbo.SIR_BankoKullanicilari;
    PRINT '✅ IX_SIR_BankoKullanicilari_BankoId kaldırıldı (yeniden oluşturulacak)';
END

-- ═══════════════════════════════════════════════════════════════════════════════
-- ADIM 4: HizmetBinasiId Kolonu Ekle (NULL olarak)
-- ═══════════════════════════════════════════════════════════════════════════════
PRINT '➕ HizmetBinasiId kolonu ekleniyor (NULL)...';

ALTER TABLE dbo.SIR_BankoKullanicilari
ADD HizmetBinasiId INT NULL;

PRINT '✅ HizmetBinasiId kolonu eklendi';

-- ═══════════════════════════════════════════════════════════════════════════════
-- ADIM 5: Mevcut Verileri Doldur (Banko'nun HizmetBinasiId'sini al)
-- ═══════════════════════════════════════════════════════════════════════════════
PRINT '🔄 Mevcut veriler dolduruluyor...';

UPDATE bk
SET bk.HizmetBinasiId = b.HizmetBinasiId
FROM dbo.SIR_BankoKullanicilari bk
INNER JOIN dbo.SIR_Bankolar b ON bk.BankoId = b.BankoId
WHERE bk.HizmetBinasiId IS NULL;

PRINT '✅ ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' kayıt güncellendi';

-- HizmetBinasiId'yi NOT NULL yap
PRINT '🔧 HizmetBinasiId NOT NULL yapılıyor...';

ALTER TABLE dbo.SIR_BankoKullanicilari
ALTER COLUMN HizmetBinasiId INT NOT NULL;

PRINT '✅ HizmetBinasiId NOT NULL yapıldı';

-- ═══════════════════════════════════════════════════════════════════════════════
-- ADIM 6: Foreign Key Ekle
-- ═══════════════════════════════════════════════════════════════════════════════
PRINT '🔗 Foreign Key ekleniyor...';

ALTER TABLE dbo.SIR_BankoKullanicilari
ADD CONSTRAINT FK_SIR_BankoKullanicilari_CMN_HizmetBinalari
FOREIGN KEY (HizmetBinasiId) REFERENCES dbo.CMN_HizmetBinalari(HizmetBinasiId)
ON DELETE NO ACTION;

PRINT '✅ Foreign Key eklendi';

-- ═══════════════════════════════════════════════════════════════════════════════
-- ADIM 7: Yeni Index'leri Oluştur
-- ═══════════════════════════════════════════════════════════════════════════════
PRINT '📊 Yeni index''ler oluşturuluyor...';

-- ⭐ BankoId index (UNIQUE - Bir banko sadece bir personele atanabilir)
CREATE UNIQUE NONCLUSTERED INDEX IX_SIR_BankoKullanicilari_BankoId
ON dbo.SIR_BankoKullanicilari(BankoId)
WHERE SilindiMi = 0;

PRINT '✅ IX_SIR_BankoKullanicilari_BankoId oluşturuldu (UNIQUE - 1-to-1 ilişki)';

-- Composite unique index: TcKimlikNo + HizmetBinasiId
CREATE UNIQUE NONCLUSTERED INDEX IX_SIR_BankoKullanicilari_TcKimlik_HizmetBinasi
ON dbo.SIR_BankoKullanicilari(TcKimlikNo, HizmetBinasiId)
WHERE SilindiMi = 0;

PRINT '✅ IX_SIR_BankoKullanicilari_TcKimlik_HizmetBinasi oluşturuldu';

-- ═══════════════════════════════════════════════════════════════════════════════
-- ADIM 8: Doğrulama
-- ═══════════════════════════════════════════════════════════════════════════════
PRINT '🔍 Doğrulama yapılıyor...';

-- Tüm kayıtların tutarlı olduğunu kontrol et
DECLARE @TutarsizKayit INT;
SELECT @TutarsizKayit = COUNT(*)
FROM dbo.SIR_BankoKullanicilari bk
INNER JOIN dbo.PER_Personeller p ON bk.TcKimlikNo = p.TcKimlikNo
INNER JOIN dbo.SIR_Bankolar b ON bk.BankoId = b.BankoId
WHERE bk.SilindiMi = 0 
  AND (bk.HizmetBinasiId != p.HizmetBinasiId OR bk.HizmetBinasiId != b.HizmetBinasiId);

IF @TutarsizKayit > 0
BEGIN
    PRINT '❌ HATA: ' + CAST(@TutarsizKayit AS VARCHAR(10)) + ' tutarsız kayıt bulundu!';
    ROLLBACK TRANSACTION;
    RETURN;
END

PRINT '✅ Tüm kayıtlar tutarlı';

-- ═══════════════════════════════════════════════════════════════════════════════
-- ADIM 9: Özet Rapor
-- ═══════════════════════════════════════════════════════════════════════════════
PRINT '';
PRINT '═══════════════════════════════════════════════════════════════════════════════';
PRINT '✅ Migration Başarıyla Tamamlandı!';
PRINT '═══════════════════════════════════════════════════════════════════════════════';
PRINT '';

SELECT 
    COUNT(*) AS ToplamKayit,
    COUNT(CASE WHEN SilindiMi = 0 THEN 1 END) AS AktifKayit,
    COUNT(CASE WHEN SilindiMi = 1 THEN 1 END) AS SilinenKayit
FROM dbo.SIR_BankoKullanicilari;

PRINT '';
PRINT '📊 Hizmet Binası Bazında Dağılım:';
SELECT 
    hb.HizmetBinasiAdi,
    COUNT(*) AS KayitSayisi
FROM dbo.SIR_BankoKullanicilari bk
INNER JOIN dbo.CMN_HizmetBinalari hb ON bk.HizmetBinasiId = hb.HizmetBinasiId
WHERE bk.SilindiMi = 0
GROUP BY hb.HizmetBinasiAdi
ORDER BY COUNT(*) DESC;

COMMIT TRANSACTION;

PRINT '';
PRINT '🎉 İşlem tamamlandı!';
PRINT '';
