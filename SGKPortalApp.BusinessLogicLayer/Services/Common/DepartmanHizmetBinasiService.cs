using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;

namespace SGKPortalApp.BusinessLogicLayer.Services.Common
{
    public class DepartmanHizmetBinasiService : IDepartmanHizmetBinasiService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DepartmanHizmetBinasiService> _logger;

        public DepartmanHizmetBinasiService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<DepartmanHizmetBinasiService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>> GetAllAsync()
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IDepartmanHizmetBinasiRepository>();
                var entities = await repository.GetActiveAsync();
                var dtos = _mapper.Map<List<DepartmanHizmetBinasiResponseDto>>(entities);

                return ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>
                    .SuccessResult(dtos, "Departman-Hizmet Binası eşleşmeleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Departman-Hizmet Binası eşleşmeleri getirilirken hata oluştu");
                return ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>
                    .ErrorResult("Veriler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<DepartmanHizmetBinasiResponseDto>> GetByIdAsync(int departmanHizmetBinasiId)
        {
            try
            {
                if (departmanHizmetBinasiId <= 0)
                {
                    return ApiResponseDto<DepartmanHizmetBinasiResponseDto>
                        .ErrorResult("Geçersiz ID");
                }

                var repository = _unitOfWork.GetRepository<IDepartmanHizmetBinasiRepository>();
                var entity = await repository.GetByIdAsync(departmanHizmetBinasiId);

                if (entity == null)
                {
                    return ApiResponseDto<DepartmanHizmetBinasiResponseDto>
                        .ErrorResult("Kayıt bulunamadı");
                }

                var dto = _mapper.Map<DepartmanHizmetBinasiResponseDto>(entity);

                return ApiResponseDto<DepartmanHizmetBinasiResponseDto>
                    .SuccessResult(dto, "Kayıt başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Departman-Hizmet Binası getirilirken hata oluştu. ID: {Id}", departmanHizmetBinasiId);
                return ApiResponseDto<DepartmanHizmetBinasiResponseDto>
                    .ErrorResult("Kayıt getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>> GetByDepartmanAsync(int departmanId)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IDepartmanHizmetBinasiRepository>();
                var entities = await repository.GetByDepartmanAsync(departmanId);
                var dtos = _mapper.Map<List<DepartmanHizmetBinasiResponseDto>>(entities);

                return ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>
                    .SuccessResult(dtos, "Departman hizmet binaları başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Departman hizmet binaları getirilirken hata oluştu. DepartmanId: {DepartmanId}", departmanId);
                return ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>
                    .ErrorResult("Veriler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>> GetByHizmetBinasiAsync(int hizmetBinasiId)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IDepartmanHizmetBinasiRepository>();
                var entities = await repository.GetByHizmetBinasiAsync(hizmetBinasiId);
                var dtos = _mapper.Map<List<DepartmanHizmetBinasiResponseDto>>(entities);

                return ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>
                    .SuccessResult(dtos, "Hizmet binası departmanları başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hizmet binası departmanları getirilirken hata oluştu. HizmetBinasiId: {HizmetBinasiId}", hizmetBinasiId);
                return ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>
                    .ErrorResult("Veriler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<DepartmanHizmetBinasiResponseDto>> CreateAsync(DepartmanHizmetBinasiCreateRequestDto request)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    var repository = _unitOfWork.GetRepository<IDepartmanHizmetBinasiRepository>();

                    // Hizmet binası aktif mi kontrol et
                    var hizmetBinasiRepo = _unitOfWork.GetRepository<IHizmetBinasiRepository>();
                    var hizmetBinasi = await hizmetBinasiRepo.GetByIdAsync(request.HizmetBinasiId);
                    
                    if (hizmetBinasi == null)
                    {
                        return ApiResponseDto<DepartmanHizmetBinasiResponseDto>
                            .ErrorResult("Hizmet binası bulunamadı");
                    }
                    
                    if (hizmetBinasi.Aktiflik != BusinessObjectLayer.Enums.Common.Aktiflik.Aktif)
                    {
                        return ApiResponseDto<DepartmanHizmetBinasiResponseDto>
                            .ErrorResult("Pasif hizmet binaları departmanlara eşleştirilemez");
                    }

                    // Aynı kombinasyon var mı kontrol et
                    var exists = await repository.ExistsAsync(request.DepartmanId, request.HizmetBinasiId);
                    if (exists)
                    {
                        return ApiResponseDto<DepartmanHizmetBinasiResponseDto>
                            .ErrorResult("Bu departman-hizmet binası eşleşmesi zaten mevcut");
                    }

                    var entity = _mapper.Map<DepartmanHizmetBinasi>(request);
                    await repository.AddAsync(entity);
                    await _unitOfWork.SaveChangesAsync();

                    var dto = _mapper.Map<DepartmanHizmetBinasiResponseDto>(entity);

                    _logger.LogInformation("Yeni departman-hizmet binası eşleşmesi oluşturuldu. Id: {Id}", entity.DepartmanHizmetBinasiId);

                    return ApiResponseDto<DepartmanHizmetBinasiResponseDto>
                        .SuccessResult(dto, "Eşleşme başarıyla oluşturuldu");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Departman-Hizmet Binası eşleşmesi oluşturulurken hata oluştu");
                    return ApiResponseDto<DepartmanHizmetBinasiResponseDto>
                        .ErrorResult("Eşleşme oluşturulurken bir hata oluştu", ex.Message);
                }
            });
        }

        public async Task<ApiResponseDto<DepartmanHizmetBinasiResponseDto>> UpdateAsync(int departmanHizmetBinasiId, DepartmanHizmetBinasiUpdateRequestDto request)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    var repository = _unitOfWork.GetRepository<IDepartmanHizmetBinasiRepository>();
                    var entity = await repository.GetByIdAsync(departmanHizmetBinasiId);

                    if (entity == null)
                    {
                        return ApiResponseDto<DepartmanHizmetBinasiResponseDto>
                            .ErrorResult("Kayıt bulunamadı");
                    }

                    entity.AnaBina = request.AnaBina;

                    repository.Update(entity);
                    await _unitOfWork.SaveChangesAsync();

                    var dto = _mapper.Map<DepartmanHizmetBinasiResponseDto>(entity);

                    _logger.LogInformation("Departman-Hizmet Binası eşleşmesi güncellendi. Id: {Id}", departmanHizmetBinasiId);

                    return ApiResponseDto<DepartmanHizmetBinasiResponseDto>
                        .SuccessResult(dto, "Eşleşme başarıyla güncellendi");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Departman-Hizmet Binası eşleşmesi güncellenirken hata oluştu. Id: {Id}", departmanHizmetBinasiId);
                    return ApiResponseDto<DepartmanHizmetBinasiResponseDto>
                        .ErrorResult("Eşleşme güncellenirken bir hata oluştu", ex.Message);
                }
            });
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int departmanHizmetBinasiId)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    var repository = _unitOfWork.GetRepository<IDepartmanHizmetBinasiRepository>();
                    var entity = await repository.GetByIdAsync(departmanHizmetBinasiId);

                    if (entity == null)
                    {
                        return ApiResponseDto<bool>
                            .ErrorResult("Kayıt bulunamadı");
                    }

                    repository.Delete(entity);
                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogInformation("Departman-Hizmet Binası eşleşmesi silindi. Id: {Id}", departmanHizmetBinasiId);

                    return ApiResponseDto<bool>
                        .SuccessResult(true, "Eşleşme başarıyla silindi");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Departman-Hizmet Binası eşleşmesi silinirken hata oluştu. Id: {Id}", departmanHizmetBinasiId);
                    return ApiResponseDto<bool>
                        .ErrorResult("Eşleşme silinirken bir hata oluştu", ex.Message);
                }
            });
        }

        public async Task<ApiResponseDto<List<DepartmanHizmetBinasiDto>>> GetAllForDropdownAsync()
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IDepartmanHizmetBinasiRepository>();
                var tuples = await repository.GetDropdownAsync();

                var dtos = tuples.Select(t => new DepartmanHizmetBinasiDto
                {
                    DepartmanHizmetBinasiId = t.Id,
                    DepartmanAdi = t.DisplayText.Split(" - ")[0],
                    HizmetBinasiAdi = t.DisplayText.Split(" - ")[1]
                }).ToList();

                return ApiResponseDto<List<DepartmanHizmetBinasiDto>>
                    .SuccessResult(dtos, "Dropdown listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dropdown listesi getirilirken hata oluştu");
                return ApiResponseDto<List<DepartmanHizmetBinasiDto>>
                    .ErrorResult("Dropdown listesi getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<DepartmanHizmetBinasiDto>>> GetDropdownByDepartmanAsync(int departmanId)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IDepartmanHizmetBinasiRepository>();
                var tuples = await repository.GetDropdownByDepartmanAsync(departmanId);

                var dtos = tuples.Select(t => new DepartmanHizmetBinasiDto
                {
                    DepartmanHizmetBinasiId = t.Id,
                    HizmetBinasiAdi = t.DisplayText
                }).ToList();

                return ApiResponseDto<List<DepartmanHizmetBinasiDto>>
                    .SuccessResult(dtos, "Dropdown listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Departman dropdown listesi getirilirken hata oluştu. DepartmanId: {DepartmanId}", departmanId);
                return ApiResponseDto<List<DepartmanHizmetBinasiDto>>
                    .ErrorResult("Dropdown listesi getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<DepartmanHizmetBinasiDto>>> GetDropdownByHizmetBinasiAsync(int hizmetBinasiId)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IDepartmanHizmetBinasiRepository>();
                var tuples = await repository.GetDropdownByHizmetBinasiAsync(hizmetBinasiId);

                var dtos = tuples.Select(t => new DepartmanHizmetBinasiDto
                {
                    DepartmanHizmetBinasiId = t.Id,
                    DepartmanAdi = t.DisplayText
                }).ToList();

                return ApiResponseDto<List<DepartmanHizmetBinasiDto>>
                    .SuccessResult(dtos, "Dropdown listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hizmet binası dropdown listesi getirilirken hata oluştu. HizmetBinasiId: {HizmetBinasiId}", hizmetBinasiId);
                return ApiResponseDto<List<DepartmanHizmetBinasiDto>>
                    .ErrorResult("Dropdown listesi getirilirken bir hata oluştu", ex.Message);
            }
        }
    }
}
