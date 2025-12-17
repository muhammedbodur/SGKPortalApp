using Microsoft.EntityFrameworkCore;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.PersonelIslemleri
{
    public class PersonelYetkiRepository : GenericRepository<PersonelYetki>, IPersonelYetkiRepository
    {
        public PersonelYetkiRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PersonelYetki>> GetByPersonelTcAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return Enumerable.Empty<PersonelYetki>();

            return await _dbSet
                .Include(py => py.ModulControllerIslem)
                    .ThenInclude(mci => mci.ModulController)
                        .ThenInclude(mc => mc.Modul)
                .AsNoTracking()
                .Where(py => py.TcKimlikNo == tcKimlikNo)
                .OrderBy(py => py.ModulControllerIslem.ModulControllerIslemAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<PersonelYetki>> GetByModulControllerIslemAsync(int modulControllerIslemId)
        {
            return await _dbSet
                .Include(py => py.Personel)
                .AsNoTracking()
                .Where(py => py.ModulControllerIslemId == modulControllerIslemId)
                .OrderBy(py => py.Personel.AdSoyad)
                .ToListAsync();
        }

        public async Task<IEnumerable<PersonelYetki>> GetActivePermissionsAsync()
        {
            return await _dbSet
                .Include(py => py.Personel)
                .Include(py => py.ModulControllerIslem)
                    .ThenInclude(mci => mci.ModulController)
                        .ThenInclude(mc => mc.Modul)
                .AsNoTracking()
                .Where(py => !py.Personel.SilindiMi)
                .OrderBy(py => py.Personel.AdSoyad)
                .ThenBy(py => py.ModulControllerIslem.ModulControllerIslemAdi)
                .ToListAsync();
        }

        public async Task<bool> HasPermissionAsync(string tcKimlikNo, int modulControllerIslemId)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo) || modulControllerIslemId <= 0)
                return false;

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(py => py.TcKimlikNo == tcKimlikNo && py.ModulControllerIslemId == modulControllerIslemId);
        }

        public async Task<YetkiSeviyesi> GetPermissionLevelAsync(string tcKimlikNo, string permissionKey)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo) || string.IsNullOrWhiteSpace(permissionKey))
                return YetkiSeviyesi.None;

            var permission = await _dbSet
                .Include(py => py.ModulControllerIslem)
                .AsNoTracking()
                .FirstOrDefaultAsync(py => py.TcKimlikNo == tcKimlikNo 
                    && py.ModulControllerIslem.PermissionKey == permissionKey);

            return permission?.YetkiSeviyesi ?? YetkiSeviyesi.None;
        }

        public async Task<Dictionary<string, int>> GetAssignedPermissionsAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return new Dictionary<string, int>();

            return await _dbSet
                .Include(py => py.ModulControllerIslem)
                .AsNoTracking()
                .Where(py => py.TcKimlikNo == tcKimlikNo 
                          && !py.SilindiMi 
                          && py.ModulControllerIslem != null 
                          && !string.IsNullOrEmpty(py.ModulControllerIslem.PermissionKey))
                .ToDictionaryAsync(
                    py => py.ModulControllerIslem!.PermissionKey,
                    py => (int)py.YetkiSeviyesi);
        }
    }
}