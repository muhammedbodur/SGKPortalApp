using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.SiramatikIslemleri
{
    public class BankoKullaniciConfiguration : IEntityTypeConfiguration<BankoKullanici>
    {
        public void Configure(EntityTypeBuilder<BankoKullanici> builder)
        {
            builder.ToTable("SIR_BankoKullanicilari", "dbo");

            builder.HasKey(bk => bk.BankoKullaniciId);

            builder.Property(bk => bk.BankoKullaniciId)
                .ValueGeneratedOnAdd();

            builder.Property(bk => bk.TcKimlikNo)
                .IsRequired()
                .HasMaxLength(11);

            builder.Property(bk => bk.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(bk => bk.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(bk => bk.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(bk => bk.HizmetBinasiId)
                .IsRequired();

            // UNIQUE: Bir banko sadece bir personele atanabilir (1-to-1)
            builder.HasIndex(bk => bk.BankoId)
                .IsUnique()
                .HasDatabaseName("IX_SIR_BankoKullanicilari_BankoId")
                .HasFilter("[SilindiMi] = 0");

            // UNIQUE: Bir personel sadece bir hizmet binasında bir bankoya atanabilir (1-to-1)
            builder.HasIndex(bk => new { bk.TcKimlikNo, bk.HizmetBinasiId })
                .IsUnique()
                .HasDatabaseName("IX_SIR_BankoKullanicilari_TcKimlik_HizmetBinasi")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(bk => !bk.SilindiMi);

            builder.HasOne(bk => bk.Personel)
                .WithMany()
                .HasForeignKey(bk => bk.TcKimlikNo)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_BankoKullanicilari_PER_Personeller");

            builder.HasOne(bk => bk.Banko)
                .WithMany(b => b.BankoKullanicilari)
                .HasForeignKey(bk => bk.BankoId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_BankoKullanicilari_SIR_Bankolar");

            builder.HasOne(bk => bk.HizmetBinasi)
                .WithMany()
                .HasForeignKey(bk => bk.HizmetBinasiId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_BankoKullanicilari_CMN_HizmetBinalari");
        }
    }
}