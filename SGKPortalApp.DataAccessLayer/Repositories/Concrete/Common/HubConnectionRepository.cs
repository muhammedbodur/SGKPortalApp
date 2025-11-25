using Microsoft.EntityFrameworkCore;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.Common
{
    public class HubConnectionRepository : GenericRepository<HubConnection>, IHubConnectionRepository
    {
        public HubConnectionRepository(SGKDbContext context) : base(context) { }

        // Kullanıcıya ait bağlantıları listeler
        public async Task<IEnumerable<HubConnection>> GetByUserAsync(string tcKimlikNo)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(hc => hc.User)
                    .ThenInclude(u => u.Personel)
                .Where(hc => hc.TcKimlikNo == tcKimlikNo)
                .ToListAsync();
        }

        // Bağlantı ID'sine göre bağlantıyı getirir
        public async Task<HubConnection?> GetByConnectionIdAsync(string connectionId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(hc => hc.User)
                    .ThenInclude(u => u.Personel)
                .FirstOrDefaultAsync(hc => hc.ConnectionId == connectionId);
        }

        // Aktif bağlantıları listeler
        public async Task<IEnumerable<HubConnection>> GetActiveConnectionsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(hc => hc.User)
                    .ThenInclude(u => u.Personel)
                .Where(hc => hc.ConnectionStatus == ConnectionStatus.online)
                .ToListAsync();
        }

        // Bağlantıyı detaylı getirir
        public async Task<HubConnection?> GetWithDetailsAsync(int hubConnectionId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(hc => hc.User)
                    .ThenInclude(u => u.Personel)
                .FirstOrDefaultAsync(hc => hc.HubConnectionId == hubConnectionId);
        }

        // Tüm bağlantıları detaylı listeler
        public async Task<IEnumerable<HubConnection>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(hc => hc.User)
                    .ThenInclude(u => u.Personel)
                .ToListAsync();
        }
    }
}
