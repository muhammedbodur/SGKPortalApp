using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;

namespace SGKPortalApp.DataAccessLayer.Configurations.ZKTeco
{
    public class SpecialCardConfiguration : IEntityTypeConfiguration<SpecialCard>
    {
        private const string TablePrefix = "ZKTeco_";
        private const string IndexPrefix = "IX_ZKTeco_";

        public void Configure(EntityTypeBuilder<SpecialCard> builder)
        {
            // Table
            builder.ToTable($"{TablePrefix}SpecialCard");

            // Primary Key (Id BaseEntity'den geliyor)
            builder.HasKey(c => c.Id);

            // Properties
            builder.Property(c => c.CardType)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.Property(c => c.CardNumber)
                   .IsRequired();

            builder.Property(c => c.CardName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(c => c.EnrollNumber)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(c => c.Notes)
                   .HasMaxLength(500);

            // Indexes
            builder.HasIndex(c => c.CardNumber)
                   .IsUnique()
                   .HasDatabaseName($"{IndexPrefix}SpecialCard_CardNumber");

            builder.HasIndex(c => c.EnrollNumber)
                   .IsUnique()
                   .HasDatabaseName($"{IndexPrefix}SpecialCard_EnrollNumber");

            builder.HasIndex(c => c.CardType)
                   .HasDatabaseName($"{IndexPrefix}SpecialCard_CardType");
        }
    }
}
