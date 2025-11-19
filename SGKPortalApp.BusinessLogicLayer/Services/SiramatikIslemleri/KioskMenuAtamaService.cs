using AutoMapper;
using Microsoft.Extensions.Logging;
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
    public class KioskMenuAtamaService : IKioskMenuAtamaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<KioskMenuAtamaService> _logger;

        public KioskMenuAtamaService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<KioskMenuAtamaService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<KioskMenuAtamaResponseDto>>> GetAllAsync()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IKioskMenuAtamaRepository>();
                var entities = await repo.GetAllAsync();
                var dtos = _mapper.Map<List<KioskMenuAtamaResponseDto>>(entities);
                return ApiResponseDto<List<KioskMenuAtamaResponseDto>>.SuccessResult(dtos, "Menü atamaları getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Menü atamaları getirilemedi");
                return ApiResponseDto<List<KioskMenuAtamaResponseDto>>.ErrorResult("Menü atamaları getirilemedi", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<KioskMenuAtamaResponseDto>>> GetByKioskAsync(int kioskId)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IKioskMenuAtamaRepository>();
                var entities = await repo.GetByKioskAsync(kioskId);
                var dtos = _mapper.Map<List<KioskMenuAtamaResponseDto>>(entities);
                return ApiResponseDto<List<KioskMenuAtamaResponseDto>>.SuccessResult(dtos, "Kiosk menü atamaları getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kiosk menü atamaları getirilemedi. KioskId: {KioskId}", kioskId);
                return ApiResponseDto<List<KioskMenuAtamaResponseDto>>.ErrorResult("Kiosk menü atamaları getirilemedi", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KioskMenuAtamaResponseDto>> GetActiveByKioskAsync(int kioskId)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IKioskMenuAtamaRepository>();
                var entity = await repo.GetActiveByKioskAsync(kioskId);
                
                if (entity == null)
                {
                    return ApiResponseDto<KioskMenuAtamaResponseDto>.ErrorResult("Aktif menü ataması bulunamadı");
                }

                var dto = _mapper.Map<KioskMenuAtamaResponseDto>(entity);
                return ApiResponseDto<KioskMenuAtamaResponseDto>.SuccessResult(dto, "Aktif menü ataması getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif menü ataması getirilemedi. KioskId: {KioskId}", kioskId);
                return ApiResponseDto<KioskMenuAtamaResponseDto>.ErrorResult("Aktif menü ataması getirilemedi", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KioskMenuAtamaResponseDto>> GetByIdAsync(int kioskMenuAtamaId)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IKioskMenuAtamaRepository>();
                var entity = await repo.GetWithDetailsAsync(kioskMenuAtamaId);
                
                if (entity == null)
                {
                    return ApiResponseDto<KioskMenuAtamaResponseDto>.ErrorResult("Menü ataması bulunamadı");
                }

                var dto = _mapper.Map<KioskMenuAtamaResponseDto>(entity);
                return ApiResponseDto<KioskMenuAtamaResponseDto>.SuccessResult(dto, "Menü ataması getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Menü ataması getirilemedi. Id: {Id}", kioskMenuAtamaId);
                return ApiResponseDto<KioskMenuAtamaResponseDto>.ErrorResult("Menü ataması getirilemedi", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KioskMenuAtamaResponseDto>> CreateAsync(KioskMenuAtamaCreateRequestDto request)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    var repo = _unitOfWork.GetRepository<IKioskMenuAtamaRepository>();

                    // Eğer yeni atama aktif ise, diğer atamaları pasifleştir
                    if (request.Aktiflik == Aktiflik.Aktif)
                    {
                        var hasActive = await repo.HasActiveAtamaAsync(request.KioskId);
                        if (hasActive)
                        {
                            return ApiResponseDto<KioskMenuAtamaResponseDto>.ErrorResult("Bu kiosk için zaten aktif bir menü ataması var. Önce onu pasifleştirin.");
                        }
                    }

                    var entity = _mapper.Map<KioskMenuAtama>(request);
                    entity.AtamaTarihi = DateTime.Now;

                    await repo.AddAsync(entity);
                    await _unitOfWork.SaveChangesAsync();

                    var dto = _mapper.Map<KioskMenuAtamaResponseDto>(entity);
                    return ApiResponseDto<KioskMenuAtamaResponseDto>.SuccessResult(dto, "Menü ataması oluşturuldu");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Menü ataması oluşturulamadı");
                    return ApiResponseDto<KioskMenuAtamaResponseDto>.ErrorResult("Menü ataması oluşturulamadı", ex.Message);
                }
            });
        }

        public async Task<ApiResponseDto<KioskMenuAtamaResponseDto>> UpdateAsync(KioskMenuAtamaUpdateRequestDto request)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    var repo = _unitOfWork.GetRepository<IKioskMenuAtamaRepository>();
                    var entity = await repo.GetByIdAsync(request.KioskMenuAtamaId);

                    if (entity == null)
                    {
                        return ApiResponseDto<KioskMenuAtamaResponseDto>.ErrorResult("Menü ataması bulunamadı");
                    }

                    // Eğer aktif yapılıyorsa, diğer atamaları pasifleştir
                    if (request.Aktiflik == Aktiflik.Aktif && entity.Aktiflik != Aktiflik.Aktif)
                    {
                        await repo.DeactivateOtherAtamasAsync(request.KioskId, request.KioskMenuAtamaId);
                    }

                    _mapper.Map(request, entity);
                    entity.DuzenlenmeTarihi = DateTime.Now;

                    repo.Update(entity);
                    await _unitOfWork.SaveChangesAsync();

                    var dto = _mapper.Map<KioskMenuAtamaResponseDto>(entity);
                    return ApiResponseDto<KioskMenuAtamaResponseDto>.SuccessResult(dto, "Menü ataması güncellendi");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Menü ataması güncellenemedi. Id: {Id}", request.KioskMenuAtamaId);
                    return ApiResponseDto<KioskMenuAtamaResponseDto>.ErrorResult("Menü ataması güncellenemedi", ex.Message);
                }
            });
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int kioskMenuAtamaId)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    var repo = _unitOfWork.GetRepository<IKioskMenuAtamaRepository>();
                    var entity = await repo.GetByIdAsync(kioskMenuAtamaId);

                    if (entity == null)
                    {
                        return ApiResponseDto<bool>.ErrorResult("Menü ataması bulunamadı");
                    }

                    entity.SilindiMi = true;
                    entity.Aktiflik = Aktiflik.Pasif;
                    entity.DuzenlenmeTarihi = DateTime.Now;

                    repo.Update(entity);
                    await _unitOfWork.SaveChangesAsync();

                    return ApiResponseDto<bool>.SuccessResult(true, "Menü ataması silindi");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Menü ataması silinemedi. Id: {Id}", kioskMenuAtamaId);
                    return ApiResponseDto<bool>.ErrorResult("Menü ataması silinemedi", ex.Message);
                }
            });
        }
    }
}
