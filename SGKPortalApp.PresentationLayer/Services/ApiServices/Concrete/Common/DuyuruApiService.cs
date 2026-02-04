using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Dashboard;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Common
{
    public class DuyuruApiService : IDuyuruApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DuyuruApiService> _logger;

        public DuyuruApiService(HttpClient httpClient, ILogger<DuyuruApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<DuyuruResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("duyuru");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllAsync failed: {Error}", errorContent);
                    return ServiceResult<List<DuyuruResponseDto>>.Fail("Duyurular listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<DuyuruResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<DuyuruResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<DuyuruResponseDto>>.Fail(
                    apiResponse?.Message ?? "Duyurular listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync exception");
                return ServiceResult<List<DuyuruResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<DuyuruResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"duyuru/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIdAsync failed: {Error}", errorContent);
                    return ServiceResult<DuyuruResponseDto>.Fail("Duyuru bulunamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<DuyuruResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<DuyuruResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<DuyuruResponseDto>.Fail(
                    apiResponse?.Message ?? "Duyuru bulunamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync exception");
                return ServiceResult<DuyuruResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<DuyuruResponseDto>> CreateAsync(DuyuruCreateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("duyuru", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("CreateAsync failed: {Error}", errorContent);
                    return ServiceResult<DuyuruResponseDto>.Fail("Duyuru oluşturulamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<DuyuruResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<DuyuruResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<DuyuruResponseDto>.Fail(
                    apiResponse?.Message ?? "Duyuru oluşturulamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync exception");
                return ServiceResult<DuyuruResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<DuyuruResponseDto>> UpdateAsync(int id, DuyuruUpdateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"duyuru/{id}", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("UpdateAsync failed: {Error}", errorContent);
                    return ServiceResult<DuyuruResponseDto>.Fail("Duyuru güncellenemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<DuyuruResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<DuyuruResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<DuyuruResponseDto>.Fail(
                    apiResponse?.Message ?? "Duyuru güncellenemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync exception");
                return ServiceResult<DuyuruResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"duyuru/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("DeleteAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("Duyuru silinemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(
                        true,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "Duyuru silinemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<string>> UploadImageAsync(Stream fileStream, string fileName)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                using var streamContent = new StreamContent(fileStream);

                var extension = Path.GetExtension(fileName).ToLowerInvariant();
                var contentType = extension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".webp" => "image/webp",
                    _ => "application/octet-stream"
                };

                streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                content.Add(streamContent, "file", fileName);

                var response = await _httpClient.PostAsync("duyuru/upload-image", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("UploadImageAsync failed: {Error}", errorContent);
                    return ServiceResult<string>.Fail("Görsel yüklenemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<string>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<string>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "Görsel başarıyla yüklendi"
                    );
                }

                return ServiceResult<string>.Fail(
                    apiResponse?.Message ?? "Görsel yüklenemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UploadImageAsync exception");
                return ServiceResult<string>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}
