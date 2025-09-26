using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common
{
    public interface IIlceRepository : IGenericRepository<Ilce>
    {
        // Temel arama
        Task<Ilce?> GetByIlceAdiAsync(string ilceAdi);

        // İl bazında ilçeler
        Task<IEnumerable<Ilce>> GetByIlAsync(int ilId);

        // İl ile birlikte getir
        Task<Ilce?> GetWithIlAsync(int ilceId);

        // Listeleme
        Task<IEnumerable<Ilce>> GetAllAsync();

        // Dropdown için
        Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync();
        Task<IEnumerable<(int Id, string Ad)>> GetByIlDropdownAsync(int ilId);
    }
}