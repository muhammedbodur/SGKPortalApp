using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri
{
    public interface IBankoService
    {
        // ═══════════════════════════════════════════════════════
        // CRUD OPERATIONS
        // ═══════════════════════════════════════════════════════

        Task<ApiResponseDto<List<BankoResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<BankoResponseDto>> GetByIdAsync(int bankoId);
        Task<ApiResponseDto<BankoResponseDto>> CreateAsync(BankoCreateRequestDto request);
        Task<ApiResponseDto<BankoResponseDto>> UpdateAsync(int bankoId, BankoUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int bankoId);

        // ═══════════════════════════════════════════════════════
        // QUERY OPERATIONS
        // ═══════════════════════════════════════════════════════

        Task<ApiResponseDto<List<BankoResponseDto>>> GetByHizmetBinasiAsync(int hizmetBinasiId);
        Task<ApiResponseDto<List<BankoKatGrupluResponseDto>>> GetGroupedByKatAsync(int hizmetBinasiId);
        Task<ApiResponseDto<List<BankoResponseDto>>> GetAvailableBankosAsync(int hizmetBinasiId);
        Task<ApiResponseDto<List<BankoResponseDto>>> GetActiveAsync();

        // ═══════════════════════════════════════════════════════
        // PERSONEL ATAMA OPERATIONS
        // ═══════════════════════════════════════════════════════

        Task<ApiResponseDto<bool>> AssignPersonelToBankoAsync(BankoPersonelAtaDto request);
        Task<ApiResponseDto<bool>> UnassignPersonelFromBankoAsync(string tcKimlikNo);
        Task<ApiResponseDto<bool>> UnassignBankoAsync(int bankoId);
        Task<ApiResponseDto<BankoResponseDto>> GetPersonelCurrentBankoAsync(string tcKimlikNo);

        // ═══════════════════════════════════════════════════════
        // MAINTENANCE OPERATIONS
        // ═══════════════════════════════════════════════════════

        Task<ApiResponseDto<int>> CleanupInconsistentBankoAssignmentsAsync();

        // ═══════════════════════════════════════════════════════
        // AKTIFLIK OPERATIONS
        // ═══════════════════════════════════════════════════════

        Task<ApiResponseDto<bool>> ToggleAktiflikAsync(int bankoId);
    }
}
