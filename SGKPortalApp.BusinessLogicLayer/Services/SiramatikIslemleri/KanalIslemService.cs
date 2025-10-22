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
    public class KanalIslemService : IKanalIslemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<KanalIslemService> _logger;

        public KanalIslemService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<KanalIslemService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<KanalIslemResponseDto>>> GetAllAsync()
        {
            try
            {
                var kanalIslemRepo = _unitOfWork.GetRepository<IKanalIslemRepository>();
                var kanalIslemler = await kanalIslemRepo.GetAllWithDetailsAsync();

                var kanalIslemDtos = _mapper.Map<List<KanalIslemResponseDto>>(kanalIslemler);

                return ApiResponseDto<List<KanalIslemResponseDto>>
                    .SuccessResult(kanalIslemDtos, "Kanal işlemler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal işlem listesi getirilirken hata oluştu");
                return ApiResponseDto<List<KanalIslemResponseDto>>
                    .ErrorResult("Kanal işlemler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KanalIslemResponseDto>> GetByIdAsync(int kanalIslemId)
        {
            try
            {
                if (kanalIslemId <= 0)
                {
                    return ApiResponseDto<KanalIslemResponseDto>
                        .ErrorResult("Geçersiz kanal işlem ID");
                }

                var kanalIslemRepo = _unitOfWork.GetRepository<IKanalIslemRepository>();
                var kanalIslem = await kanalIslemRepo.GetByIdAsync(kanalIslemId);

                if (kanalIslem == null)
                {
                    return ApiResponseDto<KanalIslemResponseDto>
                        .ErrorResult("Kanal işlem bulunamadı");
                }

                var kanalIslemDto = _mapper.Map<KanalIslemResponseDto>(kanalIslem);

                return ApiResponseDto<KanalIslemResponseDto>
                    .SuccessResult(kanalIslemDto, "Kanal işlem başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal işlem getirilirken hata oluştu. ID: {KanalIslemId}", kanalIslemId);
                return ApiResponseDto<KanalIslemResponseDto>
                    .ErrorResult("Kanal işlem getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KanalIslemResponseDto>> CreateAsync(KanalIslemCreateRequestDto request)
        {
            try
            {
                // Validation
                if (request.KanalId <= 0)
                {
                    return ApiResponseDto<KanalIslemResponseDto>
                        .ErrorResult("Kanal seçimi zorunludur");
                }

                if (request.HizmetBinasiId <= 0)
                {
                    return ApiResponseDto<KanalIslemResponseDto>
                        .ErrorResult("Hizmet binası seçimi zorunludur");
                }

                if (request.BaslangicNumara >= request.BitisNumara)
                {
                    return ApiResponseDto<KanalIslemResponseDto>
                        .ErrorResult("Başlangıç numarası, bitiş numarasından küçük olmalıdır");
                }

                var kanalIslem = _mapper.Map<KanalIslem>(request);
                kanalIslem.Aktiflik = Aktiflik.Aktif;

                var kanalIslemRepo = _unitOfWork.GetRepository<IKanalIslemRepository>();
                await kanalIslemRepo.AddAsync(kanalIslem);
                await _unitOfWork.SaveChangesAsync();

                var kanalIslemDto = _mapper.Map<KanalIslemResponseDto>(kanalIslem);

                _logger.LogInformation("Yeni kanal işlem oluşturuldu. ID: {KanalIslemId}, KanalId: {KanalId}, HizmetBinasiId: {HizmetBinasiId}",
                    kanalIslem.KanalIslemId, kanalIslem.KanalId, kanalIslem.HizmetBinasiId);

                return ApiResponseDto<KanalIslemResponseDto>
                    .SuccessResult(kanalIslemDto, "Kanal işlem başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal işlem oluşturulurken hata oluştu. KanalId: {KanalId}", request.KanalId);
                return ApiResponseDto<KanalIslemResponseDto>
                    .ErrorResult("Kanal işlem oluşturulurken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KanalIslemResponseDto>> UpdateAsync(int kanalIslemId, KanalIslemUpdateRequestDto request)
        {
            try
            {
                // Validation
                if (kanalIslemId <= 0)
                {
                    return ApiResponseDto<KanalIslemResponseDto>
                        .ErrorResult("Geçersiz kanal işlem ID");
                }

                if (request.KanalId <= 0)
                {
                    return ApiResponseDto<KanalIslemResponseDto>
                        .ErrorResult("Kanal seçimi zorunludur");
                }

                if (request.HizmetBinasiId <= 0)
                {
                    return ApiResponseDto<KanalIslemResponseDto>
                        .ErrorResult("Hizmet binası seçimi zorunludur");
                }

                if (request.BaslangicNumara >= request.BitisNumara)
                {
                    return ApiResponseDto<KanalIslemResponseDto>
                        .ErrorResult("Başlangıç numarası, bitiş numarasından küçük olmalıdır");
                }

                var kanalIslemRepo = _unitOfWork.GetRepository<IKanalIslemRepository>();
                var kanalIslem = await kanalIslemRepo.GetByIdAsync(kanalIslemId);

                if (kanalIslem == null)
                {
                    return ApiResponseDto<KanalIslemResponseDto>
                        .ErrorResult("Kanal işlem bulunamadı");
                }

                // Update
                kanalIslem.KanalId = request.KanalId;
                kanalIslem.HizmetBinasiId = request.HizmetBinasiId;
                kanalIslem.BaslangicNumara = request.BaslangicNumara;
                kanalIslem.BitisNumara = request.BitisNumara;
                kanalIslem.Aktiflik = request.Aktiflik;
                kanalIslem.DuzenlenmeTarihi = DateTime.Now;

                kanalIslemRepo.Update(kanalIslem);
                await _unitOfWork.SaveChangesAsync();

                var kanalIslemDto = _mapper.Map<KanalIslemResponseDto>(kanalIslem);

                _logger.LogInformation("Kanal işlem güncellendi. ID: {KanalIslemId}, KanalId: {KanalId}",
                    kanalIslem.KanalIslemId, kanalIslem.KanalId);

                return ApiResponseDto<KanalIslemResponseDto>
                    .SuccessResult(kanalIslemDto, "Kanal işlem başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal işlem güncellenirken hata oluştu. ID: {KanalIslemId}", kanalIslemId);
                return ApiResponseDto<KanalIslemResponseDto>
                    .ErrorResult("Kanal işlem güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int kanalIslemId)
        {
            try
            {
                if (kanalIslemId <= 0)
                {
                    return ApiResponseDto<bool>
                        .ErrorResult("Geçersiz kanal işlem ID");
                }

                var kanalIslemRepo = _unitOfWork.GetRepository<IKanalIslemRepository>();
                var kanalIslem = await kanalIslemRepo.GetByIdAsync(kanalIslemId);

                if (kanalIslem == null)
                {
                    return ApiResponseDto<bool>
                        .ErrorResult("Kanal işlem bulunamadı");
                }

                // Soft delete
                kanalIslem.SilindiMi = true;
                kanalIslem.DuzenlenmeTarihi = DateTime.Now;

                kanalIslemRepo.Update(kanalIslem);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Kanal işlem silindi. ID: {KanalIslemId}", kanalIslemId);

                return ApiResponseDto<bool>
                    .SuccessResult(true, "Kanal işlem başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal işlem silinirken hata oluştu. ID: {KanalIslemId}", kanalIslemId);
                return ApiResponseDto<bool>
                    .ErrorResult("Kanal işlem silinirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<KanalIslemResponseDto>>> GetByKanalIdAsync(int kanalId)
        {
            try
            {
                if (kanalId <= 0)
                {
                    return ApiResponseDto<List<KanalIslemResponseDto>>
                        .ErrorResult("Geçersiz kanal ID");
                }

                var kanalIslemRepo = _unitOfWork.GetRepository<IKanalIslemRepository>();
                var kanalIslemler = await kanalIslemRepo.GetByKanalAsync(kanalId);

                var kanalIslemDtos = _mapper.Map<List<KanalIslemResponseDto>>(kanalIslemler);

                return ApiResponseDto<List<KanalIslemResponseDto>>
                    .SuccessResult(kanalIslemDtos, "Kanal işlemler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal işlem listesi getirilirken hata oluştu. KanalId: {KanalId}", kanalId);
                return ApiResponseDto<List<KanalIslemResponseDto>>
                    .ErrorResult("Kanal işlemler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<KanalIslemResponseDto>>> GetByHizmetBinasiIdAsync(int hizmetBinasiId)
        {
            try
            {
                if (hizmetBinasiId <= 0)
                {
                    return ApiResponseDto<List<KanalIslemResponseDto>>
                        .ErrorResult("Geçersiz hizmet binası ID");
                }

                var kanalIslemRepo = _unitOfWork.GetRepository<IKanalIslemRepository>();
                var kanalIslemler = await kanalIslemRepo.GetByHizmetBinasiAsync(hizmetBinasiId);

                var kanalIslemDtos = _mapper.Map<List<KanalIslemResponseDto>>(kanalIslemler);

                return ApiResponseDto<List<KanalIslemResponseDto>>
                    .SuccessResult(kanalIslemDtos, "Kanal işlemler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal işlem listesi getirilirken hata oluştu. HizmetBinasiId: {HizmetBinasiId}", hizmetBinasiId);
                return ApiResponseDto<List<KanalIslemResponseDto>>
                    .ErrorResult("Kanal işlemler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<KanalIslemResponseDto>>> GetActiveAsync()
        {
            try
            {
                var kanalIslemRepo = _unitOfWork.GetRepository<IKanalIslemRepository>();
                var kanalIslemler = await kanalIslemRepo.GetActiveAsync();

                var kanalIslemDtos = _mapper.Map<List<KanalIslemResponseDto>>(kanalIslemler);

                return ApiResponseDto<List<KanalIslemResponseDto>>
                    .SuccessResult(kanalIslemDtos, "Aktif kanal işlemler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif kanal işlem listesi getirilirken hata oluştu");
                return ApiResponseDto<List<KanalIslemResponseDto>>
                    .ErrorResult("Aktif kanal işlemler getirilirken bir hata oluştu", ex.Message);
            }
        }
    }
}