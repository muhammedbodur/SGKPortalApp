using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;

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
                .HasComment("TC Kimlik Numarası - Primary Key & Foreign Key to Personel");

            builder.Property(u => u.PassWord)
                .IsRequired()
                .HasMaxLength(255)
                .HasComment("Şifre (hash'lenmiş)");

            builder.Property(u => u.SessionID)
                .HasMaxLength(100)
                .HasComment("Aktif oturum ID");

            builder.Property(u => u.AktifMi)
                .IsRequired()
                .HasDefaultValue(true)
                .HasComment("Kullanıcı aktif mi?");

            builder.Property(u => u.SonGirisTarihi)
                .HasComment("Son giriş tarihi");

            builder.Property(u => u.BasarisizGirisSayisi)
                .IsRequired()
                .HasDefaultValue(0)
                .HasComment("Başarısız giriş denemesi sayısı");

            builder.Property(u => u.HesapKilitTarihi)
                .HasComment("Hesap kilitlenme tarihi");

            builder.Property(u => u.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(u => u.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(u => u.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            // Personel ile One-to-One ilişki
            builder.HasOne(u => u.Personel)
                .WithOne(p => p.User)
                .HasForeignKey<User>(u => u.TcKimlikNo)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_CMN_Users_PER_Personeller");

            // HubConnection ile One-to-One ilişki
            builder.HasOne(u => u.HubConnection)
                .WithOne(h => h.User)
                .HasForeignKey<HubConnection>(h => h.TcKimlikNo)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SIR_HubConnections_CMN_Users");

            // Indexes
            builder.HasIndex(u => u.TcKimlikNo)
                .IsUnique()
                .HasDatabaseName("IX_CMN_Users_TcKimlikNo")
                .HasFilter("[SilindiMi] = 0");

            builder.HasIndex(u => u.SessionID)
                .HasDatabaseName("IX_CMN_Users_SessionID")
                .HasFilter("[SessionID] IS NOT NULL AND [SilindiMi] = 0");

            builder.HasQueryFilter(u => !u.SilindiMi);
        }
    }
}