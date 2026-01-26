using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.Legacy;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Legacy;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.Legacy
{
    public class LegacyAtanmaRepository : ILegacyAtanmaRepository
    {
        private readonly MysqlDbContext _context;

        public LegacyAtanmaRepository(MysqlDbContext context)
        {
            _context = context;
        }

        public async Task<List<LegacyAtanma>> GetAllAsync()
        {
            return await _context.Atanmalar.ToListAsync();
        }
    }
}
