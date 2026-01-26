using SGKPortalApp.BusinessObjectLayer.Entities.Legacy;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Legacy
{
    /// <summary>
    /// Legacy MySQL Servis tablosu için repository interface
    /// </summary>
    public interface ILegacyServisRepository
    {
        /// <summary>
        /// Tüm servisleri getirir
        /// </summary>
        Task<List<LegacyServis>> GetAllAsync(CancellationToken ct = default);
    }
}
