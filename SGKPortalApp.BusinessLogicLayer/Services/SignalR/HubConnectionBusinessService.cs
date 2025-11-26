using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SignalR;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;

namespace SGKPortalApp.BusinessLogicLayer.Services.SignalR
{
    /// <summary>
    /// Hub Connection Business Service
    /// Business Layer'da SignalR bağlantı yönetimi
    /// </summary>
    public class HubConnectionBusinessService : IHubConnectionBusinessService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HubConnectionBusinessService> _logger;

        public HubConnectionBusinessService(IUnitOfWork unitOfWork, ILogger<HubConnectionBusinessService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> CreateOrUpdateConnectionAsync(string connectionId, string tcKimlikNo)
        {
            try
            {
                var repo = _unitOfWork.Repository<HubConnection>();
                
                var connection = await repo.FirstOrDefaultAsync(c => c.TcKimlikNo == tcKimlikNo);

                if (connection != null)
                {
                    connection.ConnectionId = connectionId;
                    connection.ConnectionStatus = BusinessObjectLayer.Enums.SiramatikIslemleri.ConnectionStatus.online;
                    connection.IslemZamani = DateTime.Now;
                    connection.DuzenlenmeTarihi = DateTime.Now;
                    repo.Update(connection);
                }
                else
                {
                    connection = new HubConnection
                    {
                        TcKimlikNo = tcKimlikNo,
                        ConnectionId = connectionId,
                        ConnectionStatus = BusinessObjectLayer.Enums.SiramatikIslemleri.ConnectionStatus.online,
                        IslemZamani = DateTime.Now,
                        EklenmeTarihi = DateTime.Now,
                        DuzenlenmeTarihi = DateTime.Now
                    };
                    await repo.AddAsync(connection);
                }

                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateOrUpdateConnectionAsync hatası");
                return false;
            }
        }

        public async Task<bool> DisconnectAsync(string connectionId)
        {
            try
            {
                var repo = _unitOfWork.Repository<HubConnection>();
                var connection = await repo.FirstOrDefaultAsync(c => c.ConnectionId == connectionId);

                if (connection != null)
                {
                    repo.Delete(connection);
                    await _unitOfWork.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DisconnectAsync hatası");
                return false;
            }
        }

        public async Task<IEnumerable<HubConnection>> GetActiveConnectionsByTcKimlikNoAsync(string tcKimlikNo)
        {
            try
            {
                var repo = _unitOfWork.Repository<HubConnection>();
                var connections = await repo.FindAsync(c => c.TcKimlikNo == tcKimlikNo && 
                    c.ConnectionStatus == BusinessObjectLayer.Enums.SiramatikIslemleri.ConnectionStatus.online);
                return connections;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveConnectionsByTcKimlikNoAsync hatası");
                return Enumerable.Empty<HubConnection>();
            }
        }

        public async Task<bool> RegisterBankoConnectionAsync(int bankoId, string connectionId, string tcKimlikNo)
        {
            try
            {
                var repo = _unitOfWork.Repository<HubBankoConnection>();
                
                var bankoConnection = new HubBankoConnection
                {
                    BankoId = bankoId,
                    TcKimlikNo = tcKimlikNo,
                    BankoModuAktif = true,
                    BankoModuBaslangic = DateTime.Now,
                    IslemZamani = DateTime.Now,
                    EklenmeTarihi = DateTime.Now,
                    DuzenlenmeTarihi = DateTime.Now
                };

                await repo.AddAsync(bankoConnection);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RegisterBankoConnectionAsync hatası");
                return false;
            }
        }

        public async Task<bool> DeactivateBankoConnectionAsync(string tcKimlikNo)
        {
            try
            {
                var repo = _unitOfWork.Repository<HubBankoConnection>();
                var bankoConnection = await repo.FirstOrDefaultAsync(c => c.TcKimlikNo == tcKimlikNo);

                if (bankoConnection != null)
                {
                    repo.Delete(bankoConnection);
                    await _unitOfWork.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeactivateBankoConnectionAsync hatası");
                return false;
            }
        }

        public async Task<bool> RegisterTvConnectionAsync(int tvId, string connectionId, string tcKimlikNo)
        {
            try
            {
                var repo = _unitOfWork.Repository<HubTvConnection>();
                
                var tvConnection = new HubTvConnection
                {
                    TvId = tvId,
                    IslemZamani = DateTime.Now,
                    EklenmeTarihi = DateTime.Now,
                    DuzenlenmeTarihi = DateTime.Now
                };

                await repo.AddAsync(tvConnection);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RegisterTvConnectionAsync hatası");
                return false;
            }
        }

        public async Task<bool> IsBankoInUseAsync(int bankoId)
        {
            try
            {
                var repo = _unitOfWork.Repository<HubBankoConnection>();
                var connection = await repo.FirstOrDefaultAsync(c => c.BankoId == bankoId && c.BankoModuAktif);
                return connection != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IsBankoInUseAsync hatası");
                return false;
            }
        }

        public async Task<HubBankoConnection?> GetPersonelActiveBankoAsync(string tcKimlikNo)
        {
            try
            {
                var repo = _unitOfWork.Repository<HubBankoConnection>();
                return await repo.FirstOrDefaultAsync(c => c.TcKimlikNo == tcKimlikNo && c.BankoModuAktif);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPersonelActiveBankoAsync hatası");
                return null;
            }
        }

        public async Task<bool> UpdateConnectionTypeAsync(string connectionId, string connectionType)
        {
            try
            {
                var repo = _unitOfWork.Repository<HubConnection>();
                var connection = await repo.FirstOrDefaultAsync(c => c.ConnectionId == connectionId);

                if (connection != null)
                {
                    connection.ConnectionType = connectionType;
                    connection.DuzenlenmeTarihi = DateTime.Now;
                    repo.Update(connection);
                    await _unitOfWork.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateConnectionTypeAsync hatası");
                return false;
            }
        }

        public async Task<bool> SetConnectionStatusAsync(string connectionId, string status)
        {
            try
            {
                var repo = _unitOfWork.Repository<HubConnection>();
                var connection = await repo.FirstOrDefaultAsync(c => c.ConnectionId == connectionId);

                if (connection != null)
                {
                    if (Enum.TryParse<BusinessObjectLayer.Enums.SiramatikIslemleri.ConnectionStatus>(status, out var connectionStatus))
                    {
                        connection.ConnectionStatus = connectionStatus;
                        connection.DuzenlenmeTarihi = DateTime.Now;
                        repo.Update(connection);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetConnectionStatusAsync hatası");
                return false;
            }
        }

        public async Task<bool> IsTvInUseByTvUserAsync(int tvId)
        {
            try
            {
                var repo = _unitOfWork.Repository<HubTvConnection>();
                var connection = await repo.FirstOrDefaultAsync(c => c.TvId == tvId);
                return connection != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IsTvInUseByTvUserAsync hatası");
                return false;
            }
        }

        public async Task<bool> CreateBankoConnectionAsync(int hubConnectionId, int bankoId, string tcKimlikNo)
        {
            try
            {
                var repo = _unitOfWork.Repository<HubBankoConnection>();
                
                var bankoConnection = new HubBankoConnection
                {
                    HubConnectionId = hubConnectionId,
                    BankoId = bankoId,
                    TcKimlikNo = tcKimlikNo,
                    BankoModuAktif = true,
                    BankoModuBaslangic = DateTime.Now,
                    IslemZamani = DateTime.Now,
                    EklenmeTarihi = DateTime.Now,
                    DuzenlenmeTarihi = DateTime.Now
                };

                await repo.AddAsync(bankoConnection);
                await _unitOfWork.SaveChangesAsync();
                
                _logger.LogInformation($"✅ HubBankoConnection oluşturuldu: HubConnectionId={hubConnectionId}, BankoId={bankoId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateBankoConnectionAsync hatası");
                return false;
            }
        }

        public async Task<bool> DeactivateBankoConnectionByHubConnectionIdAsync(int hubConnectionId)
        {
            try
            {
                var bankoRepo = _unitOfWork.Repository<HubBankoConnection>();
                var userRepo = _unitOfWork.Repository<User>();
                
                // HubBankoConnection'ı bul
                var bankoConnection = await bankoRepo.FirstOrDefaultAsync(x => x.HubConnectionId == hubConnectionId && x.BankoModuAktif);
                
                if (bankoConnection == null)
                {
                    _logger.LogWarning($"⚠️ Aktif HubBankoConnection bulunamadı: HubConnectionId={hubConnectionId}");
                    return false;
                }

                // HubBankoConnection'ı deaktif et
                bankoConnection.BankoModuAktif = false;
                bankoConnection.BankoModuBitis = DateTime.Now;
                bankoConnection.DuzenlenmeTarihi = DateTime.Now;
                bankoRepo.Update(bankoConnection);

                // User tablosunu temizle
                var user = await userRepo.FirstOrDefaultAsync(x => x.TcKimlikNo == bankoConnection.TcKimlikNo);
                if (user != null)
                {
                    user.BankoModuAktif = false;
                    user.AktifBankoId = null;
                    user.BankoModuBaslangic = null;
                    user.DuzenlenmeTarihi = DateTime.Now;
                    userRepo.Update(user);
                }

                await _unitOfWork.SaveChangesAsync();
                
                _logger.LogInformation($"✅ Banko modundan çıkış yapıldı: {bankoConnection.TcKimlikNo} | Banko#{bankoConnection.BankoId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeactivateBankoConnectionByHubConnectionIdAsync hatası");
                return false;
            }
        }

        public async Task<List<HubConnection>> GetNonBankoConnectionsByTcKimlikNoAsync(string tcKimlikNo)
        {
            try
            {
                var repo = _unitOfWork.Repository<HubConnection>();
                
                // TcKimlikNo'ya ait tüm bağlantıları al
                var allConnections = await repo.GetAllAsync(x => x.TcKimlikNo == tcKimlikNo);
                
                // HubBankoConnection olmayan bağlantıları filtrele
                var nonBankoConnections = allConnections
                    .Where(x => x.HubBankoConnection == null || !x.HubBankoConnection.BankoModuAktif)
                    .ToList();
                
                return nonBankoConnections;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetNonBankoConnectionsByTcKimlikNoAsync hatası");
                return new List<HubConnection>();
            }
        }

        public async Task<HubConnection?> GetByConnectionIdAsync(string connectionId)
        {
            try
            {
                var repo = _unitOfWork.Repository<HubConnection>();
                return await repo.FirstOrDefaultAsync(x => x.ConnectionId == connectionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByConnectionIdAsync hatası");
                return null;
            }
        }

        public async Task<HubBankoConnection?> GetBankoConnectionByHubConnectionIdAsync(int hubConnectionId)
        {
            try
            {
                var repo = _unitOfWork.Repository<HubBankoConnection>();
                return await repo.FirstOrDefaultAsync(x => x.HubConnectionId == hubConnectionId && x.BankoModuAktif);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBankoConnectionByHubConnectionIdAsync hatası");
                return null;
            }
        }

        public async Task<HubTvConnection?> GetTvConnectionByHubConnectionIdAsync(int hubConnectionId)
        {
            try
            {
                var repo = _unitOfWork.Repository<HubTvConnection>();
                return await repo.FirstOrDefaultAsync(
                    x => x.HubConnectionId == hubConnectionId,
                    x => x.HubConnection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTvConnectionByHubConnectionIdAsync hatası");
                return null;
            }
        }

        public async Task<User?> GetBankoActivePersonelAsync(int bankoId)
        {
            try
            {
                var repo = _unitOfWork.Repository<User>();
                return await repo.FirstOrDefaultAsync(
                    x => x.AktifBankoId == bankoId && x.BankoModuAktif,
                    x => x.Personel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBankoActivePersonelAsync hatası");
                return null;
            }
        }
    }
}

