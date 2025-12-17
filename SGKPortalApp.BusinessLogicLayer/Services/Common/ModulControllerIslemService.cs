using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Auth;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Interfaces.SignalR;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;

namespace SGKPortalApp.BusinessLogicLayer.Services.Common
{
    public class ModulControllerIslemService : IModulControllerIslemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ModulControllerIslemService> _logger;
        private readonly ISignalRBroadcaster _broadcaster;
        private readonly IHubConnectionRepository _hubConnectionRepository;

        public ModulControllerIslemService(
            IUnitOfWork unitOfWork,
            ILogger<ModulControllerIslemService> logger,
            ISignalRBroadcaster broadcaster,
            IHubConnectionRepository hubConnectionRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _broadcaster = broadcaster;
            _hubConnectionRepository = hubConnectionRepository;
        }

        /// <summary>
        /// Kullanıcının PermissionStamp'ini günceller (yetki tanımı değişikliği için)
        /// PersonelYetkiService ile aynı pattern
        /// </summary>
        private async Task<Guid?> TouchPermissionStampAsync(string tcKimlikNo)
        {
            try
            {
                var userRepo = _unitOfWork.Repository<User>();
                var user = await userRepo.FirstOrDefaultAsync(u => u.TcKimlikNo == tcKimlikNo);
                if (user == null)
                {
                    _logger.LogWarning("PermissionStamp güncellenecek User bulunamadı. TcKimlikNo: {TcKimlikNo}", tcKimlikNo);
                    return null;
                }

                user.PermissionStamp = Guid.NewGuid();
                user.DuzenlenmeTarihi = DateTime.Now;
                userRepo.Update(user);
                await _unitOfWork.SaveChangesAsync();
                return user.PermissionStamp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PermissionStamp güncellenirken hata oluştu. TcKimlikNo: {TcKimlikNo}", tcKimlikNo);
                return null;
            }
        }

