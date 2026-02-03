using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri
{
    public interface IPersonelRepository : IGenericRepository<Personel>
    {
        // Temel arama methodları
        Task<Personel?> GetByTcKimlikNoAsync(string tcKimlikNo);
        Task<IEnumerable<Personel>> GetByTcKimlikNosAsync(IEnumerable<string> tcKimlikNos);
        Task<Personel?> GetBySicilNoAsync(int sicilNo);
        Task<Personel?> GetByPersonelKayitNoAsync(int personelKayitNo);
        Task<Personel?> GetByTcKimlikNoWithDetailsAsync(string tcKimlikNo);

        // Listeleme.methodları
        Task<IEnumerable<Personel>> GetByDepartmanAsync(int departmanId);
        Task<IEnumerable<Personel>> GetByServisAsync(int servisId);
        Task<IEnumerable<Personel>> GetByUnvanAsync(int unvanId);
        Task<IEnumerable<Personel>> GetByHizmetBinasiIdAsync(int hizmetBinasiId);
        Task<IEnumerable<Personel>> GetActivePersonelAsync();
        Task<IEnumerable<Personel>> GetActiveAsync();
        Task<IEnumerable<Personel>> GetAllWithDetailsAsync();

        // Sayfalama
        Task<PagedResponseDto<PersonelListResponseDto>> GetPagedAsync(PersonelFilterRequestDto filter);

        // Basit dropdown için
        Task<IEnumerable<(int Id, string AdSoyad)>> GetPersonelDropdownAsync();

        // EnrollNumber yönetimi
        Task<string> GetNextAvailableEnrollNumberAsync();

        // Dashboard için bugün doğanlar
        Task<IEnumerable<Personel>> GetBugunDoganlarAsync();
    }
}