using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri
{
    public interface ISiraRepository : IGenericRepository<Sira>
    {
        // Sıra numarası ile sırayı getirir
        Task<Sira?> GetBySiraNoAsync(int siraNo, int hizmetBinasiId);

        // Personel bazında sıraları listeler
        Task<IEnumerable<Sira>> GetByPersonelAsync(string tcKimlikNo);

        // Personelin aktif sırasını getirir
        Task<Sira?> GetActiveByPersonelAsync(string tcKimlikNo);

        // Hizmet binası bazında sıraları listeler
        Task<IEnumerable<Sira>> GetByHizmetBinasiAsync(int hizmetBinasiId);

        // Kanal alt işlem bazında sıraları listeler
        Task<IEnumerable<Sira>> GetByKanalAltIslemAsync(int kanalAltIslemId);

        // Tarih aralığına göre sıraları listeler
        Task<IEnumerable<Sira>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Bugünkü sıraları listeler
        Task<IEnumerable<Sira>> GetTodayAsync();

        // Aktif sıraları listeler
        Task<IEnumerable<Sira>> GetActiveAsync();

        // Beklemedeki sıraları listeler
        Task<IEnumerable<Sira>> GetWaitingAsync();

        // Tamamlanmış sıraları listeler
        Task<IEnumerable<Sira>> GetCompletedAsync();

        // Sırayı detaylı getirir
        Task<Sira?> GetWithDetailsAsync(int siraId);

        // Tüm sıraları detaylı listeler
        Task<IEnumerable<Sira>> GetAllWithDetailsAsync();

        // Günlük sıra sayısını getirir
        Task<int> GetDailySiraCountAsync(DateTime date);

        // Beklemedeki sıra sayısını getirir
        Task<int> GetWaitingSiraCountAsync();

        // Tamamlanmış sıra sayısını getirir
        Task<int> GetCompletedSiraCountAsync(DateTime date);
    }
}