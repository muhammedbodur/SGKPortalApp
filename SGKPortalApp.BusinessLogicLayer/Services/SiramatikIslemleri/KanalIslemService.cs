using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Services.SiramatikIslemleri
{
    public class KanalIslemService : IKanalIslemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<KanalIslemService> _logger;
        private readonly ICascadeHelper _cascadeHelper;
        private readonly IFieldPermissionValidationService _fieldPermissionService;

        public KanalIslemService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<KanalIslemService> logger,
            ICascadeHelper cascadeHelper,
            IFieldPermissionValidationService fieldPermissionService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cascadeHelper = cascadeHelper;
            _fieldPermissionService = fieldPermissionService;
        }

        public async Task<ApiResponseDto<List<KanalIslemResponseDto>>> GetAllAsync()
        {
            try
            {
                var kanalIslemRepo = _unitOfWork.GetRepository<IKanalIslemRepository>();
                var kanalIslemler = await kanalIslemRepo.GetAllWithDetailsAsync();

                var kanalIslemDtos = _mapper.Map<List<KanalIslemResponseDto>>(kanalIslemler);

                return ApiResponseDto<List<KanalIslemResponseDto>>
                    .SuccessResult(kanalIslemDtos, "Kanal işlemler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal işlem listesi getirilirken hata oluştu");
                return ApiResponseDto<List<KanalIslemResponseDto>>
                    .ErrorResult("Kanal işlemler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KanalIslemResponseDto>> GetByIdAsync(int kanalIslemId)
        {
            try
            {
                if (kanalIslemId <= 0)
                {
                    return ApiResponseDto<KanalIslemResponseDto>
                        .ErrorResult("Geçersiz kanal işlem ID");
                }

                var kanalIslemRepo = _unitOfWork.GetRepository<IKanalIslemRepository>();
                // GetWithDetailsAsync kullanarak Kanal ve HizmetBinasi navigation property'lerini include et
                var kanalIslem = await kanalIslemRepo.GetWithDetailsAsync(kanalIslemId);

                if (kanalIslem == null)
                {
                    return ApiResponseDto<KanalIslemResponseDto>
                        .ErrorResult("Kanal işlem bulunamadı");
                }

                var kanalIslemDto = _mapper.Map<KanalIslemResponseDto>(kanalIslem);

                return ApiResponseDto<KanalIslemResponseDto>
                    .SuccessResult(kanalIslemDto, "Kanal işlem başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal işlem getirilirken hata oluştu. ID: {KanalIslemId}", kanalIslemId);
                return ApiResponseDto<KanalIslemResponseDto>
                    .ErrorResult("Kanal işlem getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KanalIslemResponseDto>> CreateAsync(KanalIslemCreateRequestDto request)
        {
            try
            {
                // Validation
                if (request.KanalId <= 0)
                {
                    return ApiResponseDto<KanalIslemResponseDto>
                        .ErrorResult("Kanal seçimi zorunludur");
                }

                if (request.HizmetBinasiId <= 0)
                {
                    return ApiResponseDto<KanalIslemResponseDto>
                        .ErrorResult("Hizmet binası seçimi zorunludur");
                }

                if (request.BaslangicNumara >= request.BitisNumara)
                {
                    return ApiResponseDto<KanalIslemResponseDto>
                        .ErrorResult("Başlangıç numarası, bitiş numarasından küçük olmalıdır");
                }

                // Üst kaydın (Kanal) silinmiş veya pasif olup olmadığını kontrol et
                var kanalRepo = _unitOfWork.GetRepository<IKanalRepository>();
                var kanal = await kanalRepo.GetByIdAsync(request.KanalId);
                
                if (kanal == null || kanal.SilindiMi)
                {
                    return ApiResponseDto<KanalIslemResponseDto>
                        .ErrorResult("Bağlı olduğu Kanal silinmiş olduğu için bu kanal işlem eklenemez.");
                }
                
                if (kanal.Aktiflik != Aktiflik.Aktif)
                {
                    return ApiResponseDto<KanalIslemResponseDto>
                        .ErrorResult("Bağlı olduğu Kanal pasif durumda olduğu için bu kanal işlem eklenemez. Önce Kanal'ı aktif ediniz.");
                }

                var kanalIslemRepo = _unitOfWork.GetRepository<IKanalIslemRepository>();

                // Eğer sıra 0 veya belirtilmemişse, otomatik sıra ata
                if (request.Sira <= 0)
                {
                    var maxSira = await kanalIslemRepo.GetMaxSiraByKanalAndBinaAsync(request.KanalId, request.HizmetBinasiId);
                    request.Sira = maxSira + 1;
                }

                var kanalIslem = _mapper.Map<KanalIslem>(request);
                kanalIslem.Aktiflik = Aktiflik.Aktif;

                await kanalIslemRepo.AddAsync(kanalIslem);
                await _unitOfWork.SaveChangesAsync();

                var kanalIslemDto = _mapper.Map<KanalIslemResponseDto>(kanalIslem);

                _logger.LogInformation("Yeni kanal işlem oluşturuldu. ID: {KanalIslemId}, KanalId: {KanalId}, HizmetBinasiId: {HizmetBinasiId}",
                    kanalIslem.KanalIslemId, kanalIslem.KanalId, kanalIslem.HizmetBinasiId);

                return ApiResponseDto<KanalIslemResponseDto>
                    .SuccessResult(kanalIslemDto, "Kanal işlem başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal işlem oluşturulurken hata oluştu. KanalId: {KanalId}", request.KanalId);
                return ApiResponseDto<KanalIslemResponseDto>
                    .ErrorResult("Kanal işlem oluşturulurken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KanalIslemResponseDto>> UpdateAsync(int kanalIslemId, KanalIslemUpdateRequestDto request)
        {
            try
            {
                // Validation
                if (kanalIslemId <= 0)
                {
                    return ApiResponseDto<KanalIslemResponseDto>
                        .ErrorResult("Geçersiz kanal işlem ID");
                }

                if (request.KanalId <= 0)
                {
                    return ApiResponseDto<KanalIslemResponseDto>
                        .ErrorResult("Kanal seçimi zorunludur");
                }

                if (request.HizmetBinasiId <= 0)
                {
                    return ApiResponseDto<KanalIslemResponseDto>
                        .ErrorResult("Hizmet binası seçimi zorunludur");
                }

                if (request.BaslangicNumara >= request.BitisNumara)
                {
                    return ApiResponseDto<KanalIslemResponseDto>
                        .ErrorResult("Başlangıç numarası, bitiş numarasından küçük olmalıdır");
                }

                var kanalIslemRepo = _unitOfWork.GetRepository<IKanalIslemRepository>();
                var kanalIslem = await kanalIslemRepo.GetByIdAsync(kanalIslemId);

                if (kanalIslem == null)
                {
                    return ApiResponseDto<KanalIslemResponseDto>
                        .ErrorResult("Kanal işlem bulunamadı");
                }

                if (kanalIslem.SilindiMi)
                {
                    return ApiResponseDto<KanalIslemResponseDto>
                        .ErrorResult("Kanal işlem silinmiş olduğu için güncellenemez.");
                }

                var oldAktiflik = kanalIslem.Aktiflik;

                // Üst kaydın (Kanal) silinmiş veya pasif olup olmadığını kontrol et
                var kanalRepo = _unitOfWork.GetRepository<IKanalRepository>();
                var kanal = await kanalRepo.GetByIdAsync(request.KanalId);

                if (kanal == null || kanal.SilindiMi)
                {
                    return ApiResponseDto<KanalIslemResponseDto>
                        .ErrorResult("Bağlı olduğu Kanal silinmiş olduğu için bu kanal işlem güncellenemez.");
                }

                if (request.Aktiflik == Aktiflik.Aktif && kanal.Aktiflik != Aktiflik.Aktif)
                {
                    return ApiResponseDto<KanalIslemResponseDto>
                        .ErrorResult("Bağlı olduğu Kanal pasif durumda olduğu için bu kanal işlem aktif edilemez. Önce Kanal'ı aktif ediniz.");
                }

                // ⭐ Field-level permission enforcement
                var userPermissions = new Dictionary<string, BusinessObjectLayer.Enums.Common.YetkiSeviyesi>();
                var originalDto = _mapper.Map<KanalIslemUpdateRequestDto>(kanalIslem);

                var unauthorizedFields = await _fieldPermissionService.ValidateFieldPermissionsAsync(
                    request,
                    userPermissions,
                    originalDto,
                    "SIR.KANALISLEM.MANAGE",
                    null);

                if (unauthorizedFields.Any())
                {
                    _fieldPermissionService.RevertUnauthorizedFields(request, originalDto, unauthorizedFields);
                    _logger.LogWarning(
                        "KanalIslemService.UpdateAsync - Field-level permission enforcement: {Count} alan revert edildi.",
                        unauthorizedFields.Count);
                }

                // Update
                kanalIslem.KanalId = request.KanalId;
                kanalIslem.HizmetBinasiId = request.HizmetBinasiId;
                kanalIslem.Sira = request.Sira;
                kanalIslem.BaslangicNumara = request.BaslangicNumara;
                kanalIslem.BitisNumara = request.BitisNumara;
                kanalIslem.Aktiflik = request.Aktiflik;
                kanalIslem.DuzenlenmeTarihi = DateTime.Now;

                kanalIslemRepo.Update(kanalIslem);

                // Cascade: Aktiflik değişmişse child kayıtları da güncelle
                if (oldAktiflik != request.Aktiflik)
                {
                    await CascadeAktiflikUpdateAsync(kanalIslemId, request.Aktiflik);
                }

                await _unitOfWork.SaveChangesAsync();

                var kanalIslemDto = _mapper.Map<KanalIslemResponseDto>(kanalIslem);

                _logger.LogInformation("Kanal işlem güncellendi. ID: {KanalIslemId}, KanalId: {KanalId}",
                    kanalIslem.KanalIslemId, kanalIslem.KanalId);

                return ApiResponseDto<KanalIslemResponseDto>
                    .SuccessResult(kanalIslemDto, "Kanal işlem başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal işlem güncellenirken hata oluştu. ID: {KanalIslemId}", kanalIslemId);
                return ApiResponseDto<KanalIslemResponseDto>
                    .ErrorResult("Kanal işlem güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int kanalIslemId)
        {
            try
            {
                if (kanalIslemId <= 0)
                {
                    return ApiResponseDto<bool>
                        .ErrorResult("Geçersiz kanal işlem ID");
                }

                var kanalIslemRepo = _unitOfWork.GetRepository<IKanalIslemRepository>();
                var kanalIslem = await kanalIslemRepo.GetByIdAsync(kanalIslemId);

                if (kanalIslem == null)
                {
                    return ApiResponseDto<bool>
                        .ErrorResult("Kanal işlem bulunamadı");
                }

                // Cascade: Child kayıtları da sil (soft delete)
                await CascadeDeleteAsync(kanalIslemId);

                // Soft delete
                kanalIslem.SilindiMi = true;
                kanalIslem.DuzenlenmeTarihi = DateTime.Now;

                kanalIslemRepo.Update(kanalIslem);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Kanal işlem silindi. ID: {KanalIslemId}", kanalIslemId);

                return ApiResponseDto<bool>
                    .SuccessResult(true, "Kanal işlem başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal işlem silinirken hata oluştu. ID: {KanalIslemId}", kanalIslemId);
                return ApiResponseDto<bool>
                    .ErrorResult("Kanal işlem silinirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<KanalIslemResponseDto>>> GetByKanalIdAsync(int kanalId)
        {
            try
            {
                if (kanalId <= 0)
                {
                    return ApiResponseDto<List<KanalIslemResponseDto>>
                        .ErrorResult("Geçersiz kanal ID");
                }

                var kanalIslemRepo = _unitOfWork.GetRepository<IKanalIslemRepository>();
                var kanalIslemler = await kanalIslemRepo.GetByKanalAsync(kanalId);

                var kanalIslemDtos = _mapper.Map<List<KanalIslemResponseDto>>(kanalIslemler);

                return ApiResponseDto<List<KanalIslemResponseDto>>
                    .SuccessResult(kanalIslemDtos, "Kanal işlemler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal işlem listesi getirilirken hata oluştu. KanalId: {KanalId}", kanalId);
                return ApiResponseDto<List<KanalIslemResponseDto>>
                    .ErrorResult("Kanal işlemler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<KanalIslemResponseDto>>> GetByHizmetBinasiIdAsync(int hizmetBinasiId)
        {
            try
            {
                if (hizmetBinasiId <= 0)
                {
                    return ApiResponseDto<List<KanalIslemResponseDto>>
                        .ErrorResult("Geçersiz hizmet binası ID");
                }

                var kanalIslemRepo = _unitOfWork.GetRepository<IKanalIslemRepository>();
                var kanalIslemler = await kanalIslemRepo.GetByHizmetBinasiAsync(hizmetBinasiId);

                var kanalIslemDtos = _mapper.Map<List<KanalIslemResponseDto>>(kanalIslemler);

                return ApiResponseDto<List<KanalIslemResponseDto>>
                    .SuccessResult(kanalIslemDtos, "Kanal işlemler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal işlem listesi getirilirken hata oluştu. HizmetBinasiId: {HizmetBinasiId}", hizmetBinasiId);
                return ApiResponseDto<List<KanalIslemResponseDto>>
                    .ErrorResult("Kanal işlemler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<KanalIslemResponseDto>>> GetActiveAsync()
        {
            try
            {
                var kanalIslemRepo = _unitOfWork.GetRepository<IKanalIslemRepository>();
                var kanalIslemler = await kanalIslemRepo.GetActiveAsync();

                var kanalIslemDtos = _mapper.Map<List<KanalIslemResponseDto>>(kanalIslemler);

                return ApiResponseDto<List<KanalIslemResponseDto>>
                    .SuccessResult(kanalIslemDtos, "Aktif kanal işlemler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif kanal işlem listesi getirilirken hata oluştu");
                return ApiResponseDto<List<KanalIslemResponseDto>>
                    .ErrorResult("Aktif kanal işlemler getirilirken bir hata oluştu", ex.Message);
            }
        }

        /// <summary>
        /// Cascade delete: KanalIslem silindiğinde child kayıtları da siler
        /// CascadeHelper kullanarak tracking conflict'leri otomatik handle eder
        /// </summary>
        private async Task CascadeDeleteAsync(int kanalIslemId)
        {
            await _cascadeHelper.CascadeSoftDeleteAsync<KanalAltIslem>(x => x.KanalIslemId == kanalIslemId);
            _logger.LogInformation("Cascade delete: KanalIslemId={KanalIslemId}", kanalIslemId);
        }

        /// <summary>
        /// Cascade update: KanalIslem Aktiflik değiştiğinde child kayıtları da günceller
        /// CascadeHelper kullanarak tracking conflict'leri otomatik handle eder
        /// </summary>
        private async Task CascadeAktiflikUpdateAsync(int kanalIslemId, Aktiflik yeniAktiflik)
        {
            await _cascadeHelper.CascadeAktiflikUpdateAsync<KanalAltIslem>(x => x.KanalIslemId == kanalIslemId, yeniAktiflik);
            _logger.LogInformation("Cascade aktiflik update: KanalIslemId={KanalIslemId}, YeniAktiflik={Aktiflik}", kanalIslemId, yeniAktiflik);
        }
    }
}