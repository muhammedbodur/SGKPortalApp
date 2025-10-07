using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGKPortalApp.PresentationLayer.Services.StateServices;
using SGKPortalApp.PresentationLayer.Services.UIServices;
using System.Reflection;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Auth;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Auth;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Personel;

namespace SGKPortalApp.PresentationLayer.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Presentation Layer servislerini kaydeder
        /// </summary>
        public static IServiceCollection AddPresentationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            Console.WriteLine("🚀 Presentation Layer başlatılıyor...");

            // API URL'ini configuration'dan al
            var apiUrl = configuration["AppSettings:ApiUrl"]
                ?? throw new InvalidOperationException("❌ ApiUrl configuration'da bulunamadı!");

            Console.WriteLine($"🔗 API URL: {apiUrl}");

            // ═══════════════════════════════════════════════════════
            // 📦 NAMED HttpClient (Global - İhtiyaç halinde kullanılabilir)
            // ═══════════════════════════════════════════════════════
            services.AddHttpClient("SGKAPI", client =>
            {
                client.BaseAddress = new Uri(apiUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            var assembly = Assembly.GetExecutingAssembly();

            // ═══════════════════════════════════════════════════════
            // 1️⃣ STATE SERVICES
            // ═══════════════════════════════════════════════════════
            Console.WriteLine("📌 State Services kaydediliyor...");
            services.AddScoped<AppStateService>();
            services.AddScoped<UserStateService>();
            services.AddScoped<NavigationStateService>();
            Console.WriteLine("  ✅ 3 State Service kayıt edildi");

            // ═══════════════════════════════════════════════════════
            // 2️⃣ UI SERVICES (Otomatik Kayıt)
            // ═══════════════════════════════════════════════════════
            services.RegisterServicesFromNamespace(
                assembly,
                "SGKPortalApp.PresentationLayer.Services.UIServices",
                "UI Services",
                ensureCriticalServices: new Type[] { typeof(IToastService) }
            );

            // ═══════════════════════════════════════════════════════
            // 3️⃣ STORAGE SERVICES (Otomatik Kayıt)
            // ═══════════════════════════════════════════════════════
            services.RegisterServicesFromNamespace(
                assembly,
                "SGKPortalApp.PresentationLayer.Services.StorageServices",
                "Storage Services"
            );

            // ═══════════════════════════════════════════════════════
            // 4️⃣ API SERVICES - TYPED HttpClient ile kayıt
            // ═══════════════════════════════════════════════════════
            Console.WriteLine("📦 API Services kaydediliyor (Typed HttpClient)...");

            // 🔹 DepartmanApiService
            services.AddHttpClient<IDepartmanApiService, DepartmanApiService>(client =>
            {
                client.BaseAddress = new Uri(apiUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            // 🔹 PersonelApiService
            services.AddHttpClient<IPersonelApiService, PersonelApiService>(client =>
            {
                client.BaseAddress = new Uri(apiUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            // 🔹 AuthApiService
            services.AddHttpClient<IAuthApiService, AuthApiService>(client =>
            {
                client.BaseAddress = new Uri(apiUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            Console.WriteLine("  ✅ 3 API Service kayıt edildi (BaseAddress ile)");
            Console.WriteLine("🎉 Presentation Layer hazır!\n");

            return services;
        }

        /// <summary>
        /// Belirtilen namespace'deki tüm servisleri otomatik olarak kaydeder
        /// Interface-Implementation eşleştirmesi yapar
        /// </summary>
        private static IServiceCollection RegisterServicesFromNamespace(
            this IServiceCollection services,
            Assembly assembly,
            string namespaceName,
            string serviceName,
            bool optional = false,
            Type[]? ensureCriticalServices = null)
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

                // Interface'leri ve implementation'ları ayır
                var interfaces = types.Where(t => t.IsInterface).ToList();
                var implementations = types.Where(t => t.IsClass).ToList();

                int registeredCount = 0;

                // Her interface için implementation bul ve kaydet
                foreach (var interfaceType in interfaces)
                {
                    // Önce naming convention'a göre ara (IService -> Service)
                    var implementationType = implementations.FirstOrDefault(impl =>
                        interfaceType.IsAssignableFrom(impl) &&
                        impl.Name == interfaceType.Name.Substring(1) // "I" harfini çıkar
                    );

                    // Bulamazsan interface'i implement eden herhangi bir class'ı bul
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

                // Interface'i olmayan standalone servisleri kaydet
                var standaloneServices = implementations
                    .Where(impl => !interfaces.Any(i => i.IsAssignableFrom(impl)))
                    .ToList();

                foreach (var serviceType in standaloneServices)
                {
                    services.AddScoped(serviceType);
                    registeredCount++;
                    Console.WriteLine($"  ✅ {serviceType.Name} (Concrete)");
                }

                // Kritik servislerin kayıtlı olduğundan emin ol
                if (ensureCriticalServices != null)
                {
                    foreach (var criticalInterface in ensureCriticalServices)
                    {
                        // Servis zaten kayıtlı mı kontrol et
                        if (!services.Any(s => s.ServiceType == criticalInterface))
                        {
                            // Kayıtlı değilse implementation'ı bul ve kaydet
                            var implType = assembly.GetTypes()
                                .FirstOrDefault(t => criticalInterface.IsAssignableFrom(t)
                                                  && t.IsClass
                                                  && !t.IsAbstract);

                            if (implType != null)
                            {
                                services.AddScoped(criticalInterface, implType);
                                Console.WriteLine($"  ✅ Kritik servis: {criticalInterface.Name} -> {implType.Name}");
                                registeredCount++;
                            }
                            else
                            {
                                throw new InvalidOperationException(
                                    $"❌ Kritik servis '{criticalInterface.Name}' için implementation bulunamadı!");
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