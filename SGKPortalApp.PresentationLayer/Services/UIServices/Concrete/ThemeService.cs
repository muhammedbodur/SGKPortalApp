using Microsoft.JSInterop;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Services.UIServices.Concrete
{
    public class ThemeService : IThemeService
    {
        private readonly IJSRuntime _jsRuntime;
        private string _currentTheme = "light";

        public event Action? OnThemeChanged;

        public ThemeService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<string> GetThemeAsync()
        {
            try
            {
                var theme = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "theme");
                _currentTheme = string.IsNullOrEmpty(theme) ? "light" : theme;
                return _currentTheme;
            }
            catch
            {
                return _currentTheme;
            }
        }

        public async Task SetThemeAsync(string theme)
        {
            _currentTheme = theme;
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "theme", theme);
            OnThemeChanged?.Invoke();
        }

        public async Task ToggleThemeAsync()
        {
            var newTheme = _currentTheme == "light" ? "dark" : "light";
            await SetThemeAsync(newTheme);
        }
    }
}