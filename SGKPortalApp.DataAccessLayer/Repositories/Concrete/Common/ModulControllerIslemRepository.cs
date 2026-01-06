using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.Common
{
    public class ModulControllerIslemRepository : GenericRepository<ModulControllerIslem>, IModulControllerIslemRepository
    {
        private readonly IMemoryCache _cache;

        public ModulControllerIslemRepository(SGKDbContext context, IMemoryCache cache) : base(context)
        {
            _cache = cache;
        }

        public async Task<ModulControllerIslem?> GetByIslemAdiAsync(string islemAdi)
        {
            if (string.IsNullOrWhiteSpace(islemAdi))
                return null;

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(mci => mci.ModulControllerIslemAdi == islemAdi);
        }

        public async Task<IEnumerable<ModulControllerIslem>> GetByControllerAsync(int modulControllerId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(mci => mci.ModulControllerId == modulControllerId)
                .OrderBy(mci => mci.ModulControllerIslemAdi)
                .ToListAsync();
        }

        public async Task<ModulControllerIslem?> GetWithControllerAsync(int islemId)
        {
            return await _dbSet
                .Include(mci => mci.ModulController)
                    .ThenInclude(mc => mc.Modul)
                .AsNoTracking()
                .FirstOrDefaultAsync(mci => mci.ModulControllerIslemId == islemId);
        }

        public async Task<IEnumerable<ModulControllerIslem>> GetAllWithControllerAsync()
        {
            return await _dbSet
                .Include(mci => mci.ModulController)
                    .ThenInclude(mc => mc.Modul)
                .AsNoTracking()
                .OrderBy(mci => mci.ModulController.Modul.ModulAdi)
                .ThenBy(mci => mci.ModulController.ModulControllerAdi)
                .ThenBy(mci => mci.ModulControllerIslemAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<ModulControllerIslem>> GetByActionNameAsync(string actionName)
        {
            // ActionName field'ı entity'de yok, ModulControllerIslemAdi kullanıyorum
            if (string.IsNullOrWhiteSpace(actionName))
                return Enumerable.Empty<ModulControllerIslem>();

            return await _dbSet
                .AsNoTracking()
                .Where(mci => mci.ModulControllerIslemAdi.Contains(actionName))
                .OrderBy(mci => mci.ModulControllerIslemAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<ModulControllerIslem>> GetActiveAsync()
        {
            // AktifMi field'ı olmadığı için tüm kayıtları döner
            return await _dbSet
                .AsNoTracking()
                .OrderBy(mci => mci.ModulControllerIslemAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<ModulControllerIslem>> GetAllAsync()
        {
            return await _dbSet
                .Include(mci => mci.ModulController)
                    .ThenInclude(mc => mc.Modul)
                .AsNoTracking()
                .OrderBy(mci => mci.ModulController.Modul.ModulAdi)
                .ThenBy(mci => mci.ModulController.ModulControllerAdi)
                .ThenBy(mci => mci.ModulControllerIslemAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<DropdownItemDto>> GetDropdownAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(mci => mci.ModulControllerIslemAdi)
                .Select(mci => new DropdownItemDto { Id = mci.ModulControllerIslemId, Ad = mci.ModulControllerIslemAdi })
                .ToListAsync();
        }

        public async Task<IEnumerable<DropdownItemDto>> GetByControllerDropdownAsync(int modulControllerId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(mci => mci.ModulControllerId == modulControllerId)
                .OrderBy(mci => mci.ModulControllerIslemAdi)
                .Select(mci => new DropdownItemDto { Id = mci.ModulControllerIslemId, Ad = mci.ModulControllerIslemAdi })
                .ToListAsync();
        }

        /// <summary>
        /// Varsayılan yetkileri döndürür (MinYetkiSeviyesi >= None olanlar)
        /// PERFORMANS: Sık kullanıldığı için sonuç in-memory cache'lenir (5 dakika)
        /// Login performansı için kritik - her login'de çağrılıyor
        /// </summary>
        public async Task<Dictionary<string, int>> GetDefaultPermissionsAsync()
        {
            const string cacheKey = "DefaultPermissions";

            // Cache'den kontrol et
            if (_cache.TryGetValue(cacheKey, out Dictionary<string, int>? cachedPermissions) && cachedPermissions != null)
            {
                return cachedPermissions;
            }

            // Cache'de yok, veritabanından çek
            var permissions = await _dbSet
                .AsNoTracking()
                .Where(mci => !string.IsNullOrEmpty(mci.PermissionKey)
                           && mci.MinYetkiSeviyesi >= BusinessObjectLayer.Enums.Common.YetkiSeviyesi.None
                           && !mci.SilindiMi)
                .ToDictionaryAsync(
                    mci => mci.PermissionKey!,
                    mci => (int)mci.MinYetkiSeviyesi);

            // 5 dakika cache'le (absolute), 2 dakika sliding (kullanılmazsa sil)
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
                .SetSlidingExpiration(TimeSpan.FromMinutes(2));

            _cache.Set(cacheKey, permissions, cacheOptions);

            return permissions;
        }
    }
}