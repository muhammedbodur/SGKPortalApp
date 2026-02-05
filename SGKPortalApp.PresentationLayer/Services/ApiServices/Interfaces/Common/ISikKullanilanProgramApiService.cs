using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common
{
    public interface ISikKullanilanProgramApiService
    {
        Task<ServiceResult<List<SikKullanilanProgramResponseDto>>> GetAllAsync();
        Task<ServiceResult<List<SikKullanilanProgramResponseDto>>> GetActiveAsync();
        Task<ServiceResult<SikKullanilanProgramResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<SikKullanilanProgramResponseDto>> CreateAsync(SikKullanilanProgramCreateRequestDto request);
        Task<ServiceResult<SikKullanilanProgramResponseDto>> UpdateAsync(int id, SikKullanilanProgramUpdateRequestDto request);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<PagedResponseDto<SikKullanilanProgramResponseDto>>> GetPagedAsync(SikKullanilanProgramFilterRequestDto filter);
    }
}
