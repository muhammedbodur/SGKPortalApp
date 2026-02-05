using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.Common
{
    public class HaberRepository : IHaberRepository
    {
        private readonly SGKDbContext _context;

        public HaberRepository(SGKDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Haber>> GetSliderHaberleriAsync(int count = 5)
        {
            var now = DateTime.Now;

            // Vitrin resmi olan haberleri önce al
            var haberIdsWithVitrin = await _context.HaberResimler
                .AsNoTracking()
                .Where(r => r.IsVitrin && r.Aktiflik == Aktiflik.Aktif)
                .Select(r => r.HaberId)
                .Distinct()
                .ToListAsync();

            // Aktif haberler arasında vitrin resmi olanları al; yoksa GorselUrl olanları
            var haberler = await _context.Haberler
                .AsNoTracking()
                .Where(d => d.Aktiflik == Aktiflik.Aktif &&
                           d.YayinTarihi <= now &&
                           (d.BitisTarihi == null || d.BitisTarihi >= now) &&
                           (haberIdsWithVitrin.Contains(d.HaberId) || !string.IsNullOrEmpty(d.GorselUrl)))
                .OrderBy(d => d.Sira)
                .Take(count)
                .ToListAsync();

            return haberler;
        }

        public async Task<(IEnumerable<Haber> Items, int TotalCount)> GetHaberListeAsync(
            int pageNumber, int pageSize, string? searchTerm = null)
        {
            var now = DateTime.Now;

            IQueryable<Haber> query = _context.Haberler
                .AsNoTracking()
                .Where(d => d.Aktiflik == Aktiflik.Aktif &&
                           d.YayinTarihi <= now &&
                           (d.BitisTarihi == null || d.BitisTarihi >= now));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                query = query.Where(d =>
                    d.Baslik.Contains(term) ||
                    d.Icerik.Contains(term));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(d => d.YayinTarihi)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Haber?> GetHaberByIdAsync(int haberId)
        {
            var now = DateTime.Now;
            return await _context.Haberler
                .AsNoTracking()
                .FirstOrDefaultAsync(d =>
                    d.HaberId == haberId &&
                    d.Aktiflik == Aktiflik.Aktif &&
                    d.YayinTarihi <= now &&
                    (d.BitisTarihi == null || d.BitisTarihi >= now));
        }

        public async Task<IEnumerable<HaberResim>> GetHaberResimleriAsync(int haberId)
        {
            return await _context.HaberResimler
                .AsNoTracking()
                .Where(r => r.HaberId == haberId && r.Aktiflik == Aktiflik.Aktif)
                .OrderBy(r => r.Sira)
                .ToListAsync();
        }

        // ─── CRUD ───────────────────────────────────────────

        public async Task<Haber> CreateAsync(Haber haber)
        {
            haber.EklenmeTarihi = DateTime.Now;
            _context.Haberler.Add(haber);
            await _context.SaveChangesAsync();
            return haber;
        }

        public async Task<Haber> UpdateAsync(Haber haber)
        {
            haber.DuzenlenmeTarihi = DateTime.Now;
            _context.Haberler.Update(haber);
            await _context.SaveChangesAsync();
            return haber;
        }

        public async Task<bool> DeleteAsync(int haberId)
        {
            var haber = await _context.Haberler.FirstOrDefaultAsync(h => h.HaberId == haberId);
            if (haber == null) return false;

            haber.SilindiMi = true;
            haber.DuzenlenmeTarihi = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(IEnumerable<Haber> Items, int TotalCount)> GetAllForAdminAsync(
            int pageNumber, int pageSize, string? searchTerm = null)
        {
            IQueryable<Haber> query = _context.Haberler
                .AsNoTracking()
                .Where(d => !d.SilindiMi);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                query = query.Where(d =>
                    d.Baslik.Contains(term) ||
                    d.Icerik.Contains(term));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(d => d.YayinTarihi)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Haber?> GetHaberByIdForAdminAsync(int haberId)
        {
            return await _context.Haberler
                .FirstOrDefaultAsync(d => d.HaberId == haberId && !d.SilindiMi);
        }

        public async Task<HaberResim> CreateResimAsync(HaberResim haberResim)
        {
            haberResim.EklenmeTarihi = DateTime.Now;
            _context.HaberResimler.Add(haberResim);
            await _context.SaveChangesAsync();
            return haberResim;
        }

        public async Task<bool> DeleteResimAsync(int haberResimId)
        {
            var resim = await _context.HaberResimler.FirstOrDefaultAsync(r => r.HaberResimId == haberResimId);
            if (resim == null) return false;

            _context.HaberResimler.Remove(resim);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<HaberResim?> GetResimByIdAsync(int haberResimId)
        {
            return await _context.HaberResimler
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.HaberResimId == haberResimId);
        }
    }
}
