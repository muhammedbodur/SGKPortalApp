using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.SignalR;

namespace SGKPortalApp.DataAccessLayer.Configurations.SignalR
{
    public class SignalREventLogConfiguration : IEntityTypeConfiguration<SignalREventLog>
    {
        public void Configure(EntityTypeBuilder<SignalREventLog> builder)
        {
            builder.ToTable("SIG_EventLogs", "dbo");

            builder.HasKey(e => e.EventLogId);

            builder.Property(e => e.EventLogId)
                .ValueGeneratedOnAdd();

            builder.Property(e => e.EventId)
                .IsRequired();

            builder.Property(e => e.EventType)
                .IsRequired();

            builder.Property(e => e.EventName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.TargetType)
                .IsRequired();

            builder.Property(e => e.TargetId)
                .HasMaxLength(500);

            builder.Property(e => e.PersonelTc)
                .HasMaxLength(11);

            builder.Property(e => e.PayloadSummary)
                .HasMaxLength(2000);

            builder.Property(e => e.ErrorMessage)
                .HasMaxLength(500);

            builder.Property(e => e.DeliveryStatus)
                .IsRequired();

            builder.Property(e => e.SentAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            // Indexes for common queries
            builder.HasIndex(e => e.EventId)
                .IsUnique()
                .HasDatabaseName("IX_SIG_EventLogs_EventId");

            builder.HasIndex(e => e.SentAt)
                .HasDatabaseName("IX_SIG_EventLogs_SentAt");

            builder.HasIndex(e => e.EventType)
                .HasDatabaseName("IX_SIG_EventLogs_EventType");

            builder.HasIndex(e => e.DeliveryStatus)
                .HasDatabaseName("IX_SIG_EventLogs_DeliveryStatus");

            builder.HasIndex(e => e.SiraId)
                .HasDatabaseName("IX_SIG_EventLogs_SiraId")
                .HasFilter("[SiraId] IS NOT NULL");

            builder.HasIndex(e => e.BankoId)
                .HasDatabaseName("IX_SIG_EventLogs_BankoId")
                .HasFilter("[BankoId] IS NOT NULL");

            builder.HasIndex(e => e.TvId)
                .HasDatabaseName("IX_SIG_EventLogs_TvId")
                .HasFilter("[TvId] IS NOT NULL");

            builder.HasIndex(e => e.PersonelTc)
                .HasDatabaseName("IX_SIG_EventLogs_PersonelTc")
                .HasFilter("[PersonelTc] IS NOT NULL");

            builder.HasIndex(e => e.CorrelationId)
                .HasDatabaseName("IX_SIG_EventLogs_CorrelationId")
                .HasFilter("[CorrelationId] IS NOT NULL");

            // Composite index for common filtering
            builder.HasIndex(e => new { e.SentAt, e.EventType, e.DeliveryStatus })
                .HasDatabaseName("IX_SIG_EventLogs_SentAt_EventType_Status");
        }
    }
}
