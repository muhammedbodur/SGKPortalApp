using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri
{
    public interface ISendikaRepository : IGenericRepository<Sendika>
    {
        // Temel arama
        Task<Sendika?> GetBySendikaAdiAsync(string sendikaAdi);

        // Listeleme
        Task<IEnumerable<Sendika>> GetAllAsync();

        // Dropdown için
        Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync();

        // Basit istatistik
        Task<int> GetPersonelCountAsync(int sendikaId);
    }
}