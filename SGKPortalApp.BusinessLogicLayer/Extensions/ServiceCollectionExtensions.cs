using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace SGKPortalApp.BusinessLogicLayer.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Business Logic Layer servislerini kaydediyor - ANA METOT
        /// </summary>
        public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
        {
            Console.WriteLine("📦 Business Logic Layer başlatılıyor...");

            var businessAssembly = Assembly.GetExecutingAssembly();

            // ═══════════════════════════════════════════════════════
            // 1️⃣ AUTOMAPPER PROFİLLERİ
            // ═══════════════════════════════════════════════════════
            try
            {
                services.AddAutoMapper(businessAssembly);
                Console.WriteLine("  ✅ AutoMapper profilleri yüklendi");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ❌ AutoMapper yüklenemedi: {ex.Message}");
                throw new InvalidOperationException("AutoMapper profilleri yüklenirken hata oluştu", ex);
            }

            // ═══════════════════════════════════════════════════════
            // 2️⃣ MODÜL BAZINDA SERVİS KAYITLARI
            // ═══════════════════════════════════════════════════════
            services.RegisterModuleServices(businessAssembly, "Auth");
            services.RegisterModuleServices(businessAssembly, "Common");
            services.RegisterModuleServices(businessAssembly, "PersonelIslemleri");
            services.RegisterModuleServices(businessAssembly, "SiramatikIslemleri");
            services.RegisterModuleServices(businessAssembly, "PdksIslemleri");
            services.RegisterModuleServices(businessAssembly, "EshotIslemleri");
            services.RegisterModuleServices(businessAssembly, "SignalR");
            services.RegisterModuleServices(businessAssembly, "Database");

            Console.WriteLine("  🎉 Business Logic Layer hazır!\n");
            return services;
        }

        // ═══════════════════════════════════════════════════════
        // MODÜL BAZINDA KAYIT - CROSS NAMESPACE DESTEKLI
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Bir modülün tüm servislerini kaydeder (Interface ve Service farklı namespace'lerde)
        /// </summary>
        private static IServiceCollection RegisterModuleServices(
            this IServiceCollection services,
            Assembly assembly,
            string moduleName)
        {
            try
            {
                Console.WriteLine($"  📂 {moduleName} servisleri kaydediliyor...");

                var interfaceNamespace = $"SGKPortalApp.BusinessLogicLayer.Interfaces.{moduleName}";
                var serviceNamespace = $"SGKPortalApp.BusinessLogicLayer.Services.{moduleName}";

                // Interface'leri bul (Interfaces namespace'inden)
                var interfaces = assembly.GetTypes()
                    .Where(t => t.Namespace == interfaceNamespace
                                && t.IsInterface
                                && !t.IsGenericType)
                    .ToList();

                // Service implementation'larını bul (Services namespace'inden)
                var implementations = assembly.GetTypes()
                    .Where(t => t.Namespace == serviceNamespace
                                && t.IsClass
                                && !t.IsAbstract
                                && !t.IsGenericType)
                    .ToList();

                // Özel yapılandırma gerektiren servisleri hariç tut
                var excludedTypes = new[]
                {
                    "IZKTecoApiClient",      // Factory metod ile HttpClient ve baseUrl parametreleri gerekiyor
                    "IZKTecoRealtimeService" // Factory metod ile signalRUrl parametresi gerekiyor
                };

                var excludedImplementations = new[]
                {
                    "ZKTecoApiClient",       // Factory metod ile kayıt ediliyor
                    "ZKTecoRealtimeService"  // Factory metod ile kayıt ediliyor
                };

                interfaces = interfaces.Where(i => !excludedTypes.Contains(i.Name)).ToList();
                implementations = implementations.Where(i => !excludedImplementations.Contains(i.Name)).ToList();

                if (!interfaces.Any() && !implementations.Any())
                {
                    Console.WriteLine($"    ⚠️  {moduleName} modülünde servis bulunamadı");
                    return services;
                }

                Console.WriteLine($"    🔍 Bulunan interface'ler: {interfaces.Count}");
                Console.WriteLine($"    🔧 Bulunan implementation'lar: {implementations.Count}");

                int registeredCount = 0;

                // Her interface için eşleşen implementation bul
                foreach (var interfaceType in interfaces)
                {
                    // Convention: IServiceName -> ServiceName
                    var expectedImplName = interfaceType.Name.Substring(1); // "I" harfini çıkar

                    var implementationType = implementations.FirstOrDefault(impl =>
                        impl.Name == expectedImplName &&
                        interfaceType.IsAssignableFrom(impl)
                    );

                    // İsim eşleşmesi yoksa, interface'i implement eden herhangi birini al
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
                        Console.WriteLine($"    ✅ {interfaceType.Name} -> {implementationType.Name}");
                    }
                    else
                    {
                        Console.WriteLine($"    ⚠️  {interfaceType.Name} için implementation bulunamadı");
                    }
                }

                // Interface'i olmayan standalone servisler (nadiren kullanılır)
                var standaloneServices = implementations
                    .Where(impl => !interfaces.Any(i => i.IsAssignableFrom(impl)))
                    .ToList();

                foreach (var serviceType in standaloneServices)
                {
                    services.AddScoped(serviceType);
                    registeredCount++;
                    Console.WriteLine($"    ✅ {serviceType.Name} (Concrete - Interface yok)");
                }

                if (registeredCount > 0)
                {
                    Console.WriteLine($"    🎯 {moduleName}: {registeredCount} servis kayıt edildi");
                }
                else
                {
                    Console.WriteLine($"    ⚠️  {moduleName}: Hiçbir servis kayıt edilemedi");
                }

                return services;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ❌ {moduleName} kayıt hatası: {ex.Message}");
                Console.WriteLine($"    📍 Stack Trace: {ex.StackTrace}");

                // Diğer modüllerin çalışmasını engellememe
                return services;
            }
        }
    }
}