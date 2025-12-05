using Microsoft.AspNetCore.SignalR;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.ApiLayer.Services.Hubs.Interfaces;
using SGKPortalApp.ApiLayer.Services.State;

namespace SGKPortalApp.ApiLayer.Services.Hubs.Concrete
{
    /// <summary>
    /// Banko modu yönetimi servisi - ApiLayer için
    /// Business Layer servislerini doğrudan kullanır
    /// </summary>
    public class BankoModeService : IBankoModeService
    {
        private readonly IHubConnectionService _connectionService;
        private readonly IBankoService _bankoService;
        private readonly IHubContext<SiramatikHub> _hubContext;
        private readonly BankoModeStateService _stateService;
        private readonly IUserService _userService;
        private readonly ILogger<BankoModeService> _logger;

        public BankoModeService(
            IHubConnectionService connectionService,
            IBankoService bankoService,
            IHubContext<SiramatikHub> hubContext,
            BankoModeStateService stateService,
            IUserService userService,
            ILogger<BankoModeService> logger)
        {
            _connectionService = connectionService;
            _bankoService = bankoService;
            _hubContext = hubContext;
            _stateService = stateService;
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Personelin atanmış banko bilgisini getir (Layered Architecture)
        /// </summary>
        public async Task<BankoResponseDto?> GetPersonelAssignedBankoAsync(string tcKimlikNo)
        {
            try
            {
                var response = await _bankoService.GetPersonelCurrentBankoAsync(tcKimlikNo);
                if (!response.Success)
                {
                    _logger.LogWarning("Personel banko bilgisi alınamadı: {Tc}", tcKimlikNo);
                    return null;
                }

                return response.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel banko bilgisi alınamadı: {Tc}", tcKimlikNo);
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
                var response = await _userService.IsBankoModeActiveAsync(tcKimlikNo);
                return response.Success && response.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel banko modu kontrolü hatası: {Tc}", tcKimlikNo);
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
                    // Aynı kullanıcı mı kontrol et (sayfa yenileme durumu)
                    var activePersonel = await _connectionService.GetBankoActivePersonelAsync(bankoId);
                    if (activePersonel != null && activePersonel.TcKimlikNo == tcKimlikNo)
                    {
                        // Aynı kullanıcı, devam et (sayfa yenileme veya yeni sekme)
                        _logger.LogInformation($"✅ Banko#{bankoId} zaten {tcKimlikNo} tarafından kullanılıyor, devam ediliyor...");
                    }
                    else
                    {
                        var activePerson = activePersonel?.PersonelAdSoyad ?? "başka bir personel";
                        _logger.LogWarning($"❌ Banko#{bankoId} kullanımda: {activePerson}");
                        return false;
                    }
                }

                // 2. Bu personel başka bankoda mı?
                var activeBankoResponse = await _userService.GetActiveBankoIdAsync(tcKimlikNo);
                if (activeBankoResponse.Success && activeBankoResponse.Data.HasValue && activeBankoResponse.Data.Value != bankoId)
                {
                    _logger.LogWarning($"❌ {tcKimlikNo} zaten Banko#{activeBankoResponse.Data.Value}'de aktif");
                    return false;
                }

                // 3. State'i güncelle (ÖNCE!)
                _stateService.ActivateBankoMode(bankoId, tcKimlikNo);

                // 4. User tablosunda banko modunu aktif et
                var activateResponse = await _userService.ActivateBankoModeAsync(tcKimlikNo, bankoId);
                if (!activateResponse.Success || !activateResponse.Data)
                {
                    _logger.LogError($"❌ User tablosunda banko modu aktif edilemedi: {tcKimlikNo}");
                    _stateService.DeactivateBankoMode(tcKimlikNo); // Rollback
                    return false;
                }

                _logger.LogInformation($"✅ Banko modu aktif: {tcKimlikNo} -> Banko#{bankoId}");
                
                // 5. ⭐ Eski bağlantılar otomatik kapanacak
                // Widget sayfa yenilediğinde (forceLoad: true) eski connection otomatik disconnect olur
                // OnDisconnectedAsync zaten temizlik yapacak, burada bir şey yapmaya gerek yok
                _logger.LogInformation($"✅ Banko modu aktif edildi. Sayfa yenilendiğinde eski bağlantılar otomatik kapanacak.");
                
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

                // 1. User tablosunda banko modunu deaktif et - ÖNCELİKLE!
                var deactivateResponse = await _userService.DeactivateBankoModeAsync(tcKimlikNo);
                if (!deactivateResponse.Success || !deactivateResponse.Data)
                {
                    _logger.LogError($"❌ User tablosunda banko modu deaktif edilemedi: {tcKimlikNo}");
                    return false;
                }

                // 2. Personelin aktif banko oturumunu kapat
                await _connectionService.DeactivateBankoConnectionAsync(tcKimlikNo);

                // 3. State'i güncelle
                _stateService.DeactivateBankoMode(tcKimlikNo);

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
