using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;

namespace SGKPortalApp.DataAccessLayer.Configurations.ZKTeco
{
    public class DeviceConfiguration : IEntityTypeConfiguration<Device>
    {
        private const string TablePrefix = "ZKTeco_";

        public void Configure(EntityTypeBuilder<Device> builder)
        {
            // Table
            builder.ToTable($"{TablePrefix}Device");

            // Primary Key
            builder.HasKey(d => d.Id);

            // Properties
            builder.Property(d => d.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(d => d.DeviceName)
                   .HasMaxLength(250);

            builder.Property(d => d.IpAddress)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(d => d.Port)
                   .HasMaxLength(50)
                   .HasDefaultValue("4370");

            builder.Property(d => d.DeviceCode)
                   .HasMaxLength(50);

            builder.Property(d => d.DeviceInfo)
                   .HasMaxLength(255);

            builder.Property(d => d.IsActive)
                   .HasDefaultValue(true);

            // Relationships
            builder.HasOne(d => d.HizmetBinasi)
                   .WithMany()
                   .HasForeignKey(d => d.HizmetBinasiId)
                   .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(d => d.IpAddress)
                   .HasDatabaseName($"IX_{TablePrefix}Device_IpAddress");

            builder.HasIndex(d => d.DeviceCode)
                   .IsUnique()
                   .HasFilter("[DeviceCode] IS NOT NULL")
                   .HasDatabaseName($"IX_{TablePrefix}Device_DeviceCode");

            builder.HasIndex(d => d.IsActive)
                   .HasDatabaseName($"IX_{TablePrefix}Device_IsActive");
        }
    }
}
