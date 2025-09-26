using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri
{
    public interface IYetkiRepository : IGenericRepository<Yetki>
    {
        // Temel arama
        Task<Yetki?> GetByYetkiAdiAsync(string yetkiAdi);

        // Controller bazında yetkiler
        Task<IEnumerable<Yetki>> GetByControllerAsync(string controllerAdi);

        // Hiyerarşik yetkiler (üst yetki bazında)
        Task<IEnumerable<Yetki>> GetByParentYetkiAsync(int ustYetkiId);
        Task<IEnumerable<Yetki>> GetRootYetkilerAsync(); // Üst yetkisi olmayan ana yetkiler

        // Listeleme
        Task<IEnumerable<Yetki>> GetAllActiveAsync();

        // Dropdown için
        Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync();
    }
}