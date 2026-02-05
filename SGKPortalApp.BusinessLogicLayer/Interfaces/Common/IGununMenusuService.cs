using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using System;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Common
{
    public interface IGununMenusuService
    {
        Task<ApiResponseDto<List<GununMenusuResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<GununMenusuResponseDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<GununMenusuResponseDto>> CreateAsync(GununMenusuCreateRequestDto request);
        Task<ApiResponseDto<GununMenusuResponseDto>> UpdateAsync(int id, GununMenusuUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);
        Task<ApiResponseDto<PagedResponseDto<GununMenusuResponseDto>>> GetPagedAsync(GununMenusuFilterRequestDto filter);
        Task<ApiResponseDto<GununMenusuResponseDto?>> GetByDateAsync(DateTime date);
    }
}
