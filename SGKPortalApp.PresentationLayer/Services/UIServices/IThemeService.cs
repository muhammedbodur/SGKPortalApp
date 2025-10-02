namespace SGKPortalApp.PresentationLayer.Services.UIServices
{
    public interface IThemeService
    {
        event Action? OnThemeChanged;
        Task<string> GetThemeAsync();
        Task SetThemeAsync(string theme);
        Task ToggleThemeAsync();
    }
}