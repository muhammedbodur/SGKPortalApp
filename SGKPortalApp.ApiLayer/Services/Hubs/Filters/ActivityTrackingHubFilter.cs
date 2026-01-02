using Microsoft.AspNetCore.SignalR;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;

namespace SGKPortalApp.ApiLayer.Services.Hubs.Filters
{
    /// <summary>
    /// SignalR Hub metodları çağrıldığında kullanıcının SonAktiviteZamani'nı günceller
    /// Blazor Server'da middleware çalışmadığı için bu filter gerekli
    /// </summary>
    public class ActivityTrackingHubFilter : IHubFilter
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ActivityTrackingHubFilter> _logger;

        public ActivityTrackingHubFilter(
            IServiceProvider serviceProvider,
            ILogger<ActivityTrackingHubFilter> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async ValueTask<object?> InvokeMethodAsync(
            HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object?>> next)
        {
            // TcKimlikNo'yu context'ten al
            var tcKimlikNo = invocationContext.Context.User?.FindFirst("TcKimlikNo")?.Value;

            if (!string.IsNullOrEmpty(tcKimlikNo))
            {
                // Son aktivite zamanını güncelle (fire and forget)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var userRepo = unitOfWork.GetRepository<IUserRepository>();

                        var user = await userRepo.FirstOrDefaultAsync(u => u.TcKimlikNo == tcKimlikNo);
                        if (user != null)
                        {
                            user.SonAktiviteZamani = DateTime.Now;
                            userRepo.Update(user);
                            await unitOfWork.SaveChangesAsync();

                            _logger.LogDebug("🕐 SonAktiviteZamani güncellendi - TC: {TcKimlikNo}, Method: {Method}",
                                tcKimlikNo, invocationContext.HubMethodName);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌ SonAktiviteZamani güncelleme hatası - TC: {TcKimlikNo}", tcKimlikNo);
                    }
                });
            }

            // Hub metodunu çalıştır
            return await next(invocationContext);
        }
    }
}
