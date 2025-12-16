using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGKPortalApp.PresentationLayer.Services.StateServices;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Handlers;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System.Reflection;

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

            var assembly = Assembly.GetExecutingAssembly();

            // ═══════════════════════════════════════════════════════
            // 1️⃣ STATE SERVICES
            // ═══════════════════════════════════════════════════════
            Console.WriteLine("📌 State Services kaydediliyor...");
            services.AddScoped<AppStateService>();
            services.AddScoped<UserStateService>();
            services.AddScoped<NavigationStateService>();
            services.AddScoped<PermissionStateService>();
            services.AddScoped<PagePermissionService>();
            Console.WriteLine("  ✅ 5 State Service kayıt edildi");

            // ═══════════════════════════════════════════════════════
            // 1️⃣ AUTHENTICATION SERVICES (Cookie Management)
            // ═══════════════════════════════════════════════════════
            services.RegisterModuleServicesWithSubfolders(assembly, "AuthenticationServices");

            // ═══════════════════════════════════════════════════════
            // 2️⃣ USER SESSION SERVICES (Claims-based)
            // ═══════════════════════════════════════════════════════
            services.RegisterModuleServicesWithSubfolders(assembly, "UserSessionServices");

            // ═══════════════════════════════════════════════════════
            // 3️⃣ STORAGE SERVICES
            // ═══════════════════════════════════════════════════════
            services.RegisterModuleServicesWithSubfolders(assembly, "StorageServices");

            // ═══════════════════════════════════════════════════════
            // 4️⃣ UI SERVICES
            // ═══════════════════════════════════════════════════════
            services.RegisterModuleServicesWithSubfolders(assembly, "UIServices");

            // ═══════════════════════════════════════════════════════
            // 5️⃣ API SERVICES - Modül Bazında Otomatik Kayıt
            // ═══════════════════════════════════════════════════════
            services.AddTransient<ApiAuthCookieForwardingHandler>();

            services.RegisterApiServices(assembly, apiUrl, "Auth");
            services.RegisterApiServices(assembly, apiUrl, "Common");
            services.RegisterApiServices(assembly, apiUrl, "Personel");
            services.RegisterApiServices(assembly, apiUrl, "Siramatik");
            services.RegisterApiServices(assembly, apiUrl, "Pdks");
            services.RegisterApiServices(assembly, apiUrl, "Eshot");

            Console.WriteLine("🎉 Presentation Layer hazır!\n");
            return services;
        }

        // ═══════════════════════════════════════════════════════
        // API SERVICES - TYPED HttpClient ile Otomatik Kayıt
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Bir modülün tüm API servislerini Typed HttpClient ile kaydeder
        /// </summary>
        private static IServiceCollection RegisterApiServices(
            this IServiceCollection services,
            Assembly assembly,
            string apiUrl,
            string moduleName)
        {
            try
            {
                Console.WriteLine($"📦 {moduleName} API Services kaydediliyor...");

                var interfaceNamespace = $"SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.{moduleName}";
                var concreteNamespace = $"SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.{moduleName}";

                // Interface'leri bul
                var interfaces = assembly.GetTypes()
                    .Where(t => t.Namespace == interfaceNamespace
                                && t.IsInterface
                                && !t.IsGenericType)
                    .ToList();

                // Implementation'ları bul
                var implementations = assembly.GetTypes()
                    .Where(t => t.Namespace == concreteNamespace
                                && t.IsClass
                                && !t.IsAbstract
                                && !t.IsGenericType)
                    .ToList();

                if (!interfaces.Any() && !implementations.Any())
                {
                    Console.WriteLine($"    ⚠️  {moduleName} modülünde API service bulunamadı");
                    return services;
                }

                Console.WriteLine($"    🔍 Bulunan interface'ler: {interfaces.Count}");
                Console.WriteLine($"    🔧 Bulunan implementation'lar: {implementations.Count}");

                int registeredCount = 0;

                // Her interface için eşleşen implementation bul ve Typed HttpClient ile kaydet
                foreach (var interfaceType in interfaces)
                {
                    // Convention: IServiceApiService -> ServiceApiService
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
                        // 🔹 AddHttpClient<TInterface, TImplementation> - Generic metod reflection ile çağırılıyor
                        var addHttpClientMethod = typeof(HttpClientFactoryServiceCollectionExtensions)
                            .GetMethods()
                            .First(m => m.Name == "AddHttpClient"
                                        && m.IsGenericMethodDefinition
                                        && m.GetGenericArguments().Length == 2
                                        && m.GetParameters().Length == 2
                                        && m.GetParameters()[1].ParameterType.Name.Contains("Action"));

                        var genericMethod = addHttpClientMethod.MakeGenericMethod(interfaceType, implementationType);

                        // Action<HttpClient> oluştur
                        Action<HttpClient> configureClient = client =>
                        {
                            // API URL'e /api prefix'i ekle
                            var baseUrl = apiUrl.TrimEnd('/') + "/api/";
                            client.BaseAddress = new Uri(baseUrl);
                            client.Timeout = TimeSpan.FromSeconds(30);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                        };

                        // Metodu çağır: services.AddHttpClient<TInterface, TImplementation>(configureClient)
                        var builderObj = genericMethod.Invoke(null, new object[] { services, configureClient });

                        // Cookie forward handler'ını Typed HttpClient pipeline'ına ekle
                        if (builderObj is IHttpClientBuilder httpClientBuilder)
                        {
                            httpClientBuilder.AddHttpMessageHandler<ApiAuthCookieForwardingHandler>();
                        }

                        registeredCount++;
                        Console.WriteLine($"    ✅ {interfaceType.Name} -> {implementationType.Name} (Typed HttpClient)");
                    }
                    else
                    {
                        Console.WriteLine($"    ⚠️  {interfaceType.Name} için implementation bulunamadı");
                    }
                }

                if (registeredCount > 0)
                {
                    Console.WriteLine($"    🎯 {moduleName}: {registeredCount} API service kayıt edildi");
                }
                else
                {
                    Console.WriteLine($"    ⚠️  {moduleName}: Hiçbir API service kayıt edilemedi");
                }

                return services;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ❌ {moduleName} API Services kayıt hatası: {ex.Message}");
                Console.WriteLine($"    📍 Stack Trace: {ex.StackTrace}");
                return services;
            }
        }

        // ═══════════════════════════════════════════════════════
        // MODULE SERVICES - Interfaces/Concrete Alt Klasörlü
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Interfaces ve Concrete alt klasörleri olan modül servislerini kaydeder
        /// Örnek: AuthenticationServices/Interfaces ve AuthenticationServices/Concrete
        /// </summary>
        private static IServiceCollection RegisterModuleServicesWithSubfolders(
            this IServiceCollection services,
            Assembly assembly,
            string moduleName)
        {
            try
            {
                Console.WriteLine($"📦 {moduleName} servisleri kaydediliyor...");

                var interfaceNamespace = $"SGKPortalApp.PresentationLayer.Services.{moduleName}.Interfaces";
                var concreteNamespace = $"SGKPortalApp.PresentationLayer.Services.{moduleName}.Concrete";

                // Interface'leri bul
                var interfaces = assembly.GetTypes()
                    .Where(t => t.Namespace == interfaceNamespace
                                && t.IsInterface
                                && !t.IsGenericType)
                    .ToList();

                // Implementation'ları bul
                var implementations = assembly.GetTypes()
                    .Where(t => t.Namespace == concreteNamespace
                                && t.IsClass
                                && !t.IsAbstract
                                && !t.IsGenericType)
                    .ToList();

                if (!interfaces.Any() && !implementations.Any())
                {
                    Console.WriteLine($"    ⚠️  {moduleName} modülünde servis bulunamadı");
                    return services;
                }

                Console.WriteLine($"    🔍 Bulunan interface'ler: {interfaces.Count}");
                Console.WriteLine($"    🔧 Bulunan implementation'lar: {implementations.Count}");

                int registeredCount = 0;

                foreach (var interfaceType in interfaces)
                {
                    // Convention: IServiceName -> ServiceName
                    var expectedImplName = interfaceType.Name.Substring(1);

                    var implementationType = implementations.FirstOrDefault(impl =>
                        impl.Name == expectedImplName &&
                        interfaceType.IsAssignableFrom(impl)
                    );

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

                if (registeredCount > 0)
                {
                    Console.WriteLine($"    🎯 {moduleName}: {registeredCount} servis kayıt edildi");
                }

                return services;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ❌ {moduleName} kayıt hatası: {ex.Message}");
                return services;
            }
        }

        // ═══════════════════════════════════════════════════════
        // NORMAL SERVICES - Standart Scoped Kayıt
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Belirtilen namespace'deki tüm servisleri otomatik olarak kaydeder
        /// Interface-Implementation eşleştirmesi yapar (UI Services, Storage Services vb.)
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

                var interfaces = types.Where(t => t.IsInterface).ToList();
                var implementations = types.Where(t => t.IsClass).ToList();

                int registeredCount = 0;

                foreach (var interfaceType in interfaces)
                {
                    var implementationType = implementations.FirstOrDefault(impl =>
                        interfaceType.IsAssignableFrom(impl) &&
                        impl.Name == interfaceType.Name.Substring(1)
                    );

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

                var standaloneServices = implementations
                    .Where(impl => !interfaces.Any(i => i.IsAssignableFrom(impl)))
                    .ToList();

                foreach (var serviceType in standaloneServices)
                {
                    services.AddScoped(serviceType);
                    registeredCount++;
                    Console.WriteLine($"  ✅ {serviceType.Name} (Concrete)");
                }

                if (ensureCriticalServices != null)
                {
                    foreach (var criticalInterface in ensureCriticalServices)
                    {
                        if (!services.Any(s => s.ServiceType == criticalInterface))
                        {
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