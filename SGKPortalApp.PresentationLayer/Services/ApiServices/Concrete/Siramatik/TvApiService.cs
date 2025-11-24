using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Siramatik
{
    public class TvApiService : ITvApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseEndpoint = "Tv";

        public TvApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponseDto<TvResponseDto>> CreateAsync(TvCreateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(BaseEndpoint, request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<TvResponseDto>>()
                    ?? ApiResponseDto<TvResponseDto>.ErrorResult("Yanıt alınamadı");
            }
            catch (HttpRequestException ex)
            {
                return ApiResponseDto<TvResponseDto>.ErrorResult($"API isteği başarısız: {ex.Message}");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<TvResponseDto>.ErrorResult($"API çağrısı başarısız: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<TvResponseDto>> UpdateAsync(TvUpdateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/{request.TvId}", request);
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<TvResponseDto>>()
                    ?? ApiResponseDto<TvResponseDto>.ErrorResult("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<TvResponseDto>.ErrorResult($"API çağrısı başarısız: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int tvId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseEndpoint}/{tvId}");
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>()
                    ?? ApiResponseDto<bool>.ErrorResult("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<bool>.ErrorResult($"API çağrısı başarısız: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<TvResponseDto>> GetByIdAsync(int tvId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseEndpoint}/{tvId}");
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<TvResponseDto>>()
                    ?? ApiResponseDto<TvResponseDto>.ErrorResult("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<TvResponseDto>.ErrorResult($"API çağrısı başarısız: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<List<TvResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(BaseEndpoint);
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<List<TvResponseDto>>>()
                    ?? ApiResponseDto<List<TvResponseDto>>.ErrorResult("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<List<TvResponseDto>>.ErrorResult($"API çağrısı başarısız: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<List<TvResponseDto>>> GetByHizmetBinasiAsync(int hizmetBinasiId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseEndpoint}/byhizmetbinasi/{hizmetBinasiId}");
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<List<TvResponseDto>>>()
                    ?? ApiResponseDto<List<TvResponseDto>>.ErrorResult("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<List<TvResponseDto>>.ErrorResult($"API çağrısı başarısız: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<List<TvResponseDto>>> GetByKatTipiAsync(KatTipi katTipi)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseEndpoint}/bykattipi/{(int)katTipi}");
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<List<TvResponseDto>>>()
                    ?? ApiResponseDto<List<TvResponseDto>>.ErrorResult("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<List<TvResponseDto>>.ErrorResult($"API çağrısı başarısız: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<List<TvResponseDto>>> GetActiveAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseEndpoint}/active");
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<List<TvResponseDto>>>()
                    ?? ApiResponseDto<List<TvResponseDto>>.ErrorResult("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<List<TvResponseDto>>.ErrorResult($"API çağrısı başarısız: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<TvResponseDto>> GetWithDetailsAsync(int tvId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseEndpoint}/details/{tvId}");
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<TvResponseDto>>()
                    ?? ApiResponseDto<TvResponseDto>.ErrorResult("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<TvResponseDto>.ErrorResult($"API çağrısı başarısız: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<List<(int Id, string Ad)>>> GetDropdownAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseEndpoint}/dropdown");
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<List<(int Id, string Ad)>>>()
                    ?? ApiResponseDto<List<(int Id, string Ad)>>.ErrorResult("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<List<(int Id, string Ad)>>.ErrorResult($"API çağrısı başarısız: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<List<(int Id, string Ad)>>> GetByHizmetBinasiDropdownAsync(int hizmetBinasiId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseEndpoint}/dropdown/byhizmetbinasi/{hizmetBinasiId}");
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<List<(int Id, string Ad)>>>()
                    ?? ApiResponseDto<List<(int Id, string Ad)>>.ErrorResult("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<List<(int Id, string Ad)>>.ErrorResult($"API çağrısı başarısız: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> AddBankoToTvAsync(int tvId, int bankoId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{BaseEndpoint}/{tvId}/banko/{bankoId}", null);
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>()
                    ?? ApiResponseDto<bool>.ErrorResult("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<bool>.ErrorResult($"API çağrısı başarısız: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> RemoveBankoFromTvAsync(int tvId, int bankoId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseEndpoint}/{tvId}/banko/{bankoId}");
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>()
                    ?? ApiResponseDto<bool>.ErrorResult("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<bool>.ErrorResult($"API çağrısı başarısız: {ex.Message}");
            }
        }
    }
}
