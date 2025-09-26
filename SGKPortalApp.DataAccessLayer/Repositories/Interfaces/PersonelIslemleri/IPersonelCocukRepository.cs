using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri
{
    public interface IPersonelCocukRepository : IGenericRepository<PersonelCocuk>
    {
        // Personel bazında çocuklar
        Task<IEnumerable<PersonelCocuk>> GetByPersonelTcAsync(string tcKimlikNo);

        // Listeleme
        Task<IEnumerable<PersonelCocuk>> GetAllActiveAsync();

        // İstatistik
        Task<int> GetCocukCountByPersonelAsync(string tcKimlikNo);
    }
}