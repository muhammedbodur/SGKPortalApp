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

        // Kullanıcının aktif bağlantılarını listeler (BankoMode için)
        // ⭐ User, HubBankoConnection ve Banko tablolarıyla entegre
        // SQL: PER_Personeller → SIR_BankoKullanicilari → SIR_Bankolar → CMN_Users → CMN_HubConnections → SIR_HubBankoConnections
        public async Task<IEnumerable<HubConnection>> GetActiveConnectionsByTcKimlikNoAsync(string tcKimlikNo)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(hc => hc.User)
                .Include(hc => hc.HubBankoConnection)
                    .ThenInclude(hbc => hbc.Banko)
                .Where(hc =>
                    // HubConnection filtreleri
                    hc.TcKimlikNo == tcKimlikNo &&
                    hc.ConnectionStatus == ConnectionStatus.online &&
                    hc.ConnectionType == "BankoMode" &&
                    !hc.SilindiMi &&

                    // User filtreleri
                    hc.User != null &&
                    hc.User.UserType == BusinessObjectLayer.Enums.Common.UserType.Personel &&
                    hc.User.BankoModuAktif &&
                    hc.User.AktifBankoId.HasValue &&
                    hc.User.AktifBankoId.Value > 0 &&

                    // HubBankoConnection filtreleri
                    hc.HubBankoConnection != null &&
                    hc.HubBankoConnection.BankoModuAktif &&
                    !hc.HubBankoConnection.SilindiMi &&

                    // Banko filtreleri
                    hc.HubBankoConnection.Banko != null &&
                    !hc.HubBankoConnection.Banko.SilindiMi
                )
                .ToListAsync();
        }

        public async Task<IEnumerable<HubConnection>> GetStaleConnectionsAsync(System.DateTime timeoutThreshold)
        {
            return await _dbSet
                .Where(h => h.ConnectionStatus == ConnectionStatus.online && h.ConnectedAt < timeoutThreshold)
                .ToListAsync();
        }

        public async Task<int> DeactivateStaleConnectionsAsync(System.DateTime timeoutThreshold)
        {
            var staleConnections = await _dbSet
                .Where(h => h.ConnectionStatus == ConnectionStatus.online && h.ConnectedAt < timeoutThreshold)
                .ToListAsync();

            foreach (var connection in staleConnections)
            {
                connection.ConnectionStatus = ConnectionStatus.offline;
                connection.SilindiMi = true;
            }

            return staleConnections.Count;
        }

        public async Task<IEnumerable<string>> GetActiveSessionIdsAsync()
        {
            return await _dbSet
                .Where(h => h.ConnectionStatus == ConnectionStatus.online)
                .Join(_context.Users,
                    h => h.TcKimlikNo,
                    u => u.TcKimlikNo,
                    (h, u) => u.SessionID)
                .Where(sessionId => !string.IsNullOrEmpty(sessionId))
                .Distinct()
                .ToListAsync();
        }
    }
}
