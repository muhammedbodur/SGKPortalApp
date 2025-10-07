using SGKPortalApp.PresentationLayer.Services.ApiServices.Base;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete
{
    public class AuthApiService : BaseApiService, IAuthApiService
    {
        public AuthApiService(IHttpClientFactory httpClientFactory, ILogger<AuthApiService> logger)
            : base(httpClientFactory, logger)
        {
        }

        public async Task<string?> LoginAsync(string username, string password)
        {
            var request = new { Username = username, Password = password };
            var response = await PostAsync<object, LoginResponse>("auth/login", request);
            return response?.Token;
        }

        public async Task<bool> LogoutAsync()
        {
            return await PostAsync<object, bool>("auth/logout", new { });
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await GetAsync<bool>("auth/validate");
            return response;
        }

        private class LoginResponse
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}