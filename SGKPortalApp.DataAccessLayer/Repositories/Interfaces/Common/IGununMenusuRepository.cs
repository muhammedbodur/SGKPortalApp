using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common
{
    /// <summary>
    /// Günün Menüsü Repository Interface
    /// </summary>
    public interface IGununMenusuRepository : IGenericRepository<GununMenusu>
    {
        /// <summary>
        /// Bugünün menüsünü getirir
        /// </summary>
        Task<GununMenusu?> GetTodaysMenuAsync();

        /// <summary>
        /// Belirli bir tarihin menüsünü getirir
        /// </summary>
        Task<GununMenusu?> GetMenuByDateAsync(DateTime date);

        /// <summary>
        /// Tarih aralığındaki menüleri getirir
        /// </summary>
        Task<IEnumerable<GununMenusu>> GetMenusByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
