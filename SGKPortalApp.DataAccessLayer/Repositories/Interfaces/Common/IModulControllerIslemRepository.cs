using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common
{
    public interface IModulControllerIslemRepository : IGenericRepository<ModulControllerIslem>
    {
        // Temel arama
        Task<ModulControllerIslem?> GetByIslemAdiAsync(string islemAdi);

        // Controller bazında işlemler
        Task<IEnumerable<ModulControllerIslem>> GetByControllerAsync(int modulControllerId);

        // Controller ile birlikte getir
        Task<ModulControllerIslem?> GetWithControllerAsync(int islemId);
        Task<IEnumerable<ModulControllerIslem>> GetAllWithControllerAsync();

        // Action bazında
        Task<IEnumerable<ModulControllerIslem>> GetByActionNameAsync(string actionName);

        // Listeleme
        Task<IEnumerable<ModulControllerIslem>> GetActiveAsync();
        Task<IEnumerable<ModulControllerIslem>> GetAllAsync();

        // Dropdown için
        Task<IEnumerable<DropdownItemDto>> GetDropdownAsync();
        Task<IEnumerable<DropdownItemDto>> GetByControllerDropdownAsync(int modulControllerId);
    }
}