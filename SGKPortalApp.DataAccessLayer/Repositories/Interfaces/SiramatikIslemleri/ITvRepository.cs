using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri
{
    public interface ITvRepository : IGenericRepository<Tv>
    {
        // Hizmet binası bazında TV'leri listeler
        Task<IEnumerable<Tv>> GetByHizmetBinasiAsync(int hizmetBinasiId);

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

        // Hizmet binası için dropdown TV'leri listeler
        Task<IEnumerable<(int Id, string Ad)>> GetByHizmetBinasiDropdownAsync(int hizmetBinasiId);
    }
}