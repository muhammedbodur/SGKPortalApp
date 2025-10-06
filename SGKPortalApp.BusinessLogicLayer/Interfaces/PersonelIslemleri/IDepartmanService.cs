using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri
{
    public interface IDepartmanService
    {
        Task<ApiResponseDto<List<DepartmanResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<DepartmanResponseDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<DepartmanResponseDto>> CreateAsync(DepartmanCreateRequestDto request);
        Task<ApiResponseDto<DepartmanResponseDto>> UpdateAsync(int id, DepartmanUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);

        Task<ApiResponseDto<List<DepartmanResponseDto>>> GetActiveAsync();
        Task<ApiResponseDto<DepartmanResponseDto>> GetByNameAsync(string departmanAdi);
        Task<ApiResponseDto<PagedResponseDto<DepartmanResponseDto>>> GetPagedAsync(DepartmanFilterRequestDto filter);

        Task<ApiResponseDto<int>> GetPersonelCountAsync(int departmanId);
        Task<ApiResponseDto<List<(int Id, string Ad)>>> GetDropdownAsync();
    }
}