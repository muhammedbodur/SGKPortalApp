using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.Common.Interfaces.Permission;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Complex;

namespace SGKPortalApp.BusinessLogicLayer.Services.Common
{
    public class HizmetBinasiService : IHizmetBinasiService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonQueryRepository _commonQueryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<HizmetBinasiService> _logger;
        private readonly ICascadeHelper _cascadeHelper;
        private readonly IFieldPermissionValidationService _fieldPermissionService;
        private readonly IPermissionKeyResolverService _permissionKeyResolver;

        public HizmetBinasiService(
            IUnitOfWork unitOfWork,
            ICommonQueryRepository commonQueryRepository,
            IMapper mapper,
            ILogger<HizmetBinasiService> logger,
            ICascadeHelper cascadeHelper,
            IFieldPermissionValidationService fieldPermissionService,
            IPermissionKeyResolverService permissionKeyResolver)
        {
            _unitOfWork = unitOfWork;
            _commonQueryRepository = commonQueryRepository;
            _mapper = mapper;
            _logger = logger;
            _cascadeHelper = cascadeHelper;
            _fieldPermissionService = fieldPermissionService;
            _permissionKeyResolver = permissionKeyResolver;
        }

        public async Task<ApiResponseDto<List<HizmetBinasiResponseDto>>> GetAllAsync()
        {
            try
            {
                var hizmetBinasiRepo = _unitOfWork.GetRepository<IHizmetBinasiRepository>();
                var entities = await hizmetBinasiRepo.GetAllAsync();

                var dtos = new List<HizmetBinasiResponseDto>();

                foreach (var hb in entities)
                {
                    var personelCount = await hizmetBinasiRepo.GetPersonelCountAsync(hb.HizmetBinasiId);
                    var bankoCount = await hizmetBinasiRepo.GetBankoCountAsync(hb.HizmetBinasiId);
                    var tvCount = await hizmetBinasiRepo.GetTvCountAsync(hb.HizmetBinasiId);

                    dtos.Add(new HizmetBinasiResponseDto
                    {
                        HizmetBinasiId = hb.HizmetBinasiId,
                        HizmetBinasiAdi = hb.HizmetBinasiAdi,
                        DepartmanId = hb.DepartmanId,
                        DepartmanAdi = hb.Departman?.DepartmanAdi ?? string.Empty,
                        Adres = hb.Adres,
                        Aktiflik = hb.Aktiflik,
                        PersonelSayisi = personelCount,
                        BankoSayisi = bankoCount,
                        TvSayisi = tvCount,
                        EklenmeTarihi = hb.EklenmeTarihi,
                        DuzenlenmeTarihi = hb.DuzenlenmeTarihi
                    });
                }

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
                    Aktiflik = entity.Aktiflik,
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

                // Aktiflik durumu pasif yapılıyorsa personel kontrolü
                if (entity.Aktiflik == Aktiflik.Aktif && request.Aktiflik == Aktiflik.Pasif)
                {
                    var personelCount = await hizmetBinasiRepo.GetPersonelCountAsync(id);
                    if (personelCount > 0)
                        return ApiResponseDto<HizmetBinasiResponseDto>
                            .ErrorResult($"Bu hizmet binasında {personelCount} personel bulunmaktadır. Önce personelleri başka hizmet binasına taşıyınız");
                }

                // ⭐ Field-level permission enforcement
                // Permission key otomatik çözümleme (route → permission key)
                var permissionKey = _permissionKeyResolver.ResolveFromCurrentRequest() ?? "UNKNOWN";
                var userPermissions = new Dictionary<string, BusinessObjectLayer.Enums.Common.YetkiSeviyesi>();
                var originalDto = _mapper.Map<HizmetBinasiUpdateRequestDto>(entity);

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
                        "HizmetBinasiService.UpdateAsync - Field-level permission enforcement: {Count} alan revert edildi.",
                        unauthorizedFields.Count);
                }

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

                var yeniDurum = entity.Aktiflik == Aktiflik.Aktif
                    ? Aktiflik.Pasif
                    : Aktiflik.Aktif;

                entity.Aktiflik = yeniDurum;
                hizmetBinasiRepo.Update(entity);

                // Cascade: Pasif yapıldıysa child kayıtları da pasif yap
                if (yeniDurum == Aktiflik.Pasif)
                {
                    await CascadeAktiflikUpdateAsync(id);
                }

                await _unitOfWork.SaveChangesAsync();

                var statusText = yeniDurum == Aktiflik.Aktif ? "aktif" : "pasif";
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

        public async Task<ApiResponseDto<List<ServisResponseDto>>> GetServislerByHizmetBinasiIdAsync(int hizmetBinasiId)
        {
            try
            {
                if (hizmetBinasiId <= 0)
                    return ApiResponseDto<List<ServisResponseDto>>
                        .ErrorResult("Geçersiz hizmet binası ID");

                // Repository'den Tuple olarak alıyorum
                var servislerTuple = await _commonQueryRepository.GetServislerByHizmetBinasiIdAsync(hizmetBinasiId);

                // Tuple'ları ServisResponseDto'ya dönüştürüyorum
                var servisDtos = servislerTuple.Select(s => new ServisResponseDto
                {
                    ServisId = s.ServisId,
                    ServisAdi = s.ServisAdi,
                    PersonelSayisi = s.PersonelSayisi,
                    Aktiflik = Aktiflik.Aktif, // Repository'den gelen veriler zaten aktif
                    EklenmeTarihi = DateTime.Now,
                    DuzenlenmeTarihi = DateTime.Now
                }).ToList();

                _logger.LogInformation(
                    "Hizmet binası servisleri getirildi. Hizmet Binası ID: {HizmetBinasiId}, Servis Sayısı: {Count}",
                    hizmetBinasiId,
                    servisDtos.Count
                );

                return ApiResponseDto<List<ServisResponseDto>>
                    .SuccessResult(servisDtos, "Hizmet binası servisleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hizmet binası servisleri getirilirken hata oluştu. Hizmet Binası ID: {Id}", hizmetBinasiId);
                return ApiResponseDto<List<ServisResponseDto>>
                    .ErrorResult("Hizmet binası servisleri getirilirken bir hata oluştu", ex.Message);
            }
        }

        /// <summary>
        /// Cascade update: HizmetBinasi pasif yapıldığında child kayıtları da pasif yap
        /// CascadeHelper kullanarak tracking conflict'leri otomatik handle eder
        /// </summary>
        private async Task CascadeAktiflikUpdateAsync(int hizmetBinasiId)
        {
            // Banko kayıtlarını pasif yap
            await _cascadeHelper.CascadeAktiflikUpdateAsync<Banko>(x => x.HizmetBinasiId == hizmetBinasiId, Aktiflik.Pasif);
            
            // Tv kayıtlarını pasif yap
            await _cascadeHelper.CascadeAktiflikUpdateAsync<Tv>(x => x.HizmetBinasiId == hizmetBinasiId, Aktiflik.Pasif);

            _logger.LogInformation("Cascade pasif: HizmetBinasiId={HizmetBinasiId}", hizmetBinasiId);
        }
    }
}