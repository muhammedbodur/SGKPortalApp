using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.Common.Helpers;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Complex;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Services.SiramatikIslemleri
{
    public class KanalAltIslemService : IKanalAltIslemService
    {
        private readonly ISiramatikQueryRepository _siramatikQueryRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<KanalAltIslemService> _logger;
        private readonly ICascadeHelper _cascadeHelper;

        public KanalAltIslemService(
            ISiramatikQueryRepository siramatikQueryRepo,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<KanalAltIslemService> logger,
            ICascadeHelper cascadeHelper)
        {
            _siramatikQueryRepo = siramatikQueryRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cascadeHelper = cascadeHelper;
        }

        public async Task<ApiResponseDto<List<KanalAltIslemResponseDto>>> GetAllAsync()
        {
            try
            {
                var kanalAltIslemler = await _siramatikQueryRepo.GetAllKanalAltIslemlerAsync();

                return ApiResponseDto<List<KanalAltIslemResponseDto>>
                    .SuccessResult(kanalAltIslemler, "Kanal alt işlemler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal alt işlem listesi getirilirken hata oluştu");
                return ApiResponseDto<List<KanalAltIslemResponseDto>>
                    .ErrorResult("Kanal alt işlemler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<KanalAltIslemResponseDto>>> GetByDepartmanHizmetBinasiIdAsync(int departmanHizmetBinasiId)
        {
            try
            {
                if (departmanHizmetBinasiId <= 0)
                {
                    return ApiResponseDto<List<KanalAltIslemResponseDto>>
                        .ErrorResult("Geçersiz departman-hizmet binası ID");
                }

                var kanalAltIslemler = await _siramatikQueryRepo
                    .GetKanalAltIslemlerByDepartmanHizmetBinasiIdAsync(departmanHizmetBinasiId);

                return ApiResponseDto<List<KanalAltIslemResponseDto>>
                    .SuccessResult(kanalAltIslemler, 
                        $"Hizmet binası için {kanalAltIslemler.Count} kanal alt işlem getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Departman-Hizmet binası için kanal alt işlemler getirilirken hata oluştu. DepartmanHizmetBinasiId: {DepartmanHizmetBinasiId}", 
                    departmanHizmetBinasiId);
                return ApiResponseDto<List<KanalAltIslemResponseDto>>
                    .ErrorResult("Kanal alt işlemler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KanalAltIslemResponseDto>> GetByIdWithDetailsAsync(int kanalAltIslemId)
        {
            try
            {
                if (kanalAltIslemId <= 0)
                {
                    return ApiResponseDto<KanalAltIslemResponseDto>
                        .ErrorResult("Geçersiz kanal alt işlem ID");
                }

                var kanalAltIslem = await _siramatikQueryRepo
                    .GetKanalAltIslemByIdWithDetailsAsync(kanalAltIslemId);

                if (kanalAltIslem == null)
                {
                    return ApiResponseDto<KanalAltIslemResponseDto>
                        .ErrorResult("Kanal alt işlem bulunamadı");
                }

                return ApiResponseDto<KanalAltIslemResponseDto>
                    .SuccessResult(kanalAltIslem, "Kanal alt işlem başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal alt işlem getirilirken hata oluştu. ID: {KanalAltIslemId}", 
                    kanalAltIslemId);
                return ApiResponseDto<KanalAltIslemResponseDto>
                    .ErrorResult("Kanal alt işlem getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<KanalAltIslemResponseDto>>> GetByKanalIslemIdAsync(int kanalIslemId)
        {
            try
            {
                if (kanalIslemId <= 0)
                {
                    return ApiResponseDto<List<KanalAltIslemResponseDto>>
                        .ErrorResult("Geçersiz kanal işlem ID");
                }

                var kanalAltIslemler = await _siramatikQueryRepo
                    .GetKanalAltIslemlerByKanalIslemIdAsync(kanalIslemId);

                return ApiResponseDto<List<KanalAltIslemResponseDto>>
                    .SuccessResult(kanalAltIslemler, 
                        $"Kanal işlem için {kanalAltIslemler.Count} kanal alt işlem getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal işlem için kanal alt işlemler getirilirken hata oluştu. KanalIslemId: {KanalIslemId}", 
                    kanalIslemId);
                return ApiResponseDto<List<KanalAltIslemResponseDto>>
                    .ErrorResult("Kanal alt işlemler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<Dictionary<int, int>>> GetPersonelSayilariAsync(int hizmetBinasiId)
        {
            try
            {
                if (hizmetBinasiId <= 0)
                {
                    return ApiResponseDto<Dictionary<int, int>>
                        .ErrorResult("Geçersiz hizmet binası ID");
                }

                var personelSayilari = await _siramatikQueryRepo
                    .GetKanalAltIslemPersonelSayilariAsync(hizmetBinasiId);

                return ApiResponseDto<Dictionary<int, int>>
                    .SuccessResult(personelSayilari, "Personel sayıları başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel sayıları getirilirken hata oluştu. HizmetBinasiId: {HizmetBinasiId}", 
                    hizmetBinasiId);
                return ApiResponseDto<Dictionary<int, int>>
                    .ErrorResult("Personel sayıları getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<KanalAltIslemResponseDto>>> GetEslestirmeYapilmamisAsync(int hizmetBinasiId)
        {
            try
            {
                if (hizmetBinasiId <= 0)
                {
                    return ApiResponseDto<List<KanalAltIslemResponseDto>>
                        .ErrorResult("Geçersiz hizmet binası ID");
                }

                var kanalAltIslemler = await _siramatikQueryRepo
                    .GetEslestirmeYapilmamisKanalAltIslemlerAsync(hizmetBinasiId);

                return ApiResponseDto<List<KanalAltIslemResponseDto>>
                    .SuccessResult(kanalAltIslemler, 
                        $"{kanalAltIslemler.Count} eşleştirilmemiş kanal alt işlem bulundu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Eşleştirilmemiş kanal alt işlemler getirilirken hata oluştu. HizmetBinasiId: {HizmetBinasiId}", 
                    hizmetBinasiId);
                return ApiResponseDto<List<KanalAltIslemResponseDto>>
                    .ErrorResult("Eşleştirilmemiş kanal alt işlemler getirilirken bir hata oluştu", ex.Message);
            }
        }

        // CRUD Operations
        public async Task<ApiResponseDto<KanalAltIslemResponseDto>> CreateAsync(KanalAltIslemCreateRequestDto request)
        {
            try
            {
                if (request.KanalAltId <= 0)
                {
                    return ApiResponseDto<KanalAltIslemResponseDto>
                        .ErrorResult("Geçersiz kanal alt ID");
                }

                if (request.KanalIslemId <= 0)
                {
                    return ApiResponseDto<KanalAltIslemResponseDto>
                        .ErrorResult("Geçersiz kanal işlem ID");
                }

                // Üst kayıtların silinmiş veya pasif olup olmadığını kontrol et
                var kanalIslemRepo = _unitOfWork.GetRepository<IKanalIslemRepository>();
                var kanalIslem = await kanalIslemRepo.GetByIdAsync(request.KanalIslemId);
                
                if (kanalIslem == null || kanalIslem.SilindiMi)
                {
                    return ApiResponseDto<KanalAltIslemResponseDto>
                        .ErrorResult("Bağlı olduğu Kanal İşlem silinmiş olduğu için bu alt işlem eklenemez.");
                }
                
                var kanalAltRepo = _unitOfWork.GetRepository<IKanalAltRepository>();
                var kanalAlt = await kanalAltRepo.GetByIdAsync(request.KanalAltId);
                
                if (kanalAlt == null || kanalAlt.SilindiMi)
                {
                    return ApiResponseDto<KanalAltIslemResponseDto>
                        .ErrorResult("Bağlı olduğu Alt Kanal silinmiş olduğu için bu alt işlem eklenemez.");
                }
                
                // Aktif olarak eklenmeye çalışılıyorsa, üst kayıtların aktif olup olmadığını kontrol et
                if (request.Aktiflik == BusinessObjectLayer.Enums.Common.Aktiflik.Aktif)
                {
                    if (kanalIslem.Aktiflik != BusinessObjectLayer.Enums.Common.Aktiflik.Aktif)
                    {
                        return ApiResponseDto<KanalAltIslemResponseDto>
                            .ErrorResult("Bağlı olduğu Kanal İşlem pasif durumda olduğu için bu alt işlem aktif olarak eklenemez. Önce Kanal İşlem'i aktif ediniz.");
                    }
                    
                    if (kanalAlt.Aktiflik != BusinessObjectLayer.Enums.Common.Aktiflik.Aktif)
                    {
                        return ApiResponseDto<KanalAltIslemResponseDto>
                            .ErrorResult("Bağlı olduğu Alt Kanal pasif durumda olduğu için bu alt işlem aktif olarak eklenemez. Önce Alt Kanal'ı aktif ediniz.");
                    }
                }

                var kanalAltIslem = _mapper.Map<KanalAltIslem>(request);
                kanalAltIslem.Aktiflik = request.Aktiflik;

                var kanalAltIslemRepo = _unitOfWork.GetRepository<IKanalAltIslemRepository>();
                await kanalAltIslemRepo.AddAsync(kanalAltIslem);
                await _unitOfWork.SaveChangesAsync();

                var kanalAltIslemDto = _mapper.Map<KanalAltIslemResponseDto>(kanalAltIslem);

                _logger.LogInformation("Yeni kanal alt işlem oluşturuldu. ID: {KanalAltIslemId}", 
                    kanalAltIslem.KanalAltIslemId);

                return ApiResponseDto<KanalAltIslemResponseDto>
                    .SuccessResult(kanalAltIslemDto, "Kanal alt işlem başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal alt işlem oluşturulurken hata oluştu");
                return ApiResponseDto<KanalAltIslemResponseDto>
                    .ErrorResult("Kanal alt işlem oluşturulurken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KanalAltIslemResponseDto>> UpdateAsync(int kanalAltIslemId, KanalAltIslemUpdateRequestDto request)
        {
            try
            {
                if (kanalAltIslemId <= 0)
                {
                    return ApiResponseDto<KanalAltIslemResponseDto>
                        .ErrorResult("Geçersiz kanal alt işlem ID");
                }

                if (request.KanalAltId <= 0)
                {
                    return ApiResponseDto<KanalAltIslemResponseDto>
                        .ErrorResult("Geçersiz kanal alt ID");
                }

                if (request.KanalIslemId <= 0)
                {
                    return ApiResponseDto<KanalAltIslemResponseDto>
                        .ErrorResult("Geçersiz kanal işlem ID");
                }

                var kanalAltIslemRepo = _unitOfWork.GetRepository<IKanalAltIslemRepository>();
                var kanalAltIslem = await kanalAltIslemRepo.GetByIdAsync(kanalAltIslemId);

                if (kanalAltIslem == null)
                {
                    return ApiResponseDto<KanalAltIslemResponseDto>
                        .ErrorResult("Kanal alt işlem bulunamadı");
                }

                var oldAktiflik = kanalAltIslem.Aktiflik;

                // Üst kayıtların silinmiş veya pasif olup olmadığını kontrol et
                var kanalIslemRepo = _unitOfWork.GetRepository<IKanalIslemRepository>();
                var kanalIslem = await kanalIslemRepo.GetByIdAsync(request.KanalIslemId);
                
                if (kanalIslem == null || kanalIslem.SilindiMi)
                {
                    return ApiResponseDto<KanalAltIslemResponseDto>
                        .ErrorResult("Bağlı olduğu Kanal İşlem silinmiş olduğu için bu alt işlem güncellenemez.");
                }
                
                var kanalAltRepo = _unitOfWork.GetRepository<IKanalAltRepository>();
                var kanalAlt = await kanalAltRepo.GetByIdAsync(request.KanalAltId);
                
                if (kanalAlt == null || kanalAlt.SilindiMi)
                {
                    return ApiResponseDto<KanalAltIslemResponseDto>
                        .ErrorResult("Bağlı olduğu Alt Kanal silinmiş olduğu için bu alt işlem güncellenemez.");
                }
                
                // Aktif edilmeye çalışılıyorsa, üst kayıtların aktif olup olmadığını kontrol et
                if (request.Aktiflik == BusinessObjectLayer.Enums.Common.Aktiflik.Aktif)
                {
                    if (kanalIslem.Aktiflik != BusinessObjectLayer.Enums.Common.Aktiflik.Aktif)
                    {
                        return ApiResponseDto<KanalAltIslemResponseDto>
                            .ErrorResult("Bağlı olduğu Kanal İşlem pasif durumda olduğu için bu alt işlem aktif edilemez. Önce Kanal İşlem'i aktif ediniz.");
                    }
                    
                    if (kanalAlt.Aktiflik != BusinessObjectLayer.Enums.Common.Aktiflik.Aktif)
                    {
                        return ApiResponseDto<KanalAltIslemResponseDto>
                            .ErrorResult("Bağlı olduğu Alt Kanal pasif durumda olduğu için bu alt işlem aktif edilemez. Önce Alt Kanal'ı aktif ediniz.");
                    }
                }

                // Update
                kanalAltIslem.KanalAltId = request.KanalAltId;
                kanalAltIslem.KanalIslemId = request.KanalIslemId;
                kanalAltIslem.DepartmanHizmetBinasiId = request.DepartmanHizmetBinasiId;
                kanalAltIslem.Aktiflik = request.Aktiflik;
                kanalAltIslem.DuzenlenmeTarihi = DateTimeHelper.Now;

                kanalAltIslemRepo.Update(kanalAltIslem);

                // Cascade: Aktiflik değişmişse child kayıtları da güncelle
                if (oldAktiflik != request.Aktiflik)
                {
                    await CascadeAktiflikUpdateAsync(kanalAltIslemId, request.Aktiflik);
                }

                await _unitOfWork.SaveChangesAsync();

                var kanalAltIslemDto = _mapper.Map<KanalAltIslemResponseDto>(kanalAltIslem);

                _logger.LogInformation("Kanal alt işlem güncellendi. ID: {KanalAltIslemId}", 
                    kanalAltIslem.KanalAltIslemId);

                return ApiResponseDto<KanalAltIslemResponseDto>
                    .SuccessResult(kanalAltIslemDto, "Kanal alt işlem başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal alt işlem güncellenirken hata oluştu. ID: {KanalAltIslemId}", kanalAltIslemId);
                return ApiResponseDto<KanalAltIslemResponseDto>
                    .ErrorResult("Kanal alt işlem güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int kanalAltIslemId)
        {
            try
            {
                if (kanalAltIslemId <= 0)
                {
                    return ApiResponseDto<bool>
                        .ErrorResult("Geçersiz kanal alt işlem ID");
                }

                var kanalAltIslemRepo = _unitOfWork.GetRepository<IKanalAltIslemRepository>();
                var kanalAltIslem = await kanalAltIslemRepo.GetByIdAsync(kanalAltIslemId);

                if (kanalAltIslem == null)
                {
                    return ApiResponseDto<bool>
                        .ErrorResult("Kanal alt işlem bulunamadı");
                }

                // Cascade: Child kayıtları da sil (soft delete)
                await CascadeDeleteAsync(kanalAltIslemId);

                // Soft delete
                kanalAltIslem.SilindiMi = true;
                kanalAltIslem.DuzenlenmeTarihi = DateTimeHelper.Now;

                kanalAltIslemRepo.Update(kanalAltIslem);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Kanal alt işlem silindi. ID: {KanalAltIslemId}", kanalAltIslemId);

                return ApiResponseDto<bool>
                    .SuccessResult(true, "Kanal alt işlem başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal alt işlem silinirken hata oluştu. ID: {KanalAltIslemId}", kanalAltIslemId);
                return ApiResponseDto<bool>
                    .ErrorResult("Kanal alt işlem silinirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KanalAltIslemResponseDto>> GetByIdAsync(int kanalAltIslemId)
        {
            try
            {
                if (kanalAltIslemId <= 0)
                {
                    return ApiResponseDto<KanalAltIslemResponseDto>
                        .ErrorResult("Geçersiz kanal alt işlem ID");
                }

                var kanalAltIslemRepo = _unitOfWork.GetRepository<IKanalAltIslemRepository>();
                var kanalAltIslem = await kanalAltIslemRepo.GetByIdAsync(kanalAltIslemId);

                if (kanalAltIslem == null)
                {
                    return ApiResponseDto<KanalAltIslemResponseDto>
                        .ErrorResult("Kanal alt işlem bulunamadı");
                }

                var kanalAltIslemDto = _mapper.Map<KanalAltIslemResponseDto>(kanalAltIslem);

                return ApiResponseDto<KanalAltIslemResponseDto>
                    .SuccessResult(kanalAltIslemDto, "Kanal alt işlem başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal alt işlem getirilirken hata oluştu. ID: {KanalAltIslemId}", kanalAltIslemId);
                return ApiResponseDto<KanalAltIslemResponseDto>
                    .ErrorResult("Kanal alt işlem getirilirken bir hata oluştu", ex.Message);
            }
        }

        /// <summary>
        /// Cascade delete: KanalAltIslem silindiğinde child kayıtları da siler
        /// CascadeHelper kullanarak tracking conflict'leri otomatik handle eder
        /// </summary>
        private async Task CascadeDeleteAsync(int kanalAltIslemId)
        {
            // Sira kayıtlarını soft delete
            await _cascadeHelper.CascadeSoftDeleteAsync<Sira>(x => x.KanalAltIslemId == kanalAltIslemId);
            
            // KanalPersonel kayıtlarını soft delete
            await _cascadeHelper.CascadeSoftDeleteAsync<KanalPersonel>(x => x.KanalAltIslemId == kanalAltIslemId);

            _logger.LogInformation("Cascade delete: KanalAltIslemId={KanalAltIslemId}", kanalAltIslemId);
        }

        /// <summary>
        /// Cascade update: KanalAltIslem Aktiflik değiştiğinde child kayıtları da günceller
        /// CascadeHelper kullanarak tracking conflict'leri otomatik handle eder
        /// NOT: Sira entity'sinde Aktiflik field'ı yok - CascadeHelper otomatik olarak atlıyor
        /// </summary>
        private async Task CascadeAktiflikUpdateAsync(int kanalAltIslemId, BusinessObjectLayer.Enums.Common.Aktiflik yeniAktiflik)
        {
            // Sira kayıtlarını güncelle - Aktiflik field'ı yok, CascadeHelper otomatik atlayacak
            // Ama SilindiMi ile yönetilmesi gerekiyorsa özel işlem:
            if (yeniAktiflik == BusinessObjectLayer.Enums.Common.Aktiflik.Pasif)
            {
                // Pasif yapıldığında Sira'ları soft delete yap
                await _cascadeHelper.CascadeSoftDeleteAsync<Sira>(x => x.KanalAltIslemId == kanalAltIslemId);
            }

            // KanalPersonel kayıtlarını güncelle (Aktiflik property'si var)
            await _cascadeHelper.CascadeAktiflikUpdateAsync<KanalPersonel>(x => x.KanalAltIslemId == kanalAltIslemId, yeniAktiflik);

            _logger.LogInformation("Cascade aktiflik update: KanalAltIslemId={KanalAltIslemId}, YeniAktiflik={Aktiflik}", kanalAltIslemId, yeniAktiflik);
        }
    }
}
