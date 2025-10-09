using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri
{
    public interface IPersonelEgitimRepository : IGenericRepository<PersonelEgitim>
    {
        // Personel bazında eğitimler
        Task<IEnumerable<PersonelEgitim>> GetByPersonelTcAsync(string tcKimlikNo);

        // Listeleme
        Task<IEnumerable<PersonelEgitim>> GetAllActiveAsync();

        // İstatistik
        Task<int> GetEgitimCountByPersonelAsync(string tcKimlikNo);
    }
}
