using Microsoft.AspNetCore.SignalR;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;

namespace SGKPortalApp.ApiLayer.Services.Hubs.Filters
{
    /// <summary>
    /// SignalR Hub metodları çağrıldığında kullanıcının SonAktiviteZamani'nı günceller
    /// Blazor Server'da middleware çalışmadığı için bu filter gerekli
    /// Throttling: Son güncelleme 1 dakikadan eski ise güncelleme yapar (DB yükünü azaltır)
    /// </summary>
    public class ActivityTrackingHubFilter : IHubFilter
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ActivityTrackingHubFilter> _logger;

        // Throttling süresi: 1 dakika içinde tekrar güncelleme yapma
        private readonly TimeSpan _updateThrottleInterval = TimeSpan.FromMinutes(1);

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
                            // Throttling: Son güncelleme 1 dakikadan yeni ise güncelleme yapma
                            if (user.SonAktiviteZamani.HasValue)
                            {
                                var timeSinceLastUpdate = DateTime.Now - user.SonAktiviteZamani.Value;
                                if (timeSinceLastUpdate < _updateThrottleInterval)
                                {
                                    // Çok kısa süre önce güncellenmiş, atla (DB yükünü azalt)
                                    _logger.LogTrace("⏭️ SonAktiviteZamani güncellemesi atlandı (son güncelleme: {Seconds} saniye önce) - TC: {TcKimlikNo}",
                                        timeSinceLastUpdate.TotalSeconds, tcKimlikNo);
                                    return;
                                }
                            }

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
