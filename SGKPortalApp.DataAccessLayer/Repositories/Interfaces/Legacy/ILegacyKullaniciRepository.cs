using SGKPortalApp.BusinessObjectLayer.Entities.Legacy;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Legacy
{
    /// <summary>
    /// Legacy MySQL Kullanici tablosu için repository interface
    /// </summary>
    public interface ILegacyKullaniciRepository
    {
        /// <summary>
        /// Geçerli TC Kimlik No'ya sahip tüm kullanıcıları getirir
        /// </summary>
        Task<List<LegacyKullanici>> GetAllActiveAsync(CancellationToken ct = default);
    }
}
