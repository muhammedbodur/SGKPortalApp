using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common
{
    /// <summary>
    /// Sık Kullanılan Program Repository Interface
    /// </summary>
    public interface ISikKullanilanProgramRepository : IGenericRepository<SikKullanilanProgram>
    {
        /// <summary>
        /// Aktif programları sıraya göre getirir
        /// </summary>
        Task<IEnumerable<SikKullanilanProgram>> GetActiveProgramsAsync();

        /// <summary>
        /// Dashboard için sık kullanılan programları getirir
        /// </summary>
        Task<IEnumerable<SikKullanilanProgram>> GetDashboardProgramsAsync(int count = 8);
    }
}
