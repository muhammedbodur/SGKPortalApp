using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri
{
    public interface IPersonelImzaYetkisiRepository : IGenericRepository<PersonelImzaYetkisi>
    {
        // Personel bazında imza yetkileri
        Task<IEnumerable<PersonelImzaYetkisi>> GetByPersonelTcAsync(string tcKimlikNo);

        // Aktif yetkiler
        Task<IEnumerable<PersonelImzaYetkisi>> GetActiveByPersonelTcAsync(string tcKimlikNo);

        // Listeleme
        Task<IEnumerable<PersonelImzaYetkisi>> GetAllActiveAsync();

        // İstatistik
        Task<int> GetImzaYetkisiCountByPersonelAsync(string tcKimlikNo);
    }
}
