using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri
{
    public interface IKioskMenuIslemRepository : IGenericRepository<KioskMenuIslem>
    {
        Task<IEnumerable<KioskMenuIslem>> GetByKioskMenuAsync(int kioskMenuId);
        Task<KioskMenuIslem?> GetWithDetailsAsync(int kioskMenuIslemId);
        Task<bool> ExistsByMenuAndSiraAsync(int kioskMenuId, int menuSira, int? excludeId = null);
        Task<int> GetMaxSiraByMenuAsync(int kioskMenuId);
    }
}
