using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGKPortalApp.PresentationLayer.Services.StateServices;
using System.Reflection;

namespace SGKPortalApp.PresentationLayer.Extensions
{
    /// <summary>
    /// Presentation Layer Dependency Injection Extensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Tüm Presentation Layer servislerini otomatik kaydet
        /// </summary>
        public static IServiceCollection AddPresentationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            Console.WriteLine("🚀 Presentation Layer başlatılıyor...");

            // API Base URL
            var apiUrl = configuration["AppSettings:ApiUrl"]
                ?? throw new InvalidOperationException("❌ ApiUrl configuration'da bulunamadı!");

            // HttpClient Factory
            services.AddHttpClient("SGKAPI", client =>
            {
                client.BaseAddress = new Uri(apiUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            var assembly = Assembly.GetExecutingAssembly();

            // 1. State Services (Manuel - Critical)
            Console.WriteLine("📌 State Services kaydediliyor...");
            services.AddScoped<AppStateService>();
            services.AddScoped<UserStateService>();
            services.AddScoped<NavigationStateService>();
            Console.WriteLine("  ✅ 3 State Service kayıt edildi");

            // 2. UI Services (Otomatik)
            services.RegisterServicesFromNamespace(
                assembly,
                "SGKPortalApp.PresentationLayer.Services.UIServices",
                "UI Services",
                ensureCriticalServices: new Type[] { typeof(SGKPortalApp.PresentationLayer.Services.UIServices.IToastService) }
            );

            // 3. Storage Services (Otomatik)
            services.RegisterServicesFromNamespace(
                assembly,
                "SGKPortalApp.PresentationLayer.Services.StorageServices",
                "Storage Services"
            );

            // 4. API Services (Otomatik - Interface/Implementation Matching)
            services.RegisterServicesFromNamespace(
                assembly,
                "SGKPortalApp.PresentationLayer.Services.ApiServices",
                "API Services",
                optional: true // API henüz hazır olmayabilir
            );

            Console.WriteLine("🎉 Presentation Layer hazır!\n");

            return services;
        }

        /// <summary>
        /// Belirtilen namespace'deki tüm servisleri otomatik kaydet
        /// Interface ve Implementation eşleştirmesi yapar
        /// </summary>
        private static IServiceCollection RegisterServicesFromNamespace(
            this IServiceCollection services,
            Assembly assembly,
            string namespaceName,
            string serviceName,
            bool optional = false,
            Type[]? ensureCriticalServices = null) // kritik servisleri garanti et
        {
            try
            {
                Console.WriteLine($"📦 {serviceName} kaydediliyor...");

                // Namespace'deki tüm tipleri al
                var types = assembly.GetTypes()
                    .Where(t => t.Namespace == namespaceName
                                && !t.IsAbstract
                                && !t.IsGenericType)
                    .ToList();

                if (!types.Any())
                {
                    if (!optional)
                    {
                        throw new InvalidOperationException(
                            $"❌ '{namespaceName}' namespace'inde hiç tip bulunamadı!");
                    }
                    else
                    {
                        Console.WriteLine($"  ⚠️ {serviceName} (opsiyonel) - Tip bulunamadı, atlanıyor");
                        return services;
                    }
                }

                // Interface'leri ve Implementation'ları ayır
                var interfaces = types.Where(t => t.IsInterface).ToList();
                var implementations = types.Where(t => t.IsClass).ToList();

                int registeredCount = 0;

                // Her interface için implementation bul ve kaydet
                foreach (var interfaceType in interfaces)
                {
                    var implementationType = implementations.FirstOrDefault(impl =>
                        interfaceType.IsAssignableFrom(impl) &&
                        impl.Name == interfaceType.Name.Substring(1) // "I" harfini çıkar
                    );

                    // Eğer isim kuralına uymazsa, interface'i implement eden class'ı bul
                    if (implementationType == null)
                    {
                        implementationType = implementations.FirstOrDefault(impl =>
                            interfaceType.IsAssignableFrom(impl)
                        );
                    }

                    if (implementationType != null)
                    {
                        services.AddScoped(interfaceType, implementationType);
                        registeredCount++;
                        Console.WriteLine($"  ✅ {interfaceType.Name} -> {implementationType.Name}");
                    }
                    else if (!optional)
                    {
                        Console.WriteLine($"  ⚠️ {interfaceType.Name} için implementation bulunamadı!");
                    }
                }

                // Implementation'ı olmayan concrete class'ları da kaydet (BaseApiService gibi)
                var standaloneServices = implementations
                    .Where(impl => !interfaces.Any(i => i.IsAssignableFrom(impl)))
                    .ToList();

                foreach (var serviceType in standaloneServices)
                {
                    services.AddScoped(serviceType);
                    registeredCount++;
                    Console.WriteLine($"  ✅ {serviceType.Name} (Concrete)");
                }

                // Kritik servisler varsa ve kaydedilmemişse garantile
                if (ensureCriticalServices != null)
                {
                    foreach (var criticalInterface in ensureCriticalServices)
                    {
                        if (!services.Any(s => s.ServiceType == criticalInterface))
                        {
                            var implType = assembly.GetTypes()
                                .FirstOrDefault(t => criticalInterface.IsAssignableFrom(t) && t.IsClass);

                            if (implType != null)
                            {
                                services.AddScoped(criticalInterface, implType);
                                Console.WriteLine($"  ✅ Kritik servis kaydedildi: {criticalInterface.Name} -> {implType.Name}");
                                registeredCount++;
                            }
                        }
                    }
                }

                Console.WriteLine($"  ✅ {serviceName}: {registeredCount} servis kayıt edildi");

                return services;
            }
            catch (Exception ex)
            {
                if (!optional)
                {
                    Console.WriteLine($"  ❌ {serviceName} kayıt hatası: {ex.Message}");
                    throw new InvalidOperationException(
                        $"'{serviceName}' servisleri kayıt edilirken hata oluştu. Namespace: {namespaceName}",
                        ex);
                }
                else
                {
                    Console.WriteLine($"  ⚠️ {serviceName} (opsiyonel) kayıt hatası - atlandı: {ex.Message}");
                    return services;
                }
            }
        }
    }
}