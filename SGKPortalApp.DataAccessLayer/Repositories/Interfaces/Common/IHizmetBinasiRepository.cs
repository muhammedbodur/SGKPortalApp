using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common
{
    public interface IHizmetBinasiRepository : IGenericRepository<HizmetBinasi>
    {
        // Temel arama
        Task<HizmetBinasi?> GetByHizmetBinasiAdiAsync(string hizmetBinasiAdi);

        // Departman bazında binalar
        Task<IEnumerable<HizmetBinasi>> GetByDepartmanAsync(int departmanId);

        // Departman ile birlikte getir
        Task<HizmetBinasi?> GetWithDepartmanAsync(int hizmetBinasiId);

        // Listeleme
        Task<IEnumerable<HizmetBinasi>> GetActiveAsync();
        Task<IEnumerable<HizmetBinasi>> GetAllAsync();

        // Dropdown için
        Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync();
        Task<IEnumerable<(int Id, string Ad)>> GetByDepartmanDropdownAsync(int departmanId);

        // İstatistik
        Task<int> GetPersonelCountAsync(int hizmetBinasiId);
    }
}