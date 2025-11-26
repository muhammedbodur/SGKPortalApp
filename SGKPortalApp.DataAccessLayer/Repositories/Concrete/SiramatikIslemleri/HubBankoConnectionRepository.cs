using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.SiramatikIslemleri
{
    /// <summary>
    /// HubBankoConnection repository implementation
    /// </summary>
    public class HubBankoConnectionRepository : GenericRepository<HubBankoConnection>, IHubBankoConnectionRepository
    {
        public HubBankoConnectionRepository(SGKDbContext context) : base(context) { }

        public async Task<HubBankoConnection?> GetByHubConnectionIdAsync(int hubConnectionId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.HubConnection)
                .Include(x => x.Banko)
                .FirstOrDefaultAsync(x => x.HubConnectionId == hubConnectionId && x.BankoModuAktif);
        }

        public async Task<HubBankoConnection?> GetActiveByTcKimlikNoAsync(string tcKimlikNo)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.HubConnection)
                .Include(x => x.Banko)
                .FirstOrDefaultAsync(x => x.TcKimlikNo == tcKimlikNo && x.BankoModuAktif);
        }

        public async Task<HubBankoConnection?> GetActiveByBankoIdAsync(int bankoId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.HubConnection)
                .FirstOrDefaultAsync(x => x.BankoId == bankoId && x.BankoModuAktif);
        }

        public async Task<bool> DeactivateAsync(int hubBankoConnectionId)
        {
            var connection = await _dbSet.FirstOrDefaultAsync(x => x.HubBankoConnectionId == hubBankoConnectionId);
            if (connection == null) return false;

            connection.BankoModuAktif = false;
            connection.BankoModuBitis = DateTime.Now;
            connection.DuzenlenmeTarihi = DateTime.Now;
            
            _dbSet.Update(connection);
            await _context.SaveChangesAsync();
            
            return true;
        }
    }
}
