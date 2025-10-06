using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.Common
{
    public class IlConfiguration : IEntityTypeConfiguration<Il>
    {
        public void Configure(EntityTypeBuilder<Il> builder)
        {
            builder.ToTable("CMN_Iller", "dbo");

            builder.HasKey(i => i.IlId);

            builder.Property(i => i.IlId)
                .ValueGeneratedOnAdd();

            builder.Property(i => i.IlAdi)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(i => i.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(i => i.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(i => i.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(i => i.IlAdi)
                .IsUnique()
                .HasDatabaseName("IX_CMN_Iller_IlAdi")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(i => !i.SilindiMi);

            builder.HasMany(i => i.Ilceler)
                .WithOne(ic => ic.Il)
                .HasForeignKey(ic => ic.IlId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_CMN_Ilceler_CMN_Iller");
        }
    }
}