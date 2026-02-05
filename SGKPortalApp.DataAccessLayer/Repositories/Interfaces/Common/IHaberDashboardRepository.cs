using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common
{
    /// <summary>
    /// Dashboard Haber Repository Interface (slider ve liste için)
    /// </summary>
    public interface IHaberDashboardRepository : IGenericRepository<Haber>
    {
        /// <summary>
        /// Aktif haberleri sıraya göre getirir
        /// </summary>
        Task<IEnumerable<Haber>> GetActiveHaberleriAsync();

        /// <summary>
        /// Slider için görsel haberleri getirir (GorselUrl dolu olanlar)
        /// </summary>
        Task<IEnumerable<Haber>> GetSliderHaberleriAsync(int count = 5);

        /// <summary>
        /// Liste için haberleri getirir
        /// </summary>
        Task<IEnumerable<Haber>> GetListeHaberleriAsync(int count = 10);

        /// <summary>
        /// Yayın tarihine göre aktif haberleri kontrol eder
        /// </summary>
        Task<IEnumerable<Haber>> GetYayindakiHaberleriAsync();
    }
}
