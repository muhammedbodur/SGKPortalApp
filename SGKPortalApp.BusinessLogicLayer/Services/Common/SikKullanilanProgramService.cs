using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.Common.Helpers;
using SGKPortalApp.Common.Interfaces.Permission;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;

namespace SGKPortalApp.BusinessLogicLayer.Services.Common
{
    public class SikKullanilanProgramService : ISikKullanilanProgramService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SikKullanilanProgramService> _logger;
        private readonly IFieldPermissionValidationService _fieldPermissionService;
        private readonly IPermissionKeyResolverService _permissionKeyResolver;

        public SikKullanilanProgramService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<SikKullanilanProgramService> logger,
            IFieldPermissionValidationService fieldPermissionService,
            IPermissionKeyResolverService permissionKeyResolver)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _fieldPermissionService = fieldPermissionService;
            _permissionKeyResolver = permissionKeyResolver;
        }

        public async Task<ApiResponseDto<List<SikKullanilanProgramResponseDto>>> GetAllAsync()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<ISikKullanilanProgramRepository>();
                var items = await repo.GetAllAsync();
                var ordered = items.OrderBy(x => x.Sira).ThenBy(x => x.ProgramAdi).ToList();
                var dtos = _mapper.Map<List<SikKullanilanProgramResponseDto>>(ordered);
                return ApiResponseDto<List<SikKullanilanProgramResponseDto>>.SuccessResult(dtos, "Programlar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Program listesi getirilirken hata oluştu");
                return ApiResponseDto<List<SikKullanilanProgramResponseDto>>.ErrorResult("Program listesi getirilirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<SikKullanilanProgramResponseDto>>> GetActiveAsync()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<ISikKullanilanProgramRepository>();
                var items = await repo.GetActiveProgramsAsync();
                var dtos = _mapper.Map<List<SikKullanilanProgramResponseDto>>(items.ToList());
                return ApiResponseDto<List<SikKullanilanProgramResponseDto>>.SuccessResult(dtos, "Aktif programlar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif programlar getirilirken hata oluştu");
                return ApiResponseDto<List<SikKullanilanProgramResponseDto>>.ErrorResult("Aktif programlar getirilirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<SikKullanilanProgramResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<SikKullanilanProgramResponseDto>.ErrorResult("Geçersiz program ID");

                var repo = _unitOfWork.GetRepository<ISikKullanilanProgramRepository>();
                var entity = await repo.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponseDto<SikKullanilanProgramResponseDto>.ErrorResult("Program bulunamadı");

                var dto = _mapper.Map<SikKullanilanProgramResponseDto>(entity);
                return ApiResponseDto<SikKullanilanProgramResponseDto>.SuccessResult(dto, "Program başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Program getirilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<SikKullanilanProgramResponseDto>.ErrorResult("Program getirilirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<SikKullanilanProgramResponseDto>> CreateAsync(SikKullanilanProgramCreateRequestDto request)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<ISikKullanilanProgramRepository>();

                var entity = _mapper.Map<SikKullanilanProgram>(request);
                await repo.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                var dto = _mapper.Map<SikKullanilanProgramResponseDto>(entity);
                return ApiResponseDto<SikKullanilanProgramResponseDto>.SuccessResult(dto, "Program başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Program oluşturulurken hata oluştu");
                return ApiResponseDto<SikKullanilanProgramResponseDto>.ErrorResult("Program oluşturulurken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<SikKullanilanProgramResponseDto>> UpdateAsync(int id, SikKullanilanProgramUpdateRequestDto request)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<SikKullanilanProgramResponseDto>.ErrorResult("Geçersiz program ID");

                var repo = _unitOfWork.GetRepository<ISikKullanilanProgramRepository>();
                var entity = await repo.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponseDto<SikKullanilanProgramResponseDto>.ErrorResult("Program bulunamadı");

                var permissionKey = _permissionKeyResolver.ResolveFromCurrentRequest() ?? "UNKNOWN";
                var userPermissions = new Dictionary<string, YetkiSeviyesi>();
                var originalDto = _mapper.Map<SikKullanilanProgramUpdateRequestDto>(entity);

                var unauthorizedFields = await _fieldPermissionService.ValidateFieldPermissionsAsync(
                    request,
                    userPermissions,
                    originalDto,
                    permissionKey,
                    null);

                if (unauthorizedFields.Any())
                {
                    _fieldPermissionService.RevertUnauthorizedFields(request, originalDto, unauthorizedFields);
                    _logger.LogWarning("SikKullanilanProgramService.UpdateAsync - Field-level permission enforcement: {Count} alan revert edildi.", unauthorizedFields.Count);
                }

                entity.ProgramAdi = request.ProgramAdi;
                entity.Url = request.Url;
                entity.IkonClass = request.IkonClass;
                entity.RenkKodu = request.RenkKodu;
                entity.Sira = request.Sira;
                entity.Aktiflik = request.Aktiflik;
                entity.DuzenlenmeTarihi = DateTimeHelper.Now;

                repo.Update(entity);
                await _unitOfWork.SaveChangesAsync();

                var dto = _mapper.Map<SikKullanilanProgramResponseDto>(entity);
                return ApiResponseDto<SikKullanilanProgramResponseDto>.SuccessResult(dto, "Program başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Program güncellenirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<SikKullanilanProgramResponseDto>.ErrorResult("Program güncellenirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<bool>.ErrorResult("Geçersiz program ID");

                var repo = _unitOfWork.GetRepository<ISikKullanilanProgramRepository>();
                var entity = await repo.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponseDto<bool>.ErrorResult("Program bulunamadı");

                repo.Delete(entity);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponseDto<bool>.SuccessResult(true, "Program başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Program silinirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<bool>.ErrorResult("Program silinirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<PagedResponseDto<SikKullanilanProgramResponseDto>>> GetPagedAsync(SikKullanilanProgramFilterRequestDto filter)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<ISikKullanilanProgramRepository>();
                var items = await repo.GetAllAsync();

                var query = items.AsQueryable();

                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    query = query.Where(x => x.ProgramAdi.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase));
                }

                if (filter.Aktiflik.HasValue)
                {
                    query = query.Where(x => x.Aktiflik == filter.Aktiflik.Value);
                }

                query = filter.OrderBy.ToLower() switch
                {
                    "programadi" => filter.OrderDirection == "desc"
                        ? query.OrderByDescending(x => x.ProgramAdi)
                        : query.OrderBy(x => x.ProgramAdi),
                    "sira" => filter.OrderDirection == "desc"
                        ? query.OrderByDescending(x => x.Sira)
                        : query.OrderBy(x => x.Sira),
                    "aktiflik" => filter.OrderDirection == "desc"
                        ? query.OrderByDescending(x => x.Aktiflik)
                        : query.OrderBy(x => x.Aktiflik),
                    _ => query.OrderBy(x => x.Sira)
                };

                var totalCount = query.Count();
                var pagedItems = query
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToList();

                var dtos = _mapper.Map<List<SikKullanilanProgramResponseDto>>(pagedItems);

                var paged = new PagedResponseDto<SikKullanilanProgramResponseDto>
                {
                    Items = dtos,
                    TotalCount = totalCount,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize
                };

                return ApiResponseDto<PagedResponseDto<SikKullanilanProgramResponseDto>>.SuccessResult(paged, "Programlar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sayfalı program listesi getirilirken hata oluştu");
                return ApiResponseDto<PagedResponseDto<SikKullanilanProgramResponseDto>>.ErrorResult("Programlar getirilirken hata oluştu", ex.Message);
            }
        }
    }
}
