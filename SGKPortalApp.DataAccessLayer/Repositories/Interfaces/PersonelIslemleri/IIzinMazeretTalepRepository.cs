using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SGKPortalApp.BusinessObjectLayer.Entities.PdksIslemleri;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri
{
    /// <summary>
    /// İzin/Mazeret talepleri repository interface
    /// </summary>
    public interface IIzinMazeretTalepRepository : IGenericRepository<IzinMazeretTalep>
    {
        // ═══════════════════════════════════════════════════════
        // PERSONEL BAZINDA SORGULAR
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Personele ait tüm izin/mazeret talepleri
        /// </summary>
        Task<IEnumerable<IzinMazeretTalep>> GetByPersonelTcAsync(string tcKimlikNo, bool includeInactive = false);

        /// <summary>
        /// Personele ait bekleyen talepler (onay bekleyenler)
        /// </summary>
        Task<IEnumerable<IzinMazeretTalep>> GetPendingByPersonelTcAsync(string tcKimlikNo);

        /// <summary>
        /// Personele ait onaylanmış talepler
        /// </summary>
        Task<IEnumerable<IzinMazeretTalep>> GetApprovedByPersonelTcAsync(string tcKimlikNo, DateTime? startDate = null, DateTime? endDate = null);

        // ═══════════════════════════════════════════════════════
        // TARİH ÇAKIŞMASI KONTROLLERI (ÖNEMLİ!)
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Belirtilen tarih aralığında personelin başka bir izni/mazereti var mı?
        /// Çakışma kontrolü için kullanılır
        /// </summary>
        Task<bool> HasOverlappingRequestAsync(
            string tcKimlikNo,
            DateTime? baslangicTarihi,
            DateTime? bitisTarihi,
            DateTime? mazeretTarihi,
            int? excludeTalepId = null);

        /// <summary>
        /// Belirtilen tarih aralığındaki mevcut izin/mazeret talepleri
        /// </summary>
        Task<IEnumerable<IzinMazeretTalep>> GetOverlappingRequestsAsync(
            string tcKimlikNo,
            DateTime? baslangicTarihi,
            DateTime? bitisTarihi,
            DateTime? mazeretTarihi,
            int? excludeTalepId = null);

        // ═══════════════════════════════════════════════════════
        // ONAY İŞLEMLERİ
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// 1. Onayci için bekleyen talepler
        /// </summary>
        Task<IEnumerable<IzinMazeretTalep>> GetPendingForFirstApproverAsync(string onayciTcKimlikNo);

        /// <summary>
        /// 2. Onayci için bekleyen talepler
        /// </summary>
        Task<IEnumerable<IzinMazeretTalep>> GetPendingForSecondApproverAsync(string onayciTcKimlikNo);

        /// <summary>
        /// Departman bazında bekleyen talepler (yönetici görünümü)
        /// </summary>
        Task<IEnumerable<IzinMazeretTalep>> GetPendingByDepartmanAsync(int departmanId);

        /// <summary>
        /// Servis bazında bekleyen talepler
        /// </summary>
        Task<IEnumerable<IzinMazeretTalep>> GetPendingByServisAsync(int servisId);

        // ═══════════════════════════════════════════════════════
        // RAPORLAMA VE FİLTRELEME
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Gelişmiş filtreleme ile talepleri getir (raporlama için)
        /// </summary>
        Task<(IEnumerable<IzinMazeretTalep> Items, int TotalCount)> GetFilteredAsync(
            string? tcKimlikNo = null,
            int? departmanId = null,
            int? servisId = null,
            IzinMazeretTuru? turu = null,
            OnayDurumu? birinciOnayDurumu = null,
            OnayDurumu? ikinciOnayDurumu = null,
            DateTime? baslangicTarihiMin = null,
            DateTime? baslangicTarihiMax = null,
            DateTime? talepTarihiMin = null,
            DateTime? talepTarihiMax = null,
            bool? isActive = null,
            int pageNumber = 1,
            int pageSize = 50,
            string? sortBy = null,
            bool sortDescending = false);

        /// <summary>
        /// Tarih aralığındaki tüm izin/mazeret talepleri (rapor için)
        /// </summary>
        Task<IEnumerable<IzinMazeretTalep>> GetByDateRangeAsync(
            DateTime startDate,
            DateTime endDate,
            int? departmanId = null,
            int? servisId = null);

        // ═════════════════════════════════════════════════════════
        // İSTATİSTİKLER
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Personelin yıllık izin kullanım sayısı
        /// </summary>
        Task<int> GetTotalYillikIzinDaysAsync(string tcKimlikNo, int year);

        /// <summary>
        /// Personelin toplam kullanılan izin günü (türe göre)
        /// </summary>
        Task<int> GetTotalUsedDaysAsync(string tcKimlikNo, IzinMazeretTuru? turu = null, int? year = null);

        /// <summary>
        /// Departman bazında izin istatistiği
        /// </summary>
        Task<Dictionary<IzinMazeretTuru, int>> GetDepartmanStatisticsAsync(int departmanId, int year);
    }
}
