using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using System.Reflection;
using SGKPortalApp.DataAccessLayer.Repositories;
using SGKPortalApp.BusinessLogicLayer.Extensions;

namespace SGKPortalApp.Common.Extensions
{
    /// <summary>
    /// SGK Portal için Dependency Injection Extensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Veri Erişim Katmanını kaydet
        /// </summary>
        public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, string connectionString)
        {
            // DbContext
            services.AddDbContext<SGKDbContext>(options =>
            {
                options.UseSqlServer(connectionString);

#if DEBUG
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
#endif
            });

            // UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Repository'leri otomatik kaydet
            services.AddRepositoriesAutomatically();

            return services;
        }

        /// <summary>
        /// Repository'leri otomatik kaydet
        /// </summary>
        private static IServiceCollection AddRepositoriesAutomatically(this IServiceCollection services)
        {
            // DataAccessLayer assembly'sini al
            var dataAccessAssembly = typeof(SGKDbContext).Assembly;

            try
            {
                Console.WriteLine("🔍 Repository otomatik kayıt başlıyor...");
                Console.WriteLine($"📦 Assembly: {dataAccessAssembly.GetName().Name}");

                // TÜM repository interface'lerini bul - SADE FİLTRE
                var repositoryInterfaces = dataAccessAssembly.GetTypes()
                    .Where(t => t.IsInterface &&
                               t.Name.EndsWith("Repository") &&
                               t.Name.StartsWith("I") && // I ile başlayan
                               t.Name != "IGenericRepository`1" &&
                               t.Name != "IUnitOfWork")
                    .OrderBy(t => t.Name) // Alfabetik sırala
                    .ToList();

                Console.WriteLine($"📋 Bulunan interface'ler ({repositoryInterfaces.Count}):");
                foreach (var iface in repositoryInterfaces)
                {
                    Console.WriteLine($"   - {iface.Name} ({iface.Namespace})");
                }

                // TÜM repository implementation'larını bul
                var repositoryImplementations = dataAccessAssembly.GetTypes()
                    .Where(t => t.IsClass &&
                               !t.IsAbstract &&
                               t.Name.EndsWith("Repository") &&
                               !t.Name.Contains("Generic") &&
                               !t.Name.Contains("UnitOfWork"))
                    .OrderBy(t => t.Name)
                    .ToList();

                Console.WriteLine($"🔧 Bulunan implementation'lar ({repositoryImplementations.Count}):");
                foreach (var impl in repositoryImplementations)
                {
                    Console.WriteLine($"   - {impl.Name} ({impl.Namespace})");
                }

                var registeredCount = 0;

                // Her interface için implementation ara
                foreach (var interfaceType in repositoryInterfaces)
                {
                    // IPersonelRepository -> PersonelRepository
                    var expectedImplName = interfaceType.Name.Substring(1); // "I" çıkar

                    // Implementation'ı bul
                    var implementationType = repositoryImplementations
                        .FirstOrDefault(t => t.Name == expectedImplName);

                    if (implementationType != null)
                    {
                        // Gerçekten interface'i implement ediyor mu kontrol et
                        if (interfaceType.IsAssignableFrom(implementationType))
                        {
                            services.AddScoped(interfaceType, implementationType);
                            registeredCount++;
                            Console.WriteLine($"✅ {interfaceType.Name} -> {implementationType.Name}");
                        }
                        else
                        {
                            Console.WriteLine($"❌ {implementationType.Name} does not implement {interfaceType.Name}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"❌ Implementation not found for: {interfaceType.Name}");
                    }
                }

                Console.WriteLine($"🎉 Toplam {registeredCount} repository başarıyla kaydedildi");

                if (registeredCount == 0)
                {
                    Console.WriteLine("⚠️ UYARI: Hiç repository kaydedilmedi! Lütfen dosya yapısını kontrol edin.");
                    return services.AddRepositoriesManualFallback();
                }
                else if (registeredCount < 10) // En az 10 repository olması beklenir
                {
                    Console.WriteLine($"⚠️ UYARI: Sadece {registeredCount} repository kaydedildi. Eksik olanlar var mı kontrol edin.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Repository kayıt hatası: {ex.Message}");
                Console.WriteLine("🔄 Manual kayıta geçiliyor...");
                return services.AddRepositoriesManualFallback();
            }

            return services;
        }

        /// <summary>
        /// Otomatik kayıt başarısız olursa manuel kayıt devreye girer
        /// </summary>
        private static IServiceCollection AddRepositoriesManualFallback(this IServiceCollection services)
        {
            Console.WriteLine("🔧 Manuel repository kayıt yapılıyor...");

            try
            {
                // Personel Repository'leri
                services.AddScoped<SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri.IPersonelRepository,
                                  SGKPortalApp.DataAccessLayer.Repositories.Concrete.PersonelIslemleri.PersonelRepository>();

                services.AddScoped<SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri.IDepartmanRepository,
                                  SGKPortalApp.DataAccessLayer.Repositories.Concrete.PersonelIslemleri.DepartmanRepository>();

                services.AddScoped<SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri.IServisRepository,
                                  SGKPortalApp.DataAccessLayer.Repositories.Concrete.PersonelIslemleri.ServisRepository>();

                services.AddScoped<SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri.IUnvanRepository,
                                  SGKPortalApp.DataAccessLayer.Repositories.Concrete.PersonelIslemleri.UnvanRepository>();

                services.AddScoped<SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri.ISendikaRepository,
                                  SGKPortalApp.DataAccessLayer.Repositories.Concrete.PersonelIslemleri.SendikaRepository>();

                services.AddScoped<SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri.IYetkiRepository,
                                  SGKPortalApp.DataAccessLayer.Repositories.Concrete.PersonelIslemleri.YetkiRepository>();

                services.AddScoped<SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri.IPersonelYetkiRepository,
                                  SGKPortalApp.DataAccessLayer.Repositories.Concrete.PersonelIslemleri.PersonelYetkiRepository>();

                // Sıramatik Repository'leri  
                services.AddScoped<SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri.IBankoRepository,
                                  SGKPortalApp.DataAccessLayer.Repositories.Concrete.SiramatikIslemleri.BankoRepository>();

                services.AddScoped<SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri.ITvRepository,
                                  SGKPortalApp.DataAccessLayer.Repositories.Concrete.SiramatikIslemleri.TvRepository>();

                // Common Repository'leri
                services.AddScoped<SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common.IHizmetBinasiRepository,
                                  SGKPortalApp.DataAccessLayer.Repositories.Concrete.Common.HizmetBinasiRepository>();

                Console.WriteLine("✅ Manuel repository kayıt tamamlandı");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Manuel kayıt bile başarısız: {ex.Message}");
                throw new InvalidOperationException("Repository kayıt tamamen başarısız oldu", ex);
            }

            return services;
        }

        /// <summary>
        /// Business Layer servisleri
        /// </summary>
        public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
        {
            services.AddBusinessLogicLayer();

            return services;
        }

        /// <summary>
        /// Tüm SGK Portal servislerini kaydet - ANA METOT
        /// </summary>
        public static IServiceCollection AddSGKPortalServices(this IServiceCollection services, string connectionString)
        {
            Console.WriteLine("🚀 SGK Portal Services başlatılıyor...");

            // Memory Cache
            services.AddMemoryCache();

            // Data Access Layer
            services.AddDataAccessLayer(connectionString);

            // Business Layer
            services.AddBusinessLayer();

            Console.WriteLine("🎉 SGK Portal Services hazır!");
            return services;
        }

        /// <summary>
        /// Repository kayıt testini çalıştır
        /// </summary>
        public static void TestRepositoryRegistration(this IServiceProvider serviceProvider)
        {
            Console.WriteLine("🧪 Repository kayıt testi başlıyor...");

            using var scope = serviceProvider.CreateScope();
            var provider = scope.ServiceProvider;

            // Test edilecek repository'ler - TAM NAMESPACE ile
            var testCases = new[]
            {
                (typeof(SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri.IPersonelRepository), "IPersonelRepository"),
                (typeof(SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri.IDepartmanRepository), "IDepartmanRepository"),
                (typeof(SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri.IBankoRepository), "IBankoRepository"),
                (typeof(SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common.IHizmetBinasiRepository), "IHizmetBinasiRepository"),
                (typeof(IUnitOfWork), "IUnitOfWork")
            };

            var successCount = 0;
            foreach (var (repoType, name) in testCases)
            {
                try
                {
                    var repository = provider.GetService(repoType);
                    if (repository != null)
                    {
                        Console.WriteLine($"✅ {name} başarıyla alındı");
                        successCount++;
                    }
                    else
                    {
                        Console.WriteLine($"❌ {name} NULL döndü - kayıtlı değil");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"💥 {name} hata: {ex.Message}");
                }
            }

            var percentage = (successCount * 100) / testCases.Length;
            Console.WriteLine($"📊 Test sonucu: {successCount}/{testCases.Length} başarılı (%{percentage})");

            if (percentage >= 80)
            {
                Console.WriteLine("🎉 Repository kayıt sistemi çalışıyor!");
            }
            else if (percentage >= 50)
            {
                Console.WriteLine("⚠️ Repository kayıt sistemi kısmen çalışıyor.");
            }
            else
            {
                Console.WriteLine("💥 Repository kayıt sistemi çalışmıyor! Manuel kayıt gerekli.");
            }
        }
    }
}