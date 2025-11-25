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
    }
}
