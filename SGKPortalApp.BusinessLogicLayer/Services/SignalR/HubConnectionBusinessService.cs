using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SignalR;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;

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

                // Bağlantıları ConnectionId bazında yönet (aynı kullanıcı birden fazla sekme açabilir)
                var connection = await repo.FirstOrDefaultAsync(c => c.ConnectionId == connectionId);

                if (connection != null)
                {
                    connection.ConnectionStatus = BusinessObjectLayer.Enums.SiramatikIslemleri.ConnectionStatus.online;
                    connection.IslemZamani = DateTime.Now;
                    connection.DuzenlenmeTarihi = DateTime.Now;
                    connection.SilindiMi = false;
                    connection.SilinmeTarihi = null;
                    connection.SilenKullanici = null;
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
                var connection = await repo.FirstOrDefaultAsync(c => c.ConnectionId == connectionId && !c.SilindiMi);

                if (connection != null)
                {
                    // Sadece HubConnection'ı offline yap
                    // İlişkili HubBankoConnection ve HubTvConnection temizliği
                    // OnDisconnectedAsync içinde ConnectionType'a göre yapılıyor:
                    // - BankoMode: HubBankoConnection KORUNUR (sayfa yenileme için TransferBankoConnectionAsync kullanılır)
                    // - TvMode: HubTvConnection TEMİZLENİR (DeactivateTvConnectionByHubConnectionIdAsync ile)
                    connection.ConnectionStatus = BusinessObjectLayer.Enums.SiramatikIslemleri.ConnectionStatus.offline;
                    connection.IslemZamani = DateTime.Now;
                    connection.DuzenlenmeTarihi = DateTime.Now;
                    connection.LastActivityAt = DateTime.Now;
                    connection.SilindiMi = true;
                    connection.SilinmeTarihi = DateTime.Now;
                    connection.SilenKullanici = "SignalR_Disconnect";
                    repo.Update(connection);
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
                    c.ConnectionStatus == BusinessObjectLayer.Enums.SiramatikIslemleri.ConnectionStatus.online && !c.SilindiMi);
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
                // 1. HubConnection'ı bul
                var hubConnectionRepo = _unitOfWork.Repository<HubConnection>();
                var hubConnection = await hubConnectionRepo.FirstOrDefaultAsync(
                    h => h.ConnectionId == connectionId && h.ConnectionStatus == ConnectionStatus.online && h.SilindiMi == false);
                
                if (hubConnection == null)
                {
                    _logger.LogError($"HubConnection bulunamadı: {connectionId}");
                    return false;
                }
                
                var repo = _unitOfWork.Repository<HubBankoConnection>();
                
                // 2. ⭐ Bu banko veya personel için var olan tüm kayıtları fiziksel olarak sil
                var recordsToDelete = (await repo.FindAsync(
                    b => b.BankoId == bankoId || b.TcKimlikNo == tcKimlikNo)).ToList();
                
                if (recordsToDelete.Any())
                {
                    repo.DeleteRange(recordsToDelete);
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogWarning($"⚠️ {recordsToDelete.Count} eski HubBankoConnection kaydı silindi (Banko#{bankoId}, TC#{tcKimlikNo})");
                }
                
                // 3. Yeni HubBankoConnection oluştur
                var bankoConnection = new HubBankoConnection
                {
                    HubConnectionId = hubConnection.HubConnectionId,
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
                
                _logger.LogInformation($"✅ Banko bağlantısı oluşturuldu: Banko#{bankoId}, TC#{tcKimlikNo}");
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
                var bankoRepo = _unitOfWork.Repository<HubBankoConnection>();
                var userRepo = _unitOfWork.Repository<User>();

                var bankoConnection = await bankoRepo.FirstOrDefaultAsync(c => c.TcKimlikNo == tcKimlikNo && c.BankoModuAktif);

                if (bankoConnection != null)
                {
                    // HubBankoConnection'ı deaktif et ve soft-delete yap
                    bankoConnection.BankoModuAktif = false;
                    bankoConnection.BankoModuBitis = DateTime.Now;
                    bankoConnection.SilindiMi = true;
                    bankoConnection.SilinmeTarihi = DateTime.Now;
                    bankoConnection.SilenKullanici = "ExitBankoMode";
                    bankoConnection.DuzenlenmeTarihi = DateTime.Now;
                    bankoRepo.Update(bankoConnection);

                    // User tablosunu temizle
                    var user = await userRepo.FirstOrDefaultAsync(x => x.TcKimlikNo == tcKimlikNo);
                    if (user != null)
                    {
                        user.BankoModuAktif = false;
                        user.AktifBankoId = null;
                        user.BankoModuBaslangic = null;
                        user.DuzenlenmeTarihi = DateTime.Now;
                        userRepo.Update(user);
                    }

                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation($"✅ Banko modundan çıkış: {tcKimlikNo} | Banko#{bankoConnection.BankoId}");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeactivateBankoConnectionAsync hatası");
                return false;
            }
        }

        /// <summary>
        /// TV bağlantısını kaydet (2 parametreli - HubConnection'dan tcKimlikNo alır)
        /// </summary>
        public async Task<bool> RegisterTvConnectionAsync(int tvId, string connectionId)
        {
            try
            {
                // 1. HubConnection'ı bul
                var hubConnectionRepo = _unitOfWork.Repository<HubConnection>();
                var hubConnection = await hubConnectionRepo.FirstOrDefaultAsync(
                    h => h.ConnectionId == connectionId && h.ConnectionStatus == ConnectionStatus.online && h.SilindiMi == false);

                if (hubConnection == null)
                {
                    _logger.LogError($"HubConnection bulunamadı: {connectionId}");
                    return false;
                }

                var repo = _unitOfWork.Repository<HubTvConnection>();

                // 2. ⚠️ TV için eski kayıtları silmeye GEREK YOK (TvId unique değil, birden fazla kullanıcı aynı TV'yi izleyebilir)
                // Ama aynı HubConnectionId için varsa güncelle (örneğin sayfa yenileme)
                var existingTvConnection = await repo.FirstOrDefaultAsync(
                    t => t.HubConnectionId == hubConnection.HubConnectionId && !t.SilindiMi);

                if (existingTvConnection != null)
                {
                    // Mevcut kaydı güncelle
                    existingTvConnection.TvId = tvId;
                    existingTvConnection.IslemZamani = DateTime.Now;
                    existingTvConnection.DuzenlenmeTarihi = DateTime.Now;
                    repo.Update(existingTvConnection);
                    _logger.LogInformation($"✅ Mevcut TV bağlantısı güncellendi: TV#{tvId}, HubConnectionId={hubConnection.HubConnectionId}, TcKimlikNo={hubConnection.TcKimlikNo}");
                }
                else
                {
                    // 3. Yeni HubTvConnection oluştur
                    var tvConnection = new HubTvConnection
                    {
                        HubConnectionId = hubConnection.HubConnectionId, // ⭐ KRİTİK FİX!
                        TvId = tvId,
                        IslemZamani = DateTime.Now,
                        EklenmeTarihi = DateTime.Now,
                        DuzenlenmeTarihi = DateTime.Now
                    };

                    await repo.AddAsync(tvConnection);
                    _logger.LogInformation($"✅ TV bağlantısı oluşturuldu: TV#{tvId}, HubConnectionId={hubConnection.HubConnectionId}, TcKimlikNo={hubConnection.TcKimlikNo}");
                }

                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RegisterTvConnectionAsync hatası");
                return false;
            }
        }

        /// <summary>
        /// TV bağlantısını kaydet (3 parametreli - backward compatibility)
        /// </summary>
        public async Task<bool> RegisterTvConnectionAsync(int tvId, string connectionId, string tcKimlikNo)
        {
            // tcKimlikNo parametresi kullanılmıyor, HubConnection'dan alınıyor
            return await RegisterTvConnectionAsync(tvId, connectionId);
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

                // Önce ConnectionId ile sadece Id bilgisini al (AsNoTracking)
                var connectionProjection = await repo.FirstOrDefaultAsync(c => c.ConnectionId == connectionId);

                if (connectionProjection != null)
                {
                    // Sonra primary key üzerinden tracked entity'yi getir
                    var trackedConnection = await repo.GetByIdAsync(connectionProjection.HubConnectionId);

                    if (trackedConnection != null)
                    {
                        trackedConnection.ConnectionType = connectionType;
                        trackedConnection.DuzenlenmeTarihi = DateTime.Now;
                        await _unitOfWork.SaveChangesAsync();
                    }
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

                // Önce ConnectionId ile sadece Id bilgisini al (AsNoTracking)
                var connectionProjection = await repo.FirstOrDefaultAsync(c => c.ConnectionId == connectionId);

                if (connectionProjection != null &&
                    Enum.TryParse<BusinessObjectLayer.Enums.SiramatikIslemleri.ConnectionStatus>(status, out var connectionStatus))
                {
                    // Sonra primary key üzerinden tracked entity'yi getir
                    var trackedConnection = await repo.GetByIdAsync(connectionProjection.HubConnectionId);

                    if (trackedConnection != null)
                    {
                        trackedConnection.ConnectionStatus = connectionStatus;
                        trackedConnection.DuzenlenmeTarihi = DateTime.Now;
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
                var tvRepo = _unitOfWork.Repository<HubTvConnection>();
                var connectionRepo = _unitOfWork.Repository<HubConnection>();
                var userRepo = _unitOfWork.Repository<User>();

                // 1. Bu TV'deki tüm aktif HubTvConnection'ları al
                var activeTvConnections = await tvRepo.FindAsync(
                    t => t.TvId == tvId && !t.SilindiMi);

                if (!activeTvConnections.Any())
                    return false; // Kimse kullanmıyor

                // 2. Her HubTvConnection için User'ı kontrol et
                foreach (var tvConn in activeTvConnections)
                {
                    // HubConnection'ı al
                    var hubConnection = await connectionRepo.FirstOrDefaultAsync(
                        h => h.HubConnectionId == tvConn.HubConnectionId &&
                             h.ConnectionStatus == ConnectionStatus.online &&
                             !h.SilindiMi);

                    if (hubConnection != null)
                    {
                        // User'ı al
                        var user = await userRepo.FirstOrDefaultAsync(
                            u => u.TcKimlikNo == hubConnection.TcKimlikNo);

                        // ⭐ Sadece başka bir TvUser kullanıyorsa true dön
                        if (user != null && user.UserType == UserType.TvUser)
                        {
                            _logger.LogInformation($"✅ TV#{tvId} başka bir TvUser tarafından kullanılıyor: {user.TcKimlikNo}");
                            return true;
                        }
                    }
                }

                // Personel kullanıyor veya kimse kullanmıyor
                _logger.LogDebug($"✅ TV#{tvId} TvUser tarafından kullanılmıyor (Personel kullanabilir)");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IsTvInUseByTvUserAsync hatası");
                return false;
            }
        }

        public async Task<bool> IsTvInUseByOtherTvUserAsync(int tvId, string currentTcKimlikNo)
        {
            try
            {
                var tvRepo = _unitOfWork.Repository<HubTvConnection>();
                var connectionRepo = _unitOfWork.Repository<HubConnection>();
                var userRepo = _unitOfWork.Repository<User>();

                // 1. Bu TV'deki tüm aktif HubTvConnection'ları al
                var activeTvConnections = await tvRepo.FindAsync(
                    t => t.TvId == tvId && !t.SilindiMi);

                if (!activeTvConnections.Any())
                    return false; // Kimse kullanmıyor

                // 2. Her HubTvConnection için User'ı kontrol et
                foreach (var tvConn in activeTvConnections)
                {
                    // HubConnection'ı al
                    var hubConnection = await connectionRepo.FirstOrDefaultAsync(
                        h => h.HubConnectionId == tvConn.HubConnectionId &&
                             h.ConnectionStatus == ConnectionStatus.online &&
                             !h.SilindiMi);

                    if (hubConnection != null)
                    {
                        // ⭐ Kendi TcKimlikNo'yu atla (aynı kullanıcının diğer ekranları)
                        if (hubConnection.TcKimlikNo == currentTcKimlikNo)
                            continue;

                        // User'ı al
                        var user = await userRepo.FirstOrDefaultAsync(
                            u => u.TcKimlikNo == hubConnection.TcKimlikNo);

                        // ⭐ Sadece BAŞKA bir TvUser kullanıyorsa true dön
                        if (user != null && user.UserType == UserType.TvUser)
                        {
                            _logger.LogInformation($"✅ TV#{tvId} BAŞKA bir TvUser tarafından kullanılıyor: {user.TcKimlikNo} (mevcut: {currentTcKimlikNo})");
                            return true;
                        }
                    }
                }

                // Sadece kendisi kullanıyor, Personel kullanıyor veya kimse kullanmıyor
                _logger.LogDebug($"✅ TV#{tvId} başka bir TvUser tarafından kullanılmıyor (mevcut: {currentTcKimlikNo})");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IsTvInUseByOtherTvUserAsync hatası");
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

                // HubBankoConnection'ı deaktif et ve soft-delete yap
                bankoConnection.BankoModuAktif = false;
                bankoConnection.BankoModuBitis = DateTime.Now;
                bankoConnection.SilindiMi = true;
                bankoConnection.SilinmeTarihi = DateTime.Now;
                bankoConnection.SilenKullanici = "ExitBankoMode";
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
                
                // TcKimlikNo'ya ait tüm bağlantıları al (HubBankoConnection navigation property ile)
                var allConnections = await repo.GetAllAsync(x => x.HubBankoConnection);
                
                // TcKimlikNo'ya göre filtrele ve HubBankoConnection olmayan bağlantıları al
                var nonBankoConnections = allConnections
                    .Where(x => x.TcKimlikNo == tcKimlikNo && 
                               (x.HubBankoConnection == null || !x.HubBankoConnection.BankoModuAktif))
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

        public async Task<bool> TransferBankoConnectionAsync(string tcKimlikNo, string newConnectionId)
        {
            try
            {
                var connectionRepo = _unitOfWork.Repository<HubConnection>();
                var bankoRepo = _unitOfWork.Repository<HubBankoConnection>();

                // Önce yeni connection için sadece Id bilgisini al (AsNoTracking)
                var newConnectionProjection = await connectionRepo.FirstOrDefaultAsync(
                    c => c.ConnectionId == newConnectionId && !c.SilindiMi);

                if (newConnectionProjection == null)
                {
                    _logger.LogWarning("TransferBankoConnectionAsync: Yeni connection bulunamadı ({ConnectionId})", newConnectionId);
                    return false;
                }

                // Sonra primary key üzerinden tracked entity'yi getir
                var newConnection = await connectionRepo.GetByIdAsync(newConnectionProjection.HubConnectionId);

                if (newConnection == null)
                {
                    _logger.LogWarning("TransferBankoConnectionAsync: Yeni connection tracked olarak getirilemedi ({ConnectionId})", newConnectionId);
                    return false;
                }

                var bankoConnection = await bankoRepo.FirstOrDefaultAsync(
                    b => b.TcKimlikNo == tcKimlikNo && b.BankoModuAktif);

                if (bankoConnection == null)
                {
                    _logger.LogWarning("TransferBankoConnectionAsync: Aktif banko oturumu bulunamadı ({TcKimlikNo})", tcKimlikNo);
                    return false;
                }

                if (bankoConnection.HubConnectionId == newConnection.HubConnectionId)
                {
                    return true;
                }

                // Eski connection'ı soft delete yap
                var oldConnection = await connectionRepo.FirstOrDefaultAsync(
                    c => c.HubConnectionId == bankoConnection.HubConnectionId && !c.SilindiMi);

                if (oldConnection != null)
                {
                    oldConnection.ConnectionStatus = ConnectionStatus.offline;
                    oldConnection.LastActivityAt = DateTime.Now;
                    oldConnection.IslemZamani = DateTime.Now;
                    oldConnection.DuzenlenmeTarihi = DateTime.Now;
                    oldConnection.SilindiMi = true;
                    oldConnection.SilinmeTarihi = DateTime.Now;
                    oldConnection.SilenKullanici = "BankoTransfer";
                    connectionRepo.Update(oldConnection);
                }

                bankoConnection.HubConnectionId = newConnection.HubConnectionId;
                bankoConnection.IslemZamani = DateTime.Now;
                bankoConnection.DuzenlenmeTarihi = DateTime.Now;
                bankoRepo.Update(bankoConnection);

                newConnection.ConnectionType = "BankoMode";
                newConnection.ConnectionStatus = ConnectionStatus.online;
                newConnection.LastActivityAt = DateTime.Now;
                newConnection.IslemZamani = DateTime.Now;
                newConnection.DuzenlenmeTarihi = DateTime.Now;
                connectionRepo.Update(newConnection);

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("✅ Banko bağlantısı devredildi: {TcKimlikNo} -> HubConnection#{HubConnectionId}", tcKimlikNo, newConnection.HubConnectionId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TransferBankoConnectionAsync hatası");
                return false;
            }
        }

        // ═══════════════════════════════════════════════════════
        // TV MODE METHODS (mirroring Banko pattern)
        // ═══════════════════════════════════════════════════════

        public async Task<bool> CreateTvConnectionAsync(int hubConnectionId, int tvId, string tcKimlikNo)
        {
            try
            {
                var repo = _unitOfWork.Repository<HubTvConnection>();

                // TV için eski kayıtları silmeye gerek yok (birden fazla kullanıcı aynı TV'yi izleyebilir)
                // Sadece yeni kayıt oluştur
                var tvConnection = new HubTvConnection
                {
                    HubConnectionId = hubConnectionId,
                    TvId = tvId,
                    IslemZamani = DateTime.Now,
                    EklenmeTarihi = DateTime.Now,
                    DuzenlenmeTarihi = DateTime.Now
                };

                await repo.AddAsync(tvConnection);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"✅ HubTvConnection oluşturuldu: HubConnectionId={hubConnectionId}, TvId={tvId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateTvConnectionAsync hatası");
                return false;
            }
        }

        public async Task<bool> DeactivateTvConnectionByHubConnectionIdAsync(int hubConnectionId)
        {
            try
            {
                var tvRepo = _unitOfWork.Repository<HubTvConnection>();

                // HubTvConnection'ı bul
                var tvConnection = await tvRepo.FirstOrDefaultAsync(
                    x => x.HubConnectionId == hubConnectionId && !x.SilindiMi);

                if (tvConnection == null)
                {
                    _logger.LogWarning($"⚠️ HubTvConnection bulunamadı: HubConnectionId={hubConnectionId}");
                    return false;
                }

                // TV için User tablosu güncellemesi YOK (Banko'dan fark)
                // Sadece HubTvConnection'ı soft delete yap
                tvConnection.SilindiMi = true;
                tvConnection.SilinmeTarihi = DateTime.Now;
                tvConnection.SilenKullanici = "TvDisconnect";
                tvConnection.DuzenlenmeTarihi = DateTime.Now;
                tvRepo.Update(tvConnection);

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"✅ TV bağlantısı kapatıldı: HubConnectionId={hubConnectionId} | TV#{tvConnection.TvId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeactivateTvConnectionByHubConnectionIdAsync hatası");
                return false;
            }
        }

        public async Task<HubTvConnection?> GetActiveTvByTcKimlikNoAsync(string tcKimlikNo)
        {
            try
            {
                var tvRepo = _unitOfWork.Repository<HubTvConnection>();
                var connectionRepo = _unitOfWork.Repository<HubConnection>();

                // TcKimlikNo'ya ait aktif HubConnection'ları bul
                var activeConnections = await connectionRepo.FindAsync(
                    c => c.TcKimlikNo == tcKimlikNo &&
                         c.ConnectionStatus == ConnectionStatus.online &&
                         !c.SilindiMi);

                if (!activeConnections.Any())
                    return null;

                // Bu connection'lardan TV bağlantısı olanı bul
                var hubConnectionIds = activeConnections.Select(c => c.HubConnectionId).ToList();

                var tvConnection = await tvRepo.FirstOrDefaultAsync(
                    t => hubConnectionIds.Contains(t.HubConnectionId) && !t.SilindiMi,
                    t => t.HubConnection,
                    t => t.Tv);

                return tvConnection;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveTvByTcKimlikNoAsync hatası");
                return null;
            }
        }

        public async Task<User?> GetTvActiveUserAsync(int tvId)
        {
            try
            {
                var tvRepo = _unitOfWork.Repository<HubTvConnection>();
                var userRepo = _unitOfWork.Repository<User>();

                // TV'deki aktif bağlantıları bul
                var activeTvConnections = await tvRepo.FindAsync(
                    t => t.TvId == tvId && !t.SilindiMi,
                    t => t.HubConnection);

                if (!activeTvConnections.Any())
                    return null;

                // İlk aktif kullanıcıyı döndür (TV'de birden fazla kullanıcı olabilir)
                var firstConnection = activeTvConnections
                    .Where(t => t.HubConnection != null && t.HubConnection.ConnectionStatus == ConnectionStatus.online)
                    .FirstOrDefault();

                if (firstConnection?.HubConnection == null)
                    return null;

                var user = await userRepo.FirstOrDefaultAsync(
                    u => u.TcKimlikNo == firstConnection.HubConnection.TcKimlikNo,
                    u => u.Personel);

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTvActiveUserAsync hatası");
                return null;
            }
        }

        public async Task<bool> TransferTvConnectionAsync(string tcKimlikNo, string newConnectionId)
        {
            try
            {
                var connectionRepo = _unitOfWork.Repository<HubConnection>();
                var tvRepo = _unitOfWork.Repository<HubTvConnection>();

                var newConnection = await connectionRepo.FirstOrDefaultAsync(
                    c => c.ConnectionId == newConnectionId && !c.SilindiMi);

                if (newConnection == null)
                {
                    _logger.LogWarning("TransferTvConnectionAsync: Yeni connection bulunamadı ({ConnectionId})", newConnectionId);
                    return false;
                }

                // TcKimlikNo'ya ait aktif TV bağlantısını bul
                var tvConnection = await GetActiveTvByTcKimlikNoAsync(tcKimlikNo);

                if (tvConnection == null)
                {
                    _logger.LogWarning("TransferTvConnectionAsync: Aktif TV bağlantısı bulunamadı ({TcKimlikNo})", tcKimlikNo);
                    return false;
                }

                if (tvConnection.HubConnectionId == newConnection.HubConnectionId)
                {
                    return true; // Zaten aynı connection
                }

                // Eski connection'ı soft delete yap
                var oldConnection = await connectionRepo.FirstOrDefaultAsync(
                    c => c.HubConnectionId == tvConnection.HubConnectionId && !c.SilindiMi);

                if (oldConnection != null)
                {
                    oldConnection.ConnectionStatus = ConnectionStatus.offline;
                    oldConnection.LastActivityAt = DateTime.Now;
                    oldConnection.IslemZamani = DateTime.Now;
                    oldConnection.DuzenlenmeTarihi = DateTime.Now;
                    oldConnection.SilindiMi = true;
                    oldConnection.SilinmeTarihi = DateTime.Now;
                    oldConnection.SilenKullanici = "TvTransfer";
                    connectionRepo.Update(oldConnection);
                }

                // HubTvConnection'ı yeni HubConnectionId'ye bağla
                tvConnection.HubConnectionId = newConnection.HubConnectionId;
                tvConnection.IslemZamani = DateTime.Now;
                tvConnection.DuzenlenmeTarihi = DateTime.Now;
                tvRepo.Update(tvConnection);

                // Yeni connection'ı TvDisplay tipine çevir
                newConnection.ConnectionType = "TvDisplay";
                newConnection.ConnectionStatus = ConnectionStatus.online;
                newConnection.LastActivityAt = DateTime.Now;
                newConnection.IslemZamani = DateTime.Now;
                newConnection.DuzenlenmeTarihi = DateTime.Now;
                connectionRepo.Update(newConnection);

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("✅ TV bağlantısı devredildi: {TcKimlikNo} -> HubConnection#{HubConnectionId} | TV#{TvId}",
                    tcKimlikNo, newConnection.HubConnectionId, tvConnection.TvId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TransferTvConnectionAsync hatası");
                return false;
            }
        }

        public async Task<List<HubConnection>> GetNonTvConnectionsByTcKimlikNoAsync(string tcKimlikNo)
        {
            try
            {
                var repo = _unitOfWork.Repository<HubConnection>();

                // TcKimlikNo'ya ait tüm bağlantıları al (HubTvConnection navigation property ile)
                var allConnections = await repo.GetAllAsync(x => x.HubTvConnection);

                // TcKimlikNo'ya göre filtrele ve HubTvConnection olmayan bağlantıları al
                var nonTvConnections = allConnections
                    .Where(x => x.TcKimlikNo == tcKimlikNo &&
                               (x.HubTvConnection == null || x.HubTvConnection.SilindiMi))
                    .ToList();

                return nonTvConnections;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetNonTvConnectionsByTcKimlikNoAsync hatası");
                return new List<HubConnection>();
            }
        }

        // ═══════════════════════════════════════════════════════
        // CLEANUP METHODS (Background Service için API endpoint'ler)
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Uygulama başlangıcında tüm online connection'ları offline yapar
        /// Sunucu restart'larında eski kayıtlar temizlenir
        /// </summary>
        public async Task<int> CleanupAllOnStartupAsync()
        {
            try
            {
                var hubConnectionRepo = _unitOfWork.GetRepository<IHubConnectionRepository>();
                var onlineConnections = await hubConnectionRepo.GetActiveConnectionsAsync();

                var toCleanup = onlineConnections
                    .Where(c => c.ConnectionStatus == ConnectionStatus.online && !c.SilindiMi)
                    .ToList();

                if (!toCleanup.Any())
                {
                    _logger.LogInformation("Başlangıç temizliği: Temizlenecek online connection yok");
                    return 0;
                }

                foreach (var conn in toCleanup)
                {
                    conn.ConnectionStatus = ConnectionStatus.offline;
                    conn.DuzenlenmeTarihi = DateTime.Now;
                    ClearNavigationReferences(conn);
                    hubConnectionRepo.Update(conn);
                }

                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("✅ Başlangıç temizliği: {Count} connection offline yapıldı", toCleanup.Count);

                return toCleanup.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CleanupAllOnStartupAsync hatası");
                return 0;
            }
        }

        /// <summary>
        /// Stale connection'ları temizle (LastActivityAt + threshold geçmişse)
        /// </summary>
        /// <param name="staleThresholdMinutes">Stale kabul edilme süresi (dakika)</param>
        public async Task<int> CleanupStaleConnectionsAsync(int staleThresholdMinutes)
        {
            try
            {
                var hubConnectionRepo = _unitOfWork.GetRepository<IHubConnectionRepository>();
                var cutoffTime = DateTime.Now.AddMinutes(-staleThresholdMinutes);

                var onlineConnections = await hubConnectionRepo.GetActiveConnectionsAsync();
                var staleConnections = onlineConnections
                    .Where(c => c.ConnectionStatus == ConnectionStatus.online
                             && !c.SilindiMi
                             && c.LastActivityAt < cutoffTime)
                    .ToList();

                if (!staleConnections.Any())
                {
                    _logger.LogDebug("Stale connection temizliği: Temizlenecek kayıt yok");
                    return 0;
                }

                foreach (var conn in staleConnections)
                {
                    conn.ConnectionStatus = ConnectionStatus.offline;
                    conn.DuzenlenmeTarihi = DateTime.Now;
                    ClearNavigationReferences(conn);
                    hubConnectionRepo.Update(conn);
                }

                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("✅ Stale connection temizliği: {Count} connection offline yapıldı (Threshold: {Threshold} dakika)",
                    staleConnections.Count, staleThresholdMinutes);

                return staleConnections.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CleanupStaleConnectionsAsync hatası");
                return 0;
            }
        }

        /// <summary>
        /// Orphan HubBankoConnection kayıtlarını temizle
        /// HubConnection offline/silinmiş ama HubBankoConnection hala aktif olanları bulur ve temizler
        /// </summary>
        /// <returns>Temizlenen kayıt sayısı</returns>
        public async Task<int> CleanupOrphanBankoConnectionsAsync()
        {
            try
            {
                var bankoRepo = _unitOfWork.Repository<HubBankoConnection>();
                var hubRepo = _unitOfWork.Repository<HubConnection>();
                var userRepo = _unitOfWork.Repository<User>();

                // 1. Tüm aktif HubBankoConnection'ları al
                var activeBankoConnections = await bankoRepo.FindAsync(b => b.BankoModuAktif);

                if (!activeBankoConnections.Any())
                {
                    _logger.LogDebug("Orphan Banko temizliği: Aktif HubBankoConnection yok");
                    return 0;
                }

                var orphanCount = 0;

                // 2. Her birinin HubConnection'ını kontrol et
                foreach (var bankoConn in activeBankoConnections)
                {
                    var hubConnection = await hubRepo.FirstOrDefaultAsync(
                        h => h.HubConnectionId == bankoConn.HubConnectionId);

                    // 3. HubConnection yok, offline veya silinmiş ise → Orphan!
                    if (hubConnection == null ||
                        hubConnection.ConnectionStatus == ConnectionStatus.offline ||
                        hubConnection.SilindiMi)
                    {
                        // HubBankoConnection'ı deaktif et ve soft-delete yap
                        bankoConn.BankoModuAktif = false;
                        bankoConn.BankoModuBitis = DateTime.Now;
                        bankoConn.SilindiMi = true;
                        bankoConn.SilinmeTarihi = DateTime.Now;
                        bankoConn.SilenKullanici = "OrphanCleanup";
                        bankoConn.DuzenlenmeTarihi = DateTime.Now;
                        bankoRepo.Update(bankoConn);

                        // User tablosunu temizle
                        var user = await userRepo.FirstOrDefaultAsync(
                            u => u.TcKimlikNo == bankoConn.TcKimlikNo);

                        if (user != null)
                        {
                            user.BankoModuAktif = false;
                            user.AktifBankoId = null;
                            user.BankoModuBaslangic = null;
                            user.DuzenlenmeTarihi = DateTime.Now;
                            userRepo.Update(user);
                        }

                        orphanCount++;
                        _logger.LogInformation($"🧹 Orphan HubBankoConnection temizlendi: Banko#{bankoConn.BankoId} | {bankoConn.TcKimlikNo}");
                    }
                }

                if (orphanCount > 0)
                {
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation($"✅ Orphan Banko temizliği: {orphanCount} kayıt temizlendi");
                }

                return orphanCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CleanupOrphanBankoConnectionsAsync hatası");
                return 0;
            }
        }

        /// <summary>
        /// Orphan HubTvConnection kayıtlarını temizle
        /// HubConnection offline/silinmiş ama HubTvConnection hala aktif olanları bulur ve temizler
        /// </summary>
        /// <returns>Temizlenen kayıt sayısı</returns>
        public async Task<int> CleanupOrphanTvConnectionsAsync()
        {
            try
            {
                var tvRepo = _unitOfWork.Repository<HubTvConnection>();
                var hubRepo = _unitOfWork.Repository<HubConnection>();

                // 1. Tüm aktif HubTvConnection'ları al (SilindiMi = false)
                var activeTvConnections = await tvRepo.FindAsync(t => !t.SilindiMi);

                if (!activeTvConnections.Any())
                {
                    _logger.LogDebug("Orphan TV temizliği: Aktif HubTvConnection yok");
                    return 0;
                }

                var orphanCount = 0;

                // 2. Her birinin HubConnection'ını kontrol et
                foreach (var tvConn in activeTvConnections)
                {
                    var hubConnection = await hubRepo.FirstOrDefaultAsync(
                        h => h.HubConnectionId == tvConn.HubConnectionId);

                    // 3. HubConnection yok, offline veya silinmiş ise → Orphan!
                    if (hubConnection == null ||
                        hubConnection.ConnectionStatus == ConnectionStatus.offline ||
                        hubConnection.SilindiMi)
                    {
                        // HubTvConnection'ı soft-delete yap
                        tvConn.SilindiMi = true;
                        tvConn.SilinmeTarihi = DateTime.Now;
                        tvConn.SilenKullanici = "OrphanCleanup";
                        tvConn.DuzenlenmeTarihi = DateTime.Now;
                        tvRepo.Update(tvConn);

                        orphanCount++;
                        _logger.LogInformation($"🧹 Orphan HubTvConnection temizlendi: TV#{tvConn.TvId} | HubConnectionId={tvConn.HubConnectionId}");
                    }
                }

                if (orphanCount > 0)
                {
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation($"✅ Orphan TV temizliği: {orphanCount} kayıt temizlendi");
                }

                return orphanCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CleanupOrphanTvConnectionsAsync hatası");
                return 0;
            }
        }

        /// <summary>
        /// Entity navigation referanslarını temizle (EF tracking sorunlarını önlemek için)
        /// </summary>
        private static void ClearNavigationReferences(HubConnection connection)
        {
            connection.User = null;
            connection.HubBankoConnection = null;
            connection.HubTvConnection = null;
        }
    }
}

