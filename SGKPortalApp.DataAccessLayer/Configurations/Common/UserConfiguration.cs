using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.Common
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("CMN_Users", "dbo");

            builder.HasKey(u => u.TcKimlikNo);

            builder.Property(u => u.TcKimlikNo)
                .IsRequired()
                .HasMaxLength(11)
                .HasComment("TC Kimlik Numarası - Primary Key");

            builder.Property(u => u.KullaniciAdi)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.TelefonNo)
                .HasMaxLength(20);

            builder.Property(u => u.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(u => u.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(u => u.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(u => u.TcKimlikNo)
                .IsUnique()
                .HasDatabaseName("IX_CMN_Users_TcKimlikNo")
                .HasFilter("[SilindiMi] = 0");

            builder.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_CMN_Users_Email")
                .HasFilter("[SilindiMi] = 0");

            builder.HasIndex(u => u.KullaniciAdi)
                .IsUnique()
                .HasDatabaseName("IX_CMN_Users_KullaniciAdi")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(u => !u.SilindiMi);
        }
    }
}