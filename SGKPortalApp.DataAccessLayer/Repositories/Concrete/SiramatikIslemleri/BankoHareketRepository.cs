using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.SiramatikIslemleri
{
    public class BankoHareketRepository : GenericRepository<BankoHareket>, IBankoHareketRepository
    {
        public BankoHareketRepository(SGKDbContext context) : base(context) { }

        // Bankoya ait hareketleri getirir
        public async Task<IEnumerable<BankoHareket>> GetByBankoAsync(int bankoId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(bh => bh.Banko)
                .Include(bh => bh.Personel)
                .Include(bh => bh.Sira)
                .Where(bh => bh.BankoId == bankoId)
                .OrderByDescending(bh => bh.IslemBaslamaZamani)
                .ToListAsync();
        }

        // Personele ait hareketleri getirir
        public async Task<IEnumerable<BankoHareket>> GetByPersonelAsync(string tcKimlikNo)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(bh => bh.Banko)
                .Include(bh => bh.Personel)
                .Include(bh => bh.Sira)
                .Where(bh => bh.PersonelTcKimlikNo == tcKimlikNo)
                .OrderByDescending(bh => bh.IslemBaslamaZamani)
                .ToListAsync();
        }

        // Sıraya ait hareketleri getirir (read-only, detaylı)
        public async Task<IEnumerable<BankoHareket>> GetBySiraAsync(int siraId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(bh => bh.Banko)
                .Include(bh => bh.Personel)
                .Include(bh => bh.Sira)
                .Where(bh => bh.SiraId == siraId)
                .OrderBy(bh => bh.IslemBaslamaZamani)
                .ToListAsync();
        }

        // Sıraya ait hareketleri getirir (güncelleme için, navigation property'siz)
        public async Task<IEnumerable<BankoHareket>> GetBySiraForUpdateAsync(int siraId)
        {
            return await _dbSet
                .Where(bh => bh.SiraId == siraId)
                .OrderBy(bh => bh.IslemBaslamaZamani)
                .ToListAsync();
        }

        // Tarih aralığına göre hareketleri getirir
        public async Task<IEnumerable<BankoHareket>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(bh => bh.IslemBaslamaZamani >= startDate && bh.IslemBaslamaZamani <= endDate)
                .OrderByDescending(bh => bh.IslemBaslamaZamani)
                .ToListAsync();
        }

        // Banko ve tarih aralığına göre hareketleri getirir
        public async Task<IEnumerable<BankoHareket>> GetByBankoAndDateRangeAsync(int bankoId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(bh => bh.Personel)
                .Include(bh => bh.Sira)
                .Where(bh => bh.BankoId == bankoId 
                          && bh.IslemBaslamaZamani >= startDate 
                          && bh.IslemBaslamaZamani <= endDate)
                .OrderByDescending(bh => bh.IslemBaslamaZamani)
                .ToListAsync();
        }

        // Personel ve tarih aralığına göre hareketleri getirir
        public async Task<IEnumerable<BankoHareket>> GetByPersonelAndDateRangeAsync(string tcKimlikNo, DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(bh => bh.Banko)
                .Include(bh => bh.Sira)
                .Where(bh => bh.PersonelTcKimlikNo == tcKimlikNo 
                          && bh.IslemBaslamaZamani >= startDate 
                          && bh.IslemBaslamaZamani <= endDate)
                .OrderByDescending(bh => bh.IslemBaslamaZamani)
                .ToListAsync();
        }

        // Detaylı hareket getirir (tüm navigation property'ler ile)
        public async Task<BankoHareket?> GetWithDetailsAsync(long bankoHareketId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(bh => bh.Banko)
                    .ThenInclude(b => b.HizmetBinasi)
                .Include(bh => bh.Personel)
                    .ThenInclude(p => p.Servis)
                .Include(bh => bh.Sira)
                .Include(bh => bh.KanalIslem)
                    .ThenInclude(ki => ki.Kanal)
                .Include(bh => bh.KanalAltIslem)
                    .ThenInclude(kai => kai.KanalAlt)
                .FirstOrDefaultAsync(bh => bh.BankoHareketId == bankoHareketId);
        }

        // Tüm hareketleri detaylı listeler
        public async Task<IEnumerable<BankoHareket>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(bh => bh.Banko)
                .Include(bh => bh.Personel)
                .Include(bh => bh.Sira)
                .Include(bh => bh.KanalIslem)
                .Include(bh => bh.KanalAltIslem)
                .OrderByDescending(bh => bh.IslemBaslamaZamani)
                .ToListAsync();
        }

        // Tamamlanmamış hareketleri getirir (IslemBitisZamani NULL)
        public async Task<IEnumerable<BankoHareket>> GetIncompleteAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(bh => bh.Banko)
                .Include(bh => bh.Personel)
                .Include(bh => bh.Sira)
                .Where(bh => bh.IslemBitisZamani == null)
                .OrderBy(bh => bh.IslemBaslamaZamani)
                .ToListAsync();
        }

        // Banko istatistiklerini getirir
        public async Task<(int ToplamIslem, int OrtalamaSure)> GetBankoStatsAsync(int bankoId, DateTime startDate, DateTime endDate)
        {
            var hareketler = await _dbSet
                .AsNoTracking()
                .Where(bh => bh.BankoId == bankoId 
                          && bh.IslemBaslamaZamani >= startDate 
                          && bh.IslemBaslamaZamani <= endDate
                          && bh.IslemBitisZamani != null
                          && bh.IslemSuresiSaniye != null)
                .ToListAsync();

            var toplamIslem = hareketler.Count;
            var ortalamaSure = toplamIslem > 0 
                ? (int)hareketler.Average(bh => bh.IslemSuresiSaniye ?? 0) 
                : 0;

            return (toplamIslem, ortalamaSure);
        }

        // Personel istatistiklerini getirir
        public async Task<(int ToplamIslem, int OrtalamaSure)> GetPersonelStatsAsync(string tcKimlikNo, DateTime startDate, DateTime endDate)
        {
            var hareketler = await _dbSet
                .AsNoTracking()
                .Where(bh => bh.PersonelTcKimlikNo == tcKimlikNo 
                          && bh.IslemBaslamaZamani >= startDate 
                          && bh.IslemBaslamaZamani <= endDate
                          && bh.IslemBitisZamani != null
                          && bh.IslemSuresiSaniye != null)
                .ToListAsync();

            var toplamIslem = hareketler.Count;
            var ortalamaSure = toplamIslem > 0 
                ? (int)hareketler.Average(bh => bh.IslemSuresiSaniye ?? 0) 
                : 0;

            return (toplamIslem, ortalamaSure);
        }

        // Bankodaki bugünkü son çağrılan sırayı getirir (TV için)
        public async Task<BankoHareket?> GetSonCagrilanByBankoAsync(int bankoId)
        {
            var today = DateTime.Today;
            return await _dbSet
                .AsNoTracking()
                .Where(bh => bh.BankoId == bankoId 
                          && bh.IslemBaslamaZamani.Date == today
                          && !bh.SilindiMi)
                .OrderByDescending(bh => bh.IslemBaslamaZamani)
                .FirstOrDefaultAsync();
        }

        // Belirtilen bankolardaki bugünkü son çağrılan sıraları getirir (TV için)
        // Tamamlanmış olsa bile gösterilir - son çağrılma zamanına göre sıralı
        public async Task<IEnumerable<BankoHareket>> GetAktifSiralarByBankoIdsAsync(IEnumerable<int> bankoIds)
        {
            var today = DateTime.Today;
            var bankoIdList = bankoIds.ToList();
            
            return await _dbSet
                .AsNoTracking()
                .Include(bh => bh.Banko)
                .Where(bh => bankoIdList.Contains(bh.BankoId) 
                          && bh.IslemBaslamaZamani.Date == today
                          && !bh.SilindiMi)
                .OrderByDescending(bh => bh.IslemBaslamaZamani)
                .ToListAsync();
        }
    }
}
