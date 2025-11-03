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
    }
}
