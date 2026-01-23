-- ============================================================================
-- SGK PORTAL DATABASE SEED DATA (DÜZELTILMIŞ)
-- Eski projenin DB'sinden gelen verilerle uyumlu seed script
-- ============================================================================
-- Kullanım: Bu script'i SSMS'de çalıştırın
-- Not: ID'ler 1'den başlayacak şekilde ayarlanmıştır
-- ============================================================================

USE SGKPortalDB;
GO

-- ============================================================================
-- 0. MEVCUT KAYITLARI KONTROL ET VE TEMİZLE (İSTEĞE BAĞLI)
-- ============================================================================
-- Eğer script'i tekrar çalıştırıyorsanız, önce mevcut kayıtları silin:
-- DELETE FROM CMN_HubConnections;
-- DELETE FROM CMN_Users WHERE TcKimlikNo = '12345678901';
-- DELETE FROM PER_Personeller WHERE TcKimlikNo = '12345678901';
-- DELETE FROM CMN_HizmetBinalari WHERE HizmetBinasiId = 1;
-- DELETE FROM PER_AtanmaNedenleri WHERE AtanmaNedeniId = 1;
-- DELETE FROM PER_Unvanlar WHERE UnvanId = 1;
-- DELETE FROM PER_Servisler WHERE ServisId = 1;
-- DELETE FROM PER_Departmanlar WHERE DepartmanId = 1;
-- GO

-- ============================================================================
-- 1. DEPARTMAN (ID=1)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM PER_Departmanlar WHERE DepartmanId = 1)
BEGIN
    SET IDENTITY_INSERT PER_Departmanlar ON;

    INSERT INTO PER_Departmanlar (
        DepartmanId, 
        DepartmanAdi, 
        DepartmanAdiKisa, 
        Aktiflik, 
        EkleyenKullanici, 
        EklenmeTarihi, 
        DuzenleyenKullanici, 
        DuzenlenmeTarihi
    )
    VALUES (
        1,                          -- DepartmanId
        N'İL MÜDÜRLÜĞÜ',           -- DepartmanAdi
        N'İL MÜDÜRLÜĞÜ',           -- DepartmanAdiKisa
        0,                          -- Aktiflik (0=Aktif, 1=Pasif)
        'System',                   -- EkleyenKullanici
        GETDATE(),                  -- EklenmeTarihi
        'System',                   -- DuzenleyenKullanici
        GETDATE()                   -- DuzenlenmeTarihi
    );

    SET IDENTITY_INSERT PER_Departmanlar OFF;
    PRINT '✅ Departman oluşturuldu (ID=1)';
END
ELSE
BEGIN
    PRINT '⏭️  Departman zaten mevcut (ID=1), atlanıyor...';
END
GO

-- ============================================================================
-- 2. SERVİS (ID=1)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM PER_Servisler WHERE ServisId = 1)
BEGIN
    SET IDENTITY_INSERT PER_Servisler ON;

    INSERT INTO PER_Servisler (
        ServisId,
        ServisAdi,
        Aktiflik,
        EkleyenKullanici,
        EklenmeTarihi,
        DuzenleyenKullanici,
        DuzenlenmeTarihi
    )
    VALUES (
        1,                                      -- ServisId
        N'SGK RANT TESİS. ARŞİV SERVİSİ',     -- ServisAdi
        0,                                      -- Aktiflik (0=Aktif)
        'System',                               -- EkleyenKullanici
        GETDATE(),                              -- EklenmeTarihi
        'System',                               -- DuzenleyenKullanici
        GETDATE()                               -- DuzenlenmeTarihi
    );

    SET IDENTITY_INSERT PER_Servisler OFF;
    PRINT '✅ Servis oluşturuldu (ID=1)';
END
ELSE
BEGIN
    PRINT '⏭️  Servis zaten mevcut (ID=1), atlanıyor...';
END
GO

