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
using System;
using System.Collections.Generic;
using System.Linq;

namespace SGKPortalApp.BusinessLogicLayer.Services.Common
{
    public class OnemliLinkService : IOnemliLinkService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OnemliLinkService> _logger;
        private readonly IFieldPermissionValidationService _fieldPermissionService;
        private readonly IPermissionKeyResolverService _permissionKeyResolver;

        public OnemliLinkService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<OnemliLinkService> logger,
            IFieldPermissionValidationService fieldPermissionService,
            IPermissionKeyResolverService permissionKeyResolver)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _fieldPermissionService = fieldPermissionService;
            _permissionKeyResolver = permissionKeyResolver;
        }

        public async Task<ApiResponseDto<List<OnemliLinkResponseDto>>> GetAllAsync()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IOnemliLinkRepository>();
                var items = await repo.GetAllAsync();
                var ordered = items.OrderBy(x => x.Sira).ToList();
                var dtos = _mapper.Map<List<OnemliLinkResponseDto>>(ordered);
                return ApiResponseDto<List<OnemliLinkResponseDto>>.SuccessResult(dtos, "Önemli linkler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Önemli link listesi getirilirken hata oluştu");
                return ApiResponseDto<List<OnemliLinkResponseDto>>.ErrorResult("Önemli link listesi getirilirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<OnemliLinkResponseDto>>> GetActiveAsync()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IOnemliLinkRepository>();
                var items = await repo.GetActiveLinksAsync();
                var dtos = _mapper.Map<List<OnemliLinkResponseDto>>(items.ToList());
                return ApiResponseDto<List<OnemliLinkResponseDto>>.SuccessResult(dtos, "Aktif linkler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif linkler getirilirken hata oluştu");
                return ApiResponseDto<List<OnemliLinkResponseDto>>.ErrorResult("Aktif linkler getirilirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<OnemliLinkResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<OnemliLinkResponseDto>.ErrorResult("Geçersiz link ID");

                var repo = _unitOfWork.GetRepository<IOnemliLinkRepository>();
                var entity = await repo.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponseDto<OnemliLinkResponseDto>.ErrorResult("Link bulunamadı");

                var dto = _mapper.Map<OnemliLinkResponseDto>(entity);
                return ApiResponseDto<OnemliLinkResponseDto>.SuccessResult(dto, "Link başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Link getirilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<OnemliLinkResponseDto>.ErrorResult("Link getirilirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<OnemliLinkResponseDto>> CreateAsync(OnemliLinkCreateRequestDto request)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IOnemliLinkRepository>();

                var entity = _mapper.Map<OnemliLink>(request);
                await repo.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                var dto = _mapper.Map<OnemliLinkResponseDto>(entity);
                return ApiResponseDto<OnemliLinkResponseDto>.SuccessResult(dto, "Link başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Link oluşturulurken hata oluştu");
                return ApiResponseDto<OnemliLinkResponseDto>.ErrorResult("Link oluşturulurken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<OnemliLinkResponseDto>> UpdateAsync(int id, OnemliLinkUpdateRequestDto request)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<OnemliLinkResponseDto>.ErrorResult("Geçersiz link ID");

                var repo = _unitOfWork.GetRepository<IOnemliLinkRepository>();
                var entity = await repo.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponseDto<OnemliLinkResponseDto>.ErrorResult("Link bulunamadı");

                var permissionKey = _permissionKeyResolver.ResolveFromCurrentRequest() ?? "UNKNOWN";
                var userPermissions = new Dictionary<string, YetkiSeviyesi>();
                var originalDto = _mapper.Map<OnemliLinkUpdateRequestDto>(entity);

                var unauthorizedFields = await _fieldPermissionService.ValidateFieldPermissionsAsync(
                    request,
                    userPermissions,
                    originalDto,
                    permissionKey,
                    null);

                if (unauthorizedFields.Any())
                {
                    _fieldPermissionService.RevertUnauthorizedFields(request, originalDto, unauthorizedFields);
                    _logger.LogWarning("OnemliLinkService.UpdateAsync - Field-level permission enforcement: {Count} alan revert edildi.", unauthorizedFields.Count);
                }

                entity.LinkAdi = request.LinkAdi;
                entity.Url = request.Url;
                entity.Sira = request.Sira;
                entity.Aktiflik = request.Aktiflik;
                entity.DuzenlenmeTarihi = DateTimeHelper.Now;

                repo.Update(entity);
                await _unitOfWork.SaveChangesAsync();

                var dto = _mapper.Map<OnemliLinkResponseDto>(entity);
                return ApiResponseDto<OnemliLinkResponseDto>.SuccessResult(dto, "Link başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Link güncellenirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<OnemliLinkResponseDto>.ErrorResult("Link güncellenirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<bool>.ErrorResult("Geçersiz link ID");

                var repo = _unitOfWork.GetRepository<IOnemliLinkRepository>();
                var entity = await repo.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponseDto<bool>.ErrorResult("Link bulunamadı");

                repo.Delete(entity);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponseDto<bool>.SuccessResult(true, "Link başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Link silinirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<bool>.ErrorResult("Link silinirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<PagedResponseDto<OnemliLinkResponseDto>>> GetPagedAsync(OnemliLinkFilterRequestDto filter)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IOnemliLinkRepository>();
                var items = await repo.GetAllAsync();

                var query = items.AsQueryable();

                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    query = query.Where(x => x.LinkAdi.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase));
                }

                if (filter.Aktiflik.HasValue)
                {
                    query = query.Where(x => x.Aktiflik == filter.Aktiflik.Value);
                }

                query = filter.OrderBy.ToLower() switch
                {
                    "linkadi" => filter.OrderDirection == "desc"
                        ? query.OrderByDescending(x => x.LinkAdi)
                        : query.OrderBy(x => x.LinkAdi),
                    "sira" => filter.OrderDirection == "desc"
                        ? query.OrderByDescending(x => x.Sira)
                        : query.OrderBy(x => x.Sira),
                    _ => query.OrderBy(x => x.Sira)
                };

                var totalCount = query.Count();
                var pagedItems = query
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToList();

                var dtos = _mapper.Map<List<OnemliLinkResponseDto>>(pagedItems);

                var paged = new PagedResponseDto<OnemliLinkResponseDto>
                {
                    Items = dtos,
                    TotalCount = totalCount,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize
                };

                return ApiResponseDto<PagedResponseDto<OnemliLinkResponseDto>>.SuccessResult(paged, "Linkler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sayfalı link listesi getirilirken hata oluştu");
                return ApiResponseDto<PagedResponseDto<OnemliLinkResponseDto>>.ErrorResult("Linkler getirilirken hata oluştu", ex.Message);
            }
        }
    }
}
