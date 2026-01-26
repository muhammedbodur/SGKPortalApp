using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Common
{
    public interface IDepartmanHizmetBinasiService
    {
        Task<ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<DepartmanHizmetBinasiResponseDto>> GetByIdAsync(int departmanHizmetBinasiId);
        Task<ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>> GetByDepartmanAsync(int departmanId);
        Task<ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>> GetByHizmetBinasiAsync(int hizmetBinasiId);
        Task<ApiResponseDto<DepartmanHizmetBinasiResponseDto>> CreateAsync(DepartmanHizmetBinasiCreateRequestDto request);
        Task<ApiResponseDto<DepartmanHizmetBinasiResponseDto>> UpdateAsync(int departmanHizmetBinasiId, DepartmanHizmetBinasiUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int departmanHizmetBinasiId);
        Task<ApiResponseDto<List<DepartmanHizmetBinasiDto>>> GetAllForDropdownAsync();
        Task<ApiResponseDto<List<DepartmanHizmetBinasiDto>>> GetDropdownByDepartmanAsync(int departmanId);
        Task<ApiResponseDto<List<DepartmanHizmetBinasiDto>>> GetDropdownByHizmetBinasiAsync(int hizmetBinasiId);
        Task<ApiResponseDto<int>> GetDepartmanHizmetBinasiIdAsync(int departmanId, int hizmetBinasiId);
    }
}
