using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.Common
{
    /// <summary>
    /// Sık Kullanılan Program Repository Implementation
    /// </summary>
    public class SikKullanilanProgramRepository : GenericRepository<SikKullanilanProgram>, ISikKullanilanProgramRepository
    {
        public SikKullanilanProgramRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SikKullanilanProgram>> GetActiveProgramsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.Aktiflik == Aktiflik.Aktif && !p.SilindiMi)
                .OrderBy(p => p.Sira)
                .ToListAsync();
        }

        public async Task<IEnumerable<SikKullanilanProgram>> GetDashboardProgramsAsync(int count = 8)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.Aktiflik == Aktiflik.Aktif && !p.SilindiMi)
                .OrderBy(p => p.Sira)
                .Take(count)
                .ToListAsync();
        }
    }
}
