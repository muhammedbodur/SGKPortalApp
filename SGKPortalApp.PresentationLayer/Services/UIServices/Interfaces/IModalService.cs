namespace SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces
{
    public interface IModalService
    {
        Task<bool> ShowConfirmAsync(string message, string title = "Onay");
        Task ShowAlertAsync(string message, string title = "Bilgi");
    }
}