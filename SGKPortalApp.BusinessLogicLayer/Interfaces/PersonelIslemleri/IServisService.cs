using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri
{
    public interface IServisService
    {
        Task<ApiResponseDto<List<ServisResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<ServisResponseDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<ServisResponseDto>> CreateAsync(ServisCreateRequestDto request);
        Task<ApiResponseDto<ServisResponseDto>> UpdateAsync(int id, ServisUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);

        Task<ApiResponseDto<List<ServisResponseDto>>> GetActiveAsync();
        Task<ApiResponseDto<ServisResponseDto>> GetByNameAsync(string servisAdi);
        Task<ApiResponseDto<PagedResponseDto<ServisResponseDto>>> GetPagedAsync(ServisFilterRequestDto filter);

        Task<ApiResponseDto<int>> GetPersonelCountAsync(int servisId);
        Task<ApiResponseDto<List<(int Id, string Ad)>>> GetDropdownAsync();
    }
}