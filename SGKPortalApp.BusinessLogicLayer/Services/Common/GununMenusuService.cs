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
    public class GununMenusuService : IGununMenusuService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GununMenusuService> _logger;
        private readonly IFieldPermissionValidationService _fieldPermissionService;
        private readonly IPermissionKeyResolverService _permissionKeyResolver;

        public GununMenusuService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GununMenusuService> logger,
            IFieldPermissionValidationService fieldPermissionService,
            IPermissionKeyResolverService permissionKeyResolver)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _fieldPermissionService = fieldPermissionService;
            _permissionKeyResolver = permissionKeyResolver;
        }

        public async Task<ApiResponseDto<List<GununMenusuResponseDto>>> GetAllAsync()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IGununMenusuRepository>();
                var items = await repo.GetAllAsync();

                var ordered = items
                    .OrderByDescending(x => x.Tarih)
                    .ThenByDescending(x => x.EklenmeTarihi)
                    .ToList();

                var dtos = _mapper.Map<List<GununMenusuResponseDto>>(ordered);
                return ApiResponseDto<List<GununMenusuResponseDto>>.SuccessResult(dtos, "Günün menüleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Günün menüsü listesi getirilirken hata oluştu");
                return ApiResponseDto<List<GununMenusuResponseDto>>.ErrorResult("Günün menüsü listesi getirilirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<GununMenusuResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<GununMenusuResponseDto>.ErrorResult("Geçersiz menü ID");

                var repo = _unitOfWork.GetRepository<IGununMenusuRepository>();
                var entity = await repo.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponseDto<GununMenusuResponseDto>.ErrorResult("Menü bulunamadı");

                var dto = _mapper.Map<GununMenusuResponseDto>(entity);
                return ApiResponseDto<GununMenusuResponseDto>.SuccessResult(dto, "Menü başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Menü getirilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<GununMenusuResponseDto>.ErrorResult("Menü getirilirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<GununMenusuResponseDto?>> GetByDateAsync(DateTime date)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IGununMenusuRepository>();
                var entity = await repo.GetMenuByDateAsync(date);

                if (entity == null)
                    return ApiResponseDto<GununMenusuResponseDto?>.SuccessResult(null, "Menü bulunamadı");

                var dto = _mapper.Map<GununMenusuResponseDto>(entity);
                return ApiResponseDto<GununMenusuResponseDto?>.SuccessResult(dto, "Menü başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Menü getirilirken hata oluştu. Date: {Date}", date);
                return ApiResponseDto<GununMenusuResponseDto?>.ErrorResult("Menü getirilirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<GununMenusuResponseDto>> CreateAsync(GununMenusuCreateRequestDto request)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IGununMenusuRepository>();

                var existsSameDate = await repo.ExistsAsync(x => x.Tarih.Date == request.Tarih.Date);
                if (existsSameDate)
                    return ApiResponseDto<GununMenusuResponseDto>.ErrorResult("Bu tarih için zaten bir menü kaydı mevcut");

                var entity = _mapper.Map<GununMenusu>(request);
                await repo.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                var dto = _mapper.Map<GununMenusuResponseDto>(entity);
                return ApiResponseDto<GununMenusuResponseDto>.SuccessResult(dto, "Menü başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Menü oluşturulurken hata oluştu");
                return ApiResponseDto<GununMenusuResponseDto>.ErrorResult("Menü oluşturulurken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<GununMenusuResponseDto>> UpdateAsync(int id, GununMenusuUpdateRequestDto request)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<GununMenusuResponseDto>.ErrorResult("Geçersiz menü ID");

                var repo = _unitOfWork.GetRepository<IGununMenusuRepository>();
                var entity = await repo.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponseDto<GununMenusuResponseDto>.ErrorResult("Menü bulunamadı");

                var existsSameDate = await repo.ExistsAsync(x => x.MenuId != id && x.Tarih.Date == request.Tarih.Date);
                if (existsSameDate)
                    return ApiResponseDto<GununMenusuResponseDto>.ErrorResult("Bu tarih için zaten başka bir menü kaydı mevcut");

                var permissionKey = _permissionKeyResolver.ResolveFromCurrentRequest() ?? "UNKNOWN";
                var userPermissions = new Dictionary<string, YetkiSeviyesi>();
                var originalDto = _mapper.Map<GununMenusuUpdateRequestDto>(entity);

                var unauthorizedFields = await _fieldPermissionService.ValidateFieldPermissionsAsync(
                    request,
                    userPermissions,
                    originalDto,
                    permissionKey,
                    null);

                if (unauthorizedFields.Any())
                {
                    _fieldPermissionService.RevertUnauthorizedFields(request, originalDto, unauthorizedFields);
                    _logger.LogWarning("GununMenusuService.UpdateAsync - Field-level permission enforcement: {Count} alan revert edildi.", unauthorizedFields.Count);
                }

                entity.Tarih = request.Tarih.Date;
                entity.Icerik = request.Icerik;
                entity.Aktiflik = request.Aktiflik;
                entity.DuzenlenmeTarihi = DateTimeHelper.Now;

                repo.Update(entity);
                await _unitOfWork.SaveChangesAsync();

                var dto = _mapper.Map<GununMenusuResponseDto>(entity);
                return ApiResponseDto<GununMenusuResponseDto>.SuccessResult(dto, "Menü başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Menü güncellenirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<GununMenusuResponseDto>.ErrorResult("Menü güncellenirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<bool>.ErrorResult("Geçersiz menü ID");

                var repo = _unitOfWork.GetRepository<IGununMenusuRepository>();
                var entity = await repo.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponseDto<bool>.ErrorResult("Menü bulunamadı");

                repo.Delete(entity);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponseDto<bool>.SuccessResult(true, "Menü başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Menü silinirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<bool>.ErrorResult("Menü silinirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<PagedResponseDto<GununMenusuResponseDto>>> GetPagedAsync(GununMenusuFilterRequestDto filter)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IGununMenusuRepository>();

                var start = filter.StartDate?.Date;
                var end = filter.EndDate?.Date;
                var search = filter.SearchTerm?.Trim();

                var hasSearch = !string.IsNullOrWhiteSpace(search);

                System.Linq.Expressions.Expression<Func<GununMenusu, bool>> predicate = x =>
                    (start == null || x.Tarih >= start.Value) &&
                    (end == null || x.Tarih <= end.Value) &&
                    (!hasSearch || x.Icerik.Contains(search!)) &&
                    (filter.Aktiflik == null || x.Aktiflik == filter.Aktiflik.Value);

                var ascending = string.Equals(filter.OrderDirection, "asc", StringComparison.OrdinalIgnoreCase);

                Func<GununMenusu, object> orderSelector = filter.OrderBy.ToLower() switch
                {
                    "tarih" => x => x.Tarih,
                    "eklenmetarihi" => x => x.EklenmeTarihi,
                    _ => x => x.Tarih
                };

                var all = await repo.FindAsync(predicate);

                IEnumerable<GununMenusu> ordered = ascending
                    ? all.OrderBy(orderSelector)
                    : all.OrderByDescending(orderSelector);

                var totalCount = ordered.Count();
                var pagedItems = ordered
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToList();

                var dtos = _mapper.Map<List<GununMenusuResponseDto>>(pagedItems);

                var paged = new PagedResponseDto<GununMenusuResponseDto>
                {
                    Items = dtos,
                    TotalCount = totalCount,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize
                };

                return ApiResponseDto<PagedResponseDto<GununMenusuResponseDto>>.SuccessResult(paged, "Menüler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sayfalı menü listesi getirilirken hata oluştu");
                return ApiResponseDto<PagedResponseDto<GununMenusuResponseDto>>.ErrorResult("Menüler getirilirken hata oluştu", ex.Message);
            }
        }
    }
}
