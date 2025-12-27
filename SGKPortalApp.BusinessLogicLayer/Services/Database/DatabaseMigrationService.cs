using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Database;
using SGKPortalApp.DataAccessLayer.Context;

namespace SGKPortalApp.BusinessLogicLayer.Services.Database
{
    /// <summary>
    /// Database Migration yönetimi servisi
    /// Production deployment sırasında migration'ları otomatik uygular
    /// Development'ta manuel migration kullanılır (Add-Migration, Update-Database)
    /// </summary>
    public class DatabaseMigrationService : IDatabaseMigrationService
    {
        private readonly SGKDbContext _context;
        private readonly ILogger<DatabaseMigrationService> _logger;

        public DatabaseMigrationService(
            SGKDbContext context,
            ILogger<DatabaseMigrationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task ApplyMigrationsAsync()
        {
            try
            {
                var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
                
                if (pendingMigrations.Any())
                {
                    _logger.LogInformation("📊 Bekleyen migration'lar uygulanıyor...");
                    await _context.Database.MigrateAsync();
                    _logger.LogInformation("✅ Migration'lar başarıyla uygulandı");
                }
                else
                {
                    _logger.LogInformation("✅ Veritabanı güncel");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Migration hatası: {Message}", ex.Message);
                throw;
            }
        }
    }
}
