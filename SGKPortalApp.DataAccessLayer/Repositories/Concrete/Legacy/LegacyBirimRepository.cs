using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.Legacy;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Legacy;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.Legacy
{
    /// <summary>
    /// Legacy MySQL Birim tablosu için repository implementasyonu
    /// </summary>
    public class LegacyBirimRepository : ILegacyBirimRepository
    {
        private readonly MysqlDbContext _context;

        public LegacyBirimRepository(MysqlDbContext context)
        {
            _context = context;
        }

        public async Task<List<LegacyBirim>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Birimler
                .AsNoTracking()
                .ToListAsync(ct);
        }
    }
}
