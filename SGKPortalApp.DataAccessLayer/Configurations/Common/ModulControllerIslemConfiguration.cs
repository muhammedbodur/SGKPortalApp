using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.Common
{
    public class ModulControllerIslemConfiguration : IEntityTypeConfiguration<ModulControllerIslem>
    {
        public void Configure(EntityTypeBuilder<ModulControllerIslem> builder)
        {
            builder.ToTable("PER_ModulControllerIslemleri", "dbo");

            builder.HasKey(mci => mci.ModulControllerIslemId);

            builder.Property(mci => mci.ModulControllerIslemId)
                .ValueGeneratedOnAdd();

            builder.Property(mci => mci.ModulControllerIslemAdi)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(mci => mci.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(mci => mci.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(mci => mci.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(mci => new { mci.ModulControllerId, mci.ModulControllerIslemAdi })
                .IsUnique()
                .HasDatabaseName("IX_PER_ModulControllerIslemleri_Controller_Islem")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(mci => !mci.SilindiMi);
        }
    }
}