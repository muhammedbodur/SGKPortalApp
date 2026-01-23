using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Complex;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Services.SiramatikIslemleri
{
    public class KanalPersonelService : IKanalPersonelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISiramatikQueryRepository _siramatikQueryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<KanalPersonelService> _logger;

        public KanalPersonelService(
            IUnitOfWork unitOfWork,
            ISiramatikQueryRepository siramatikQueryRepository,
            IMapper mapper,
            ILogger<KanalPersonelService> logger)
        {
            _unitOfWork = unitOfWork;
            _siramatikQueryRepository = siramatikQueryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<KanalPersonelResponseDto>> CreateAsync(KanalPersonelCreateRequestDto request)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(request.TcKimlikNo))
                {
                    return ApiResponseDto<KanalPersonelResponseDto>
                        .ErrorResult("TC Kimlik No boş olamaz");
                }

                if (request.KanalAltIslemId <= 0)
                {
                    return ApiResponseDto<KanalPersonelResponseDto>
                        .ErrorResult("Geçersiz kanal alt işlem ID");
                }

                var kanalPersonelRepo = _unitOfWork.GetRepository<IKanalPersonelRepository>();

                // Önce ilişkili kayıtların varlığını kontrol et
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var personelExists = await personelRepo.GetByTcKimlikNoAsync(request.TcKimlikNo);
                
                if (personelExists == null)
                {
                    return ApiResponseDto<KanalPersonelResponseDto>
                        .ErrorResult("Personel bulunamadı");
                }

                var kanalAltIslemRepo = _unitOfWork.GetRepository<IKanalAltIslemRepository>();
                var kanalAltIslemExists = await kanalAltIslemRepo.GetByIdAsync(request.KanalAltIslemId);
                
                if (kanalAltIslemExists == null)
                {
                    return ApiResponseDto<KanalPersonelResponseDto>
                        .ErrorResult("Kanal alt işlem bulunamadı");
                }

                // Aktif çakışma kontrolü
                var hasActiveConflict = await kanalPersonelRepo.HasConflictAsync(request.TcKimlikNo, request.KanalAltIslemId);
                if (hasActiveConflict)
                {
                    return ApiResponseDto<KanalPersonelResponseDto>
                        .ErrorResult("Bu personel zaten bu kanal alt işlemine atanmış");
                }

                // Pasif kayıt var mı kontrol et
                var inactiveRecord = await kanalPersonelRepo.GetInactiveRecordAsync(request.TcKimlikNo, request.KanalAltIslemId);

                KanalPersonel kanalPersonel;
                
                if (inactiveRecord != null)
                {
                    // Pasif kayıt varsa, onu aktif hale getir ve güncelle
                    inactiveRecord.Aktiflik = request.Aktiflik;
                    inactiveRecord.Uzmanlik = request.Uzmanlik;
                    inactiveRecord.SilindiMi = false;
                    kanalPersonelRepo.Update(inactiveRecord);
                    kanalPersonel = inactiveRecord;
                    
                    _logger.LogInformation(
                        "Pasif kayıt aktif hale getirildi. KanalPersonelId: {KanalPersonelId}, TcKimlikNo: {TcKimlikNo}, KanalAltIslemId: {KanalAltIslemId}",
                        inactiveRecord.KanalPersonelId, request.TcKimlikNo, request.KanalAltIslemId);
                }
                else
                {
                    // Yeni kayıt oluştur
                    kanalPersonel = new KanalPersonel
                    {
                        TcKimlikNo = request.TcKimlikNo,
                        KanalAltIslemId = request.KanalAltIslemId,
                        Uzmanlik = request.Uzmanlik,
                        Aktiflik = request.Aktiflik
                    };
                    await kanalPersonelRepo.AddAsync(kanalPersonel);
                }

                await _unitOfWork.SaveChangesAsync();

                // Detaylı veriyi getir
                var createdEntity = await kanalPersonelRepo.GetWithDetailsAsync(kanalPersonel.KanalPersonelId);
                var kanalPersonelDto = _mapper.Map<KanalPersonelResponseDto>(createdEntity);

                return ApiResponseDto<KanalPersonelResponseDto>
                    .SuccessResult(kanalPersonelDto, "Personel ataması başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel ataması oluşturulurken hata oluştu. TcKimlikNo: {TcKimlikNo}, KanalAltIslemId: {KanalAltIslemId}", 
                    request.TcKimlikNo, request.KanalAltIslemId);
                
                var errorMessage = ex.InnerException != null 
                    ? $"{ex.Message} - Inner: {ex.InnerException.Message}" 
                    : ex.Message;
                
                return ApiResponseDto<KanalPersonelResponseDto>
                    .ErrorResult("Personel ataması oluşturulurken bir hata oluştu", errorMessage);
            }
        }

        public async Task<ApiResponseDto<KanalPersonelResponseDto>> UpdateAsync(int kanalPersonelId, KanalPersonelUpdateRequestDto request)
        {
            try
            {
                if (kanalPersonelId <= 0)
                {
                    return ApiResponseDto<KanalPersonelResponseDto>
                        .ErrorResult("Geçersiz kanal personel ID");
                }

                var kanalPersonelRepo = _unitOfWork.GetRepository<IKanalPersonelRepository>();
                var kanalPersonel = await kanalPersonelRepo.GetByIdAsync(kanalPersonelId);

                if (kanalPersonel == null)
                {
                    return ApiResponseDto<KanalPersonelResponseDto>
                        .ErrorResult("Personel ataması bulunamadı");
                }

                // Update
                kanalPersonel.Uzmanlik = request.Uzmanlik;
                kanalPersonel.Aktiflik = request.Aktiflik;

                kanalPersonelRepo.Update(kanalPersonel);
                await _unitOfWork.SaveChangesAsync();

                // Detaylı veriyi getir
                var updatedEntity = await kanalPersonelRepo.GetWithDetailsAsync(kanalPersonelId);
                var kanalPersonelDto = _mapper.Map<KanalPersonelResponseDto>(updatedEntity);

                return ApiResponseDto<KanalPersonelResponseDto>
                    .SuccessResult(kanalPersonelDto, "Personel ataması başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel ataması güncellenirken hata oluştu. ID: {KanalPersonelId}", kanalPersonelId);
                return ApiResponseDto<KanalPersonelResponseDto>
                    .ErrorResult("Personel ataması güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int kanalPersonelId)
        {
            try
            {
                if (kanalPersonelId <= 0)
                {
                    return ApiResponseDto<bool>
                        .ErrorResult("Geçersiz kanal personel ID");
                }

                var kanalPersonelRepo = _unitOfWork.GetRepository<IKanalPersonelRepository>();
                var kanalPersonel = await kanalPersonelRepo.GetByIdAsync(kanalPersonelId);

                if (kanalPersonel == null)
                {
                    return ApiResponseDto<bool>
                        .ErrorResult("Personel ataması bulunamadı");
                }

                // Soft delete
                kanalPersonel.Aktiflik = Aktiflik.Pasif;
                kanalPersonelRepo.Update(kanalPersonel);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponseDto<bool>
                    .SuccessResult(true, "Personel ataması başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel ataması silinirken hata oluştu. ID: {KanalPersonelId}", kanalPersonelId);
                return ApiResponseDto<bool>
                    .ErrorResult("Personel ataması silinirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KanalPersonelResponseDto>> GetByIdAsync(int kanalPersonelId)
        {
            try
            {
                if (kanalPersonelId <= 0)
                {
                    return ApiResponseDto<KanalPersonelResponseDto>
                        .ErrorResult("Geçersiz kanal personel ID");
                }

                var kanalPersonelRepo = _unitOfWork.GetRepository<IKanalPersonelRepository>();
                var kanalPersonel = await kanalPersonelRepo.GetWithDetailsAsync(kanalPersonelId);

                if (kanalPersonel == null)
                {
                    return ApiResponseDto<KanalPersonelResponseDto>
                        .ErrorResult("Personel ataması bulunamadı");
                }

                var kanalPersonelDto = _mapper.Map<KanalPersonelResponseDto>(kanalPersonel);

                return ApiResponseDto<KanalPersonelResponseDto>
                    .SuccessResult(kanalPersonelDto, "Personel ataması başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel ataması getirilirken hata oluştu. ID: {KanalPersonelId}", kanalPersonelId);
                return ApiResponseDto<KanalPersonelResponseDto>
                    .ErrorResult("Personel ataması getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<KanalPersonelResponseDto>>> GetPersonellerByDepartmanHizmetBinasiIdAsync(int departmanHizmetBinasiId)
        {
            try
            {
                if (departmanHizmetBinasiId <= 0)
                {
                    return ApiResponseDto<List<KanalPersonelResponseDto>>
                        .ErrorResult("Geçersiz departman-hizmet binası ID");
                }

                var kanalPersonelRepo = _unitOfWork.GetRepository<IKanalPersonelRepository>();
                
                var kanalPersoneller = await kanalPersonelRepo.GetByDepartmanHizmetBinasiIdAsync(departmanHizmetBinasiId);

                _logger.LogInformation(
                    "Departman-Hizmet binası personel atamaları getirildi. DepartmanHizmetBinasiId: {DepartmanHizmetBinasiId}, Adet: {Count}",
                    departmanHizmetBinasiId,
                    kanalPersoneller.Count());

                var kanalPersonelDtos = _mapper.Map<List<KanalPersonelResponseDto>>(kanalPersoneller);

                return ApiResponseDto<List<KanalPersonelResponseDto>>
                    .SuccessResult(
                        kanalPersonelDtos,
                        $"{kanalPersonelDtos.Count} personel getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Hizmet binası personel atamaları getirilirken hata oluştu. DepartmanHizmetBinasiId: {DepartmanHizmetBinasiId}",
                    departmanHizmetBinasiId);

                return ApiResponseDto<List<KanalPersonelResponseDto>>
                    .ErrorResult("Personel atamaları getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<KanalPersonelResponseDto>>> GetByPersonelTcAsync(string tcKimlikNo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tcKimlikNo))
                {
                    return ApiResponseDto<List<KanalPersonelResponseDto>>
                        .ErrorResult("TC Kimlik No boş olamaz");
                }

                var kanalPersonelRepo = _unitOfWork.GetRepository<IKanalPersonelRepository>();
                var kanalPersoneller = await kanalPersonelRepo.GetByPersonelAsync(tcKimlikNo);

                var kanalPersonelDtos = _mapper.Map<List<KanalPersonelResponseDto>>(kanalPersoneller);

                return ApiResponseDto<List<KanalPersonelResponseDto>>
                    .SuccessResult(kanalPersonelDtos, "Personel atamaları başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel atamaları getirilirken hata oluştu. TcKimlikNo: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<List<KanalPersonelResponseDto>>
                    .ErrorResult("Personel atamaları getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<KanalPersonelResponseDto>>> GetByKanalAltIslemIdAsync(int kanalAltIslemId)
        {
            try
            {
                if (kanalAltIslemId <= 0)
                {
                    return ApiResponseDto<List<KanalPersonelResponseDto>>
                        .ErrorResult("Geçersiz kanal alt işlem ID");
                }

                var kanalPersonelRepo = _unitOfWork.GetRepository<IKanalPersonelRepository>();
                var kanalPersoneller = await kanalPersonelRepo.GetByKanalAltIslemAsync(kanalAltIslemId);

                var kanalPersonelDtos = _mapper.Map<List<KanalPersonelResponseDto>>(kanalPersoneller);

                return ApiResponseDto<List<KanalPersonelResponseDto>>
                    .SuccessResult(kanalPersonelDtos, "Personel atamaları başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal alt işlem personel atamaları getirilirken hata oluştu. KanalAltIslemId: {KanalAltIslemId}", kanalAltIslemId);
                return ApiResponseDto<List<KanalPersonelResponseDto>>
                    .ErrorResult("Personel atamaları getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<PersonelAtamaMatrixDto>>> GetPersonelAtamaMatrixAsync(int departmanHizmetBinasiId)
        {
            try
            {
                if (departmanHizmetBinasiId <= 0)
                {
                    return ApiResponseDto<List<PersonelAtamaMatrixDto>>
                        .ErrorResult("Geçersiz departman-hizmet binası ID");
                }

                var matrixData = await _siramatikQueryRepository
                    .GetPersonelAtamaMatrixByDepartmanHizmetBinasiIdAsync(departmanHizmetBinasiId);

                if (!matrixData.Any())
                {
                    _logger.LogWarning(
                        "Departman-Hizmet binasında personel bulunamadı. DepartmanHizmetBinasiId: {DepartmanHizmetBinasiId}",
                        departmanHizmetBinasiId);

                    return ApiResponseDto<List<PersonelAtamaMatrixDto>>
                        .SuccessResult(
                            new List<PersonelAtamaMatrixDto>(),
                            "Bu hizmet binasında personel bulunamadı");
                }

                _logger.LogInformation(
                    "Personel atama matrix getirildi. DepartmanHizmetBinasiId: {DepartmanHizmetBinasiId}, Personel Sayısı: {Count}",
                    departmanHizmetBinasiId,
                    matrixData.Count);

                return ApiResponseDto<List<PersonelAtamaMatrixDto>>
                    .SuccessResult(
                        matrixData,
                        $"{matrixData.Count} personel getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Personel atama matrix getirilirken hata oluştu. DepartmanHizmetBinasiId: {DepartmanHizmetBinasiId}",
                    departmanHizmetBinasiId);

                return ApiResponseDto<List<PersonelAtamaMatrixDto>>
                    .ErrorResult("Personel atama matrix getirilirken bir hata oluştu", ex.Message);
            }
        }
    }
}