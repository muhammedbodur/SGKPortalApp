using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;

namespace SGKPortalApp.DataAccessLayer.Configurations.ZKTeco
{
    public class SpecialCardConfiguration : IEntityTypeConfiguration<SpecialCard>
    {
        private const string TablePrefix = "ZKTeco_";

        public void Configure(EntityTypeBuilder<SpecialCard> builder)
        {
            // Table
            builder.ToTable($"{TablePrefix}SpecialCard");

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
                   .HasDatabaseName($"IX_{TablePrefix}SpecialCard_CardNumber");

            builder.HasIndex(c => c.EnrollNumber)
                   .IsUnique()
                   .HasDatabaseName($"IX_{TablePrefix}SpecialCard_EnrollNumber");

            builder.HasIndex(c => c.CardType)
                   .HasDatabaseName($"IX_{TablePrefix}SpecialCard_CardType");

            builder.HasIndex(c => c.IsActive)
                   .HasDatabaseName($"IX_{TablePrefix}SpecialCard_IsActive");

            builder.HasIndex(c => c.CurrentUserSicilNo)
                   .HasDatabaseName($"IX_{TablePrefix}SpecialCard_CurrentUserSicilNo");
        }
    }
}
