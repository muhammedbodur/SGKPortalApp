using AutoMapper;
using Microsoft.Extensions.Logging;
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
    public class DepartmanService : IDepartmanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DepartmanService> _logger;

        public DepartmanService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<DepartmanService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<DepartmanResponseDto>>> GetAllAsync()
        {
            try
            {
                var departmanRepo = _unitOfWork.GetRepository<IDepartmanRepository>();
                var departmanlar = await departmanRepo.GetActiveAsync();
                var departmanDtos = _mapper.Map<List<DepartmanResponseDto>>(departmanlar);

                return ApiResponseDto<List<DepartmanResponseDto>>
                    .SuccessResult(departmanDtos, "Departmanlar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Departman listesi getirilirken hata oluştu");
                return ApiResponseDto<List<DepartmanResponseDto>>
                    .ErrorResult("Departmanlar getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<DepartmanResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<DepartmanResponseDto>
                        .ErrorResult("Geçersiz departman ID");

                var departmanRepo = _unitOfWork.GetRepository<IDepartmanRepository>();
                var departman = await departmanRepo.GetByIdAsync(id);

                if (departman == null)
                    return ApiResponseDto<DepartmanResponseDto>
                        .ErrorResult("Departman bulunamadı");

                var departmanDto = _mapper.Map<DepartmanResponseDto>(departman);
                return ApiResponseDto<DepartmanResponseDto>
                    .SuccessResult(departmanDto, "Departman başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Departman getirilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<DepartmanResponseDto>
                    .ErrorResult("Departman getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<DepartmanResponseDto>> CreateAsync(DepartmanCreateRequestDto request)
        {
            try
            {
                var departmanRepo = _unitOfWork.GetRepository<IDepartmanRepository>();
                var existingDepartman = await departmanRepo.GetByDepartmanAdiAsync(request.DepartmanAdi);

                if (existingDepartman != null)
                    return ApiResponseDto<DepartmanResponseDto>
                        .ErrorResult("Bu isimde bir departman zaten mevcut");

                var departman = _mapper.Map<Departman>(request);
                await departmanRepo.AddAsync(departman);
                await _unitOfWork.SaveChangesAsync();

                var departmanDto = _mapper.Map<DepartmanResponseDto>(departman);
                _logger.LogInformation("Yeni departman oluşturuldu. ID: {Id}", departman.DepartmanId);

                return ApiResponseDto<DepartmanResponseDto>
                    .SuccessResult(departmanDto, "Departman başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Departman oluşturulurken hata oluştu");
                return ApiResponseDto<DepartmanResponseDto>
                    .ErrorResult("Departman oluşturulurken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<DepartmanResponseDto>> UpdateAsync(int id, DepartmanUpdateRequestDto request)
        {
            try
            {
                var departmanRepo = _unitOfWork.GetRepository<IDepartmanRepository>();
                var departman = await departmanRepo.GetByIdAsync(id);

                if (departman == null)
                    return ApiResponseDto<DepartmanResponseDto>
                        .ErrorResult("Departman bulunamadı");

                var existingDepartman = await departmanRepo.GetByDepartmanAdiAsync(request.DepartmanAdi);
                if (existingDepartman != null && existingDepartman.DepartmanId != id)
                    return ApiResponseDto<DepartmanResponseDto>
                        .ErrorResult("Bu isimde başka bir departman zaten mevcut");

                // Aktiflik durumu pasif yapılıyorsa personel kontrolü
                if (departman.Aktiflik == Aktiflik.Aktif && request.Aktiflik == Aktiflik.Pasif)
                {
                    var personelCount = await departmanRepo.GetPersonelCountAsync(id);
                    if (personelCount > 0)
                        return ApiResponseDto<DepartmanResponseDto>
                            .ErrorResult($"Bu departmanda {personelCount} personel bulunmaktadır. Önce personelleri başka departmana taşıyınız");
                }

                _mapper.Map(request, departman);
                departmanRepo.Update(departman);
                await _unitOfWork.SaveChangesAsync();

                var departmanDto = _mapper.Map<DepartmanResponseDto>(departman);
                _logger.LogInformation("Departman güncellendi. ID: {Id}", id);

                return ApiResponseDto<DepartmanResponseDto>
                    .SuccessResult(departmanDto, "Departman başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Departman güncellenirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<DepartmanResponseDto>
                    .ErrorResult("Departman güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                var departmanRepo = _unitOfWork.GetRepository<IDepartmanRepository>();

                var personelCount = await departmanRepo.GetPersonelCountAsync(id);
                if (personelCount > 0)
                    return ApiResponseDto<bool>
                        .ErrorResult($"Bu departmanda {personelCount} personel bulunmaktadır. Önce personelleri başka departmana taşıyınız");

                var departman = await departmanRepo.GetByIdAsync(id);
                if (departman == null)
                    return ApiResponseDto<bool>
                        .ErrorResult("Departman bulunamadı");

                departmanRepo.Delete(departman);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Departman silindi. ID: {Id}", id);
                return ApiResponseDto<bool>
                    .SuccessResult(true, "Departman başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Departman silinirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<bool>
                    .ErrorResult("Departman silinirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<DepartmanResponseDto>>> GetActiveAsync()
        {
            try
            {
                var departmanRepo = _unitOfWork.GetRepository<IDepartmanRepository>();
                var departmanlar = await departmanRepo.GetActiveAsync();
                var departmanDtos = _mapper.Map<List<DepartmanResponseDto>>(departmanlar);

                return ApiResponseDto<List<DepartmanResponseDto>>
                    .SuccessResult(departmanDtos, "Aktif departmanlar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif departmanlar getirilirken hata oluştu");
                return ApiResponseDto<List<DepartmanResponseDto>>
                    .ErrorResult("Aktif departmanlar getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<DepartmanResponseDto>> GetByNameAsync(string departmanAdi)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(departmanAdi))
                    return ApiResponseDto<DepartmanResponseDto>
                        .ErrorResult("Departman adı boş olamaz");

                var departmanRepo = _unitOfWork.GetRepository<IDepartmanRepository>();
                var departman = await departmanRepo.GetByDepartmanAdiAsync(departmanAdi);

                if (departman == null)
                    return ApiResponseDto<DepartmanResponseDto>
                        .ErrorResult("Departman bulunamadı");

                var departmanDto = _mapper.Map<DepartmanResponseDto>(departman);
                return ApiResponseDto<DepartmanResponseDto>
                    .SuccessResult(departmanDto, "Departman başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Departman isimle getirilirken hata oluştu. Ad: {Ad}", departmanAdi);
                return ApiResponseDto<DepartmanResponseDto>
                    .ErrorResult("Departman getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<PagedResponseDto<DepartmanResponseDto>>> GetPagedAsync(DepartmanFilterRequestDto filter)
        {
            try
            {
                var departmanRepo = _unitOfWork.Repository<Departman>();

                var (items, totalCount) = await departmanRepo.GetPagedAsync(
                    pageNumber: filter.PageNumber,
                    pageSize: filter.PageSize,
                    filter: d =>
                        (string.IsNullOrEmpty(filter.SearchTerm) || d.DepartmanAdi.Contains(filter.SearchTerm)) &&
                        (!filter.Aktiflik.HasValue || d.Aktiflik == filter.Aktiflik),
                    orderBy: d => d.DepartmanAdi,
                    ascending: filter.OrderDirection == "asc"
                );

                var pagedResponse = new PagedResponseDto<DepartmanResponseDto>
                {
                    Items = _mapper.Map<List<DepartmanResponseDto>>(items),
                    TotalCount = totalCount,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize
                };

                return ApiResponseDto<PagedResponseDto<DepartmanResponseDto>>
                    .SuccessResult(pagedResponse, "Sayfalı departman listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sayfalı departman listesi getirilirken hata oluştu");
                return ApiResponseDto<PagedResponseDto<DepartmanResponseDto>>
                    .ErrorResult("Departmanlar getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<int>> GetPersonelCountAsync(int departmanId)
        {
            try
            {
                if (departmanId <= 0)
                    return ApiResponseDto<int>
                        .ErrorResult("Geçersiz departman ID");

                var departmanRepo = _unitOfWork.GetRepository<IDepartmanRepository>();
                var count = await departmanRepo.GetPersonelCountAsync(departmanId);

                return ApiResponseDto<int>
                    .SuccessResult(count, "Personel sayısı başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel sayısı getirilirken hata oluştu. DepartmanId: {Id}", departmanId);
                return ApiResponseDto<int>
                    .ErrorResult("Personel sayısı getirilirken bir hata oluştu", ex.Message);
            }
        }

        // ✅ YENİ EKLENEN METOD
        public async Task<ApiResponseDto<List<(int Id, string Ad)>>> GetDropdownAsync()
        {
            try
            {
                var departmanRepo = _unitOfWork.GetRepository<IDepartmanRepository>();
                var dropdown = await departmanRepo.GetDropdownAsync();
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