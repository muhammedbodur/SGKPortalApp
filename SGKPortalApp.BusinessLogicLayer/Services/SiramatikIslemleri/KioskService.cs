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
    public class KioskService : IKioskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<KioskService> _logger;
        private readonly ICascadeHelper _cascadeHelper;

        public KioskService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<KioskService> logger,
            ICascadeHelper cascadeHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cascadeHelper = cascadeHelper;
        }

        public async Task<ApiResponseDto<List<KioskResponseDto>>> GetAllAsync()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IKioskRepository>();
                var list = await repo.GetAllAsync();
                var dto = _mapper.Map<List<KioskResponseDto>>(list);
                return ApiResponseDto<List<KioskResponseDto>>.SuccessResult(dto, "Kiosk listesi getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kiosk listesi getirilemedi");
                return ApiResponseDto<List<KioskResponseDto>>.ErrorResult("Kiosk listesi getirilemedi", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KioskResponseDto>> GetByIdAsync(int kioskId)
        {
            if (kioskId <= 0)
            {
                return ApiResponseDto<KioskResponseDto>.ErrorResult("Geçersiz kiosk ID");
            }

            try
            {
                var repo = _unitOfWork.GetRepository<IKioskRepository>();
                var entity = await repo.GetByIdAsync(kioskId);
                if (entity == null)
                {
                    return ApiResponseDto<KioskResponseDto>.ErrorResult("Kiosk bulunamadı");
                }

                var dto = _mapper.Map<KioskResponseDto>(entity);
                return ApiResponseDto<KioskResponseDto>.SuccessResult(dto, "Kiosk getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kiosk getirilemedi. ID: {KioskId}", kioskId);
                return ApiResponseDto<KioskResponseDto>.ErrorResult("Kiosk getirilemedi", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KioskResponseDto>> CreateAsync(KioskCreateRequestDto request)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    var repo = _unitOfWork.GetRepository<IKioskRepository>();
                    var exists = await repo.ExistsByNameAsync(request.KioskAdi, request.HizmetBinasiId);
                    if (exists)
                    {
                        return ApiResponseDto<KioskResponseDto>.ErrorResult("Bu binada aynı isimde kiosk mevcut");
                    }

                    var entity = _mapper.Map<BusinessObjectLayer.Entities.SiramatikIslemleri.Kiosk>(request);
                    entity.Aktiflik = request.Aktiflik;

                    await repo.AddAsync(entity);
                    await _unitOfWork.SaveChangesAsync();

                    var dto = _mapper.Map<KioskResponseDto>(entity);
                    return ApiResponseDto<KioskResponseDto>.SuccessResult(dto, "Kiosk oluşturuldu");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Kiosk oluşturulurken hata");
                    return ApiResponseDto<KioskResponseDto>.ErrorResult("Kiosk oluşturulamadı", ex.Message);
                }
            });
        }

        public async Task<ApiResponseDto<KioskResponseDto>> UpdateAsync(KioskUpdateRequestDto request)
        {
            if (request.KioskId <= 0)
            {
                return ApiResponseDto<KioskResponseDto>.ErrorResult("Geçersiz kiosk ID");
            }

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    var repo = _unitOfWork.GetRepository<IKioskRepository>();
                    var entity = await repo.GetByIdAsync(request.KioskId);
                    if (entity == null)
                    {
                        return ApiResponseDto<KioskResponseDto>.ErrorResult("Kiosk bulunamadı");
                    }

                    if (!string.Equals(entity.KioskAdi, request.KioskAdi, StringComparison.OrdinalIgnoreCase) || entity.HizmetBinasiId != request.HizmetBinasiId)
                    {
                        var exists = await repo.ExistsByNameAsync(request.KioskAdi, request.HizmetBinasiId);
                        if (exists)
                        {
                            return ApiResponseDto<KioskResponseDto>.ErrorResult("Bu binada aynı isimde kiosk mevcut");
                        }
                    }

                    var oldAktiflik = entity.Aktiflik;

                    _mapper.Map(request, entity);
                    entity.DuzenlenmeTarihi = DateTime.Now;

                    repo.Update(entity);

                    // Cascade: Aktiflik değişmişse child kayıtları da güncelle
                    if (oldAktiflik != request.Aktiflik && request.Aktiflik == Aktiflik.Pasif)
                    {
                        await CascadeAktiflikUpdateAsync(request.KioskId);
                    }

                    await _unitOfWork.SaveChangesAsync();

                    var dto = _mapper.Map<KioskResponseDto>(entity);
                    return ApiResponseDto<KioskResponseDto>.SuccessResult(dto, "Kiosk güncellendi");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Kiosk güncellenirken hata. ID: {KioskId}", request.KioskId);
                    return ApiResponseDto<KioskResponseDto>.ErrorResult("Kiosk güncellenemedi", ex.Message);
                }
            });
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int kioskId)
        {
            if (kioskId <= 0)
            {
                return ApiResponseDto<bool>.ErrorResult("Geçersiz kiosk ID");
            }

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    var repo = _unitOfWork.GetRepository<IKioskRepository>();
                    var entity = await repo.GetByIdAsync(kioskId);
                    if (entity == null)
                    {
                        return ApiResponseDto<bool>.ErrorResult("Kiosk bulunamadı");
                    }

                    // Cascade: Child kayıtları da sil (soft delete)
                    await CascadeDeleteAsync(kioskId);

                    entity.SilindiMi = true;
                    entity.Aktiflik = Aktiflik.Pasif;
                    entity.DuzenlenmeTarihi = DateTime.Now;

                    repo.Update(entity);
                    await _unitOfWork.SaveChangesAsync();

                    return ApiResponseDto<bool>.SuccessResult(true, "Kiosk silindi");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Kiosk silinirken hata. ID: {KioskId}", kioskId);
                    return ApiResponseDto<bool>.ErrorResult("Kiosk silinemedi", ex.Message);
                }
            });
        }

        public async Task<ApiResponseDto<List<KioskResponseDto>>> GetByHizmetBinasiAsync(int hizmetBinasiId)
        {
            if (hizmetBinasiId <= 0)
            {
                return ApiResponseDto<List<KioskResponseDto>>.ErrorResult("Geçersiz hizmet binası ID");
            }

            try
            {
                var repo = _unitOfWork.GetRepository<IKioskRepository>();
                var list = await repo.GetByHizmetBinasiAsync(hizmetBinasiId);
                var dto = _mapper.Map<List<KioskResponseDto>>(list);
                return ApiResponseDto<List<KioskResponseDto>>.SuccessResult(dto, "Kiosk listesi getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hizmet binası kioskları getirilemedi. HizmetBinasiId: {HizmetBinasiId}", hizmetBinasiId);
                return ApiResponseDto<List<KioskResponseDto>>.ErrorResult("Kiosk listesi getirilemedi", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<KioskResponseDto>>> GetActiveAsync()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IKioskRepository>();
                var list = await repo.GetActiveAsync();
                var dto = _mapper.Map<List<KioskResponseDto>>(list);
                return ApiResponseDto<List<KioskResponseDto>>.SuccessResult(dto, "Aktif kiosklar getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif kiosklar getirilemedi");
                return ApiResponseDto<List<KioskResponseDto>>.ErrorResult("Aktif kiosklar getirilemedi", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KioskResponseDto>> GetWithMenuAsync(int kioskId)
        {
            if (kioskId <= 0)
            {
                return ApiResponseDto<KioskResponseDto>.ErrorResult("Geçersiz kiosk ID");
            }

            try
            {
                var repo = _unitOfWork.GetRepository<IKioskRepository>();
                var entity = await repo.GetWithMenuAsync(kioskId);
                if (entity == null)
                {
                    return ApiResponseDto<KioskResponseDto>.ErrorResult("Kiosk bulunamadı");
                }

                var dto = _mapper.Map<KioskResponseDto>(entity);
                return ApiResponseDto<KioskResponseDto>.SuccessResult(dto, "Kiosk detay getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kiosk detay getirilemedi. ID: {KioskId}", kioskId);
                return ApiResponseDto<KioskResponseDto>.ErrorResult("Kiosk detay getirilemedi", ex.Message);
            }
        }

        /// <summary>
        /// Cascade delete: Kiosk silindiğinde child kayıtları da siler
        /// CascadeHelper kullanarak tracking conflict'leri otomatik handle eder
        /// </summary>
        private async Task CascadeDeleteAsync(int kioskId)
        {
            await _cascadeHelper.CascadeSoftDeleteAsync<KioskMenuAtama>(x => x.KioskId == kioskId);
            _logger.LogInformation("Cascade delete: KioskId={KioskId}", kioskId);
        }

        /// <summary>
        /// Cascade update: Kiosk pasif yapıldığında child kayıtları da pasif yap
        /// CascadeHelper kullanarak tracking conflict'leri otomatik handle eder
        /// </summary>
        private async Task CascadeAktiflikUpdateAsync(int kioskId)
        {
            await _cascadeHelper.CascadeAktiflikUpdateAsync<KioskMenuAtama>(x => x.KioskId == kioskId, Aktiflik.Pasif);
            _logger.LogInformation("Cascade pasif: KioskId={KioskId}", kioskId);
        }
    }
}
