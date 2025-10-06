using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.SiramatikIslemleri
{
    public class SiraConfiguration : IEntityTypeConfiguration<Sira>
    {
        public void Configure(EntityTypeBuilder<Sira> builder)
        {
            builder.ToTable("SIR_Siralar", "dbo", t => t.UseSqlOutputClause(false));

            builder.HasKey(s => s.SiraId);

            builder.Property(s => s.SiraId)
                .ValueGeneratedOnAdd();

            builder.Property(s => s.SiraNo)
                .IsRequired()
                .HasComment("Sıra numarası");

            builder.Property(s => s.SiraAlisZamani)
                .IsRequired()
                .HasComment("Sıra alış zamanı");

            builder.Property(s => s.BeklemeDurum)
                .HasConversion<int>()
                .IsRequired()
                .HasComment("Bekleme durumu");

            builder.Property(s => s.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(s => s.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(s => s.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            // Indexes
            builder.HasIndex(s => new { s.SiraNo, s.HizmetBinasiId, s.SiraAlisZamani })
                .IsUnique()
                .HasDatabaseName("IX_SIR_Siralar_SiraNo_HizmetBinasi_Zaman")
                .HasFilter("[SilindiMi] = 0");

            builder.HasIndex(s => s.BeklemeDurum)
                .HasDatabaseName("IX_SIR_Siralar_BeklemeDurum");

            builder.HasIndex(s => s.SiraAlisZamani)
                .HasDatabaseName("IX_SIR_Siralar_SiraAlisZamani");

            builder.HasIndex(s => new { s.KanalAltIslemId, s.BeklemeDurum })
                .HasDatabaseName("IX_SIR_Siralar_KanalAltIslem_BeklemeDurum");

            builder.HasQueryFilter(s => !s.SilindiMi);

            // Relationships (SGKDbContext'te zaten tanımlı)
        }
    }
}