-- ============================================================================
-- PERSONEL VE USER IMPORT - ÖRNEK SATIR
-- kullanici.sql'den örnek dönüşüm
-- ============================================================================
USE SGKPortalDB;
GO

PRINT '📋 Örnek Personel ve User import...';
GO

-- ============================================================================
-- ÖRNEK KAYIT: ID=2, TC=63043044554, BANU İŞBAŞARAN
-- ============================================================================
-- Kaynak: INSERT INTO `kullanici` VALUES (2, 10469589664, '2026-01-16 13:57:49', 63043044554, '19811981', 'BANU İŞBAŞARAN', 402590, '87', 'İZMİR İL MÜDÜRLÜĞÜ', 4, 18, '25', 'İZMİR İL MÜDÜRLÜĞÜ', 5457, 'bisbasaran@sgk.gov.tr', '', 5380128765, 0, 0, '', '', 0, 115, 115, '1787 SK.NO.20/3 BOSTANLI KARŞIYAKA', 'BOSTANLI', 'KARŞIYAKA', 'İZMİR', '', 'CAMİ DURAĞI', 1, '1981-09-29', 6, 2, '1', '0RH(+)', '1787 SK.NO.20/3 BOSTANLI K.YAKA', '', '', '', 'İZMİR', 'AKIN', 1, '81.280.189', '', '', '', '', 0, '6', 'MEMUR', 0, '', '', '', '', '', 2, 0, '2026-01-16 09:26:00', 2, 0, 0, 0, 2, 0, 0, 0, 0, 0, 'd.m.Y', 0, 12, 'TR740001500158007296979877', '', '6839952', NULL, '2023-01-01', 0, 0, 10, '2', 't');

-- ALAN AÇIKLAMALARI:
-- username (4. alan) = 63043044554 → TcKimlikNo
-- kullaniciadi (6. alan) = 'BANU İŞBAŞARAN' → AdSoyad
-- sicilno (7. alan) = 402590 → SicilNo
-- birim (10. alan) = 4 → DepartmanId
-- bina (11. alan) = 18 → HizmetBinasiId (ama bizde 4 numaralı departman için HizmetBinasiId=4 olmalı)
-- servis (12. alan) = '25' → ServisId (string, parse edilmeli)
-- email (15. alan) = 'bisbasaran@sgk.gov.tr' → Email
-- ceptel (17. alan) = 5380128765 → CepTelefonu
-- dahili (19. alan) = 5457 → DahiliTelefon (0 ise NULL)
-- adres (25. alan) = '1787 SK.NO.20/3 BOSTANLI KARŞIYAKA' → Adres
-- ilce (28. alan) = 'KARŞIYAKA' → IlceAdi (İlce tablosundan ID bulunmalı)
-- il (29. alan) = 'İZMİR' → IlAdi (İl tablosundan ID bulunmalı)
-- meddur (32. alan) = 1 → MedeniDurumu (1=Evli, 0=Bekar)
-- dogumtarihi (33. alan) = '1981-09-29' → DogumTarihi
-- esdurumu (34. alan) = 6 → EsininIsDurumu (enum mapping gerekli)
-- evdurum (35. alan) = 2 → EvDurumu (0=Diger, 1=Kiraci, 2=Evsahibi)
-- cinsiyet (36. alan) = '1' → Cinsiyet ('0'=Erkek, '1'=Kadin)
-- kan (37. alan) = '0RH(+)' → KanGrubu (enum mapping gerekli)
-- calisandurum (68. alan) = 1 → PersonelAktiflikDurum (1=Aktif, 2=Pasif, 3=Emekli)
-- tabldot (98. alan) = 12 → Tabldot
-- kart_no (101. alan) = '6839952' → KartNo
-- password (5. alan) = '19811981' → User.PassWord

