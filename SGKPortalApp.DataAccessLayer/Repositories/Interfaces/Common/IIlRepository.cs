using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common
{
    public interface IIlRepository : IGenericRepository<Il>
    {
        // Temel arama
        Task<Il?> GetByIlAdiAsync(string ilAdi);

        // İlçelerle birlikte getir
        Task<Il?> GetWithIlcelerAsync(int ilId);

        // Listeleme
        Task<IEnumerable<Il>> GetAllAsync();

        // Dropdown için
        Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync();

        // İstatistik
        Task<int> GetIlceCountAsync(int ilId);
    }
}