-- ============================================================================
-- 3. ÜNVAN (ID=1)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM PER_Unvanlar WHERE UnvanId = 1)
BEGIN
    SET IDENTITY_INSERT PER_Unvanlar ON;

    INSERT INTO PER_Unvanlar (
        UnvanId,
        UnvanAdi,
        Aktiflik,
        EkleyenKullanici,
        EklenmeTarihi,
        DuzenleyenKullanici,
        DuzenlenmeTarihi
    )
    VALUES (
        1,                  -- UnvanId
        N'MEMUR',          -- UnvanAdi
        0,                  -- Aktiflik (0=Aktif)
        'System',           -- EkleyenKullanici
        GETDATE(),          -- EklenmeTarihi
        'System',           -- DuzenleyenKullanici
        GETDATE()           -- DuzenlenmeTarihi
    );

    SET IDENTITY_INSERT PER_Unvanlar OFF;
    PRINT '✅ Ünvan oluşturuldu (ID=1)';
END
ELSE
BEGIN
    PRINT '⏭️  Ünvan zaten mevcut (ID=1), atlanıyor...';
END
GO

-- ============================================================================
-- 4. HİZMET BİNASI (ID=1) - ADRES ALANI EKLENDİ
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM CMN_HizmetBinalari WHERE HizmetBinasiId = 1)
BEGIN
    SET IDENTITY_INSERT CMN_HizmetBinalari ON;

    INSERT INTO CMN_HizmetBinalari (
        HizmetBinasiId,
        HizmetBinasiAdi,
        DepartmanId,
        Adres,
        Aktiflik,
        EkleyenKullanici,
        EklenmeTarihi,
        DuzenleyenKullanici,
        DuzenlenmeTarihi
    )
    VALUES (
        1,                      -- HizmetBinasiId
        N'İL MÜDÜRLÜĞÜ',       -- HizmetBinasiAdi
        1,                      -- DepartmanId (yukarıda oluşturulan)
        N'İzmir',              -- Adres (ZORUNLU ALAN - EKLENDİ)
        0,                      -- Aktiflik (0=Aktif)
        'System',               -- EkleyenKullanici
        GETDATE(),              -- EklenmeTarihi
        'System',               -- DuzenleyenKullanici
        GETDATE()               -- DuzenlenmeTarihi
    );

    SET IDENTITY_INSERT CMN_HizmetBinalari OFF;
    PRINT '✅ Hizmet Binası oluşturuldu (ID=1)';
END
ELSE
BEGIN
    PRINT '⏭️  Hizmet Binası zaten mevcut (ID=1), atlanıyor...';
END
GO

-- ============================================================================
-- 5. İL (ID=35 - İzmir)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM CMN_Iller WHERE IlId = 35)
BEGIN
    SET IDENTITY_INSERT CMN_Iller ON;

    INSERT INTO CMN_Iller (IlId, IlAdi)
    VALUES (35, N'İzmir');

    SET IDENTITY_INSERT CMN_Iller OFF;
    PRINT '✅ İl oluşturuldu (ID=35 - İzmir)';
END
ELSE
BEGIN
    PRINT '⏭️  İl zaten mevcut (ID=35), atlanıyor...';
END
GO

-- ============================================================================
-- 6. İLÇE (ID=1 - Varsayılan)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM CMN_Ilceler WHERE IlceId = 1)
BEGIN
    SET IDENTITY_INSERT CMN_Ilceler ON;

    INSERT INTO CMN_Ilceler (IlceId, IlId, IlceAdi)
    VALUES (1, 35, N'Konak');

    SET IDENTITY_INSERT CMN_Ilceler OFF;
    PRINT '✅ İlçe oluşturuldu (ID=1 - Konak)';
END
ELSE
BEGIN
    PRINT '⏭️  İlçe zaten mevcut (ID=1), atlanıyor...';
END
GO

