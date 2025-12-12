using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel
{
    public interface ISendikaApiService
    {
        Task<ServiceResult<List<SendikaResponseDto>>> GetAllAsync();
        Task<ServiceResult<SendikaResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<SendikaResponseDto>> CreateAsync(SendikaCreateRequestDto request);
        Task<ServiceResult<SendikaResponseDto>> UpdateAsync(int id, SendikaUpdateRequestDto request);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<int>> GetPersonelCountAsync(int sendikaId);
    }
}
