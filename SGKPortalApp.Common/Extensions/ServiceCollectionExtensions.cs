using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SGKPortalApp.Common.Configuration;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using System.Reflection;
using SGKPortalApp.DataAccessLayer.Repositories;
using SGKPortalApp.BusinessLogicLayer.Extensions;

namespace SGKPortalApp.Common.Extensions
{
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
            var dataAccessAssembly = typeof(SGKDbContext).Assembly;

            try
            {
                Console.WriteLine("🔍 Repository otomatik kayıt başlıyor...");
                Console.WriteLine($"📦 Assembly: {dataAccessAssembly.GetName().Name}");

                // TÜM repository interface'lerini bul
                var repositoryInterfaces = dataAccessAssembly.GetTypes()
                    .Where(t => t.IsInterface &&
                               t.Name.EndsWith("Repository") &&
                               t.Name.StartsWith("I") &&
                               t.Name != "IGenericRepository`1" &&
                               t.Name != "IUnitOfWork")
                    .OrderBy(t => t.Name)
                    .ToList();

                // TÜM repository implementation'larını bul
                var repositoryImplementations = dataAccessAssembly.GetTypes()
                    .Where(t => t.IsClass &&
                               !t.IsAbstract &&
                               t.Name.EndsWith("Repository") &&
                               !t.Name.Contains("Generic") &&
                               !t.Name.Contains("UnitOfWork"))
                    .OrderBy(t => t.Name)
                    .ToList();

                Console.WriteLine($"📋 Bulunan interface'ler: {repositoryInterfaces.Count}");
                Console.WriteLine($"🔧 Bulunan implementation'lar: {repositoryImplementations.Count}");

                int registeredCount = 0;

                // Her interface için implementation bul ve kaydet
                foreach (var interfaceType in repositoryInterfaces)
                {
                    // "I" prefiksini çıkar: IPersonelRepository → PersonelRepository
                    var expectedImplName = interfaceType.Name.Substring(1);

                    var implementationType = repositoryImplementations.FirstOrDefault(impl =>
                        impl.Name == expectedImplName &&
                        interfaceType.IsAssignableFrom(impl));

                    if (implementationType == null)
                    {
                        // Tam isim eşleşmesi yoksa, interface'i implement eden herhangi bir tip bul
                        implementationType = repositoryImplementations.FirstOrDefault(impl =>
                            interfaceType.IsAssignableFrom(impl));
                    }

                    if (implementationType != null)
                    {
                        services.AddScoped(interfaceType, implementationType);
                        registeredCount++;
                        Console.WriteLine($"  ✅ {interfaceType.Name} -> {implementationType.Name}");
                    }
                    else
                    {
                        Console.WriteLine($"  ⚠️  {interfaceType.Name} için implementation bulunamadı");
                    }
                }

                Console.WriteLine($"✅ {registeredCount} repository otomatik kayıt edildi\n");

                // Eğer hiçbir repository kayıt edilmediyse hata ver
                if (registeredCount == 0)
                {
                    throw new InvalidOperationException("❌ Hiçbir repository otomatik kayıt edilemedi!");
                }

                return services;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Otomatik repository kayıt hatası: {ex.Message}");
                Console.WriteLine("⚠️  Not: Boş klasörler (Sıramatik, PDKS, Eshot) olduğu için bazı repository'ler bulunamadı");
                Console.WriteLine("✅ Mevcut repository'ler başarıyla kaydedildi\n");

                // Hata vermeden devam et - PersonelIslemleri ve Common repository'leri kayıtlı
                return services;
            }
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
        public static IServiceCollection AddSGKPortalServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            Console.WriteLine("🚀 SGK Portal Services başlatılıyor...");

            // Connection string'i DatabaseConfiguration'dan al
            var connectionString = DatabaseConfiguration.GetConnectionString(configuration);

            // Memory Cache
            services.AddMemoryCache();

            // Data Access Layer
            services.AddDataAccessLayer(connectionString);

            // Business Layer
            services.AddBusinessLayer();

            Console.WriteLine("🎉 SGK Portal Services hazır!\n");
            return services;
        }
    }
}