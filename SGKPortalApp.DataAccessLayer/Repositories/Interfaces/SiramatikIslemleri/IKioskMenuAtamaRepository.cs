using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri
{
    public interface IKioskMenuAtamaRepository : IGenericRepository<KioskMenuAtama>
    {
        Task<IEnumerable<KioskMenuAtama>> GetByKioskAsync(int kioskId);
        Task<KioskMenuAtama?> GetActiveByKioskAsync(int kioskId);
        Task<KioskMenuAtama?> GetWithDetailsAsync(int kioskMenuAtamaId);
        Task<bool> HasActiveAtamaAsync(int kioskId);
        Task DeactivateOtherAtamasAsync(int kioskId, int currentAtamaId);
    }
}
