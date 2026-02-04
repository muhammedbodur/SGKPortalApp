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

        public async Task<IEnumerable<Duyuru>> GetSliderHaberleriAsync(int count = 5)
        {
            var now = DateTime.Now;

            // Vitrin resmi olan haberleri önce al
            var haberIdsWithVitrin = await _context.DuyuruResimler
                .AsNoTracking()
                .Where(r => r.IsVitrin && r.Aktiflik == Aktiflik.Aktif)
                .Select(r => r.DuyuruId)
                .Distinct()
                .ToListAsync();

            // Aktif haberler arasında vitrin resmi olanları al; yoksa GorselUrl olanları
            var haberler = await _context.Duyurular
                .AsNoTracking()
                .Where(d => d.Aktiflik == Aktiflik.Aktif &&
                           d.YayinTarihi <= now &&
                           (d.BitisTarihi == null || d.BitisTarihi >= now) &&
                           (haberIdsWithVitrin.Contains(d.DuyuruId) || !string.IsNullOrEmpty(d.GorselUrl)))
                .OrderBy(d => d.Sira)
                .Take(count)
                .ToListAsync();

            return haberler;
        }

        public async Task<(IEnumerable<Duyuru> Items, int TotalCount)> GetHaberListeAsync(
            int pageNumber, int pageSize, string? searchTerm = null)
        {
            var now = DateTime.Now;

            IQueryable<Duyuru> query = _context.Duyurular
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

        public async Task<Duyuru?> GetHaberByIdAsync(int haberId)
        {
            var now = DateTime.Now;
            return await _context.Duyurular
                .AsNoTracking()
                .FirstOrDefaultAsync(d =>
                    d.DuyuruId == haberId &&
                    d.Aktiflik == Aktiflik.Aktif &&
                    d.YayinTarihi <= now &&
                    (d.BitisTarihi == null || d.BitisTarihi >= now));
        }

        public async Task<IEnumerable<DuyuruResim>> GetHaberResimleriAsync(int haberId)
        {
            return await _context.DuyuruResimler
                .AsNoTracking()
                .Where(r => r.DuyuruId == haberId && r.Aktiflik == Aktiflik.Aktif)
                .OrderBy(r => r.Sira)
                .ToListAsync();
        }
    }
}
