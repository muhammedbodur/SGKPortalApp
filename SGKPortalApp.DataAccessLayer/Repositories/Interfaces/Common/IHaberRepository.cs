using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common
{
    /// <summary>
    /// Haber Repository Interface
    /// </summary>
    public interface IHaberRepository : IGenericRepository<Haber>
    {
        /// <summary>
        /// Aktif haberleri sıraya göre getirir
        /// </summary>
        Task<IEnumerable<Haber>> GetActiveHaberlerAsync();

        /// <summary>
        /// Slider için görsel haberleri getirir (GorselUrl dolu olanlar)
        /// </summary>
        Task<IEnumerable<Haber>> GetSliderHaberlerAsync(int count = 5);

        /// <summary>
        /// Liste için haberleri getirir
        /// </summary>
        Task<IEnumerable<Haber>> GetListeHaberlerAsync(int count = 10);

        /// <summary>
        /// Yayın tarihine göre aktif haberleri kontrol eder
        /// </summary>
        Task<IEnumerable<Haber>> GetYayindakiHaberlerAsync();

        /// <summary>
        /// Haber detayını görselleriyle birlikte getirir
        /// </summary>
        Task<Haber?> GetByIdWithGorsellerAsync(int id);
    }
}
