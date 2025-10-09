using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri
{
    public interface IPersonelEngelRepository : IGenericRepository<PersonelEngel>
    {
        // Personel bazında engeller
        Task<IEnumerable<PersonelEngel>> GetByPersonelTcAsync(string tcKimlikNo);

        // Listeleme
        Task<IEnumerable<PersonelEngel>> GetAllActiveAsync();

        // İstatistik
        Task<int> GetEngelCountByPersonelAsync(string tcKimlikNo);
    }
}
