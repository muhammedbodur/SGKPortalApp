using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.Legacy;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Legacy;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.Legacy
{
    /// <summary>
    /// Legacy MySQL Servis tablosu için repository implementasyonu
    /// </summary>
    public class LegacyServisRepository : ILegacyServisRepository
    {
        private readonly MysqlDbContext _context;

        public LegacyServisRepository(MysqlDbContext context)
        {
            _context = context;
        }

        public async Task<List<LegacyServis>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Servisler
                .AsNoTracking()
                .ToListAsync(ct);
        }
    }
}
