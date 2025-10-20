using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Complex;

namespace SGKPortalApp.BusinessLogicLayer.Services.SiramatikIslemleri
{
    public class KanalAltIslemService : IKanalAltIslemService
    {
        private readonly ISiramatikQueryRepository _siramatikQueryRepo;
        private readonly ILogger<KanalAltIslemService> _logger;

        public KanalAltIslemService(
            ISiramatikQueryRepository siramatikQueryRepo,
            ILogger<KanalAltIslemService> logger)
        {
            _siramatikQueryRepo = siramatikQueryRepo;
            _logger = logger;
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

        public async Task<ApiResponseDto<List<KanalAltIslemResponseDto>>> GetByHizmetBinasiIdAsync(int hizmetBinasiId)
        {
            try
            {
                if (hizmetBinasiId <= 0)
                {
                    return ApiResponseDto<List<KanalAltIslemResponseDto>>
                        .ErrorResult("Geçersiz hizmet binası ID");
                }

                var kanalAltIslemler = await _siramatikQueryRepo
                    .GetKanalAltIslemlerByHizmetBinasiIdAsync(hizmetBinasiId);

                return ApiResponseDto<List<KanalAltIslemResponseDto>>
                    .SuccessResult(kanalAltIslemler, 
                        $"Hizmet binası için {kanalAltIslemler.Count} kanal alt işlem getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hizmet binası için kanal alt işlemler getirilirken hata oluştu. HizmetBinasiId: {HizmetBinasiId}", 
                    hizmetBinasiId);
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
    }
}
