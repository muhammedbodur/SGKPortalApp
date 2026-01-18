using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Pdks
{
    public interface IIzinSorumluApiService
    {
        /// <summary>
        /// Tüm izin sorumluları getirir
        /// </summary>
        Task<ServiceResult<List<IzinSorumluResponseDto>>> GetAllAsync();

        /// <summary>
        /// Sadece aktif izin sorumluları getirir
        /// </summary>
        Task<ServiceResult<List<IzinSorumluResponseDto>>> GetActiveAsync();

        /// <summary>
        /// Yeni izin sorumlusu oluşturur
        /// </summary>
        Task<ServiceResult<IzinSorumluResponseDto>> CreateAsync(IzinSorumluCreateDto request);

        /// <summary>
        /// İzin sorumlusunu günceller
        /// </summary>
        Task<ServiceResult<IzinSorumluResponseDto>> UpdateAsync(int id, IzinSorumluUpdateDto request);

        /// <summary>
        /// İzin sorumlusunu pasif yapar
        /// </summary>
        Task<ServiceResult<bool>> DeactivateAsync(int id);

        /// <summary>
        /// İzin sorumlusunu aktif yapar
        /// </summary>
        Task<ServiceResult<bool>> ActivateAsync(int id);

        /// <summary>
        /// İzin sorumlusunu siler
        /// </summary>
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}
