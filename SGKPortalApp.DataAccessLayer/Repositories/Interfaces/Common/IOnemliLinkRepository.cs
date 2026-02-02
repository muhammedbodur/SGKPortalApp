using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common
{
    /// <summary>
    /// Önemli Link Repository Interface
    /// </summary>
    public interface IOnemliLinkRepository : IGenericRepository<OnemliLink>
    {
        /// <summary>
        /// Aktif linkleri sıraya göre getirir
        /// </summary>
        Task<IEnumerable<OnemliLink>> GetActiveLinksAsync();

        /// <summary>
        /// Dashboard için önemli linkleri getirir
        /// </summary>
        Task<IEnumerable<OnemliLink>> GetDashboardLinksAsync(int count = 10);
    }
}
