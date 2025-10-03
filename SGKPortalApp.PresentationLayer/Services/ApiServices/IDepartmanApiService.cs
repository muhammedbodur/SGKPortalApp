using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices
{
    /// <summary>
    /// Departman API servisi arayüzü
    /// </summary>
    public interface IDepartmanApiService
    {
        Task<List<DepartmanResponseDto>> GetAllAsync();
        Task<DepartmanResponseDto?> GetByIdAsync(int id);
        Task<DepartmanResponseDto> CreateAsync(DepartmanCreateRequestDto request);
        Task<DepartmanResponseDto> UpdateAsync(int id, DepartmanUpdateRequestDto request);
        Task<bool> DeleteAsync(int id);
        Task<List<DepartmanResponseDto>> GetActiveAsync();
    }
}