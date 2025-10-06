using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.SiramatikIslemleri
{
    public class TvBankoConfiguration : IEntityTypeConfiguration<TvBanko>
    {
        public void Configure(EntityTypeBuilder<TvBanko> builder)
        {
            builder.ToTable("SIR_TvBankolari", "dbo");

            builder.HasKey(tb => tb.TvBankoId);

            builder.Property(tb => tb.TvBankoId)
                .ValueGeneratedOnAdd();

            builder.Property(tb => tb.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(tb => tb.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(tb => tb.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(tb => new { tb.TvId, tb.BankoId })
                .IsUnique()
                .HasDatabaseName("IX_SIR_TvBankolari_Tv_Banko")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(tb => !tb.SilindiMi);

            builder.HasOne(tb => tb.Tv)
                .WithMany(t => t.TvBankolar)
                .HasForeignKey(tb => tb.TvId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_TvBankolari_SIR_Tvler");

            builder.HasOne(tb => tb.Banko)
                .WithMany(b => b.TvBankolar)
                .HasForeignKey(tb => tb.BankoId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_TvBankolari_SIR_Bankolar");
        }
    }
}