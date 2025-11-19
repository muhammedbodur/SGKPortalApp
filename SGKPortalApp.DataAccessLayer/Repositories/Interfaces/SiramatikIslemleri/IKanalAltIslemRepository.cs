using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri
{
    public interface IKanalAltIslemRepository : IGenericRepository<KanalAltIslem>
    {
        // Alt kanal bazında işlemleri listeler
        Task<IEnumerable<KanalAltIslem>> GetByKanalAltAsync(int kanalAltId);

        // Hizmet binası bazında işlemleri listeler
        Task<IEnumerable<KanalAltIslem>> GetByHizmetBinasiAsync(int hizmetBinasiId);

        // Kanal işlemi bazında işlemleri listeler
        Task<IEnumerable<KanalAltIslem>> GetByKanalIslemAsync(int kanalIslemId);

        // Tarih aralığına göre işlemleri listeler
        Task<IEnumerable<KanalAltIslem>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // İşlemi detaylı getirir
        Task<KanalAltIslem?> GetWithDetailsAsync(int kanalAltIslemId);

        // Tüm işlemleri detaylı listeler
        Task<IEnumerable<KanalAltIslem>> GetAllWithDetailsAsync();

        // Aktif işlemleri listeler
        Task<IEnumerable<KanalAltIslem>> GetActiveAsync();
    }
}