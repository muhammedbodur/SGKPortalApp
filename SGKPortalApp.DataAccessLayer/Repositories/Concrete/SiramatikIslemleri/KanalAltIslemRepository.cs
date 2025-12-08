using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.SiramatikIslemleri
{
    public class KanalAltIslemRepository : GenericRepository<KanalAltIslem>, IKanalAltIslemRepository
    {
        public KanalAltIslemRepository(SGKDbContext context) : base(context) { }

        // Alt kanal bazında işlemleri listeler
        public async Task<IEnumerable<KanalAltIslem>> GetByKanalAltAsync(int kanalAltId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kai => kai.KanalAlt)
                .Where(kai => kai.KanalAltId == kanalAltId)
                .ToListAsync();
        }

        // Hizmet binası bazında işlemleri listeler
        public async Task<IEnumerable<KanalAltIslem>> GetByHizmetBinasiAsync(int hizmetBinasiId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kai => kai.HizmetBinasi)
                .Where(kai => kai.HizmetBinasiId == hizmetBinasiId)
                .ToListAsync();
        }

        // Kanal işlemi bazında işlemleri listeler
        public async Task<IEnumerable<KanalAltIslem>> GetByKanalIslemAsync(int kanalIslemId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kai => kai.KanalIslem)
                .Where(kai => kai.KanalIslemId == kanalIslemId)
                .ToListAsync();
        }

        // Tarih aralığına göre işlemleri listeler
        public async Task<IEnumerable<KanalAltIslem>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(kai => kai.EklenmeTarihi >= startDate && kai.EklenmeTarihi <= endDate)
                .ToListAsync();
        }

        // İşlemi detaylı getirir
        public async Task<KanalAltIslem?> GetWithDetailsAsync(int kanalAltIslemId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kai => kai.KanalAlt)
                .Include(kai => kai.HizmetBinasi)
                .Include(kai => kai.KanalIslem)
                .Include(kai => kai.Siralar)
                .Include(kai => kai.KanalPersonelleri)
                .FirstOrDefaultAsync(kai => kai.KanalAltIslemId == kanalAltIslemId);
        }

        // Tüm işlemleri detaylı listeler
        public async Task<IEnumerable<KanalAltIslem>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kai => kai.KanalAlt)
                .Include(kai => kai.HizmetBinasi)
                .Include(kai => kai.KanalIslem)
                .Include(kai => kai.Siralar)
                .Include(kai => kai.KanalPersonelleri)
                .ToListAsync();
        }

        // Aktif işlemleri listeler
        public async Task<IEnumerable<KanalAltIslem>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(kai => kai.Aktiflik == Aktiflik.Aktif)
                .ToListAsync();
        }

        // ID ile işlemi getirir (tracking olmadan)
        public async Task<KanalAltIslem?> GetByIdNoTrackingAsync(int kanalAltIslemId)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(kai => kai.KanalAltIslemId == kanalAltIslemId);
        }
    }
}