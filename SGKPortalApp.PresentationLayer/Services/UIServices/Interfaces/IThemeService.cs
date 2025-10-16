namespace SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces
{
    public interface IThemeService
    {
        event Action? OnThemeChanged;
        Task<string> GetThemeAsync();
        Task SetThemeAsync(string theme);
        Task ToggleThemeAsync();
    }
}