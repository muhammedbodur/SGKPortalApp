using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Exceptions;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Services.PersonelIslemleri
{
    public class PersonelService : IPersonelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PersonelService> _logger;

        public PersonelService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<PersonelService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<PersonelResponseDto>>> GetAllAsync()
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var personeller = await personelRepo.GetAllWithDetailsAsync();
                var personelDtos = _mapper.Map<List<PersonelResponseDto>>(personeller);

                return ApiResponseDto<List<PersonelResponseDto>>
                    .SuccessResult(personelDtos, "Personeller başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel listesi getirilirken hata oluştu");
                return ApiResponseDto<List<PersonelResponseDto>>
                    .ErrorResult("Personeller getirilirken bir hata oluştu", ex.Message);
            }
        }


        public async Task<ApiResponseDto<PersonelResponseDto>> GetByTcKimlikNoAsync(string tcKimlikNo)
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var personel = await personelRepo.GetByTcKimlikNoWithDetailsAsync(tcKimlikNo);

                if (personel == null)
                {
                    return ApiResponseDto<PersonelResponseDto>
                        .ErrorResult("Personel bulunamadı");
                }

                var personelDto = _mapper.Map<PersonelResponseDto>(personel);
                return ApiResponseDto<PersonelResponseDto>
                    .SuccessResult(personelDto, "Personel başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel getirilirken hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<PersonelResponseDto>
                    .ErrorResult("Personel getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<PersonelResponseDto>> CreateAsync(PersonelCreateRequestDto request)
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();

                // TC Kimlik No kontrolü
                var existingPersonel = await personelRepo.GetByTcKimlikNoAsync(request.TcKimlikNo);
                if (existingPersonel != null)
                {
                    return ApiResponseDto<PersonelResponseDto>
                        .ErrorResult("Bu TC Kimlik No ile kayıtlı personel zaten mevcut");
                }

                var personel = _mapper.Map<Personel>(request);
                await personelRepo.AddAsync(personel);
                await _unitOfWork.SaveChangesAsync();

                var personelDto = _mapper.Map<PersonelResponseDto>(personel);
                return ApiResponseDto<PersonelResponseDto>
                    .SuccessResult(personelDto, "Personel başarıyla oluşturuldu");
            }
            catch (DatabaseException ex)
            {
                _logger.LogWarning(ex, "Veritabanı kısıtlama hatası: {ErrorType}", ex.ErrorType);
                return ApiResponseDto<PersonelResponseDto>
                    .ErrorResult(ex.UserFriendlyMessage, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel oluşturulurken hata oluştu");
                return ApiResponseDto<PersonelResponseDto>
                    .ErrorResult("Personel oluşturulurken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<PersonelResponseDto>> UpdateAsync(string tcKimlikNo, PersonelUpdateRequestDto request)
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var personel = await personelRepo.GetByTcKimlikNoAsync(tcKimlikNo);

                if (personel == null)
                {
                    return ApiResponseDto<PersonelResponseDto>
                        .ErrorResult("Personel bulunamadı");
                }

                _mapper.Map(request, personel);
                personelRepo.Update(personel);
                await _unitOfWork.SaveChangesAsync();

                var personelDto = _mapper.Map<PersonelResponseDto>(personel);
                return ApiResponseDto<PersonelResponseDto>
                    .SuccessResult(personelDto, "Personel başarıyla güncellendi");
            }
            catch (DatabaseException ex)
            {
                _logger.LogWarning(ex, "Veritabanı kısıtlama hatası: {ErrorType}", ex.ErrorType);
                return ApiResponseDto<PersonelResponseDto>
                    .ErrorResult(ex.UserFriendlyMessage, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel güncellenirken hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<PersonelResponseDto>
                    .ErrorResult("Personel güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(string tcKimlikNo)
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var personel = await personelRepo.GetByTcKimlikNoAsync(tcKimlikNo);

                if (personel == null)
                {
                    return ApiResponseDto<bool>
                        .ErrorResult("Personel bulunamadı");
                }

                personelRepo.Delete(personel);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponseDto<bool>
                    .SuccessResult(true, "Personel başarıyla silindi");
            }
            catch (DatabaseException ex)
            {
                _logger.LogWarning(ex, "Veritabanı kısıtlama hatası: {ErrorType}", ex.ErrorType);
                return ApiResponseDto<bool>
                    .ErrorResult(ex.UserFriendlyMessage, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel silinirken hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<bool>
                    .ErrorResult("Personel silinirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<PersonelResponseDto>>> GetActiveAsync()
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var personeller = await personelRepo.GetActiveAsync();
                var personelDtos = _mapper.Map<List<PersonelResponseDto>>(personeller);

                return ApiResponseDto<List<PersonelResponseDto>>
                    .SuccessResult(personelDtos, "Aktif personeller başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif personeller getirilirken hata oluştu");
                return ApiResponseDto<List<PersonelResponseDto>>
                    .ErrorResult("Aktif personeller getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<PagedResponseDto<PersonelListResponseDto>>> GetPagedAsync(PersonelFilterRequestDto filter)
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var pagedResult = await personelRepo.GetPagedAsync(filter);
                
                return ApiResponseDto<PagedResponseDto<PersonelListResponseDto>>
                    .SuccessResult(pagedResult, "Sayfalı personel listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sayfalı personel listesi getirilirken hata oluştu");
                return ApiResponseDto<PagedResponseDto<PersonelListResponseDto>>
                    .ErrorResult("Sayfalı personel listesi getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<PersonelResponseDto>>> GetByDepartmanAsync(int departmanId)
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var personeller = await personelRepo.GetByDepartmanAsync(departmanId);
                var personelDtos = _mapper.Map<List<PersonelResponseDto>>(personeller);

                return ApiResponseDto<List<PersonelResponseDto>>
                    .SuccessResult(personelDtos, "Departman personelleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Departman personelleri getirilirken hata oluştu. DepartmanId: {DepartmanId}", departmanId);
                return ApiResponseDto<List<PersonelResponseDto>>
                    .ErrorResult("Departman personelleri getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<PersonelResponseDto>>> GetByServisAsync(int servisId)
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var personeller = await personelRepo.GetByServisAsync(servisId);
                var personelDtos = _mapper.Map<List<PersonelResponseDto>>(personeller);

                return ApiResponseDto<List<PersonelResponseDto>>
                    .SuccessResult(personelDtos, "Servis personelleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Servis personelleri getirilirken hata oluştu. ServisId: {ServisId}", servisId);
                return ApiResponseDto<List<PersonelResponseDto>>
                    .ErrorResult("Servis personelleri getirilirken bir hata oluştu", ex.Message);
            }
        }
    }
}
