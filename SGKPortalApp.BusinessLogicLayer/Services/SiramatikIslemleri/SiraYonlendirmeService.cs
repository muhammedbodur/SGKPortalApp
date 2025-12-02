using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Services.SiramatikIslemleri
{
    /// <summary>
    /// Sıra Yönlendirme Servisi - Business Logic
    /// </summary>
    public class SiraYonlendirmeService : ISiraYonlendirmeService
    {
        private readonly ISiraRepository _siraRepository;
        private readonly IBankoRepository _bankoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SiraYonlendirmeService> _logger;

        public SiraYonlendirmeService(
            ISiraRepository siraRepository,
            IBankoRepository bankoRepository,
            IUnitOfWork unitOfWork,
            ILogger<SiraYonlendirmeService> logger)
        {
            _siraRepository = siraRepository;
            _bankoRepository = bankoRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponseDto<bool>> YonlendirSiraAsync(SiraYonlendirmeDto request)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    // Validasyonlar
                    if (request.SiraId <= 0)
                    {
                        return ApiResponseDto<bool>.ErrorResult("Geçersiz sıra ID");
                    }

                    if (string.IsNullOrWhiteSpace(request.YonlendirenPersonelTc) || request.YonlendirenPersonelTc.Length != 11)
                    {
                        return ApiResponseDto<bool>.ErrorResult("Geçersiz personel TC Kimlik No");
                    }

                    // Sırayı getir
                    var sira = await _siraRepository.GetSiraForYonlendirmeAsync(request.SiraId);
                    if (sira == null)
                    {
                        return ApiResponseDto<bool>.ErrorResult("Sıra bulunamadı");
                    }

                    // Sıra sadece Cagrildi durumundayken yönlendirilebilir
                    if (sira.BeklemeDurum != BeklemeDurum.Cagrildi)
                    {
                        return ApiResponseDto<bool>.ErrorResult("Sadece çağrılmış sıralar yönlendirilebilir");
                    }

                    // Kaynak ve hedef bankoları kontrol et
                    var kaynakBanko = await _bankoRepository.GetByIdAsync(request.YonlendirmeBankoId);
                    if (kaynakBanko == null)
                    {
                        return ApiResponseDto<bool>.ErrorResult("Kaynak banko bulunamadı");
                    }

                    var hedefBanko = await _bankoRepository.GetByIdAsync(request.HedefBankoId);
                    if (hedefBanko == null)
                    {
                        return ApiResponseDto<bool>.ErrorResult("Hedef banko bulunamadı");
                    }

                    // Aynı bankoya yönlendirme kontrolü
                    if (request.YonlendirmeBankoId == request.HedefBankoId)
                    {
                        return ApiResponseDto<bool>.ErrorResult("Sıra aynı bankoya yönlendirilemez");
                    }

                    // Yönlendirme işlemini yap
                    var result = await _siraRepository.YonlendirSiraAsync(
                        request.SiraId,
                        request.YonlendirmeBankoId,
                        request.HedefBankoId,
                        request.YonlendirenPersonelTc,
                        request.YonlendirmeTipi,
                        request.YonlendirmeNedeni
                    );

                    if (!result)
                    {
                        return ApiResponseDto<bool>.ErrorResult("Yönlendirme işlemi başarısız oldu");
                    }

                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogInformation(
                        "Sıra yönlendirildi. SiraId: {SiraId}, SiraNo: {SiraNo}, Kaynak: {KaynakBankoId}, Hedef: {HedefBankoId}, Tip: {YonlendirmeTipi}",
                        sira.SiraId, sira.SiraNo, request.YonlendirmeBankoId, request.HedefBankoId, request.YonlendirmeTipi);

                    return ApiResponseDto<bool>.SuccessResult(true, "Sıra başarıyla yönlendirildi");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Sıra yönlendirilirken hata oluştu. SiraId: {SiraId}", request.SiraId);
                    return ApiResponseDto<bool>.ErrorResult("Sıra yönlendirilirken bir hata oluştu", ex.Message);
                }
            });
        }

        public async Task<ApiResponseDto<int>> GetYonlendirilmisSiraCountAsync(int bankoId)
        {
            try
            {
                if (bankoId <= 0)
                {
                    return ApiResponseDto<int>.ErrorResult("Geçersiz banko ID");
                }

                var banko = await _bankoRepository.GetByIdAsync(bankoId);
                if (banko == null)
                {
                    return ApiResponseDto<int>.ErrorResult("Banko bulunamadı");
                }

                // Hedef banko olarak bu bankoya yönlendirilmiş sıraları say
                var siralar = await _siraRepository.GetAllWithDetailsAsync();
                var count = siralar.Count(s =>
                    s.HedefBankoId == bankoId &&
                    s.BeklemeDurum == BeklemeDurum.Yonlendirildi &&
                    !s.SilindiMi);

                return ApiResponseDto<int>.SuccessResult(count, $"{count} adet yönlendirilmiş sıra bulundu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Yönlendirilmiş sıra sayısı getirilirken hata oluştu. BankoId: {BankoId}", bankoId);
                return ApiResponseDto<int>.ErrorResult("Yönlendirilmiş sıra sayısı getirilirken bir hata oluştu", ex.Message);
            }
        }
    }
}
