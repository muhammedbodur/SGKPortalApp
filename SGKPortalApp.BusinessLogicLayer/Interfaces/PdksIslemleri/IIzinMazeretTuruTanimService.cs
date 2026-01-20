using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri
{
    public interface IIzinMazeretTuruTanimService
    {
        Task<ApiResponseDto<List<IzinMazeretTuruResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<List<IzinMazeretTuruResponseDto>>> GetActiveAsync();
        Task<ApiResponseDto<IzinMazeretTuruResponseDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<IzinMazeretTuruResponseDto>> CreateAsync(IzinMazeretTuruResponseDto dto);
        Task<ApiResponseDto<IzinMazeretTuruResponseDto>> UpdateAsync(int id, IzinMazeretTuruResponseDto dto);
        Task<ApiResponseDto<bool>> ToggleActiveAsync(int id);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);
    }
}
