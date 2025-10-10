using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri
{
    public interface ISendikaService
    {
        Task<ApiResponseDto<List<SendikaResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<SendikaResponseDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<SendikaResponseDto>> CreateAsync(SendikaCreateRequestDto request);
        Task<ApiResponseDto<SendikaResponseDto>> UpdateAsync(int id, SendikaUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);
    }
}
