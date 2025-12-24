using Microsoft.Extensions.Logging;
using SGKPortalApp.Common.Interfaces.Permission;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Complex;

namespace SGKPortalApp.BusinessLogicLayer.Services.Common
{
    public class ModulControllerService : IModulControllerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ModulControllerService> _logger;
        private readonly IYetkiQueryRepository _yetkiQueryRepository;
        private readonly IFieldPermissionValidationService _fieldPermissionService;
        private readonly IPermissionKeyResolverService _permissionKeyResolver;

        public ModulControllerService(
            IUnitOfWork unitOfWork,
            ILogger<ModulControllerService> logger,
            IYetkiQueryRepository yetkiQueryRepository,
            IFieldPermissionValidationService fieldPermissionService,
            IPermissionKeyResolverService permissionKeyResolver)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _yetkiQueryRepository = yetkiQueryRepository;
            _fieldPermissionService = fieldPermissionService;
            _permissionKeyResolver = permissionKeyResolver;
        }

        public async Task<ApiResponseDto<List<ModulControllerResponseDto>>> GetAllAsync()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IModulControllerRepository>();
                var controllers = await repo.GetAllWithIslemlerAsync();

                var dtos = controllers.Select(c => new ModulControllerResponseDto
                {
                    ModulControllerId = c.ModulControllerId,
                    ModulControllerAdi = c.ModulControllerAdi,
                    ModulId = c.ModulId,
                    ModulAdi = c.Modul?.ModulAdi ?? string.Empty,
                    UstModulControllerId = c.UstModulControllerId,
                    UstModulControllerAdi = c.UstModulController?.ModulControllerAdi,
                    EklenmeTarihi = c.EklenmeTarihi,
                    DuzenlenmeTarihi = c.DuzenlenmeTarihi,
                    IslemCount = c.ModulControllerIslemler?.Count ?? 0
                }).ToList();

                return ApiResponseDto<List<ModulControllerResponseDto>>.SuccessResult(dtos, "Controller listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller listesi getirilirken hata oluştu");
                return ApiResponseDto<List<ModulControllerResponseDto>>.ErrorResult("Controller listesi getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<ModulControllerResponseDto>>> GetByModulIdAsync(int modulId)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IModulControllerRepository>();
                var controllers = await repo.GetByModulAsync(modulId);

                var modulRepo = _unitOfWork.GetRepository<IModulRepository>();
                var modul = await modulRepo.GetByIdAsync(modulId);

                var dtos = controllers.Select(c => new ModulControllerResponseDto
                {
                    ModulControllerId = c.ModulControllerId,
                    ModulControllerAdi = c.ModulControllerAdi,
                    ModulId = c.ModulId,
                    ModulAdi = modul?.ModulAdi ?? string.Empty,
                    UstModulControllerId = c.UstModulControllerId,
                    UstModulControllerAdi = c.UstModulController?.ModulControllerAdi,
                    EklenmeTarihi = c.EklenmeTarihi,
                    DuzenlenmeTarihi = c.DuzenlenmeTarihi,
                    IslemCount = c.ModulControllerIslemler?.Count ?? 0
                }).ToList();

                return ApiResponseDto<List<ModulControllerResponseDto>>.SuccessResult(dtos, "Controller listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Modüle ait controller listesi getirilirken hata oluştu. ModulId: {ModulId}", modulId);
                return ApiResponseDto<List<ModulControllerResponseDto>>.ErrorResult("Controller listesi getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<ModulControllerResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IModulControllerRepository>();
                var controller = await repo.GetWithIslemlerAsync(id);

                if (controller == null)
                    return ApiResponseDto<ModulControllerResponseDto>.ErrorResult("Controller bulunamadı");

                var dto = new ModulControllerResponseDto
                {
                    ModulControllerId = controller.ModulControllerId,
                    ModulControllerAdi = controller.ModulControllerAdi,
                    ModulId = controller.ModulId,
                    ModulAdi = controller.Modul?.ModulAdi ?? string.Empty,
                    UstModulControllerId = controller.UstModulControllerId,
                    UstModulControllerAdi = controller.UstModulController?.ModulControllerAdi,
                    EklenmeTarihi = controller.EklenmeTarihi,
                    DuzenlenmeTarihi = controller.DuzenlenmeTarihi,
                    IslemCount = controller.ModulControllerIslemler?.Count ?? 0
                };

                return ApiResponseDto<ModulControllerResponseDto>.SuccessResult(dto, "Controller başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller getirilirken hata oluştu. Id: {Id}", id);
                return ApiResponseDto<ModulControllerResponseDto>.ErrorResult("Controller getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<ModulControllerResponseDto>> CreateAsync(ModulControllerCreateRequestDto request)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IModulControllerRepository>();
                var modulRepo = _unitOfWork.GetRepository<IModulRepository>();

                var modul = await modulRepo.GetByIdAsync(request.ModulId);
                if (modul == null)
                    return ApiResponseDto<ModulControllerResponseDto>.ErrorResult("Seçilen modül bulunamadı");

                var existing = await repo.GetByControllerAdiAsync(request.ModulControllerAdi);
                if (existing != null && existing.ModulId == request.ModulId)
                    return ApiResponseDto<ModulControllerResponseDto>.ErrorResult("Bu modülde aynı isimde controller zaten mevcut");

                var entity = new ModulController
                {
                    ModulControllerAdi = request.ModulControllerAdi,
                    ModulId = request.ModulId,
                    Modul = modul,
                    UstModulControllerId = request.UstModulControllerId
                };

                await repo.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                var dto = new ModulControllerResponseDto
                {
                    ModulControllerId = entity.ModulControllerId,
                    ModulControllerAdi = entity.ModulControllerAdi,
                    ModulId = entity.ModulId,
                    ModulAdi = modul.ModulAdi,
                    UstModulControllerId = entity.UstModulControllerId,
                    UstModulControllerAdi = entity.UstModulController?.ModulControllerAdi,
                    EklenmeTarihi = entity.EklenmeTarihi,
                    DuzenlenmeTarihi = entity.DuzenlenmeTarihi,
                    IslemCount = 0
                };

                return ApiResponseDto<ModulControllerResponseDto>.SuccessResult(dto, "Controller başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller oluşturulurken hata oluştu");
                return ApiResponseDto<ModulControllerResponseDto>.ErrorResult("Controller oluşturulurken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<ModulControllerResponseDto>> UpdateAsync(int id, ModulControllerUpdateRequestDto request)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IModulControllerRepository>();
                var modulRepo = _unitOfWork.GetRepository<IModulRepository>();

                var entity = await repo.GetByIdAsync(id);
                if (entity == null)
                    return ApiResponseDto<ModulControllerResponseDto>.ErrorResult("Controller bulunamadı");

                var modul = await modulRepo.GetByIdAsync(request.ModulId);
                if (modul == null)
                    return ApiResponseDto<ModulControllerResponseDto>.ErrorResult("Seçilen modül bulunamadı");

                // ⭐ Field-level permission enforcement
                // Permission key otomatik çözümleme (route → permission key)
                var permissionKey = _permissionKeyResolver.ResolveFromCurrentRequest() ?? "UNKNOWN";
                var userPermissions = new Dictionary<string, BusinessObjectLayer.Enums.Common.YetkiSeviyesi>();
                var originalDto = new ModulControllerUpdateRequestDto
                {
                    ModulControllerId = entity.ModulControllerId,
                    ModulControllerAdi = entity.ModulControllerAdi,
                    ModulId = entity.ModulId
                };

                var unauthorizedFields = await _fieldPermissionService.ValidateFieldPermissionsAsync(
                    request,
                    userPermissions,
                    originalDto,
                    permissionKey,
                    null);

                if (unauthorizedFields.Any())
                {
                    _fieldPermissionService.RevertUnauthorizedFields(request, originalDto, unauthorizedFields);
                    _logger.LogWarning(
                        "ModulControllerService.UpdateAsync - Field-level permission enforcement: {Count} alan revert edildi.",
                        unauthorizedFields.Count);
                }

                entity.ModulControllerAdi = request.ModulControllerAdi;
                entity.ModulId = request.ModulId;
                entity.UstModulControllerId = request.UstModulControllerId;
                entity.DuzenlenmeTarihi = DateTime.Now;

                repo.Update(entity);
                await _unitOfWork.SaveChangesAsync();

                var dto = new ModulControllerResponseDto
                {
                    ModulControllerId = entity.ModulControllerId,
                    ModulControllerAdi = entity.ModulControllerAdi,
                    ModulId = entity.ModulId,
                    ModulAdi = modul.ModulAdi,
                    UstModulControllerId = entity.UstModulControllerId,
                    UstModulControllerAdi = entity.UstModulController?.ModulControllerAdi,
                    EklenmeTarihi = entity.EklenmeTarihi,
                    DuzenlenmeTarihi = entity.DuzenlenmeTarihi
                };

                return ApiResponseDto<ModulControllerResponseDto>.SuccessResult(dto, "Controller başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller güncellenirken hata oluştu. Id: {Id}", id);
                return ApiResponseDto<ModulControllerResponseDto>.ErrorResult("Controller güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IModulControllerRepository>();
                var entity = await repo.GetWithIslemlerAsync(id);

                if (entity == null)
                    return ApiResponseDto<bool>.ErrorResult("Controller bulunamadı");

                if (entity.ModulControllerIslemler?.Any() == true)
                    return ApiResponseDto<bool>.ErrorResult($"Bu controller'a bağlı {entity.ModulControllerIslemler.Count} işlem bulunmaktadır. Önce işlemleri silmelisiniz.");

                repo.Delete(entity);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponseDto<bool>.SuccessResult(true, "Controller başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller silinirken hata oluştu. Id: {Id}", id);
                return ApiResponseDto<bool>.ErrorResult("Controller silinirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<DropdownItemDto>>> GetDropdownAsync()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IModulControllerRepository>();
                var controllers = await repo.GetAllAsync();

                var dropdown = controllers.Select(c => new DropdownItemDto
                {
                    Id = c.ModulControllerId,
                    Ad = $"{c.Modul?.ModulAdi ?? "?"} / {c.ModulControllerAdi}"
                }).ToList();

                return ApiResponseDto<List<DropdownItemDto>>.SuccessResult(dropdown, "Dropdown listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller dropdown listesi getirilirken hata oluştu");
                return ApiResponseDto<List<DropdownItemDto>>.ErrorResult("Dropdown listesi getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<DropdownItemDto>>> GetDropdownByModulIdAsync(int modulId)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IModulControllerRepository>();
                var controllers = await repo.GetByModulAsync(modulId);

                // Recursive query ile tüm controller'ların FullPath bilgisini al
                var hierarchyData = await _yetkiQueryRepository.GetModulControllersWithHierarchyAsync(modulId);
                var fullPathDict = hierarchyData.ToDictionary(h => h.ModulControllerId, h => h.FullPath);

                var dropdown = controllers.Select(c => new DropdownItemDto
                {
                    Id = c.ModulControllerId,
                    // Hiyerarşik görünüm: "Parent > Child" formatında
                    Ad = c.UstModulController != null
                        ? $"{c.UstModulController.ModulControllerAdi} > {c.ModulControllerAdi}"
                        : c.ModulControllerAdi,
                    // Metadata olarak FullPath bilgisini tut (route önerisi için)
                    // Örnek: "/Siramatik/Banko"
                    Metadata = fullPathDict.ContainsKey(c.ModulControllerId) 
                        ? fullPathDict[c.ModulControllerId] 
                        : null
                }).ToList();

                return ApiResponseDto<List<DropdownItemDto>>.SuccessResult(dropdown, "Dropdown listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Modüle ait controller dropdown listesi getirilirken hata oluştu. ModulId: {ModulId}", modulId);
                return ApiResponseDto<List<DropdownItemDto>>.ErrorResult("Dropdown listesi getirilirken bir hata oluştu", ex.Message);
            }
        }
    }
}
