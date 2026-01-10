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
    }
}
