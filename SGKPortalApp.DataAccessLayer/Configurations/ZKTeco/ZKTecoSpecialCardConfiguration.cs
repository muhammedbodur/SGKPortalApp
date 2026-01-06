using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;

namespace SGKPortalApp.DataAccessLayer.Configurations.ZKTeco
{
    public class ZKTecoSpecialCardConfiguration : IEntityTypeConfiguration<ZKTecoSpecialCard>
    {
        public void Configure(EntityTypeBuilder<ZKTecoSpecialCard> builder)
        {
            // Table
            builder.ToTable("ZKTeco_SpecialCard");

            // Primary Key
            builder.HasKey(c => c.Id);

            // Properties
            builder.Property(c => c.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(c => c.CardType)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(c => c.CardNumber)
                   .IsRequired();

            builder.Property(c => c.CardName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(c => c.EnrollNumber)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(c => c.IsActive)
                   .HasDefaultValue(true);

            builder.Property(c => c.TemporaryUserName)
                   .HasMaxLength(100);

            builder.Property(c => c.Notes)
                   .HasMaxLength(500);

            builder.Property(c => c.CreatedByTcKimlikNo)
                   .HasMaxLength(11);

            // Relationships
            builder.HasOne(c => c.CurrentUser)
                   .WithMany()
                   .HasForeignKey(c => c.CurrentUserSicilNo)
                   .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(c => c.CardNumber)
                   .IsUnique()
                   .HasDatabaseName("IX_ZKTecoSpecialCard_CardNumber");

            builder.HasIndex(c => c.EnrollNumber)
                   .IsUnique()
                   .HasDatabaseName("IX_ZKTecoSpecialCard_EnrollNumber");

            builder.HasIndex(c => c.CardType)
                   .HasDatabaseName("IX_ZKTecoSpecialCard_CardType");

            builder.HasIndex(c => c.IsActive)
                   .HasDatabaseName("IX_ZKTecoSpecialCard_IsActive");

            builder.HasIndex(c => c.CurrentUserSicilNo)
                   .HasDatabaseName("IX_ZKTecoSpecialCard_CurrentUserSicilNo");
        }
    }
}
