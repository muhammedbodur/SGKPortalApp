using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common
{
    public interface IHizmetBinasiApiService
    {
        Task<ServiceResult<List<HizmetBinasiResponseDto>>> GetAllAsync();
        Task<ServiceResult<HizmetBinasiResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<HizmetBinasiDetailResponseDto>> GetDetailByIdAsync(int id);
        Task<ServiceResult<HizmetBinasiResponseDto>> CreateAsync(HizmetBinasiCreateRequestDto request);
        Task<ServiceResult<HizmetBinasiResponseDto>> UpdateAsync(int id, HizmetBinasiUpdateRequestDto request);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<List<HizmetBinasiResponseDto>>> GetActiveAsync();
        Task<ServiceResult<List<HizmetBinasiResponseDto>>> GetByDepartmanAsync(int departmanId);
        Task<ServiceResult<int>> GetPersonelCountAsync(int hizmetBinasiId);
        Task<ServiceResult<bool>> ToggleStatusAsync(int id);
        Task<ServiceResult<List<ServisResponseDto>>> GetServislerByHizmetBinasiIdAsync(int hizmetBinasiId);
    }
}