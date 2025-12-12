using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Services.PersonelIslemleri
{
    public class ServisService : IServisService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ServisService> _logger;

        public ServisService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ServisService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<ServisResponseDto>>> GetAllAsync()
        {
            try
            {
                var servisRepo = _unitOfWork.GetRepository<IServisRepository>();
                var servisler = await servisRepo.GetActiveAsync();
                var servisDtos = _mapper.Map<List<ServisResponseDto>>(servisler);

                return ApiResponseDto<List<ServisResponseDto>>
                    .SuccessResult(servisDtos, "Servisler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Servis listesi getirilirken hata oluştu");
                return ApiResponseDto<List<ServisResponseDto>>
                    .ErrorResult("Servisler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<ServisResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<ServisResponseDto>
                        .ErrorResult("Geçersiz servis ID");

                var servisRepo = _unitOfWork.GetRepository<IServisRepository>();
                var servis = await servisRepo.GetByIdAsync(id);

                if (servis == null)
                    return ApiResponseDto<ServisResponseDto>
                        .ErrorResult("Servis bulunamadı");

                var servisDto = _mapper.Map<ServisResponseDto>(servis);
                return ApiResponseDto<ServisResponseDto>
                    .SuccessResult(servisDto, "Servis başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Servis getirilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<ServisResponseDto>
                    .ErrorResult("Servis getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<ServisResponseDto>> CreateAsync(ServisCreateRequestDto request)
        {
            try
            {
                var servisRepo = _unitOfWork.GetRepository<IServisRepository>();
                var existingServis = await servisRepo.GetByServisAdiAsync(request.ServisAdi);

                if (existingServis != null)
                    return ApiResponseDto<ServisResponseDto>
                        .ErrorResult("Bu isimde bir servis zaten mevcut");

                var servis = _mapper.Map<Servis>(request);
                await servisRepo.AddAsync(servis);
                await _unitOfWork.SaveChangesAsync();

                var servisDto = _mapper.Map<ServisResponseDto>(servis);
                _logger.LogInformation("Yeni servis oluşturuldu. ID: {Id}", servis.ServisId);

                return ApiResponseDto<ServisResponseDto>
                    .SuccessResult(servisDto, "Servis başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Servis oluşturulurken hata oluştu");
                return ApiResponseDto<ServisResponseDto>
                    .ErrorResult("Servis oluşturulurken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<ServisResponseDto>> UpdateAsync(int id, ServisUpdateRequestDto request)
        {
            try
            {
                var servisRepo = _unitOfWork.GetRepository<IServisRepository>();
                var servis = await servisRepo.GetByIdAsync(id);

                if (servis == null)
                    return ApiResponseDto<ServisResponseDto>
                        .ErrorResult("Servis bulunamadı");

                var existingServis = await servisRepo.GetByServisAdiAsync(request.ServisAdi);
                if (existingServis != null && existingServis.ServisId != id)
                    return ApiResponseDto<ServisResponseDto>
                        .ErrorResult("Bu isimde başka bir servis zaten mevcut");

                // Aktiflik durumu pasif yapılıyorsa personel kontrolü
                if (servis.Aktiflik == Aktiflik.Aktif && request.Aktiflik == Aktiflik.Pasif)
                {
                    var personelCount = await GetPersonelCountAsync(id);
                    if (personelCount.Data > 0)
                        return ApiResponseDto<ServisResponseDto>
                            .ErrorResult($"Bu serviste {personelCount.Data} personel bulunmaktadır. Önce personelleri başka servise taşıyınız");
                }

                _mapper.Map(request, servis);
                servisRepo.Update(servis);
                await _unitOfWork.SaveChangesAsync();

                var servisDto = _mapper.Map<ServisResponseDto>(servis);
                _logger.LogInformation("Servis güncellendi. ID: {Id}", id);

                return ApiResponseDto<ServisResponseDto>
                    .SuccessResult(servisDto, "Servis başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Servis güncellenirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<ServisResponseDto>
                    .ErrorResult("Servis güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                var servisRepo = _unitOfWork.GetRepository<IServisRepository>();

                // Serviste personel var mı kontrol et
                var personelCount = await GetPersonelCountAsync(id);
                if (personelCount.Data > 0)
                    return ApiResponseDto<bool>
                        .ErrorResult($"Bu serviste {personelCount.Data} personel bulunmaktadır. Önce personelleri başka servise taşıyınız");

                var servis = await servisRepo.GetByIdAsync(id);
                if (servis == null)
                    return ApiResponseDto<bool>
                        .ErrorResult("Servis bulunamadı");

                servisRepo.Delete(servis);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Servis silindi. ID: {Id}", id);
                return ApiResponseDto<bool>
                    .SuccessResult(true, "Servis başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Servis silinirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<bool>
                    .ErrorResult("Servis silinirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<ServisResponseDto>>> GetActiveAsync()
        {
            try
            {
                var servisRepo = _unitOfWork.GetRepository<IServisRepository>();
                var servisler = await servisRepo.GetActiveAsync();
                var servisDtos = _mapper.Map<List<ServisResponseDto>>(servisler);

                return ApiResponseDto<List<ServisResponseDto>>
                    .SuccessResult(servisDtos, "Aktif servisler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif servisler getirilirken hata oluştu");
                return ApiResponseDto<List<ServisResponseDto>>
                    .ErrorResult("Aktif servisler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<ServisResponseDto>> GetByNameAsync(string servisAdi)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(servisAdi))
                    return ApiResponseDto<ServisResponseDto>
                        .ErrorResult("Servis adı boş olamaz");

                var servisRepo = _unitOfWork.GetRepository<IServisRepository>();
                var servis = await servisRepo.GetByServisAdiAsync(servisAdi);

                if (servis == null)
                    return ApiResponseDto<ServisResponseDto>
                        .ErrorResult("Servis bulunamadı");

                var servisDto = _mapper.Map<ServisResponseDto>(servis);
                return ApiResponseDto<ServisResponseDto>
                    .SuccessResult(servisDto, "Servis başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Servis isimle getirilirken hata oluştu. Ad: {Ad}", servisAdi);
                return ApiResponseDto<ServisResponseDto>
                    .ErrorResult("Servis getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<PagedResponseDto<ServisResponseDto>>> GetPagedAsync(ServisFilterRequestDto filter)
        {
            try
            {
                var servisRepo = _unitOfWork.Repository<Servis>();

                var (items, totalCount) = await servisRepo.GetPagedAsync(
                    pageNumber: filter.PageNumber,
                    pageSize: filter.PageSize,
                    filter: s =>
                        (string.IsNullOrEmpty(filter.SearchTerm) || s.ServisAdi.Contains(filter.SearchTerm)) &&
                        (!filter.Aktiflik.HasValue || s.Aktiflik == filter.Aktiflik),
                    orderBy: s => s.ServisAdi,
                    ascending: filter.OrderDirection == "asc"
                );

                var pagedResponse = new PagedResponseDto<ServisResponseDto>
                {
                    Items = _mapper.Map<List<ServisResponseDto>>(items),
                    TotalCount = totalCount,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize
                };

                return ApiResponseDto<PagedResponseDto<ServisResponseDto>>
                    .SuccessResult(pagedResponse, "Sayfalı servis listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sayfalı servis listesi getirilirken hata oluştu");
                return ApiResponseDto<PagedResponseDto<ServisResponseDto>>
                    .ErrorResult("Servisler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<int>> GetPersonelCountAsync(int servisId)
        {
            try
            {
                if (servisId <= 0)
                    return ApiResponseDto<int>
                        .ErrorResult("Geçersiz servis ID");

                // Serviste aktif personel sayısı hesapla
                var count = await _unitOfWork.Repository<Personel>()
                    .CountAsync(p => p.ServisId == servisId && !p.SilindiMi && p.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif);

                return ApiResponseDto<int>
                    .SuccessResult(count, "Personel sayısı başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel sayısı getirilirken hata oluştu. ServisId: {Id}", servisId);
                return ApiResponseDto<int>
                    .ErrorResult("Personel sayısı getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<(int Id, string Ad)>>> GetDropdownAsync()
        {
            try
            {
                var servisRepo = _unitOfWork.GetRepository<IServisRepository>();
                var dropdown = await servisRepo.GetDropdownAsync();
                var dropdownList = dropdown.ToList();

                return ApiResponseDto<List<(int Id, string Ad)>>
                    .SuccessResult(dropdownList, "Dropdown listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dropdown listesi getirilirken hata oluştu");
                return ApiResponseDto<List<(int Id, string Ad)>>
                    .ErrorResult("Dropdown listesi getirilirken bir hata oluştu", ex.Message);
            }
        }
    }
}