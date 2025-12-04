using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Siramatik
{
    /// <summary>
    /// Kiosk Simülatör API Servisi
    /// Masaüstü kiosk uygulamasını simüle eder
    /// </summary>
    public class KioskSimulatorApiService : IKioskSimulatorApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<KioskSimulatorApiService> _logger;
        private const string BaseEndpoint = "siramatik/kiosk-sira-alma";

        public KioskSimulatorApiService(HttpClient httpClient, ILogger<KioskSimulatorApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Tüm aktif kiosk'ları listele
        /// </summary>
        public async Task<ServiceResult<List<KioskResponseDto>>> GetAllKiosklarAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("siramatik/kiosk");
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllKiosklarAsync failed: {Error}", error);
                    return ServiceResult<List<KioskResponseDto>>.Fail("Kiosk listesi alınamadı");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<KioskResponseDto>>>();
                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<KioskResponseDto>>.Ok(apiResponse.Data, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<List<KioskResponseDto>>.Fail(apiResponse?.Message ?? "Kiosk listesi alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllKiosklarAsync exception");
                return ServiceResult<List<KioskResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        /// <summary>
        /// Belirli bir kiosk'un menülerini listele (Yeni Yapı)
        /// </summary>
        public async Task<ServiceResult<List<KioskMenuDto>>> GetKioskMenulerByKioskIdAsync(int kioskId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseEndpoint}/menuler-by-kiosk/{kioskId}");
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetKioskMenulerByKioskIdAsync failed: {Error}", error);
                    return ServiceResult<List<KioskMenuDto>>.Fail("Menüler alınamadı");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<KioskMenuDto>>>();
                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<KioskMenuDto>>.Ok(apiResponse.Data, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<List<KioskMenuDto>>.Fail(apiResponse?.Message ?? "Menüler alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetKioskMenulerByKioskIdAsync exception");
                return ServiceResult<List<KioskMenuDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        /// <summary>
        /// Seçilen menüdeki alt işlemleri kiosk bazlı listele (Yeni Yapı)
        /// </summary>
        public async Task<ServiceResult<List<KioskAltIslemDto>>> GetKioskMenuAltIslemleriByKioskIdAsync(int kioskId, int kioskMenuId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseEndpoint}/alt-islemler-by-kiosk/{kioskId}/{kioskMenuId}");
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetKioskMenuAltIslemleriByKioskIdAsync failed: {Error}", error);
                    return ServiceResult<List<KioskAltIslemDto>>.Fail("Alt işlemler alınamadı");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<KioskAltIslemDto>>>();
                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<KioskAltIslemDto>>.Ok(apiResponse.Data, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<List<KioskAltIslemDto>>.Fail(apiResponse?.Message ?? "Alt işlemler alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetKioskMenuAltIslemleriByKioskIdAsync exception");
                return ServiceResult<List<KioskAltIslemDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        // ESKİ METODLAR (Geriye uyumluluk için)
        /// <summary>
        /// [ESKİ] Hizmet binasındaki kiosk menülerini listele
        /// </summary>
        public async Task<ServiceResult<List<KioskMenuDto>>> GetKioskMenulerAsync(int hizmetBinasiId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseEndpoint}/menuler/{hizmetBinasiId}");
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetKioskMenulerAsync failed: {Error}", error);
                    return ServiceResult<List<KioskMenuDto>>.Fail("Menüler alınamadı");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<KioskMenuDto>>>();
                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<KioskMenuDto>>.Ok(apiResponse.Data, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<List<KioskMenuDto>>.Fail(apiResponse?.Message ?? "Menüler alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetKioskMenulerAsync exception");
                return ServiceResult<List<KioskMenuDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        /// <summary>
        /// [ESKİ] Seçilen kiosk menüsündeki alt işlemleri listele
        /// </summary>
        public async Task<ServiceResult<List<KioskAltIslemDto>>> GetKioskMenuAltIslemleriAsync(int hizmetBinasiId, int kioskMenuId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseEndpoint}/alt-islemler/{hizmetBinasiId}/{kioskMenuId}");
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetKioskMenuAltIslemleriAsync failed: {Error}", error);
                    return ServiceResult<List<KioskAltIslemDto>>.Fail("Alt işlemler alınamadı");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<KioskAltIslemDto>>>();
                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<KioskAltIslemDto>>.Ok(apiResponse.Data, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<List<KioskAltIslemDto>>.Fail(apiResponse?.Message ?? "Alt işlemler alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetKioskMenuAltIslemleriAsync exception");
                return ServiceResult<List<KioskAltIslemDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        /// <summary>
        /// Kiosk'tan sıra al
        /// </summary>
        public async Task<ServiceResult<KioskSiraAlResponseDto>> SiraAlAsync(KioskSiraAlRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseEndpoint}/sira-al", request);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("SiraAlAsync failed: {Error}", error);
                    
                    // Hata mesajını parse etmeye çalış
                    try
                    {
                        var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<KioskSiraAlResponseDto>>();
                        return ServiceResult<KioskSiraAlResponseDto>.Fail(errorResponse?.Message ?? "Sıra alınamadı");
                    }
                    catch
                    {
                        return ServiceResult<KioskSiraAlResponseDto>.Fail("Sıra alınamadı");
                    }
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<KioskSiraAlResponseDto>>();
                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<KioskSiraAlResponseDto>.Ok(apiResponse.Data, apiResponse.Message ?? "Sıra alındı");
                }

                return ServiceResult<KioskSiraAlResponseDto>.Fail(apiResponse?.Message ?? "Sıra alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SiraAlAsync exception");
                return ServiceResult<KioskSiraAlResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        /// <summary>
        /// Belirli bir işlem için bekleyen sıra sayısını getir
        /// </summary>
        public async Task<ServiceResult<int>> GetBekleyenSayisiAsync(int hizmetBinasiId, int kanalAltIslemId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseEndpoint}/bekleyen-sayi?hizmetBinasiId={hizmetBinasiId}&kanalAltIslemId={kanalAltIslemId}");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<int>.Fail("Bekleyen sayısı alınamadı");
                }

                var result = await response.Content.ReadFromJsonAsync<BekleyenSayisiResponse>();
                return ServiceResult<int>.Ok(result?.BekleyenSiraSayisi ?? 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBekleyenSayisiAsync exception");
                return ServiceResult<int>.Fail($"Hata: {ex.Message}");
            }
        }

        /// <summary>
        /// Belirli bir işlem için aktif personel var mı kontrol et
        /// </summary>
        public async Task<ServiceResult<bool>> HasAktifPersonelAsync(int hizmetBinasiId, int kanalAltIslemId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseEndpoint}/aktif-personel-var?hizmetBinasiId={hizmetBinasiId}&kanalAltIslemId={kanalAltIslemId}");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<bool>.Fail("Aktif personel kontrolü yapılamadı");
                }

                var result = await response.Content.ReadFromJsonAsync<AktifPersonelResponse>();
                return ServiceResult<bool>.Ok(result?.AktifPersonelVar ?? false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HasAktifPersonelAsync exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        // Response helper classes
        private class BekleyenSayisiResponse
        {
            public int BekleyenSiraSayisi { get; set; }
        }

        private class AktifPersonelResponse
        {
            public bool AktifPersonelVar { get; set; }
        }
    }
}
