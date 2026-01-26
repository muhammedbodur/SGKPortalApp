using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.Legacy;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Legacy;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.Legacy
{
    /// <summary>
    /// Legacy MySQL Kullanici tablosu için repository implementasyonu
    /// </summary>
    public class LegacyKullaniciRepository : ILegacyKullaniciRepository
    {
        private readonly MysqlDbContext _context;

        public LegacyKullaniciRepository(MysqlDbContext context)
        {
            _context = context;
        }

        public async Task<List<LegacyKullanici>> GetAllActiveAsync(CancellationToken ct = default)
        {
            return await _context.Kullanicilar
                .AsNoTracking()
                .Where(k => k.Username != null && k.Username > 10000000000) // Geçerli TC'ler
                .ToListAsync(ct);
        }
    }
}
