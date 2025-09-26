using SGKPortalApp.BusinessObjectLayer;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri
{
    public interface IUnvanRepository : IGenericRepository<Unvan>
    {
        // Temel arama
        Task<Unvan?> GetByUnvanAdiAsync(string unvanAdi);

        // Listeleme
        Task<IEnumerable<Unvan>> GetActiveAsync();

        // Dropdown için
        Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync();
    }
}