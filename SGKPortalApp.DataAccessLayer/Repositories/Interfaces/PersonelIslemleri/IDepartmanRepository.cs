using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri
{
    public interface IDepartmanRepository : IGenericRepository<Departman>
    {
        // Temel arama
        Task<Departman?> GetByDepartmanAdiAsync(string departmanAdi);

        // Listeleme
        Task<IEnumerable<Departman>> GetActiveAsync();

        // Dropdown için
        Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync();

        // Basit istatistik
        Task<int> GetPersonelCountAsync(int departmanId);
    }
}