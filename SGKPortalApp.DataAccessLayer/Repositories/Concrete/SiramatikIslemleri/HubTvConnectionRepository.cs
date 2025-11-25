using Microsoft.EntityFrameworkCore;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.SiramatikIslemleri
{
    public class HubTvConnectionRepository : GenericRepository<HubTvConnection>, IHubTvConnectionRepository
    {
        public HubTvConnectionRepository(SGKDbContext context) : base(context) { }

        // TV'ye ait bağlantıları listeler
        public async Task<IEnumerable<HubTvConnection>> GetByTvAsync(int tvId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(htc => htc.Tv)
                .Include(htc => htc.HubConnection)
                .Where(htc => htc.TvId == tvId)
                .ToListAsync();
        }

        // Bağlantı ID'sine göre TV bağlantısını getirir
        public async Task<HubTvConnection?> GetByConnectionIdAsync(string connectionId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(htc => htc.Tv)
                .Include(htc => htc.HubConnection)
                .FirstOrDefaultAsync(htc => htc.HubConnection != null && htc.HubConnection.ConnectionId == connectionId);
        }

        // Aktif TV bağlantılarını listeler
        public async Task<IEnumerable<HubTvConnection>> GetActiveConnectionsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(htc => htc.Tv)
                .Include(htc => htc.HubConnection)
                .Where(htc => htc.HubConnection != null && htc.HubConnection.ConnectionStatus == ConnectionStatus.online)
                .ToListAsync();
        }

        // TV bağlantısını detaylı getirir
        public async Task<HubTvConnection?> GetWithDetailsAsync(int hubTvConnectionId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(htc => htc.Tv)
                .Include(htc => htc.HubConnection)
                .FirstOrDefaultAsync(htc => htc.HubTvConnectionId == hubTvConnectionId);
        }

        // Tüm TV bağlantılarını detaylı listeler
        public async Task<IEnumerable<HubTvConnection>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(htc => htc.Tv)
                .Include(htc => htc.HubConnection)
                .ToListAsync();
        }
    }
}