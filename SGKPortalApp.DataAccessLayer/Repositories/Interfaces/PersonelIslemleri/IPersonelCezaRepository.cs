using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri
{
    public interface IPersonelCezaRepository : IGenericRepository<PersonelCeza>
    {
        // Personel bazında cezalar
        Task<IEnumerable<PersonelCeza>> GetByPersonelTcAsync(string tcKimlikNo);

        // Listeleme
        Task<IEnumerable<PersonelCeza>> GetAllActiveAsync();

        // İstatistik
        Task<int> GetCezaCountByPersonelAsync(string tcKimlikNo);
    }
}
