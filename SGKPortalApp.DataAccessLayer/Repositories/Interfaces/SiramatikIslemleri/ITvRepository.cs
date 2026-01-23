using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri
{
    public interface ITvRepository : IGenericRepository<Tv>
    {
        // Departman-hizmet binası bazında TV'leri listeler
        Task<IEnumerable<Tv>> GetByDepartmanHizmetBinasiAsync(int departmanHizmetBinasiId);

        // TV'yi detaylı getirir
        Task<Tv?> GetWithDetailsAsync(int tvId);

        // Tüm TV'leri detaylı listeler
        Task<IEnumerable<Tv>> GetAllWithDetailsAsync();

        // Banko eşleşmiş TV'leri listeler
        Task<IEnumerable<Tv>> GetWithBankoAsync();

        // Bankoya atanmış TV'yi getirir
        Task<Tv?> GetByBankoAsync(int bankoId);

        // Kat tipine göre TV'leri listeler
        Task<IEnumerable<Tv>> GetByKatTipiAsync(KatTipi katTipi);

        // Aktif TV'leri listeler
        Task<IEnumerable<Tv>> GetActiveAsync();

        // Tüm TV'leri listeler
        Task<IEnumerable<Tv>> GetAllAsync();

        // Dropdown için TV'leri listeler
        Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync();

        // Departman-hizmet binası için dropdown TV'leri listeler
        Task<IEnumerable<(int Id, string Ad)>> GetByDepartmanHizmetBinasiDropdownAsync(int departmanHizmetBinasiId);

        // TV'ye bağlı bankoları getirir
        Task<IEnumerable<TvBanko>> GetTvBankolarAsync(int tvId);

        // Bankoya bağlı TV'leri getirir (TvBanko ilişkisi üzerinden)
        Task<IEnumerable<TvBanko>> GetTvBankolarByBankoIdAsync(int bankoId);
    }
}