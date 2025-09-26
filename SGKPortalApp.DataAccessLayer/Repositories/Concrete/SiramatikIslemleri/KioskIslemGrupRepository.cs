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
    public class KioskIslemGrupRepository : GenericRepository<KioskIslemGrup>, IKioskIslemGrupRepository
    {
        public KioskIslemGrupRepository(SGKDbContext context) : base(context) { }

        // Kiosk grubu bazında işlem gruplarını listeler
        public async Task<IEnumerable<KioskIslemGrup>> GetByKioskGrupAsync(int kioskGrupId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kig => kig.KioskGrup)
                .Where(kig => kig.KioskGrupId == kioskGrupId)
                .ToListAsync();
        }

        // Hizmet binası bazında işlem gruplarını listeler
        public async Task<IEnumerable<KioskIslemGrup>> GetByHizmetBinasiAsync(int hizmetBinasiId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kig => kig.HizmetBinasi)
                .Where(kig => kig.HizmetBinasiId == hizmetBinasiId)
                .ToListAsync();
        }

        // Tarih aralığına göre işlem gruplarını listeler
        public async Task<IEnumerable<KioskIslemGrup>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(kig => kig.EklenmeTarihi >= startDate && kig.EklenmeTarihi <= endDate)
                .ToListAsync();
        }

        // İşlem grubunu detaylı getirir
        public async Task<KioskIslemGrup?> GetWithDetailsAsync(int kioskIslemGrupId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kig => kig.KioskGrup)
                .Include(kig => kig.HizmetBinasi)
                .Include(kig => kig.KanalAlt)
                .FirstOrDefaultAsync(kig => kig.KioskIslemGrupId == kioskIslemGrupId);
        }

        // Tüm işlem gruplarını detaylı listeler
        public async Task<IEnumerable<KioskIslemGrup>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kig => kig.KioskGrup)
                .Include(kig => kig.HizmetBinasi)
                .Include(kig => kig.KanalAlt)
                .ToListAsync();
        }

        // Aktif işlem gruplarını listeler
        public async Task<IEnumerable<KioskIslemGrup>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(kig => kig.KioskIslemGrupAktiflik == Aktiflik.Aktif)
                .ToListAsync();
        }
    }
}