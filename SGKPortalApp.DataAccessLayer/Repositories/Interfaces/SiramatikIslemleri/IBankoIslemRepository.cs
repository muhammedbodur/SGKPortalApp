using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri
{
    public interface IBankoIslemRepository : IGenericRepository<BankoIslem>
    {
        // Banko grubuna göre işlemleri listeler
        Task<IEnumerable<BankoIslem>> GetByBankoGrupAsync(BankoGrup bankoGrup);

        // Üst işlem ID'sine göre işlemleri listeler
        Task<IEnumerable<BankoIslem>> GetByUstIslemAsync(int bankoUstIslemId);

        // Tarih aralığına göre işlemleri listeler
        Task<IEnumerable<BankoIslem>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // İşlemi detaylı getirir
        Task<BankoIslem?> GetWithDetailsAsync(int bankoIslemId);

        // Tüm işlemleri detaylı listeler
        Task<IEnumerable<BankoIslem>> GetAllWithDetailsAsync();

        // Aktif işlemleri listeler
        Task<IEnumerable<BankoIslem>> GetActiveAsync();

        // Dropdown için işlemleri listeler
        Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync();

        // Günlük işlem sayısını getirir
        Task<int> GetDailyIslemCountAsync(DateTime date);
    }
}