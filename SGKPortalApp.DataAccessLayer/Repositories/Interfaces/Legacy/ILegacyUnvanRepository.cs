using SGKPortalApp.BusinessObjectLayer.Entities.Legacy;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Legacy
{
    /// <summary>
    /// Legacy MySQL Unvan tablosu için repository interface
    /// </summary>
    public interface ILegacyUnvanRepository
    {
        /// <summary>
        /// Tüm ünvanları getirir
        /// </summary>
        Task<List<LegacyUnvan>> GetAllAsync(CancellationToken ct = default);
    }
}
