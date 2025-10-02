namespace SGKPortalApp.PresentationLayer.Services.UIServices
{
    public interface IModalService
    {
        Task<bool> ShowConfirmAsync(string message, string title = "Onay");
        Task ShowAlertAsync(string message, string title = "Bilgi");
    }
}