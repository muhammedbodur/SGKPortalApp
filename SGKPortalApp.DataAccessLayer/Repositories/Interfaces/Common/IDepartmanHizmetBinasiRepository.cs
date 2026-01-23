using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common
{
    public interface IDepartmanHizmetBinasiRepository : IGenericRepository<DepartmanHizmetBinasi>
    {
        Task<IEnumerable<DepartmanHizmetBinasi>> GetByDepartmanAsync(int departmanId);
        Task<IEnumerable<DepartmanHizmetBinasi>> GetByHizmetBinasiAsync(int hizmetBinasiId);
        Task<DepartmanHizmetBinasi?> GetByDepartmanAndHizmetBinasiAsync(int departmanId, int hizmetBinasiId);
        Task<IEnumerable<DepartmanHizmetBinasi>> GetActiveAsync();
        Task<IEnumerable<(int Id, string DisplayText)>> GetDropdownAsync();
        Task<IEnumerable<(int Id, string DisplayText)>> GetDropdownByDepartmanAsync(int departmanId);
        Task<IEnumerable<(int Id, string DisplayText)>> GetDropdownByHizmetBinasiAsync(int hizmetBinasiId);
        Task<bool> ExistsAsync(int departmanId, int hizmetBinasiId);
    }
}
