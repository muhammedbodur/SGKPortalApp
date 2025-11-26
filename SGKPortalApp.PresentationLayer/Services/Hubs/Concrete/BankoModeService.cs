using Microsoft.AspNetCore.SignalR;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
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
        private readonly IUserApiService _userApiService;
        private readonly ILogger<BankoModeService> _logger;

        public BankoModeService(
            IHubConnectionService connectionService,
            IBankoApiService bankoApiService,
            IHubContext<SiramatikHub> hubContext,
            BankoModeStateService stateService,
            IUserApiService userApiService,
            ILogger<BankoModeService> logger)
        {
            _connectionService = connectionService;
            _bankoApiService = bankoApiService;
            _hubContext = hubContext;
            _stateService = stateService;
            _userApiService = userApiService;
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
        /// Personel banko modunda mı? (User tablosundan kontrol - API üzerinden)
        /// </summary>
        public async Task<bool> IsPersonelInBankoModeAsync(string tcKimlikNo)
        {
            try
            {
                var result = await _userApiService.IsBankoModeActiveAsync(tcKimlikNo);
                return result.Success && result.Data;
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
                return user?.PersonelAdSoyad;
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
        public async Task<bool> EnterBankoModeAsync(string tcKimlikNo, int bankoId, string? currentConnectionId = null)
        {
            try
            {
                _logger.LogInformation($"🏦 Banko moduna geçiliyor: {tcKimlikNo} -> Banko#{bankoId} | Aktif ConnectionId: {currentConnectionId}");

                // 1. Bu banko başka personel tarafından kullanılıyor mu?
                var bankoInUse = await IsBankoInUseAsync(bankoId);
                if (bankoInUse)
                {
                    var activePerson = await GetBankoActivePersonelNameAsync(bankoId);
                    _logger.LogWarning($"❌ Banko#{bankoId} kullanımda: {activePerson}");
                    return false;
                }

                // 2. Bu personel başka bankoda mı? (User tablosundan kontrol - API üzerinden)
                var activeBankoResult = await _userApiService.GetActiveBankoIdAsync(tcKimlikNo);
                if (activeBankoResult.Success && activeBankoResult.Data.HasValue && activeBankoResult.Data.Value != bankoId)
                {
                    _logger.LogWarning($"❌ {tcKimlikNo} zaten Banko#{activeBankoResult.Data.Value}'de aktif");
                    return false;
                }

                // 3. State'i güncelle (ÖNCE!)
                _stateService.ActivateBankoMode(bankoId, tcKimlikNo);

                // 4. User tablosunda banko modunu aktif et (API üzerinden)
                var activateResult = await _userApiService.ActivateBankoModeAsync(tcKimlikNo, bankoId);
                if (!activateResult.Success)
                {
                    _logger.LogError($"❌ User tablosunda banko modu aktif edilemedi: {tcKimlikNo}");
                    _stateService.DeactivateBankoMode(); // Rollback
                    return false;
                }

                _logger.LogInformation($"✅ Banko modu aktif: {tcKimlikNo} -> Banko#{bankoId}");
                
                // 5. Arka planda HubBankoConnection OLMAYAN bağlantıları kapat (await etme!)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        // Aktif tab'ın yenilenmesi ve HubBankoConnection oluşturması için gecikme
                        await Task.Delay(1000);
                        
                        // HubBankoConnection olmayan (normal) bağlantıları al (DTO)
                        var nonBankoConnectionDtos = await _connectionService.GetNonBankoConnectionsByTcKimlikNoAsync(tcKimlikNo);

                        if (nonBankoConnectionDtos.Any())
                        {
                            _logger.LogInformation($"🔄 {nonBankoConnectionDtos.Count} adet eski bağlantı kapatılıyor...");
                            
                            // Eski bağlantıları kapat (ForceLogout)
                            foreach (var connDto in nonBankoConnectionDtos)
                            {
                                await _hubContext.Clients.Client(connDto.ConnectionId)
                                    .SendAsync("ForceLogout", "Banko moduna geçildi. Diğer sekmeler kapatılıyor.");

                                await _connectionService.DisconnectAsync(connDto.ConnectionId);

                                _logger.LogInformation($"⚠️ Eski bağlantı kapatıldı: {connDto.ConnectionId}");
                            }
                        }
                        else
                        {
                            _logger.LogInformation($"✅ Kapatılacak eski bağlantı yok - Banko modu bağlantısı başarıyla oluşturuldu");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Eski bağlantıları kapatma hatası");
                    }
                });
                
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

                // 1. User tablosunda banko modunu deaktif et (API üzerinden) - ÖNCELİKLE!
                var deactivateResult = await _userApiService.DeactivateBankoModeAsync(tcKimlikNo);
                if (!deactivateResult.Success)
                {
                    _logger.LogError($"❌ User tablosunda banko modu deaktif edilemedi: {tcKimlikNo}");
                    return false;
                }

                // 2. Personelin aktif banko oturumunu kapat
                await _connectionService.DeactivateBankoConnectionAsync(tcKimlikNo);

                // 3. State'i güncelle
                _stateService.DeactivateBankoMode();

                _logger.LogInformation($"✅ Banko modundan çıkıldı: {tcKimlikNo}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Banko modundan çıkış hatası: {tcKimlikNo}");
                return false;
            }
        }
    }
}
