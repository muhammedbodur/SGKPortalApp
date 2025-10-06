using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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

            builder.Property(htc => htc.ConnectionId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(htc => htc.ConnectionStatus)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(htc => htc.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(htc => htc.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(htc => htc.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(htc => new { htc.TvId, htc.ConnectionId, htc.ConnectionStatus })
                .HasDatabaseName("IX_SIR_HubTvConnections_Tv_ConnId_Status");

            builder.HasIndex(htc => htc.TvId)
                .IsUnique()
                .HasDatabaseName("IX_SIR_HubTvConnections_TvId")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(htc => !htc.SilindiMi);

            builder.HasOne(htc => htc.Tv)
                .WithOne(t => t.HubTvConnection)
                .HasForeignKey<HubTvConnection>(htc => htc.TvId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_HubTvConnections_SIR_Tvler");
        }
    }
}