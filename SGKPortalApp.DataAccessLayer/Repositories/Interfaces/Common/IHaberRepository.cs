using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common
{
    /// <summary>
    /// Haber (Duyuru) Repository - liste, detay, arama
    /// </summary>
    public interface IHaberRepository
    {
        /// <summary>
        /// Slider için vitrin fotoğrafı olan aktif haberleri getirir
        /// </summary>
        Task<IEnumerable<Duyuru>> GetSliderHaberleriAsync(int count = 5);

        /// <summary>
        /// Tüm aktif haberleri, optiyonel arama ile, sayfalama ile getirir
        /// </summary>
        Task<(IEnumerable<Duyuru> Items, int TotalCount)> GetHaberListeAsync(
            int pageNumber, int pageSize, string? searchTerm = null);

        /// <summary>
        /// Tek haber detayını getirir
        /// </summary>
        Task<Duyuru?> GetHaberByIdAsync(int haberId);

        /// <summary>
        /// Bir habere ait tüm resimlerini sıralı getirir
        /// </summary>
        Task<IEnumerable<DuyuruResim>> GetHaberResimleriAsync(int haberId);
    }
}
