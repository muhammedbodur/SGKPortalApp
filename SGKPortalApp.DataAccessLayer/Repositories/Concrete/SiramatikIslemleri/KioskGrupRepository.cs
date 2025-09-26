using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.SiramatikIslemleri
{
    public class KioskGrupRepository : GenericRepository<KioskGrup>, IKioskGrupRepository
    {
        public KioskGrupRepository(SGKDbContext context) : base(context) { }

        // Hizmet binası bazında kiosk gruplarını listeler
        public async Task<IEnumerable<KioskGrup>> GetByHizmetBinasiAsync(int hizmetBinasiId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kg => kg.KioskIslemGruplari)
                .Where(kg => kg.KioskIslemGruplari!.Any(kig => kig.HizmetBinasiId == hizmetBinasiId))
                .ToListAsync();
        }

        // Kiosk grup adıyla grubu getirir
        public async Task<KioskGrup?> GetByAdAsync(string kioskGrupAdi)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(kg => kg.KioskGrupAdi == kioskGrupAdi);
        }

        // Kiosk grubunu detaylı getirir
        public async Task<KioskGrup?> GetWithDetailsAsync(int kioskGrupId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kg => kg.KioskIslemGruplari)
                .FirstOrDefaultAsync(kg => kg.KioskGrupId == kioskGrupId);
        }

        // Tüm kiosk gruplarını detaylı listeler
        public async Task<IEnumerable<KioskGrup>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kg => kg.KioskIslemGruplari)
                .ToListAsync();
        }

        // Aktif kiosk gruplarını listeler
        public async Task<IEnumerable<KioskGrup>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(kg => kg.Aktiflik == Aktiflik.Aktif)
                .ToListAsync();
        }

        // Dropdown için kiosk gruplarını listeler
        public async Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Select(kg => new ValueTuple<int, string>(kg.KioskGrupId, kg.KioskGrupAdi))
                .ToListAsync();
        }
    }
}