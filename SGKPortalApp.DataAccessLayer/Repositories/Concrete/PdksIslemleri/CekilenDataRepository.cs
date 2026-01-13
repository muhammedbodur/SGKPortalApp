using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PdksIslemleri;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.PdksIslemleri
{
    public class CekilenDataRepository : GenericRepository<CekilenData>, ICekilenDataRepository
    {
        public CekilenDataRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<List<CekilenData>> GetByEnrollNumberAsync(string enrollNumber)
        {
            return await _context.CekilenDatalar
                .Where(c => c.KayitNo == enrollNumber)
                .OrderByDescending(c => c.Tarih)
                .ToListAsync();
        }

        public async Task<List<CekilenData>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.CekilenDatalar
                .Where(c => c.Tarih >= startDate && c.Tarih <= endDate)
                .OrderByDescending(c => c.Tarih)
                .ToListAsync();
        }

        public async Task<List<CekilenData>> GetByDeviceIpAsync(string deviceIp)
        {
            return await _context.CekilenDatalar
                .Where(c => c.CihazIp == deviceIp)
                .OrderByDescending(c => c.Tarih)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(string enrollNumber, DateTime date)
        {
            return await _context.CekilenDatalar
                .AnyAsync(c => c.KayitNo == enrollNumber && c.Tarih == date);
        }

        public async Task<bool> ExistsByKayitNoAndTarihAsync(string kayitNo, DateTime tarih)
        {
            return await _context.CekilenDatalar
                .AnyAsync(c => c.KayitNo == kayitNo && c.Tarih == tarih);
        }

        public async Task<int> BulkInsertAsync(List<CekilenData> records)
        {
            if (records == null || !records.Any())
                return 0;

            await _context.CekilenDatalar.AddRangeAsync(records);
            return await _context.SaveChangesAsync();
        }
    }
}
