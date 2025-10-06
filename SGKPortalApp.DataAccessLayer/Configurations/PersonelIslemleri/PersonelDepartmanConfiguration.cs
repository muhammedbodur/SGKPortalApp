using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.PersonelIslemleri
{
    public class PersonelDepartmanConfiguration : IEntityTypeConfiguration<PersonelDepartman>
    {
        public void Configure(EntityTypeBuilder<PersonelDepartman> builder)
        {
            builder.ToTable("PER_PersonelDepartmanlar", "dbo");

            builder.HasKey(pd => pd.PersonelDepartmanId);

            builder.Property(pd => pd.PersonelDepartmanId)
                .ValueGeneratedOnAdd();

            builder.Property(pd => pd.TcKimlikNo)
                .IsRequired()
                .HasMaxLength(11);

            builder.Property(pd => pd.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(pd => pd.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(pd => pd.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(pd => new { pd.TcKimlikNo, pd.DepartmanId })
                .HasDatabaseName("IX_PER_PersonelDepartmanlar_Tc_Departman");

            builder.HasQueryFilter(pd => !pd.SilindiMi);

            // Relationships - Junction Table
            builder.HasOne(pd => pd.Personel)
                .WithMany()
                .HasForeignKey(pd => pd.TcKimlikNo)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_PER_PersonelDepartmanlar_PER_Personeller");

            builder.HasOne(pd => pd.Departman)
                .WithMany()
                .HasForeignKey(pd => pd.DepartmanId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_PER_PersonelDepartmanlar_PER_Departmanlar");
        }
    }
}