using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common
{
    public interface IResmiTatilRepository : IGenericRepository<ResmiTatil>
    {
        /// <summary>
        /// Yıla göre resmi tatilleri getir
        /// </summary>
        Task<IEnumerable<ResmiTatil>> GetByYearAsync(int year);

        /// <summary>
        /// Belirli bir tarihin tatil olup olmadığını kontrol et
        /// </summary>
        Task<bool> IsHolidayAsync(DateTime date);

        /// <summary>
        /// Belirli bir tarihteki tatil adını getir
        /// </summary>
        Task<string?> GetHolidayNameAsync(DateTime date);

        /// <summary>
        /// Aktif resmi tatilleri getir
        /// </summary>
        Task<IEnumerable<ResmiTatil>> GetActiveAsync();

        /// <summary>
        /// Belirli bir tarihte tatil var mı kontrol et
        /// </summary>
        Task<bool> ExistsByDateAsync(DateTime date, int? excludeId = null);
    }
}
