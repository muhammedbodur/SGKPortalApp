using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri
{
    public interface IBankoKullaniciRepository : IGenericRepository<BankoKullanici>
    {
        // Bankoya atanmış kullanıcıyı getirir
        Task<BankoKullanici?> GetByBankoAsync(int bankoId);

        // Personele atanmış banko kullanıcısını getirir
        Task<BankoKullanici?> GetByPersonelAsync(string tcKimlikNo);

        // Kullanıcıyı detaylı getirir
        Task<BankoKullanici?> GetWithDetailsAsync(int bankoKullaniciId);

        // Tüm kullanıcıları detaylı listeler
        Task<IEnumerable<BankoKullanici>> GetAllWithDetailsAsync();

        // Aktif atamaları listeler
        Task<IEnumerable<BankoKullanici>> GetActiveAssignmentsAsync();

        // Tüm atamaları listeler
        Task<IEnumerable<BankoKullanici>> GetAllAsync();

        // Tarih aralığına göre atamaları listeler
        Task<IEnumerable<BankoKullanici>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Banko atanmış mı kontrol eder
        Task<bool> IsBankoAssignedAsync(int bankoId);

        // Personel atanmış mı kontrol eder
        Task<bool> IsPersonelAssignedAsync(string tcKimlikNo);

        // Personeli bankosundan çıkarır
        Task UnassignPersonelAsync(string tcKimlikNo);

        // Bankoyu boşaltır
        Task UnassignBankoAsync(int bankoId);
    }
}