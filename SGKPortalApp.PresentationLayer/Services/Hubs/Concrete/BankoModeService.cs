using Microsoft.AspNetCore.SignalR;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.Hubs.Interfaces;
using SGKPortalApp.PresentationLayer.Services.State;

namespace SGKPortalApp.PresentationLayer.Services.Hubs.Concrete
{
    /// <summary>
    /// Banko modu yönetimi servisi (C# Öncelikli - Layered Architecture)
    /// </summary>
    public class BankoModeService : IBankoModeService
    {
        private readonly IHubConnectionService _connectionService;
        private readonly IBankoApiService _bankoApiService;
        private readonly IHubContext<SiramatikHub> _hubContext;
        private readonly BankoModeStateService _stateService;
        private readonly ILogger<BankoModeService> _logger;

        public BankoModeService(
            IHubConnectionService connectionService,
            IBankoApiService bankoApiService,
            IHubContext<SiramatikHub> hubContext,
            BankoModeStateService stateService,
            ILogger<BankoModeService> logger)
        {
            _connectionService = connectionService;
            _bankoApiService = bankoApiService;
            _hubContext = hubContext;
            _stateService = stateService;
            _logger = logger;
        }

        /// <summary>
        /// Personelin atanmış banko bilgisini getir (Layered Architecture)
        /// </summary>
        public async Task<BankoResponseDto?> GetPersonelAssignedBankoAsync(string tcKimlikNo)
        {
            try
            {
                // ApiService üzerinden Business Layer'a git
                var result = await _bankoApiService.GetPersonelCurrentBankoAsync(tcKimlikNo);
                return result.Success ? result.Data : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Personel banko bilgisi alınamadı: {tcKimlikNo}");
                return null;
            }
        }

        /// <summary>
        /// Personel banko modunda mı?
        /// </summary>
        public async Task<bool> IsPersonelInBankoModeAsync(string tcKimlikNo)
        {
            try
            {
                var bankoConnection = await _connectionService.GetPersonelActiveBankoAsync(tcKimlikNo);
                return bankoConnection != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Personel banko modu kontrolü hatası: {tcKimlikNo}");
                return false;
            }
        }

        /// <summary>
        /// Banko kullanımda mı?
        /// </summary>
        public async Task<bool> IsBankoInUseAsync(int bankoId)
        {
            try
            {
                return await _connectionService.IsBankoInUseAsync(bankoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Banko kullanım kontrolü hatası: Banko#{bankoId}");
                return false;
            }
        }

        /// <summary>
        /// Bankodaki aktif personel bilgisini getir
        /// </summary>
        public async Task<string?> GetBankoActivePersonelNameAsync(int bankoId)
        {
            try
            {
                var user = await _connectionService.GetBankoActivePersonelAsync(bankoId);
                return user?.Personel?.AdSoyad;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Banko personel bilgisi alınamadı: Banko#{bankoId}");
                return null;
            }
        }

        /// <summary>
        /// Banko moduna geç (Tam C# implementasyonu)
        /// </summary>
        public async Task<bool> EnterBankoModeAsync(string tcKimlikNo, int bankoId)
        {
            try
            {
                _logger.LogInformation($"🏦 Banko moduna geçiliyor: {tcKimlikNo} -> Banko#{bankoId}");

                // 1. Bu banko başka personel tarafından kullanılıyor mu?
                var bankoInUse = await IsBankoInUseAsync(bankoId);
                if (bankoInUse)
                {
                    var activePerson = await GetBankoActivePersonelNameAsync(bankoId);
                    _logger.LogWarning($"❌ Banko#{bankoId} kullanımda: {activePerson}");
                    return false;
                }

                // 2. Bu personel başka bankoda mı?
                var existingBanko = await _connectionService.GetPersonelActiveBankoAsync(tcKimlikNo);
                if (existingBanko != null && existingBanko.BankoId != bankoId)
                {
                    _logger.LogWarning($"❌ {tcKimlikNo} zaten Banko#{existingBanko.BankoId}'de aktif");
                    return false;
                }

                // 3. Bu personelin tüm aktif bağlantılarını al
                var activeConnections = await _connectionService.GetActiveConnectionsByTcKimlikNoAsync(tcKimlikNo);
                
                if (activeConnections.Any())
                {
                    // 4. Diğer tab'ları kapat (ForceLogout)
                    foreach (var conn in activeConnections)
                    {
                        await _hubContext.Clients.Client(conn.ConnectionId)
                            .SendAsync("ForceLogout", "Banko moduna geçildi. Diğer sekmeler kapatılıyor.");
                        
                        await _connectionService.DisconnectAsync(conn.ConnectionId);
                        
                        _logger.LogInformation($"⚠️ Diğer tab kapatıldı: {conn.ConnectionId}");
                    }
                }

                // 5. Yeni bağlantı oluştur (BankoMode)
                // NOT: Blazor Server'da ConnectionId'yi alamıyoruz, bu yüzden
                // personel banko sayfasına gittiğinde OnConnectedAsync'de oluşturulacak
                
                // 6. State'i güncelle
                _stateService.ActivateBankoMode(bankoId, tcKimlikNo);
                
                _logger.LogInformation($"✅ Banko modu aktif: {tcKimlikNo} -> Banko#{bankoId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Banko moduna giriş hatası: {tcKimlikNo}");
                return false;
            }
        }

        /// <summary>
        /// Banko modundan çık (Tam C# implementasyonu)
        /// </summary>
        public async Task<bool> ExitBankoModeAsync(string tcKimlikNo)
        {
            try
            {
                _logger.LogInformation($"🚪 Banko modundan çıkılıyor: {tcKimlikNo}");

                // 1. Personelin aktif banko oturumunu kapat
                var success = await _connectionService.DeactivateBankoConnectionAsync(tcKimlikNo);
                
                if (success)
                {
                    // 2. State'i güncelle
                    _stateService.DeactivateBankoMode();
                    
                    _logger.LogInformation($"✅ Banko modundan çıkıldı: {tcKimlikNo}");
                }
                
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Banko modundan çıkış hatası: {tcKimlikNo}");
                return false;
            }
        }
    }
}
