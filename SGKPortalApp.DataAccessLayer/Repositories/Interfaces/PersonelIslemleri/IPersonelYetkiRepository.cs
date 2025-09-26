using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri
{
    public interface IPersonelYetkiRepository : IGenericRepository<PersonelYetki>
    {
        // Personel bazında yetkiler
        Task<IEnumerable<PersonelYetki>> GetByPersonelTcAsync(string tcKimlikNo);

        // Yetki bazında personeller
        Task<IEnumerable<PersonelYetki>> GetByYetkiAsync(int yetkiId);

        // Aktif yetkiler
        Task<IEnumerable<PersonelYetki>> GetActivePermissionsAsync();

        // Personelin belirli yetkisi var mı kontrolü
        Task<bool> HasPermissionAsync(string tcKimlikNo, int yetkiId);
    }
}