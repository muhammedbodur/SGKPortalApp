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
    public class KanalService : IKanalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<KanalService> _logger;

        public KanalService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<KanalService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<KanalResponseDto>>> GetAllAsync()
        {
            try
            {
                var kanalRepo = _unitOfWork.GetRepository<IKanalRepository>();
                var kanallar = await kanalRepo.GetAllAsync();
                
                var kanalDtos = _mapper.Map<List<KanalResponseDto>>(kanallar);

                return ApiResponseDto<List<KanalResponseDto>>
                    .SuccessResult(kanalDtos, "Kanallar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal listesi getirilirken hata oluştu");
                return ApiResponseDto<List<KanalResponseDto>>
                    .ErrorResult("Kanallar getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KanalResponseDto>> GetByIdAsync(int kanalId)
        {
            try
            {
                if (kanalId <= 0)
                {
                    return ApiResponseDto<KanalResponseDto>
                        .ErrorResult("Geçersiz kanal ID");
                }

                var kanalRepo = _unitOfWork.GetRepository<IKanalRepository>();
                var kanal = await kanalRepo.GetByIdAsync(kanalId);

                if (kanal == null)
                {
                    return ApiResponseDto<KanalResponseDto>
                        .ErrorResult("Kanal bulunamadı");
                }

                var kanalDto = _mapper.Map<KanalResponseDto>(kanal);

                return ApiResponseDto<KanalResponseDto>
                    .SuccessResult(kanalDto, "Kanal başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal getirilirken hata oluştu. ID: {KanalId}", kanalId);
                return ApiResponseDto<KanalResponseDto>
                    .ErrorResult("Kanal getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KanalResponseDto>> CreateAsync(KanalCreateRequestDto request)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(request.KanalAdi))
                {
                    return ApiResponseDto<KanalResponseDto>
                        .ErrorResult("Kanal adı boş olamaz");
                }

                var kanal = _mapper.Map<Kanal>(request);
                kanal.Aktiflik = Aktiflik.Aktif;

                var kanalRepo = _unitOfWork.GetRepository<IKanalRepository>();
                await kanalRepo.AddAsync(kanal);
                await _unitOfWork.SaveChangesAsync();

                var kanalDto = _mapper.Map<KanalResponseDto>(kanal);

                _logger.LogInformation("Yeni kanal oluşturuldu. ID: {KanalId}, Ad: {KanalAdi}", 
                    kanal.KanalId, kanal.KanalAdi);

                return ApiResponseDto<KanalResponseDto>
                    .SuccessResult(kanalDto, "Kanal başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal oluşturulurken hata oluştu. Ad: {KanalAdi}", request.KanalAdi);
                return ApiResponseDto<KanalResponseDto>
                    .ErrorResult("Kanal oluşturulurken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KanalResponseDto>> UpdateAsync(int kanalId, KanalUpdateRequestDto request)
        {
            try
            {
                // Validation
                if (kanalId <= 0)
                {
                    return ApiResponseDto<KanalResponseDto>
                        .ErrorResult("Geçersiz kanal ID");
                }

                if (string.IsNullOrWhiteSpace(request.KanalAdi))
                {
                    return ApiResponseDto<KanalResponseDto>
                        .ErrorResult("Kanal adı boş olamaz");
                }

                var kanalRepo = _unitOfWork.GetRepository<IKanalRepository>();
                var kanal = await kanalRepo.GetByIdAsync(kanalId);

                if (kanal == null)
                {
                    return ApiResponseDto<KanalResponseDto>
                        .ErrorResult("Kanal bulunamadı");
                }

                // Update
                kanal.KanalAdi = request.KanalAdi.Trim();
                kanal.Aktiflik = request.Aktiflik;
                kanal.DuzenlenmeTarihi = DateTime.Now;

                kanalRepo.Update(kanal);
                await _unitOfWork.SaveChangesAsync();

                var kanalDto = _mapper.Map<KanalResponseDto>(kanal);

                _logger.LogInformation("Kanal güncellendi. ID: {KanalId}, Ad: {KanalAdi}", 
                    kanal.KanalId, kanal.KanalAdi);

                return ApiResponseDto<KanalResponseDto>
                    .SuccessResult(kanalDto, "Kanal başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal güncellenirken hata oluştu. ID: {KanalId}", kanalId);
                return ApiResponseDto<KanalResponseDto>
                    .ErrorResult("Kanal güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int kanalId)
        {
            try
            {
                if (kanalId <= 0)
                {
                    return ApiResponseDto<bool>
                        .ErrorResult("Geçersiz kanal ID");
                }

                var kanalRepo = _unitOfWork.GetRepository<IKanalRepository>();
                var kanal = await kanalRepo.GetByIdAsync(kanalId);

                if (kanal == null)
                {
                    return ApiResponseDto<bool>
                        .ErrorResult("Kanal bulunamadı");
                }

                // Soft delete
                kanal.SilindiMi = true;
                kanal.DuzenlenmeTarihi = DateTime.Now;

                kanalRepo.Update(kanal);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Kanal silindi. ID: {KanalId}", kanalId);

                return ApiResponseDto<bool>
                    .SuccessResult(true, "Kanal başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal silinirken hata oluştu. ID: {KanalId}", kanalId);
                return ApiResponseDto<bool>
                    .ErrorResult("Kanal silinirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<KanalResponseDto>>> GetActiveAsync()
        {
            try
            {
                var kanalRepo = _unitOfWork.GetRepository<IKanalRepository>();
                var kanallar = await kanalRepo.GetActiveAsync();
                
                var kanalDtos = _mapper.Map<List<KanalResponseDto>>(kanallar);

                return ApiResponseDto<List<KanalResponseDto>>
                    .SuccessResult(kanalDtos, "Aktif kanallar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif kanal listesi getirilirken hata oluştu");
                return ApiResponseDto<List<KanalResponseDto>>
                    .ErrorResult("Aktif kanallar getirilirken bir hata oluştu", ex.Message);
            }
        }
    }
}
