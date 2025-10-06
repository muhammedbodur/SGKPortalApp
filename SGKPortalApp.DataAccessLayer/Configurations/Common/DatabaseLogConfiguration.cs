using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.Common
{
    public class DatabaseLogConfiguration : IEntityTypeConfiguration<DatabaseLog>
    {
        public void Configure(EntityTypeBuilder<DatabaseLog> builder)
        {
            builder.ToTable("CMN_DatabaseLogs", "dbo");

            builder.HasKey(dl => dl.DatabaseLogId);

            builder.Property(dl => dl.DatabaseLogId)
                .ValueGeneratedOnAdd();

            builder.Property(dl => dl.TableName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(dl => dl.DatabaseAction)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(dl => dl.BeforeData)
                .HasMaxLength(4000);

            builder.Property(dl => dl.AfterData)
                .HasMaxLength(4000);

            builder.Property(dl => dl.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.HasIndex(dl => dl.TableName)
                .HasDatabaseName("IX_CMN_DatabaseLogs_TableName");

            builder.HasIndex(dl => dl.EklenmeTarihi)
                .HasDatabaseName("IX_CMN_DatabaseLogs_EklenmeTarihi");

            // Log tablosu için soft delete yok
        }
    }
}