using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Services.PersonelIslemleri
{
    public class YetkiService : IYetkiService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<YetkiService> _logger;

        public YetkiService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<YetkiService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<YetkiResponseDto>>> GetAllAsync()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IYetkiRepository>();
                var yetkiler = await repo.GetAllActiveAsync();
                var dtos = _mapper.Map<List<YetkiResponseDto>>(yetkiler);

                return ApiResponseDto<List<YetkiResponseDto>>.SuccessResult(dtos, "Yetkiler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Yetki listesi getirilirken hata oluştu");
                return ApiResponseDto<List<YetkiResponseDto>>.ErrorResult("Yetkiler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<YetkiResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<YetkiResponseDto>.ErrorResult("Geçersiz yetki ID");

                var repo = _unitOfWork.GetRepository<IYetkiRepository>();
                var entity = await repo.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponseDto<YetkiResponseDto>.ErrorResult("Yetki bulunamadı");

                var dto = _mapper.Map<YetkiResponseDto>(entity);
                return ApiResponseDto<YetkiResponseDto>.SuccessResult(dto, "Yetki başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Yetki getirilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<YetkiResponseDto>.ErrorResult("Yetki getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<YetkiResponseDto>> CreateAsync(YetkiCreateRequestDto request)
        {
            try
            {
                var genericRepo = _unitOfWork.Repository<Yetki>();
                var exists = await genericRepo.ExistsAsync(y => y.YetkiAdi == request.YetkiAdi && y.UstYetkiId == request.UstYetkiId);
                if (exists)
                    return ApiResponseDto<YetkiResponseDto>.ErrorResult("Bu yetki (üst yetki + ad) zaten mevcut");

                var entity = _mapper.Map<Yetki>(request);
                await genericRepo.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                var dto = _mapper.Map<YetkiResponseDto>(entity);
                _logger.LogInformation("Yeni yetki oluşturuldu. ID: {Id}", entity.YetkiId);

                return ApiResponseDto<YetkiResponseDto>.SuccessResult(dto, "Yetki başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Yetki oluşturulurken hata oluştu");
                return ApiResponseDto<YetkiResponseDto>.ErrorResult("Yetki oluşturulurken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<YetkiResponseDto>> UpdateAsync(int id, YetkiUpdateRequestDto request)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<YetkiResponseDto>.ErrorResult("Geçersiz yetki ID");

                var genericRepo = _unitOfWork.Repository<Yetki>();
                var entity = await genericRepo.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponseDto<YetkiResponseDto>.ErrorResult("Yetki bulunamadı");

                var exists = await genericRepo.ExistsAsync(y => y.YetkiId != id && y.YetkiAdi == request.YetkiAdi && y.UstYetkiId == request.UstYetkiId);
                if (exists)
                    return ApiResponseDto<YetkiResponseDto>.ErrorResult("Bu yetki (üst yetki + ad) zaten mevcut");

                _mapper.Map(request, entity);
                genericRepo.Update(entity);
                await _unitOfWork.SaveChangesAsync();

                var dto = _mapper.Map<YetkiResponseDto>(entity);
                _logger.LogInformation("Yetki güncellendi. ID: {Id}", id);

                return ApiResponseDto<YetkiResponseDto>.SuccessResult(dto, "Yetki başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Yetki güncellenirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<YetkiResponseDto>.ErrorResult("Yetki güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<bool>.ErrorResult("Geçersiz yetki ID");

                var repo = _unitOfWork.GetRepository<IYetkiRepository>();
                var entity = await repo.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponseDto<bool>.ErrorResult("Yetki bulunamadı");

                var children = await repo.GetByParentYetkiAsync(id);
                if (children.Any())
                    return ApiResponseDto<bool>.ErrorResult("Alt yetkileri olan yetki silinemez");

                var usedByPersonel = await _unitOfWork.Repository<SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri.PersonelYetki>()
                    .ExistsAsync(py => py.YetkiId == id);
                if (usedByPersonel)
                    return ApiResponseDto<bool>.ErrorResult("Personellere atanmış yetki silinemez");

                repo.Delete(entity);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Yetki silindi (soft delete). ID: {Id}", id);
                return ApiResponseDto<bool>.SuccessResult(true, "Yetki başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Yetki silinirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<bool>.ErrorResult("Yetki silinirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<YetkiResponseDto>>> GetRootAsync()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IYetkiRepository>();
                var items = await repo.GetRootYetkilerAsync();
                var dtos = _mapper.Map<List<YetkiResponseDto>>(items);

                return ApiResponseDto<List<YetkiResponseDto>>.SuccessResult(dtos, "Kök yetkiler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kök yetkiler getirilirken hata oluştu");
                return ApiResponseDto<List<YetkiResponseDto>>.ErrorResult("Kök yetkiler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<YetkiResponseDto>>> GetChildrenAsync(int ustYetkiId)
        {
            try
            {
                if (ustYetkiId <= 0)
                    return ApiResponseDto<List<YetkiResponseDto>>.ErrorResult("Geçersiz üst yetki ID");

                var repo = _unitOfWork.GetRepository<IYetkiRepository>();
                var items = await repo.GetByParentYetkiAsync(ustYetkiId);
                var dtos = _mapper.Map<List<YetkiResponseDto>>(items);

                return ApiResponseDto<List<YetkiResponseDto>>.SuccessResult(dtos, "Alt yetkiler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Alt yetkiler getirilirken hata oluştu. UstYetkiId: {UstYetkiId}", ustYetkiId);
                return ApiResponseDto<List<YetkiResponseDto>>.ErrorResult("Alt yetkiler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<DropdownItemDto>>> GetDropdownAsync()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IYetkiRepository>();
                var dropdown = await repo.GetDropdownAsync();
                var list = dropdown.ToList();

                return ApiResponseDto<List<DropdownItemDto>>.SuccessResult(list, "Dropdown listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Yetki dropdown listesi getirilirken hata oluştu");
                return ApiResponseDto<List<DropdownItemDto>>.ErrorResult("Dropdown listesi getirilirken bir hata oluştu", ex.Message);
            }
        }
    }
}
