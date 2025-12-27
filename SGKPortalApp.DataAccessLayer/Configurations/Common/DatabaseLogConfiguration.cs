using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

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

            // ═══════ BASIC FIELDS ═══════
            builder.Property(dl => dl.TcKimlikNo)
                .IsRequired()
                .HasMaxLength(11);

            builder.Property(dl => dl.TableName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(dl => dl.DatabaseAction)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(dl => dl.IslemZamani)
                .IsRequired();

            builder.Property(dl => dl.ActionTime)
                .IsRequired(false); // Obsolete field, nullable for backward compatibility

            // ═══════ HYBRID STORAGE ═══════
            builder.Property(dl => dl.StorageType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(LogStorageType.Database);

            builder.Property(dl => dl.FileReference)
                .HasMaxLength(500);

            builder.Property(dl => dl.BeforeData)
                .HasMaxLength(int.MaxValue); // NVARCHAR(MAX) for large JSON

            builder.Property(dl => dl.AfterData)
                .HasMaxLength(int.MaxValue); // NVARCHAR(MAX) for large JSON

            // ═══════ TRANSACTION GROUPING ═══════
            builder.Property(dl => dl.TransactionId)
                .HasColumnType("UNIQUEIDENTIFIER");

            builder.Property(dl => dl.IsGroupedLog)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(dl => dl.OperationType)
                .HasMaxLength(50);

            builder.Property(dl => dl.ChangeSummary)
                .HasMaxLength(1000);

            builder.Property(dl => dl.TotalChangeCount)
                .IsRequired(false);

            builder.Property(dl => dl.SaveChangesCount)
                .IsRequired(false);

            builder.Property(dl => dl.AffectedTables)
                .HasMaxLength(1000);

            // ═══════ DOMAIN USER ═══════
            builder.Property(dl => dl.DomainUser)
                .HasMaxLength(200);

            builder.Property(dl => dl.MachineName)
                .HasMaxLength(100);

            builder.Property(dl => dl.IsDomainUserMismatch)
                .IsRequired()
                .HasDefaultValue(false);

            // ═══════ NETWORK INFO ═══════
            builder.Property(dl => dl.IpAddress)
                .HasMaxLength(100);

            builder.Property(dl => dl.UserAgent)
                .HasMaxLength(500);

            // ═══════ METADATA ═══════
            builder.Property(dl => dl.ChangedFieldCount)
                .IsRequired(false);

            builder.Property(dl => dl.DataSizeBytes)
                .IsRequired(false);

            builder.Property(dl => dl.BulkOperationCount)
                .IsRequired(false);

            // ═══════ BASE ENTITY PROPERTIES ═══════
            builder.Property(dl => dl.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            // ═══════ INDEXES (Performance Optimization) ═══════

            // 1. Kullanıcı bazlı sorgular (en sık kullanılan)
            builder.HasIndex(dl => new { dl.TcKimlikNo, dl.IslemZamani })
                .HasDatabaseName("IX_CMN_DatabaseLogs_TcKimlikNo_IslemZamani")
                .IsDescending(false, true); // TcKimlikNo ASC, IslemZamani DESC

            // 2. Tablo bazlı sorgular
            builder.HasIndex(dl => new { dl.TableName, dl.IslemZamani })
                .HasDatabaseName("IX_CMN_DatabaseLogs_TableName_IslemZamani")
                .IsDescending(false, true);

            // 3. Transaction gruplandırma
            builder.HasIndex(dl => dl.TransactionId)
                .HasDatabaseName("IX_CMN_DatabaseLogs_TransactionId")
                .HasFilter("[TransactionId] IS NOT NULL"); // Partial index

            // 4. Grouped log sorguları
            builder.HasIndex(dl => new { dl.IsGroupedLog, dl.IslemZamani })
                .HasDatabaseName("IX_CMN_DatabaseLogs_IsGroupedLog_IslemZamani")
                .HasFilter("[IsGroupedLog] = 1") // Sadece grouped loglar
                .IsDescending(false, true);

            // 5. Domain user uyumsuzluk sorguları (güvenlik)
            builder.HasIndex(dl => new { dl.IsDomainUserMismatch, dl.IslemZamani })
                .HasDatabaseName("IX_CMN_DatabaseLogs_IsDomainUserMismatch_IslemZamani")
                .HasFilter("[IsDomainUserMismatch] = 1") // Sadece uyumsuz kayıtlar
                .IsDescending(false, true);

            // 6. Domain user bazlı sorgular
            builder.HasIndex(dl => new { dl.DomainUser, dl.IslemZamani })
                .HasDatabaseName("IX_CMN_DatabaseLogs_DomainUser_IslemZamani")
                .HasFilter("[DomainUser] IS NOT NULL")
                .IsDescending(false, true);

            // 7. İşlem tipi bazlı sorgular
            builder.HasIndex(dl => new { dl.DatabaseAction, dl.IslemZamani })
                .HasDatabaseName("IX_CMN_DatabaseLogs_DatabaseAction_IslemZamani")
                .IsDescending(false, true);

            // 8. Machine name bazlı sorgular (güvenlik analizi)
            builder.HasIndex(dl => new { dl.MachineName, dl.IslemZamani })
                .HasDatabaseName("IX_CMN_DatabaseLogs_MachineName_IslemZamani")
                .HasFilter("[MachineName] IS NOT NULL")
                .IsDescending(false, true);

            // 9. IP address bazlı sorgular
            builder.HasIndex(dl => new { dl.IpAddress, dl.IslemZamani })
                .HasDatabaseName("IX_CMN_DatabaseLogs_IpAddress_IslemZamani")
                .HasFilter("[IpAddress] IS NOT NULL")
                .IsDescending(false, true);

            // 10. Dosya referansı lookup (detay görüntüleme)
            builder.HasIndex(dl => dl.FileReference)
                .HasDatabaseName("IX_CMN_DatabaseLogs_FileReference")
                .HasFilter("[FileReference] IS NOT NULL");

            // 11. Tarih bazlı sorgular (retention policy için)
            builder.HasIndex(dl => dl.IslemZamani)
                .HasDatabaseName("IX_CMN_DatabaseLogs_IslemZamani")
                .IsDescending(true); // Yeni kayıtlar önce

            // 12. Zaman bazlı temizleme (EklenmeTarihi - mevcut index korundu)
            builder.HasIndex(dl => dl.EklenmeTarihi)
                .HasDatabaseName("IX_CMN_DatabaseLogs_EklenmeTarihi");

            // ═══════ NO SOFT DELETE FOR LOG TABLE ═══════
            // Log tablosu için soft delete yok, gerçek silme yapılır
        }
    }
}
