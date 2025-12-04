using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SignalR;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SignalR;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.ApiLayer.Services.Hubs.Interfaces;
using System.Linq;

namespace SGKPortalApp.ApiLayer.Services.Hubs.Concrete
{
    /// <summary>
    /// Hub Connection Service - ApiLayer için
    /// Business Layer'daki IHubConnectionBusinessService'i doğrudan kullanır
    /// </summary>
    public class HubConnectionService : IHubConnectionService
    {
        private readonly IHubConnectionBusinessService _businessService;
        private readonly ILogger<HubConnectionService> _logger;

        public HubConnectionService(
            IHubConnectionBusinessService businessService,
            ILogger<HubConnectionService> logger)
        {
            _businessService = businessService;
            _logger = logger;
        }

        // ═══════════════════════════════════════════════════════
        // CORE CONNECTION MANAGEMENT
        // ═══════════════════════════════════════════════════════

        public async Task<bool> CreateOrUpdateUserConnectionAsync(string connectionId, string tcKimlikNo)
        {
            return await _businessService.CreateOrUpdateConnectionAsync(connectionId, tcKimlikNo);
        }

        public async Task<bool> DisconnectAsync(string connectionId)
        {
            return await _businessService.DisconnectAsync(connectionId);
        }

        public async Task<List<HubConnectionResponseDto>> GetActiveConnectionsByTcKimlikNoAsync(string tcKimlikNo)
        {
            var entities = await _businessService.GetActiveConnectionsByTcKimlikNoAsync(tcKimlikNo);
            return entities.Select(MapHubConnection).ToList();
        }

        public async Task<HubConnectionResponseDto?> GetByConnectionIdAsync(string connectionId)
        {
            var entity = await _businessService.GetByConnectionIdAsync(connectionId);
            return entity is null ? null : MapHubConnection(entity);
        }

        public async Task<bool> UpdateConnectionTypeAsync(string connectionId, string connectionType)
        {
            return await _businessService.UpdateConnectionTypeAsync(connectionId, connectionType);
        }

        public async Task<bool> SetConnectionStatusAsync(string connectionId, string status)
        {
            return await _businessService.SetConnectionStatusAsync(connectionId, status);
        }

        public async Task<bool> RegisterUserConnectionAsync(string connectionId, string tcKimlikNo, string connectionType)
        {
            var created = await _businessService.CreateOrUpdateConnectionAsync(connectionId, tcKimlikNo);
            if (!created) return false;
            
            if (connectionType != "MainLayout")
            {
                return await _businessService.UpdateConnectionTypeAsync(connectionId, connectionType);
            }
            return true;
        }

        // ═══════════════════════════════════════════════════════
        // BANKO CONNECTION MANAGEMENT
        // ═══════════════════════════════════════════════════════

        public async Task<bool> RegisterBankoConnectionAsync(int bankoId, string connectionId, string tcKimlikNo)
        {
            return await _businessService.RegisterBankoConnectionAsync(bankoId, connectionId, tcKimlikNo);
        }

        public async Task<bool> RegisterBankoConnectionAsync(int bankoId, string connectionId, ConnectionStatus status)
        {
            return await _businessService.RegisterBankoConnectionAsync(bankoId, connectionId, "");
        }

        public async Task<bool> UnregisterBankoConnectionAsync(int bankoId, string connectionId)
        {
            return await _businessService.DeactivateBankoConnectionAsync("");
        }

        public async Task<bool> DeactivateBankoConnectionAsync(string tcKimlikNo)
        {
            return await _businessService.DeactivateBankoConnectionAsync(tcKimlikNo);
        }

        public async Task<bool> IsBankoInUseAsync(int bankoId)
        {
            return await _businessService.IsBankoInUseAsync(bankoId);
        }

        public async Task<bool> IsBankoConnectedAsync(int bankoId)
        {
            return await _businessService.IsBankoInUseAsync(bankoId);
        }

        public async Task<HubBankoConnectionResponseDto?> GetPersonelActiveBankoAsync(string tcKimlikNo)
        {
            var entity = await _businessService.GetPersonelActiveBankoAsync(tcKimlikNo);
            return entity is null ? null : MapHubBankoConnection(entity);
        }

