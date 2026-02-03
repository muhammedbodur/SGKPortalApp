using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel
{
    public interface IUnvanApiService
    {
        Task<ServiceResult<List<UnvanResponseDto>>> GetAllAsync();
        Task<ServiceResult<UnvanResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<UnvanResponseDto>> CreateAsync(UnvanCreateRequestDto request);
        Task<ServiceResult<UnvanResponseDto>> UpdateAsync(int id, UnvanUpdateRequestDto request);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<List<UnvanResponseDto>>> GetActiveAsync();
        Task<ServiceResult<int>> GetPersonelCountAsync(int unvanId);
    }
}