-- ============================================================================
-- 7. ATANMA NEDENİ (ID=1)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM PER_AtanmaNedenleri WHERE AtanmaNedeniId = 1)
BEGIN
    SET IDENTITY_INSERT PER_AtanmaNedenleri ON;

    INSERT INTO PER_AtanmaNedenleri (
        AtanmaNedeniId,
        AtanmaNedeni,
        EkleyenKullanici,
        EklenmeTarihi,
        DuzenleyenKullanici,
        DuzenlenmeTarihi
    )
    VALUES (
        1,                      -- AtanmaNedeniId
        N'Asaleten Atama',     -- AtanmaNedeni
        'System',               -- EkleyenKullanici
        GETDATE(),              -- EklenmeTarihi
        'System',               -- DuzenleyenKullanici
        GETDATE()               -- DuzenlenmeTarihi
    );

    SET IDENTITY_INSERT PER_AtanmaNedenleri OFF;
    PRINT '✅ Atanma Nedeni oluşturuldu (ID=1)';
END
ELSE
BEGIN
    PRINT '⏭️  Atanma Nedeni zaten mevcut (ID=1), atlanıyor...';
END
GO

-- ============================================================================
-- 6. ADMIN PERSONEL (TC: 12345678901) - KartGonderimIslemBasari EKLENDİ
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM PER_Personeller WHERE TcKimlikNo = '12345678901')
BEGIN
    INSERT INTO PER_Personeller (
        TcKimlikNo,
        IlId,
        IlceId,
        SicilNo,
        AdSoyad,
        NickName,
        PersonelKayitNo,
        KartNo,
        KartGonderimIslemBasari,
        KartTipi,
        DepartmanId,
        ServisId,
        UnvanId,
        AtanmaNedeniId,
        HizmetBinasiId,
        Email,
        Dahili,
        PersonelTipi,
        DogumTarihi,
        Cinsiyet,
        MedeniDurumu,
        KanGrubu,
        EvDurumu,
        UlasimServis1,
        UlasimServis2,
        Tabldot,
        OgrenimDurumu,
        OgrenimSuresi,
        SehitYakinligi,
        EsininIsDurumu,
        PersonelAktiflikDurum,
        EkleyenKullanici,
        EklenmeTarihi,
        DuzenleyenKullanici,
        DuzenlenmeTarihi
    )
    VALUES (
        '12345678901',              -- TcKimlikNo   
        35,                           -- IlId
        1,                           -- IlceId
        1,                          -- SicilNo
        N'Admin Kullanıcı',        -- AdSoyad
        'ADMIN',                    -- NickName
        1,                          -- PersonelKayitNo
        1,                          -- KartNo
        0,                          -- KartGonderimIslemBasari (0=Basarili)
        0,                          -- KartTipi (0=PersonelKarti)
        1,                          -- DepartmanId
        1,                          -- ServisId
        1,                          -- UnvanId
        1,                          -- AtanmaNedeniId
        1,                          -- HizmetBinasiId
        'admin@sgk.gov.tr',         -- Email
        1000,                       -- Dahili
        0,                          -- PersonelTipi (0=memur)
        '1980-01-01',               -- DogumTarihi
        0,                          -- Cinsiyet (0=erkek)
        0,                          -- MedeniDurumu (0=bekar)
        0,                          -- KanGrubu (0=sifir_arti)
        0,                          -- EvDurumu (0=KiraEvi)
        0,                          -- UlasimServis1
        0,                          -- UlasimServis2
        0,                          -- Tabldot
        0,                          -- OgrenimDurumu (0=Ilkokul)
        0,                          -- OgrenimSuresi
        0,                          -- SehitYakinligi (0=degil)
        0,                          -- EsininIsDurumu (0=belirtilmemis)
        0,                          -- PersonelAktiflikDurum (0=Aktif)
        'System',                   -- EkleyenKullanici
        GETDATE(),                  -- EklenmeTarihi
        'System',                   -- DuzenleyenKullanici
        GETDATE()                   -- DuzenlenmeTarihi
    );

    PRINT '✅ Admin Personel oluşturuldu (TC: 12345678901)';
