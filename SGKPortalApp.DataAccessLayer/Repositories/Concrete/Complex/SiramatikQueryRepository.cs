using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Complex;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.Complex
{
    public class SiramatikQueryRepository : ISiramatikQueryRepository
    {
        private readonly SGKDbContext _context;

        public SiramatikQueryRepository(SGKDbContext context)
        {
            _context = context;
        }

        // ═══════════════════════════════════════════════════════
        // KANAL ALT İŞLEM SORGULARI
        // ═══════════════════════════════════════════════════════

        public async Task<List<KanalAltIslemResponseDto>> GetAllKanalAltIslemlerAsync()
        {
            var query = from kai in _context.KanalAltIslemleri
                        join ka in _context.KanallarAlt on kai.KanalAltId equals ka.KanalAltId
                        join k in _context.Kanallar on ka.KanalId equals k.KanalId
                        join ki in _context.KanalIslemleri on kai.KanalIslemId equals ki.KanalIslemId
                        select new KanalAltIslemResponseDto
                        {
                            KanalAltIslemId = kai.KanalAltIslemId,
                            KanalAltId = kai.KanalAltId,
                            KanalAltAdi = ka.KanalAltAdi,
                            KanalIslemId = kai.KanalIslemId,
                            KanalAdi = k.KanalAdi,
                            Aktiflik = kai.Aktiflik,
                            EklenmeTarihi = kai.EklenmeTarihi,
                            DuzenlenmeTarihi = kai.DuzenlenmeTarihi,
                            PersonelSayisi = _context.KanalPersonelleri
                                .Count(kp => kp.KanalAltIslemId == kai.KanalAltIslemId && kp.Aktiflik == BusinessObjectLayer.Enums.Common.Aktiflik.Aktif)
                        };

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<List<KanalAltIslemResponseDto>> GetKanalAltIslemlerByHizmetBinasiIdAsync(int hizmetBinasiId)
        {
            var query = from kai in _context.KanalAltIslemleri
                        join ka in _context.KanallarAlt on kai.KanalAltId equals ka.KanalAltId
                        join k in _context.Kanallar on ka.KanalId equals k.KanalId
                        join ki in _context.KanalIslemleri on kai.KanalIslemId equals ki.KanalIslemId
                        join hb in _context.HizmetBinalari on kai.HizmetBinasiId equals hb.HizmetBinasiId
                        where kai.HizmetBinasiId == hizmetBinasiId
                        select new KanalAltIslemResponseDto
                        {
                            KanalAltIslemId = kai.KanalAltIslemId,
                            KanalAltId = kai.KanalAltId,
                            KanalAltAdi = ka.KanalAltAdi,
                            KanalIslemId = kai.KanalIslemId,
                            HizmetBinasiId = hb.HizmetBinasiId,
                            HizmetBinasiAdi = hb.HizmetBinasiAdi,
                            KanalAdi = k.KanalAdi,
                            Aktiflik = kai.Aktiflik,
                            EklenmeTarihi = kai.EklenmeTarihi,
                            DuzenlenmeTarihi = kai.DuzenlenmeTarihi,
                            PersonelSayisi = _context.KanalPersonelleri
                                .Count(kp => kp.KanalAltIslemId == kai.KanalAltIslemId && kp.Aktiflik == BusinessObjectLayer.Enums.Common.Aktiflik.Aktif)
                        };

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<KanalAltIslemResponseDto?> GetKanalAltIslemByIdWithDetailsAsync(int kanalAltIslemId)
        {
            var query = from kai in _context.KanalAltIslemleri
                        join ka in _context.KanallarAlt on kai.KanalAltId equals ka.KanalAltId
                        join k in _context.Kanallar on ka.KanalId equals k.KanalId
                        join ki in _context.KanalIslemleri on kai.KanalIslemId equals ki.KanalIslemId
                        join hb in _context.HizmetBinalari on kai.HizmetBinasiId equals hb.HizmetBinasiId
                        where kai.KanalAltIslemId == kanalAltIslemId
                           && !kai.SilindiMi
                        select new KanalAltIslemResponseDto
                        {
                            KanalAltIslemId = kai.KanalAltIslemId,
                            KanalAltId = kai.KanalAltId,
                            KanalAltAdi = ka.KanalAltAdi,
                            HizmetBinasiId = kai.HizmetBinasiId,
                            KanalIslemId = kai.KanalIslemId,
                            KanalAdi = k.KanalAdi,
                            Aktiflik = kai.Aktiflik,
                            EklenmeTarihi = kai.EklenmeTarihi,
                            DuzenlenmeTarihi = kai.DuzenlenmeTarihi,
                            PersonelSayisi = _context.KanalPersonelleri
                                .Count(kp => kp.KanalAltIslemId == kai.KanalAltIslemId
                                          && kp.Aktiflik == BusinessObjectLayer.Enums.Common.Aktiflik.Aktif
                                          && !kp.SilindiMi)
                        };

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<List<KanalAltIslemResponseDto>> GetKanalAltIslemlerByKanalIslemIdAsync(int kanalIslemId)
        {
            var query = from kai in _context.KanalAltIslemleri
                        join ka in _context.KanallarAlt on kai.KanalAltId equals ka.KanalAltId
                        join k in _context.Kanallar on ka.KanalId equals k.KanalId
                        join ki in _context.KanalIslemleri on kai.KanalIslemId equals ki.KanalIslemId
                        where kai.KanalIslemId == kanalIslemId
                        select new KanalAltIslemResponseDto
                        {
                            KanalAltIslemId = kai.KanalAltIslemId,
                            KanalAltId = kai.KanalAltId,
                            KanalAltAdi = ka.KanalAltAdi,
                            KanalIslemId = kai.KanalIslemId,
                            KanalAdi = k.KanalAdi,
                            Aktiflik = kai.Aktiflik,
                            EklenmeTarihi = kai.EklenmeTarihi,
                            DuzenlenmeTarihi = kai.DuzenlenmeTarihi,
                            PersonelSayisi = _context.KanalPersonelleri
                                .Count(kp => kp.KanalAltIslemId == kai.KanalAltIslemId && kp.Aktiflik == BusinessObjectLayer.Enums.Common.Aktiflik.Aktif)
                        };

            return await query.AsNoTracking().ToListAsync();
        }

        // ═══════════════════════════════════════════════════════
        // KANAL İŞLEM SORGULARI
        // ═══════════════════════════════════════════════════════

        public async Task<List<KanalIslemResponseDto>> GetKanalIslemlerByHizmetBinasiIdAsync(int hizmetBinasiId)
        {
            var query = from ki in _context.KanalIslemleri
                        join k in _context.Kanallar on ki.KanalId equals k.KanalId
                        join kai in _context.KanalAltIslemleri on ki.KanalIslemId equals kai.KanalIslemId into kaiGroup
                        where _context.KanalAltIslemleri.Any(x => x.KanalIslemId == ki.KanalIslemId && x.HizmetBinasiId == hizmetBinasiId)
                        select new KanalIslemResponseDto
                        {
                            KanalIslemId = ki.KanalIslemId,
                            KanalId = ki.KanalId,
                            KanalAdi = k.KanalAdi,
                            Sira = ki.Sira,
                            Aktiflik = ki.Aktiflik,
                            EklenmeTarihi = ki.EklenmeTarihi,
                            DuzenlenmeTarihi = ki.DuzenlenmeTarihi,
                            KanalAltIslemSayisi = kaiGroup.Count()
                        };

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<KanalIslemResponseDto?> GetKanalIslemByIdWithDetailsAsync(int kanalIslemId)
        {
            var query = from ki in _context.KanalIslemleri
                        join k in _context.Kanallar on ki.KanalId equals k.KanalId
                        where ki.KanalIslemId == kanalIslemId
                        select new KanalIslemResponseDto
                        {
                            KanalIslemId = ki.KanalIslemId,
                            KanalId = ki.KanalId,
                            KanalAdi = k.KanalAdi,
                            Sira = ki.Sira,
                            Aktiflik = ki.Aktiflik,
                            EklenmeTarihi = ki.EklenmeTarihi,
                            DuzenlenmeTarihi = ki.DuzenlenmeTarihi,
                            KanalAltIslemSayisi = _context.KanalAltIslemleri.Count(x => x.KanalIslemId == ki.KanalIslemId)
                        };

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        // ═══════════════════════════════════════════════════════
        // KANAL PERSONEL SORGULARI
        // ═══════════════════════════════════════════════════════

        public async Task<List<KanalPersonelResponseDto>> GetKanalPersonellerByHizmetBinasiIdAsync(int hizmetBinasiId)
        {
            var query = from kp in _context.KanalPersonelleri
                        join p in _context.Personeller on kp.TcKimlikNo equals p.TcKimlikNo
                        join kai in _context.KanalAltIslemleri on kp.KanalAltIslemId equals kai.KanalAltIslemId
                        join ka in _context.KanallarAlt on kai.KanalAltId equals ka.KanalAltId
                        where kai.HizmetBinasiId == hizmetBinasiId
                        select new KanalPersonelResponseDto
                        {
                            KanalPersonelId = kp.KanalPersonelId,
                            TcKimlikNo = kp.TcKimlikNo,
                            PersonelAdSoyad = p.AdSoyad,
                            KanalAltIslemId = kp.KanalAltIslemId,
                            KanalAltIslemAdi = ka.KanalAltAdi,
                            Uzmanlik = kp.Uzmanlik,
                            Aktiflik = kp.Aktiflik,
                            EklenmeTarihi = kp.EklenmeTarihi,
                            DuzenlenmeTarihi = kp.DuzenlenmeTarihi
                        };

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<List<KanalPersonelResponseDto>> GetKanalPersonellerByKanalAltIslemIdAsync(int kanalAltIslemId)
        {
            var query = from kp in _context.KanalPersonelleri
                        join p in _context.Personeller on kp.TcKimlikNo equals p.TcKimlikNo
                        join kai in _context.KanalAltIslemleri on kp.KanalAltIslemId equals kai.KanalAltIslemId
                        join ka in _context.KanallarAlt on kai.KanalAltId equals ka.KanalAltId
                        where kp.KanalAltIslemId == kanalAltIslemId
                        select new KanalPersonelResponseDto
                        {
                            KanalPersonelId = kp.KanalPersonelId,
                            TcKimlikNo = kp.TcKimlikNo,
                            PersonelAdSoyad = p.AdSoyad,
                            KanalAltIslemId = kp.KanalAltIslemId,
                            KanalAltIslemAdi = ka.KanalAltAdi,
                            Uzmanlik = kp.Uzmanlik,
                            Aktiflik = kp.Aktiflik,
                            EklenmeTarihi = kp.EklenmeTarihi,
                            DuzenlenmeTarihi = kp.DuzenlenmeTarihi
                        };

            return await query.AsNoTracking().ToListAsync();
        }

        // ═══════════════════════════════════════════════════════
        // İSTATİSTİK VE DASHBOARD SORGULARI
        // ═══════════════════════════════════════════════════════

        public async Task<Dictionary<int, int>> GetKanalAltIslemPersonelSayilariAsync(int hizmetBinasiId)
        {
            var query = from kai in _context.KanalAltIslemleri
                        where kai.HizmetBinasiId == hizmetBinasiId
                        select new
                        {
                            KanalAltIslemId = kai.KanalAltIslemId,
                            PersonelSayisi = _context.KanalPersonelleri
                                .Count(kp => kp.KanalAltIslemId == kai.KanalAltIslemId && 
                                           kp.Aktiflik == BusinessObjectLayer.Enums.Common.Aktiflik.Aktif)
                        };

            var result = await query.AsNoTracking().ToListAsync();
            return result.ToDictionary(x => x.KanalAltIslemId, x => x.PersonelSayisi);
        }

        public async Task<List<KanalAltIslemResponseDto>> GetEslestirmeYapilmamisKanalAltIslemlerAsync(int hizmetBinasiId)
        {
            var query = from kai in _context.KanalAltIslemleri
                        join ka in _context.KanallarAlt on kai.KanalAltId equals ka.KanalAltId
                        join k in _context.Kanallar on ka.KanalId equals k.KanalId
                        join ki in _context.KanalIslemleri on kai.KanalIslemId equals ki.KanalIslemId
                        where kai.HizmetBinasiId == hizmetBinasiId &&
                              !_context.KanalPersonelleri.Any(kp => kp.KanalAltIslemId == kai.KanalAltIslemId && 
                                                                   kp.Aktiflik == BusinessObjectLayer.Enums.Common.Aktiflik.Aktif)
                        select new KanalAltIslemResponseDto
                        {
                            KanalAltIslemId = kai.KanalAltIslemId,
                            KanalAltId = kai.KanalAltId,
                            KanalAltAdi = ka.KanalAltAdi,
                            KanalIslemId = kai.KanalIslemId,
                            KanalAdi = k.KanalAdi,
                            Aktiflik = kai.Aktiflik,
                            EklenmeTarihi = kai.EklenmeTarihi,
                            DuzenlenmeTarihi = kai.DuzenlenmeTarihi,
                            PersonelSayisi = 0
                        };

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<List<KanalPersonelResponseDto>> GetPersonelKanalAtamalarByHizmetBinasiIdAsync(int hizmetBinasiId)
        {
            var query = from personel in _context.Personeller
                        join kanalPersonel in _context.KanalPersonelleri
                            on personel.TcKimlikNo equals kanalPersonel.TcKimlikNo into kpGroup
                        from kp in kpGroup.DefaultIfEmpty() 
                        join kanalAltIslem in _context.KanalAltIslemleri
                            on kp.KanalAltIslemId equals kanalAltIslem.KanalAltIslemId into kaiGroup
                        from kai in kaiGroup.DefaultIfEmpty() 
                        join kanalAlt in _context.KanallarAlt
                            on kai.KanalAltId equals kanalAlt.KanalAltId into kaGroup
                        from ka in kaGroup.DefaultIfEmpty() 
                        join kanal in _context.Kanallar
                            on ka.KanalId equals kanal.KanalId into kGroup
                        from k in kGroup.DefaultIfEmpty() 
                        where personel.HizmetBinasiId == hizmetBinasiId
                           && personel.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif
                        select new KanalPersonelResponseDto
                        {
                            // KanalPersonel bilgileri
                            KanalPersonelId = kp != null ? kp.KanalPersonelId : 0,

                            // Personel bilgileri
                            TcKimlikNo = personel.TcKimlikNo,
                            PersonelAdSoyad = personel.AdSoyad,

                            // Kanal Alt İşlem bilgileri
                            KanalAltIslemId = kp != null ? kp.KanalAltIslemId : 0,
                            KanalAltIslemAdi = kai != null && ka != null && k != null
                                ? $"{k.KanalAdi} - {ka.KanalAltAdi}"
                                : "Atama Yapılmamış",

                            // Uzmanlık ve Aktiflik
                            Uzmanlik = kp != null ? kp.Uzmanlik : BusinessObjectLayer.Enums.SiramatikIslemleri.PersonelUzmanlik.BilgisiYok,
                            Aktiflik = (Aktiflik)personel.PersonelAktiflikDurum,
                            Resim = personel.Resim,

                            // Tarihler (atanmış personeller için)
                            EklenmeTarihi = kp != null ? kp.EklenmeTarihi : DateTime.MinValue,
                            DuzenlenmeTarihi = kp != null ? kp.DuzenlenmeTarihi : DateTime.MinValue
                        };

            return await query
                .AsNoTracking()
                .OrderBy(x => x.PersonelAdSoyad)
                .ToListAsync();
        }

        public async Task<List<PersonelAtamaMatrixDto>> GetPersonelAtamaMatrixByHizmetBinasiIdAsync(int hizmetBinasiId)
        {
            // 1. Hizmet binasındaki aktif personelleri getir
            var personelQuery = from p in _context.Personeller
                                where p.HizmetBinasiId == hizmetBinasiId
                                   && p.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif
                                   && !p.SilindiMi
                                select new PersonelAtamaMatrixDto
                                {
                                    TcKimlikNo = p.TcKimlikNo,
                                    SicilNo = p.SicilNo,
                                    PersonelAdSoyad = p.AdSoyad,
                                    DepartmanId = p.DepartmanId,
                                    DepartmanAdi = p.Departman != null ? p.Departman.DepartmanAdi : "",
                                    ServisId = p.ServisId,
                                    ServisAdi = p.Servis != null ? p.Servis.ServisAdi : "",
                                    HizmetBinasiId = p.HizmetBinasiId,
                                    HizmetBinasiAdi = p.HizmetBinasi != null ? p.HizmetBinasi.HizmetBinasiAdi : "",
                                    Resim = p.Resim,
                                    Aktiflik = (Aktiflik)p.PersonelAktiflikDurum,
                                    KanalAtamalari = new List<PersonelKanalAtamaDto>()
                                };

            var personelList = await personelQuery
                .AsNoTracking()
                .OrderBy(x => x.PersonelAdSoyad)
                .ToListAsync();

            // 2. Bu personellerin kanal atamalarını getir
            var tcKimlikNolar = personelList.Select(p => p.TcKimlikNo).ToList();

            var atamaQuery = from kp in _context.KanalPersonelleri
                             join kai in _context.KanalAltIslemleri on kp.KanalAltIslemId equals kai.KanalAltIslemId
                             join ka in _context.KanallarAlt on kai.KanalAltId equals ka.KanalAltId
                             join k in _context.Kanallar on ka.KanalId equals k.KanalId
                             where tcKimlikNolar.Contains(kp.TcKimlikNo)
                                && kai.HizmetBinasiId == hizmetBinasiId
                                && kp.Aktiflik == Aktiflik.Aktif
                                && !kp.SilindiMi
                             select new
                             {
                                 kp.TcKimlikNo,
                                 Atama = new PersonelKanalAtamaDto
                                 {
                                     KanalPersonelId = kp.KanalPersonelId,
                                     KanalAltIslemId = kp.KanalAltIslemId,
                                     KanalAltIslemAdi = $"{k.KanalAdi} - {ka.KanalAltAdi}",
                                     Uzmanlik = kp.Uzmanlik,
                                     EklenmeTarihi = kp.EklenmeTarihi,
                                     DuzenlenmeTarihi = kp.DuzenlenmeTarihi
                                 }
                             };

            var atamalar = await atamaQuery.AsNoTracking().ToListAsync();

            // 3. Atamaları personellere grupla
            var atamaGruplari = atamalar
                .GroupBy(a => a.TcKimlikNo)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Atama).ToList());

            // 4. Her personele atamalarını ekle
            foreach (var personel in personelList)
            {
                if (atamaGruplari.ContainsKey(personel.TcKimlikNo))
                {
                    personel.KanalAtamalari = atamaGruplari[personel.TcKimlikNo];
                }
            }

            return personelList;
        }

        // ═══════════════════════════════════════════════════════
        // BANKO SIRA ÇAĞIRMA PANELİ
        // ═══════════════════════════════════════════════════════
        public async Task<List<SiraCagirmaResponseDto>> GetBankoPanelBekleyenSiralarAsync(string tcKimlikNo)
        {
            /*
            DECLARE @TcKimlikNo NVARCHAR(11) = '16406457430';
            DECLARE @Bugun DATE = CAST(GETDATE() AS DATE);

            -- Personelin uzmanlık kayıtları (CTE)
            ;WITH PersonelinUzmanlikKayitlari AS (
                SELECT DISTINCT
                    kp.KanalAltIslemId,
                    kp.Uzmanlik
                FROM SIR_KanalPersonelleri AS kp WITH (NOLOCK)
                WHERE kp.TcKimlikNo = @TcKimlikNo
                    AND kp.Aktiflik = 1
                    AND kp.Uzmanlik != 0
                    AND kp.SilindiMi = 0
            )

            -- Ana sorgu
            SELECT DISTINCT
                s.SiraId,
                s.SiraNo,
                s.KanalAltAdi,
                s.SiraAlisZamani,
                s.IslemBaslamaZamani,
                s.BeklemeDurum,
                uzm.Uzmanlik,
                b.BankoId,
                hb.HizmetBinasiId,
                hb.HizmetBinasiAdi,
                p.AdSoyad AS PersonelAdSoyad,
                s.YonlendirildiMi,
                s.YonlendirmeTipi,
                s.HedefBankoId,
                s.YonlendirenBankoId,
                s.YonlendirenPersonelTc,
                pYonlendiren.AdSoyad AS YonlendirenPersonelAdSoyad,
                -- ⭐ ORDER BY için gerekli hesaplanan sütunları SELECT'e ekliyoruz
                CASE 
                    WHEN s.BeklemeDurum = 1 THEN 0   -- Çağrıldı
                    WHEN s.BeklemeDurum = 3 THEN 1   -- Yönlendirildi
                    ELSE 2                            -- Beklemede
                END AS DurumOnceligi,
                CASE 
                    WHEN s.BeklemeDurum = 0 THEN 
                        CASE uzm.Uzmanlik
                            WHEN 3 THEN 0  -- Şef
                            WHEN 2 THEN 1  -- Uzman
                            WHEN 1 THEN 2  -- Yrd. Uzman
                            ELSE 3
                        END
                    ELSE 99
                END AS UzmanlikOnceligi
            FROM SIR_BankoKullanicilari AS bk WITH (NOLOCK)
            INNER JOIN SIR_Bankolar AS b WITH (NOLOCK) 
                ON b.BankoId = bk.BankoId
                AND b.BankoAktiflik = 1
                AND b.SilindiMi = 0
            INNER JOIN PER_Personeller AS p WITH (NOLOCK) 
                ON p.TcKimlikNo = bk.TcKimlikNo 
                AND p.PersonelAktiflikDurum = 1 
                AND p.HizmetBinasiId = b.HizmetBinasiId
                AND p.SilindiMi = 0
            INNER JOIN CMN_Users AS u WITH (NOLOCK) 
                ON u.TcKimlikNo = p.TcKimlikNo 
                AND u.BankoModuAktif = 1
                AND u.AktifMi = 1
            INNER JOIN CMN_HizmetBinalari AS hb WITH (NOLOCK) 
                ON hb.HizmetBinasiId = bk.HizmetBinasiId
            INNER JOIN PersonelinUzmanlikKayitlari AS uzm 
                ON 1=1
            INNER JOIN SIR_KanalAltIslemleri AS kai WITH (NOLOCK) 
                ON kai.KanalAltIslemId = uzm.KanalAltIslemId
                AND kai.Aktiflik = 1 
                AND kai.SilindiMi = 0
            INNER JOIN SIR_Siralar AS s WITH (NOLOCK) 
                ON s.KanalAltIslemId = kai.KanalAltIslemId 
                AND s.HizmetBinasiId = bk.HizmetBinasiId
                AND s.SilindiMi = 0
            LEFT JOIN PER_Personeller AS pYonlendiren WITH (NOLOCK)
                ON pYonlendiren.TcKimlikNo = s.YonlendirenPersonelTc
                AND pYonlendiren.SilindiMi = 0
            WHERE bk.TcKimlikNo = @TcKimlikNo
                AND bk.SilindiMi = 0
                AND uzm.Uzmanlik IN (1, 2, 3)
                AND CAST(s.SiraAlisZamani AS DATE) = @Bugun
                AND (
                    -- 1. Normal Bekleyen Sıralar
                    s.BeklemeDurum = 0
        
                    -- 2. Çağrılmış EN SON Sıra
                    OR (s.BeklemeDurum = 1 
                        AND s.TcKimlikNo = @TcKimlikNo
                        AND s.SiraId = (
                            SELECT TOP 1 s2.SiraId
                            FROM SIR_Siralar s2 WITH (NOLOCK)
                            WHERE s2.TcKimlikNo = @TcKimlikNo
                                AND s2.BeklemeDurum = 1
                                AND CAST(s2.SiraAlisZamani AS DATE) = @Bugun
                                AND s2.SilindiMi = 0
                            ORDER BY s2.SiraNo DESC
                        ))
        
                    -- 3. Şef'e Yönlendirilmiş
                    OR (s.BeklemeDurum = 3 
                        AND s.YonlendirildiMi = 1 
                        AND s.YonlendirmeTipi = 2
                        AND s.TcKimlikNo != @TcKimlikNo 
                        AND s.HedefBankoId IS NULL 
                        AND EXISTS (
                            SELECT 1 
                            FROM SIR_KanalPersonelleri kp2 WITH (NOLOCK)
                            WHERE kp2.TcKimlikNo = @TcKimlikNo
                                AND kp2.Aktiflik = 1
                                AND kp2.Uzmanlik = 3  -- Şef
                                AND kp2.SilindiMi = 0
                        ))
        
                    -- 4. Başka Bankoya Yönlendirilmiş
                    OR (s.BeklemeDurum = 3 
                        AND s.YonlendirildiMi = 1 
                        AND s.YonlendirmeTipi = 1
                        AND s.TcKimlikNo != @TcKimlikNo
                        AND s.HedefBankoId = bk.BankoId)
        
                    -- 5. Genel Uzmana Yönlendirilmiş
                    OR (s.BeklemeDurum = 3 
                        AND s.YonlendirildiMi = 1 
                        AND s.YonlendirmeTipi = 3
                        AND s.TcKimlikNo != @TcKimlikNo
                        AND s.HedefBankoId IS NULL 
                        AND EXISTS (
                            SELECT 1 
                            FROM SIR_KanalPersonelleri kp2 WITH (NOLOCK)
                            WHERE kp2.TcKimlikNo = @TcKimlikNo
                                AND kp2.Aktiflik = 1
                                AND kp2.Uzmanlik = 2  -- Uzman
                                AND kp2.SilindiMi = 0
                        ))
                )
            ORDER BY
                DurumOnceligi ASC,        -- ⭐ SELECT'teki alias kullanılıyor
                UzmanlikOnceligi ASC,     -- ⭐ SELECT'teki alias kullanılıyor
                s.SiraAlisZamani ASC; 
            */
            var today = DateTime.Today;

            // ADIM 1: Personelin kanal alt işlem ID'lerini al
            var personelKanalAltIslemIds = await (
                from kp in _context.KanalPersonelleri
                where kp.TcKimlikNo == tcKimlikNo
                   && kp.Aktiflik == Aktiflik.Aktif
                   && kp.Uzmanlik != PersonelUzmanlik.BilgisiYok
                   && !kp.SilindiMi
                select kp.KanalAltIslemId
            ).Distinct().ToListAsync();

            if (!personelKanalAltIslemIds.Any())
                return new List<SiraCagirmaResponseDto>();

            // Personelin GENEL şef yetkisi var mı?
            var personelSefMi = await _context.KanalPersonelleri
                .AnyAsync(kp => kp.TcKimlikNo == tcKimlikNo
                             && kp.Aktiflik == Aktiflik.Aktif
                             && kp.Uzmanlik == PersonelUzmanlik.Sef
                             && !kp.SilindiMi);

            // Personelin GENEL uzman yetkisi var mı?
            var personelUzmanMi = await _context.KanalPersonelleri
                .AnyAsync(kp => kp.TcKimlikNo == tcKimlikNo
                             && kp.Aktiflik == Aktiflik.Aktif
                             && kp.Uzmanlik == PersonelUzmanlik.Uzman
                             && !kp.SilindiMi);

            // ADIM 2: Çağrılmış EN SON sıra
            var sonCagirilanSiraId = await (
                from s in _context.Siralar
                where s.TcKimlikNo == tcKimlikNo
                   && s.BeklemeDurum == BeklemeDurum.Cagrildi
                   && s.SiraAlisZamani.Date == today
                   && !s.SilindiMi
                orderby s.SiraNo descending
                select s.SiraId
            ).FirstOrDefaultAsync();

            // ADIM 3: Ana sorgu
            var query = from bk in _context.BankoKullanicilari
                        join b in _context.Bankolar on bk.BankoId equals b.BankoId
                        join p in _context.Personeller on bk.TcKimlikNo equals p.TcKimlikNo
                        join u in _context.Users on p.TcKimlikNo equals u.TcKimlikNo
                        join hb in _context.HizmetBinalari on bk.HizmetBinasiId equals hb.HizmetBinasiId
                        join kai in _context.KanalAltIslemleri on true equals true
                        join kp in _context.KanalPersonelleri on new { kai.KanalAltIslemId, TcKimlikNo = tcKimlikNo }
                            equals new { kp.KanalAltIslemId, kp.TcKimlikNo }
                        join s in _context.Siralar on kai.KanalAltIslemId equals s.KanalAltIslemId
                        join pYonlendiren in _context.Personeller on s.YonlendirenPersonelTc equals pYonlendiren.TcKimlikNo into yonlendirenGroup
                        from pYonlendiren in yonlendirenGroup.DefaultIfEmpty()
                        where bk.TcKimlikNo == tcKimlikNo
                           && !bk.SilindiMi
                           && b.BankoAktiflik == Aktiflik.Aktif
                           && !b.SilindiMi
                           && p.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif
                           && p.HizmetBinasiId == b.HizmetBinasiId
                           && !p.SilindiMi
                           && u.BankoModuAktif
                           && u.AktifMi
                           && personelKanalAltIslemIds.Contains(kai.KanalAltIslemId)
                           && kai.Aktiflik == Aktiflik.Aktif
                           && !kai.SilindiMi
                           && kp.Aktiflik == Aktiflik.Aktif
                           && !kp.SilindiMi
                           && kp.Uzmanlik != PersonelUzmanlik.BilgisiYok
                           && s.HizmetBinasiId == bk.HizmetBinasiId
                           && !s.SilindiMi
                           && s.SiraAlisZamani.Date == today
                           && (
                                // 1. Normal Bekleyen Sıralar
                                s.BeklemeDurum == BeklemeDurum.Beklemede

                                // 2. Çağrılmış EN SON Sıra ⭐ !s.YonlendirildiMi eklendi
                                || (s.BeklemeDurum == BeklemeDurum.Cagrildi
                                    && s.TcKimlikNo == tcKimlikNo
                                    && s.SiraId == sonCagirilanSiraId)

                                // 3. Şef'e Yönlendirilmiş
                                || (s.BeklemeDurum == BeklemeDurum.Yonlendirildi
                                    && s.YonlendirildiMi
                                    && s.YonlendirmeTipi == YonlendirmeTipi.Sef
                                    && s.TcKimlikNo != tcKimlikNo
                                    && s.HedefBankoId == null
                                    && personelSefMi)

                                // 4. Başka Bankoya Yönlendirilmiş
                                || (s.BeklemeDurum == BeklemeDurum.Yonlendirildi
                                    && s.YonlendirildiMi
                                    && s.YonlendirmeTipi == YonlendirmeTipi.BaskaBanko
                                    && s.TcKimlikNo != tcKimlikNo
                                    && s.HedefBankoId == bk.BankoId)

                                // 5. Genel Uzmana Yönlendirilmiş
                                || (s.BeklemeDurum == BeklemeDurum.Yonlendirildi
                                    && s.YonlendirildiMi
                                    && s.YonlendirmeTipi == YonlendirmeTipi.UzmanPersonel
                                    && s.TcKimlikNo != tcKimlikNo
                                    && s.HedefBankoId == null
                                    && personelUzmanMi)
                              )
                        select new
                        {
                            s.SiraId,
                            s.SiraNo,
                            s.KanalAltAdi,
                            s.BeklemeDurum,
                            s.SiraAlisZamani,
                            s.IslemBaslamaZamani,
                            hb.HizmetBinasiId,
                            hb.HizmetBinasiAdi,
                            Uzmanlik = kp.Uzmanlik,
                            PersonelAdSoyad = p.AdSoyad,
                            BankoId = bk.BankoId,
                            s.YonlendirildiMi,
                            s.YonlendirmeTipi,
                            s.HedefBankoId,
                            s.YonlendirenBankoId,
                            s.YonlendirenPersonelTc,
                            YonlendirenPersonelAdSoyad = pYonlendiren != null ? pYonlendiren.AdSoyad : null,

                            // ORDER BY için hesaplanan değerler
                            DurumOnceligi = s.BeklemeDurum == BeklemeDurum.Cagrildi ? 0
                                          : s.BeklemeDurum == BeklemeDurum.Yonlendirildi ? 1
                                          : 2,
                            UzmanlikOnceligi = s.BeklemeDurum == BeklemeDurum.Beklemede
                                             ? (kp.Uzmanlik == PersonelUzmanlik.Sef ? 0
                                              : kp.Uzmanlik == PersonelUzmanlik.Uzman ? 1
                                              : kp.Uzmanlik == PersonelUzmanlik.YrdUzman ? 2
                                              : 3)
                                             : 99
                        };

            var rawResult = await query
                .AsNoTracking()
                .Distinct()
                .OrderBy(x => x.DurumOnceligi)
                .ThenBy(x => x.UzmanlikOnceligi)
                .ThenBy(x => x.SiraAlisZamani)
                .ToListAsync();

            // ADIM 4: DTO mapping
            var result = rawResult
                .Select(x => new SiraCagirmaResponseDto
                {
                    SiraId = x.SiraId,
                    SiraNo = x.SiraNo,
                    KanalAltAdi = x.KanalAltAdi,
                    BeklemeDurum = x.BeklemeDurum,
                    SiraAlisZamani = x.SiraAlisZamani,
                    IslemBaslamaZamani = x.IslemBaslamaZamani,
                    PersonelAdSoyad = x.PersonelAdSoyad,
                    HizmetBinasiId = x.HizmetBinasiId,
                    HizmetBinasiAdi = x.HizmetBinasiAdi,
                    Uzmanlik = x.Uzmanlik,  // ⭐ Eklendi
                    YonlendirildiMi = x.YonlendirildiMi,
                    YonlendirmeTipi = x.YonlendirmeTipi,
                    YonlendirenPersonelTc = x.YonlendirenPersonelTc,
                    YonlendirenPersonelAdSoyad = x.YonlendirenPersonelAdSoyad,
                    YonlendirmeAciklamasi = BuildYonlendirmeAciklamasi(
                        x.YonlendirildiMi,
                        x.YonlendirmeTipi,
                        x.Uzmanlik,
                        x.HedefBankoId,
                        x.BankoId,
                        x.YonlendirenPersonelAdSoyad)
                })
                .ToList();

            return result;

            // Local function (aynı kalıyor)
            string? BuildYonlendirmeAciklamasi(
                bool yonlendirildiMi,
                YonlendirmeTipi? yonlendirmeTipi,
                PersonelUzmanlik uzmanlik,
                int? hedefBankoId,
                int bankoId,
                string? yonlendirenPersonelAdSoyad)
            {
                if (!yonlendirildiMi || yonlendirmeTipi == null)
                    return null;

                var yonlendirenBilgi = !string.IsNullOrEmpty(yonlendirenPersonelAdSoyad)
                    ? $"{yonlendirenPersonelAdSoyad} tarafından"
                    : "";

                return yonlendirmeTipi switch
                {
                    YonlendirmeTipi.Sef =>
                        $"Şef değerlendirmesi için size {yonlendirenBilgi} yönlendirildi.",

                    YonlendirmeTipi.UzmanPersonel when uzmanlik == PersonelUzmanlik.Uzman =>
                        $"Uzman görüşü beklendiği için size {yonlendirenBilgi} aktarılmış durumda.",

                    YonlendirmeTipi.UzmanPersonel =>
                        $"Uzman personele {yonlendirenBilgi} yönlendirildi.",

                    YonlendirmeTipi.BaskaBanko when hedefBankoId.HasValue && hedefBankoId == bankoId =>
                        $"Bu sıra doğrudan bankonuza {yonlendirenBilgi} yönlendirildi.",

                    YonlendirmeTipi.BaskaBanko =>
                        $"Başka bir bankoya {yonlendirenBilgi} yönlendirilmiş sıra.",

                    _ => null
                };
            }
        }

        // ═══════════════════════════════════════════════════════
        // SIGNALR BROADCAST İÇİN ETKİLENEN PERSONELLER
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Belirli bir sıranın çağrılması/tamamlanması durumunda etkilenen personellerin TC listesini döner.
        /// Aynı KanalAltIslem'e atanmış ve banko modunda olan personeller etkilenir.
        /// </summary>
        public async Task<List<string>> GetSiraEtkilenenPersonellerAsync(int siraId)
        {
            // 1. Sıranın KanalAltIslemId ve HizmetBinasiId'sini bul
            var sira = await _context.Siralar
                .AsNoTracking()
                .Where(s => s.SiraId == siraId)
                .Select(s => new { s.KanalAltIslemId, s.HizmetBinasiId })
                .FirstOrDefaultAsync();

            if (sira == null)
                return new List<string>();

            return await GetBankoModundakiPersonellerAsync(sira.HizmetBinasiId, sira.KanalAltIslemId);
        }

        /// <summary>
        /// Belirli bir HizmetBinasi ve KanalAltIslem için banko modunda olan personellerin TC listesini döner.
        /// </summary>
        public async Task<List<string>> GetBankoModundakiPersonellerAsync(int hizmetBinasiId, int kanalAltIslemId)
        {
            // Banko modunda olan personelleri bul:
            // 1. KanalPersonel tablosunda bu KanalAltIslem'e atanmış
            // 2. User tablosunda BankoModuAktif = true
            // 3. Aynı HizmetBinasi'nda

            var etkilenenPersoneller = await (
                from kp in _context.KanalPersonelleri
                join u in _context.Users on kp.TcKimlikNo equals u.TcKimlikNo
                join bk in _context.BankoKullanicilari on u.TcKimlikNo equals bk.TcKimlikNo into bkJoin
                from bk in bkJoin.DefaultIfEmpty()
                join b in _context.Bankolar on bk.BankoId equals b.BankoId into bJoin
                from b in bJoin.DefaultIfEmpty()
                where kp.KanalAltIslemId == kanalAltIslemId
                   && kp.Aktiflik == Aktiflik.Aktif
                   && !kp.SilindiMi
                   && u.BankoModuAktif == true
                   && (b == null || b.HizmetBinasiId == hizmetBinasiId)
                select kp.TcKimlikNo
            ).Distinct().ToListAsync();

            return etkilenenPersoneller;
        }

        /// <summary>
        /// Belirli bir HizmetBinasi ve KanalAltId için banko modunda olan ve en az Yrd.Uzman yetkisine sahip personellerin TC listesini döner.
        /// Kiosk sıra alma için kullanılır - sadece işlem yapabilecek personel varsa sıra alınabilir.
        /// 
        /// NOT: kanalAltId parametresi KanalAlt tablosundaki ID'dir (KanalAltIslem değil!)
        /// KanalPersonel.KanalAltIslemId → KanalAltIslem.KanalAltId üzerinden eşleştirilir.
        /// 
        /// Kontroller:
        /// 1. KanalPersonel: Bu KanalAlt'a ait bir KanalAltIslem'e atanmış, aktif, en az Yrd.Uzman
        /// 2. User: BankoModuAktif = true VE AktifBankoId != null (personel ŞU AN banko modunda mı?)
        /// 3. Banko: User.AktifBankoId üzerinden - aynı hizmet binasında ve aktif mi?
        /// </summary>
        public async Task<List<string>> GetBankoModundakiYetkiliPersonellerAsync(int hizmetBinasiId, int kanalAltId)
        {
            // KanalPersonel → KanalAltIslem → KanalAlt üzerinden eşleştirme
            var yetkiliPersoneller = await (
                from kp in _context.KanalPersonelleri
                join kai in _context.KanalAltIslemleri on kp.KanalAltIslemId equals kai.KanalAltIslemId
                join u in _context.Users on kp.TcKimlikNo equals u.TcKimlikNo
                join b in _context.Bankolar on u.AktifBankoId equals b.BankoId  // User'ın aktif bankosunu al
                where kai.KanalAltId == kanalAltId                    // ⭐ KanalAlt ID eşleşmesi
                   && kai.HizmetBinasiId == hizmetBinasiId            // ⭐ Aynı hizmet binasındaki KanalAltIslem
                   && kai.Aktiflik == Aktiflik.Aktif                  // KanalAltIslem aktif
                   && !kai.SilindiMi                                  // KanalAltIslem silinmemiş
                   && kp.Aktiflik == Aktiflik.Aktif                   // Personel ataması aktif
                   && !kp.SilindiMi                                   // Personel ataması silinmemiş
                   && kp.Uzmanlik != PersonelUzmanlik.BilgisiYok      // En az Yrd.Uzman (1, 2, 3)
                   && u.BankoModuAktif == true                        // ⭐ ŞU AN banko modunda
                   && u.AktifBankoId != null                          // ⭐ Aktif banko ID'si var
                   && u.AktifMi == true                               // Kullanıcı aktif
                   && b.HizmetBinasiId == hizmetBinasiId              // Aynı hizmet binasında
                   && b.BankoAktiflik == Aktiflik.Aktif               // Banko aktif
                   && !b.SilindiMi                                    // Banko silinmemiş
                select kp.TcKimlikNo
            ).Distinct().ToListAsync();

            return yetkiliPersoneller;
        }

        // ═══════════════════════════════════════════════════════
        // KIOSK MENÜ SORGULARI
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Belirli bir Kiosk için menüleri detaylı olarak getirir
        /// Menüler için aktif personel sayısı, işlem sayısı gibi istatistiksel bilgileri içerir
        /// </summary>
        public async Task<List<KioskMenuDetayResponseDto>> GetKioskMenulerByKioskIdAsync(int kioskId)
        {
            /*
            SELECT
            -- Hizmet Binası Bilgileri
            hb.HizmetBinasiId,
            hb.HizmetBinasiAdi,
    
            -- Kiosk Bilgileri
            k.KioskId,
            k.KioskAdi,
            k.KioskIp,
    
            -- Menü Bilgileri (Gruplanacak)
            km.KioskMenuId,
            km.MenuAdi,
            km.Aciklama AS MenuAciklama,
            km.MenuSira AS MenuSiraNo,
    
            -- Menü Atama Bilgileri
            MAX(kma.KioskMenuAtamaId) AS KioskMenuAtamaId,
            MAX(kma.AtamaTarihi) AS AtamaTarihi,
    
            -- Aggregate Bilgiler
            COUNT(DISTINCT kmi.KioskMenuIslemId) AS ToplamIslemSayisi,
            COUNT(DISTINCT ka.KanalAltId) AS ToplamKanalAltSayisi,
            COUNT(DISTINCT kp.TcKimlikNo) AS ToplamPersonelSayisi
    
        FROM dbo.CMN_HizmetBinalari hb WITH (NOLOCK)
            INNER JOIN dbo.SIR_KioskTanim AS k WITH (NOLOCK) 
                ON hb.HizmetBinasiId = k.HizmetBinasiId
            INNER JOIN dbo.SIR_KioskMenuAtama AS kma WITH (NOLOCK)
                ON k.KioskId = kma.KioskId
            INNER JOIN dbo.SIR_KioskMenuTanim AS km WITH (NOLOCK)
                ON kma.KioskMenuId = km.KioskMenuId
            INNER JOIN dbo.SIR_KioskMenuIslem AS kmi WITH (NOLOCK)
                ON km.KioskMenuId = kmi.KioskMenuId
            INNER JOIN dbo.SIR_KanallarAlt AS ka WITH (NOLOCK)
                ON kmi.KanalAltId = ka.KanalAltId
            INNER JOIN dbo.SIR_KanalAltIslemleri AS kai WITH (NOLOCK)
                ON kai.KanalAltId = ka.KanalAltId 
                AND kai.HizmetBinasiId = k.HizmetBinasiId
            INNER JOIN dbo.SIR_KanalPersonelleri AS kp WITH (NOLOCK)
                ON kp.KanalAltIslemId = kai.KanalAltIslemId
            INNER JOIN dbo.CMN_Users AS u WITH (NOLOCK)
                ON u.TcKimlikNo = kp.TcKimlikNo
        
        WHERE 
            -- Kiosk filtresi
            k.KioskId = 1 -- Aliağa SGM'deki kiosk
        
            -- Soft delete kontrolleri
            AND hb.SilindiMi = 0
            AND k.SilindiMi = 0
            AND kma.SilindiMi = 0
            AND km.SilindiMi = 0
            AND kmi.SilindiMi = 0
            AND ka.SilindiMi = 0
    
            -- Aktiflik kontrolleri
            AND hb.HizmetBinasiAktiflik = 1  -- Aktif
            AND k.Aktiflik = 1               -- Aktif
            AND kma.Aktiflik = 1             -- Aktif
            AND km.Aktiflik = 1              -- Aktif
            AND kmi.Aktiflik = 1             -- Aktif
            AND ka.Aktiflik = 1              -- Aktif
            AND u.BankoModuAktif = 1         -- Banko Modu Aktiflik
            AND kp.Uzmanlik != 0             -- Konusunda Bilgisi Olan Personel

        GROUP BY 
            hb.HizmetBinasiId,
            hb.HizmetBinasiAdi,
            k.KioskId,
            k.KioskAdi,
            k.KioskIp,
            km.KioskMenuId,
            km.MenuAdi,
            km.Aciklama,
            km.MenuSira

        ORDER BY 
            k.KioskAdi,       
            km.MenuSira;
 
            */
            var query = from hb in _context.HizmetBinalari
                        join k in _context.Kiosklar on hb.HizmetBinasiId equals k.HizmetBinasiId
                        join kma in _context.KioskMenuAtamalari on k.KioskId equals kma.KioskId
                        join km in _context.KioskMenuler on kma.KioskMenuId equals km.KioskMenuId
                        join kmi in _context.KioskMenuIslemleri on km.KioskMenuId equals kmi.KioskMenuId
                        join ka in _context.KanallarAlt on kmi.KanalAltId equals ka.KanalAltId
                        join kai in _context.KanalAltIslemleri on new { ka.KanalAltId, k.HizmetBinasiId }
                            equals new { kai.KanalAltId, kai.HizmetBinasiId }
                        join kp in _context.KanalPersonelleri on kai.KanalAltIslemId equals kp.KanalAltIslemId
                        join u in _context.Users on kp.TcKimlikNo equals u.TcKimlikNo
                        where k.KioskId == kioskId
                           // Soft delete kontrolleri
                           && !hb.SilindiMi
                           && !k.SilindiMi
                           && !kma.SilindiMi
                           && !km.SilindiMi
                           && !kmi.SilindiMi
                           && !ka.SilindiMi
                           // Aktiflik kontrolleri
                           && hb.HizmetBinasiAktiflik == Aktiflik.Aktif
                           && k.Aktiflik == Aktiflik.Aktif
                           && kma.Aktiflik == Aktiflik.Aktif
                           && km.Aktiflik == Aktiflik.Aktif
                           && kmi.Aktiflik == Aktiflik.Aktif
                           && ka.Aktiflik == Aktiflik.Aktif
                           && u.BankoModuAktif == true
                           && kp.Uzmanlik != PersonelUzmanlik.BilgisiYok  // Konusunda bilgisi olan personel
                        group new { hb, k, kma, km, kmi, ka, kp } by new
                        {
                            hb.HizmetBinasiId,
                            hb.HizmetBinasiAdi,
                            k.KioskId,
                            k.KioskAdi,
                            k.KioskIp,
                            km.KioskMenuId,
                            km.MenuAdi,
                            km.Aciklama,
                            km.MenuSira
                        } into g
                        select new KioskMenuDetayResponseDto
                        {
                            // Hizmet Binası Bilgileri
                            HizmetBinasiId = g.Key.HizmetBinasiId,
                            HizmetBinasiAdi = g.Key.HizmetBinasiAdi,

                            // Kiosk Bilgileri
                            KioskId = g.Key.KioskId,
                            KioskAdi = g.Key.KioskAdi,
                            KioskIp = g.Key.KioskIp,

                            // Menü Bilgileri
                            KioskMenuId = g.Key.KioskMenuId,
                            MenuAdi = g.Key.MenuAdi,
                            MenuAciklama = g.Key.Aciklama,
                            MenuSiraNo = g.Key.MenuSira,

                            // Menü Atama Bilgileri
                            KioskMenuAtamaId = g.Max(x => x.kma.KioskMenuAtamaId),
                            AtamaTarihi = g.Max(x => x.kma.AtamaTarihi),

                            // Aggregate Bilgiler
                            ToplamIslemSayisi = g.Select(x => x.kmi.KioskMenuIslemId).Distinct().Count(),
                            ToplamKanalAltSayisi = g.Select(x => x.ka.KanalAltId).Distinct().Count(),
                            ToplamPersonelSayisi = g.Select(x => x.kp.TcKimlikNo).Distinct().Count()
                        };

            var result = await query
                .AsNoTracking()
                .OrderBy(x => x.KioskAdi)
                .ThenBy(x => x.MenuSiraNo)
                .ToListAsync();

            return result;
        }

        /// <summary>
        /// Belirli bir Kiosk'taki seçilen menü için alt kanal işlemlerini getirir
        /// Sadece aktif personel (Yrd.Uzman+) olan ve banko modunda bulunan işlemler döner
        ///
        /// SQL Karşılığı:
        /// -- Adım 1: Kiosk'un hizmet binası ID'sini al
        /// DECLARE @HizmetBinasiId INT = (SELECT HizmetBinasiId FROM SIR_KioskTanim WHERE KioskId = @kioskId AND SilindiMi = 0)
        /// DECLARE @Bugun DATE = CAST(GETDATE() AS DATE)
        ///
        /// -- Adım 2: Menüdeki alt işlemleri getir (sadece aktif personeli olanlar)
        /// SELECT
        ///     kmi.KioskMenuIslemId,
        ///     kmi.KanalAltId,
        ///     ka.KanalAltAdi,
        ///     k.KanalAdi,
        ///     kmi.MenuSira,
        ///     kai.KanalAltIslemId,
        ///     (SELECT COUNT(*)
        ///      FROM SIR_Siralar s WITH (NOLOCK)
        ///      WHERE s.KanalAltIslemId = kai.KanalAltIslemId
        ///        AND s.HizmetBinasiId = @HizmetBinasiId
        ///        AND s.BeklemeDurum = 0  -- Beklemede
        ///        AND CAST(s.SiraAlisZamani AS DATE) = @Bugun
        ///        AND s.SilindiMi = 0
        ///     ) AS BekleyenSiraSayisi
        /// FROM SIR_KioskMenuIslem kmi WITH (NOLOCK)
        /// INNER JOIN SIR_KanallarAlt ka WITH (NOLOCK)
        ///     ON ka.KanalAltId = kmi.KanalAltId
        /// INNER JOIN SIR_Kanallar k WITH (NOLOCK)
        ///     ON k.KanalId = ka.KanalId
        /// INNER JOIN SIR_KanalAltIslemleri kai WITH (NOLOCK)
        ///     ON kai.KanalAltId = ka.KanalAltId
        ///     AND kai.HizmetBinasiId = @HizmetBinasiId
        /// WHERE kmi.KioskMenuId = @kioskMenuId
        ///   AND kmi.SilindiMi = 0 AND kmi.Aktiflik = 1
        ///   AND ka.SilindiMi = 0 AND ka.Aktiflik = 1
        ///   AND kai.SilindiMi = 0 AND kai.Aktiflik = 1
        ///   -- Aktif personel kontrolü (Yrd.Uzman+ ve ŞU AN banko modunda)
        ///   AND EXISTS (
        ///       SELECT 1
        ///       FROM SIR_KanalPersonelleri kp WITH (NOLOCK)
        ///       INNER JOIN CMN_Users u WITH (NOLOCK)
        ///           ON u.TcKimlikNo = kp.TcKimlikNo
        ///       INNER JOIN SIR_Bankolar b WITH (NOLOCK)
        ///           ON b.BankoId = u.AktifBankoId  -- User'ın ŞU ANKİ aktif bankosu
        ///       WHERE kp.KanalAltIslemId = kai.KanalAltIslemId
        ///         AND kp.Aktiflik = 1 AND kp.SilindiMi = 0
        ///         AND kp.Uzmanlik != 0  -- En az Yrd.Uzman (1,2,3)
        ///         AND u.BankoModuAktif = 1  -- Banko modunda
        ///         AND u.AktifBankoId IS NOT NULL  -- Aktif banko ID'si var
        ///         AND u.AktifMi = 1  -- Kullanıcı aktif
        ///         AND b.HizmetBinasiId = @HizmetBinasiId  -- Aynı hizmet binasında
        ///         AND b.BankoAktiflik = 1 AND b.SilindiMi = 0
        ///   )
        /// ORDER BY kmi.MenuSira
        /// </summary>
        public async Task<List<KioskAltIslemDto>> GetKioskMenuAltIslemleriByKioskIdAsync(int kioskId, int kioskMenuId)
        {
            var today = DateTime.Today;

            // Önce kiosk'un hizmet binası ID'sini al
            var kiosk = await _context.Kiosklar
                .AsNoTracking()
                .Where(k => k.KioskId == kioskId && !k.SilindiMi)
                .Select(k => new { k.HizmetBinasiId })
                .FirstOrDefaultAsync();

            if (kiosk == null)
                return new List<KioskAltIslemDto>();

            var hizmetBinasiId = kiosk.HizmetBinasiId;

            // Ana sorgu: Menüdeki alt işlemleri getir
            var query = from kmi in _context.KioskMenuIslemleri
                        join ka in _context.KanallarAlt on kmi.KanalAltId equals ka.KanalAltId
                        join k in _context.Kanallar on ka.KanalId equals k.KanalId
                        join kai in _context.KanalAltIslemleri on new { ka.KanalAltId, HizmetBinasiId = hizmetBinasiId }
                            equals new { kai.KanalAltId, kai.HizmetBinasiId }
                        where kmi.KioskMenuId == kioskMenuId
                           && !kmi.SilindiMi
                           && kmi.Aktiflik == Aktiflik.Aktif
                           && !ka.SilindiMi
                           && ka.Aktiflik == Aktiflik.Aktif
                           && !kai.SilindiMi
                           && kai.Aktiflik == Aktiflik.Aktif
                        select new
                        {
                            kmi.KioskMenuIslemId,
                            kmi.KanalAltId,
                            ka.KanalAltAdi,
                            k.KanalAdi,
                            kmi.MenuSira,
                            KanalAltIslemId = kai.KanalAltIslemId,
                            HizmetBinasiId = hizmetBinasiId
                        };

            var menuIslemler = await query
                .AsNoTracking()
                .OrderBy(x => x.MenuSira)
                .ToListAsync();

            var result = new List<KioskAltIslemDto>();

            foreach (var islem in menuIslemler)
            {
                // Bu işlem için aktif personel var mı? (Yrd.Uzman+ ve banko modunda)
                var aktifPersonelVar = await (
                    from kp in _context.KanalPersonelleri
                    join u in _context.Users on kp.TcKimlikNo equals u.TcKimlikNo
                    join b in _context.Bankolar on u.AktifBankoId equals b.BankoId
                    where kp.KanalAltIslemId == islem.KanalAltIslemId
                       && kp.Aktiflik == Aktiflik.Aktif
                       && !kp.SilindiMi
                       && kp.Uzmanlik != PersonelUzmanlik.BilgisiYok  // En az Yrd.Uzman
                       && u.BankoModuAktif == true
                       && u.AktifBankoId != null
                       && u.AktifMi == true
                       && b.HizmetBinasiId == hizmetBinasiId
                       && b.BankoAktiflik == Aktiflik.Aktif
                       && !b.SilindiMi
                    select kp.TcKimlikNo
                ).AnyAsync();

                // Sadece aktif personeli olan işlemleri ekle
                if (aktifPersonelVar)
                {
                    // Bekleyen sıra sayısını hesapla
                    var bekleyenSayisi = await _context.Siralar
                        .AsNoTracking()
                        .CountAsync(s => s.KanalAltIslemId == islem.KanalAltIslemId
                                      && s.HizmetBinasiId == hizmetBinasiId
                                      && s.BeklemeDurum == BeklemeDurum.Beklemede
                                      && s.SiraAlisZamani.Date == today
                                      && !s.SilindiMi);

                    result.Add(new KioskAltIslemDto
                    {
                        KioskMenuIslemId = islem.KioskMenuIslemId,
                        KanalAltId = islem.KanalAltId,
                        KanalAltAdi = islem.KanalAltAdi,
                        KanalAdi = islem.KanalAdi,
                        MenuSira = islem.MenuSira,
                        BekleyenSiraSayisi = bekleyenSayisi,
                        AktifPersonelVar = true,
                        TahminiBeklemeSuresi = bekleyenSayisi * 5 // Ortalama 5 dk/sıra
                    });
                }
            }

            return result;
        }
    }
}
