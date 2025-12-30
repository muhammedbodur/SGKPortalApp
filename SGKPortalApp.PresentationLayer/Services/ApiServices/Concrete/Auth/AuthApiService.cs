using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Auth;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Auth;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Base;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Auth;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Auth
{
    public class AuthApiService : BaseApiService, IAuthApiService
    {
        public AuthApiService(HttpClient httpClient, ILogger<AuthApiService> logger)
            : base(httpClient, logger)
        {
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            return await PostAsync<LoginRequestDto, LoginResponseDto>("auth/login", request);
        }

        public async Task<VerifyIdentityResponseDto?> VerifyIdentityAsync(VerifyIdentityRequestDto request)
        {
            return await PostAsync<VerifyIdentityRequestDto, VerifyIdentityResponseDto>("auth/verify-identity", request);
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            var response = await PostAsync<ResetPasswordRequestDto, object>("auth/reset-password", request);
            return response != null;
        }

        public async Task<bool> LogoutAsync()
        {
            var response = await PostAsync<object, object>("auth/logout", new { });
            return response != null;
        }

        public async Task<DomainInfoDto?> GetDomainInfoAsync()
        {
            return await GetAsync<DomainInfoDto>("auth/domain-info");
        }
    }
}