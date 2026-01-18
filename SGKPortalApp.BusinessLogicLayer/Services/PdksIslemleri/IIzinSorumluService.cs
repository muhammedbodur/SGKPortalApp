using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri
{
    /// <summary>
    /// İzin Sorumlu Atama Service Interface
    /// </summary>
    public interface IIzinSorumluService
    {
        /// <summary>
        /// Tüm izin sorumlu atamalarını getir
        /// </summary>
        Task<ApiResponseDto<List<IzinSorumluResponseDto>>> GetAllAsync();

        /// <summary>
        /// Aktif izin sorumlu atamalarını getir
        /// </summary>
        Task<ApiResponseDto<List<IzinSorumluResponseDto>>> GetActiveAsync();

        /// <summary>
        /// ID'ye göre izin sorumlu ataması getir
        /// </summary>
        Task<ApiResponseDto<IzinSorumluResponseDto>> GetByIdAsync(int id);

        /// <summary>
        /// Departman/Servis bazında sorumluları getir
        /// </summary>
        Task<ApiResponseDto<List<IzinSorumluResponseDto>>> GetByDepartmanServisAsync(int? departmanId, int? servisId);

        /// <summary>
        /// Personel için sorumluları getir (personelin bulunduğu departman/servis için)
        /// </summary>
        Task<ApiResponseDto<List<IzinSorumluResponseDto>>> GetSorumluForPersonelAsync(string personelTcKimlikNo);

        /// <summary>
        /// Yeni izin sorumlusu ata
        /// </summary>
        Task<ApiResponseDto<IzinSorumluResponseDto>> CreateAsync(IzinSorumluCreateDto request);

        /// <summary>
        /// İzin sorumlusu güncelle
        /// </summary>
        Task<ApiResponseDto<IzinSorumluResponseDto>> UpdateAsync(IzinSorumluUpdateDto request);

        /// <summary>
        /// İzin sorumlusunu pasif yap (Soft delete)
        /// </summary>
        Task<ApiResponseDto<bool>> DeactivateAsync(int id);

        /// <summary>
        /// İzin sorumlusunu aktif yap
        /// </summary>
        Task<ApiResponseDto<bool>> ActivateAsync(int id);

        /// <summary>
        /// İzin sorumlusunu sil (Hard delete)
        /// </summary>
        Task<ApiResponseDto<bool>> DeleteAsync(int id);
    }
}
