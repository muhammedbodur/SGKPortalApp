using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri
{
    public interface IKioskMenuRepository : IGenericRepository<KioskMenu>
    {
        Task<IEnumerable<KioskMenu>> GetActiveAsync();

        Task<KioskMenu?> GetWithKiosksAsync(int kioskMenuId);

        Task<bool> ExistsByNameAsync(string menuAdi);

        Task<int> GetMaxSiraAsync();
    }
}
