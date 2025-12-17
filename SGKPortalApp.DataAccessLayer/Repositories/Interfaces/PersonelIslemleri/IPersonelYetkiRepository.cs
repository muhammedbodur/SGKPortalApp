using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri
{
    public interface IPersonelYetkiRepository : IGenericRepository<PersonelYetki>
    {
        // Personel bazında yetkiler
        Task<IEnumerable<PersonelYetki>> GetByPersonelTcAsync(string tcKimlikNo);

        // ModulControllerIslem bazında personeller
        Task<IEnumerable<PersonelYetki>> GetByModulControllerIslemAsync(int modulControllerIslemId);

        // Aktif yetkiler
        Task<IEnumerable<PersonelYetki>> GetActivePermissionsAsync();

        // Personelin belirli yetkisi var mı kontrolü
        Task<bool> HasPermissionAsync(string tcKimlikNo, int modulControllerIslemId);

        // Personelin belirli permission key için yetki seviyesi
        Task<YetkiSeviyesi> GetPermissionLevelAsync(string tcKimlikNo, string permissionKey);

        // Personele atanmış tüm yetkileri dictionary olarak döner (PermissionKey -> YetkiSeviyesi)
        Task<Dictionary<string, int>> GetAssignedPermissionsAsync(string tcKimlikNo);
    }
}