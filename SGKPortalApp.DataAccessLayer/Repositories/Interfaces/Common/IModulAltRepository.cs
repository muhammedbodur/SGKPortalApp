using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common
{
    public interface IModulAltRepository : IGenericRepository<ModulAlt>
    {
        // Temel arama
        Task<ModulAlt?> GetByModulAltAdiAsync(string modulAltAdi);

        // Ana modül bazında alt modüller
        Task<IEnumerable<ModulAlt>> GetByModulAsync(int modulId);

        // Ana modül ile birlikte getir
        Task<ModulAlt?> GetWithModulAsync(int modulAltId);
        Task<IEnumerable<ModulAlt>> GetAllWithModulAsync();

        // Hiyerarşik yapı (parent-child)
        Task<IEnumerable<ModulAlt>> GetByParentModulAltAsync(int parentModulAltId);
        Task<IEnumerable<ModulAlt>> GetRootModulAltlarAsync(); // Parent'ı olmayan ana alt modüller

        // Listeleme
        Task<IEnumerable<ModulAlt>> GetActiveAsync();
        Task<IEnumerable<ModulAlt>> GetAllAsync();

        // Dropdown için
        Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync();
        Task<IEnumerable<(int Id, string Ad)>> GetByModulDropdownAsync(int modulId);
    }
}