using Microsoft.EntityFrameworkCore;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.Common
{
    public class IlceRepository : GenericRepository<Ilce>, IIlceRepository
    {
        public IlceRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<Ilce?> GetByIlceAdiAsync(string ilceAdi)
        {
            if (string.IsNullOrWhiteSpace(ilceAdi))
                return null;

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(ic => ic.IlceAdi == ilceAdi);
        }

        public async Task<IEnumerable<Ilce>> GetByIlAsync(int ilId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(ic => ic.IlId == ilId)
                .OrderBy(ic => ic.IlceAdi)
                .ToListAsync();
        }

        public async Task<Ilce?> GetWithIlAsync(int ilceId)
        {
            return await _dbSet
                .Include(ic => ic.Il)
                .AsNoTracking()
                .FirstOrDefaultAsync(ic => ic.IlceId == ilceId);
        }

        public async Task<IEnumerable<Ilce>> GetAllAsync()
        {
            return await _dbSet
                .Include(ic => ic.Il)
                .AsNoTracking()
                .OrderBy(ic => ic.Il.IlAdi)
                .ThenBy(ic => ic.IlceAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(ic => ic.IlceAdi)
                .Select(ic => new ValueTuple<int, string>(ic.IlceId, ic.IlceAdi))
                .ToListAsync();
        }

        public async Task<IEnumerable<(int Id, string Ad)>> GetByIlDropdownAsync(int ilId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(ic => ic.IlId == ilId)
                .OrderBy(ic => ic.IlceAdi)
                .Select(ic => new ValueTuple<int, string>(ic.IlceId, ic.IlceAdi))
                .ToListAsync();
        }
    }
}