using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.Common.Interfaces.Permission;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;
using SGKPortalApp.Common.Helpers;

namespace SGKPortalApp.BusinessLogicLayer.Services.Common
{
    public class IlceService : IIlceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<IlceService> _logger;
        private readonly IFieldPermissionValidationService _fieldPermissionService;
        private readonly IPermissionKeyResolverService _permissionKeyResolver;

        public IlceService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<IlceService> logger,
            IFieldPermissionValidationService fieldPermissionService,
            IPermissionKeyResolverService permissionKeyResolver)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _fieldPermissionService = fieldPermissionService;
            _permissionKeyResolver = permissionKeyResolver;
        }

        public async Task<ApiResponseDto<List<IlceResponseDto>>> GetAllAsync()
        {
            try
            {
                var ilceRepo = _unitOfWork.GetRepository<IIlceRepository>();
                var ilceler = await ilceRepo.GetAllAsync();
                var ilceDtos = _mapper.Map<List<IlceResponseDto>>(ilceler);

                return ApiResponseDto<List<IlceResponseDto>>
                    .SuccessResult(ilceDtos, "İlçeler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İlçe listesi getirilirken hata oluştu");
                return ApiResponseDto<List<IlceResponseDto>>
                    .ErrorResult("İlçeler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<IlceResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<IlceResponseDto>
                        .ErrorResult("Geçersiz ilçe ID");

                var ilceRepo = _unitOfWork.GetRepository<IIlceRepository>();
                var ilce = await ilceRepo.GetWithIlAsync(id);

                if (ilce == null)
                    return ApiResponseDto<IlceResponseDto>
                        .ErrorResult("İlçe bulunamadı");

                var ilceDto = _mapper.Map<IlceResponseDto>(ilce);
                return ApiResponseDto<IlceResponseDto>
                    .SuccessResult(ilceDto, "İlçe başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İlçe getirilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<IlceResponseDto>
                    .ErrorResult("İlçe getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<IlceResponseDto>> CreateAsync(IlceCreateRequestDto request)
        {
            try
            {
                // İl kontrolü
                var ilRepo = _unitOfWork.GetRepository<IIlRepository>();
                var il = await ilRepo.GetByIdAsync(request.IlId);
                if (il == null)
                    return ApiResponseDto<IlceResponseDto>
                        .ErrorResult("Seçilen il bulunamadı");

                var ilceRepo = _unitOfWork.GetRepository<IIlceRepository>();
                var existingIlce = await ilceRepo.GetByIlceAdiAsync(request.IlceAdi);

                if (existingIlce != null)
                    return ApiResponseDto<IlceResponseDto>
                        .ErrorResult("Bu isimde bir ilçe zaten mevcut");

                var ilce = _mapper.Map<Ilce>(request);
                await ilceRepo.AddAsync(ilce);
                await _unitOfWork.SaveChangesAsync();

                // İl bilgisi ile birlikte getir
                ilce = await ilceRepo.GetWithIlAsync(ilce.IlceId);
                var ilceDto = _mapper.Map<IlceResponseDto>(ilce);
                _logger.LogInformation("Yeni ilçe oluşturuldu. ID: {Id}", ilce!.IlceId);

                return ApiResponseDto<IlceResponseDto>
                    .SuccessResult(ilceDto, "İlçe başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İlçe oluşturulurken hata oluştu");
                return ApiResponseDto<IlceResponseDto>
                    .ErrorResult("İlçe oluşturulurken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<IlceResponseDto>> UpdateAsync(int id, IlceUpdateRequestDto request)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<IlceResponseDto>
                        .ErrorResult("Geçersiz ilçe ID");

                // İl kontrolü
                var ilRepo = _unitOfWork.GetRepository<IIlRepository>();
                var il = await ilRepo.GetByIdAsync(request.IlId);
                if (il == null)
                    return ApiResponseDto<IlceResponseDto>
                        .ErrorResult("Seçilen il bulunamadı");

                var ilceRepo = _unitOfWork.GetRepository<IIlceRepository>();
                var ilce = await ilceRepo.GetByIdAsync(id);

                if (ilce == null)
                    return ApiResponseDto<IlceResponseDto>
                        .ErrorResult("İlçe bulunamadı");

                var existingIlce = await ilceRepo.GetByIlceAdiAsync(request.IlceAdi);
                if (existingIlce != null && existingIlce.IlceId != id)
                    return ApiResponseDto<IlceResponseDto>
                        .ErrorResult("Bu isimde başka bir ilçe zaten mevcut");

                // ⭐ Field-level permission enforcement
                // Permission key otomatik çözümleme (route → permission key)
                var permissionKey = _permissionKeyResolver.ResolveFromCurrentRequest() ?? "UNKNOWN";
                var userPermissions = new Dictionary<string, BusinessObjectLayer.Enums.Common.YetkiSeviyesi>();
                var originalDto = _mapper.Map<IlceUpdateRequestDto>(ilce);

                var unauthorizedFields = await _fieldPermissionService.ValidateFieldPermissionsAsync(
                    request,
                    userPermissions,
                    originalDto,
                    permissionKey,
                    null);

                if (unauthorizedFields.Any())
                {
                    _fieldPermissionService.RevertUnauthorizedFields(request, originalDto, unauthorizedFields);
                    _logger.LogWarning(
                        "IlceService.UpdateAsync - Field-level permission enforcement: {Count} alan revert edildi.",
                        unauthorizedFields.Count);
                }

                ilce.IlId = request.IlId;
                ilce.IlceAdi = request.IlceAdi;
                ilce.DuzenlenmeTarihi = DateTimeHelper.Now;

                ilceRepo.Update(ilce);
                await _unitOfWork.SaveChangesAsync();

                // İl bilgisi ile birlikte getir
                ilce = await ilceRepo.GetWithIlAsync(id);
                var ilceDto = _mapper.Map<IlceResponseDto>(ilce);
                _logger.LogInformation("İlçe güncellendi. ID: {Id}", id);

                return ApiResponseDto<IlceResponseDto>
                    .SuccessResult(ilceDto, "İlçe başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İlçe güncellenirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<IlceResponseDto>
                    .ErrorResult("İlçe güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<bool>
                        .ErrorResult("Geçersiz ilçe ID");

                var ilceRepo = _unitOfWork.GetRepository<IIlceRepository>();
                var ilce = await ilceRepo.GetByIdAsync(id);

                if (ilce == null)
                    return ApiResponseDto<bool>
                        .ErrorResult("İlçe bulunamadı");

                ilceRepo.Delete(ilce);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("İlçe silindi. ID: {Id}", id);
                return ApiResponseDto<bool>
                    .SuccessResult(true, "İlçe başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İlçe silinirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<bool>
                    .ErrorResult("İlçe silinirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<IlceResponseDto>>> GetByIlAsync(int ilId)
        {
            try
            {
                var ilceRepo = _unitOfWork.GetRepository<IIlceRepository>();
                var ilceler = await ilceRepo.GetByIlAsync(ilId);
                var ilceDtos = _mapper.Map<List<IlceResponseDto>>(ilceler);

                return ApiResponseDto<List<IlceResponseDto>>
                    .SuccessResult(ilceDtos, "İlçeler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İl bazlı ilçe listesi getirilirken hata oluştu");
                return ApiResponseDto<List<IlceResponseDto>>
                    .ErrorResult("İlçeler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<IlceResponseDto>> GetByNameAsync(string ilceAdi)
        {
            try
            {
                var ilceRepo = _unitOfWork.GetRepository<IIlceRepository>();
                var ilce = await ilceRepo.GetByIlceAdiAsync(ilceAdi);

                if (ilce == null)
                    return ApiResponseDto<IlceResponseDto>
                        .ErrorResult("İlçe bulunamadı");

                ilce = await ilceRepo.GetWithIlAsync(ilce.IlceId);
                var ilceDto = _mapper.Map<IlceResponseDto>(ilce);

                return ApiResponseDto<IlceResponseDto>
                    .SuccessResult(ilceDto, "İlçe başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İlçe getirilirken hata oluştu");
                return ApiResponseDto<IlceResponseDto>
                    .ErrorResult("İlçe getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<PagedResponseDto<IlceResponseDto>>> GetPagedAsync(IlceFilterRequestDto filter)
        {
            try
            {
                var ilceRepo = _unitOfWork.GetRepository<IIlceRepository>();
                var ilceler = await ilceRepo.GetAllAsync();

                // Filtreleme
                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    ilceler = ilceler.Where(i => i.IlceAdi.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase));
                }

                if (filter.IlId.HasValue && filter.IlId.Value > 0)
                {
                    ilceler = ilceler.Where(i => i.IlId == filter.IlId.Value);
                }

                // Sıralama
                ilceler = filter.OrderBy.ToLower() switch
                {
                    "ilceadi" => filter.OrderDirection == "desc" 
                        ? ilceler.OrderByDescending(i => i.IlceAdi) 
                        : ilceler.OrderBy(i => i.IlceAdi),
                    "iladi" => filter.OrderDirection == "desc" 
                        ? ilceler.OrderByDescending(i => i.Il.IlAdi) 
                        : ilceler.OrderBy(i => i.Il.IlAdi),
                    "eklenmetarihi" => filter.OrderDirection == "desc" 
                        ? ilceler.OrderByDescending(i => i.EklenmeTarihi) 
                        : ilceler.OrderBy(i => i.EklenmeTarihi),
                    _ => ilceler.OrderBy(i => i.Il.IlAdi).ThenBy(i => i.IlceAdi)
                };

                var totalCount = ilceler.Count();
                var pagedData = ilceler
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToList();

                var ilceDtos = _mapper.Map<List<IlceResponseDto>>(pagedData);

                var pagedResult = new PagedResponseDto<IlceResponseDto>
                {
                    Items = ilceDtos,
                    TotalCount = totalCount,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize
                };

                return ApiResponseDto<PagedResponseDto<IlceResponseDto>>
                    .SuccessResult(pagedResult, "İlçeler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sayfalı ilçe listesi getirilirken hata oluştu");
                return ApiResponseDto<PagedResponseDto<IlceResponseDto>>
                    .ErrorResult("İlçeler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<(int Id, string Ad)>>> GetDropdownAsync()
        {
            try
            {
                var ilceRepo = _unitOfWork.GetRepository<IIlceRepository>();
                var dropdown = await ilceRepo.GetDropdownAsync();

                return ApiResponseDto<List<(int Id, string Ad)>>
                    .SuccessResult(dropdown.ToList(), "İlçe dropdown listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İlçe dropdown listesi getirilirken hata oluştu");
                return ApiResponseDto<List<(int Id, string Ad)>>
                    .ErrorResult("İlçe dropdown listesi getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<(int Id, string Ad)>>> GetByIlDropdownAsync(int ilId)
        {
            try
            {
                var ilceRepo = _unitOfWork.GetRepository<IIlceRepository>();
                var dropdown = await ilceRepo.GetByIlDropdownAsync(ilId);

                return ApiResponseDto<List<(int Id, string Ad)>>
                    .SuccessResult(dropdown.ToList(), "İl bazlı ilçe dropdown listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İl bazlı ilçe dropdown listesi getirilirken hata oluştu");
                return ApiResponseDto<List<(int Id, string Ad)>>
                    .ErrorResult("İl bazlı ilçe dropdown listesi getirilirken bir hata oluştu", ex.Message);
            }
        }
    }
}
