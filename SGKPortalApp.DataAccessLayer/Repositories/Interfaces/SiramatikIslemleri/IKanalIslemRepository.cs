using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri
{
    public interface IKanalIslemRepository : IGenericRepository<KanalIslem>
    {
        // Kanal bazında işlemleri listeler
        Task<IEnumerable<KanalIslem>> GetByKanalAsync(int kanalId);

        // Departman-Hizmet binası bazında işlemleri listeler
        Task<IEnumerable<KanalIslem>> GetByDepartmanHizmetBinasiAsync(int departmanHizmetBinasiId);

        // Tarih aralığına göre işlemleri listeler
        Task<IEnumerable<KanalIslem>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // İşlemi detaylı getirir
        Task<KanalIslem?> GetWithDetailsAsync(int kanalIslemId);

        // Tüm işlemleri detaylı listeler
        Task<IEnumerable<KanalIslem>> GetAllWithDetailsAsync();

        // Aktif işlemleri listeler
        Task<IEnumerable<KanalIslem>> GetActiveAsync();

        // Numara aralığına göre işlemleri listeler
        Task<IEnumerable<KanalIslem>> GetByNumaraAraligiAsync(int baslangicNumara, int bitisNumara);

        // Günlük işlem sayısını getirir
        Task<int> GetDailyIslemCountAsync(int kanalId, DateTime date);

        // Kanal ve departman-hizmet binası için maksimum sıra numarasını getirir
        Task<int> GetMaxSiraByKanalAndBinaAsync(int kanalId, int departmanHizmetBinasiId);
    }
}