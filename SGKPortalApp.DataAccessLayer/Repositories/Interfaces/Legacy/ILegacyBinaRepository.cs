using SGKPortalApp.BusinessObjectLayer.Entities.Legacy;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Legacy
{
    /// <summary>
    /// Legacy MySQL Bina tablosu için repository interface
    /// </summary>
    public interface ILegacyBinaRepository
    {
        /// <summary>
        /// Tüm binaları getirir
        /// </summary>
        Task<List<LegacyBina>> GetAllAsync(CancellationToken ct = default);
    }
}
