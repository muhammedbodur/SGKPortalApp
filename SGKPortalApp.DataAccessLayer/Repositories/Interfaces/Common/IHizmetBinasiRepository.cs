using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common
{
    public interface IHizmetBinasiRepository : IGenericRepository<HizmetBinasi>
    {
        /// <summary>
        /// Hizmet Binası adına göre arama (unique kontrol için)
        /// </summary>
        Task<HizmetBinasi?> GetByHizmetBinasiAdiAsync(string hizmetBinasiAdi);

        /// <summary>
        /// Departmana göre hizmet binalarını getir
        /// </summary>
        Task<IEnumerable<HizmetBinasi>> GetByDepartmanAsync(int departmanId);

        /// <summary>
        /// Hizmet Binasını departman bilgisi ile birlikte getir
        /// </summary>
        Task<HizmetBinasi?> GetWithDepartmanAsync(int hizmetBinasiId);

        /// <summary>
        /// Hizmet Binasını personelleri ile birlikte getir
        /// </summary>
        Task<HizmetBinasi?> GetWithPersonellerAsync(int hizmetBinasiId);

        /// <summary>
        /// Hizmet Binasını bankoları ile birlikte getir
        /// </summary>
        Task<HizmetBinasi?> GetWithBankolarAsync(int hizmetBinasiId);

        /// <summary>
        /// Hizmet Binasını TV'leri ile birlikte getir
        /// </summary>
        Task<HizmetBinasi?> GetWithTvlerAsync(int hizmetBinasiId);

        /// <summary>
        /// Hizmet Binasını tüm ilişkili verilerle birlikte getir (Detail için)
        /// </summary>
        Task<HizmetBinasi?> GetWithAllDetailsAsync(int hizmetBinasiId);

        /// <summary>
        /// Aktif hizmet binalarını getir
        /// </summary>
        Task<IEnumerable<HizmetBinasi>> GetActiveAsync();

        /// <summary>
        /// Hizmet Binasındaki personel sayısını getir
        /// </summary>
        Task<int> GetPersonelCountAsync(int hizmetBinasiId);

        /// <summary>
        /// Hizmet Binasındaki banko sayısını getir
        /// </summary>
        Task<int> GetBankoCountAsync(int hizmetBinasiId);

        /// <summary>
        /// Hizmet Binasındaki TV sayısını getir
        /// </summary>
        Task<int> GetTvCountAsync(int hizmetBinasiId);

        /// <summary>
        /// Dropdown için ID ve Ad listesi
        /// </summary>
        Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync();
    }
}