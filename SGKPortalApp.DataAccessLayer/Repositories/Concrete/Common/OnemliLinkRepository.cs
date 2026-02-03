using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.Common
{
    /// <summary>
    /// Önemli Link Repository Implementation
    /// </summary>
    public class OnemliLinkRepository : GenericRepository<OnemliLink>, IOnemliLinkRepository
    {
        public OnemliLinkRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<OnemliLink>> GetActiveLinksAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(l => l.Aktiflik == Aktiflik.Aktif && !l.SilindiMi)
                .OrderBy(l => l.Sira)
                .ToListAsync();
        }

        public async Task<IEnumerable<OnemliLink>> GetDashboardLinksAsync(int count = 10)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(l => l.Aktiflik == Aktiflik.Aktif && !l.SilindiMi)
                .OrderBy(l => l.Sira)
                .Take(count)
                .ToListAsync();
        }
    }
}
