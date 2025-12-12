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
    public class KanalAltService : IKanalAltService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<KanalAltService> _logger;
        private readonly ICascadeHelper _cascadeHelper;

        public KanalAltService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<KanalAltService> logger, ICascadeHelper cascadeHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cascadeHelper = cascadeHelper;
        }

        public async Task<ApiResponseDto<List<KanalAltResponseDto>>> GetAllAsync()
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IKanalAltRepository>();
                var kanalAltlar = await repository.GetAllWithDetailsAsync();
                var kanalAltDtos = _mapper.Map<List<KanalAltResponseDto>>(kanalAltlar);
                return ApiResponseDto<List<KanalAltResponseDto>>.SuccessResult(kanalAltDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync error");
                return ApiResponseDto<List<KanalAltResponseDto>>.ErrorResult("Alt kanallar listelenirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KanalAltResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IKanalAltRepository>();
                var kanalAlt = await repository.GetWithDetailsAsync(id);
                
                if (kanalAlt == null)
                    return ApiResponseDto<KanalAltResponseDto>.ErrorResult("Alt kanal bulunamadı");

                var kanalAltDto = _mapper.Map<KanalAltResponseDto>(kanalAlt);
                return ApiResponseDto<KanalAltResponseDto>.SuccessResult(kanalAltDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync error");
                return ApiResponseDto<KanalAltResponseDto>.ErrorResult("Alt kanal getirilirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KanalAltResponseDto>> CreateAsync(KanalAltKanalCreateRequestDto request)
        {
            try
            {
                // Üst kaydın (Kanal) silinmiş veya pasif olup olmadığını kontrol et
                var kanalRepo = _unitOfWork.GetRepository<IKanalRepository>();
                var kanal = await kanalRepo.GetByIdAsync(request.KanalId);
                
                if (kanal == null || kanal.SilindiMi)
                {
                    return ApiResponseDto<KanalAltResponseDto>.ErrorResult("Bağlı olduğu Kanal silinmiş olduğu için bu alt kanal eklenemez.");
                }
                
                if (request.Aktiflik == Aktiflik.Aktif && kanal.Aktiflik != Aktiflik.Aktif)
                {
                    return ApiResponseDto<KanalAltResponseDto>.ErrorResult("Bağlı olduğu Kanal pasif durumda olduğu için bu alt kanal aktif olarak eklenemez. Önce Kanal'ı aktif ediniz.");
                }

                // Aynı kanal altında aynı isimde alt kanal var mı kontrol et
                var kanalAltRepo = _unitOfWork.GetRepository<IKanalAltRepository>();
                var existingKanalAlt = await kanalAltRepo.FindAsync(ka => ka.KanalId == request.KanalId && ka.KanalAltAdi == request.KanalAltAdi && !ka.SilindiMi);
                if (existingKanalAlt.Any())
                {
                    return ApiResponseDto<KanalAltResponseDto>.ErrorResult("Bu kanalda aynı isimde bir alt kanal zaten mevcut");
                }

                var kanalAlt = _mapper.Map<KanalAlt>(request);
                var repository = _unitOfWork.Repository<KanalAlt>();
                await repository.AddAsync(kanalAlt);
                await _unitOfWork.SaveChangesAsync();

                var kanalAltDto = _mapper.Map<KanalAltResponseDto>(kanalAlt);
                return ApiResponseDto<KanalAltResponseDto>.SuccessResult(kanalAltDto, "Alt kanal başarıyla eklendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync error");
                return ApiResponseDto<KanalAltResponseDto>.ErrorResult("Alt kanal eklenirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KanalAltResponseDto>> UpdateAsync(int id, KanalAltKanalUpdateRequestDto request)
        {
            try
            {
                var repository = _unitOfWork.Repository<KanalAlt>();
                var kanalAlt = await repository.GetByIdAsync(id);

                if (kanalAlt == null)
                    return ApiResponseDto<KanalAltResponseDto>.ErrorResult("Alt kanal bulunamadı");

                var oldAktiflik = kanalAlt.Aktiflik;

                // Üst kaydın (Kanal) silinmiş veya pasif olup olmadığını kontrol et
                var kanalRepo = _unitOfWork.GetRepository<IKanalRepository>();
                var kanal = await kanalRepo.GetByIdAsync(request.KanalId);
                
                if (kanal == null || kanal.SilindiMi)
                {
                    return ApiResponseDto<KanalAltResponseDto>.ErrorResult("Bağlı olduğu Kanal silinmiş olduğu için bu alt kanal güncellenemez.");
                }
                
                if (request.Aktiflik == Aktiflik.Aktif && kanal.Aktiflik != Aktiflik.Aktif)
                {
                    return ApiResponseDto<KanalAltResponseDto>.ErrorResult("Bağlı olduğu Kanal pasif durumda olduğu için bu alt kanal aktif edilemez. Önce Kanal'ı aktif ediniz.");
                }

                // Aynı kanal altında aynı isimde başka alt kanal var mı kontrol et (kendisi hariç)
                var kanalAltRepo = _unitOfWork.GetRepository<IKanalAltRepository>();
                var existingKanalAlt = await kanalAltRepo.FindAsync(ka => ka.KanalId == request.KanalId && ka.KanalAltAdi == request.KanalAltAdi && ka.KanalAltId != id && !ka.SilindiMi);
                if (existingKanalAlt.Any())
                {
                    return ApiResponseDto<KanalAltResponseDto>.ErrorResult("Bu kanalda aynı isimde bir alt kanal zaten mevcut");
                }

                _mapper.Map(request, kanalAlt);
                repository.Update(kanalAlt);

                // Cascade: Aktiflik değişmişse child kayıtları da güncelle
                if (oldAktiflik != request.Aktiflik)
                {
                    await CascadeAktiflikUpdateAsync(id, request.Aktiflik);
                }

                await _unitOfWork.SaveChangesAsync();

                var kanalAltDto = _mapper.Map<KanalAltResponseDto>(kanalAlt);
                return ApiResponseDto<KanalAltResponseDto>.SuccessResult(kanalAltDto, "Alt kanal başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync error");
                return ApiResponseDto<KanalAltResponseDto>.ErrorResult("Alt kanal güncellenirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                var repository = _unitOfWork.Repository<KanalAlt>();
                var kanalAlt = await repository.GetByIdAsync(id);

                if (kanalAlt == null)
                    return ApiResponseDto<bool>.ErrorResult("Alt kanal bulunamadı");

                // Cascade: Child kayıtları da sil (soft delete)
                await CascadeDeleteAsync(id);

                repository.Delete(kanalAlt);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponseDto<bool>.SuccessResult(true, "Alt kanal başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync error");
                return ApiResponseDto<bool>.ErrorResult("Alt kanal silinirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<KanalAltResponseDto>>> GetActiveAsync()
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IKanalAltRepository>();
                var kanalAltlar = await repository.GetActiveAsync();
                var kanalAltDtos = _mapper.Map<List<KanalAltResponseDto>>(kanalAltlar);
                return ApiResponseDto<List<KanalAltResponseDto>>.SuccessResult(kanalAltDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveAsync error");
                return ApiResponseDto<List<KanalAltResponseDto>>.ErrorResult("Aktif alt kanallar listelenirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<KanalAltResponseDto>>> GetByKanalIdAsync(int kanalId)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IKanalAltRepository>();
                var kanalAltlar = await repository.GetByKanalAsync(kanalId);
                var kanalAltDtos = _mapper.Map<List<KanalAltResponseDto>>(kanalAltlar);
                return ApiResponseDto<List<KanalAltResponseDto>>.SuccessResult(kanalAltDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByKanalIdAsync error");
                return ApiResponseDto<List<KanalAltResponseDto>>.ErrorResult("Alt kanallar listelenirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KanalAltResponseDto>> GetWithDetailsAsync(int id)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IKanalAltRepository>();
                var kanalAlt = await repository.GetWithDetailsAsync(id);
                
                if (kanalAlt == null)
                    return ApiResponseDto<KanalAltResponseDto>.ErrorResult("Alt kanal bulunamadı");

                var kanalAltDto = _mapper.Map<KanalAltResponseDto>(kanalAlt);
                return ApiResponseDto<KanalAltResponseDto>.SuccessResult(kanalAltDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetWithDetailsAsync error");
                return ApiResponseDto<KanalAltResponseDto>.ErrorResult("Alt kanal getirilirken hata oluştu", ex.Message);
            }
        }

        /// <summary>
        /// Cascade delete: KanalAlt silindiğinde child kayıtları da siler
        /// CascadeHelper kullanarak tracking conflict'leri otomatik handle eder
        /// </summary>
        private async Task CascadeDeleteAsync(int kanalAltId)
        {
            // KanalAltIslem kayıtlarını soft delete
            await _cascadeHelper.CascadeSoftDeleteAsync<KanalAltIslem>(x => x.KanalAltId == kanalAltId);
            
            // KioskMenuIslem kayıtlarını soft delete
            await _cascadeHelper.CascadeSoftDeleteAsync<KioskMenuIslem>(x => x.KanalAltId == kanalAltId);

            _logger.LogInformation("Cascade delete: KanalAltId={KanalAltId}", kanalAltId);
        }

        /// <summary>
        /// Cascade update: KanalAlt Aktiflik değiştiğinde child kayıtları da günceller
        /// CascadeHelper kullanarak tracking conflict'leri otomatik handle eder
        /// </summary>
        private async Task CascadeAktiflikUpdateAsync(int kanalAltId, Aktiflik yeniAktiflik)
        {
            // KanalAltIslem kayıtlarını güncelle
            await _cascadeHelper.CascadeAktiflikUpdateAsync<KanalAltIslem>(x => x.KanalAltId == kanalAltId, yeniAktiflik);
            
            // KioskMenuIslem kayıtlarını güncelle
            await _cascadeHelper.CascadeAktiflikUpdateAsync<KioskMenuIslem>(x => x.KanalAltId == kanalAltId, yeniAktiflik);

            _logger.LogInformation("Cascade aktiflik update: KanalAltId={KanalAltId}, YeniAktiflik={Aktiflik}", kanalAltId, yeniAktiflik);
        }
    }
}
