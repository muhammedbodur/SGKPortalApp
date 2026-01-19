using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common
{
    public interface IResmiTatilApiService
    {
        Task<ApiResponseDto<List<ResmiTatilResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<ResmiTatilResponseDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<List<ResmiTatilResponseDto>>> GetByYearAsync(int year);
        Task<ApiResponseDto<ResmiTatilResponseDto>> CreateAsync(ResmiTatilCreateRequestDto request);
        Task<ApiResponseDto<ResmiTatilResponseDto>> UpdateAsync(ResmiTatilUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);
        Task<ApiResponseDto<bool>> IsHolidayAsync(DateTime date);
        Task<ApiResponseDto<string>> GetHolidayNameAsync(DateTime date);
        Task<ApiResponseDto<int>> SyncHolidaysFromGoogleCalendarAsync(ResmiTatilSyncRequestDto request);
    }
}
