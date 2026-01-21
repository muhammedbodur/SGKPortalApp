using Microsoft.EntityFrameworkCore;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SGKPortalApp.BusinessObjectLayer.Entities.PdksIslemleri;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.PersonelIslemleri
{
    /// <summary>
    /// İzin/Mazeret talepleri repository implementation
    /// Çakışma kontrolü, filtreleme ve raporlama metodları içerir
    /// </summary>
    public class IzinMazeretTalepRepository : GenericRepository<IzinMazeretTalep>, IIzinMazeretTalepRepository
    {
        public IzinMazeretTalepRepository(SGKDbContext context) : base(context)
        {
        }

        // ═══════════════════════════════════════════════════════
        // PERSONEL BAZINDA SORGULAR
        // ═══════════════════════════════════════════════════════

        public async Task<IEnumerable<IzinMazeretTalep>> GetByPersonelTcAsync(string tcKimlikNo, bool includeInactive = false)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return Enumerable.Empty<IzinMazeretTalep>();

            var query = _dbSet
                .AsNoTracking()
                .Include(t => t.Personel)
                .Include(t => t.BirinciOnayci)
                .Include(t => t.IkinciOnayci)
                .Where(t => t.TcKimlikNo == tcKimlikNo && !t.SilindiMi);

            if (!includeInactive)
            {
                query = query.Where(t => t.IsActive);
            }

            return await query
                .OrderByDescending(t => t.TalepTarihi)
                .ToListAsync();
        }

        public async Task<IEnumerable<IzinMazeretTalep>> GetPendingByPersonelTcAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return Enumerable.Empty<IzinMazeretTalep>();

            return await _dbSet
                .AsNoTracking()
                .Include(t => t.Personel)
                .Where(t => t.TcKimlikNo == tcKimlikNo &&
                           !t.SilindiMi &&
                           t.IsActive &&
                           (t.BirinciOnayDurumu == OnayDurumu.Beklemede ||
                            t.IkinciOnayDurumu == OnayDurumu.Beklemede))
                .OrderByDescending(t => t.TalepTarihi)
                .ToListAsync();
        }

        public async Task<IEnumerable<IzinMazeretTalep>> GetApprovedByPersonelTcAsync(
            string tcKimlikNo,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return Enumerable.Empty<IzinMazeretTalep>();

            var query = _dbSet
                .AsNoTracking()
                .Include(t => t.Personel)
                .Where(t => t.TcKimlikNo == tcKimlikNo &&
                           !t.SilindiMi &&
                           t.IsActive &&
                           t.BirinciOnayDurumu == OnayDurumu.Onaylandi &&
                           t.IkinciOnayDurumu == OnayDurumu.Onaylandi);

            if (startDate.HasValue)
            {
                query = query.Where(t =>
                    (t.BaslangicTarihi >= startDate.Value) ||
                    (t.MazeretTarihi >= startDate.Value));
            }

            if (endDate.HasValue)
            {
                query = query.Where(t =>
                    (t.BitisTarihi <= endDate.Value) ||
                    (t.MazeretTarihi <= endDate.Value));
            }

            return await query
                .OrderByDescending(t => t.BaslangicTarihi ?? t.MazeretTarihi)
                .ToListAsync();
        }

        // ═══════════════════════════════════════════════════════
        // TARİH ÇAKIŞMASI KONTROLLERI (ÖNEMLİ!)
        // ═══════════════════════════════════════════════════════

        public async Task<bool> HasOverlappingRequestAsync(
            string tcKimlikNo,
            DateTime? baslangicTarihi,
            DateTime? bitisTarihi,
            DateTime? mazeretTarihi,
            int? excludeTalepId = null)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return false;

            var overlapping = await GetOverlappingRequestsAsync(
                tcKimlikNo,
                baslangicTarihi,
                bitisTarihi,
                mazeretTarihi,
                excludeTalepId);

            return overlapping.Any();
        }

        public async Task<IEnumerable<IzinMazeretTalep>> GetOverlappingRequestsAsync(
            string tcKimlikNo,
            DateTime? baslangicTarihi,
            DateTime? bitisTarihi,
            DateTime? mazeretTarihi,
            int? excludeTalepId = null)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return Enumerable.Empty<IzinMazeretTalep>();

            var query = _dbSet
                .AsNoTracking()
                .Where(t => t.TcKimlikNo == tcKimlikNo &&
                           !t.SilindiMi &&
                           t.IsActive &&
                           // Sadece onaylanmış veya bekleyen talepleri kontrol et
                           (t.BirinciOnayDurumu != OnayDurumu.Reddedildi &&
                            t.IkinciOnayDurumu != OnayDurumu.Reddedildi));

            if (excludeTalepId.HasValue)
            {
                query = query.Where(t => t.IzinMazeretTalepId != excludeTalepId.Value);
            }

            // İzin için çakışma kontrolü (tarih aralığı)
            if (baslangicTarihi.HasValue && bitisTarihi.HasValue)
            {
                query = query.Where(t =>
                    // Mevcut talep de izin ise
                    (t.BaslangicTarihi.HasValue && t.BitisTarihi.HasValue &&
                     ((baslangicTarihi <= t.BitisTarihi && bitisTarihi >= t.BaslangicTarihi))) ||
                    // Mevcut talep mazeret ise ve tarih çakışıyor
                    (t.MazeretTarihi.HasValue &&
                     t.MazeretTarihi >= baslangicTarihi &&
                     t.MazeretTarihi <= bitisTarihi));
            }
            // Mazeret için çakışma kontrolü (tek gün)
            else if (mazeretTarihi.HasValue)
            {
                query = query.Where(t =>
                    // Mevcut talep izin ise ve mazeret tarihi izin aralığında
                    (t.BaslangicTarihi.HasValue && t.BitisTarihi.HasValue &&
                     mazeretTarihi >= t.BaslangicTarihi &&
                     mazeretTarihi <= t.BitisTarihi) ||
                    // Mevcut talep de mazeret ise ve aynı gün
                    (t.MazeretTarihi.HasValue && t.MazeretTarihi == mazeretTarihi));
            }

            return await query.ToListAsync();
        }

        // ═══════════════════════════════════════════════════════
        // ONAY İŞLEMLERİ
        // ═══════════════════════════════════════════════════════

        public async Task<IEnumerable<IzinMazeretTalep>> GetPendingForFirstApproverAsync(string onayciTcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(onayciTcKimlikNo))
                return Enumerable.Empty<IzinMazeretTalep>();

            return await _dbSet
                .AsNoTracking()
                .Include(t => t.Personel)
                    .ThenInclude(p => p!.Departman)
                .Include(t => t.Personel)
                    .ThenInclude(p => p!.Servis)
                .Include(t => t.BirinciOnayci)
                .Include(t => t.IkinciOnayci)
                .Where(t => t.BirinciOnayciTcKimlikNo == onayciTcKimlikNo &&
                           !t.SilindiMi &&
                           t.IsActive &&
                           t.BirinciOnayDurumu == OnayDurumu.Beklemede)
                .OrderBy(t => t.TalepTarihi)
                .ToListAsync();
        }

        public async Task<IEnumerable<IzinMazeretTalep>> GetPendingForSecondApproverAsync(string onayciTcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(onayciTcKimlikNo))
                return Enumerable.Empty<IzinMazeretTalep>();

            return await _dbSet
                .AsNoTracking()
                .Include(t => t.Personel)
                    .ThenInclude(p => p!.Departman)
                .Include(t => t.Personel)
                    .ThenInclude(p => p!.Servis)
                .Include(t => t.BirinciOnayci)
                .Include(t => t.IkinciOnayci)
                .Where(t => t.IkinciOnayciTcKimlikNo == onayciTcKimlikNo &&
                           !t.SilindiMi &&
                           t.IsActive &&
                           t.IkinciOnayDurumu == OnayDurumu.Beklemede)
                .OrderBy(t => t.TalepTarihi)
                .ToListAsync();
        }

        public async Task<IEnumerable<IzinMazeretTalep>> GetPendingByDepartmanAsync(int departmanId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(t => t.Personel)
                    .ThenInclude(p => p!.Departman)
                .Include(t => t.Personel)
                    .ThenInclude(p => p!.Servis)
                .Where(t => t.Personel!.DepartmanId == departmanId &&
                           !t.SilindiMi &&
                           t.IsActive &&
                           (t.BirinciOnayDurumu == OnayDurumu.Beklemede ||
                            t.IkinciOnayDurumu == OnayDurumu.Beklemede))
                .OrderBy(t => t.TalepTarihi)
                .ToListAsync();
        }

        public async Task<IEnumerable<IzinMazeretTalep>> GetPendingByServisAsync(int servisId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(t => t.Personel)
                    .ThenInclude(p => p!.Departman)
                .Include(t => t.Personel)
                    .ThenInclude(p => p!.Servis)
                .Where(t => t.Personel!.ServisId == servisId &&
                           !t.SilindiMi &&
                           t.IsActive &&
                           (t.BirinciOnayDurumu == OnayDurumu.Beklemede ||
                            t.IkinciOnayDurumu == OnayDurumu.Beklemede))
                .OrderBy(t => t.TalepTarihi)
                .ToListAsync();
        }

        // ═══════════════════════════════════════════════════════
        // RAPORLAMA VE FİLTRELEME
        // ═══════════════════════════════════════════════════════

        public async Task<(IEnumerable<IzinMazeretTalep> Items, int TotalCount)> GetFilteredAsync(
            string? tcKimlikNo = null,
            int? departmanId = null,
            int? servisId = null,
            int? izinMazeretTuruId = null,
            OnayDurumu? birinciOnayDurumu = null,
            OnayDurumu? ikinciOnayDurumu = null,
            DateTime? baslangicTarihiMin = null,
            DateTime? baslangicTarihiMax = null,
            DateTime? talepTarihiMin = null,
            DateTime? talepTarihiMax = null,
            bool? isActive = null,
            bool? izinIslendiMi = null,
            int pageNumber = 1,
            int pageSize = 50,
            string? sortBy = null,
            bool sortDescending = false)
        {
            var query = _dbSet
                .AsNoTracking()
                .Include(t => t.Personel)
                    .ThenInclude(p => p!.Departman)
                .Include(t => t.Personel)
                    .ThenInclude(p => p!.Servis)
                .Include(t => t.BirinciOnayci)
                .Include(t => t.IkinciOnayci)
                .Include(t => t.IzinMazeretTuru)
                .Where(t => !t.SilindiMi);

            // Filtreler
            if (!string.IsNullOrWhiteSpace(tcKimlikNo))
            {
                query = query.Where(t => t.TcKimlikNo == tcKimlikNo);
            }

            if (departmanId.HasValue)
            {
                query = query.Where(t => t.Personel!.DepartmanId == departmanId.Value);
            }

            if (servisId.HasValue)
            {
                query = query.Where(t => t.Personel!.ServisId == servisId.Value);
            }

            if (izinMazeretTuruId.HasValue)
            {
                query = query.Where(t => t.IzinMazeretTuruId == izinMazeretTuruId.Value);
            }

            if (birinciOnayDurumu.HasValue)
            {
                query = query.Where(t => t.BirinciOnayDurumu == birinciOnayDurumu.Value);
            }

            if (ikinciOnayDurumu.HasValue)
            {
                query = query.Where(t => t.IkinciOnayDurumu == ikinciOnayDurumu.Value);
            }

            if (baslangicTarihiMin.HasValue)
            {
                query = query.Where(t =>
                    (t.BaslangicTarihi >= baslangicTarihiMin.Value) ||
                    (t.MazeretTarihi >= baslangicTarihiMin.Value));
            }

            if (baslangicTarihiMax.HasValue)
            {
                query = query.Where(t =>
                    (t.BaslangicTarihi <= baslangicTarihiMax.Value) ||
                    (t.MazeretTarihi <= baslangicTarihiMax.Value));
            }

            if (talepTarihiMin.HasValue)
            {
                query = query.Where(t => t.TalepTarihi >= talepTarihiMin.Value);
            }

            if (talepTarihiMax.HasValue)
            {
                query = query.Where(t => t.TalepTarihi <= talepTarihiMax.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(t => t.IsActive == isActive.Value);
            }

            if (izinIslendiMi.HasValue)
            {
                query = query.Where(t => t.IzinIslendiMi == izinIslendiMi.Value);
            }

            // Toplam kayıt sayısı
            var totalCount = await query.CountAsync();

            // Sıralama
            query = ApplySorting(query, sortBy, sortDescending);

            // Sayfalama
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<IzinMazeretTalep>> GetByDateRangeAsync(
            DateTime startDate,
            DateTime endDate,
            int? departmanId = null,
            int? servisId = null)
        {
            var query = _dbSet
                .AsNoTracking()
                .Include(t => t.Personel)
                    .ThenInclude(p => p!.Departman)
                .Include(t => t.Personel)
                    .ThenInclude(p => p!.Servis)
                .Where(t => !t.SilindiMi &&
                           t.IsActive &&
                           ((t.BaslangicTarihi >= startDate && t.BaslangicTarihi <= endDate) ||
                            (t.BitisTarihi >= startDate && t.BitisTarihi <= endDate) ||
                            (t.MazeretTarihi >= startDate && t.MazeretTarihi <= endDate)));

            if (departmanId.HasValue)
            {
                query = query.Where(t => t.Personel!.DepartmanId == departmanId.Value);
            }

            if (servisId.HasValue)
            {
                query = query.Where(t => t.Personel!.ServisId == servisId.Value);
            }

            return await query
                .OrderBy(t => t.BaslangicTarihi ?? t.MazeretTarihi)
                .ToListAsync();
        }

        // ═══════════════════════════════════════════════════════
        // İSTATİSTİKLER
        // ═══════════════════════════════════════════════════════

        public async Task<int> GetTotalYillikIzinDaysAsync(string tcKimlikNo, int year)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return 0;

            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year, 12, 31);

            var totalDays = await _dbSet
                .AsNoTracking()
                .Where(t => t.TcKimlikNo == tcKimlikNo &&
                           !t.SilindiMi &&
                           t.IsActive &&
                           t.BirinciOnayDurumu == OnayDurumu.Onaylandi &&
                           t.IkinciOnayDurumu == OnayDurumu.Onaylandi &&
                           t.BaslangicTarihi >= startDate &&
                           t.BitisTarihi <= endDate)
                .SumAsync(t => t.ToplamGun ?? 0);

            return totalDays;
        }

        public async Task<int> GetTotalUsedDaysAsync(string tcKimlikNo, int? izinMazeretTuruId = null, int? year = null)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return 0;

            var query = _dbSet
                .AsNoTracking()
                .Where(t => t.TcKimlikNo == tcKimlikNo &&
                           !t.SilindiMi &&
                           t.IsActive &&
                           t.BirinciOnayDurumu == OnayDurumu.Onaylandi &&
                           t.IkinciOnayDurumu == OnayDurumu.Onaylandi);

            if (izinMazeretTuruId.HasValue)
            {
                query = query.Where(t => t.IzinMazeretTuruId == izinMazeretTuruId.Value);
            }

            if (year.HasValue)
            {
                var startDate = new DateTime(year.Value, 1, 1);
                var endDate = new DateTime(year.Value, 12, 31);
                query = query.Where(t => t.BaslangicTarihi >= startDate && t.BitisTarihi <= endDate);
            }

            return await query.SumAsync(t => t.ToplamGun ?? 0);
        }

        public async Task<Dictionary<int, int>> GetDepartmanStatisticsAsync(int departmanId, int year)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year, 12, 31);

            return await _dbSet
                .AsNoTracking()
                .Where(t => t.Personel!.DepartmanId == departmanId &&
                           !t.SilindiMi &&
                           t.IsActive &&
                           t.BirinciOnayDurumu == OnayDurumu.Onaylandi &&
                           t.IkinciOnayDurumu == OnayDurumu.Onaylandi &&
                           t.BaslangicTarihi >= startDate &&
                           t.BitisTarihi <= endDate)
                .GroupBy(t => t.IzinMazeretTuruId)
                .Select(g => new { TuruId = g.Key, TotalDays = g.Sum(t => t.ToplamGun ?? 0) })
                .ToDictionaryAsync(x => x.TuruId, x => x.TotalDays);
        }

        // ═══════════════════════════════════════════════════════
        // YARDIMCI METODLAR
        // ═══════════════════════════════════════════════════════

        private IQueryable<IzinMazeretTalep> ApplySorting(
            IQueryable<IzinMazeretTalep> query,
            string? sortBy,
            bool sortDescending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return query.OrderByDescending(t => t.TalepTarihi);
            }

            return sortBy.ToLower() switch
            {
                "tarih" => sortDescending
                    ? query.OrderByDescending(t => t.BaslangicTarihi ?? t.MazeretTarihi)
                    : query.OrderBy(t => t.BaslangicTarihi ?? t.MazeretTarihi),
                "adsoyad" => sortDescending
                    ? query.OrderByDescending(t => t.Personel!.AdSoyad)
                    : query.OrderBy(t => t.Personel!.AdSoyad),
                "tur" => sortDescending
                    ? query.OrderByDescending(t => t.IzinMazeretTuruId)
                    : query.OrderBy(t => t.IzinMazeretTuruId),
                "taleptarihi" => sortDescending
                    ? query.OrderByDescending(t => t.TalepTarihi)
                    : query.OrderBy(t => t.TalepTarihi),
                _ => query.OrderByDescending(t => t.TalepTarihi)
            };
        }
    }
}
