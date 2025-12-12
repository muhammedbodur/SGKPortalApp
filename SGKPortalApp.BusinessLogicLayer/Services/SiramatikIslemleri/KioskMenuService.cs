using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Services.SiramatikIslemleri
{
    public class KioskMenuService : IKioskMenuService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<KioskMenuService> _logger;
        private readonly ICascadeHelper _cascadeHelper;

        public KioskMenuService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<KioskMenuService> logger,
            ICascadeHelper cascadeHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cascadeHelper = cascadeHelper;
        }

        public async Task<ApiResponseDto<List<KioskMenuResponseDto>>> GetAllAsync()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IKioskMenuRepository>();
                var menus = await repo.GetAllAsync();
                var dto = _mapper.Map<List<KioskMenuResponseDto>>(menus);
                return ApiResponseDto<List<KioskMenuResponseDto>>.SuccessResult(dto, "Kiosk menüleri getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kiosk menüleri listelenirken hata");
                return ApiResponseDto<List<KioskMenuResponseDto>>.ErrorResult("Kiosk menüleri listelenirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KioskMenuResponseDto>> GetByIdAsync(int kioskMenuId)
        {
            if (kioskMenuId <= 0)
            {
                return ApiResponseDto<KioskMenuResponseDto>.ErrorResult("Geçersiz kiosk menü ID");
            }

            try
            {
                var repo = _unitOfWork.GetRepository<IKioskMenuRepository>();
                var menu = await repo.GetWithKiosksAsync(kioskMenuId);
                if (menu == null)
                {
                    return ApiResponseDto<KioskMenuResponseDto>.ErrorResult("Kiosk menü bulunamadı");
                }

                var dto = _mapper.Map<KioskMenuResponseDto>(menu);
                return ApiResponseDto<KioskMenuResponseDto>.SuccessResult(dto, "Kiosk menü getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kiosk menü getirilemedi. ID: {KioskMenuId}", kioskMenuId);
                return ApiResponseDto<KioskMenuResponseDto>.ErrorResult("Kiosk menü getirilemedi", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KioskMenuResponseDto>> CreateAsync(KioskMenuCreateRequestDto request)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    var repo = _unitOfWork.GetRepository<IKioskMenuRepository>();
                    var exists = await repo.ExistsByNameAsync(request.MenuAdi);
                    if (exists)
                    {
                        return ApiResponseDto<KioskMenuResponseDto>.ErrorResult("Bu menü adı zaten kullanılıyor");
                    }

                    // Eğer sıra 0 veya belirtilmemişse, otomatik sıra ata
                    if (request.MenuSira <= 0)
                    {
                        var maxSira = await repo.GetMaxSiraAsync();
                        request.MenuSira = maxSira + 1;
                    }

                    var entity = _mapper.Map<KioskMenu>(request);
                    entity.Aktiflik = request.Aktiflik;

                    await repo.AddAsync(entity);
                    await _unitOfWork.SaveChangesAsync();

                    var dto = _mapper.Map<KioskMenuResponseDto>(entity);
                    return ApiResponseDto<KioskMenuResponseDto>.SuccessResult(dto, "Kiosk menü oluşturuldu");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Kiosk menü oluşturulurken hata");
                    return ApiResponseDto<KioskMenuResponseDto>.ErrorResult("Kiosk menü oluşturulamadı", ex.Message);
                }
            });
        }

        public async Task<ApiResponseDto<KioskMenuResponseDto>> UpdateAsync(KioskMenuUpdateRequestDto request)
        {
            if (request.KioskMenuId <= 0)
            {
                return ApiResponseDto<KioskMenuResponseDto>.ErrorResult("Geçersiz kiosk menü ID");
            }

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    var repo = _unitOfWork.GetRepository<IKioskMenuRepository>();
                    var entity = await repo.GetByIdAsync(request.KioskMenuId);
                    if (entity == null)
                    {
                        return ApiResponseDto<KioskMenuResponseDto>.ErrorResult("Kiosk menü bulunamadı");
                    }

                    if (!string.Equals(entity.MenuAdi, request.MenuAdi, StringComparison.OrdinalIgnoreCase))
                    {
                        var exists = await repo.ExistsByNameAsync(request.MenuAdi);
                        if (exists)
                        {
                            return ApiResponseDto<KioskMenuResponseDto>.ErrorResult("Bu menü adı zaten kullanılıyor");
                        }
                    }

                    var oldAktiflik = entity.Aktiflik;

                    _mapper.Map(request, entity);
                    entity.DuzenlenmeTarihi = DateTime.Now;

                    repo.Update(entity);

                    // Cascade: Aktiflik değişmişse child kayıtları da güncelle
                    if (oldAktiflik != request.Aktiflik && request.Aktiflik == Aktiflik.Pasif)
                    {
                        await CascadeAktiflikUpdateAsync(request.KioskMenuId);
                    }

                    await _unitOfWork.SaveChangesAsync();

                    var dto = _mapper.Map<KioskMenuResponseDto>(entity);
                    return ApiResponseDto<KioskMenuResponseDto>.SuccessResult(dto, "Kiosk menü güncellendi");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Kiosk menü güncellenirken hata. ID: {KioskMenuId}", request.KioskMenuId);
                    return ApiResponseDto<KioskMenuResponseDto>.ErrorResult("Kiosk menü güncellenemedi", ex.Message);
                }
            });
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int kioskMenuId)
        {
            if (kioskMenuId <= 0)
            {
                return ApiResponseDto<bool>.ErrorResult("Geçersiz kiosk menü ID");
            }

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    var repo = _unitOfWork.GetRepository<IKioskMenuRepository>();
                    var entity = await repo.GetWithKiosksAsync(kioskMenuId);
                    if (entity == null)
                    {
                        return ApiResponseDto<bool>.ErrorResult("Kiosk menü bulunamadı");
                    }

                    // Cascade: Child kayıtları da sil (soft delete)
                    await CascadeDeleteAsync(kioskMenuId);

                    entity.SilindiMi = true;
                    entity.Aktiflik = Aktiflik.Pasif;
                    entity.DuzenlenmeTarihi = DateTime.Now;

                    repo.Update(entity);
                    await _unitOfWork.SaveChangesAsync();

                    return ApiResponseDto<bool>.SuccessResult(true, "Kiosk menü silindi");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Kiosk menü silinirken hata. ID: {KioskMenuId}", kioskMenuId);
                    return ApiResponseDto<bool>.ErrorResult("Kiosk menü silinemedi", ex.Message);
                }
            });
        }

        public async Task<ApiResponseDto<List<KioskMenuResponseDto>>> GetActiveAsync()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IKioskMenuRepository>();
                var menus = await repo.GetActiveAsync();
                var dto = _mapper.Map<List<KioskMenuResponseDto>>(menus);
                return ApiResponseDto<List<KioskMenuResponseDto>>.SuccessResult(dto, "Aktif kiosk menüler getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif kiosk menüler getirilemedi");
                return ApiResponseDto<List<KioskMenuResponseDto>>.ErrorResult("Aktif kiosk menüler getirilemedi", ex.Message);
            }
        }

        /// <summary>
        /// Cascade delete: KioskMenu silindiğinde child kayıtları da siler
        /// CascadeHelper kullanarak tracking conflict'leri otomatik handle eder
        /// </summary>
        private async Task CascadeDeleteAsync(int kioskMenuId)
        {
            await _cascadeHelper.CascadeSoftDeleteAsync<KioskMenuAtama>(x => x.KioskMenuId == kioskMenuId);
            await _cascadeHelper.CascadeSoftDeleteAsync<KioskMenuIslem>(x => x.KioskMenuId == kioskMenuId);
            _logger.LogInformation("Cascade delete: KioskMenuId={KioskMenuId}", kioskMenuId);
        }

        /// <summary>
        /// Cascade update: KioskMenu pasif yapıldığında child kayıtları da pasif yap
        /// CascadeHelper kullanarak tracking conflict'leri otomatik handle eder
        /// </summary>
        private async Task CascadeAktiflikUpdateAsync(int kioskMenuId)
        {
            await _cascadeHelper.CascadeAktiflikUpdateAsync<KioskMenuAtama>(x => x.KioskMenuId == kioskMenuId, Aktiflik.Pasif);
            await _cascadeHelper.CascadeAktiflikUpdateAsync<KioskMenuIslem>(x => x.KioskMenuId == kioskMenuId, Aktiflik.Pasif);
            _logger.LogInformation("Cascade pasif: KioskMenuId={KioskMenuId}", kioskMenuId);
        }
    }
}
