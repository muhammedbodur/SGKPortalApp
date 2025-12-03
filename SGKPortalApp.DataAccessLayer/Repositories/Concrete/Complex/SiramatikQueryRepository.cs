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
            ),
            --  YENİ: Personelin genel şef yetkisi var mı?
            PersonelSefMi AS (
                SELECT CASE WHEN EXISTS (
                    SELECT 1
                    FROM SIR_KanalPersonelleri AS kp WITH (NOLOCK)
                    WHERE kp.TcKimlikNo = @TcKimlikNo
                      AND kp.Aktiflik = 1
                      AND kp.Uzmanlik = 3  -- Şef
                      AND kp.SilindiMi = 0
                ) THEN 1 ELSE 0 END AS Sef
            ),
            --  YENİ: Personelin genel uzman yetkisi var mı?
            PersonelUzmanMi AS (
                SELECT CASE WHEN EXISTS (
                    SELECT 1
                    FROM SIR_KanalPersonelleri AS kp WITH (NOLOCK)
                    WHERE kp.TcKimlikNo = @TcKimlikNo
                      AND kp.Aktiflik = 1
                      AND kp.Uzmanlik = 1  -- Uzman
                      AND kp.SilindiMi = 0
                ) THEN 1 ELSE 0 END AS Uzman
            ),
            --  YENİ: Çağrılmış EN SON sıra ID'si
            SonCagirilanSira AS (
                SELECT TOP 1 s.SiraId
                FROM SIR_Siralar s WITH (NOLOCK)
                WHERE s.TcKimlikNo = @TcKimlikNo
                  AND s.BeklemeDurum = 1  -- Çağrıldı
                  AND s.YonlendirildiMi = 0
                  AND CAST(s.SiraAlisZamani AS DATE) = @Bugun
                  AND s.SilindiMi = 0
                ORDER BY s.SiraNo DESC
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
                pYonlendiren.AdSoyad AS YonlendirenPersonelAdSoyad
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
            CROSS JOIN PersonelSefMi AS psm
            CROSS JOIN PersonelUzmanMi AS pum
            CROSS JOIN SonCagirilanSira AS scs
            WHERE bk.TcKimlikNo = @TcKimlikNo
                AND bk.SilindiMi = 0
                AND uzm.Uzmanlik IN (1, 2, 3)
                AND CAST(s.SiraAlisZamani AS DATE) = @Bugun
                AND (
                    -- 1. Normal Bekleyen Sıralar
                    s.BeklemeDurum = 0

                    -- 2. Çağrılmış EN SON Sıra (Sadece bu personele ait)
                    OR (s.BeklemeDurum = 1 
                        AND s.TcKimlikNo = @TcKimlikNo
                        AND s.YonlendirildiMi = 0
                        AND s.SiraId = scs.SiraId)  --  CTE'den alıyoruz

                    -- 3. Şef'e Yönlendirilmiş  DEĞİŞTİRİLDİ
                    OR (s.BeklemeDurum = 3 
                        AND s.YonlendirildiMi = 1 
                        AND s.YonlendirmeTipi = 2
                        AND s.TcKimlikNo != @TcKimlikNo
                        AND s.HedefBankoId IS NULL 
                        AND psm.Sef = 1)  --  Genel şef yetkisi kontrolü

                    -- 4. Başka Bankoya Yönlendirilmiş
                    OR (s.BeklemeDurum = 3 
                        AND s.YonlendirildiMi = 1 
                        AND s.YonlendirmeTipi = 1 
                        AND s.HedefBankoId = bk.BankoId)

                    -- 5. Genel Uzmana Yönlendirilmiş  DEĞİŞTİRİLDİ
                    OR (s.BeklemeDurum = 3 
                        AND s.YonlendirildiMi = 1 
                        AND s.YonlendirmeTipi = 3 
                        AND s.HedefBankoId IS NULL 
                        AND pum.Uzman = 1)  
                )
            ORDER BY 
                s.BeklemeDurum DESC,      
                s.YonlendirildiMi DESC,   
                uzm.Uzmanlik ASC,         
                s.SiraNo ASC;             
            */
            var today = DateTime.Today;

            // ADIM 1: Personelin kanal alt işlem ID'lerini al (IN clause için)
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

            //  YENİ: Personelin GENEL şef yetkisi var mı?
            var personelSefMi = await _context.KanalPersonelleri
                .AnyAsync(kp => kp.TcKimlikNo == tcKimlikNo
                             && kp.Aktiflik == Aktiflik.Aktif
                             && kp.Uzmanlik == PersonelUzmanlik.Sef
                             && !kp.SilindiMi);

            //  YENİ: Personelin GENEL uzman yetkisi var mı?
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
                   && !s.YonlendirildiMi
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

                                // 2. Çağrılmış EN SON Sıra
                                || (s.BeklemeDurum == BeklemeDurum.Cagrildi
                                    && s.TcKimlikNo == tcKimlikNo
                                    && !s.YonlendirildiMi
                                    && s.SiraId == sonCagirilanSiraId)

                                // 3. Şef'e Yönlendirilmiş  DEĞİŞTİRİLDİ
                                || (s.BeklemeDurum == BeklemeDurum.Yonlendirildi
                                    && s.YonlendirildiMi
                                    && s.YonlendirmeTipi == YonlendirmeTipi.Sef
                                    && s.TcKimlikNo != tcKimlikNo
                                    && s.HedefBankoId == null
                                    && personelSefMi)  //  Önceden hesaplanmış değer

                                // 4. Başka Bankoya Yönlendirilmiş
                                || (s.BeklemeDurum == BeklemeDurum.Yonlendirildi
                                    && s.YonlendirildiMi
                                    && s.YonlendirmeTipi == YonlendirmeTipi.BaskaBanko
                                    && s.TcKimlikNo != tcKimlikNo
                                    && s.HedefBankoId == bk.BankoId)

                                // 5. Genel Uzmana Yönlendirilmiş  DEĞİŞTİRİLDİ
                                || (s.BeklemeDurum == BeklemeDurum.Yonlendirildi
                                    && s.YonlendirildiMi
                                    && s.YonlendirmeTipi == YonlendirmeTipi.UzmanPersonel
                                    && s.TcKimlikNo != tcKimlikNo
                                    && s.HedefBankoId == null
                                    && personelUzmanMi)  //  Önceden hesaplanmış değer
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
                            YonlendirenPersonelAdSoyad = pYonlendiren != null ? pYonlendiren.AdSoyad : null
                        };

            var rawResult = await query
                .AsNoTracking()
                .Distinct()
                .OrderByDescending(x => x.BeklemeDurum)
                .ThenByDescending(x => x.YonlendirildiMi)
                .ThenBy(x => x.Uzmanlik)
                .ThenBy(x => x.SiraNo)
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

            // Local function
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

    }
}
