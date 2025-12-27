using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SGKPortalApp.BusinessObjectLayer.Options;
using SGKPortalApp.Common.Interfaces;
using SGKPortalApp.DataAccessLayer.Services.Audit;
using System.IO;

namespace SGKPortalApp.DataAccessLayer.Extensions
{
    /// <summary>
    /// Audit logging sistemi için DI extension methods
    /// </summary>
    public static class AuditLoggingServiceExtensions
    {
        /// <summary>
        /// Audit logging sistemini DI container'a ekler
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Configuration</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddAuditLogging(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Configuration options
            var section = configuration.GetSection(AuditLoggingOptions.SectionName);
            services.AddOptions<AuditLoggingOptions>()
                .Bind(section)
                .PostConfigure(options =>
                {
                    // Relative path ise solution root'a göre absolute path'e çevir
                    if (!Path.IsPathRooted(options.BasePath))
                    {
                        // ApiLayer veya PresentationLayer working directory'sinden çık, solution root'a git
                        var currentDir = Directory.GetCurrentDirectory();
                        var solutionRoot = Directory.GetParent(currentDir)?.FullName ?? currentDir;
                        options.BasePath = Path.GetFullPath(Path.Combine(solutionRoot, options.BasePath));
                    }

                    // BasePath klasörünü oluştur
                    Directory.CreateDirectory(options.BasePath);
                });

            // Core services
            // Note: ICurrentUserService should be registered by the application layer
            services.AddSingleton<IAuditConfigurationCache, AuditConfigurationCache>();
            services.AddSingleton<IAuditFileWriter, AuditFileWriter>();

            // Interceptor (Scoped olmalı çünkü DbContext scoped)
            services.AddScoped<AuditLogInterceptor>();

            // Background maintenance service
            services.AddHostedService<AuditMaintenanceService>();

            return services;
        }

        /// <summary>
        /// DbContext options builder'a audit interceptor'ı ekler
        /// </summary>
        /// <param name="optionsBuilder">Options builder</param>
        /// <param name="serviceProvider">Service provider (interceptor resolve etmek için)</param>
        /// <returns>Options builder</returns>
        public static DbContextOptionsBuilder AddAuditLoggingInterceptor(
            this DbContextOptionsBuilder optionsBuilder,
            IServiceProvider serviceProvider)
        {
            var interceptor = serviceProvider.GetRequiredService<AuditLogInterceptor>();
            optionsBuilder.AddInterceptors(interceptor);
            return optionsBuilder;
        }
    }
}
