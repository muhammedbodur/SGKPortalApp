using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;

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
        Task<ServiceResult<IzinMazeretTalepFilterResponseDto>> GetFilteredAsync(IzinMazeretTalepFilterRequestDto filter);
        Task<ServiceResult<List<PersonelResponseDto>>> GetAvailableApproversAsync(string tcKimlikNo);
        Task<ServiceResult<List<IzinMazeretTuruResponseDto>>> GetAvailableLeaveTypesAsync();
        Task<ServiceResult<bool>> ProcessSgkIslemAsync(IzinSgkIslemRequestDto request);
    }
}
