using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.Common
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("CMN_Users", "dbo");

            builder.HasKey(u => u.TcKimlikNo);

            builder.Property(u => u.TcKimlikNo)
                .IsRequired()
                .HasMaxLength(11)
                .HasComment("TC Kimlik Numarası - Primary Key & Foreign Key to Personel or TV User ID");

            builder.Property(u => u.UserType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(BusinessObjectLayer.Enums.Common.UserType.Personel)
                .HasComment("Kullanıcı tipi: Personel veya TvUser");

            builder.Property(u => u.PassWord)
                .IsRequired()
                .HasMaxLength(255)
                .HasComment("Şifre (hash'lenmiş)");

            builder.Property(u => u.SessionID)
                .HasMaxLength(100)
                .HasComment("Aktif oturum ID");

            builder.Property(u => u.AktifMi)
                .IsRequired()
                .HasDefaultValue(true)
                .HasComment("Kullanıcı aktif mi?");

            builder.Property(u => u.SonGirisTarihi)
                .HasComment("Son giriş tarihi");

            builder.Property(u => u.BasarisizGirisSayisi)
                .IsRequired()
                .HasDefaultValue(0)
                .HasComment("Başarısız giriş denemesi sayısı");

            builder.Property(u => u.HesapKilitTarihi)
                .HasComment("Hesap kilitlenme tarihi");

            // Banko Modu Bilgileri
            builder.Property(u => u.BankoModuAktif)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Kullanıcı banko modunda mı? (true ise sadece sıra çağırma kullanılabilir)");

            builder.Property(u => u.AktifBankoId)
                .HasComment("Aktif banko ID (banko modundaysa)");

            builder.Property(u => u.BankoModuBaslangic)
                .HasComment("Banko moduna geçiş zamanı");

            builder.Property(u => u.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(u => u.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(u => u.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            // Personel ile One-to-One ilişki (Opsiyonel - Sadece Personel tipinde)
            // Personel.TcKimlikNo → User.TcKimlikNo (FK)
            // UserType = Personel ise → Personel kaydı ZORUNLU
            // UserType = TvUser ise → Personel kaydı YOK
            // NOT: Personel eklenirken User zaten var olmalı (FK constraint)
            //      User eklenirken Personel olması gerekmez (navigation property nullable)
            builder.HasOne(u => u.Personel)
                .WithOne(p => p.User)
                .HasForeignKey<Personel>(p => p.TcKimlikNo)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false); // User için Personel navigation property opsiyonel

            // TV ile One-to-One ilişki (Opsiyonel - Sadece TvUser tipinde)
            // Tv.TcKimlikNo → User.TcKimlikNo (FK, nullable)
            // UserType = TvUser ise → Tv kaydı ZORUNLU
            // UserType = Personel ise → Tv kaydı YOK
            // NOT: Tv eklenirken User zaten var olmalı (FK constraint)
            //      User eklenirken Tv olması gerekmez (navigation property nullable)
            builder.HasOne(u => u.Tv)
                .WithOne(t => t.User)
                .HasForeignKey<BusinessObjectLayer.Entities.SiramatikIslemleri.Tv>(t => t.TcKimlikNo)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false) // User için Tv navigation property opsiyonel
                .HasConstraintName("FK_CMN_Users_SIR_Tvler");

            // HubConnection ile One-to-Many ilişki
            // Bir kullanıcının birden fazla HubConnection'ı olabilir (farklı tab'lar)
            // HubConnection.TcKimlikNo → User.TcKimlikNo (FK, required)
            // NOT: HubConnection eklenirken User ZORUNLU (TcKimlikNo required)
            //      User eklenirken HubConnection gerekmez (collection boş olabilir)
            builder.HasMany(u => u.HubConnections)
                .WithOne(h => h.User)
                .HasForeignKey(h => h.TcKimlikNo)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false) // User için HubConnection navigation property opsiyonel
                .HasConstraintName("FK_CMN_HubConnections_CMN_Users");

            // Indexes
            builder.HasIndex(u => u.TcKimlikNo)
                .IsUnique()
                .HasDatabaseName("IX_CMN_Users_TcKimlikNo")
                .HasFilter("[SilindiMi] = 0");

            builder.HasIndex(u => u.UserType)
                .HasDatabaseName("IX_CMN_Users_UserType")
                .HasFilter("[SilindiMi] = 0");

            builder.HasIndex(u => u.SessionID)
                .HasDatabaseName("IX_CMN_Users_SessionID")
                .HasFilter("[SessionID] IS NOT NULL AND [SilindiMi] = 0");

            builder.HasQueryFilter(u => !u.SilindiMi);
        }
    }
}