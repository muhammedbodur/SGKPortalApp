using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik
{
    public interface IBankoApiService
    {
        // Query Operations
        Task<ServiceResult<List<BankoResponseDto>>> GetAllAsync();
        Task<ServiceResult<BankoResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<List<BankoResponseDto>>> GetByHizmetBinasiAsync(int hizmetBinasiId);
        Task<ServiceResult<List<BankoKatGrupluResponseDto>>> GetGroupedByKatAsync(int hizmetBinasiId);
        Task<ServiceResult<List<BankoResponseDto>>> GetAvailableBankosAsync(int hizmetBinasiId);
        Task<ServiceResult<List<BankoResponseDto>>> GetActiveAsync();
        Task<ServiceResult<BankoResponseDto>> GetPersonelCurrentBankoAsync(string tcKimlikNo);

        // CRUD Operations
        Task<ServiceResult<BankoResponseDto>> CreateAsync(BankoCreateRequestDto dto);
        Task<ServiceResult<BankoResponseDto>> UpdateAsync(int id, BankoUpdateRequestDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);

        // Personel Atama Operations
        Task<ServiceResult<bool>> PersonelAtaAsync(BankoPersonelAtaDto dto);
        Task<ServiceResult<bool>> PersonelCikarAsync(string tcKimlikNo);
        Task<ServiceResult<bool>> BankoBoşaltAsync(int bankoId);

        // Aktiflik Operations
        Task<ServiceResult<bool>> ToggleAktiflikAsync(int bankoId);
    }
}
