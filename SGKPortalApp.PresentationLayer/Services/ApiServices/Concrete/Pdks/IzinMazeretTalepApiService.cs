using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Pdks;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Pdks
{
    public class IzinMazeretTalepApiService : IIzinMazeretTalepApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<IzinMazeretTalepApiService> _logger;

        public IzinMazeretTalepApiService(HttpClient httpClient, ILogger<IzinMazeretTalepApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<IzinMazeretTalepResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"izin-mazeret-talep/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIdAsync failed: {Error}", errorContent);
                    return ServiceResult<IzinMazeretTalepResponseDto>.Fail("Talep bulunamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<IzinMazeretTalepResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<IzinMazeretTalepResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<IzinMazeretTalepResponseDto>.Fail(
                    apiResponse?.Message ?? "Talep bulunamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync Exception");
                return ServiceResult<IzinMazeretTalepResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IzinMazeretTalepResponseDto>> CreateAsync(IzinMazeretTalepCreateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("izin-mazeret-talep", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("CreateAsync failed: {Error}", errorContent);
                    
                    // API'den gelen hata mesajını parse et
                    try
                    {
                        var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<IzinMazeretTalepResponseDto>>();
                        if (errorResponse != null && !string.IsNullOrEmpty(errorResponse.Message))
                        {
                            return ServiceResult<IzinMazeretTalepResponseDto>.Fail(errorResponse.Message);
                        }
                    }
                    catch
                    {
                        // Parse edilemezse raw content'i kullan
                    }
                    
                    return ServiceResult<IzinMazeretTalepResponseDto>.Fail("Talep oluşturulamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<IzinMazeretTalepResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<IzinMazeretTalepResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "Talep başarıyla oluşturuldu"
                    );
                }

                return ServiceResult<IzinMazeretTalepResponseDto>.Fail(
                    apiResponse?.Message ?? "Talep oluşturulamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync Exception");
                return ServiceResult<IzinMazeretTalepResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IzinMazeretTalepResponseDto>> UpdateAsync(int id, IzinMazeretTalepUpdateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"izin-mazeret-talep/{id}", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("UpdateAsync failed: {Error}", errorContent);
                    
                    // API'den gelen hata mesajını parse et
                    try
                    {
                        var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<IzinMazeretTalepResponseDto>>();
                        if (errorResponse != null && !string.IsNullOrEmpty(errorResponse.Message))
                        {
                            return ServiceResult<IzinMazeretTalepResponseDto>.Fail(errorResponse.Message);
                        }
                    }
                    catch
                    {
                        // Parse edilemezse fallback mesaj
                    }
                    
                    return ServiceResult<IzinMazeretTalepResponseDto>.Fail("Talep güncellenemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<IzinMazeretTalepResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<IzinMazeretTalepResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "Talep başarıyla güncellendi"
                    );
                }

                return ServiceResult<IzinMazeretTalepResponseDto>.Fail(
                    apiResponse?.Message ?? "Talep güncellenemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync Exception");
                return ServiceResult<IzinMazeretTalepResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"izin-mazeret-talep/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("DeleteAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("Talep silinemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(true, apiResponse.Message ?? "Talep başarıyla silindi");
                }

                return ServiceResult<bool>.Fail(apiResponse?.Message ?? "Talep silinemedi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> CancelAsync(int id, string iptalNedeni)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"izin-mazeret-talep/{id}/cancel", iptalNedeni);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("CancelAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("Talep iptal edilemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(true, apiResponse.Message ?? "Talep başarıyla iptal edildi");
                }

                return ServiceResult<bool>.Fail(apiResponse?.Message ?? "Talep iptal edilemedi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CancelAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<IzinMazeretTalepListResponseDto>>> GetByPersonelAsync(string tcKimlikNo)
        {
            try
            {
                var response = await _httpClient.GetAsync($"izin-mazeret-talep/personel/{tcKimlikNo}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByPersonelAsync failed: {Error}", errorContent);
                    return ServiceResult<List<IzinMazeretTalepListResponseDto>>.Fail("Talepler alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<IzinMazeretTalepListResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<IzinMazeretTalepListResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<IzinMazeretTalepListResponseDto>>.Fail(
                    apiResponse?.Message ?? "Talepler alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByPersonelAsync Exception");
                return ServiceResult<List<IzinMazeretTalepListResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<IzinMazeretTalepListResponseDto>>> GetPendingApprovalsAsync(string sorumluTc)
        {
            try
            {
                var response = await _httpClient.GetAsync($"izin-mazeret-talep/pending-approvals/{sorumluTc}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetPendingApprovalsAsync failed: {Error}", errorContent);
                    return ServiceResult<List<IzinMazeretTalepListResponseDto>>.Fail("Onay bekleyen talepler alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<IzinMazeretTalepListResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<IzinMazeretTalepListResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<IzinMazeretTalepListResponseDto>>.Fail(
                    apiResponse?.Message ?? "Onay bekleyen talepler alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPendingApprovalsAsync Exception");
                return ServiceResult<List<IzinMazeretTalepListResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ApproveOrRejectAsync(int id, IzinMazeretTalepOnayRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"izin-mazeret-talep/{id}/approve", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("ApproveOrRejectAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("Onay işlemi yapılamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(true, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<bool>.Fail(apiResponse?.Message ?? "Onay işlemi yapılamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ApproveOrRejectAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<OverlapCheckResponseDto>> CheckOverlapAsync(IzinMazeretTalepCreateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("izin-mazeret-talep/check-overlap", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("CheckOverlapAsync failed: {Error}", errorContent);
                    return ServiceResult<OverlapCheckResponseDto>.Fail("Çakışma kontrolü yapılamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<OverlapCheckResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<OverlapCheckResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<OverlapCheckResponseDto>.Fail(
                    apiResponse?.Message ?? "Çakışma kontrolü yapılamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CheckOverlapAsync Exception");
                return ServiceResult<OverlapCheckResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<(List<IzinMazeretTalepListResponseDto> Items, int TotalCount)>> GetFilteredAsync(IzinMazeretTalepFilterRequestDto filter)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("izin-mazeret-talep/filter", filter);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetFilteredAsync failed: {Error}", errorContent);
                    return ServiceResult<(List<IzinMazeretTalepListResponseDto>, int)>.Fail("Filtrelenmiş talepler alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<(List<IzinMazeretTalepListResponseDto> Items, int TotalCount)>>();

                if (apiResponse?.Success == true && apiResponse.Data.Items != null)
                {
                    return ServiceResult<(List<IzinMazeretTalepListResponseDto>, int)>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<(List<IzinMazeretTalepListResponseDto>, int)>.Fail(
                    apiResponse?.Message ?? "Filtrelenmiş talepler alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFilteredAsync Exception");
                return ServiceResult<(List<IzinMazeretTalepListResponseDto>, int)>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri.PersonelResponseDto>>> GetAvailableApproversAsync(string tcKimlikNo)
        {
            try
            {
                var response = await _httpClient.GetAsync($"izin-mazeret-talep/available-approvers/{tcKimlikNo}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAvailableApproversAsync failed: {Error}", errorContent);
                    return ServiceResult<List<SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri.PersonelResponseDto>>.Fail("Onaycılar alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri.PersonelResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri.PersonelResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri.PersonelResponseDto>>.Fail(
                    apiResponse?.Message ?? "Onaycılar alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAvailableApproversAsync Exception");
                return ServiceResult<List<SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri.PersonelResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<IzinMazeretTuruResponseDto>>> GetAvailableLeaveTypesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("izin-mazeret-talep/leave-types");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAvailableLeaveTypesAsync failed: {Error}", errorContent);
                    return ServiceResult<List<IzinMazeretTuruResponseDto>>.Fail("İzin türleri alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<IzinMazeretTuruResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<IzinMazeretTuruResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<IzinMazeretTuruResponseDto>>.Fail(
                    apiResponse?.Message ?? "İzin türleri alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAvailableLeaveTypesAsync Exception");
                return ServiceResult<List<IzinMazeretTuruResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ProcessSgkIslemAsync(IzinSgkIslemRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("izin-mazeret-talep/sgk-islem", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("ProcessSgkIslemAsync failed: {Error}", errorContent);

                    try
                    {
                        var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
                        if (errorResponse != null && !string.IsNullOrEmpty(errorResponse.Message))
                        {
                            return ServiceResult<bool>.Fail(errorResponse.Message);
                        }
                    }
                    catch
                    {
                        // Parse edilemezse fallback
                    }

                    return ServiceResult<bool>.Fail("SGK işlemi yapılamadı");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<bool>.Fail(apiResponse?.Message ?? "SGK işlemi yapılamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProcessSgkIslemAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}
