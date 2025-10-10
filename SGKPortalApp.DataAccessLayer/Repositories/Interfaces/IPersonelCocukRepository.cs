using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces
{
    public interface IPersonelCocukRepository : IGenericRepository<PersonelCocuk>
    {
        Task<List<PersonelCocuk>> GetByPersonelTcKimlikNoAsync(string tcKimlikNo);
    }
}
