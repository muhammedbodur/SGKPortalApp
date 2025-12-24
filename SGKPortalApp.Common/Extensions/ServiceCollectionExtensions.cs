using Microsoft.Extensions.DependencyInjection;
using SGKPortalApp.Common.Interfaces.Permission;
using SGKPortalApp.Common.Services.Permission;

namespace SGKPortalApp.Common.Extensions
{
    /// <summary>
    /// Common Layer servis kayıtları
    /// Shared services (hem PresentationLayer hem BusinessLogicLayer tarafından kullanılır)
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Common Layer servislerini DI container'a kaydet
        /// </summary>
        public static IServiceCollection AddCommonServices(this IServiceCollection services)
        {
            Console.WriteLine("📦 Common Layer servisleri kaydediliyor...");

            // Permission Services
            services.AddScoped<IPermissionKeyResolverService, PermissionKeyResolverService>();
            Console.WriteLine("  ✅ IPermissionKeyResolverService → PermissionKeyResolverService");

            Console.WriteLine("  🎉 Common Layer hazır!\n");
            return services;
        }
    }
}
