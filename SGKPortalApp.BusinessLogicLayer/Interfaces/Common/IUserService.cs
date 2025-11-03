using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Common
{
    public interface IUserService
    {
        // CRUD Operations
        Task<ApiResponseDto<UserResponseDto>> GetByTcKimlikNoAsync(string tcKimlikNo);
        Task<ApiResponseDto<List<UserResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<List<UserResponseDto>>> GetActiveUsersAsync();
        Task<ApiResponseDto<List<UserResponseDto>>> GetLockedUsersAsync();
        
        Task<ApiResponseDto<UserResponseDto>> UpdateAsync(string tcKimlikNo, UserUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(string tcKimlikNo);
        
        // Password Operations
        Task<ApiResponseDto<bool>> ChangePasswordAsync(string tcKimlikNo, string oldPassword, string newPassword);
        Task<ApiResponseDto<bool>> ResetPasswordAsync(string tcKimlikNo);
        
        // Account Management
        Task<ApiResponseDto<bool>> LockUserAsync(string tcKimlikNo);
        Task<ApiResponseDto<bool>> UnlockUserAsync(string tcKimlikNo);
        Task<ApiResponseDto<bool>> ActivateUserAsync(string tcKimlikNo);
        Task<ApiResponseDto<bool>> DeactivateUserAsync(string tcKimlikNo);
        
        // Session Management
        Task<ApiResponseDto<bool>> ClearSessionAsync(string tcKimlikNo);
        Task<ApiResponseDto<UserResponseDto>> GetBySessionIdAsync(string sessionId);
    }
}
