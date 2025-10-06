using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.Common
{
    public class LoginLogoutLogConfiguration : IEntityTypeConfiguration<LoginLogoutLog>
    {
        public void Configure(EntityTypeBuilder<LoginLogoutLog> builder)
        {
            builder.ToTable("CMN_LoginLogoutLogs", "dbo");

            builder.HasKey(ll => ll.LoginLogoutLogId);

            builder.Property(ll => ll.LoginLogoutLogId)
                .ValueGeneratedOnAdd();

            builder.Property(ll => ll.TcKimlikNo)
                .IsRequired()
                .HasMaxLength(11);

            builder.Property(ll => ll.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.HasIndex(ll => ll.TcKimlikNo)
                .HasDatabaseName("IX_CMN_LoginLogoutLogs_TcKimlikNo");

            builder.HasIndex(ll => ll.EklenmeTarihi)
                .HasDatabaseName("IX_CMN_LoginLogoutLogs_EklenmeTarihi");

        }
    }
}