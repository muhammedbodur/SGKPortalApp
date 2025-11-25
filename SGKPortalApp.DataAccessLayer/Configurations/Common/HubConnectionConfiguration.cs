using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.Common
{
    public class HubConnectionConfiguration : IEntityTypeConfiguration<HubConnection>
    {
        public void Configure(EntityTypeBuilder<HubConnection> builder)
        {
            builder.ToTable("CMN_HubConnections", "dbo");

            builder.HasKey(hc => hc.HubConnectionId);

            builder.Property(hc => hc.HubConnectionId)
                .ValueGeneratedOnAdd();

            builder.Property(hc => hc.TcKimlikNo)
                .IsRequired()
                .HasMaxLength(11)
                .HasComment("User TcKimlikNo - ZORUNLU (Personel veya TV User) - Bir kullanıcının birden fazla bağlantısı olabilir");

            builder.Property(hc => hc.ConnectionId)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("SignalR ConnectionId - Unique");

            builder.Property(hc => hc.ConnectionType)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("MainLayout")
                .HasComment("Bağlantı Tipi: MainLayout, TvDisplay, BankoMode, Monitoring");

            builder.Property(hc => hc.ConnectionStatus)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(hc => hc.ConnectedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Bağlantı kurulma zamanı");

            builder.Property(hc => hc.LastActivityAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Son aktivite zamanı");

            builder.Property(hc => hc.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(hc => hc.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(hc => hc.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            // Index'ler
            // ConnectionId unique olmalı (Her SignalR bağlantısı benzersiz)
            builder.HasIndex(hc => hc.ConnectionId)
                .IsUnique()
                .HasDatabaseName("IX_CMN_HubConnections_ConnectionId")
                .HasFilter("[SilindiMi] = 0");

            // TcKimlikNo + ConnectionType composite index (Bir kullanıcının aynı tipte birden fazla bağlantısı olabilir)
            builder.HasIndex(hc => new { hc.TcKimlikNo, hc.ConnectionType, hc.ConnectionStatus })
                .HasDatabaseName("IX_CMN_HubConnections_Tc_Type_Status")
                .HasFilter("[SilindiMi] = 0");

            // ConnectionStatus index (Online bağlantıları hızlı bulmak için)
            builder.HasIndex(hc => hc.ConnectionStatus)
                .HasDatabaseName("IX_CMN_HubConnections_Status")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(hc => !hc.SilindiMi);

            // HubConnection - User ilişkisi (Many-to-One)
            // Bir kullanıcının birden fazla HubConnection'ı olabilir (farklı tab'lar)
            builder.HasOne(hc => hc.User)
                .WithMany(u => u.HubConnections)
                .HasForeignKey(hc => hc.TcKimlikNo)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true)
                .HasConstraintName("FK_CMN_HubConnections_CMN_Users");
        }
    }
}