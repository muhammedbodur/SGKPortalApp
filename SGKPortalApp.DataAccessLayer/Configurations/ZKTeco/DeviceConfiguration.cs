using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;

namespace SGKPortalApp.DataAccessLayer.Configurations.ZKTeco
{
    public class DeviceConfiguration : IEntityTypeConfiguration<Device>
    {
        private const string TablePrefix = "ZKTeco_";
        private const string IndexPrefix = "IX_ZKTeco_";

        public void Configure(EntityTypeBuilder<Device> builder)
        {
            // Table
            builder.ToTable($"{TablePrefix}Device");

            // Primary Key
            builder.HasKey(d => d.DeviceId);

            // Properties
            builder.Property(d => d.DeviceId)
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

            // Foreign Key (HizmetBinasiId) - Navigation property yok, sadece FK

            // Indexes
            builder.HasIndex(d => d.IpAddress)
                   .HasDatabaseName($"{IndexPrefix}Device_IpAddress");

            builder.HasIndex(d => d.DeviceCode)
                   .IsUnique()
                   .HasFilter("[DeviceCode] IS NOT NULL")
                   .HasDatabaseName($"{IndexPrefix}Device_DeviceCode");

            builder.HasIndex(d => d.IsActive)
                   .HasDatabaseName($"{IndexPrefix}Device_IsActive");
        }
    }
}
