using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel
{
    public interface IServisApiService
    {
        Task<ServiceResult<List<ServisResponseDto>>> GetAllAsync();
        Task<ServiceResult<ServisResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<ServisResponseDto>> CreateAsync(ServisCreateRequestDto request);
        Task<ServiceResult<ServisResponseDto>> UpdateAsync(int id, ServisUpdateRequestDto request);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<List<ServisResponseDto>>> GetActiveAsync();
        Task<ServiceResult<int>> GetPersonelCountAsync(int servisId);
    }
}