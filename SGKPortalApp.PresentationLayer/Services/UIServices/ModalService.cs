namespace SGKPortalApp.PresentationLayer.Services.UIServices
{
    public class ModalService : IModalService
    {
        private TaskCompletionSource<bool>? _confirmTcs;

        public event Func<string, string, Task>? OnShowConfirm;
        public event Func<string, string, Task>? OnShowAlert;
        public event Action<bool>? OnConfirmResult;

        public async Task<bool> ShowConfirmAsync(string message, string title = "Onay")
        {
            _confirmTcs = new TaskCompletionSource<bool>();

            if (OnShowConfirm != null)
            {
                await OnShowConfirm.Invoke(message, title);
            }

            return await _confirmTcs.Task;
        }

        public async Task ShowAlertAsync(string message, string title = "Bilgi")
        {
            if (OnShowAlert != null)
            {
                await OnShowAlert.Invoke(message, title);
            }
        }

        public void SetConfirmResult(bool result)
        {
            _confirmTcs?.TrySetResult(result);
            OnConfirmResult?.Invoke(result);
        }
    }
}