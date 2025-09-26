using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri
{
    public interface ITvBankoRepository : IGenericRepository<TvBanko>
    {
        // TV bazında banko eşleştirmelerini listeler
        Task<IEnumerable<TvBanko>> GetByTvAsync(int tvId);

        // Banko bazında TV eşleştirmelerini listeler
        Task<IEnumerable<TvBanko>> GetByBankoAsync(int bankoId);

        // Eşleştirmeyi detaylı getirir
        Task<TvBanko?> GetWithDetailsAsync(int tvBankoId);

        // Tüm eşleştirmeleri detaylı listeler
        Task<IEnumerable<TvBanko>> GetAllWithDetailsAsync();

        // Aktif eşleştirmeleri listeler
        Task<IEnumerable<TvBanko>> GetActiveAsync();
    }
}