using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.SiramatikIslemleri
{
    public class BankoConfiguration : IEntityTypeConfiguration<Banko>
    {
        public void Configure(EntityTypeBuilder<Banko> builder)
        {
            builder.ToTable("SIR_Bankolar", "dbo");

            builder.HasKey(b => b.BankoId);

            builder.Property(b => b.BankoId)
                .ValueGeneratedOnAdd();

            builder.Property(b => b.BankoNo)
                .IsRequired()
                .HasComment("Banko numarası");

            builder.Property(b => b.BankoTipi)
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasComment("Banko tipi (Normal/Oncelikli/vb)");

            builder.Property(b => b.KatTipi)
                .HasConversion<int>()
                .HasComment("Kat bilgisi");

            builder.Property(b => b.BankoAktiflik)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(Aktiflik.Aktif)
                .HasComment("Banko aktiflik durumu");

            builder.Property(b => b.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(b => b.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(b => b.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            // Indexes
            builder.HasIndex(b => new { b.HizmetBinasiId, b.BankoNo })
                .IsUnique()
                .HasDatabaseName("IX_SIR_Bankolar_HizmetBinasi_BankoNo")
                .HasFilter("[SilindiMi] = 0");

            builder.HasIndex(b => b.BankoAktiflik)
                .HasDatabaseName("IX_SIR_Bankolar_BankoAktiflik");

            builder.HasQueryFilter(b => !b.SilindiMi);

            // Relationships
            builder.HasOne(b => b.HizmetBinasi)
                .WithMany()
                .HasForeignKey(b => b.HizmetBinasiId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_Bankolar_CMN_HizmetBinalari");
        }
    }
}