        public Task<int?> GetBankoIdByConnectionIdAsync(string connectionId)
        {
            throw new NotImplementedException("Henüz implement edilmedi");
        }

        public Task<string?> GetConnectionIdByBankoIdAsync(int bankoId)
        {
            throw new NotImplementedException("Henüz implement edilmedi");
        }

        public Task<Dictionary<int, string>> GetAllActiveBankoConnectionsAsync()
        {
            throw new NotImplementedException("Henüz implement edilmedi");
        }

        // ═══════════════════════════════════════════════════════
        // TV CONNECTION MANAGEMENT
        // ═══════════════════════════════════════════════════════

        public async Task<bool> RegisterTvConnectionAsync(int tvId, string connectionId, ConnectionStatus status)
        {
            return await _businessService.RegisterTvConnectionAsync(tvId, connectionId, "");
        }

        public async Task<bool> RegisterTvConnectionAsync(int tvId, string connectionId)
        {
            return await _businessService.RegisterTvConnectionAsync(tvId, connectionId, "");
        }

        public Task<bool> UnregisterTvConnectionAsync(int tvId, string connectionId)
        {
            throw new NotImplementedException("Henüz implement edilmedi");
        }

        public async Task<bool> IsTvInUseByTvUserAsync(int tvId)
        {
            return await _businessService.IsTvInUseByTvUserAsync(tvId);
        }

        public async Task<bool> IsTvConnectedAsync(int tvId)
        {
            return await _businessService.IsTvInUseByTvUserAsync(tvId);
        }

        public Task<int?> GetTvIdByConnectionIdAsync(string connectionId)
        {
            throw new NotImplementedException("Henüz implement edilmedi");
        }

        public Task<string?> GetConnectionIdByTvIdAsync(int tvId)
        {
            throw new NotImplementedException("Henüz implement edilmedi");
        }

        public Task<Dictionary<int, string>> GetAllActiveConnectionsAsync()
        {
            throw new NotImplementedException("Henüz implement edilmedi");
        }

        public async Task<bool> UpdateConnectionStatusAsync(string connectionId, ConnectionStatus status)
        {
            return await _businessService.SetConnectionStatusAsync(connectionId, status.ToString());
        }

        public async Task<bool> CreateBankoConnectionAsync(int hubConnectionId, int bankoId, string tcKimlikNo)
        {
            return await _businessService.CreateBankoConnectionAsync(hubConnectionId, bankoId, tcKimlikNo);
        }

        public async Task<bool> DeactivateBankoConnectionByHubConnectionIdAsync(int hubConnectionId)
        {
            return await _businessService.DeactivateBankoConnectionByHubConnectionIdAsync(hubConnectionId);
        }

        public async Task<List<HubConnectionResponseDto>> GetNonBankoConnectionsByTcKimlikNoAsync(string tcKimlikNo)
        {
            var entities = await _businessService.GetNonBankoConnectionsByTcKimlikNoAsync(tcKimlikNo);
            return entities.Select(MapHubConnection).ToList();
        }

        public async Task<HubBankoConnectionResponseDto?> GetBankoConnectionByHubConnectionIdAsync(int hubConnectionId)
        {
            var entity = await _businessService.GetBankoConnectionByHubConnectionIdAsync(hubConnectionId);
            return entity is null ? null : MapHubBankoConnection(entity);
        }

        public async Task<HubTvConnectionResponseDto?> GetTvConnectionByHubConnectionIdAsync(int hubConnectionId)
        {
            var entity = await _businessService.GetTvConnectionByHubConnectionIdAsync(hubConnectionId);
            return entity is null ? null : MapHubTvConnection(entity);
        }

        public async Task<UserResponseDto?> GetBankoActivePersonelAsync(int bankoId)
        {
            var entity = await _businessService.GetBankoActivePersonelAsync(bankoId);
            return entity is null ? null : MapUser(entity);
        }

        public async Task<bool> TransferBankoConnectionAsync(string tcKimlikNo, string connectionId)
        {
            return await _businessService.TransferBankoConnectionAsync(tcKimlikNo, connectionId);
        }

        // ═══════════════════════════════════════════════════════
        // TV MODE METHODS
        // ═══════════════════════════════════════════════════════

