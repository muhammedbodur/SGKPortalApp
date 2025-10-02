namespace SGKPortalApp.PresentationLayer.Services.UIServices
{
    public class ToastService : IToastService
    {
        public event Action<ToastMessage>? OnShow;

        public void ShowSuccess(string message, string? title = null)
        {
            Show(message, title, ToastType.Success);
        }

        public void ShowError(string message, string? title = null)
        {
            Show(message, title, ToastType.Error);
        }

        public void ShowWarning(string message, string? title = null)
        {
            Show(message, title, ToastType.Warning);
        }

        public void ShowInfo(string message, string? title = null)
        {
            Show(message, title, ToastType.Info);
        }

        private void Show(string message, string? title, ToastType type)
        {
            var toast = new ToastMessage
            {
                Message = message,
                Title = title,
                Type = type
            };

            OnShow?.Invoke(toast);
        }
    }
}