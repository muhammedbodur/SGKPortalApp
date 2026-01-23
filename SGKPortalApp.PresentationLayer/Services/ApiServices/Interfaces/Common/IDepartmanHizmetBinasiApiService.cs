using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common
{
    public interface IDepartmanHizmetBinasiApiService
    {
        Task<ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<DepartmanHizmetBinasiResponseDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>> GetByDepartmanAsync(int departmanId);
        Task<ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>> GetByHizmetBinasiAsync(int hizmetBinasiId);
        Task<ApiResponseDto<List<DepartmanHizmetBinasiDto>>> GetDropdownAsync();
        Task<ApiResponseDto<List<DepartmanHizmetBinasiDto>>> GetDropdownByDepartmanAsync(int departmanId);
        Task<ApiResponseDto<List<DepartmanHizmetBinasiDto>>> GetDropdownByHizmetBinasiAsync(int hizmetBinasiId);
        Task<ApiResponseDto<DepartmanHizmetBinasiResponseDto>> CreateAsync(DepartmanHizmetBinasiCreateRequestDto request);
        Task<ApiResponseDto<DepartmanHizmetBinasiResponseDto>> UpdateAsync(int id, DepartmanHizmetBinasiUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);
    }
}
