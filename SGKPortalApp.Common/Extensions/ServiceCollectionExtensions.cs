using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Concrete;
using System.Reflection;

namespace SGKPortalApp.Common.Extensions
{
    /// <summary>
    /// Dependency Injection için basit extension metodları
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Veri Erişim Katmanını kaydet
        /// </summary>
        public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, string connectionString)
        {
            // Veritabanı bağlantısı
            services.AddDbContext<SGKDbContext>(options =>
                options.UseSqlServer(connectionString));

            // UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Repository'leri otomatik kaydet
            services.AddRepositories();

            return services;
        }

        /// <summary>
        /// Repository'leri otomatik kaydet
        /// </summary>
        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Assembly'yi al
            var assembly = Assembly.GetExecutingAssembly();

            // Repository interface'lerini bul
            var repositoryInterfaces = assembly.GetTypes()
                .Where(t => t.IsInterface && t.Name.EndsWith("Repository") && t.Name != "IGenericRepository`1")
                .ToList();

            // Her interface için implementation bul ve kaydet
            foreach (var interfaceType in repositoryInterfaces)
            {
                // IPersonelRepository -> PersonelRepository
                var implementationName = interfaceType.Name.Substring(1); // "I" yi çıkar
                var implementationType = assembly.GetTypes()
                    .FirstOrDefault(t => t.Name == implementationName && t.IsClass);

                if (implementationType != null)
                {
                    services.AddScoped(interfaceType, implementationType);
                }
            }

            return services;
        }

        /// <summary>
        /// Tüm katmanları kaydet
        /// </summary>
        public static IServiceCollection AddSGKPortalServices(this IServiceCollection services, string connectionString)
        {
            // Veri katmanı
            services.AddDataAccessLayer(connectionString);

            // Business katmanı (ileride eklenecek)
            // services.AddBusinessLayer();

            return services;
        }
    }
}