        /// <summary>
        /// Belirtilen kullanıcıya yetki değişikliğini broadcast eder
        /// PersonelYetkiService ile aynı pattern
        /// </summary>
        private async Task BroadcastPermissionsChangedAsync(string tcKimlikNo, Guid? permissionStamp)
        {
            try
            {
                var connections = await _hubConnectionRepository.GetByUserAsync(tcKimlikNo);
                var connectionIds = connections
                    .Where(c => c.ConnectionStatus == ConnectionStatus.online && !c.SilindiMi)
                    .Select(c => c.ConnectionId)
                    .Distinct()
                    .ToList();

                if (!connectionIds.Any())
                    return;

                await _broadcaster.SendToConnectionsAsync(
                    connectionIds,
                    "permissionsChanged",
                    new
                    {
                        TcKimlikNo = tcKimlikNo,
                        PermissionStamp = permissionStamp,
                        Timestamp = DateTime.Now
                    });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "permissionsChanged broadcast hatası. TcKimlikNo: {TcKimlikNo}", tcKimlikNo);
            }
        }

        /// <summary>
        /// Tüm aktif kullanıcılara yetki tanımlarının değiştiğini bildirir.
        /// Her kullanıcının PermissionStamp'i güncellenir ve permissionsChanged event'i gönderilir (claims güncellemesi için)
        /// </summary>
        private async Task BroadcastPermissionDefinitionsChangedAsync()
        {
            try
            {
                var connections = await _hubConnectionRepository.GetActiveConnectionsAsync();

                // Sadece online ve silinmemiş connection'ları al
                var userConnections = connections
                    .Where(c => c.ConnectionStatus == ConnectionStatus.online && !c.SilindiMi)
                    .GroupBy(c => c.TcKimlikNo)
                    .Select(g => g.Key)
                    .Distinct()
                    .ToList();

                if (!userConnections.Any())
                {
                    _logger.LogDebug("BroadcastPermissionDefinitionsChangedAsync: Aktif connection bulunamadı");
                    return;
                }

                // Her kullanıcı için stamp güncelle ve broadcast yap
                // PersonelYetkiService ile aynı pattern kullanılıyor
                foreach (var tcKimlikNo in userConnections)
                {
                    var stamp = await TouchPermissionStampAsync(tcKimlikNo);
                    await BroadcastPermissionsChangedAsync(tcKimlikNo, stamp);
                }

                _logger.LogInformation("permissionsChanged broadcast edildi (definition change). Kullanıcı sayısı: {Count}", userConnections.Count);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "permissionsChanged broadcast hatası (definition change)");
            }
        }

        public async Task<ApiResponseDto<List<ModulControllerIslemResponseDto>>> GetAllAsync()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IModulControllerIslemRepository>();
                var islemler = await repo.GetAllWithControllerAsync();

                var dtos = islemler.Select(i => new ModulControllerIslemResponseDto
                {
                    ModulControllerIslemId = i.ModulControllerIslemId,
                    ModulControllerIslemAdi = i.ModulControllerIslemAdi,
                    IslemTipi = i.IslemTipi,
                    Route = i.Route,
                    PermissionKey = i.PermissionKey,
                    MinYetkiSeviyesi = i.MinYetkiSeviyesi,
                    SayfaTipi = i.SayfaTipi,
                    UstIslemId = i.UstIslemId,
                    UstIslemAdi = i.UstIslem?.ModulControllerIslemAdi,
                    Aciklama = i.Aciklama,
                    DtoTypeName = i.DtoTypeName,
                    DtoFieldName = i.DtoFieldName,
                    ModulControllerId = i.ModulControllerId,
                    ModulControllerAdi = i.ModulController?.ModulControllerAdi ?? string.Empty,
                    ModulId = i.ModulController?.ModulId ?? 0,
                    ModulAdi = i.ModulController?.Modul?.ModulAdi ?? string.Empty,
                    EklenmeTarihi = i.EklenmeTarihi,
                    DuzenlenmeTarihi = i.DuzenlenmeTarihi
                }).ToList();

                return ApiResponseDto<List<ModulControllerIslemResponseDto>>.SuccessResult(dtos, "İşlem listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İşlem listesi getirilirken hata oluştu");
                return ApiResponseDto<List<ModulControllerIslemResponseDto>>.ErrorResult("İşlem listesi getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<ModulControllerIslemResponseDto>>> GetByControllerIdAsync(int controllerId)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IModulControllerIslemRepository>();
                var islemler = await repo.GetByControllerAsync(controllerId);

                var controllerRepo = _unitOfWork.GetRepository<IModulControllerRepository>();
                var controller = await controllerRepo.GetWithIslemlerAsync(controllerId);

                var dtos = islemler.Select(i => new ModulControllerIslemResponseDto
                {
                    ModulControllerIslemId = i.ModulControllerIslemId,
                    ModulControllerIslemAdi = i.ModulControllerIslemAdi,
                    IslemTipi = i.IslemTipi,
                    Route = i.Route,
                    PermissionKey = i.PermissionKey,
                    MinYetkiSeviyesi = i.MinYetkiSeviyesi,
                    SayfaTipi = i.SayfaTipi,
                    UstIslemId = i.UstIslemId,
                    UstIslemAdi = i.UstIslem?.ModulControllerIslemAdi,
                    Aciklama = i.Aciklama,
                    DtoTypeName = i.DtoTypeName,
                    DtoFieldName = i.DtoFieldName,
                    ModulControllerId = i.ModulControllerId,
                    ModulControllerAdi = controller?.ModulControllerAdi ?? string.Empty,
                    ModulId = controller?.ModulId ?? 0,
                    ModulAdi = controller?.Modul?.ModulAdi ?? string.Empty,
                    EklenmeTarihi = i.EklenmeTarihi,
                    DuzenlenmeTarihi = i.DuzenlenmeTarihi
                }).ToList();

                return ApiResponseDto<List<ModulControllerIslemResponseDto>>.SuccessResult(dtos, "İşlem listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller'a ait işlem listesi getirilirken hata oluştu. ControllerId: {ControllerId}", controllerId);
                return ApiResponseDto<List<ModulControllerIslemResponseDto>>.ErrorResult("İşlem listesi getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<ModulControllerIslemResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IModulControllerIslemRepository>();
                var islem = await repo.GetWithControllerAsync(id);

                if (islem == null)
                    return ApiResponseDto<ModulControllerIslemResponseDto>.ErrorResult("İşlem bulunamadı");

                var dto = new ModulControllerIslemResponseDto
                {
                    ModulControllerIslemId = islem.ModulControllerIslemId,
                    ModulControllerIslemAdi = islem.ModulControllerIslemAdi,
                    IslemTipi = islem.IslemTipi,
                    Route = islem.Route,
                    PermissionKey = islem.PermissionKey,
                    MinYetkiSeviyesi = islem.MinYetkiSeviyesi,
                    SayfaTipi = islem.SayfaTipi,
                    UstIslemId = islem.UstIslemId,
                    UstIslemAdi = islem.UstIslem?.ModulControllerIslemAdi,
                    Aciklama = islem.Aciklama,
                    DtoTypeName = islem.DtoTypeName,
                    DtoFieldName = islem.DtoFieldName,
                    ModulControllerId = islem.ModulControllerId,
                    ModulControllerAdi = islem.ModulController?.ModulControllerAdi ?? string.Empty,
                    ModulId = islem.ModulController?.ModulId ?? 0,
                    ModulAdi = islem.ModulController?.Modul?.ModulAdi ?? string.Empty,
                    EklenmeTarihi = islem.EklenmeTarihi,
                    DuzenlenmeTarihi = islem.DuzenlenmeTarihi
                };

                return ApiResponseDto<ModulControllerIslemResponseDto>.SuccessResult(dto, "İşlem başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İşlem getirilirken hata oluştu. Id: {Id}", id);
                return ApiResponseDto<ModulControllerIslemResponseDto>.ErrorResult("İşlem getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<ModulControllerIslemResponseDto>> CreateAsync(ModulControllerIslemCreateRequestDto request)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IModulControllerIslemRepository>();
                var controllerRepo = _unitOfWork.GetRepository<IModulControllerRepository>();

                var controller = await controllerRepo.GetWithIslemlerAsync(request.ModulControllerId);
                if (controller == null)
                    return ApiResponseDto<ModulControllerIslemResponseDto>.ErrorResult("Seçilen controller bulunamadı");

                var existing = await repo.GetByIslemAdiAsync(request.ModulControllerIslemAdi);
                if (existing != null && existing.ModulControllerId == request.ModulControllerId)
                    return ApiResponseDto<ModulControllerIslemResponseDto>.ErrorResult("Bu controller'da aynı isimde işlem zaten mevcut");

                var entity = new ModulControllerIslem
                {
                    ModulControllerIslemAdi = request.ModulControllerIslemAdi,
                    ModulControllerId = request.ModulControllerId,
                    IslemTipi = request.IslemTipi,
                    Route = request.Route,
                    PermissionKey = request.PermissionKey,
                    MinYetkiSeviyesi = request.MinYetkiSeviyesi,
                    SayfaTipi = request.SayfaTipi,
                    UstIslemId = request.UstIslemId,
                    Aciklama = request.Aciklama,
                    DtoTypeName = request.DtoTypeName,
                    DtoFieldName = request.DtoFieldName
                };

                await repo.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                // Tüm aktif kullanıcılara yetki tanımlarının değiştiğini bildir
                await BroadcastPermissionDefinitionsChangedAsync();

                var dto = new ModulControllerIslemResponseDto
                {
                    ModulControllerIslemId = entity.ModulControllerIslemId,
                    ModulControllerIslemAdi = entity.ModulControllerIslemAdi,
                    IslemTipi = entity.IslemTipi,
                    Route = entity.Route,
                    PermissionKey = entity.PermissionKey,
                    MinYetkiSeviyesi = entity.MinYetkiSeviyesi,
                    SayfaTipi = entity.SayfaTipi,
                    UstIslemId = entity.UstIslemId,
                    Aciklama = entity.Aciklama,
                    DtoTypeName = entity.DtoTypeName,
                    DtoFieldName = entity.DtoFieldName,
                    ModulControllerId = entity.ModulControllerId,
                    ModulControllerAdi = controller.ModulControllerAdi,
                    ModulId = controller.ModulId,
                    ModulAdi = controller.Modul?.ModulAdi ?? string.Empty,
                    EklenmeTarihi = entity.EklenmeTarihi,
                    DuzenlenmeTarihi = entity.DuzenlenmeTarihi
                };

                return ApiResponseDto<ModulControllerIslemResponseDto>.SuccessResult(dto, "İşlem başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İşlem oluşturulurken hata oluştu");
                return ApiResponseDto<ModulControllerIslemResponseDto>.ErrorResult("İşlem oluşturulurken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<ModulControllerIslemResponseDto>> UpdateAsync(int id, ModulControllerIslemUpdateRequestDto request)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IModulControllerIslemRepository>();
                var controllerRepo = _unitOfWork.GetRepository<IModulControllerRepository>();

                var entity = await repo.GetByIdAsync(id);
                if (entity == null)
                    return ApiResponseDto<ModulControllerIslemResponseDto>.ErrorResult("İşlem bulunamadı");

                var controller = await controllerRepo.GetWithIslemlerAsync(request.ModulControllerId);
                if (controller == null)
                    return ApiResponseDto<ModulControllerIslemResponseDto>.ErrorResult("Seçilen controller bulunamadı");

                entity.ModulControllerIslemAdi = request.ModulControllerIslemAdi;
                entity.ModulControllerId = request.ModulControllerId;
                entity.IslemTipi = request.IslemTipi;
                entity.Route = request.Route;
                entity.PermissionKey = request.PermissionKey;
                entity.MinYetkiSeviyesi = request.MinYetkiSeviyesi;
                entity.SayfaTipi = request.SayfaTipi;
                entity.UstIslemId = request.UstIslemId;
                entity.Aciklama = request.Aciklama;
                entity.DtoTypeName = request.DtoTypeName;
                entity.DtoFieldName = request.DtoFieldName;
                entity.DuzenlenmeTarihi = DateTime.Now;

                repo.Update(entity);
                await _unitOfWork.SaveChangesAsync();

                // Tüm aktif kullanıcılara yetki tanımlarının değiştiğini bildir
                await BroadcastPermissionDefinitionsChangedAsync();

                var dto = new ModulControllerIslemResponseDto
                {
                    ModulControllerIslemId = entity.ModulControllerIslemId,
                    ModulControllerIslemAdi = entity.ModulControllerIslemAdi,
                    IslemTipi = entity.IslemTipi,
                    Route = entity.Route,
                    PermissionKey = entity.PermissionKey,
                    MinYetkiSeviyesi = entity.MinYetkiSeviyesi,
                    SayfaTipi = entity.SayfaTipi,
                    UstIslemId = entity.UstIslemId,
                    Aciklama = entity.Aciklama,
                    DtoTypeName = entity.DtoTypeName,
                    DtoFieldName = entity.DtoFieldName,
                    ModulControllerId = entity.ModulControllerId,
                    ModulControllerAdi = controller.ModulControllerAdi,
                    ModulId = controller.ModulId,
                    ModulAdi = controller.Modul?.ModulAdi ?? string.Empty,
                    EklenmeTarihi = entity.EklenmeTarihi,
                    DuzenlenmeTarihi = entity.DuzenlenmeTarihi
                };

                return ApiResponseDto<ModulControllerIslemResponseDto>.SuccessResult(dto, "İşlem başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İşlem güncellenirken hata oluştu. Id: {Id}", id);
                return ApiResponseDto<ModulControllerIslemResponseDto>.ErrorResult("İşlem güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IModulControllerIslemRepository>();
                var entity = await repo.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponseDto<bool>.ErrorResult("İşlem bulunamadı");

                repo.Delete(entity);
                await _unitOfWork.SaveChangesAsync();

                // Tüm aktif kullanıcılara yetki tanımlarının değiştiğini bildir
                await BroadcastPermissionDefinitionsChangedAsync();

                return ApiResponseDto<bool>.SuccessResult(true, "İşlem başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İşlem silinirken hata oluştu. Id: {Id}", id);
                return ApiResponseDto<bool>.ErrorResult("İşlem silinirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<DropdownItemDto>>> GetDropdownAsync()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IModulControllerIslemRepository>();
                var dropdown = await repo.GetDropdownAsync();

                return ApiResponseDto<List<DropdownItemDto>>
                    .SuccessResult(dropdown.ToList(), "Dropdown listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ModulControllerIslem dropdown listesi getirilirken hata oluştu");
                return ApiResponseDto<List<DropdownItemDto>>
                    .ErrorResult("Dropdown listesi getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<DropdownItemDto>>> GetDropdownByControllerIdAsync(int controllerId)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IModulControllerIslemRepository>();
                var dropdown = await repo.GetByControllerDropdownAsync(controllerId);

                return ApiResponseDto<List<DropdownItemDto>>
                    .SuccessResult(dropdown.ToList(), "Dropdown listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller'a ait işlem dropdown listesi getirilirken hata oluştu. ControllerId: {ControllerId}", controllerId);
                return ApiResponseDto<List<DropdownItemDto>>
                    .ErrorResult("Dropdown listesi getirilirken bir hata oluştu", ex.Message);
            }
        }
    }
}
