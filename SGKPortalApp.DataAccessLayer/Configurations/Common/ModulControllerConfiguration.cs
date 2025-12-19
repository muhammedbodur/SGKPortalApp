using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.Common
{
    public class ModulControllerConfiguration : IEntityTypeConfiguration<ModulController>
    {
        public void Configure(EntityTypeBuilder<ModulController> builder)
        {
            builder.ToTable("PER_ModulControllers", "dbo");

            builder.HasKey(mc => mc.ModulControllerId);

            builder.Property(mc => mc.ModulControllerId)
                .ValueGeneratedOnAdd();

            builder.Property(mc => mc.ModulControllerAdi)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(mc => mc.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(mc => mc.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(mc => mc.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            // Hiyerarşik yapı için UstModulControllerId
            builder.Property(mc => mc.UstModulControllerId)
                .IsRequired(false);

            builder.HasIndex(mc => new { mc.ModulId, mc.ModulControllerAdi })
                .IsUnique()
                .HasDatabaseName("IX_PER_ModulControllers_Modul_Controller")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(mc => !mc.SilindiMi);

            // Self-referencing relationship (Parent-Child)
            builder.HasOne(mc => mc.UstModulController)
                .WithMany(mc => mc.AltModulControllers)
                .HasForeignKey(mc => mc.UstModulControllerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_PER_ModulControllers_UstController");

            builder.HasMany(mc => mc.ModulControllerIslemler)
                .WithOne(mci => mci.ModulController)
                .HasForeignKey(mci => mci.ModulControllerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_PER_ModulControllerIslemleri_PER_ModulControllers");
        }
    }
}