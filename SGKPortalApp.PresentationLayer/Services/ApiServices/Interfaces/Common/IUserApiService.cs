using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common
{
    public interface IUserApiService
    {
        // User Sorgulama
        Task<ServiceResult<UserResponseDto>> GetByTcKimlikNoAsync(string tcKimlikNo);

        // Password Operations
        Task<ServiceResult<bool>> ChangePasswordAsync(string tcKimlikNo, SGKPortalApp.BusinessObjectLayer.DTOs.Request.Account.ChangePasswordRequestDto request);
        
        // Banko Modu İşlemleri
        Task<ServiceResult<bool>> ActivateBankoModeAsync(string tcKimlikNo, int bankoId);
        Task<ServiceResult<bool>> DeactivateBankoModeAsync(string tcKimlikNo);
        Task<ServiceResult<bool>> IsBankoModeActiveAsync(string tcKimlikNo);
        Task<ServiceResult<int?>> GetActiveBankoIdAsync(string tcKimlikNo);
    }
}
