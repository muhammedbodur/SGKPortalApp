using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Common
{
    public interface ISikKullanilanProgramService
    {
        Task<ApiResponseDto<List<SikKullanilanProgramResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<List<SikKullanilanProgramResponseDto>>> GetActiveAsync();
        Task<ApiResponseDto<SikKullanilanProgramResponseDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<SikKullanilanProgramResponseDto>> CreateAsync(SikKullanilanProgramCreateRequestDto request);
        Task<ApiResponseDto<SikKullanilanProgramResponseDto>> UpdateAsync(int id, SikKullanilanProgramUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);
        Task<ApiResponseDto<PagedResponseDto<SikKullanilanProgramResponseDto>>> GetPagedAsync(SikKullanilanProgramFilterRequestDto filter);
    }
}
