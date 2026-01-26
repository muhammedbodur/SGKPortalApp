using SGKPortalApp.BusinessObjectLayer.Entities.Legacy;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Legacy
{
    /// <summary>
    /// Legacy MySQL Birim tablosu için repository interface
    /// </summary>
    public interface ILegacyBirimRepository
    {
        /// <summary>
        /// Tüm birimleri getirir
        /// </summary>
        Task<List<LegacyBirim>> GetAllAsync(CancellationToken ct = default);
    }
}
