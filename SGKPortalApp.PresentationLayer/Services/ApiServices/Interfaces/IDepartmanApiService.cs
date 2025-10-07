using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces
{
    public interface IDepartmanApiService
    {
        Task<ServiceResult<List<DepartmanResponseDto>>> GetAllAsync();
        Task<ServiceResult<DepartmanResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<DepartmanResponseDto>> CreateAsync(DepartmanCreateRequestDto request);
        Task<ServiceResult<DepartmanResponseDto>> UpdateAsync(int id, DepartmanUpdateRequestDto request);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<List<DepartmanResponseDto>>> GetActiveAsync();
        Task<ServiceResult<int>> GetPersonelCountAsync(int departmanId);
    }
}
