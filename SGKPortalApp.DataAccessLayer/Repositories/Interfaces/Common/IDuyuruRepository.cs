using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common
{
    /// <summary>
    /// Duyuru Repository Interface
    /// </summary>
    public interface IDuyuruRepository : IGenericRepository<Duyuru>
    {
        /// <summary>
        /// Aktif duyuruları sıraya göre getirir
        /// </summary>
        Task<IEnumerable<Duyuru>> GetActiveDuyurularAsync();

        /// <summary>
        /// Slider için görsel duyuruları getirir (GorselUrl dolu olanlar)
        /// </summary>
        Task<IEnumerable<Duyuru>> GetSliderDuyurularAsync(int count = 5);

        /// <summary>
        /// Liste için duyuruları getirir
        /// </summary>
        Task<IEnumerable<Duyuru>> GetListeDuyurularAsync(int count = 10);

        /// <summary>
        /// Yayın tarihine göre aktif duyuruları kontrol eder
        /// </summary>
        Task<IEnumerable<Duyuru>> GetYayindakiDuyurularAsync();
    }
}
