using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.Common
{
    public class ModulConfiguration : IEntityTypeConfiguration<Modul>
    {
        public void Configure(EntityTypeBuilder<Modul> builder)
        {
            builder.ToTable("PER_Moduller", "dbo");

            builder.HasKey(m => m.ModulId);

            builder.Property(m => m.ModulId)
                .ValueGeneratedOnAdd();

            builder.Property(m => m.ModulAdi)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(m => m.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(m => m.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(m => m.ModulAdi)
                .IsUnique()
                .HasDatabaseName("IX_PER_Moduller_ModulAdi")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(m => !m.SilindiMi);

            builder.HasMany(m => m.ModulControllers)
                .WithOne(mc => mc.Modul)
                .HasForeignKey(mc => mc.ModulId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_PER_ModulControllers_PER_Moduller");
        }
    }
}