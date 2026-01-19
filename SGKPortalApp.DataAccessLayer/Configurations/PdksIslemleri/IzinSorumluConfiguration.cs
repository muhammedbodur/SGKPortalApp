using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.PdksIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.PdksIslemleri
{
    public class IzinSorumluConfiguration : IEntityTypeConfiguration<IzinSorumlu>
    {
        public void Configure(EntityTypeBuilder<IzinSorumlu> builder)
        {
            // Table
            builder.ToTable("PDKS_IzinSorumlu", "dbo");

            // Primary Key
            builder.HasKey(s => s.IzinSorumluId);

            // Properties
            builder.Property(s => s.IzinSorumluId)
                .ValueGeneratedOnAdd();

            builder.Property(s => s.DepartmanId)
                .IsRequired(false)
                .HasComment("Sorumlu olunan departman (Null ise tüm departmanlar için geçerli)");

            builder.Property(s => s.ServisId)
                .IsRequired(false)
                .HasComment("Sorumlu olunan servis (Null ise tüm servisler için geçerli)");

            builder.Property(s => s.SorumluPersonelTcKimlikNo)
                .IsRequired()
                .HasMaxLength(11)
                .HasComment("Sorumlu personelin TC Kimlik No");

            builder.Property(s => s.OnaySeviyesi)
                .IsRequired()
                .HasDefaultValue(1)
                .HasComment("Onay seviyesi (1: Birinci Onayci, 2: İkinci Onayci)");

            builder.Property(s => s.Aktif)
                .IsRequired()
                .HasDefaultValue(true)
                .HasComment("Sorumluluk aktif mi?");

            builder.Property(s => s.Aciklama)
                .HasMaxLength(500)
                .HasComment("Açıklama/Not");

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
            builder.HasIndex(s => s.DepartmanId)
                .HasDatabaseName("IX_PDKS_IzinSorumlu_DepartmanId");

            builder.HasIndex(s => s.ServisId)
                .HasDatabaseName("IX_PDKS_IzinSorumlu_ServisId");

            builder.HasIndex(s => s.SorumluPersonelTcKimlikNo)
                .HasDatabaseName("IX_PDKS_IzinSorumlu_SorumluPersonelTcKimlikNo");

            builder.HasIndex(s => s.Aktif)
                .HasDatabaseName("IX_PDKS_IzinSorumlu_Aktif");

            builder.HasIndex(s => new { s.Aktif, s.SilindiMi })
                .HasDatabaseName("IX_PDKS_IzinSorumlu_Aktif_SilindiMi");

            builder.HasIndex(s => new { s.DepartmanId, s.ServisId, s.OnaySeviyesi })
                .HasDatabaseName("IX_PDKS_IzinSorumlu_Kapsam_OnaySeviyesi");

            // Soft Delete Query Filter
            builder.HasQueryFilter(s => !s.SilindiMi);

            // Relationships
            builder.HasOne(s => s.Departman)
                .WithMany()
                .HasForeignKey(s => s.DepartmanId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_PDKS_IzinSorumlu_PER_Departmanlar");

            builder.HasOne(s => s.Servis)
                .WithMany()
                .HasForeignKey(s => s.ServisId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_PDKS_IzinSorumlu_PER_Servisler");

            builder.HasOne(s => s.SorumluPersonel)
                .WithMany(p => p.IzinSorumluluklar)
                .HasForeignKey(s => s.SorumluPersonelTcKimlikNo)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_PDKS_IzinSorumlu_PER_Personeller");
        }
    }
}
