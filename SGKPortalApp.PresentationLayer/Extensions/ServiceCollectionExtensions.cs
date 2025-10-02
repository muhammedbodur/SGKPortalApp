using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGKPortalApp.PresentationLayer.Services.StateServices;
using System.Reflection;

namespace SGKPortalApp.PresentationLayer.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPresentationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var apiUrl = configuration["AppSettings:ApiUrl"]
                ?? throw new InvalidOperationException("ApiUrl bulunamadı");

            // HttpClient
            services.AddHttpClient("SGKAPI", client =>
            {
                client.BaseAddress = new Uri(apiUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            var assembly = Assembly.GetExecutingAssembly();

            // 1. State Services (Critical - Manuel)
            services.AddScoped<AppStateService>();
            services.AddScoped<UserStateService>();
            services.AddScoped<NavigationStateService>();

            // 2. UI Services (Critical - Otomatik)
            RegisterServicesAutomatically(services, assembly,
                "SGKPortalApp.PresentationLayer.Services.UIServices");

            // 3. Storage Services (Critical - Otomatik)
            RegisterServicesAutomatically(services, assembly,
                "SGKPortalApp.PresentationLayer.Services.StorageServices");

            // 4. API Services (Optional - Otomatik)
            RegisterServicesAutomatically(services, assembly,
                "SGKPortalApp.PresentationLayer.Services.ApiServices", optional: true);

            Console.WriteLine("✅ Presentation Layer servisleri yüklendi");

            return services;
        }

        private static void RegisterServicesAutomatically(
            IServiceCollection services,
            Assembly assembly,
            string namespaceName,
            bool optional = false)
        {
            try
            {
                var types = assembly.GetTypes()
                    .Where(t => t.Namespace == namespaceName
                        && t.IsClass
                        && !t.IsAbstract
                        && !t.IsGenericType)
                    .ToList();

                if (!types.Any() && !optional)
                {
                    throw new InvalidOperationException(
                        $"❌ Namespace '{namespaceName}' içinde servis bulunamadı!");
                }

                foreach (var implementationType in types)
                {
                    var interfaceType = implementationType.GetInterfaces()
                        .FirstOrDefault(i => i.Name == $"I{implementationType.Name}");

                    if (interfaceType != null)
                    {
                        services.AddScoped(interfaceType, implementationType);
                        Console.WriteLine($"  ✓ {interfaceType.Name} -> {implementationType.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                if (!optional)
                {
                    Console.WriteLine($"❌ Servis kaydı hatası ({namespaceName}): {ex.Message}");
                    throw;
                }
                else
                {
                    Console.WriteLine($"⚠️ Opsiyonel servisler atlandı ({namespaceName})");
                }
            }
        }
    }
}