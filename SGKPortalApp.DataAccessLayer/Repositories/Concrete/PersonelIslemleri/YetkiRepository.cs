using Microsoft.EntityFrameworkCore;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.PersonelIslemleri
{
    public class YetkiRepository : GenericRepository<Yetki>, IYetkiRepository
    {
        public YetkiRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<Yetki?> GetByYetkiAdiAsync(string yetkiAdi)
        {
            if (string.IsNullOrWhiteSpace(yetkiAdi))
                return null;

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(y => y.YetkiAdi == yetkiAdi);
        }

        public async Task<IEnumerable<Yetki>> GetByControllerAsync(string controllerAdi)
        {
            if (string.IsNullOrWhiteSpace(controllerAdi))
                return Enumerable.Empty<Yetki>();

            return await _dbSet
                .AsNoTracking()
                .Where(y => y.ControllerAdi == controllerAdi)
                .OrderBy(y => y.YetkiAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<Yetki>> GetByParentYetkiAsync(int ustYetkiId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(y => y.UstYetkiId == ustYetkiId)
                .OrderBy(y => y.YetkiAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<Yetki>> GetRootYetkilerAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(y => y.UstYetkiId == null)
                .OrderBy(y => y.YetkiAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<Yetki>> GetAllActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(y => y.YetkiAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<DropdownItemDto>> GetDropdownAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(y => y.YetkiAdi)
                .Select(y => new DropdownItemDto { Id = y.YetkiId, Ad = y.YetkiAdi })
                .ToListAsync();
        }
    }
}