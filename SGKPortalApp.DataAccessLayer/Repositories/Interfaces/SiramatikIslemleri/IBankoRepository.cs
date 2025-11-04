using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri
{
    public interface IBankoRepository : IGenericRepository<Banko>
    {
        // Banko numarası ve hizmet binasıyla bankoyu getirir
        Task<Banko?> GetByBankoNoAsync(int bankoNo, int hizmetBinasiId);

        // Hizmet binasındaki bankoları listeler
        Task<IEnumerable<Banko>> GetByHizmetBinasiAsync(int hizmetBinasiId);

        // Bankoyu hizmet binası ile getirir
        Task<Banko?> GetWithHizmetBinasiAsync(int bankoId);

        // Tüm bankoları hizmet binası ile listeler
        Task<IEnumerable<Banko>> GetAllWithHizmetBinasiAsync();

        // Kullanıcı atanmış bankoları listeler
        Task<IEnumerable<Banko>> GetWithKullaniciAsync();

        // Personelin bankosunu getirir
        Task<Banko?> GetByKullaniciAsync(string tcKimlikNo);

        // Aktif bankoları listeler
        Task<IEnumerable<Banko>> GetActiveAsync();

        // Tüm bankoları listeler
        Task<IEnumerable<Banko>> GetAllAsync();

        // Dropdown için bankoları listeler
        Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync();

        // Hizmet binası için dropdown bankoları listeler
        Task<IEnumerable<(int Id, string Ad)>> GetByHizmetBinasiDropdownAsync(int hizmetBinasiId);

        // Hizmet binasındaki aktif banko sayısını getirir
        Task<int> GetActiveBankoCountAsync(int hizmetBinasiId);

        // Banko tipine göre bankoları listeler
        Task<IEnumerable<Banko>> GetByBankoTipiAsync(BankoTipi bankoTipi);

        // Kat tipine göre bankoları listeler
        Task<IEnumerable<Banko>> GetByKatTipiAsync(KatTipi katTipi);

        // Boş bankoları listeler (personel atanmamış)
        Task<IEnumerable<Banko>> GetAvailableBankosAsync(int hizmetBinasiId);

        // Bankoyu atanmış personel ile getirir
        Task<Banko?> GetWithPersonelAsync(int bankoId);

        // Bina bazlı kat gruplu bankoları getirir
        Task<Dictionary<KatTipi, List<Banko>>> GetGroupedByKatAsync(int hizmetBinasiId);
    }
}