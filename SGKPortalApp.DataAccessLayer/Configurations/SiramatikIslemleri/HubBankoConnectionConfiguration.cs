using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.SiramatikIslemleri
{
    public class HubBankoConnectionConfiguration : IEntityTypeConfiguration<HubBankoConnection>
    {
        public void Configure(EntityTypeBuilder<HubBankoConnection> builder)
        {
            builder.ToTable("SIR_HubBankoConnections", "dbo");

            builder.HasKey(hbc => hbc.HubBankoConnectionId);

            builder.Property(hbc => hbc.HubBankoConnectionId)
                .ValueGeneratedOnAdd();

            builder.Property(hbc => hbc.HubConnectionId)
                .IsRequired()
                .HasComment("HubConnection ID - ZORUNLU (Personel olmak zorunda)");

            builder.Property(hbc => hbc.BankoId)
                .IsRequired()
                .HasComment("Banko ID - UNIQUE (Bir bankoya sadece 1 personel)");

            builder.Property(hbc => hbc.TcKimlikNo)
                .IsRequired()
                .HasMaxLength(11)
                .HasComment("Personel TcKimlikNo - UNIQUE (Bir personel aynı anda sadece 1 bankoda)");

            builder.Property(hbc => hbc.BankoModuAktif)
                .IsRequired()
                .HasDefaultValue(true)
                .HasComment("Banko modu aktif mi?");

            builder.Property(hbc => hbc.BankoModuBaslangic)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Banko moduna giriş zamanı");

            builder.Property(hbc => hbc.BankoModuBitis)
                .HasComment("Banko modundan çıkış zamanı (nullable)");

            builder.Property(hbc => hbc.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(hbc => hbc.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(hbc => hbc.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            // Index'ler
            // HubConnectionId unique (Her bağlantı sadece 1 bankoya bağlı)
            builder.HasIndex(hbc => hbc.HubConnectionId)
                .IsUnique()
                .HasDatabaseName("IX_SIR_HubBankoConnections_HubConnectionId")
                .HasFilter("[SilindiMi] = 0");

            // BankoId unique (Bir bankoya sadece 1 personel)
            builder.HasIndex(hbc => hbc.BankoId)
                .IsUnique()
                .HasDatabaseName("IX_SIR_HubBankoConnections_BankoId")
                .HasFilter("[SilindiMi] = 0 AND [BankoModuAktif] = 1");

            // TcKimlikNo unique (Bir personel aynı anda sadece 1 bankoda)
            builder.HasIndex(hbc => hbc.TcKimlikNo)
                .IsUnique()
                .HasDatabaseName("IX_SIR_HubBankoConnections_TcKimlikNo")
                .HasFilter("[SilindiMi] = 0 AND [BankoModuAktif] = 1");

            builder.HasQueryFilter(hbc => !hbc.SilindiMi);

            // HubConnection ile 1-1 ilişki (Zorunlu)
            builder.HasOne(hbc => hbc.HubConnection)
                .WithOne(hc => hc.HubBankoConnection)
                .HasForeignKey<HubBankoConnection>(hbc => hbc.HubConnectionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SIR_HubBankoConnections_CMN_HubConnections");

            // Banko ile 1-1 ilişki (Zorunlu)
            builder.HasOne(hbc => hbc.Banko)
                .WithOne(b => b.HubBankoConnection)
                .HasForeignKey<HubBankoConnection>(hbc => hbc.BankoId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_HubBankoConnections_SIR_Bankolar");
        }
    }
}
