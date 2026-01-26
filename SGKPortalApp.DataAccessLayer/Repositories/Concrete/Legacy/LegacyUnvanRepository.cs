using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.Legacy;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Legacy;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.Legacy
{
    /// <summary>
    /// Legacy MySQL Unvan tablosu için repository implementasyonu
    /// </summary>
    public class LegacyUnvanRepository : ILegacyUnvanRepository
    {
        private readonly MysqlDbContext _context;

        public LegacyUnvanRepository(MysqlDbContext context)
        {
            _context = context;
        }

        public async Task<List<LegacyUnvan>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Unvanlar
                .AsNoTracking()
                .ToListAsync(ct);
        }
    }
}
