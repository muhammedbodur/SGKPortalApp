using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.Common
{
    public class UserDomainMappingConfiguration : IEntityTypeConfiguration<UserDomainMapping>
    {
        public void Configure(EntityTypeBuilder<UserDomainMapping> builder)
        {
            builder.ToTable("CMN_UserDomainMappings", "dbo");

            builder.HasKey(udm => udm.UserDomainMappingId);

            builder.Property(udm => udm.UserDomainMappingId)
                .ValueGeneratedOnAdd();

            // ═══════ REQUIRED FIELDS ═══════
            builder.Property(udm => udm.TcKimlikNo)
                .IsRequired()
                .HasMaxLength(11);

            builder.Property(udm => udm.DomainUser)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(udm => udm.MachineName)
                .HasMaxLength(100);

            builder.Property(udm => udm.LastVerified)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(udm => udm.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(udm => udm.Notes)
                .HasMaxLength(500);

            // ═══════ BASE ENTITY PROPERTIES ═══════
            builder.Property(udm => udm.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            // ═══════ INDEXES ═══════

            // 1. Unique constraint: Her TcKimlikNo için tek bir aktif mapping
            builder.HasIndex(udm => new { udm.TcKimlikNo, udm.IsActive })
                .HasDatabaseName("IX_CMN_UserDomainMappings_TcKimlikNo_IsActive")
                .IsUnique()
                .HasFilter("[IsActive] = 1"); // Sadece aktif kayıtlar unique

            // 2. Domain user bazlı sorgular
            builder.HasIndex(udm => udm.DomainUser)
                .HasDatabaseName("IX_CMN_UserDomainMappings_DomainUser");

            // 3. TcKimlikNo bazlı hızlı lookup
            builder.HasIndex(udm => udm.TcKimlikNo)
                .HasDatabaseName("IX_CMN_UserDomainMappings_TcKimlikNo");

            // 4. Machine name bazlı sorgular
            builder.HasIndex(udm => udm.MachineName)
                .HasDatabaseName("IX_CMN_UserDomainMappings_MachineName")
                .HasFilter("[MachineName] IS NOT NULL");

            // 5. Last verified date (bakım için)
            builder.HasIndex(udm => udm.LastVerified)
                .HasDatabaseName("IX_CMN_UserDomainMappings_LastVerified");
        }
    }
}
