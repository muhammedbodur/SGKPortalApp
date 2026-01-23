using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SGKPortalApp.DataAccessLayer.Context
{
    /// <summary>
    /// Design-time DbContext factory for EF Core migrations
    /// Bu sayede "Add-Migration" komutu otomatik olarak SGKDbContext kullanır
    /// </summary>
    public class SGKDbContextFactory : IDesignTimeDbContextFactory<SGKDbContext>
    {
        public SGKDbContext CreateDbContext(string[] args)
        {
            // DataAccessLayer projesinin root dizinini bul
            var basePath = Directory.GetCurrentDirectory();
            
            // Configuration dosyasını yükle
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile(Path.Combine(basePath, "..", "appsettings.Shared.json"), optional: true)
                .Build();

            // DbContextOptionsBuilder oluştur
            var optionsBuilder = new DbContextOptionsBuilder<SGKDbContext>();
            
            // Connection string'i al
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            // SQL Server kullan
            optionsBuilder.UseSqlServer(connectionString);

            return new SGKDbContext(optionsBuilder.Options);
        }
    }
}
