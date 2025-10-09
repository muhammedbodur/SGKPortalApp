using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri
{
    public interface IPersonelHizmetRepository : IGenericRepository<PersonelHizmet>
    {
        // Personel bazında hizmet geçmişi
        Task<IEnumerable<PersonelHizmet>> GetByPersonelTcAsync(string tcKimlikNo);

        // Listeleme
        Task<IEnumerable<PersonelHizmet>> GetAllActiveAsync();

        // İstatistik
        Task<int> GetHizmetCountByPersonelAsync(string tcKimlikNo);
    }
}