        public async Task<bool> CreateTvConnectionAsync(int hubConnectionId, int tvId, string tcKimlikNo)
        {
            return await _businessService.CreateTvConnectionAsync(hubConnectionId, tvId, tcKimlikNo);
        }

        public async Task<bool> DeactivateTvConnectionByHubConnectionIdAsync(int hubConnectionId)
        {
            return await _businessService.DeactivateTvConnectionByHubConnectionIdAsync(hubConnectionId);
        }

        public async Task<List<HubConnectionResponseDto>> GetNonTvConnectionsByTcKimlikNoAsync(string tcKimlikNo)
        {
            var entities = await _businessService.GetNonTvConnectionsByTcKimlikNoAsync(tcKimlikNo);
            return entities.Select(MapHubConnection).ToList();
        }

        public async Task<HubTvConnectionResponseDto?> GetActiveTvByTcKimlikNoAsync(string tcKimlikNo)
        {
            var entity = await _businessService.GetActiveTvByTcKimlikNoAsync(tcKimlikNo);
            return entity is null ? null : MapHubTvConnection(entity);
        }

        public async Task<UserResponseDto?> GetTvActiveUserAsync(int tvId)
        {
            var entity = await _businessService.GetTvActiveUserAsync(tvId);
            return entity is null ? null : MapUser(entity);
        }

        public async Task<bool> TransferTvConnectionAsync(string tcKimlikNo, string connectionId)
        {
            return await _businessService.TransferTvConnectionAsync(tcKimlikNo, connectionId);
        }

        public async Task<bool> IsTvInUseByOtherTvUserAsync(int tvId, string currentTcKimlikNo)
        {
            return await _businessService.IsTvInUseByOtherTvUserAsync(tvId, currentTcKimlikNo);
        }

        private static HubConnectionResponseDto MapHubConnection(HubConnection entity)
            => new()
            {
                HubConnectionId = entity.HubConnectionId,
                TcKimlikNo = entity.TcKimlikNo,
                ConnectionId = entity.ConnectionId,
                ConnectionType = entity.ConnectionType,
                ConnectionStatus = entity.ConnectionStatus,
                ConnectedAt = entity.ConnectedAt,
                LastActivityAt = entity.LastActivityAt
            };

        private static HubBankoConnectionResponseDto MapHubBankoConnection(HubBankoConnection entity)
            => new()
            {
                HubBankoConnectionId = entity.HubBankoConnectionId,
                HubConnectionId = entity.HubConnectionId,
                BankoId = entity.BankoId,
                TcKimlikNo = entity.TcKimlikNo,
                BankoModuAktif = entity.BankoModuAktif,
                BankoModuBaslangic = entity.BankoModuBaslangic,
                BankoModuBitis = entity.BankoModuBitis
            };

        private static HubTvConnectionResponseDto MapHubTvConnection(HubTvConnection entity)
            => new()
            {
                HubTvConnectionId = entity.HubTvConnectionId,
                HubConnectionId = entity.HubConnectionId,
                TvId = entity.TvId,
                TcKimlikNo = entity.HubConnection?.TcKimlikNo ?? string.Empty
            };

        private static UserResponseDto MapUser(User entity)
        {
            var dto = new UserResponseDto
            {
                TcKimlikNo = entity.TcKimlikNo,
                AktifMi = entity.AktifMi,
                SonGirisTarihi = entity.SonGirisTarihi,
                BasarisizGirisSayisi = entity.BasarisizGirisSayisi,
                HesapKilitTarihi = entity.HesapKilitTarihi,
                SessionID = entity.SessionID,
                EklenmeTarihi = entity.EklenmeTarihi,
                DuzenlenmeTarihi = entity.DuzenlenmeTarihi
            };

            if (entity.Personel is Personel personel)
            {
                dto.PersonelAdSoyad = personel.AdSoyad;
                dto.Email = personel.Email;
                dto.CepTelefonu = personel.CepTelefonu;
                dto.SicilNo = personel.SicilNo;
                dto.DepartmanAdi = personel.Departman?.DepartmanAdi;
                dto.ServisAdi = personel.Servis?.ServisAdi;
            }

            return dto;
        }
    }
}
