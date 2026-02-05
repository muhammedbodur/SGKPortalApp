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
        Task<IEnumerable<Haber>> GetSliderHaberleriAsync(int count = 5);

        /// <summary>
        /// Tüm aktif haberleri, optiyonel arama ile, sayfalama ile getirir
        /// </summary>
        Task<(IEnumerable<Haber> Items, int TotalCount)> GetHaberListeAsync(
            int pageNumber, int pageSize, string? searchTerm = null);

        /// <summary>
        /// Tek haber detayını getirir
        /// </summary>
        Task<Haber?> GetHaberByIdAsync(int haberId);

        /// <summary>
        /// Bir habere ait tüm resimlerini sıralı getirir
        /// </summary>
        Task<IEnumerable<HaberResim>> GetHaberResimleriAsync(int haberId);

        // ─── CRUD ───────────────────────────────────────────

        Task<Haber> CreateAsync(Haber haber);
        Task<Haber> UpdateAsync(Haber haber);
        Task<bool> DeleteAsync(int haberId);

        /// <summary>
        /// Admin için tüm haberleri getirir (tarih/aktiflik filtresi yok, sadece SilindiMi=false)
        /// </summary>
        Task<(IEnumerable<Haber> Items, int TotalCount)> GetAllForAdminAsync(
            int pageNumber, int pageSize, string? searchTerm = null);

        /// <summary>
        /// Admin için tarih filtresi olmadan tek haber getir
        /// </summary>
        Task<Haber?> GetHaberByIdForAdminAsync(int haberId);

        Task<HaberResim> CreateResimAsync(HaberResim haberResim);
        Task<bool> DeleteResimAsync(int haberResimId);
        Task<HaberResim?> GetResimByIdAsync(int haberResimId);
    }
}
