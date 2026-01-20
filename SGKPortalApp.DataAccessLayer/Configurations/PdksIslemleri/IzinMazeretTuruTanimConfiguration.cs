using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.PdksIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.PdksIslemleri
{
    /// <summary>
    /// IzinMazeretTuruTanim entity EF Core yapılandırması
    /// </summary>
    public class IzinMazeretTuruTanimConfiguration : IEntityTypeConfiguration<IzinMazeretTuruTanim>
    {
        public void Configure(EntityTypeBuilder<IzinMazeretTuruTanim> builder)
        {
            builder.ToTable("PDKS_IzinMazeretTuruTanim", "dbo");

            builder.HasKey(t => t.IzinMazeretTuruId);

            builder.Property(t => t.IzinMazeretTuruId)
                .ValueGeneratedOnAdd();

            builder.Property(t => t.TuruAdi)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.KisaKod)
                .HasMaxLength(50);

            builder.Property(t => t.Aciklama)
                .HasMaxLength(500);

            builder.Property(t => t.BirinciOnayciGerekli)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(t => t.IkinciOnayciGerekli)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(t => t.PlanliIzinMi)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(t => t.Sira)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(t => t.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(t => t.RenkKodu)
                .HasMaxLength(20);

            builder.Property(t => t.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(t => t.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(t => t.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            // İndeksler
            builder.HasIndex(t => t.TuruAdi)
                .HasDatabaseName("IX_PDKS_IzinMazeretTuruTanim_TuruAdi");

            builder.HasIndex(t => t.KisaKod)
                .HasDatabaseName("IX_PDKS_IzinMazeretTuruTanim_KisaKod");

            builder.HasIndex(t => t.IsActive)
                .HasDatabaseName("IX_PDKS_IzinMazeretTuruTanim_IsActive");

            builder.HasIndex(t => t.Sira)
                .HasDatabaseName("IX_PDKS_IzinMazeretTuruTanim_Sira");

            // Query Filter (Soft Delete)
            builder.HasQueryFilter(t => !t.SilindiMi);
        }
    }
}
