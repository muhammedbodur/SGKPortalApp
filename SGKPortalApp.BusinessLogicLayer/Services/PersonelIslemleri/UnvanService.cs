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
    public class UnvanService : IUnvanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UnvanService> _logger;

        public UnvanService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UnvanService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<UnvanResponseDto>>> GetAllAsync()
        {
            try
            {
                var unvanRepo = _unitOfWork.GetRepository<IUnvanRepository>();
                var unvanlar = await unvanRepo.GetActiveAsync();
                var unvanDtos = _mapper.Map<List<UnvanResponseDto>>(unvanlar);

                return ApiResponseDto<List<UnvanResponseDto>>
                    .SuccessResult(unvanDtos, "Unvanlar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unvan listesi getirilirken hata oluştu");
                return ApiResponseDto<List<UnvanResponseDto>>
                    .ErrorResult("Unvanlar getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<UnvanResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<UnvanResponseDto>
                        .ErrorResult("Geçersiz unvan ID");

                var unvanRepo = _unitOfWork.GetRepository<IUnvanRepository>();
                var unvan = await unvanRepo.GetByIdAsync(id);

                if (unvan == null)
                    return ApiResponseDto<UnvanResponseDto>
                        .ErrorResult("Unvan bulunamadı");

                var unvanDto = _mapper.Map<UnvanResponseDto>(unvan);
                return ApiResponseDto<UnvanResponseDto>
                    .SuccessResult(unvanDto, "Unvan başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unvan getirilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<UnvanResponseDto>
                    .ErrorResult("Unvan getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<UnvanResponseDto>> CreateAsync(UnvanCreateRequestDto request)
        {
            try
            {
                var unvanRepo = _unitOfWork.GetRepository<IUnvanRepository>();
                var existingUnvan = await unvanRepo.GetByUnvanAdiAsync(request.UnvanAdi);

                if (existingUnvan != null)
                    return ApiResponseDto<UnvanResponseDto>
                        .ErrorResult("Bu isimde bir unvan zaten mevcut");

                var unvan = _mapper.Map<Unvan>(request);
                await unvanRepo.AddAsync(unvan);
                await _unitOfWork.SaveChangesAsync();

                var unvanDto = _mapper.Map<UnvanResponseDto>(unvan);
                _logger.LogInformation("Yeni unvan oluşturuldu. ID: {Id}", unvan.UnvanId);

                return ApiResponseDto<UnvanResponseDto>
                    .SuccessResult(unvanDto, "Unvan başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unvan oluşturulurken hata oluştu");
                return ApiResponseDto<UnvanResponseDto>
                    .ErrorResult("Unvan oluşturulurken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<UnvanResponseDto>> UpdateAsync(int id, UnvanUpdateRequestDto request)
        {
            try
            {
                var unvanRepo = _unitOfWork.GetRepository<IUnvanRepository>();
                var unvan = await unvanRepo.GetByIdAsync(id);

                if (unvan == null)
                    return ApiResponseDto<UnvanResponseDto>
                        .ErrorResult("Unvan bulunamadı");

                var existingUnvan = await unvanRepo.GetByUnvanAdiAsync(request.UnvanAdi);
                if (existingUnvan != null && existingUnvan.UnvanId != id)
                    return ApiResponseDto<UnvanResponseDto>
                        .ErrorResult("Bu isimde başka bir unvan zaten mevcut");

                // Aktiflik durumu pasif yapılıyorsa personel kontrolü
                _logger.LogInformation("Unvan UpdateAsync - ID: {Id}, Mevcut Aktiflik: {CurrentAktiflik}, İstenen Aktiflik: {RequestAktiflik}",
                    id, unvan.Aktiflik, request.Aktiflik);

                if (unvan.Aktiflik == Aktiflik.Aktif && request.Aktiflik == Aktiflik.Pasif)
                {
                    _logger.LogInformation("Aktiflik kontrolü yapılıyor - UnvanId: {Id}", id);
                    var personelCount = await GetPersonelCountAsync(id);
                    _logger.LogInformation("Personel sayısı kontrolü - UnvanId: {Id}, Success: {Success}, Count: {Count}",
                        id, personelCount.Success, personelCount.Data);

                    if (!personelCount.Success)
                        return ApiResponseDto<UnvanResponseDto>
                            .ErrorResult(personelCount.Message);

                    if (personelCount.Data > 0)
                        return ApiResponseDto<UnvanResponseDto>
                            .ErrorResult($"Bu unvanda {personelCount.Data} personel bulunmaktadır. Önce personelleri başka unvana taşıyınız");
                }
                else
                {
                    _logger.LogInformation("Aktiflik kontrolü atlandı - Koşul sağlanmadı");
                }

                _mapper.Map(request, unvan);
                unvanRepo.Update(unvan);
                await _unitOfWork.SaveChangesAsync();

                var unvanDto = _mapper.Map<UnvanResponseDto>(unvan);
                _logger.LogInformation("Unvan güncellendi. ID: {Id}", id);

                return ApiResponseDto<UnvanResponseDto>
                    .SuccessResult(unvanDto, "Unvan başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unvan güncellenirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<UnvanResponseDto>
                    .ErrorResult("Unvan güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                var unvanRepo = _unitOfWork.GetRepository<IUnvanRepository>();

                // Unvanda personel var mı kontrol et
                var personelCount = await GetPersonelCountAsync(id);
                if (!personelCount.Success)
                    return ApiResponseDto<bool>
                        .ErrorResult(personelCount.Message);

                if (personelCount.Data > 0)
                    return ApiResponseDto<bool>
                        .ErrorResult($"Bu unvanda {personelCount.Data} personel bulunmaktadır. Önce personelleri başka unvana taşıyınız");

                var unvan = await unvanRepo.GetByIdAsync(id);
                if (unvan == null)
                    return ApiResponseDto<bool>
                        .ErrorResult("Unvan bulunamadı");

                unvanRepo.Delete(unvan);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Unvan silindi. ID: {Id}", id);
                return ApiResponseDto<bool>
                    .SuccessResult(true, "Unvan başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unvan silinirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<bool>
                    .ErrorResult("Unvan silinirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<UnvanResponseDto>>> GetActiveAsync()
        {
            try
            {
                var unvanRepo = _unitOfWork.GetRepository<IUnvanRepository>();
                var unvanlar = await unvanRepo.GetActiveAsync();
                var unvanDtos = _mapper.Map<List<UnvanResponseDto>>(unvanlar);

                return ApiResponseDto<List<UnvanResponseDto>>
                    .SuccessResult(unvanDtos, "Aktif unvanlar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif unvanlar getirilirken hata oluştu");
                return ApiResponseDto<List<UnvanResponseDto>>
                    .ErrorResult("Aktif unvanlar getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<UnvanResponseDto>> GetByNameAsync(string unvanAdi)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(unvanAdi))
                    return ApiResponseDto<UnvanResponseDto>
                        .ErrorResult("Unvan adı boş olamaz");

                var unvanRepo = _unitOfWork.GetRepository<IUnvanRepository>();
                var unvan = await unvanRepo.GetByUnvanAdiAsync(unvanAdi);

                if (unvan == null)
                    return ApiResponseDto<UnvanResponseDto>
                        .ErrorResult("Unvan bulunamadı");

                var unvanDto = _mapper.Map<UnvanResponseDto>(unvan);
                return ApiResponseDto<UnvanResponseDto>
                    .SuccessResult(unvanDto, "Unvan başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unvan isimle getirilirken hata oluştu. Ad: {Ad}", unvanAdi);
                return ApiResponseDto<UnvanResponseDto>
                    .ErrorResult("Unvan getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<PagedResponseDto<UnvanResponseDto>>> GetPagedAsync(UnvanFilterRequestDto filter)
        {
            try
            {
                var unvanRepo = _unitOfWork.Repository<Unvan>();

                var (items, totalCount) = await unvanRepo.GetPagedAsync(
                    pageNumber: filter.PageNumber,
                    pageSize: filter.PageSize,
                    filter: u =>
                        (string.IsNullOrEmpty(filter.SearchTerm) || u.UnvanAdi.Contains(filter.SearchTerm)) &&
                        (!filter.Aktiflik.HasValue || u.Aktiflik == filter.Aktiflik),
                    orderBy: u => u.UnvanAdi,
                    ascending: filter.OrderDirection == "asc"
                );

                var pagedResponse = new PagedResponseDto<UnvanResponseDto>
                {
                    Items = _mapper.Map<List<UnvanResponseDto>>(items),
                    TotalCount = totalCount,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize
                };

                return ApiResponseDto<PagedResponseDto<UnvanResponseDto>>
                    .SuccessResult(pagedResponse, "Sayfalı unvan listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sayfalı unvan listesi getirilirken hata oluştu");
                return ApiResponseDto<PagedResponseDto<UnvanResponseDto>>
                    .ErrorResult("Unvanlar getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<int>> GetPersonelCountAsync(int unvanId)
        {
            try
            {
                if (unvanId <= 0)
                    return ApiResponseDto<int>
                        .ErrorResult("Geçersiz unvan ID");

                // Unvanda aktif personel sayısı hesapla
                var count = await _unitOfWork.Repository<Personel>()
                    .CountAsync(p => p.UnvanId == unvanId && !p.SilindiMi && p.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif);

                return ApiResponseDto<int>
                    .SuccessResult(count, "Personel sayısı başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel sayısı getirilirken hata oluştu. UnvanId: {Id}", unvanId);
                return ApiResponseDto<int>
                    .ErrorResult("Personel sayısı getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<(int Id, string Ad)>>> GetDropdownAsync()
        {
            try
            {
                var unvanRepo = _unitOfWork.GetRepository<IUnvanRepository>();
                var dropdown = await unvanRepo.GetDropdownAsync();
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