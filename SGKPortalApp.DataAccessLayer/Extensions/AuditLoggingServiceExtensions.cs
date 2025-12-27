using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessLogicLayer.Services.Common;
using SGKPortalApp.BusinessObjectLayer.Options;
using SGKPortalApp.DataAccessLayer.Services.Audit;

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
            services.Configure<AuditLoggingOptions>(
                configuration.GetSection(AuditLoggingOptions.SectionName));

            // Core services
            services.AddScoped<ICurrentUserService, CurrentUserService>();
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
