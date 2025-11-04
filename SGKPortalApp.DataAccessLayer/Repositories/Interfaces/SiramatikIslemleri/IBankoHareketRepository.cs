using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri
{
    public interface IBankoHareketRepository : IGenericRepository<BankoHareket>
    {
        // Bankoya ait hareketleri getirir
        Task<IEnumerable<BankoHareket>> GetByBankoAsync(int bankoId);

        // Personele ait hareketleri getirir
        Task<IEnumerable<BankoHareket>> GetByPersonelAsync(string tcKimlikNo);

        // Sıraya ait hareketleri getirir
        Task<IEnumerable<BankoHareket>> GetBySiraAsync(int siraId);

        // Tarih aralığına göre hareketleri getirir
        Task<IEnumerable<BankoHareket>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Banko ve tarih aralığına göre hareketleri getirir
        Task<IEnumerable<BankoHareket>> GetByBankoAndDateRangeAsync(int bankoId, DateTime startDate, DateTime endDate);

        // Personel ve tarih aralığına göre hareketleri getirir
        Task<IEnumerable<BankoHareket>> GetByPersonelAndDateRangeAsync(string tcKimlikNo, DateTime startDate, DateTime endDate);

        // Detaylı hareket getirir (tüm navigation property'ler ile)
        Task<BankoHareket?> GetWithDetailsAsync(long bankoHareketId);

        // Tüm hareketleri detaylı listeler
        Task<IEnumerable<BankoHareket>> GetAllWithDetailsAsync();

        // Tamamlanmamış hareketleri getirir (IslemBitisZamani NULL)
        Task<IEnumerable<BankoHareket>> GetIncompleteAsync();

        // Banko istatistiklerini getirir
        Task<(int ToplamIslem, int OrtalamaSure)> GetBankoStatsAsync(int bankoId, DateTime startDate, DateTime endDate);

        // Personel istatistiklerini getirir
        Task<(int ToplamIslem, int OrtalamaSure)> GetPersonelStatsAsync(string tcKimlikNo, DateTime startDate, DateTime endDate);
    }
}
