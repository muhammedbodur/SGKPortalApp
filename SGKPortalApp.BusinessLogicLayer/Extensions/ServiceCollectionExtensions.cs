using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace SGKPortalApp.BusinessLogicLayer.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Business Logic Layer servislerini kaydet
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
            services.AddPersonelIslemleriServices(businessAssembly);
            services.AddSiramatikIslemleriServices(businessAssembly);
            services.AddPdksIslemleriServices(businessAssembly);
            services.AddEshotIslemleriServices(businessAssembly);

            Console.WriteLine("  🎉 Business Logic Layer hazır!\n");
            return services;
        }

        // ═══════════════════════════════════════════════════════
        // MODÜL BAZINDA KAYIT METODLARI
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Personel İşlemleri modül servisleri
        /// </summary>
        private static IServiceCollection AddPersonelIslemleriServices(
            this IServiceCollection services,
            Assembly assembly)
        {
            services.RegisterServicesFromNamespace(
                assembly,
                "SGKPortalApp.BusinessLogicLayer.Services.PersonelIslemleri",
                "Personel İşlemleri"
            );
            return services;
        }

        /// <summary>
        /// Sıramatik İşlemleri modül servisleri
        /// </summary>
        private static IServiceCollection AddSiramatikIslemleriServices(
            this IServiceCollection services,
            Assembly assembly)
        {
            services.RegisterServicesFromNamespace(
                assembly,
                "SGKPortalApp.BusinessLogicLayer.Services.SiramatikIslemleri",
                "Sıramatik İşlemleri"
            );
            return services;
        }

        /// <summary>
        /// PDKS İşlemleri modül servisleri
        /// </summary>
        private static IServiceCollection AddPdksIslemleriServices(
            this IServiceCollection services,
            Assembly assembly)
        {
            services.RegisterServicesFromNamespace(
                assembly,
                "SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri",
                "PDKS İşlemleri"
            );
            return services;
        }

        /// <summary>
        /// Eshot İşlemleri modül servisleri
        /// </summary>
        private static IServiceCollection AddEshotIslemleriServices(
            this IServiceCollection services,
            Assembly assembly)
        {
            services.RegisterServicesFromNamespace(
                assembly,
                "SGKPortalApp.BusinessLogicLayer.Services.EshotIslemleri",
                "Eshot İşlemleri"
            );
            return services;
        }

        // ═══════════════════════════════════════════════════════
        // HELPER METOD: NAMESPACE BAZINDA OTOMATİK KAYIT
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Belirtilen namespace'deki tüm servisleri otomatik olarak kaydeder
        /// Convention: IServiceName -> ServiceName
        /// </summary>
        private static IServiceCollection RegisterServicesFromNamespace(
            this IServiceCollection services,
            Assembly assembly,
            string namespaceName,
            string moduleName)
        {
            try
            {
                Console.WriteLine($"  📂 {moduleName} servisleri kaydediliyor...");

                // Namespace'deki tüm tipleri al
                var types = assembly.GetTypes()
                    .Where(t => t.Namespace == namespaceName
                                && !t.IsAbstract
                                && !t.IsGenericType)
                    .ToList();

                if (!types.Any())
                {
                    Console.WriteLine($"    ⚠️  {namespaceName} namespace'inde tip bulunamadı");
                    return services;
                }

                // Interface'leri ve implementation'ları ayır
                var interfaces = types.Where(t => t.IsInterface).ToList();
                var implementations = types.Where(t => t.IsClass).ToList();

                int registeredCount = 0;

                // Her interface için implementation bul ve kaydet
                foreach (var interfaceType in interfaces)
                {
                    // Convention: IServiceName -> ServiceName
                    var implementationType = implementations.FirstOrDefault(impl =>
                        interfaceType.IsAssignableFrom(impl) &&
                        impl.Name == interfaceType.Name.Substring(1) // "I" harfini çıkar
                    );

                    // Bulamazsan, interface'i implement eden herhangi bir sınıfı bul
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

                // Interface'i olmayan standalone servisler
                var standaloneServices = implementations
                    .Where(impl => !interfaces.Any(i => i.IsAssignableFrom(impl)))
                    .ToList();

                foreach (var serviceType in standaloneServices)
                {
                    services.AddScoped(serviceType);
                    registeredCount++;
                    Console.WriteLine($"    ✅ {serviceType.Name} (Concrete)");
                }

                Console.WriteLine($"    🎯 {moduleName}: {registeredCount} servis kayıt edildi");

                return services;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ❌ {moduleName} kayıt hatası: {ex.Message}");
                throw new InvalidOperationException(
                    $"'{moduleName}' servisleri kayıt edilirken hata oluştu. Namespace: {namespaceName}",
                    ex);
            }
        }
    }
}