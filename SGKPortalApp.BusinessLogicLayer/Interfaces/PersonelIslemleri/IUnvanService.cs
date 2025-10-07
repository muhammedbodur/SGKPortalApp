using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri
{
    public interface IUnvanService
    {
        Task<ApiResponseDto<List<UnvanResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<UnvanResponseDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<UnvanResponseDto>> CreateAsync(UnvanCreateRequestDto request);
        Task<ApiResponseDto<UnvanResponseDto>> UpdateAsync(int id, UnvanUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);

        Task<ApiResponseDto<List<UnvanResponseDto>>> GetActiveAsync();
        Task<ApiResponseDto<UnvanResponseDto>> GetByNameAsync(string unvanAdi);
        Task<ApiResponseDto<PagedResponseDto<UnvanResponseDto>>> GetPagedAsync(UnvanFilterRequestDto filter);

        Task<ApiResponseDto<int>> GetPersonelCountAsync(int unvanId);
        Task<ApiResponseDto<List<(int Id, string Ad)>>> GetDropdownAsync();
    }
}