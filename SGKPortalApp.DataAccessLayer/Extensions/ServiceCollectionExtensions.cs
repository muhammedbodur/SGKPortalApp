using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Services.Database;
using SGKPortalApp.DataAccessLayer.Services.Interfaces;
using System.Reflection;

namespace SGKPortalApp.DataAccessLayer.Extensions
{
    /// <summary>
    /// DataAccessLayer servis kayıtları
    /// Repository Pattern ve UnitOfWork
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// DataAccessLayer servislerini DI container'a kaydet
        /// </summary>
        public static IServiceCollection AddDataAccessLayer(
            this IServiceCollection services,
            string connectionString)
        {
            Console.WriteLine("📦 DataAccessLayer servisleri kaydediliyor...");

            // DbContext kaydet
            services.AddDbContext<SGKDbContext>((serviceProvider, options) =>
            {
                options.UseSqlServer(connectionString);

                // Audit logging interceptor'ı ekle
                options.AddAuditLoggingInterceptor(serviceProvider);
            });
            Console.WriteLine("  ✅ SGKDbContext kaydedildi");

            // UnitOfWork Pattern
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            Console.WriteLine("  ✅ IUnitOfWork → UnitOfWork");

            // Database Migration Service
            services.AddScoped<IDatabaseMigrationService, DatabaseMigrationService>();
            Console.WriteLine("  ✅ IDatabaseMigrationService → DatabaseMigrationService");

            // Tüm repository interface ve implementation'larını otomatik kaydet
            RegisterRepositories(services);

            Console.WriteLine("  🎉 DataAccessLayer hazır!\n");
            return services;
        }

        /// <summary>
        /// Assembly içindeki tüm repository interface ve implementation'larını otomatik kaydet
        /// Convention: IEntityRepository (interface) → EntityRepository (concrete)
        /// </summary>
        private static void RegisterRepositories(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Tüm repository interface'lerini bul
            var repositoryInterfaces = assembly.GetTypes()
                .Where(t => t.IsInterface &&
                           t.Name.EndsWith("Repository") &&
                           t.Namespace != null &&
                           t.Namespace.Contains("Repositories.Interfaces") &&
                           t != typeof(IUnitOfWork) &&
                           !t.Name.Contains("IGenericRepository"))
                .ToList();

            int registeredCount = 0;

            foreach (var interfaceType in repositoryInterfaces)
            {
                // Implementation'ı bul (convention: IEntityRepository → EntityRepository)
                var expectedImplementationName = interfaceType.Name.Substring(1); // "I" karakterini kaldır

                var implementationType = assembly.GetTypes()
                    .FirstOrDefault(t => !t.IsInterface &&
                                       !t.IsAbstract &&
                                       t.Name == expectedImplementationName &&
                                       interfaceType.IsAssignableFrom(t));

                if (implementationType != null)
                {
                    services.AddScoped(interfaceType, implementationType);
                    registeredCount++;
                }
            }

            Console.WriteLine($"  ✅ {registeredCount} Repository kaydedildi (auto-discovery)");
        }
    }
}
