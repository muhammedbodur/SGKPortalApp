using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri
{
    public interface IAtanmaNedenleriRepository : IGenericRepository<AtanmaNedenleri>
    {
        // Temel arama
        Task<AtanmaNedenleri?> GetByAtanmaNedeniAsync(string atanmaNedeni);

        // Listeleme
        Task<IEnumerable<AtanmaNedenleri>> GetAllAsync();

        // Dropdown için
        Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync();

        // Basit istatistik
        Task<int> GetPersonelCountAsync(int atanmaNedeniId);
    }
}