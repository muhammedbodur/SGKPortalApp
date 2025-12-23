using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Services.PersonelIslemleri
{
    public class SendikaService : ISendikaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SendikaService> _logger;
        private readonly IFieldPermissionValidationService _fieldPermissionService;

        public SendikaService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<SendikaService> logger,
            IFieldPermissionValidationService fieldPermissionService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _fieldPermissionService = fieldPermissionService;
        }

        public async Task<ApiResponseDto<List<SendikaResponseDto>>> GetAllAsync()
        {
            try
            {
                var sendikaRepo = _unitOfWork.GetRepository<ISendikaRepository>();
                var sendikalar = await sendikaRepo.GetAllAsync();
                var sendikaDtos = _mapper.Map<List<SendikaResponseDto>>(sendikalar);

                return ApiResponseDto<List<SendikaResponseDto>>
                    .SuccessResult(sendikaDtos, "Sendikalar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sendika listesi getirilirken hata oluştu");
                return ApiResponseDto<List<SendikaResponseDto>>
                    .ErrorResult("Sendikalar getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<SendikaResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<SendikaResponseDto>
                        .ErrorResult("Geçersiz sendika ID");

                var sendikaRepo = _unitOfWork.GetRepository<ISendikaRepository>();
                var sendika = await sendikaRepo.GetByIdAsync(id);

                if (sendika == null)
                    return ApiResponseDto<SendikaResponseDto>
                        .ErrorResult("Sendika bulunamadı");

                var sendikaDto = _mapper.Map<SendikaResponseDto>(sendika);
                return ApiResponseDto<SendikaResponseDto>
                    .SuccessResult(sendikaDto, "Sendika başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sendika getirilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<SendikaResponseDto>
                    .ErrorResult("Sendika getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<SendikaResponseDto>> CreateAsync(SendikaCreateRequestDto request)
        {
            try
            {
                var sendikaRepo = _unitOfWork.GetRepository<ISendikaRepository>();
                var existingSendika = await sendikaRepo.GetBySendikaAdiAsync(request.SendikaAdi);

                if (existingSendika != null)
                    return ApiResponseDto<SendikaResponseDto>
                        .ErrorResult("Bu isimde bir sendika zaten mevcut");

                var sendika = _mapper.Map<Sendika>(request);
                sendika.Aktiflik = Aktiflik.Aktif;
                
                await sendikaRepo.AddAsync(sendika);
                await _unitOfWork.SaveChangesAsync();

                var sendikaDto = _mapper.Map<SendikaResponseDto>(sendika);
                _logger.LogInformation("Yeni sendika oluşturuldu. ID: {Id}", sendika.SendikaId);

                return ApiResponseDto<SendikaResponseDto>
                    .SuccessResult(sendikaDto, "Sendika başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sendika oluşturulurken hata oluştu");
                return ApiResponseDto<SendikaResponseDto>
                    .ErrorResult("Sendika oluşturulurken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<SendikaResponseDto>> UpdateAsync(int id, SendikaUpdateRequestDto request)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<SendikaResponseDto>
                        .ErrorResult("Geçersiz sendika ID");

                var sendikaRepo = _unitOfWork.GetRepository<ISendikaRepository>();
                var sendika = await sendikaRepo.GetByIdAsync(id);

                if (sendika == null)
                    return ApiResponseDto<SendikaResponseDto>
                        .ErrorResult("Sendika bulunamadı");

                // İsim değişikliği kontrolü
                if (sendika.SendikaAdi != request.SendikaAdi)
                {
                    var existingSendika = await sendikaRepo.GetBySendikaAdiAsync(request.SendikaAdi);
                    if (existingSendika != null && existingSendika.SendikaId != id)
                        return ApiResponseDto<SendikaResponseDto>
                            .ErrorResult("Bu isimde bir sendika zaten mevcut");
                }

                // Aktiflik durumu pasif yapılıyorsa personel kontrolü
                if (sendika.Aktiflik == Aktiflik.Aktif && request.Aktiflik == Aktiflik.Pasif)
                {
                    var personelCount = await sendikaRepo.GetPersonelCountAsync(id);
                    if (personelCount > 0)
                        return ApiResponseDto<SendikaResponseDto>
                            .ErrorResult($"Bu sendikaya bağlı {personelCount} personel bulunmaktadır. Önce personelleri düzenleyiniz");
                }

                // Field permission validation
                var validationResult = await _fieldPermissionService.ValidateFieldPermissionsAsync(
                    sendika,
                    request,
                    "PER.SENDIKA.MANAGE");

                if (!validationResult.Success)
                    return ApiResponseDto<SendikaResponseDto>.ErrorResult(validationResult.Message);

                _mapper.Map(request, sendika);
                sendikaRepo.Update(sendika);
                await _unitOfWork.SaveChangesAsync();

                var sendikaDto = _mapper.Map<SendikaResponseDto>(sendika);
                _logger.LogInformation("Sendika güncellendi. ID: {Id}", id);

                return ApiResponseDto<SendikaResponseDto>
                    .SuccessResult(sendikaDto, "Sendika başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sendika güncellenirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<SendikaResponseDto>
                    .ErrorResult("Sendika güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<bool>
                        .ErrorResult("Geçersiz sendika ID");

                var sendikaRepo = _unitOfWork.GetRepository<ISendikaRepository>();
                var sendika = await sendikaRepo.GetByIdAsync(id);

                if (sendika == null)
                    return ApiResponseDto<bool>
                        .ErrorResult("Sendika bulunamadı");

                // Personel kontrolü
                var personelCount = await sendikaRepo.GetPersonelCountAsync(id);
                if (personelCount > 0)
                    return ApiResponseDto<bool>
                        .ErrorResult($"Bu sendikaya bağlı {personelCount} personel bulunmaktadır. Silme işlemi yapılamaz.");

                // Soft delete
                sendika.Aktiflik = Aktiflik.Pasif;
                sendikaRepo.Update(sendika);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Sendika silindi (soft delete). ID: {Id}", id);

                return ApiResponseDto<bool>
                    .SuccessResult(true, "Sendika başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sendika silinirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<bool>
                    .ErrorResult("Sendika silinirken bir hata oluştu", ex.Message);
            }
        }
    }
}
