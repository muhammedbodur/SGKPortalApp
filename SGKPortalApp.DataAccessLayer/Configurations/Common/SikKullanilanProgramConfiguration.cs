using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.Common
{
    public class SikKullanilanProgramConfiguration : IEntityTypeConfiguration<SikKullanilanProgram>
    {
        public void Configure(EntityTypeBuilder<SikKullanilanProgram> builder)
        {
            // Table
            builder.ToTable("CMN_SikKullanilanProgramlar", "dbo");

            // Primary Key
            builder.HasKey(s => s.ProgramId);

            // Properties
            builder.Property(s => s.ProgramId)
                .ValueGeneratedOnAdd();

            builder.Property(s => s.ProgramAdi)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("Program adı");

            builder.Property(s => s.Url)
                .IsRequired()
                .HasMaxLength(500)
                .HasComment("Program URL");

            builder.Property(s => s.IkonClass)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("bx-desktop")
                .HasComment("İkon CSS class");

            builder.Property(s => s.RenkKodu)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("primary")
                .HasComment("Renk kodu");

            builder.Property(s => s.Sira)
                .IsRequired()
                .HasComment("Program sırası");

            builder.Property(s => s.Aktiflik)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(Aktiflik.Aktif)
                .HasComment("Program aktiflik durumu (0: Pasif, 1: Aktif)");

            // Auditing
            builder.Property(s => s.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Kayıt oluşturulma tarihi");

            builder.Property(s => s.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Son güncelleme tarihi");

            builder.Property(s => s.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Soft delete flag");

            // Indexes
            builder.HasIndex(s => s.Sira)
                .HasDatabaseName("IX_CMN_SikKullanilanProgramlar_Sira");

            builder.HasIndex(s => s.Aktiflik)
                .HasDatabaseName("IX_CMN_SikKullanilanProgramlar_Aktiflik");

            builder.HasIndex(s => new { s.Aktiflik, s.SilindiMi })
                .HasDatabaseName("IX_CMN_SikKullanilanProgramlar_Aktiflik_SilindiMi");

            // Soft Delete Query Filter
            builder.HasQueryFilter(s => !s.SilindiMi);
        }
    }
}
