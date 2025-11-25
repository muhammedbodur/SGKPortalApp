using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.SiramatikIslemleri
{
    public class HubTvConnectionConfiguration : IEntityTypeConfiguration<HubTvConnection>
    {
        public void Configure(EntityTypeBuilder<HubTvConnection> builder)
        {
            builder.ToTable("SIR_HubTvConnections", "dbo");

            builder.HasKey(htc => htc.HubTvConnectionId);

            builder.Property(htc => htc.HubTvConnectionId)
                .ValueGeneratedOnAdd();

            builder.Property(htc => htc.TvId)
                .IsRequired()
                .HasComment("TV ID - Birden fazla kullanıcı aynı TV'yi izleyebilir");

            builder.Property(htc => htc.HubConnectionId)
                .IsRequired()
                .HasComment("HubConnection ID - ZORUNLU (TV User veya Personel)");

            builder.Property(htc => htc.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(htc => htc.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(htc => htc.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            // Index'ler
            // HubConnectionId unique (Her bağlantı sadece 1 TV'ye bağlı)
            builder.HasIndex(htc => htc.HubConnectionId)
                .IsUnique()
                .HasDatabaseName("IX_SIR_HubTvConnections_HubConnectionId")
                .HasFilter("[SilindiMi] = 0");

            // TvId index (Birden fazla bağlantı aynı TV'yi izleyebilir - UNIQUE DEĞİL)
            builder.HasIndex(htc => htc.TvId)
                .HasDatabaseName("IX_SIR_HubTvConnections_TvId")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(htc => !htc.SilindiMi);

            // HubConnection ile One-to-One ilişki
            builder.HasOne(htc => htc.HubConnection)
                .WithOne(hc => hc.HubTvConnection)
                .HasForeignKey<HubTvConnection>(htc => htc.HubConnectionId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true)
                .HasConstraintName("FK_SIR_HubTvConnections_CMN_HubConnections");

            // TV ile Many-to-One ilişki (Birden fazla bağlantı aynı TV'yi izleyebilir)
            builder.HasOne(htc => htc.Tv)
                .WithMany(t => t.HubTvConnections)
                .HasForeignKey(htc => htc.TvId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(true)
                .HasConstraintName("FK_SIR_HubTvConnections_SIR_Tvler");
        }
    }
}