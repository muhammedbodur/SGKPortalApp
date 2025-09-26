using SGKPortalApp.BusinessObjectLayer;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri
{
    public interface IServisRepository : IGenericRepository<Servis>
    {
        // Temel arama
        Task<Servis?> GetByServisAdiAsync(string servisAdi);

        // Listeleme
        Task<IEnumerable<Servis>> GetActiveAsync();

        // Dropdown için
        Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync();
    }
}