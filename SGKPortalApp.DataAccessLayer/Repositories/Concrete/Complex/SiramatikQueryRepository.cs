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
                                .Count(kp => kp.KanalAltIslemId == kai.KanalAltIslemId 
                                          && kp.Aktiflik == BusinessObjectLayer.Enums.Common.Aktiflik.Aktif
                                          && !kp.SilindiMi)
                        };

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<List<KanalAltIslemResponseDto>> GetKanalAltIslemlerByDepartmanHizmetBinasiIdAsync(int departmanHizmetBinasiId)
        {
            var query = from kai in _context.KanalAltIslemleri
                        join ka in _context.KanallarAlt on kai.KanalAltId equals ka.KanalAltId
                        join k in _context.Kanallar on ka.KanalId equals k.KanalId
                        join ki in _context.KanalIslemleri on kai.KanalIslemId equals ki.KanalIslemId
                        join dhb in _context.DepartmanHizmetBinalari on kai.DepartmanHizmetBinasiId equals dhb.DepartmanHizmetBinasiId
                        join hb in _context.HizmetBinalari on dhb.HizmetBinasiId equals hb.HizmetBinasiId
                        where kai.DepartmanHizmetBinasiId == departmanHizmetBinasiId
                              && !kai.SilindiMi
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
                                .Count(kp => kp.KanalAltIslemId == kai.KanalAltIslemId 
                                          && kp.Aktiflik == BusinessObjectLayer.Enums.Common.Aktiflik.Aktif
                                          && !kp.SilindiMi
                                          && kp.Personel != null && kp.Personel.HizmetBinasiId == dhb.HizmetBinasiId
                                          && kp.Personel.DepartmanId == dhb.DepartmanId),
                            SefSayisi = _context.KanalPersonelleri
                                .Count(kp => kp.KanalAltIslemId == kai.KanalAltIslemId 
                                          && kp.Aktiflik == BusinessObjectLayer.Enums.Common.Aktiflik.Aktif
                                          && !kp.SilindiMi
                                          && kp.Personel != null && kp.Personel.HizmetBinasiId == dhb.HizmetBinasiId
                                          && kp.Personel.DepartmanId == dhb.DepartmanId
                                          && kp.Uzmanlik == BusinessObjectLayer.Enums.SiramatikIslemleri.PersonelUzmanlik.Sef),
                            UzmanSayisi = _context.KanalPersonelleri
                                .Count(kp => kp.KanalAltIslemId == kai.KanalAltIslemId 
                                          && kp.Aktiflik == BusinessObjectLayer.Enums.Common.Aktiflik.Aktif
                                          && !kp.SilindiMi
                                          && kp.Personel != null && kp.Personel.HizmetBinasiId == dhb.HizmetBinasiId
                                          && kp.Personel.DepartmanId == dhb.DepartmanId
                                          && kp.Uzmanlik == BusinessObjectLayer.Enums.SiramatikIslemleri.PersonelUzmanlik.Uzman),
                            YrdUzmanSayisi = _context.KanalPersonelleri
                                .Count(kp => kp.KanalAltIslemId == kai.KanalAltIslemId 
                                          && kp.Aktiflik == BusinessObjectLayer.Enums.Common.Aktiflik.Aktif
                                          && !kp.SilindiMi
                                          && kp.Personel != null && kp.Personel.HizmetBinasiId == dhb.HizmetBinasiId
                                          && kp.Personel.DepartmanId == dhb.DepartmanId
                                          && kp.Uzmanlik == BusinessObjectLayer.Enums.SiramatikIslemleri.PersonelUzmanlik.YrdUzman)
                        };

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<KanalAltIslemResponseDto?> GetKanalAltIslemByIdWithDetailsAsync(int kanalAltIslemId)
        {
            var query = from kai in _context.KanalAltIslemleri
                        join ka in _context.KanallarAlt on kai.KanalAltId equals ka.KanalAltId
                        join k in _context.Kanallar on ka.KanalId equals k.KanalId
                        join ki in _context.KanalIslemleri on kai.KanalIslemId equals ki.KanalIslemId
                        join dhb in _context.DepartmanHizmetBinalari on kai.DepartmanHizmetBinasiId equals dhb.DepartmanHizmetBinasiId
                        join hb in _context.HizmetBinalari on dhb.HizmetBinasiId equals hb.HizmetBinasiId
                        where kai.KanalAltIslemId == kanalAltIslemId
                           && !kai.SilindiMi
                        select new KanalAltIslemResponseDto
                        {
                            KanalAltIslemId = kai.KanalAltIslemId,
                            KanalAltId = kai.KanalAltId,
                            KanalAltAdi = ka.KanalAltAdi,
                            HizmetBinasiId = dhb.HizmetBinasiId,
                            HizmetBinasiAdi = hb.HizmetBinasiAdi,
                            KanalIslemId = kai.KanalIslemId,
                            KanalAdi = k.KanalAdi,
                            Aktiflik = kai.Aktiflik,
                            EklenmeTarihi = kai.EklenmeTarihi,
                            DuzenlenmeTarihi = kai.DuzenlenmeTarihi,
                            PersonelSayisi = _context.KanalPersonelleri
                                .Count(kp => kp.KanalAltIslemId == kai.KanalAltIslemId
                                          && kp.Aktiflik == BusinessObjectLayer.Enums.Common.Aktiflik.Aktif
                                          && !kp.SilindiMi),
                            SefSayisi = _context.KanalPersonelleri
                                .Count(kp => kp.KanalAltIslemId == kai.KanalAltIslemId 
                                          && kp.Aktiflik == BusinessObjectLayer.Enums.Common.Aktiflik.Aktif
                                          && !kp.SilindiMi
                                          && kp.Uzmanlik == BusinessObjectLayer.Enums.SiramatikIslemleri.PersonelUzmanlik.Sef),
                            UzmanSayisi = _context.KanalPersonelleri
                                .Count(kp => kp.KanalAltIslemId == kai.KanalAltIslemId 
                                          && kp.Aktiflik == BusinessObjectLayer.Enums.Common.Aktiflik.Aktif
                                          && !kp.SilindiMi
                                          && kp.Uzmanlik == BusinessObjectLayer.Enums.SiramatikIslemleri.PersonelUzmanlik.Uzman),
                            YrdUzmanSayisi = _context.KanalPersonelleri
                                .Count(kp => kp.KanalAltIslemId == kai.KanalAltIslemId 
                                          && kp.Aktiflik == BusinessObjectLayer.Enums.Common.Aktiflik.Aktif
                                          && !kp.SilindiMi
                                          && kp.Uzmanlik == BusinessObjectLayer.Enums.SiramatikIslemleri.PersonelUzmanlik.YrdUzman)
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
                                .Count(kp => kp.KanalAltIslemId == kai.KanalAltIslemId 
                                          && kp.Aktiflik == BusinessObjectLayer.Enums.Common.Aktiflik.Aktif
                                          && !kp.SilindiMi)
                        };

            return await query.AsNoTracking().ToListAsync();
        }

        // ═══════════════════════════════════════════════════════
        // KANAL İŞLEM SORGULARI
        // ═══════════════════════════════════════════════════════

        public async Task<List<KanalIslemResponseDto>> GetKanalIslemlerByDepartmanHizmetBinasiIdAsync(int departmanHizmetBinasiId)
        {
            var query = from ki in _context.KanalIslemleri
                        join k in _context.Kanallar on ki.KanalId equals k.KanalId
                        join dhb in _context.DepartmanHizmetBinalari on ki.DepartmanHizmetBinasiId equals dhb.DepartmanHizmetBinasiId
                        where ki.DepartmanHizmetBinasiId == departmanHizmetBinasiId
                              && !ki.SilindiMi
                        select new KanalIslemResponseDto
                        {
                            KanalIslemId = ki.KanalIslemId,
                            KanalId = ki.KanalId,
                            KanalAdi = k.KanalAdi,
                            HizmetBinasiId = dhb.HizmetBinasiId,
                            Sira = ki.Sira,
                            BaslangicNumara = ki.BaslangicNumara,
                            BitisNumara = ki.BitisNumara,
                            Aktiflik = ki.Aktiflik,
                            EklenmeTarihi = ki.EklenmeTarihi,
                            DuzenlenmeTarihi = ki.DuzenlenmeTarihi,
                            KanalAltIslemSayisi = _context.KanalAltIslemleri
                                .Count(x => x.KanalIslemId == ki.KanalIslemId 
                                         && x.DepartmanHizmetBinasiId == departmanHizmetBinasiId
                                         && !x.SilindiMi 
                                         && x.Aktiflik == BusinessObjectLayer.Enums.Common.Aktiflik.Aktif
                                         && x.KanalAlt != null 
                                         && !x.KanalAlt.SilindiMi 
                                         && x.KanalAlt.Aktiflik == BusinessObjectLayer.Enums.Common.Aktiflik.Aktif)
                        };

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<KanalIslemResponseDto?> GetKanalIslemByIdWithDetailsAsync(int kanalIslemId)
        {
            var query = from ki in _context.KanalIslemleri
                        join k in _context.Kanallar on ki.KanalId equals k.KanalId
                        join dhb in _context.DepartmanHizmetBinalari on ki.DepartmanHizmetBinasiId equals dhb.DepartmanHizmetBinasiId
                        where ki.KanalIslemId == kanalIslemId
                              && !ki.SilindiMi
                        select new KanalIslemResponseDto
                        {
                            KanalIslemId = ki.KanalIslemId,
                            KanalId = ki.KanalId,
                            KanalAdi = k.KanalAdi,
                            HizmetBinasiId = dhb.HizmetBinasiId,
                            Sira = ki.Sira,
                            BaslangicNumara = ki.BaslangicNumara,
                            BitisNumara = ki.BitisNumara,
                            Aktiflik = ki.Aktiflik,
                            EklenmeTarihi = ki.EklenmeTarihi,
                            DuzenlenmeTarihi = ki.DuzenlenmeTarihi,
                            KanalAltIslemSayisi = _context.KanalAltIslemleri
                                .Count(x => x.KanalIslemId == ki.KanalIslemId 
                                         && !x.SilindiMi 
                                         && x.Aktiflik == BusinessObjectLayer.Enums.Common.Aktiflik.Aktif
                                         && x.KanalAlt != null 
                                         && !x.KanalAlt.SilindiMi 
                                         && x.KanalAlt.Aktiflik == BusinessObjectLayer.Enums.Common.Aktiflik.Aktif)
                        };

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        // ═══════════════════════════════════════════════════════
        // KANAL PERSONEL SORGULARI
        // ═══════════════════════════════════════════════════════

        public async Task<List<KanalPersonelResponseDto>> GetKanalPersonellerByDepartmanHizmetBinasiIdAsync(int departmanHizmetBinasiId)
        {
            var query = from kp in _context.KanalPersonelleri
                        join p in _context.Personeller on kp.TcKimlikNo equals p.TcKimlikNo
                        join kai in _context.KanalAltIslemleri on kp.KanalAltIslemId equals kai.KanalAltIslemId
                        join ka in _context.KanallarAlt on kai.KanalAltId equals ka.KanalAltId
                        where kai.DepartmanHizmetBinasiId == departmanHizmetBinasiId
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

        public async Task<Dictionary<int, int>> GetKanalAltIslemPersonelSayilariAsync(int departmanHizmetBinasiId)
        {
            var query = from kai in _context.KanalAltIslemleri
                        where kai.DepartmanHizmetBinasiId == departmanHizmetBinasiId
                        select new
                        {
                            KanalAltIslemId = kai.KanalAltIslemId,
                            PersonelSayisi = _context.KanalPersonelleri
                                .Count(kp => kp.KanalAltIslemId == kai.KanalAltIslemId 
                                          && kp.Aktiflik == BusinessObjectLayer.Enums.Common.Aktiflik.Aktif
                                          && !kp.SilindiMi)
                        };

            var result = await query.AsNoTracking().ToListAsync();
            return result.ToDictionary(x => x.KanalAltIslemId, x => x.PersonelSayisi);
        }

        public async Task<List<KanalAltIslemResponseDto>> GetEslestirmeYapilmamisKanalAltIslemlerAsync(int departmanHizmetBinasiId)
        {
            var query = from kai in _context.KanalAltIslemleri
                        join ka in _context.KanallarAlt on kai.KanalAltId equals ka.KanalAltId
                        join k in _context.Kanallar on ka.KanalId equals k.KanalId
                        join ki in _context.KanalIslemleri on kai.KanalIslemId equals ki.KanalIslemId
                        where kai.DepartmanHizmetBinasiId == departmanHizmetBinasiId &&
                              !_context.KanalPersonelleri.Any(kp => kp.KanalAltIslemId == kai.KanalAltIslemId 
                                                                && kp.Aktiflik == BusinessObjectLayer.Enums.Common.Aktiflik.Aktif
                                                                && !kp.SilindiMi)
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

        public async Task<List<KanalPersonelResponseDto>> GetPersonelKanalAtamalarByDepartmanHizmetBinasiIdAsync(int departmanHizmetBinasiId)
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
                        join dhb in _context.DepartmanHizmetBinalari
                            on kai.DepartmanHizmetBinasiId equals dhb.DepartmanHizmetBinasiId into dhbGroup
                        from dhb in dhbGroup.DefaultIfEmpty()
                        where (kai == null || kai.DepartmanHizmetBinasiId == departmanHizmetBinasiId)
                           && (dhb == null || dhb.DepartmanHizmetBinasiId == departmanHizmetBinasiId)
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

        public async Task<List<PersonelAtamaMatrixDto>> GetPersonelAtamaMatrixByDepartmanHizmetBinasiIdAsync(int departmanHizmetBinasiId)
        {
            // 1. Departman-Hizmet binasındaki aktif personelleri getir
            var dhb = await _context.DepartmanHizmetBinalari
                .FirstOrDefaultAsync(x => x.DepartmanHizmetBinasiId == departmanHizmetBinasiId);
            
            if (dhb == null) return new List<PersonelAtamaMatrixDto>();
            
            var personelQuery = from p in _context.Personeller
                                where p.HizmetBinasiId == dhb.HizmetBinasiId
                                   && p.DepartmanId == dhb.DepartmanId
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
                                && kai.DepartmanHizmetBinasiId == departmanHizmetBinasiId
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
                AND b.Aktiflik = 1
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
                                AND kp2.KanalAltIslemId = s.KanalAltIslemId  -- ⭐ Sıranın kanalında
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
                                AND kp2.KanalAltIslemId = s.KanalAltIslemId  -- ⭐ Sıranın kanalında
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
                        join dhb in _context.DepartmanHizmetBinalari on bk.DepartmanHizmetBinasiId equals dhb.DepartmanHizmetBinasiId
                        join hb in _context.HizmetBinalari on dhb.HizmetBinasiId equals hb.HizmetBinasiId

                        // ⭐ YENİ: HubConnection ve HubBankoConnection JOIN'leri
                        join hc in _context.HubConnections on u.TcKimlikNo equals hc.TcKimlikNo
                        join hbc in _context.HubBankoConnections on hc.HubConnectionId equals hbc.HubConnectionId

                        join kai in _context.KanalAltIslemleri on true equals true
                        join kp in _context.KanalPersonelleri on new { kai.KanalAltIslemId, TcKimlikNo = tcKimlikNo }
                            equals new { kp.KanalAltIslemId, kp.TcKimlikNo }
                        join s in _context.Siralar on kai.KanalAltIslemId equals s.KanalAltIslemId
                        join pYonlendiren in _context.Personeller on s.YonlendirenPersonelTc equals pYonlendiren.TcKimlikNo into yonlendirenGroup
                        from pYonlendiren in yonlendirenGroup.DefaultIfEmpty()

                        // ⭐ YENİ: Şef yönlendirmesi için kanal bazlı uzm JOIN
                        join uzm in _context.KanalPersonelleri on new { s.KanalAltIslemId, TcKimlikNo = tcKimlikNo }
                            equals new { uzm.KanalAltIslemId, uzm.TcKimlikNo } into uzmGroup
                        from uzm in uzmGroup.DefaultIfEmpty()

                        where bk.TcKimlikNo == tcKimlikNo
                           && !bk.SilindiMi
                           && b.Aktiflik == Aktiflik.Aktif
                           && !b.SilindiMi
                           && p.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif
                           && p.HizmetBinasiId == dhb.HizmetBinasiId
                           && !p.SilindiMi
                           && u.BankoModuAktif
                           && u.AktifMi

                           // ⭐ YENİ: HubConnection filtreleri
                           && hc.ConnectionStatus == ConnectionStatus.online
                           && hc.ConnectionType == "BankoMode"
                           && !hc.SilindiMi
                           && hbc.BankoModuAktif
                           && !hbc.SilindiMi

                           && personelKanalAltIslemIds.Contains(kai.KanalAltIslemId)
                           && kai.Aktiflik == Aktiflik.Aktif
                           && !kai.SilindiMi
                           && kp.Aktiflik == Aktiflik.Aktif
                           && !kp.SilindiMi
                           && kp.Uzmanlik != PersonelUzmanlik.BilgisiYok
                           && s.DepartmanHizmetBinasiId == bk.DepartmanHizmetBinasiId
                           && !s.SilindiMi
                           && s.SiraAlisZamani.Date == today
                           && (
                                // 1. Normal Bekleyen Sıralar
                                s.BeklemeDurum == BeklemeDurum.Beklemede

                                // 2. Çağrılmış EN SON Sıra
                                || (s.BeklemeDurum == BeklemeDurum.Cagrildi
                                    && s.TcKimlikNo == tcKimlikNo
                                    && s.SiraId == sonCagirilanSiraId)

                                // 3. Şef'e Yönlendirilmiş
                                // ⭐ YENİ: Kanal bazlı kontrol - personelin O kanalda Şef olması gerekiyor
                                || (s.BeklemeDurum == BeklemeDurum.Yonlendirildi
                                    && s.YonlendirildiMi
                                    && s.YonlendirmeTipi == YonlendirmeTipi.Sef
                                    && s.TcKimlikNo != tcKimlikNo
                                    && s.HedefBankoId == null
                                    && uzm.Uzmanlik == PersonelUzmanlik.Sef
                                    && uzm.KanalAltIslemId == s.KanalAltIslemId)

                                // 4. Başka Bankoya Yönlendirilmiş
                                || (s.BeklemeDurum == BeklemeDurum.Yonlendirildi
                                    && s.YonlendirildiMi
                                    && s.YonlendirmeTipi == YonlendirmeTipi.BaskaBanko
                                    && s.TcKimlikNo != tcKimlikNo
                                    && s.HedefBankoId == bk.BankoId)

                                // 5. Genel Uzmana Yönlendirilmiş
                                // ⭐ YENİ: Kanal bazlı kontrol - personelin O kanalda Uzman olması gerekiyor
                                || (s.BeklemeDurum == BeklemeDurum.Yonlendirildi
                                    && s.YonlendirildiMi
                                    && s.YonlendirmeTipi == YonlendirmeTipi.UzmanPersonel
                                    && s.TcKimlikNo != tcKimlikNo
                                    && s.HedefBankoId == null
                                    && uzm.Uzmanlik == PersonelUzmanlik.Uzman
                                    && uzm.KanalAltIslemId == s.KanalAltIslemId)
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

        /// <summary>
        /// ⭐ Personelin ilk çağrılabilir sırasını getirir (sadece tek sıra - performans için)
        /// Çağrılabilir: BeklemeDurum = Beklemede veya Yonlendirildi
        /// </summary>
        public async Task<SiraCagirmaResponseDto?> GetIlkCagrilabilirSiraAsync(string tcKimlikNo)
        {
            // GetBankoPanelBekleyenSiralarAsync metodunu kullan ve sadece ilk çağrılabilir sırayı döndür
            var siralar = await GetBankoPanelBekleyenSiralarAsync(tcKimlikNo);
            
            // İlk çağrılabilir sıra: Yönlendirildi veya Beklemede durumunda olan
            return siralar.FirstOrDefault(s => 
                s.BeklemeDurum == BeklemeDurum.Yonlendirildi || 
                s.BeklemeDurum == BeklemeDurum.Beklemede);
        }

        /// <summary>
        /// ⭐ INCREMENTAL UPDATE: Belirli bir sıra alındığında/yönlendirildiğinde,
        /// o sırayı görebilecek TÜM personellerin güncel sıra listelerini getirir.
        /// Her satırda PersonelTc ve ConnectionId bilgisi yer alır.
        /// SignalR ile her personele kendi listesi gönderilir.
        /// </summary>
        public async Task<List<SiraCagirmaResponseDto>> GetBankoPanelBekleyenSiralarBySiraIdAsync(int siraId)
        {
            /*
            DECLARE @VerilmeSiraId INT = 66;
            DECLARE @Bugun DATE = CAST(GETDATE() AS DATE);

            -- ADIM 1: Verilen sıranın bilgilerini al
            DECLARE @SiraKanalAltIslemId INT;
            DECLARE @SiraHizmetBinasiId INT;
            DECLARE @SiraYonlendirmeTipi INT;

            SELECT 
                @SiraKanalAltIslemId = s.KanalAltIslemId,
                @SiraHizmetBinasiId = s.HizmetBinasiId,
                @SiraYonlendirmeTipi = s.YonlendirmeTipi
            FROM SIR_Siralar s WITH (NOLOCK)
            WHERE s.SiraId = @VerilmeSiraId;

            -- ADIM 2: Bu sırayı alabilecek tüm personelleri bul
            ;WITH HedefPersoneller AS (
                SELECT DISTINCT kp.TcKimlikNo
                FROM SIR_KanalPersonelleri kp WITH (NOLOCK)
                WHERE kp.KanalAltIslemId = @SiraKanalAltIslemId
                    AND kp.Aktiflik = 1
                    AND kp.SilindiMi = 0
                    AND (
                        -- Normal personeller (kanal uzmanlığı olan)
                        kp.Uzmanlik IN (1, 2, 3)
            
                        -- Veya yönlendirme tipine göre ek kontroller
                        OR (
                            @SiraYonlendirmeTipi = 2  -- Şef'e yönlendirildiyse
                            AND kp.Uzmanlik = 3
                        )
                        OR (
                            @SiraYonlendirmeTipi = 3  -- Uzman personele yönlendirildiyse
                            AND kp.Uzmanlik = 2
                        )
                    )
            )
            -- ⭐ PersonelUzmanlikKayitlari CTE kaldırıldı - artık doğrudan sıranın kanalı üzerinden JOIN yapılıyor

            -- ADIM 3: Tüm hedef personellerin sıralarını getir
            SELECT DISTINCT
                hp.TcKimlikNo AS PersonelTc,
                pPersonel.AdSoyad AS PersonelAdSoyad,
                s.SiraId,
                s.SiraNo,
		            hc.ConnectionId,
                s.KanalAltAdi,
                s.SiraAlisZamani,
                s.IslemBaslamaZamani,
                s.BeklemeDurum,
                kp.Uzmanlik,  -- ⭐ uzm yerine kp (sıranın kanalındaki uzmanlık)
                b.BankoId,
                hb.HizmetBinasiId,
                hb.HizmetBinasiAdi,
                s.YonlendirildiMi,
                s.YonlendirmeTipi,
                s.HedefBankoId,
                s.YonlendirenBankoId,
                s.YonlendirenPersonelTc,
                pYonlendiren.AdSoyad AS YonlendirenPersonelAdSoyad,
                -- Öncelik hesaplamaları
                CASE 
                    WHEN s.BeklemeDurum = 1 THEN 0
                    WHEN s.BeklemeDurum = 3 THEN 1
                    ELSE 2
                END AS DurumOnceligi,
                CASE 
                    WHEN s.BeklemeDurum = 0 THEN 
                        CASE kp.Uzmanlik  -- ⭐ uzm yerine kp
                            WHEN 3 THEN 0
                            WHEN 2 THEN 1
                            WHEN 1 THEN 2
                            ELSE 3
                        END
                    ELSE 99
                END AS UzmanlikOnceligi
            FROM HedefPersoneller hp
            INNER JOIN PER_Personeller pPersonel WITH (NOLOCK)
                ON pPersonel.TcKimlikNo = hp.TcKimlikNo
                AND pPersonel.PersonelAktiflikDurum = 1
                AND pPersonel.SilindiMi = 0
            INNER JOIN SIR_BankoKullanicilari bk WITH (NOLOCK)
                ON bk.TcKimlikNo = hp.TcKimlikNo
                AND bk.SilindiMi = 0
            INNER JOIN SIR_Bankolar b WITH (NOLOCK)
                ON b.BankoId = bk.BankoId
                AND b.Aktiflik = 1
                AND b.SilindiMi = 0
                AND b.HizmetBinasiId = @SiraHizmetBinasiId
            INNER JOIN CMN_Users u WITH (NOLOCK)
                ON u.TcKimlikNo = hp.TcKimlikNo
                AND u.BankoModuAktif = 1
                AND u.AktifMi = 1
            INNER JOIN CMN_HubConnections AS hc WITH (NOLOCK)
		            ON hc.TcKimlikNo = u.TcKimlikNo AND hc.ConnectionStatus = 'online' AND hc.SilindiMi = 0
            INNER JOIN SIR_HubBankoConnections AS hbc  WITH (NOLOCK)
		            ON  hbc.HubConnectionId = hc.HubConnectionId
            INNER JOIN CMN_HizmetBinalari hb WITH (NOLOCK)
                ON hb.HizmetBinasiId = bk.HizmetBinasiId
            -- ⭐ Önce sıraları al, sonra o sıranın kanalında personelin uzmanlığını bul
            INNER JOIN SIR_Siralar s WITH (NOLOCK)
                ON s.HizmetBinasiId = bk.HizmetBinasiId
                AND s.SilindiMi = 0
            -- ⭐ Personelin O SIRANIN KANALINDA uzmanlığını bul (kritik JOIN)
            INNER JOIN SIR_KanalPersonelleri kp WITH (NOLOCK)
                ON kp.TcKimlikNo = hp.TcKimlikNo
                AND kp.KanalAltIslemId = s.KanalAltIslemId  -- ⭐ Sıranın kanalı ile eşleştir
                AND kp.Aktiflik = 1
                AND kp.Uzmanlik != 0
                AND kp.SilindiMi = 0
            INNER JOIN SIR_KanalAltIslemleri kai WITH (NOLOCK)
                ON kai.KanalAltIslemId = s.KanalAltIslemId
                AND kai.Aktiflik = 1
                AND kai.SilindiMi = 0
            LEFT JOIN PER_Personeller pYonlendiren WITH (NOLOCK)
                ON pYonlendiren.TcKimlikNo = s.YonlendirenPersonelTc
                AND pYonlendiren.SilindiMi = 0

            WHERE CAST(s.SiraAlisZamani AS DATE) = @Bugun
                AND kp.Uzmanlik IN (1, 2, 3)  -- ⭐ uzm yerine kp
                AND (
                    -- 1. Normal Bekleyen Sıralar
                    s.BeklemeDurum = 0
        
                    -- 2. Çağrılmış EN SON Sıra (bu personele ait)
                    OR (s.BeklemeDurum = 1 
                        AND s.TcKimlikNo = hp.TcKimlikNo
                        AND s.SiraId = (
                            SELECT TOP 1 s2.SiraId
                            FROM SIR_Siralar s2 WITH (NOLOCK)
                            WHERE s2.TcKimlikNo = hp.TcKimlikNo
                                AND s2.BeklemeDurum = 1
                                AND CAST(s2.SiraAlisZamani AS DATE) = @Bugun
                                AND s2.SilindiMi = 0
                            ORDER BY s2.SiraNo DESC
                        ))
        
                    -- 3. Şef'e Yönlendirilmiş
                    -- ⭐ kp.KanalAltIslemId = s.KanalAltIslemId JOIN'de zaten sağlanıyor
                    OR (s.BeklemeDurum = 3 
                        AND s.YonlendirildiMi = 1 
                        AND s.YonlendirmeTipi = 2
                        AND s.TcKimlikNo != hp.TcKimlikNo 
                        AND s.HedefBankoId IS NULL 
                        AND kp.Uzmanlik = 3)  -- ⭐ uzm yerine kp
        
                    -- 4. Başka Bankoya Yönlendirilmiş
                    OR (s.BeklemeDurum = 3 
                        AND s.YonlendirildiMi = 1 
                        AND s.YonlendirmeTipi = 1
                        AND s.TcKimlikNo != hp.TcKimlikNo
                        AND s.HedefBankoId = bk.BankoId)
        
                    -- 5. Genel Uzmana Yönlendirilmiş
                    -- ⭐ kp.KanalAltIslemId = s.KanalAltIslemId JOIN'de zaten sağlanıyor
                    OR (s.BeklemeDurum = 3 
                        AND s.YonlendirildiMi = 1 
                        AND s.YonlendirmeTipi = 3
                        AND s.TcKimlikNo != hp.TcKimlikNo
                        AND s.HedefBankoId IS NULL 
                        AND kp.Uzmanlik = 2)  -- ⭐ uzm yerine kp
                )
            ORDER BY
                hp.TcKimlikNo ASC,
                DurumOnceligi ASC,
                UzmanlikOnceligi ASC,
                s.SiraAlisZamani ASC,
		            s.SiraNo ASC;
            */
            var today = DateTime.Today;

            // ADIM 1: Verilen sıranın bilgilerini al
            var siraInfo = await (
                from s in _context.Siralar
                where s.SiraId == siraId && !s.SilindiMi
                select new
                {
                    s.KanalAltIslemId,
                    s.DepartmanHizmetBinasiId,
                    s.YonlendirmeTipi
                }
            ).FirstOrDefaultAsync();

            if (siraInfo == null)
                return new List<SiraCagirmaResponseDto>();

            // ADIM 2: Bu sırayı alabilecek tüm personelleri bul (HedefPersoneller CTE)
            var hedefPersonellerQuery = from kp in _context.KanalPersonelleri
                                        where kp.KanalAltIslemId == siraInfo.KanalAltIslemId
                                           && kp.Aktiflik == Aktiflik.Aktif
                                           && !kp.SilindiMi
                                           && (
                                               // Normal personeller (kanal uzmanlığı olan)
                                               kp.Uzmanlik == PersonelUzmanlik.YrdUzman
                                               || kp.Uzmanlik == PersonelUzmanlik.Uzman
                                               || kp.Uzmanlik == PersonelUzmanlik.Sef
                                               // Veya yönlendirme tipine göre ek kontroller
                                               || (siraInfo.YonlendirmeTipi == YonlendirmeTipi.Sef && kp.Uzmanlik == PersonelUzmanlik.Sef)
                                               || (siraInfo.YonlendirmeTipi == YonlendirmeTipi.UzmanPersonel && kp.Uzmanlik == PersonelUzmanlik.Uzman)
                                           )
                                        select kp.TcKimlikNo;

            var hedefPersoneller = await hedefPersonellerQuery.Distinct().ToListAsync();

            if (!hedefPersoneller.Any())
                return new List<SiraCagirmaResponseDto>();

            // Ana sorgu - tamamen veritabanında çalışacak şekilde
            var query = from pPersonel in _context.Personeller
                        where hedefPersoneller.Contains(pPersonel.TcKimlikNo)
                        join bk in _context.BankoKullanicilari on pPersonel.TcKimlikNo equals bk.TcKimlikNo
                        join b in _context.Bankolar on bk.BankoId equals b.BankoId
                        join u in _context.Users on pPersonel.TcKimlikNo equals u.TcKimlikNo
                        join hc in _context.HubConnections on u.TcKimlikNo equals hc.TcKimlikNo
                        join hbc in _context.HubBankoConnections on hc.HubConnectionId equals hbc.HubConnectionId
                        join dhb in _context.DepartmanHizmetBinalari on bk.DepartmanHizmetBinasiId equals dhb.DepartmanHizmetBinasiId
                        join hb in _context.HizmetBinalari on dhb.HizmetBinasiId equals hb.HizmetBinasiId
                        join s in _context.Siralar on bk.DepartmanHizmetBinasiId equals s.DepartmanHizmetBinasiId
                        join kp in _context.KanalPersonelleri on new { TcKimlikNo = pPersonel.TcKimlikNo, s.KanalAltIslemId } equals new { kp.TcKimlikNo, kp.KanalAltIslemId }
                        join kai in _context.KanalAltIslemleri on s.KanalAltIslemId equals kai.KanalAltIslemId
                        join pYonlendiren in _context.Personeller on s.YonlendirenPersonelTc equals pYonlendiren.TcKimlikNo into yonGroup
                        from pYonlendiren in yonGroup.DefaultIfEmpty()
                        let hp = pPersonel.TcKimlikNo
                        let uzm = kp
                        where pPersonel.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif
                           && !pPersonel.SilindiMi
                           && !bk.SilindiMi
                           && b.Aktiflik == Aktiflik.Aktif
                           && !b.SilindiMi
                           && b.DepartmanHizmetBinasiId == siraInfo.DepartmanHizmetBinasiId
                           && u.BankoModuAktif
                           && u.AktifMi
                           && hc.ConnectionStatus == ConnectionStatus.online
                           && hc.ConnectionType == "BankoMode" 
                           && !hc.SilindiMi
                           && hbc.BankoModuAktif 
                           && !hbc.SilindiMi 
                           && kp.Aktiflik == Aktiflik.Aktif
                           && !kp.SilindiMi
                           && kp.Uzmanlik != PersonelUzmanlik.BilgisiYok
                           && kai.Aktiflik == Aktiflik.Aktif
                           && !kai.SilindiMi
                           && s.DepartmanHizmetBinasiId == bk.DepartmanHizmetBinasiId
                           && !s.SilindiMi
                           && s.SiraAlisZamani.Date == today
                           && (
                                // 1. Normal Bekleyen Sıralar
                                s.BeklemeDurum == BeklemeDurum.Beklemede

                                // 2. Çağrılmış EN SON Sıra (bu personele ait)
                                || (s.BeklemeDurum == BeklemeDurum.Cagrildi
                                    && s.TcKimlikNo == hp)

                                // 3. Şef'e Yönlendirilmiş
                                // ⭐ JOIN'de kp.KanalAltIslemId == s.KanalAltIslemId eşleşmesi zaten yapılıyor
                                || (s.BeklemeDurum == BeklemeDurum.Yonlendirildi
                                    && s.YonlendirildiMi
                                    && s.YonlendirmeTipi == YonlendirmeTipi.Sef
                                    && s.TcKimlikNo != hp
                                    && s.HedefBankoId == null
                                    && uzm.Uzmanlik == PersonelUzmanlik.Sef)

                                // 4. Başka Bankoya Yönlendirilmiş
                                || (s.BeklemeDurum == BeklemeDurum.Yonlendirildi
                                    && s.YonlendirildiMi
                                    && s.YonlendirmeTipi == YonlendirmeTipi.BaskaBanko
                                    && s.TcKimlikNo != hp
                                    && s.HedefBankoId == bk.BankoId)

                                // 5. Genel Uzmana Yönlendirilmiş
                                // ⭐ JOIN'de kp.KanalAltIslemId == s.KanalAltIslemId eşleşmesi zaten yapılıyor
                                || (s.BeklemeDurum == BeklemeDurum.Yonlendirildi
                                    && s.YonlendirildiMi
                                    && s.YonlendirmeTipi == YonlendirmeTipi.UzmanPersonel
                                    && s.TcKimlikNo != hp
                                    && s.HedefBankoId == null
                                    && uzm.Uzmanlik == PersonelUzmanlik.Uzman)
                              )
                        select new
                        {
                            PersonelTc = hp,
                            PersonelAdSoyad = pPersonel.AdSoyad,
                            ConnectionId = hc.ConnectionId,
                            s.SiraId,
                            s.SiraNo,
                            s.KanalAltAdi,
                            s.SiraAlisZamani,
                            s.IslemBaslamaZamani,
                            s.BeklemeDurum,
                            Uzmanlik = uzm.Uzmanlik,
                            BankoId = b.BankoId,
                            hb.HizmetBinasiId,
                            hb.HizmetBinasiAdi,
                            s.YonlendirildiMi,
                            s.YonlendirmeTipi,
                            s.HedefBankoId,
                            s.YonlendirenBankoId,
                            s.YonlendirenPersonelTc,
                            YonlendirenPersonelAdSoyad = pYonlendiren != null ? pYonlendiren.AdSoyad : null,
                            // Öncelik hesaplamaları
                            DurumOnceligi = s.BeklemeDurum == BeklemeDurum.Cagrildi ? 0
                                          : s.BeklemeDurum == BeklemeDurum.Yonlendirildi ? 1
                                          : 2,
                            UzmanlikOnceligi = s.BeklemeDurum == BeklemeDurum.Beklemede
                                             ? (uzm.Uzmanlik == PersonelUzmanlik.Sef ? 0
                                              : uzm.Uzmanlik == PersonelUzmanlik.Uzman ? 1
                                              : uzm.Uzmanlik == PersonelUzmanlik.YrdUzman ? 2
                                              : 3)
                                             : 99
                        };

            var rawResult = await query
                .AsNoTracking()
                .Distinct()
                .OrderBy(x => x.PersonelTc)
                .ThenBy(x => x.DurumOnceligi)
                .ThenBy(x => x.UzmanlikOnceligi)
                .ThenBy(x => x.SiraAlisZamani)
                .ThenBy(x => x.SiraNo)
                .ToListAsync();

            // DTO mapping
            var result = rawResult
                .Select(x => new SiraCagirmaResponseDto
                {
                    PersonelTc = x.PersonelTc,
                    PersonelAdSoyad = x.PersonelAdSoyad,
                    ConnectionId = x.ConnectionId,
                    SiraId = x.SiraId,
                    SiraNo = x.SiraNo,
                    KanalAltAdi = x.KanalAltAdi,
                    BeklemeDurum = x.BeklemeDurum,
                    SiraAlisZamani = x.SiraAlisZamani,
                    IslemBaslamaZamani = x.IslemBaslamaZamani,
                    HizmetBinasiId = x.HizmetBinasiId,
                    HizmetBinasiAdi = x.HizmetBinasiAdi,
                    Uzmanlik = x.Uzmanlik,
                    BankoId = x.BankoId,
                    YonlendirildiMi = x.YonlendirildiMi,
                    YonlendirmeTipi = x.YonlendirmeTipi,
                    YonlendirenPersonelTc = x.YonlendirenPersonelTc,
                    YonlendirenPersonelAdSoyad = x.YonlendirenPersonelAdSoyad,
                    YonlendirmeAciklamasi = BuildYonlendirmeAciklamasiForIncremental(
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
            string? BuildYonlendirmeAciklamasiForIncremental(
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
            // 1. Sıranın KanalAltIslemId ve DepartmanHizmetBinasiId'sini bul
            var sira = await _context.Siralar
                .AsNoTracking()
                .Where(s => s.SiraId == siraId)
                .Select(s => new { s.KanalAltIslemId, s.DepartmanHizmetBinasiId })
                .FirstOrDefaultAsync();

            if (sira == null)
                return new List<string>();

            return await GetBankoModundakiPersonellerAsync(sira.DepartmanHizmetBinasiId, sira.KanalAltIslemId);
        }

        /// <summary>
        /// Belirli bir DepartmanHizmetBinasi ve KanalAltIslem için banko modunda olan personellerin TC listesini döner.
        /// </summary>
        public async Task<List<string>> GetBankoModundakiPersonellerAsync(int departmanHizmetBinasiId, int kanalAltIslemId)
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
                   && (b == null || b.DepartmanHizmetBinasiId == departmanHizmetBinasiId)
                select kp.TcKimlikNo
            ).Distinct().ToListAsync();

            return etkilenenPersoneller;
        }

        /// <summary>
        /// Belirli bir DepartmanHizmetBinasi ve KanalAltIslemId için banko modunda olan ve en az Yrd.Uzman yetkisine sahip personellerin TC listesini döner.
        /// Kiosk sıra alma için kullanılır - sadece işlem yapabilecek personel varsa sıra alınabilir.
        /// 
        /// NOT: kanalAltIslemId parametresi KanalAltIslem tablosundaki ID'dir!
        /// BankoKullanici tablosu üzerinden personel-banko eşleşmesi yapılır.
        /// 
        /// Kontroller:
        /// 1. KanalPersonel: Bu KanalAltIslem'e atanmış, aktif, en az Yrd.Uzman
        /// 2. User: BankoModuAktif = true (personel ŞU AN banko modunda mı?)
        /// 3. BankoKullanici: Personel bir bankoya atanmış mı?
        /// 4. Banko: Aynı departman-hizmet binasında ve aktif mi?
        /// </summary>
        public async Task<List<string>> GetBankoModundakiYetkiliPersonellerAsync(int departmanHizmetBinasiId, int kanalAltIslemId)
        {
            // Yeni proje mantığı: KanalAltIslem → Banko (DepartmanHizmetBinasiId üzerinden) → BankoKullanici → KanalPersonel
            var yetkiliPersoneller = await (
                from kai in _context.KanalAltIslemleri
                join ka in _context.KanallarAlt on kai.KanalAltId equals ka.KanalAltId
                join b in _context.Bankolar on kai.DepartmanHizmetBinasiId equals b.DepartmanHizmetBinasiId
                join bk in _context.BankoKullanicilari on b.BankoId equals bk.BankoId
                join kp in _context.KanalPersonelleri on new { bk.TcKimlikNo, kai.KanalAltIslemId } 
                    equals new { kp.TcKimlikNo, kp.KanalAltIslemId }
                join u in _context.Users on kp.TcKimlikNo equals u.TcKimlikNo
                where kai.KanalAltIslemId == kanalAltIslemId          // ⭐ KanalAltIslem ID eşleşmesi
                   && kai.Aktiflik == Aktiflik.Aktif                  // KanalAltIslem aktif
                   && !kai.SilindiMi                                  // KanalAltIslem silinmemiş
                   && kp.Aktiflik == Aktiflik.Aktif                   // Personel ataması aktif
                   && !kp.SilindiMi                                   // Personel ataması silinmemiş
                   && kp.Uzmanlik != PersonelUzmanlik.BilgisiYok      // En az Yrd.Uzman (1, 2, 3)
                   && u.BankoModuAktif == true                        // ⭐ ŞU AN banko modunda
                   && b.Aktiflik == Aktiflik.Aktif               // Banko aktif
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
            AND hb.Aktiflik = 1  -- Aktif
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
            var query = from k in _context.Kiosklar
                        join dhb in _context.DepartmanHizmetBinalari on k.DepartmanHizmetBinasiId equals dhb.DepartmanHizmetBinasiId
                        join hb in _context.HizmetBinalari on dhb.HizmetBinasiId equals hb.HizmetBinasiId
                        join kma in _context.KioskMenuAtamalari on k.KioskId equals kma.KioskId
                        join km in _context.KioskMenuler on kma.KioskMenuId equals km.KioskMenuId
                        join kmi in _context.KioskMenuIslemleri on km.KioskMenuId equals kmi.KioskMenuId
                        join ka in _context.KanallarAlt on kmi.KanalAltId equals ka.KanalAltId
                        join kai in _context.KanalAltIslemleri on new { ka.KanalAltId, k.DepartmanHizmetBinasiId }
                            equals new { kai.KanalAltId, kai.DepartmanHizmetBinasiId }
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
                           && !kp.SilindiMi
                           // Aktiflik kontrolleri
                           && hb.Aktiflik == Aktiflik.Aktif
                           && k.Aktiflik == Aktiflik.Aktif
                           && kma.Aktiflik == Aktiflik.Aktif
                           && km.Aktiflik == Aktiflik.Aktif
                           && kmi.Aktiflik == Aktiflik.Aktif
                           && ka.Aktiflik == Aktiflik.Aktif
                           && kp.Aktiflik == Aktiflik.Aktif  // KanalPersonel aktiflik kontrolü
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

        
        public async Task<List<KioskAltIslemDto>> GetKioskMenuAltIslemleriByKioskIdAsync(int kioskId, int kioskMenuId)
        {
            /*
            SELECT DISTINCT
                -- Alt İşlem Bilgileri
                kmi.KioskMenuIslemId,
                kmi.KanalAltId,
                ka.KanalAltAdi,
                kn.KanalAdi,
                kmi.MenuSira,
                kai.KanalAltIslemId

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
                INNER JOIN dbo.SIR_Kanallar AS kn WITH (NOLOCK)
                    ON ka.KanalId = kn.KanalId
                INNER JOIN dbo.SIR_KanalAltIslemleri AS kai WITH (NOLOCK)
                    ON kai.KanalAltId = ka.KanalAltId 
                    AND kai.HizmetBinasiId = k.HizmetBinasiId
                INNER JOIN dbo.SIR_KanalPersonelleri AS kp WITH (NOLOCK)
                    ON kp.KanalAltIslemId = kai.KanalAltIslemId
                INNER JOIN dbo.CMN_Users AS u WITH (NOLOCK)
                    ON u.TcKimlikNo = kp.TcKimlikNo

            WHERE 
                -- Kiosk ve Menü filtresi
                k.KioskId = @kioskId
                AND km.KioskMenuId = @kioskMenuId

                -- Soft delete kontrolleri
                AND hb.SilindiMi = 0
                AND k.SilindiMi = 0
                AND kma.SilindiMi = 0
                AND km.SilindiMi = 0
                AND kmi.SilindiMi = 0
                AND ka.SilindiMi = 0

                -- Aktiflik kontrolleri
                AND hb.Aktiflik = 1
                AND k.Aktiflik = 1
                AND kma.Aktiflik = 1
                AND km.Aktiflik = 1
                AND kmi.Aktiflik = 1
                AND ka.Aktiflik = 1
                AND u.BankoModuAktif = 1
                AND kp.Uzmanlik != 0

            ORDER BY kmi.MenuSira
            */

            var today = DateTime.Today;

            // Kiosk'un DepartmanHizmetBinasiId'sini al
            var kioskDhbId = await _context.Kiosklar
                .Where(k => k.KioskId == kioskId)
                .Select(k => k.DepartmanHizmetBinasiId)
                .FirstOrDefaultAsync();

            if (kioskDhbId == 0)
                return new List<KioskAltIslemDto>();

            // GetKioskMenulerByKioskIdAsync ile aynı yapıda tek sorgu
            var query = from k in _context.Kiosklar
                        join dhb in _context.DepartmanHizmetBinalari on k.DepartmanHizmetBinasiId equals dhb.DepartmanHizmetBinasiId
                        join hb in _context.HizmetBinalari on dhb.HizmetBinasiId equals hb.HizmetBinasiId
                        join kma in _context.KioskMenuAtamalari on k.KioskId equals kma.KioskId
                        join km in _context.KioskMenuler on kma.KioskMenuId equals km.KioskMenuId
                        join kmi in _context.KioskMenuIslemleri on km.KioskMenuId equals kmi.KioskMenuId
                        join ka in _context.KanallarAlt on kmi.KanalAltId equals ka.KanalAltId
                        join kn in _context.Kanallar on ka.KanalId equals kn.KanalId
                        join kai in _context.KanalAltIslemleri on new { ka.KanalAltId, k.DepartmanHizmetBinasiId }
                            equals new { kai.KanalAltId, kai.DepartmanHizmetBinasiId }
                        join kp in _context.KanalPersonelleri on kai.KanalAltIslemId equals kp.KanalAltIslemId
                        join u in _context.Users on kp.TcKimlikNo equals u.TcKimlikNo
                        where k.KioskId == kioskId
                           && km.KioskMenuId == kioskMenuId
                           // Soft delete kontrolleri
                           && !hb.SilindiMi
                           && !k.SilindiMi
                           && !kma.SilindiMi
                           && !km.SilindiMi
                           && !kmi.SilindiMi
                           && !ka.SilindiMi
                           && !kp.SilindiMi
                           // Aktiflik kontrolleri
                           && hb.Aktiflik == Aktiflik.Aktif
                           && k.Aktiflik == Aktiflik.Aktif
                           && kma.Aktiflik == Aktiflik.Aktif
                           && km.Aktiflik == Aktiflik.Aktif
                           && kmi.Aktiflik == Aktiflik.Aktif
                           && ka.Aktiflik == Aktiflik.Aktif
                           && kp.Aktiflik == Aktiflik.Aktif  // KanalPersonel aktiflik kontrolü
                           && u.BankoModuAktif == true
                           && kp.Uzmanlik != PersonelUzmanlik.BilgisiYok
                        select new
                        {
                            kmi.KioskMenuIslemId,
                            kmi.KanalAltId,
                            ka.KanalAltAdi,
                            kn.KanalAdi,
                            kmi.MenuSira,
                            kai.KanalAltIslemId,
                            dhb.HizmetBinasiId
                        };

            // Distinct ile benzersiz alt işlemleri al
            var altIslemler = await query
                .AsNoTracking()
                .Select(x => new
                {
                    x.KioskMenuIslemId,
                    x.KanalAltId,
                    x.KanalAltAdi,
                    x.KanalAdi,
                    x.MenuSira,
                    x.KanalAltIslemId,
                    x.HizmetBinasiId
                })
                .Distinct()
                .OrderBy(x => x.MenuSira)
                .ToListAsync();

            var result = new List<KioskAltIslemDto>();

            foreach (var islem in altIslemler)
            {
                // Bu işlem için aktif personel var mı? (Yrd.Uzman+ ve banko modunda)
                // NOT: SQL sorgusundaki mantıkla uyumlu - sadece BankoModuAktif kontrolü
                var aktifPersonelVar = await (
                    from kp in _context.KanalPersonelleri
                    join u in _context.Users on kp.TcKimlikNo equals u.TcKimlikNo
                    where kp.KanalAltIslemId == islem.KanalAltIslemId
                       && kp.Aktiflik == Aktiflik.Aktif
                       && !kp.SilindiMi
                       && kp.Uzmanlik != PersonelUzmanlik.BilgisiYok  // En az Yrd.Uzman
                       && u.BankoModuAktif == true
                       && u.AktifMi == true
                    select kp.TcKimlikNo
                ).AnyAsync();

                // Bekleyen sıra sayısını hesapla
                var bekleyenSayisi = await _context.Siralar
                    .AsNoTracking()
                    .CountAsync(s => s.KanalAltIslemId == islem.KanalAltIslemId
                                  && s.DepartmanHizmetBinasiId == kioskDhbId
                                  && s.BeklemeDurum == BeklemeDurum.Beklemede
                                  && s.SiraAlisZamani.Date == today
                                  && !s.SilindiMi);

                result.Add(new KioskAltIslemDto
                {
                    KioskMenuIslemId = islem.KioskMenuIslemId,
                    KanalAltId = islem.KanalAltId,
                    KanalAltIslemId = islem.KanalAltIslemId, // Sıra alma için gerekli
                    KanalAltAdi = islem.KanalAltAdi,
                    KanalAdi = islem.KanalAdi,
                    MenuSira = islem.MenuSira,
                    BekleyenSiraSayisi = bekleyenSayisi,
                    AktifPersonelVar = aktifPersonelVar,  // Gerçek kontrol sonucu
                    TahminiBeklemeSuresi = bekleyenSayisi * 5 // Ortalama 5 dk/sıra
                });
            }

            return result;
        }

        public async Task<SiraNoBilgisiDto?> GetSiraNoAsync(int kanalAltIslemId)
        {
            /*
            =============================================
            Stored Procedure: sp_GetSiraNo
            Açıklama: KanalAltIslemId üzerinden sıra numarası bilgisini getirir
            NOT: GetKioskMenuAltIslemleriByKioskIdAsync ile aynı mantık - BankoKullanici join'i yok
            =============================================
            CREATE PROCEDURE [dbo].[sp_GetSiraNo]
                @KanalAltIslemId INT
            AS
            BEGIN
                SET NOCOUNT ON;

                DECLARE @Bugun DATE = CAST(GETDATE() AS DATE)

                ;WITH MaxSiraNo AS (
                    -- Aynı KanalIslemId'ye sahip TÜM KanalAltIslemleri için bugünkü max sıra no
                    SELECT 
                        kai_target.KanalAltIslemId,
                        kai_target.KanalIslemId,
                        ISNULL(MAX(s.SiraNo), ki.BaslangicNumara - 1) AS LastNumber,
                        ki.BaslangicNumara,
                        ki.BitisNumara
                    FROM SIR_KanalAltIslemleri kai_target WITH (NOLOCK)
                    INNER JOIN SIR_KanalIslemleri ki WITH (NOLOCK) ON kai_target.KanalIslemId = ki.KanalIslemId
                    LEFT JOIN SIR_KanalAltIslemleri kai_all WITH (NOLOCK) ON kai_all.KanalIslemId = kai_target.KanalIslemId
                        AND kai_all.Aktiflik = 1 AND kai_all.SilindiMi = 0
                    LEFT JOIN SIR_Siralar s WITH (NOLOCK) ON kai_all.KanalAltIslemId = s.KanalAltIslemId
                        AND CAST(s.SiraAlisZamani AS DATE) = @Bugun AND s.SilindiMi = 0
                    WHERE kai_target.KanalAltIslemId = @KanalAltIslemId
                    AND kai_target.SilindiMi = 0
                    AND ki.SilindiMi = 0
                    GROUP BY kai_target.KanalAltIslemId, kai_target.KanalIslemId, ki.BaslangicNumara, ki.BitisNumara
                )
                SELECT TOP 1
                    CASE 
                        WHEN msn.LastNumber >= msn.BaslangicNumara AND msn.LastNumber < msn.BitisNumara 
                        THEN msn.LastNumber + 1 
                        ELSE msn.BaslangicNumara 
                    END AS SiraNo,
                    hb.HizmetBinasiId,
                    hb.HizmetBinasiAdi,
                    ka.KanalAltAdi,
                    kai.KanalAltIslemId,
                    msn.KanalIslemId,
                    msn.BaslangicNumara,
                    msn.BitisNumara
                FROM SIR_KanalAltIslemleri kai WITH (NOLOCK)
                INNER JOIN MaxSiraNo msn ON kai.KanalAltIslemId = msn.KanalAltIslemId
                INNER JOIN SIR_KanallarAlt ka WITH (NOLOCK) ON kai.KanalAltId = ka.KanalAltId
                INNER JOIN CMN_HizmetBinalari hb WITH (NOLOCK) ON kai.HizmetBinasiId = hb.HizmetBinasiId
                INNER JOIN SIR_KanalPersonelleri kp WITH (NOLOCK) ON kai.KanalAltIslemId = kp.KanalAltIslemId
                INNER JOIN CMN_Users u WITH (NOLOCK) ON kp.TcKimlikNo = u.TcKimlikNo
                WHERE kai.Aktiflik = 1 AND kai.SilindiMi = 0
                AND kai.KanalAltIslemId = @KanalAltIslemId
                AND kp.Aktiflik = 1 AND kp.SilindiMi = 0
                AND kp.Uzmanlik != 0  -- En az Yrd.Uzman
                AND u.BankoModuAktif = 1
            END
            GO

            -- Kullanım: EXEC sp_GetSiraNo @KanalAltIslemId = 5
            */

            var today = DateTime.Today;

            // Tek sorgu ile tüm işlemi yap (CTE benzeri yapı)
            // 1. Önce max sıra numarasını hesapla (aynı KanalIslemId'ye sahip tüm KanalAltIslemleri için)
            var maxSiraNoQuery = from kai_target in _context.KanalAltIslemleri
                                 join ki in _context.KanalIslemleri on kai_target.KanalIslemId equals ki.KanalIslemId
                                 where kai_target.KanalAltIslemId == kanalAltIslemId
                                    && !kai_target.SilindiMi
                                    && !ki.SilindiMi
                                 select new { kai_target.KanalIslemId, ki.BaslangicNumara, ki.BitisNumara };

            var kanalIslemBilgi = await maxSiraNoQuery.FirstOrDefaultAsync();
            if (kanalIslemBilgi == null)
                return null;

            // Aynı KanalIslemId'ye sahip tüm KanalAltIslemleri için bugünkü max sıra no
            var maxSiraNo = await (
                from kai_all in _context.KanalAltIslemleri
                join s in _context.Siralar on kai_all.KanalAltIslemId equals s.KanalAltIslemId into sJoin
                from sLeft in sJoin.DefaultIfEmpty()
                where kai_all.KanalIslemId == kanalIslemBilgi.KanalIslemId
                   && kai_all.Aktiflik == Aktiflik.Aktif
                   && !kai_all.SilindiMi
                   && (sLeft == null || (sLeft.SiraAlisZamani.Date == today && !sLeft.SilindiMi))
                select (int?)sLeft.SiraNo
            ).MaxAsync();

            var lastNumber = maxSiraNo ?? (kanalIslemBilgi.BaslangicNumara - 1);

            // 2. Personel kontrolü ve sonuç sorgusu (GetKioskMenuAltIslemleriByKioskIdAsync ile BİREBİR aynı mantık)
            var result = await (
                from kai in _context.KanalAltIslemleri
                join ka in _context.KanallarAlt on kai.KanalAltId equals ka.KanalAltId
                join dhb in _context.DepartmanHizmetBinalari on kai.DepartmanHizmetBinasiId equals dhb.DepartmanHizmetBinasiId
                join hb in _context.HizmetBinalari on dhb.HizmetBinasiId equals hb.HizmetBinasiId
                join kp in _context.KanalPersonelleri on kai.KanalAltIslemId equals kp.KanalAltIslemId
                join u in _context.Users on kp.TcKimlikNo equals u.TcKimlikNo
                where kai.KanalAltIslemId == kanalAltIslemId
                   && !kp.SilindiMi
                   && kp.Aktiflik == Aktiflik.Aktif
                   && u.BankoModuAktif == true
                   && kp.Uzmanlik != PersonelUzmanlik.BilgisiYok  // En az Yrd.Uzman
                select new SiraNoBilgisiDto
                {
                    // Sıra numarası hesaplama: aralık kontrolü
                    SiraNo = lastNumber >= kanalIslemBilgi.BaslangicNumara && lastNumber < kanalIslemBilgi.BitisNumara
                        ? lastNumber + 1
                        : kanalIslemBilgi.BaslangicNumara,
                    HizmetBinasiId = hb.HizmetBinasiId,
                    HizmetBinasiAdi = hb.HizmetBinasiAdi,
                    KanalAltAdi = ka.KanalAltAdi,
                    KanalAltIslemId = kai.KanalAltIslemId,
                    KanalIslemId = kanalIslemBilgi.KanalIslemId,
                    BaslangicNumara = kanalIslemBilgi.BaslangicNumara,
                    BitisNumara = kanalIslemBilgi.BitisNumara
                }
            ).FirstOrDefaultAsync();

            return result;
        }
    }
}
