using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;

namespace SGKPortalApp.DataAccessLayer.Configurations.ZKTeco
{
    public class CekilenDataConfiguration : IEntityTypeConfiguration<CekilenData>
    {
        private const string TablePrefix = "ZKTeco_";

        public void Configure(EntityTypeBuilder<CekilenData> builder)
        {
            // Table
            builder.ToTable($"{TablePrefix}CekilenData");

            // Primary Key
            builder.HasKey(c => c.Id);

            // Properties
            builder.Property(c => c.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(c => c.KayitNo)
                   .HasMaxLength(50);

            builder.Property(c => c.Dogrulama)
                   .HasMaxLength(50);

            builder.Property(c => c.GirisCikisModu)
                   .HasMaxLength(50);

            builder.Property(c => c.WorkCode)
                   .HasMaxLength(50);

            builder.Property(c => c.Reserved)
                   .HasMaxLength(50);

            builder.Property(c => c.CihazAdi)
                   .HasMaxLength(50);

            builder.Property(c => c.CihazIp)
                   .HasMaxLength(50);

            builder.Property(c => c.IsProcessed)
                   .HasDefaultValue(false);

            builder.Property(c => c.CekilmeTarihi)
                   .HasDefaultValueSql("GETDATE()");

            // Relationships
            builder.HasOne(c => c.Device)
                   .WithMany()
                   .HasForeignKey(c => c.DeviceId)
                   .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(c => c.KayitNo)
                   .HasDatabaseName($"IX_{TablePrefix}CekilenData_KayitNo");

            builder.HasIndex(c => c.DeviceId)
                   .HasDatabaseName($"IX_{TablePrefix}CekilenData_DeviceId");

            builder.HasIndex(c => c.Tarih)
                   .HasDatabaseName($"IX_{TablePrefix}CekilenData_Tarih");

            builder.HasIndex(c => c.IsProcessed)
                   .HasDatabaseName($"IX_{TablePrefix}CekilenData_IsProcessed");

            builder.HasIndex(c => new { c.KayitNo, c.Tarih, c.Saat })
                   .HasDatabaseName($"IX_{TablePrefix}CekilenData_Lookup");
        }
    }
}
