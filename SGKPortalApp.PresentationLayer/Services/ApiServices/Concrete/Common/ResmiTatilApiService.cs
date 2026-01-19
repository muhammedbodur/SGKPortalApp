using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Common
{
    public class ResmiTatilApiService : IResmiTatilApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ResmiTatilApiService> _logger;
        private const string BaseUrl = "resmitatil";

        public ResmiTatilApiService(HttpClient httpClient, ILogger<ResmiTatilApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<ResmiTatilResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<List<ResmiTatilResponseDto>>>(BaseUrl);
                return response ?? ApiResponseDto<List<ResmiTatilResponseDto>>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resmi tatiller getirilirken hata oluştu");
                return ApiResponseDto<List<ResmiTatilResponseDto>>.ErrorResult("Resmi tatiller getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<ResmiTatilResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<ResmiTatilResponseDto>>($"{BaseUrl}/{id}");
                return response ?? ApiResponseDto<ResmiTatilResponseDto>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resmi tatil getirilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<ResmiTatilResponseDto>.ErrorResult("Resmi tatil getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<List<ResmiTatilResponseDto>>> GetByYearAsync(int year)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<List<ResmiTatilResponseDto>>>($"{BaseUrl}/year/{year}");
                return response ?? ApiResponseDto<List<ResmiTatilResponseDto>>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Yıla göre resmi tatiller getirilirken hata oluştu. Yıl: {Year}", year);
                return ApiResponseDto<List<ResmiTatilResponseDto>>.ErrorResult("Resmi tatiller getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<ResmiTatilResponseDto>> CreateAsync(ResmiTatilCreateRequestDto request)
        {
            try
            {
                var httpResponse = await _httpClient.PostAsJsonAsync(BaseUrl, request);
                var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponseDto<ResmiTatilResponseDto>>();
                return response ?? ApiResponseDto<ResmiTatilResponseDto>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resmi tatil eklenirken hata oluştu");
                return ApiResponseDto<ResmiTatilResponseDto>.ErrorResult("Resmi tatil eklenirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<ResmiTatilResponseDto>> UpdateAsync(ResmiTatilUpdateRequestDto request)
        {
            try
            {
                var httpResponse = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{request.TatilId}", request);
                var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponseDto<ResmiTatilResponseDto>>();
                return response ?? ApiResponseDto<ResmiTatilResponseDto>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resmi tatil güncellenirken hata oluştu");
                return ApiResponseDto<ResmiTatilResponseDto>.ErrorResult("Resmi tatil güncellenirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                var httpResponse = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
                var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
                return response ?? ApiResponseDto<bool>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resmi tatil silinirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<bool>.ErrorResult("Resmi tatil silinirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<bool>> IsHolidayAsync(DateTime date)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<bool>>($"{BaseUrl}/is-holiday?date={date:yyyy-MM-dd}");
                return response ?? ApiResponseDto<bool>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tatil kontrolü yapılırken hata oluştu");
                return ApiResponseDto<bool>.ErrorResult("Tatil kontrolü yapılırken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<string>> GetHolidayNameAsync(DateTime date)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<string>>($"{BaseUrl}/holiday-name?date={date:yyyy-MM-dd}");
                return response ?? ApiResponseDto<string>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tatil adı getirilirken hata oluştu");
                return ApiResponseDto<string>.ErrorResult("Tatil adı getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<int>> SyncHolidaysFromNagerDateAsync(ResmiTatilSyncRequestDto request)
        {
            try
            {
                var httpResponse = await _httpClient.PostAsJsonAsync($"{BaseUrl}/sync", request);
                var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponseDto<int>>();
                return response ?? ApiResponseDto<int>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resmi tatiller senkronize edilirken hata oluştu");
                return ApiResponseDto<int>.ErrorResult("Resmi tatiller senkronize edilirken hata oluştu");
            }
        }
    }
}