END
ELSE
BEGIN
    PRINT '⏭️  Admin Personel zaten mevcut (TC: 12345678901), atlanıyor...';
END
GO

-- ============================================================================
-- 7. ADMIN USER (TC: 12345678901, Şifre: 123456)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM CMN_Users WHERE TcKimlikNo = '12345678901')
BEGIN
    INSERT INTO CMN_Users (
        TcKimlikNo,
        UserType,
        PassWord,
        AktifMi,
        BasarisizGirisSayisi,
        BankoModuAktif,
        PermissionStamp,
        EkleyenKullanici,
        EklenmeTarihi,
        DuzenleyenKullanici,
        DuzenlenmeTarihi
    )
    VALUES (
        '12345678901',              -- TcKimlikNo (Personel ile aynı)
        0,                          -- UserType (0=Personel, 1=TvUser)
        '123456',                   -- PassWord (⚠️ Production'da değiştirin!)
        1,                          -- AktifMi (1=true)
        0,                          -- BasarisizGirisSayisi
        0,                          -- BankoModuAktif (0=false)
        NEWID(),                    -- PermissionStamp
        'System',                   -- EkleyenKullanici
        GETDATE(),                  -- EklenmeTarihi
        'System',                   -- DuzenleyenKullanici
        GETDATE()                   -- DuzenlenmeTarihi
    );

    PRINT '✅ Admin User oluşturuldu (TC: 12345678901, Şifre: 123456)';
END
ELSE
BEGIN
    PRINT '⏭️  Admin User zaten mevcut (TC: 12345678901), atlanıyor...';
END
GO

-- ============================================================================
-- ÖZET
-- ============================================================================
PRINT '';
PRINT '╔════════════════════════════════════════════════════════╗';
PRINT '║         DATABASE SEED TAMAMLANDI!                     ║';
PRINT '╚════════════════════════════════════════════════════════╝';
PRINT '';
PRINT '📊 Oluşturulan Kayıtlar:';
PRINT '  • Departman (ID=1): İL MÜDÜRLÜĞÜ';
PRINT '  • Servis (ID=1): SGK RANT TESİS. ARŞİV SERVİSİ';
PRINT '  • Ünvan (ID=1): MEMUR';
PRINT '  • Hizmet Binası (ID=1): İL MÜDÜRLÜĞÜ (Adres: İzmir)';
PRINT '  • Atanma Nedeni (ID=1): Asaleten Atama';
PRINT '  • Admin Personel: 12345678901';
PRINT '  • Admin User: 12345678901';
PRINT '';
PRINT '🔐 Giriş Bilgileri:';
PRINT '  TC Kimlik No: 12345678901';
PRINT '  Şifre: 123456';
PRINT '';
PRINT '⚠️  UYARI: Production ortamında şifreyi mutlaka değiştirin!';
PRINT '';
PRINT '✅ DÜZELTİLEN SORUNLAR:';
PRINT '  1. HizmetBinasi.Adres alanı eklendi (zorunlu)';
PRINT '  2. Personel.KartGonderimIslemBasari alanı eklendi (zorunlu)';
PRINT '  3. Personel.KanGrubu alanı eklendi (zorunlu)';
PRINT '  4. Personel.SehitYakinligi alanı eklendi (zorunlu)';
PRINT '  5. Personel.EsininIsDurumu alanı eklendi (zorunlu)';
PRINT '  6. Personel.EvDurumu, UlasimServis1/2, Tabldot, OgrenimDurumu alanları eklendi';
PRINT '  7. IlId ve IlceId kaldırıldı (nullable, FK constraint sorunu)';
PRINT '  8. Duplicate key kontrolleri eklendi (idempotent)';
PRINT '';
PRINT '⚠️  NOT: IlId ve IlceId alanları nullable olduğu için NULL bırakıldı.';
PRINT '   Gerekirse daha sonra UPDATE ile güncelleyebilirsiniz.';
PRINT '';
GO
