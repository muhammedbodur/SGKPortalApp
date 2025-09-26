using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri
{
    public interface IPersonelRepository : IGenericRepository<Personel>
    {
        // Temel arama metodları
        Task<Personel?> GetByTcKimlikNoAsync(string tcKimlikNo);
        Task<Personel?> GetBySicilNoAsync(int sicilNo);

        // Listeleme metodları
        Task<IEnumerable<Personel>> GetByDepartmanAsync(int departmanId);
        Task<IEnumerable<Personel>> GetByUnvanAsync(int unvanId);
        Task<IEnumerable<Personel>> GetActivePersonelAsync();

        // Basit dropdown için
        Task<IEnumerable<(int Id, string AdSoyad)>> GetPersonelDropdownAsync();
    }
}