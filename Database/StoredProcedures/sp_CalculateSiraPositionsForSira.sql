-- ═══════════════════════════════════════════════════════════════════════════════
-- STORED PROCEDURE: sp_CalculateSiraPositionsForSira
-- ═══════════════════════════════════════════════════════════════════════════════
-- AMAÇ:
-- Belirli bir sıranın (yeni alınan veya yönlendirilen) her bir etkilenen personelin
-- panelinde hangi pozisyonda görüneceğini hesaplar.
--
-- KULLANIM SENARYOLARI:
-- 1. Kiosk'tan yeni sıra alındığında - SignalR broadcast için pozisyon hesaplama
-- 2. Sıra yönlendirildiğinde - Hedef personeller için pozisyon hesaplama
--
-- INPUT PARAMETERS:
-- @SiraId INT - Pozisyonu hesaplanacak sıranın ID'si
--
-- OUTPUT:
-- TcKimlikNo NVARCHAR(11) - Personel TC kimlik numarası
-- Position INT           - Sıranın o personelin panelindeki pozisyonu (0-based index)
--
-- NOTES:
-- - GetBankoPanelBekleyenSiralarAsync metodunun SQL karşılığıdır
-- - Personel bazlı sıralama mantığını kullanır:
--   1. Durum önceliği (Çağrıldı=0, Yönlendirildi=1, Beklemede=2)
--   2. Uzmanlık önceliği (Şef=0, Uzman=1, Yrd.Uzman=2) - sadece Beklemede durumu için
--   3. Sıra alış zamanı (ASC)
-- ═══════════════════════════════════════════════════════════════════════════════

