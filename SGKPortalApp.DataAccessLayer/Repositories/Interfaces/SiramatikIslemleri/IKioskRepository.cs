using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri
{
    public interface IKioskRepository : IGenericRepository<Kiosk>
    {
        Task<IEnumerable<Kiosk>> GetByHizmetBinasiAsync(int hizmetBinasiId);

        Task<Kiosk?> GetWithMenuAsync(int kioskId);

        Task<IEnumerable<Kiosk>> GetWithMenuAsync(IEnumerable<int> kioskIds);

        Task<IEnumerable<Kiosk>> GetActiveAsync();

        Task<bool> ExistsByNameAsync(string kioskAdi, int hizmetBinasiId);
    }
}
