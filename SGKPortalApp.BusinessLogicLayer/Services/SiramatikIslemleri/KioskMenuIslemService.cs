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
    public class KioskMenuIslemService : IKioskMenuIslemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<KioskMenuIslemService> _logger;

        public KioskMenuIslemService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<KioskMenuIslemService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<KioskMenuIslemResponseDto>>> GetByKioskMenuAsync(int kioskMenuId)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IKioskMenuIslemRepository>();
                var entities = await repo.GetByKioskMenuAsync(kioskMenuId);
                var dtos = _mapper.Map<List<KioskMenuIslemResponseDto>>(entities);
                return ApiResponseDto<List<KioskMenuIslemResponseDto>>.SuccessResult(dtos, "Menü işlemleri getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Menü işlemleri getirilemedi. KioskMenuId: {KioskMenuId}", kioskMenuId);
                return ApiResponseDto<List<KioskMenuIslemResponseDto>>.ErrorResult("Menü işlemleri getirilemedi", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KioskMenuIslemResponseDto>> GetByIdAsync(int kioskMenuIslemId)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IKioskMenuIslemRepository>();
                var entity = await repo.GetWithDetailsAsync(kioskMenuIslemId);
                
                if (entity == null)
                {
                    return ApiResponseDto<KioskMenuIslemResponseDto>.ErrorResult("Menü işlemi bulunamadı");
                }

                var dto = _mapper.Map<KioskMenuIslemResponseDto>(entity);
                return ApiResponseDto<KioskMenuIslemResponseDto>.SuccessResult(dto, "Menü işlemi getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Menü işlemi getirilemedi. Id: {Id}", kioskMenuIslemId);
                return ApiResponseDto<KioskMenuIslemResponseDto>.ErrorResult("Menü işlemi getirilemedi", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KioskMenuIslemResponseDto>> CreateAsync(KioskMenuIslemCreateRequestDto request)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    var repo = _unitOfWork.GetRepository<IKioskMenuIslemRepository>();

                    // Eğer sıra 0 veya belirtilmemişse, otomatik sıra ata
                    if (request.MenuSira <= 0)
                    {
                        var maxSira = await repo.GetMaxSiraByMenuAsync(request.KioskMenuId);
                        request.MenuSira = maxSira + 1;
                    }
                    else
                    {
                        // Sıra kontrolü
                        var siraExists = await repo.ExistsByMenuAndSiraAsync(request.KioskMenuId, request.MenuSira);
                        if (siraExists)
                        {
                            return ApiResponseDto<KioskMenuIslemResponseDto>.ErrorResult("Bu sıra numarası zaten kullanılıyor");
                        }
                    }

                    var entity = _mapper.Map<KioskMenuIslem>(request);
                    await repo.AddAsync(entity);
                    await _unitOfWork.SaveChangesAsync();

                    var dto = _mapper.Map<KioskMenuIslemResponseDto>(entity);
                    return ApiResponseDto<KioskMenuIslemResponseDto>.SuccessResult(dto, "Menü işlemi oluşturuldu");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Menü işlemi oluşturulamadı");
                    return ApiResponseDto<KioskMenuIslemResponseDto>.ErrorResult("Menü işlemi oluşturulamadı", ex.Message);
                }
            });
        }

        public async Task<ApiResponseDto<KioskMenuIslemResponseDto>> UpdateAsync(KioskMenuIslemUpdateRequestDto request)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    var repo = _unitOfWork.GetRepository<IKioskMenuIslemRepository>();
                    var entity = await repo.GetByIdAsync(request.KioskMenuIslemId);

                    if (entity == null)
                    {
                        return ApiResponseDto<KioskMenuIslemResponseDto>.ErrorResult("Menü işlemi bulunamadı");
                    }

                    // Sıra kontrolü (kendisi hariç)
                    var siraExists = await repo.ExistsByMenuAndSiraAsync(request.KioskMenuId, request.MenuSira, request.KioskMenuIslemId);
                    if (siraExists)
                    {
                        return ApiResponseDto<KioskMenuIslemResponseDto>.ErrorResult("Bu sıra numarası zaten kullanılıyor");
                    }

                    _mapper.Map(request, entity);
                    entity.DuzenlenmeTarihi = DateTime.Now;

                    repo.Update(entity);
                    await _unitOfWork.SaveChangesAsync();

                    var dto = _mapper.Map<KioskMenuIslemResponseDto>(entity);
                    return ApiResponseDto<KioskMenuIslemResponseDto>.SuccessResult(dto, "Menü işlemi güncellendi");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Menü işlemi güncellenemedi. Id: {Id}", request.KioskMenuIslemId);
                    return ApiResponseDto<KioskMenuIslemResponseDto>.ErrorResult("Menü işlemi güncellenemedi", ex.Message);
                }
            });
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int kioskMenuIslemId)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    var repo = _unitOfWork.GetRepository<IKioskMenuIslemRepository>();
                    var entity = await repo.GetByIdAsync(kioskMenuIslemId);

                    if (entity == null)
                    {
                        return ApiResponseDto<bool>.ErrorResult("Menü işlemi bulunamadı");
                    }

                    entity.SilindiMi = true;
                    entity.Aktiflik = Aktiflik.Pasif;
                    entity.DuzenlenmeTarihi = DateTime.Now;

                    repo.Update(entity);
                    await _unitOfWork.SaveChangesAsync();

                    return ApiResponseDto<bool>.SuccessResult(true, "Menü işlemi silindi");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Menü işlemi silinemedi. Id: {Id}", kioskMenuIslemId);
                    return ApiResponseDto<bool>.ErrorResult("Menü işlemi silinemedi", ex.Message);
                }
            });
        }

        public async Task<ApiResponseDto<bool>> UpdateSiraAsync(int kioskMenuIslemId, int yeniSira)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    var repo = _unitOfWork.GetRepository<IKioskMenuIslemRepository>();
                    var entity = await repo.GetByIdAsync(kioskMenuIslemId);

                    if (entity == null)
                    {
                        return ApiResponseDto<bool>.ErrorResult("Menü işlemi bulunamadı");
                    }

                    // Sıra kontrolü
                    var siraExists = await repo.ExistsByMenuAndSiraAsync(entity.KioskMenuId, yeniSira, kioskMenuIslemId);
                    if (siraExists)
                    {
                        return ApiResponseDto<bool>.ErrorResult("Bu sıra numarası zaten kullanılıyor");
                    }

                    entity.MenuSira = yeniSira;
                    entity.DuzenlenmeTarihi = DateTime.Now;

                    repo.Update(entity);
                    await _unitOfWork.SaveChangesAsync();

                    return ApiResponseDto<bool>.SuccessResult(true, "Sıra güncellendi");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Sıra güncellenemedi. Id: {Id}", kioskMenuIslemId);
                    return ApiResponseDto<bool>.ErrorResult("Sıra güncellenemedi", ex.Message);
                }
            });
        }
    }
}
