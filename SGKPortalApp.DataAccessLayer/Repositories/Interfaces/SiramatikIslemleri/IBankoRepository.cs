using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri
{
    public interface IBankoRepository : IGenericRepository<Banko>
    {
        // Banko numarası ve departman-hizmet binası kombinasyonuyla bankoyu getirir
        Task<Banko?> GetByBankoNoAsync(int bankoNo, int departmanHizmetBinasiId);

        // Departman-hizmet binasındaki bankoları listeler
        Task<IEnumerable<Banko>> GetByDepartmanHizmetBinasiAsync(int departmanHizmetBinasiId);

        // Bankoyu departman-hizmet binası ile getirir
        Task<Banko?> GetWithDepartmanHizmetBinasiAsync(int bankoId);

        // Tüm bankoları departman-hizmet binası ile listeler
        Task<IEnumerable<Banko>> GetAllWithDepartmanHizmetBinasiAsync();

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

        // Departman-hizmet binası için dropdown bankoları listeler
        Task<IEnumerable<(int Id, string Ad)>> GetByDepartmanHizmetBinasiDropdownAsync(int departmanHizmetBinasiId);

        // Departman-hizmet binasındaki aktif banko sayısını getirir
        Task<int> GetActiveBankoCountAsync(int departmanHizmetBinasiId);

        // Banko tipine göre bankoları listeler
        Task<IEnumerable<Banko>> GetByBankoTipiAsync(BankoTipi bankoTipi);

        // Kat tipine göre bankoları listeler
        Task<IEnumerable<Banko>> GetByKatTipiAsync(KatTipi katTipi);

        // Boş bankoları listeler (personel atanmamış)
        Task<IEnumerable<Banko>> GetAvailableBankosAsync(int departmanHizmetBinasiId);

        // Bankoyu atanmış personel ile getirir
        Task<Banko?> GetWithPersonelAsync(int bankoId);

        // Departman-hizmet binası bazlı kat gruplu bankoları getirir
        Task<Dictionary<KatTipi, List<Banko>>> GetGroupedByKatAsync(int departmanHizmetBinasiId);
    }
}