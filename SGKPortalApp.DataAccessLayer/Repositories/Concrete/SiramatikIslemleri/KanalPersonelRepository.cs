using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.SiramatikIslemleri
{
    public class KanalPersonelRepository : GenericRepository<KanalPersonel>, IKanalPersonelRepository
    {
        public KanalPersonelRepository(SGKDbContext context) : base(context) { }

        // Personel bazında atamaları listeler
        public async Task<IEnumerable<KanalPersonel>> GetByPersonelAsync(string tcKimlikNo)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kp => kp.Personel)
                .Include(kp => kp.KanalAltIslem)
                .Where(kp => kp.TcKimlikNo == tcKimlikNo)
                .ToListAsync();
        }

        // Kanal alt işlem bazında personelleri listeler
        public async Task<IEnumerable<KanalPersonel>> GetByKanalAltIslemAsync(int kanalAltIslemId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kp => kp.Personel)
                .Include(kp => kp.KanalAltIslem)
                .Where(kp => kp.KanalAltIslemId == kanalAltIslemId)
                .ToListAsync();
        }

        // Departman-Hizmet binası bazında atamalari listeler
        public async Task<IEnumerable<KanalPersonel>> GetByDepartmanHizmetBinasiIdAsync(int departmanHizmetBinasiId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kp => kp.Personel)
                .Include(kp => kp.KanalAltIslem)
                    .ThenInclude(kai => kai.DepartmanHizmetBinasi)
                .Where(kp => kp.KanalAltIslem != null && kp.KanalAltIslem.DepartmanHizmetBinasiId == departmanHizmetBinasiId)
                .ToListAsync();
        }

        // Atamayı detaylı getirir
        public async Task<KanalPersonel?> GetWithDetailsAsync(int kanalPersonelId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kp => kp.Personel)
                .Include(kp => kp.KanalAltIslem)
                .FirstOrDefaultAsync(kp => kp.KanalPersonelId == kanalPersonelId);
        }

        // Tüm atamaları detaylı listeler
        public async Task<IEnumerable<KanalPersonel>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kp => kp.Personel)
                .Include(kp => kp.KanalAltIslem)
                .ToListAsync();
        }

        // Aktif atamaları listeler
        public async Task<IEnumerable<KanalPersonel>> GetActiveAssignmentsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kp => kp.Personel)
                .Include(kp => kp.KanalAltIslem)
                .Where(kp => kp.Aktiflik == Aktiflik.Aktif)
                .ToListAsync();
        }

        // Tüm atamaları listeler
        public async Task<IEnumerable<KanalPersonel>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .ToListAsync();
        }

        // Çakışma kontrolü yapar
        public async Task<bool> HasConflictAsync(string tcKimlikNo, int kanalAltIslemId)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(kp => kp.TcKimlikNo == tcKimlikNo && kp.KanalAltIslemId == kanalAltIslemId && kp.Aktiflik == Aktiflik.Aktif);
        }

        // Pasif veya silinmiş kayıt var mı kontrol eder
        public async Task<KanalPersonel?> GetInactiveRecordAsync(string tcKimlikNo, int kanalAltIslemId)
        {
            return await _dbSet
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(kp => kp.TcKimlikNo == tcKimlikNo 
                                        && kp.KanalAltIslemId == kanalAltIslemId
                                        && kp.Aktiflik == Aktiflik.Pasif);
        }

        // Uzmanlık bazında personelleri listeler
        public async Task<IEnumerable<KanalPersonel>> GetByUzmanlikAsync(PersonelUzmanlik uzmanlik)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kp => kp.Personel)
                .Include(kp => kp.KanalAltIslem)
                .Where(kp => kp.Uzmanlik == uzmanlik)
                .ToListAsync();
        }
    }
}