using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Exceptions;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;

namespace SGKPortalApp.BusinessLogicLayer.Services.PersonelIslemleri
{
    public class PersonelCocukService : IPersonelCocukService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PersonelCocukService> _logger;

        public PersonelCocukService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<PersonelCocukService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<PersonelCocukResponseDto>>> GetAllAsync()
        {
            try
            {
                var cocuklar = await _unitOfWork.Repository<PersonelCocuk>().GetAllAsync();
                var cocuklarDto = _mapper.Map<List<PersonelCocukResponseDto>>(cocuklar);

                return ApiResponseDto<List<PersonelCocukResponseDto>>
                    .SuccessResult(cocuklarDto, "Çocuk bilgileri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Çocuk bilgileri getirilirken hata oluştu");
                return ApiResponseDto<List<PersonelCocukResponseDto>>
                    .ErrorResult("Çocuk bilgileri getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<PersonelCocukResponseDto>>> GetByPersonelTcKimlikNoAsync(string tcKimlikNo)
        {
            try
            {
                var repository = _unitOfWork.Repository<PersonelCocuk>() as IPersonelCocukRepository;
                if (repository == null)
                {
                    return ApiResponseDto<List<PersonelCocukResponseDto>>
                        .ErrorResult("Repository bulunamadı");
                }

                var cocuklar = await repository.GetByPersonelTcKimlikNoAsync(tcKimlikNo);
                var cocuklarDto = _mapper.Map<List<PersonelCocukResponseDto>>(cocuklar);

                return ApiResponseDto<List<PersonelCocukResponseDto>>
                    .SuccessResult(cocuklarDto, "Çocuk bilgileri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel çocuk bilgileri getirilirken hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<List<PersonelCocukResponseDto>>
                    .ErrorResult("Çocuk bilgileri getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<PersonelCocukResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var cocuk = await _unitOfWork.Repository<PersonelCocuk>().GetByIdAsync(id);
                
                if (cocuk == null)
                    return ApiResponseDto<PersonelCocukResponseDto>.ErrorResult("Çocuk bilgisi bulunamadı");

                var cocukDto = _mapper.Map<PersonelCocukResponseDto>(cocuk);
                return ApiResponseDto<PersonelCocukResponseDto>
                    .SuccessResult(cocukDto, "Çocuk bilgisi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Çocuk bilgisi getirilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<PersonelCocukResponseDto>
                    .ErrorResult("Çocuk bilgisi getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<PersonelCocukResponseDto>> CreateAsync(PersonelCocukCreateRequestDto request)
        {
            try
            {
                var cocuk = _mapper.Map<PersonelCocuk>(request);
                await _unitOfWork.Repository<PersonelCocuk>().AddAsync(cocuk);
                await _unitOfWork.SaveChangesAsync();

                var cocukDto = _mapper.Map<PersonelCocukResponseDto>(cocuk);
                return ApiResponseDto<PersonelCocukResponseDto>
                    .SuccessResult(cocukDto, "Çocuk bilgisi başarıyla eklendi");
            }
            catch (DatabaseException ex)
            {
                _logger.LogWarning(ex, "Veritabanı kısıtlama hatası: {ErrorType}", ex.ErrorType);
                return ApiResponseDto<PersonelCocukResponseDto>
                    .ErrorResult(ex.UserFriendlyMessage, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Çocuk bilgisi eklenirken hata oluştu");
                return ApiResponseDto<PersonelCocukResponseDto>
                    .ErrorResult("Çocuk bilgisi eklenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<PersonelCocukResponseDto>> UpdateAsync(int id, PersonelCocukUpdateRequestDto request)
        {
            try
            {
                var cocuk = await _unitOfWork.Repository<PersonelCocuk>().GetByIdAsync(id);
                
                if (cocuk == null)
                    return ApiResponseDto<PersonelCocukResponseDto>.ErrorResult("Çocuk bilgisi bulunamadı");

                _mapper.Map(request, cocuk);
                _unitOfWork.Repository<PersonelCocuk>().Update(cocuk);
                await _unitOfWork.SaveChangesAsync();

                var cocukDto = _mapper.Map<PersonelCocukResponseDto>(cocuk);
                return ApiResponseDto<PersonelCocukResponseDto>
                    .SuccessResult(cocukDto, "Çocuk bilgisi başarıyla güncellendi");
            }
            catch (DatabaseException ex)
            {
                _logger.LogWarning(ex, "Veritabanı kısıtlama hatası: {ErrorType}", ex.ErrorType);
                return ApiResponseDto<PersonelCocukResponseDto>
                    .ErrorResult(ex.UserFriendlyMessage, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Çocuk bilgisi güncellenirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<PersonelCocukResponseDto>
                    .ErrorResult("Çocuk bilgisi güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                var cocuk = await _unitOfWork.Repository<PersonelCocuk>().GetByIdAsync(id);
                
                if (cocuk == null)
                    return ApiResponseDto<bool>.ErrorResult("Çocuk bilgisi bulunamadı");

                _unitOfWork.Repository<PersonelCocuk>().Delete(cocuk);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponseDto<bool>
                    .SuccessResult(true, "Çocuk bilgisi başarıyla silindi");
            }
            catch (DatabaseException ex)
            {
                _logger.LogWarning(ex, "Veritabanı kısıtlama hatası: {ErrorType}", ex.ErrorType);
                return ApiResponseDto<bool>
                    .ErrorResult(ex.UserFriendlyMessage, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Çocuk bilgisi silinirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<bool>
                    .ErrorResult("Çocuk bilgisi silinirken bir hata oluştu", ex.Message);
            }
        }
    }
}
