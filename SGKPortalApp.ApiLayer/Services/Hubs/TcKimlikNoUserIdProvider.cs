using Microsoft.AspNetCore.SignalR;

namespace SGKPortalApp.ApiLayer.Services.Hubs
{
    /// <summary>
    /// SignalR için User ID Provider
    /// TcKimlikNo claim'ini kullanarak kullanıcıları tanımlar
    /// Bu sayede Clients.User(tcKimlikNo) ile belirli kullanıcıya mesaj gönderilebilir
    /// </summary>
    public class TcKimlikNoUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            // TcKimlikNo claim'ini al
            var tcKimlikNo = connection.User?.FindFirst("TcKimlikNo")?.Value;
            
            return tcKimlikNo;
        }
    }
}