-- ============================================================================
-- 1. PERSONEL INSERT
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM PER_Personeller WHERE TcKimlikNo = '63043044554')
BEGIN
    DECLARE @IlId INT = (SELECT TOP 1 IlId FROM CMN_Iller WHERE IlAdi = N'İZMİR');
    DECLARE @IlceId INT = (SELECT TOP 1 IlceId FROM CMN_Ilceler WHERE IlceAdi = N'KARŞIYAKA' AND IlId = @IlId);
    DECLARE @DepartmanId INT = 4;
    DECLARE @HizmetBinasiId INT = 4; -- Departman ID ile eşleşmeli
    DECLARE @ServisId INT = 25;
    DECLARE @UnvanId INT = 87;
    
    -- Eğer İl/İlçe bulunamazsa default değerler (İzmir)
    IF @IlId IS NULL SET @IlId = 35;
    IF @IlceId IS NULL SET @IlceId = (SELECT TOP 1 IlceId FROM CMN_Ilceler WHERE IlId = @IlId);
    
    INSERT INTO PER_Personeller (
        TcKimlikNo, SicilNo, AdSoyad, NickName, PersonelKayitNo,
        KartNo, KartNoAktiflikTarihi, KartNoDuzenlenmeTarihi, KartTipi, KartGonderimIslemBasari,
        DepartmanId, ServisId, UnvanId, AtanmaNedeniId, HizmetBinasiId,
        IlId, IlceId, SendikaId,
        Gorev, PersonelTipi, Email, Dahili, CepTelefonu, CepTelefonu2, EvTelefonu,
        Adres, Semt, DogumTarihi, Cinsiyet, MedeniDurumu, KanGrubu, EvDurumu,
        UlasimServis1, UlasimServis2, Tabldot, PersonelAktiflikDurum,
        EmekliSicilNo, OgrenimDurumu, BitirdigiOkul, BitirdigiBolum, OgrenimSuresi, Bransi,
        SehitYakinligi, EsininAdi, EsininIsDurumu, EsininUnvani, EsininIsAdresi, EsininIsSemt,
        EsininIsIlId, EsininIsIlceId, Resim,
        Aktiflik, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi
    )
    VALUES (
        '63043044554', 402590, N'BANU İŞBAŞARAN', N'BANU', 0,
        6839952, '2023-01-01', NULL, 0, 0,
        @DepartmanId, @ServisId, @UnvanId, 1, @HizmetBinasiId,
        @IlId, @IlceId, 1,
        N'İZMİR İL MÜDÜRLÜĞÜ', 0, 'bisbasaran@sgk.gov.tr', 5457, '5380128765', NULL, NULL,
        N'1787 SK.NO.20/3 BOSTANLI KARŞIYAKA', N'BOSTANLI', '1981-09-29', 1, 1, 0, 1,
        115, 115, 12, 0,
        NULL, 0, NULL, NULL, 0, NULL,
        0, N'AKIN', 5, NULL, N'1787 SK.NO.20/3 BOSTANLI K.YAKA', NULL,
        @IlId, @IlceId, NULL,
        0, 'IMPORT', GETDATE(), 'IMPORT', GETDATE()
    );
    
    PRINT '  ✅ Personel eklendi: 63043044554 - BANU İŞBAŞARAN';
END
ELSE
BEGIN
    PRINT '  ⏭️  Personel zaten mevcut: 63043044554';
END
GO

-- ============================================================================
-- 2. USER INSERT
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM CMN_Users WHERE TcKimlikNo = '63043044554')
BEGIN
    INSERT INTO CMN_Users (
        TcKimlikNo,
        UserType,
        PassWord,
        AktifMi,
        EkleyenKullanici,
        EklenmeTarihi,
        DuzenleyenKullanici,
        DuzenlenmeTarihi
    )
    VALUES (
        '63043044554',                      -- TcKimlikNo
        0,                                  -- UserType (0=Personel)
        '19811981',                         -- PassWord (kullanici.password)
        1,                                  -- AktifMi (1=Aktif)
        'IMPORT',                           -- EkleyenKullanici
        GETDATE(),                          -- EklenmeTarihi
        'IMPORT',                           -- DuzenleyenKullanici
        GETDATE()                           -- DuzenlenmeTarihi
    );
    
    PRINT '  ✅ User eklendi: 63043044554';
END
ELSE
BEGIN
    PRINT '  ⏭️  User zaten mevcut: 63043044554';
END
GO

PRINT '✅ Örnek import tamamlandı';
GO

-- ============================================================================
-- ENUM MAPPING NOTLARI:
-- ============================================================================
-- Cinsiyet: '0'=Erkek(0), '1'=Kadin(1)
-- MedeniDurumu: 0=Bekar(0), 1=Evli(1)
-- KanGrubu: '0'=sifir_arti(0), 'ARH(+)'=a_arti(1), 'BRH(+)'=b_arti(3), vb.
-- PersonelAktiflikDurum: calisandurum 1=Aktif(0), 2=Pasif(1), 3=Emekli(2)
-- EvDurumu: 0=Diger(0), 1=Kiraci(1), 2=Evsahibi(2)
-- EsininIsDurumu: 1=Emekli(0), 2=Bekar(1), 3=Evhanimi(2), 4=OzelCalisiyor(3), 5=KamuCalisiyor(4), 6=KamuCalisiyor(5)
-- SehitYakinligi: 0=Degil(0), 1=Evet(1)
-- OgrenimDurumu: default=Ilkokul(0)
-- PersonelTipi: default=Memur(0)
-- KartTipi: default=PersonelKarti(0)
