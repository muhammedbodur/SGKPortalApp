namespace SGKPortalApp.PresentationLayer.Services.UIServices
{
    public interface ILoadingService
    {
        event Action<bool>? OnLoadingChanged;
        void Show(string? message = null);
        void Hide();
        bool IsLoading { get; }
        string? LoadingMessage { get; }
    }
}