using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;

namespace SGKPortalApp.BusinessLogicLayer.Services.Common
{
    public class ModulService : IModulService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ModulService> _logger;

        public ModulService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ModulService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<ModulResponseDto>>> GetAllAsync()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IModulRepository>();
                var moduller = await repo.GetAllAsync();

                var dtos = new List<ModulResponseDto>();
                foreach (var modul in moduller)
                {
                    var dto = _mapper.Map<ModulResponseDto>(modul);
                    dto.ControllerCount = await repo.GetControllerCountAsync(modul.ModulId);
                    dtos.Add(dto);
                }

                return ApiResponseDto<List<ModulResponseDto>>.SuccessResult(dtos, "Modüller başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Modül listesi getirilirken hata oluştu");
                return ApiResponseDto<List<ModulResponseDto>>.ErrorResult("Modüller getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<ModulResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<ModulResponseDto>.ErrorResult("Geçersiz modül ID");

                var repo = _unitOfWork.GetRepository<IModulRepository>();
                var entity = await repo.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponseDto<ModulResponseDto>.ErrorResult("Modül bulunamadı");

                var dto = _mapper.Map<ModulResponseDto>(entity);
                dto.ControllerCount = await repo.GetControllerCountAsync(entity.ModulId);

                return ApiResponseDto<ModulResponseDto>.SuccessResult(dto, "Modül başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Modül getirilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<ModulResponseDto>.ErrorResult("Modül getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<ModulResponseDto>> CreateAsync(ModulCreateRequestDto request)
        {
            try
            {
                var genericRepo = _unitOfWork.Repository<Modul>();

                // Check if module name already exists
                var exists = await genericRepo.ExistsAsync(m => m.ModulAdi == request.ModulAdi);
                if (exists)
                    return ApiResponseDto<ModulResponseDto>.ErrorResult("Bu modül adı zaten mevcut");

                var entity = _mapper.Map<Modul>(request);
                await genericRepo.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                var dto = _mapper.Map<ModulResponseDto>(entity);
                dto.ControllerCount = 0;

                _logger.LogInformation("Yeni modül oluşturuldu. ID: {Id}, Adı: {Ad}", entity.ModulId, entity.ModulAdi);

                return ApiResponseDto<ModulResponseDto>.SuccessResult(dto, "Modül başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Modül oluşturulurken hata oluştu");
                return ApiResponseDto<ModulResponseDto>.ErrorResult("Modül oluşturulurken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<ModulResponseDto>> UpdateAsync(int id, ModulUpdateRequestDto request)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<ModulResponseDto>.ErrorResult("Geçersiz modül ID");

                var genericRepo = _unitOfWork.Repository<Modul>();
                var entity = await genericRepo.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponseDto<ModulResponseDto>.ErrorResult("Modül bulunamadı");

                // Check if new name already exists for different module
                var exists = await genericRepo.ExistsAsync(m => m.ModulAdi == request.ModulAdi && m.ModulId != id);
                if (exists)
                    return ApiResponseDto<ModulResponseDto>.ErrorResult("Bu modül adı zaten mevcut");

                _mapper.Map(request, entity);
                genericRepo.Update(entity);
                await _unitOfWork.SaveChangesAsync();

                var repo = _unitOfWork.GetRepository<IModulRepository>();
                var dto = _mapper.Map<ModulResponseDto>(entity);
                dto.ControllerCount = await repo.GetControllerCountAsync(entity.ModulId);

                _logger.LogInformation("Modül güncellendi. ID: {Id}, Yeni Adı: {Ad}", id, entity.ModulAdi);

                return ApiResponseDto<ModulResponseDto>.SuccessResult(dto, "Modül başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Modül güncellenirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<ModulResponseDto>.ErrorResult("Modül güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<bool>.ErrorResult("Geçersiz modül ID");

                var genericRepo = _unitOfWork.Repository<Modul>();
                var entity = await genericRepo.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponseDto<bool>.ErrorResult("Modül bulunamadı");

                // Check if module has controllers
                var repo = _unitOfWork.GetRepository<IModulRepository>();
                var controllerCount = await repo.GetControllerCountAsync(id);
                if (controllerCount > 0)
                    return ApiResponseDto<bool>.ErrorResult($"Bu modüle bağlı {controllerCount} controller bulunmaktadır. Önce onları silmelisiniz.");

                genericRepo.Delete(entity);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Modül silindi. ID: {Id}, Adı: {Ad}", id, entity.ModulAdi);

                return ApiResponseDto<bool>.SuccessResult(true, "Modül başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Modül silinirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<bool>.ErrorResult("Modül silinirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<DropdownItemDto>>> GetDropdownAsync()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IModulRepository>();
                var dropdown = await repo.GetDropdownAsync();

                var dtos = dropdown.Select(d => new DropdownItemDto
                {
                    Id = d.Id,
                    Ad = d.Ad
                }).ToList();

                return ApiResponseDto<List<DropdownItemDto>>.SuccessResult(dtos, "Dropdown listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Modül dropdown listesi getirilirken hata oluştu");
                return ApiResponseDto<List<DropdownItemDto>>.ErrorResult("Dropdown listesi getirilirken bir hata oluştu", ex.Message);
            }
        }
    }
}
