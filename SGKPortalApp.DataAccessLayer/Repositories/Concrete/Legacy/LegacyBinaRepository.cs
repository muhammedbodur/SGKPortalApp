using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.Legacy;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Legacy;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.Legacy
{
    /// <summary>
    /// Legacy MySQL Bina tablosu için repository implementasyonu
    /// </summary>
    public class LegacyBinaRepository : ILegacyBinaRepository
    {
        private readonly MysqlDbContext _context;

        public LegacyBinaRepository(MysqlDbContext context)
        {
            _context = context;
        }

        public async Task<List<LegacyBina>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Binalar
                .AsNoTracking()
                .ToListAsync(ct);
        }
    }
}
