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

        public async Task<ApiResponseDto<List<KioskMenuIslemResponseDto>>> GetAllAsync()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IKioskMenuIslemRepository>();
                var entities = await repo.FindAsync(x => !x.SilindiMi);
                var dtos = _mapper.Map<List<KioskMenuIslemResponseDto>>(entities);
                return ApiResponseDto<List<KioskMenuIslemResponseDto>>.SuccessResult(dtos, "Tüm menü işlemleri getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tüm menü işlemleri getirilemedi");
                return ApiResponseDto<List<KioskMenuIslemResponseDto>>.ErrorResult("Menü işlemleri getirilemedi", ex.Message);
            }
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

                    // Üst kayıtların silinmiş veya pasif olup olmadığını kontrol et
                    var kioskMenuRepo = _unitOfWork.GetRepository<IKioskMenuRepository>();
                    var kioskMenu = await kioskMenuRepo.GetByIdAsync(request.KioskMenuId);
                    
                    if (kioskMenu == null || kioskMenu.SilindiMi)
                    {
                        return ApiResponseDto<KioskMenuIslemResponseDto>.ErrorResult("Bağlı olduğu Kiosk Menü silinmiş olduğu için bu menü işlemi eklenemez.");
                    }
                    
                    var kanalAltRepo = _unitOfWork.GetRepository<IKanalAltRepository>();
                    var kanalAlt = await kanalAltRepo.GetByIdAsync(request.KanalAltId);
                    
                    if (kanalAlt == null || kanalAlt.SilindiMi)
                    {
                        return ApiResponseDto<KioskMenuIslemResponseDto>.ErrorResult("Bağlı olduğu Alt Kanal silinmiş olduğu için bu menü işlemi eklenemez.");
                    }
                    
                    // Aktif olarak eklenmeye çalışılıyorsa, üst kayıtların aktif olup olmadığını kontrol et
                    if (request.Aktiflik == Aktiflik.Aktif)
                    {
                        if (kioskMenu.Aktiflik != Aktiflik.Aktif)
                        {
                            return ApiResponseDto<KioskMenuIslemResponseDto>.ErrorResult("Bağlı olduğu Kiosk Menü pasif durumda olduğu için bu menü işlemi aktif olarak eklenemez. Önce Kiosk Menü'yü aktif ediniz.");
                        }
                        
                        if (kanalAlt.Aktiflik != Aktiflik.Aktif)
                        {
                            return ApiResponseDto<KioskMenuIslemResponseDto>.ErrorResult("Bağlı olduğu Alt Kanal pasif durumda olduğu için bu menü işlemi aktif olarak eklenemez. Önce Alt Kanal'ı aktif ediniz.");
                        }
                    }

                    // Aynı KanalAltId başka bir menüde zaten kullanılıyor mu kontrol et
                    var existingIslem = await repo.FindAsync(x => x.KanalAltId == request.KanalAltId && !x.SilindiMi);
                    if (existingIslem.Any())
                    {
                        var mevcutMenu = existingIslem.First();
                        return ApiResponseDto<KioskMenuIslemResponseDto>.ErrorResult($"Bu alt kanal zaten başka bir menüye atanmış. Her alt kanal sadece bir menüye atanabilir.");
                    }

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

                    // Üst kayıtların silinmiş veya pasif olup olmadığını kontrol et
                    var kioskMenuRepo = _unitOfWork.GetRepository<IKioskMenuRepository>();
                    var kioskMenu = await kioskMenuRepo.GetByIdAsync(request.KioskMenuId);
                    
                    if (kioskMenu == null || kioskMenu.SilindiMi)
                    {
                        return ApiResponseDto<KioskMenuIslemResponseDto>.ErrorResult("Bağlı olduğu Kiosk Menü silinmiş olduğu için bu menü işlemi güncellenemez.");
                    }
                    
                    var kanalAltRepo = _unitOfWork.GetRepository<IKanalAltRepository>();
                    var kanalAlt = await kanalAltRepo.GetByIdAsync(request.KanalAltId);
                    
                    if (kanalAlt == null || kanalAlt.SilindiMi)
                    {
                        return ApiResponseDto<KioskMenuIslemResponseDto>.ErrorResult("Bağlı olduğu Alt Kanal silinmiş olduğu için bu menü işlemi güncellenemez.");
                    }
                    
                    // Aktif edilmeye çalışılıyorsa, üst kayıtların aktif olup olmadığını kontrol et
                    if (request.Aktiflik == Aktiflik.Aktif)
                    {
                        if (kioskMenu.Aktiflik != Aktiflik.Aktif)
                        {
                            return ApiResponseDto<KioskMenuIslemResponseDto>.ErrorResult("Bağlı olduğu Kiosk Menü pasif durumda olduğu için bu menü işlemi aktif edilemez. Önce Kiosk Menü'yü aktif ediniz.");
                        }
                        
                        if (kanalAlt.Aktiflik != Aktiflik.Aktif)
                        {
                            return ApiResponseDto<KioskMenuIslemResponseDto>.ErrorResult("Bağlı olduğu Alt Kanal pasif durumda olduğu için bu menü işlemi aktif edilemez. Önce Alt Kanal'ı aktif ediniz.");
                        }
                    }

                    // KanalAltId değiştirildiyse, yeni KanalAltId başka bir menüde kullanılıyor mu kontrol et
                    if (entity.KanalAltId != request.KanalAltId)
                    {
                        var existingIslem = await repo.FindAsync(x => x.KanalAltId == request.KanalAltId && !x.SilindiMi);
                        if (existingIslem.Any())
                        {
                            return ApiResponseDto<KioskMenuIslemResponseDto>.ErrorResult($"Bu alt kanal zaten başka bir menüye atanmış. Her alt kanal sadece bir menüye atanabilir.");
                        }
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
