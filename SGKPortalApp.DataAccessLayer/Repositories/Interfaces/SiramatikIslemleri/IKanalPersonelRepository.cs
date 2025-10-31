using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri
{
    public interface IKanalPersonelRepository : IGenericRepository<KanalPersonel>
    {
        // Personel bazında atamaları listeler
        Task<IEnumerable<KanalPersonel>> GetByPersonelAsync(string tcKimlikNo);

        // Kanal alt işlem bazında personelleri listeler
        Task<IEnumerable<KanalPersonel>> GetByKanalAltIslemAsync(int kanalAltIslemId);

        // Hizmet binası bazında atamalari listeler
        Task<IEnumerable<KanalPersonel>> GetByHizmetBinasiIdAsync(int hizmetBinasiId);

        // Atamayı detaylı getirir
        Task<KanalPersonel?> GetWithDetailsAsync(int kanalPersonelId);

        // Tüm atamaları detaylı listeler
        Task<IEnumerable<KanalPersonel>> GetAllWithDetailsAsync();

        // Aktif atamaları listeler
        Task<IEnumerable<KanalPersonel>> GetActiveAssignmentsAsync();

        // Tüm atamaları listeler
        Task<IEnumerable<KanalPersonel>> GetAllAsync();

        // Çakışma kontrolü yapar
        Task<bool> HasConflictAsync(string tcKimlikNo, int kanalAltIslemId);

        // Pasif veya silinmiş kayıt var mı kontrol eder
        Task<KanalPersonel?> GetInactiveRecordAsync(string tcKimlikNo, int kanalAltIslemId);

        // Uzmanlık bazında personelleri listeler
        Task<IEnumerable<KanalPersonel>> GetByUzmanlikAsync(PersonelUzmanlik uzmanlik);
    }
}