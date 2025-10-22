using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
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
                            Sira = kai.Sira,
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
                            KanalAdi = k.KanalAdi,
                            Sira = kai.Sira,
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
                        where kai.KanalAltIslemId == kanalAltIslemId
                        select new KanalAltIslemResponseDto
                        {
                            KanalAltIslemId = kai.KanalAltIslemId,
                            KanalAltId = kai.KanalAltId,
                            KanalAltAdi = ka.KanalAltAdi,
                            KanalIslemId = kai.KanalIslemId,
                            KanalAdi = k.KanalAdi,
                            Sira = kai.Sira,
                            Aktiflik = kai.Aktiflik,
                            EklenmeTarihi = kai.EklenmeTarihi,
                            DuzenlenmeTarihi = kai.DuzenlenmeTarihi,
                            PersonelSayisi = _context.KanalPersonelleri
                                .Count(kp => kp.KanalAltIslemId == kai.KanalAltIslemId && kp.Aktiflik == BusinessObjectLayer.Enums.Common.Aktiflik.Aktif)
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
                            Sira = kai.Sira,
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
                            Sira = kai.Sira,
                            Aktiflik = kai.Aktiflik,
                            EklenmeTarihi = kai.EklenmeTarihi,
                            DuzenlenmeTarihi = kai.DuzenlenmeTarihi,
                            PersonelSayisi = 0
                        };

            return await query.AsNoTracking().ToListAsync();
        }
    }
}
