namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Auth
{
    public interface IAuthApiService
    {
        Task<string?> LoginAsync(string username, string password);
        Task<bool> LogoutAsync();
        Task<bool> ValidateTokenAsync(string token);
    }
}