using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;

namespace SGKPortalApp.DataAccessLayer.Configurations.ZKTeco
{
    public class ZKTecoDeviceConfiguration : IEntityTypeConfiguration<ZKTecoDevice>
    {
        public void Configure(EntityTypeBuilder<ZKTecoDevice> builder)
        {
            // Table
            builder.ToTable("ZKTeco_Device");

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

            // Indexes
            builder.HasIndex(d => d.IpAddress)
                   .HasDatabaseName("IX_ZKTecoDevice_IpAddress");

            builder.HasIndex(d => d.DeviceCode)
                   .IsUnique()
                   .HasFilter("[DeviceCode] IS NOT NULL")
                   .HasDatabaseName("IX_ZKTecoDevice_DeviceCode");

            builder.HasIndex(d => d.IsActive)
                   .HasDatabaseName("IX_ZKTecoDevice_IsActive");
        }
    }
}
