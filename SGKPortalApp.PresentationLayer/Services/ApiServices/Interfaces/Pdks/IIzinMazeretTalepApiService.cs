using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Pdks
{
    public interface IIzinMazeretTalepApiService
    {
        Task<ServiceResult<IzinMazeretTalepResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<IzinMazeretTalepResponseDto>> CreateAsync(IzinMazeretTalepCreateRequestDto request);
        Task<ServiceResult<IzinMazeretTalepResponseDto>> UpdateAsync(int id, IzinMazeretTalepUpdateRequestDto request);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<bool>> CancelAsync(int id, string iptalNedeni);
        Task<ServiceResult<List<IzinMazeretTalepListResponseDto>>> GetByPersonelAsync(string tcKimlikNo);
        Task<ServiceResult<List<IzinMazeretTalepListResponseDto>>> GetPendingApprovalsAsync(string sorumluTc);
        Task<ServiceResult<bool>> ApproveOrRejectAsync(int id, IzinMazeretTalepOnayRequestDto request);
        Task<ServiceResult<OverlapCheckResponseDto>> CheckOverlapAsync(IzinMazeretTalepCreateRequestDto request);
        Task<ServiceResult<List<IzinMazeretTalepListResponseDto>>> GetFilteredAsync(IzinMazeretTalepFilterRequestDto filter);
    }
}
