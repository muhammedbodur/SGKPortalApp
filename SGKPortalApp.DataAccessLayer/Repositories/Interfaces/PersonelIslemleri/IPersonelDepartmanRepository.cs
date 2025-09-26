using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri
{
    public interface IPersonelDepartmanRepository : IGenericRepository<PersonelDepartman>
    {
        // Personel bazında departman geçmişi
        Task<IEnumerable<PersonelDepartman>> GetByPersonelTcAsync(string tcKimlikNo);

        // Departman bazında personeller
        Task<IEnumerable<PersonelDepartman>> GetByDepartmanAsync(int departmanId);

        // Aktif atamalar
        Task<IEnumerable<PersonelDepartman>> GetActiveAssignmentsAsync();

        // Personelin aktif departmanı
        Task<PersonelDepartman?> GetActiveAssignmentByPersonelAsync(string tcKimlikNo);
    }
}