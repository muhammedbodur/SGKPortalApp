namespace SGKPortalApp.PresentationLayer.Services.UIServices
{
    public interface IToastService
    {
        event Action<ToastMessage>? OnShow;
        void ShowSuccess(string message, string? title = null);
        void ShowError(string message, string? title = null);
        void ShowWarning(string message, string? title = null);
        void ShowInfo(string message, string? title = null);
    }

    public class ToastMessage
    {
        public string Message { get; set; } = string.Empty;
        public string? Title { get; set; }
        public ToastType Type { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    public enum ToastType
    {
        Success,
        Error,
        Warning,
        Info
    }
}