CREATE OR ALTER PROCEDURE [dbo].[sp_CalculateSiraPositionsForSira]
    @SiraId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Değişkenler
    DECLARE @KanalAltIslemId INT;
    DECLARE @HizmetBinasiId INT;
    DECLARE @BeklemeDurum TINYINT;
    DECLARE @YonlendirildiMi BIT;
    DECLARE @YonlendirmeTipi TINYINT;
    DECLARE @HedefBankoId INT;
    DECLARE @SiraAlisZamani DATETIME;
    DECLARE @Bugun DATE = CAST(GETDATE() AS DATE);

    -- ═══════════════════════════════════════════════════════════════════
    -- ADIM 1: Sıra bilgilerini al
    -- ═══════════════════════════════════════════════════════════════════
    SELECT
        @KanalAltIslemId = KanalAltIslemId,
        @HizmetBinasiId = HizmetBinasiId,
        @BeklemeDurum = BeklemeDurum,
        @YonlendirildiMi = ISNULL(YonlendirildiMi, 0),
        @YonlendirmeTipi = YonlendirmeTipi,
        @HedefBankoId = HedefBankoId,
        @SiraAlisZamani = SiraAlisZamani
    FROM SIR_Siralar WITH (NOLOCK)
    WHERE SiraId = @SiraId
        AND SilindiMi = 0;

    -- Sıra bulunamadıysa boş sonuç dön
    IF @KanalAltIslemId IS NULL
        RETURN;

    -- ═══════════════════════════════════════════════════════════════════
    -- ADIM 2: Etkilenen personelleri bul
    -- ═══════════════════════════════════════════════════════════════════
    -- Yönlendirme tipine göre doğru personelleri bul
    ;WITH EtkilenenPersoneller AS (
        SELECT DISTINCT kp.TcKimlikNo
        FROM SIR_KanalPersonelleri AS kp WITH (NOLOCK)
        INNER JOIN CMN_Users AS u WITH (NOLOCK)
            ON u.TcKimlikNo = kp.TcKimlikNo
            AND u.BankoModuAktif = 1
            AND u.AktifMi = 1
        LEFT JOIN SIR_BankoKullanicilari AS bk WITH (NOLOCK)
            ON bk.TcKimlikNo = u.TcKimlikNo
            AND bk.SilindiMi = 0
        LEFT JOIN SIR_Bankolar AS b WITH (NOLOCK)
            ON b.BankoId = bk.BankoId
            AND b.HizmetBinasiId = @HizmetBinasiId
            AND b.BankoAktiflik = 1
            AND b.SilindiMi = 0
        WHERE kp.KanalAltIslemId = @KanalAltIslemId
            AND kp.Aktiflik = 1
            AND kp.SilindiMi = 0
            -- Yönlendirme tipine göre filtrele
            AND (
                -- Normal bekleyen sıra veya başka bankoya yönlendirildi ise
                (@BeklemeDurum = 0 OR @YonlendirmeTipi = 1)

                -- Şef'e yönlendirildi ise sadece Şef personeller
                OR (@YonlendirmeTipi = 2 AND kp.Uzmanlik = 3)

                -- Uzman'a yönlendirildi ise sadece Uzman personeller
                OR (@YonlendirmeTipi = 3 AND kp.Uzmanlik = 2)
            )
            -- Başka bankoya yönlendirildi ise sadece hedef bankodaki personel
            AND (
                @YonlendirmeTipi != 1
                OR (@YonlendirmeTipi = 1 AND bk.BankoId = @HedefBankoId)
            )
    ),

    -- ═══════════════════════════════════════════════════════════════════
    -- ADIM 3: Her personel için tüm sıralarını al ve sırala
    -- ═══════════════════════════════════════════════════════════════════
    PersonelSiralari AS (
        SELECT
            ep.TcKimlikNo,
            s.SiraId,
            s.SiraNo,
            s.SiraAlisZamani,
            s.BeklemeDurum,
            kp.Uzmanlik,

            -- Durum Önceliği
            CASE
                WHEN s.BeklemeDurum = 1 THEN 0   -- Çağrıldı
                WHEN s.BeklemeDurum = 3 THEN 1   -- Yönlendirildi
                ELSE 2                            -- Beklemede
            END AS DurumOnceligi,

            -- Uzmanlık Önceliği (Sadece Beklemede durumu için)
            CASE
                WHEN s.BeklemeDurum = 0 THEN
                    CASE kp.Uzmanlik
                        WHEN 3 THEN 0  -- Şef
                        WHEN 2 THEN 1  -- Uzman
                        WHEN 1 THEN 2  -- Yrd. Uzman
                        ELSE 3
                    END
                ELSE 99  -- Çağrıldı/Yönlendirildi için uzmanlık önemsiz
            END AS UzmanlikOnceligi,

            -- Pozisyon hesaplamada kullanmak için sıralama numarası
            ROW_NUMBER() OVER (
                PARTITION BY ep.TcKimlikNo
                ORDER BY
                    CASE
                        WHEN s.BeklemeDurum = 1 THEN 0
                        WHEN s.BeklemeDurum = 3 THEN 1
                        ELSE 2
                    END ASC,
                    CASE
                        WHEN s.BeklemeDurum = 0 THEN
                            CASE kp.Uzmanlik
                                WHEN 3 THEN 0
                                WHEN 2 THEN 1
                                WHEN 1 THEN 2
                                ELSE 3
                            END
                        ELSE 99
                    END ASC,
                    s.SiraAlisZamani ASC
            ) AS SiraNumarasi

        FROM EtkilenenPersoneller AS ep

        -- Personelin kanal alt işlem kayıtları
        INNER JOIN SIR_KanalPersonelleri AS kp WITH (NOLOCK)
            ON kp.TcKimlikNo = ep.TcKimlikNo
            AND kp.Aktiflik = 1
            AND kp.Uzmanlik != 0
            AND kp.SilindiMi = 0

        -- Personelin banko kullanıcı kaydı
        INNER JOIN SIR_BankoKullanicilari AS bk WITH (NOLOCK)
            ON bk.TcKimlikNo = ep.TcKimlikNo
            AND bk.SilindiMi = 0

        -- Banko bilgisi
        INNER JOIN SIR_Bankolar AS b WITH (NOLOCK)
            ON b.BankoId = bk.BankoId
            AND b.BankoAktiflik = 1
            AND b.SilindiMi = 0

        -- Personel kaydı
        INNER JOIN PER_Personeller AS p WITH (NOLOCK)
            ON p.TcKimlikNo = bk.TcKimlikNo
            AND p.PersonelAktiflikDurum = 1
            AND p.HizmetBinasiId = b.HizmetBinasiId
            AND p.SilindiMi = 0

        -- User kaydı
        INNER JOIN CMN_Users AS u WITH (NOLOCK)
            ON u.TcKimlikNo = p.TcKimlikNo
            AND u.BankoModuAktif = 1
            AND u.AktifMi = 1

        -- Hizmet binası
        INNER JOIN CMN_HizmetBinalari AS hb WITH (NOLOCK)
            ON hb.HizmetBinasiId = bk.HizmetBinasiId

        -- Kanal alt işlem
        INNER JOIN SIR_KanalAltIslemleri AS kai WITH (NOLOCK)
            ON kai.KanalAltIslemId = kp.KanalAltIslemId
            AND kai.Aktiflik = 1
            AND kai.SilindiMi = 0

        -- Sıralar
        INNER JOIN SIR_Siralar AS s WITH (NOLOCK)
            ON s.KanalAltIslemId = kai.KanalAltIslemId
            AND s.HizmetBinasiId = bk.HizmetBinasiId
            AND s.SilindiMi = 0
            AND CAST(s.SiraAlisZamani AS DATE) = @Bugun

        WHERE
            -- GetBankoPanelBekleyenSiralarAsync filtreleri
            (
                -- 1. Normal Bekleyen Sıralar
                s.BeklemeDurum = 0

                -- 2. Çağrılmış EN SON Sıra (her personel için kendi çağırdığı)
                OR (s.BeklemeDurum = 1
                    AND s.TcKimlikNo = ep.TcKimlikNo
                    AND s.SiraId = (
                        SELECT TOP 1 s2.SiraId
                        FROM SIR_Siralar s2 WITH (NOLOCK)
                        WHERE s2.TcKimlikNo = ep.TcKimlikNo
                            AND s2.BeklemeDurum = 1
                            AND CAST(s2.SiraAlisZamani AS DATE) = @Bugun
                            AND s2.SilindiMi = 0
                        ORDER BY s2.SiraNo DESC
                    ))

                -- 3. Şef'e Yönlendirilmiş (sadece Şef personeller görsün)
                OR (s.BeklemeDurum = 3
                    AND s.YonlendirildiMi = 1
                    AND s.YonlendirmeTipi = 2
                    AND s.TcKimlikNo != ep.TcKimlikNo
                    AND s.HedefBankoId IS NULL
                    AND EXISTS (
                        SELECT 1
                        FROM SIR_KanalPersonelleri kp2 WITH (NOLOCK)
                        WHERE kp2.TcKimlikNo = ep.TcKimlikNo
                            AND kp2.Aktiflik = 1
                            AND kp2.Uzmanlik = 3  -- Şef
                            AND kp2.SilindiMi = 0
                    ))

                -- 4. Başka Bankoya Yönlendirilmiş (hedef bankodaki personel görsün)
                OR (s.BeklemeDurum = 3
                    AND s.YonlendirildiMi = 1
                    AND s.YonlendirmeTipi = 1
                    AND s.TcKimlikNo != ep.TcKimlikNo
                    AND s.HedefBankoId = bk.BankoId)

                -- 5. Genel Uzmana Yönlendirilmiş (sadece Uzman personeller görsün)
                OR (s.BeklemeDurum = 3
                    AND s.YonlendirildiMi = 1
                    AND s.YonlendirmeTipi = 3
                    AND s.TcKimlikNo != ep.TcKimlikNo
                    AND s.HedefBankoId IS NULL
                    AND EXISTS (
                        SELECT 1
                        FROM SIR_KanalPersonelleri kp2 WITH (NOLOCK)
                        WHERE kp2.TcKimlikNo = ep.TcKimlikNo
                            AND kp2.Aktiflik = 1
                            AND kp2.Uzmanlik = 2  -- Uzman
                            AND kp2.SilindiMi = 0
                    ))
            )
    )

    -- ═══════════════════════════════════════════════════════════════════
    -- ADIM 4: Sadece istenen sıra için pozisyonu hesapla
    -- ═══════════════════════════════════════════════════════════════════
    SELECT DISTINCT
        TcKimlikNo,
        (SiraNumarasi - 1) AS Position  -- 0-based index (frontend için)
    FROM PersonelSiralari
    WHERE SiraId = @SiraId
    ORDER BY TcKimlikNo;

END;
GO
