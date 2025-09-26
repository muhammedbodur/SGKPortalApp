using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common
{
    public interface IModulControllerRepository : IGenericRepository<ModulController>
    {
        // Temel arama
        Task<ModulController?> GetByControllerAdiAsync(string controllerAdi);

        // Modül bazında controller'lar
        Task<IEnumerable<ModulController>> GetByModulAsync(int modulId);

        // İşlemler ile birlikte getir
        Task<ModulController?> GetWithIslemlerAsync(int modulControllerId);
        Task<IEnumerable<ModulController>> GetAllWithIslemlerAsync();

        // Listeleme
        Task<IEnumerable<ModulController>> GetActiveAsync();
        Task<IEnumerable<ModulController>> GetAllAsync();

        // Dropdown için
        Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync();
        Task<IEnumerable<(int Id, string Ad)>> GetByModulDropdownAsync(int modulId);

        // İstatistik
        Task<int> GetIslemCountAsync(int modulControllerId);
    }
}