using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Pdks
{
    public interface IIzinMazeretTuruTanimApiService
    {
        Task<ServiceResult<List<IzinMazeretTuruResponseDto>>> GetAllAsync();
        Task<ServiceResult<List<IzinMazeretTuruResponseDto>>> GetActiveAsync();
        Task<ServiceResult<IzinMazeretTuruResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<IzinMazeretTuruResponseDto>> CreateAsync(IzinMazeretTuruResponseDto dto);
        Task<ServiceResult<IzinMazeretTuruResponseDto>> UpdateAsync(int id, IzinMazeretTuruResponseDto dto);
        Task<ServiceResult<bool>> ToggleActiveAsync(int id);
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}
