using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common
{
    public interface IModulRepository : IGenericRepository<Modul>
    {
        // Temel arama
        Task<Modul?> GetByModulAdiAsync(string modulAdi);

        // Controller'ları ile birlikte getir
        Task<Modul?> GetWithControllersAsync(int modulId);
        Task<IEnumerable<Modul>> GetAllWithControllersAsync();

        // Listeleme
        Task<IEnumerable<Modul>> GetActiveAsync();
        Task<IEnumerable<Modul>> GetAllAsync();

        // Dropdown için
        Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync();

        // İstatistik
        Task<int> GetControllerCountAsync(int modulId);
    }
}