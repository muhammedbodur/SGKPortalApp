using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Dashboard;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Common
{
    public interface IHaberService
    {
        Task<ApiResponseDto<List<HaberResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<HaberResponseDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<HaberResponseDto>> GetByIdWithGorsellerAsync(int id);
        Task<ApiResponseDto<HaberResponseDto>> CreateAsync(HaberCreateRequestDto request);
        Task<ApiResponseDto<HaberResponseDto>> UpdateAsync(int id, HaberUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);

        Task<ApiResponseDto<List<HaberResponseDto>>> GetSliderHaberlerAsync(int count = 5);
        Task<ApiResponseDto<List<HaberResponseDto>>> GetListeHaberlerAsync(int count = 10);
        Task<ApiResponseDto<List<HaberResponseDto>>> GetAktifHaberlerAsync();

        // Görsel yönetimi
        Task<ApiResponseDto<HaberGorselResponseDto>> AddGorselAsync(int haberId, string gorselUrl, bool vitrinFoto = false);
        Task<ApiResponseDto<bool>> RemoveGorselAsync(int haberGorselId);
        Task<ApiResponseDto<bool>> SetVitrinFotoAsync(int haberGorselId);
    }
}
