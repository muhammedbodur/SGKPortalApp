using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;

namespace SGKPortalApp.BusinessLogicLayer.Services.Common
{
    public class HizmetBinasiService : IHizmetBinasiService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<HizmetBinasiService> _logger;

        public HizmetBinasiService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<HizmetBinasiService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<HizmetBinasiResponseDto>>> GetAllAsync()
        {
            try
            {
                var hizmetBinasiRepo = _unitOfWork.GetRepository<IHizmetBinasiRepository>();
                var entities = await hizmetBinasiRepo.GetAllAsync();

                var dtos = entities.Select(hb => new HizmetBinasiResponseDto
                {
                    HizmetBinasiId = hb.HizmetBinasiId,
                    HizmetBinasiAdi = hb.HizmetBinasiAdi,
                    DepartmanId = hb.DepartmanId,
                    DepartmanAdi = hb.Departman?.DepartmanAdi ?? string.Empty,
                    Adres = hb.Adres,
                    HizmetBinasiAktiflik = hb.HizmetBinasiAktiflik,
                    PersonelSayisi = hb.Personeller?.Count ?? 0,
                    BankoSayisi = hb.Bankolar?.Count ?? 0,
                    TvSayisi = hb.Tvler?.Count ?? 0,
                    EklenmeTarihi = hb.EklenmeTarihi,
                    DuzenlenmeTarihi = hb.DuzenlenmeTarihi
                }).ToList();

                return ApiResponseDto<List<HizmetBinasiResponseDto>>
                    .SuccessResult(dtos, "Hizmet binaları başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hizmet binaları getirilirken hata oluştu");
                return ApiResponseDto<List<HizmetBinasiResponseDto>>
                    .ErrorResult("Hizmet binaları getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<HizmetBinasiResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var hizmetBinasiRepo = _unitOfWork.GetRepository<IHizmetBinasiRepository>();
                var entity = await hizmetBinasiRepo.GetWithDepartmanAsync(id);

                if (entity == null)
                    return ApiResponseDto<HizmetBinasiResponseDto>.ErrorResult("Hizmet binası bulunamadı");

                var personelCount = await hizmetBinasiRepo.GetPersonelCountAsync(id);
                var bankoCount = await hizmetBinasiRepo.GetBankoCountAsync(id);
                var tvCount = await hizmetBinasiRepo.GetTvCountAsync(id);

                var dto = new HizmetBinasiResponseDto
                {
                    HizmetBinasiId = entity.HizmetBinasiId,
                    HizmetBinasiAdi = entity.HizmetBinasiAdi,
                    DepartmanId = entity.DepartmanId,
                    DepartmanAdi = entity.Departman?.DepartmanAdi ?? string.Empty,
                    Adres = entity.Adres,
                    HizmetBinasiAktiflik = entity.HizmetBinasiAktiflik,
                    PersonelSayisi = personelCount,
                    BankoSayisi = bankoCount,
                    TvSayisi = tvCount,
                    EklenmeTarihi = entity.EklenmeTarihi,
                    DuzenlenmeTarihi = entity.DuzenlenmeTarihi
                };

                return ApiResponseDto<HizmetBinasiResponseDto>
                    .SuccessResult(dto, "Hizmet binası başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hizmet binası getirilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<HizmetBinasiResponseDto>
                    .ErrorResult("Hizmet binası getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<HizmetBinasiDetailResponseDto>> GetDetailByIdAsync(int id)
        {
            try
            {
                var hizmetBinasiRepo = _unitOfWork.GetRepository<IHizmetBinasiRepository>();
                var entity = await hizmetBinasiRepo.GetWithAllDetailsAsync(id);

                if (entity == null)
                    return ApiResponseDto<HizmetBinasiDetailResponseDto>
                        .ErrorResult("Hizmet binası bulunamadı");

                var dto = _mapper.Map<HizmetBinasiDetailResponseDto>(entity);

                return ApiResponseDto<HizmetBinasiDetailResponseDto>
                    .SuccessResult(dto, "Hizmet binası detayları başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hizmet binası detayı getirilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<HizmetBinasiDetailResponseDto>
                    .ErrorResult("Hizmet binası detayı getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<HizmetBinasiResponseDto>> CreateAsync(HizmetBinasiCreateRequestDto request)
        {
            try
            {
                var hizmetBinasiRepo = _unitOfWork.GetRepository<IHizmetBinasiRepository>();

                // Aynı isimde hizmet binası var mı kontrol et
                var exists = await hizmetBinasiRepo.GetByHizmetBinasiAdiAsync(request.HizmetBinasiAdi);
                if (exists != null)
                    return ApiResponseDto<HizmetBinasiResponseDto>
                        .ErrorResult("Bu isimde bir hizmet binası zaten mevcut");

                var entity = _mapper.Map<HizmetBinasi>(request);
                await hizmetBinasiRepo.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                var dto = _mapper.Map<HizmetBinasiResponseDto>(entity);
                _logger.LogInformation("Yeni hizmet binası oluşturuldu. ID: {Id}", entity.HizmetBinasiId);

                return ApiResponseDto<HizmetBinasiResponseDto>
                    .SuccessResult(dto, "Hizmet binası başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hizmet binası oluşturulurken hata oluştu");
                return ApiResponseDto<HizmetBinasiResponseDto>
                    .ErrorResult("Hizmet binası oluşturulurken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<HizmetBinasiResponseDto>> UpdateAsync(int id, HizmetBinasiUpdateRequestDto request)
        {
            try
            {
                var hizmetBinasiRepo = _unitOfWork.GetRepository<IHizmetBinasiRepository>();
                var entity = await hizmetBinasiRepo.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponseDto<HizmetBinasiResponseDto>.ErrorResult("Hizmet binası bulunamadı");

                // Aynı isimde başka hizmet binası var mı kontrol et
                var existingEntity = await hizmetBinasiRepo.GetByHizmetBinasiAdiAsync(request.HizmetBinasiAdi);
                if (existingEntity != null && existingEntity.HizmetBinasiId != id)
                    return ApiResponseDto<HizmetBinasiResponseDto>
                        .ErrorResult("Bu isimde başka bir hizmet binası zaten mevcut");

                _mapper.Map(request, entity);
                hizmetBinasiRepo.Update(entity);
                await _unitOfWork.SaveChangesAsync();

                var dto = _mapper.Map<HizmetBinasiResponseDto>(entity);
                _logger.LogInformation("Hizmet binası güncellendi. ID: {Id}", id);

                return ApiResponseDto<HizmetBinasiResponseDto>
                    .SuccessResult(dto, "Hizmet binası başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hizmet binası güncellenirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<HizmetBinasiResponseDto>
                    .ErrorResult("Hizmet binası güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                var hizmetBinasiRepo = _unitOfWork.GetRepository<IHizmetBinasiRepository>();
                var entity = await hizmetBinasiRepo.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponseDto<bool>.ErrorResult("Hizmet binası bulunamadı");

                // İlişkili kayıt kontrolü
                var personelCount = await hizmetBinasiRepo.GetPersonelCountAsync(id);
                if (personelCount > 0)
                    return ApiResponseDto<bool>
                        .ErrorResult($"Bu hizmet binasına ait {personelCount} personel bulunmaktadır. Önce personelleri başka bir hizmet binasına taşıyın.");

                hizmetBinasiRepo.Delete(entity);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Hizmet binası silindi. ID: {Id}", id);

                return ApiResponseDto<bool>
                    .SuccessResult(true, "Hizmet binası başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hizmet binası silinirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<bool>
                    .ErrorResult("Hizmet binası silinirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<HizmetBinasiResponseDto>>> GetActiveAsync()
        {
            try
            {
                var hizmetBinasiRepo = _unitOfWork.GetRepository<IHizmetBinasiRepository>();
                var entities = await hizmetBinasiRepo.GetActiveAsync();

                var dtos = _mapper.Map<List<HizmetBinasiResponseDto>>(entities);

                return ApiResponseDto<List<HizmetBinasiResponseDto>>
                    .SuccessResult(dtos, "Aktif hizmet binaları başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif hizmet binaları getirilirken hata oluştu");
                return ApiResponseDto<List<HizmetBinasiResponseDto>>
                    .ErrorResult("Aktif hizmet binaları getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<HizmetBinasiResponseDto>>> GetByDepartmanAsync(int departmanId)
        {
            try
            {
                var hizmetBinasiRepo = _unitOfWork.GetRepository<IHizmetBinasiRepository>();
                var entities = await hizmetBinasiRepo.GetByDepartmanAsync(departmanId);

                var dtos = _mapper.Map<List<HizmetBinasiResponseDto>>(entities);

                return ApiResponseDto<List<HizmetBinasiResponseDto>>
                    .SuccessResult(dtos, "Departmana ait hizmet binaları başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Departmana ait hizmet binaları getirilirken hata oluştu. Departman ID: {DepartmanId}", departmanId);
                return ApiResponseDto<List<HizmetBinasiResponseDto>>
                    .ErrorResult("Departmana ait hizmet binaları getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<int>> GetPersonelCountAsync(int hizmetBinasiId)
        {
            try
            {
                var hizmetBinasiRepo = _unitOfWork.GetRepository<IHizmetBinasiRepository>();
                var count = await hizmetBinasiRepo.GetPersonelCountAsync(hizmetBinasiId);

                return ApiResponseDto<int>
                    .SuccessResult(count, "Personel sayısı başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel sayısı getirilirken hata oluştu. Hizmet Binası ID: {Id}", hizmetBinasiId);
                return ApiResponseDto<int>
                    .ErrorResult("Personel sayısı getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> ToggleStatusAsync(int id)
        {
            try
            {
                var hizmetBinasiRepo = _unitOfWork.GetRepository<IHizmetBinasiRepository>();
                var entity = await hizmetBinasiRepo.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponseDto<bool>.ErrorResult("Hizmet binası bulunamadı");

                entity.HizmetBinasiAktiflik = entity.HizmetBinasiAktiflik == Aktiflik.Aktif
                    ? Aktiflik.Pasif
                    : Aktiflik.Aktif;

                hizmetBinasiRepo.Update(entity);
                await _unitOfWork.SaveChangesAsync();

                var statusText = entity.HizmetBinasiAktiflik == Aktiflik.Aktif ? "aktif" : "pasif";
                _logger.LogInformation("Hizmet binası durumu değiştirildi. ID: {Id}, Yeni Durum: {Status}", id, statusText);

                return ApiResponseDto<bool>
                    .SuccessResult(true, $"Hizmet binası {statusText} yapıldı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hizmet binası durumu değiştirilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<bool>
                    .ErrorResult("Hizmet binası durumu değiştirilirken bir hata oluştu", ex.Message);
            }
        }
    }
}