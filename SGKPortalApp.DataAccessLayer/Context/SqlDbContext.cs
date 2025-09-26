using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;

namespace SGKPortalApp.DataAccessLayer.Context
{
    public class SGKDbContext : DbContext
    {
        public SGKDbContext(DbContextOptions<SGKDbContext> options) : base(options)
        {
        }

        #region Common Tables
        public DbSet<User> Users { get; set; }
        public DbSet<Il> Iller { get; set; }
        public DbSet<Ilce> Ilceler { get; set; }
        public DbSet<DatabaseLog> DatabaseLogs { get; set; }
        public DbSet<LoginLogoutLog> LoginLogoutLogs { get; set; }
        public DbSet<HizmetBinasi> HizmetBinalari { get; set; }
        #endregion

        #region Personel İşlemleri
        public DbSet<Personel> Personeller { get; set; }
        public DbSet<PersonelCocuk> PersonelCocuklari { get; set; }
        public DbSet<PersonelDepartman> PersonelDepartmanlar { get; set; }
        public DbSet<PersonelYetki> PersonelYetkileri { get; set; }
        public DbSet<Departman> Departmanlar { get; set; }
        public DbSet<Servis> Servisler { get; set; }
        public DbSet<Unvan> Unvanlar { get; set; }
        public DbSet<Sendika> Sendikalar { get; set; }
        public DbSet<AtanmaNedenleri> AtanmaNedenleri { get; set; }
        public DbSet<Yetki> Yetkiler { get; set; }
        public DbSet<Modul> Moduller { get; set; }
        public DbSet<ModulController> ModulControllers { get; set; }
        public DbSet<ModulControllerIslem> ModulControllerIslemleri { get; set; }
        #endregion

        #region Sıramatik İşlemleri
        public DbSet<Banko> Bankolar { get; set; }
        public DbSet<BankoKullanici> BankoKullanicilari { get; set; }
        public DbSet<BankoIslem> BankoIslemleri { get; set; }
        public DbSet<Kanal> Kanallar { get; set; }
        public DbSet<KanalAlt> KanallarAlt { get; set; }
        public DbSet<KanalIslem> KanalIslemleri { get; set; }
        public DbSet<KanalAltIslem> KanalAltIslemleri { get; set; }
        public DbSet<KanalPersonel> KanalPersonelleri { get; set; }
        public DbSet<KioskGrup> KioskGruplari { get; set; }
        public DbSet<KioskIslemGrup> KioskIslemGruplari { get; set; }
        public DbSet<Tv> Tvler { get; set; }
        public DbSet<TvBanko> TvBankolari { get; set; }
        public DbSet<Sira> Siralar { get; set; }
        public DbSet<HubConnection> HubConnections { get; set; }
        public DbSet<HubTvConnection> HubTvConnections { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureTableNaming(modelBuilder);
            ConfigureRelationships(modelBuilder);
            ConfigurePerformanceIndexes(modelBuilder);
            ConfigureEnumConversions(modelBuilder);
            ConfigureBusinessRules(modelBuilder);
            ConfigureAuditingRules(modelBuilder);
        }

        #region Table Naming Configuration
        private void ConfigureTableNaming(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var entityName = entity.ClrType.Name;
                var modulePrefix = GetModulePrefix(entity.ClrType);
                var tableName = $"{modulePrefix}_{GetPluralName(entityName)}";
                modelBuilder.Entity(entity.ClrType).ToTable(tableName);
            }
        }

        private string GetModulePrefix(Type entityType)
        {
            var namespaceName = entityType.Namespace ?? string.Empty;

            if (namespaceName.Contains("PersonelIslemleri"))
                return "PER";
            else if (namespaceName.Contains("SiramatikIslemleri"))
                return "SM";
            else if (namespaceName.Contains("PdksIslemleri"))
                return "PDKS";
            else if (namespaceName.Contains("EshotIslemleri"))
                return "ESHOT";
            else if (namespaceName.Contains("Common") ||
                     entityType.Name.Contains("User") ||
                     entityType.Name.Contains("Log") ||
                     entityType.Name.Contains("Il"))
                return "GNL";
            else
                return "GEN";
        }

        private string GetPluralName(string entityName)
        {
            var turkishPluralRules = new Dictionary<string, string>
            {
                { "Personel", "Personeller" },
                { "PersonelCocuk", "PersonelCocuklari" },
                { "PersonelDepartman", "PersonelDepartmanlar" },
                { "PersonelYetki", "PersonelYetkileri" },
                { "Departman", "Departmanlar" },
                { "Unvan", "Unvanlar" },
                { "Servis", "Servisler" },
                { "Sendika", "Sendikalar" },
                { "AtanmaNedenleri", "AtanmaNedenleri" },
                { "Banko", "Bankolar" },
                { "BankoKullanici", "BankoKullanicilari" },
                { "BankoIslem", "BankoIslemleri" },
                { "Kanal", "Kanallar" },
                { "KanalAlt", "KanallarAlt" },
                { "KanalIslem", "KanalIslemleri" },
                { "KanalAltIslem", "KanalAltIslemleri" },
                { "KanalPersonel", "KanalPersonelleri" },
                { "KioskGrup", "KioskGruplari" },
                { "KioskIslemGrup", "KioskIslemGruplari" },
                { "Tv", "Tvler" },
                { "TvBanko", "TvBankolari" },
                { "Il", "Iller" },
                { "Ilce", "Ilceler" },
                { "Sira", "Siralar" },
                { "User", "Users" },
                { "DatabaseLog", "DatabaseLogs" },
                { "LoginLogoutLog", "LoginLogoutLogs" },
                { "HizmetBinasi", "HizmetBinalari" },
                { "HubConnection", "HubConnections" },
                { "HubTvConnection", "HubTvConnections" },
                { "Yetki", "Yetkiler" },
                { "Modul", "Moduller" },
                { "ModulController", "ModulControllers" },
                { "ModulControllerIslem", "ModulControllerIslemleri" }
            };

            return turkishPluralRules.ContainsKey(entityName)
                ? turkishPluralRules[entityName]
                : entityName + "lar";
        }
        #endregion

        #region Relationships Configuration
        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            ConfigurePersonelRelationships(modelBuilder);
            ConfigureSiramatikRelationships(modelBuilder);
            ConfigureCommonRelationships(modelBuilder);
            ConfigureAuthorizationRelationships(modelBuilder);
        }

        private void ConfigurePersonelRelationships(ModelBuilder modelBuilder)
        {
            // Personel ana ilişkileri
            modelBuilder.Entity<Personel>(entity =>
            {
                entity.HasOne(p => p.Departman)
                      .WithMany(d => d.Personeller)
                      .HasForeignKey(p => p.DepartmanId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Servis)
                      .WithMany(s => s.Personeller)
                      .HasForeignKey(p => p.ServisId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Unvan)
                      .WithMany(u => u.Personeller)
                      .HasForeignKey(p => p.UnvanId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Il)
                      .WithMany()
                      .HasForeignKey(p => p.IlId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Ilce)
                      .WithMany()
                      .HasForeignKey(p => p.IlceId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.AtanmaNedeni)
                      .WithMany(a => a.Personeller)
                      .HasForeignKey(p => p.AtanmaNedeniId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Sendika)
                      .WithMany(s => s.Personeller)
                      .HasForeignKey(p => p.SendikaId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.HizmetBinasi)
                      .WithMany()
                      .HasForeignKey(p => p.HizmetBinasiId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.EsininIsIl)
                      .WithMany()
                      .HasForeignKey(p => p.EsininIsIlId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.EsininIsIlce)
                      .WithMany()
                      .HasForeignKey(p => p.EsininIsIlceId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // PersonelCocuk ilişkisi
            modelBuilder.Entity<PersonelCocuk>()
                .HasOne(pc => pc.Personel)
                .WithMany(p => p.PersonelCocuklari)
                .HasForeignKey(pc => pc.PersonelTcKimlikNo)
                .OnDelete(DeleteBehavior.Cascade);

            // PersonelYetki ilişkisi
            modelBuilder.Entity<PersonelYetki>()
                .HasOne(py => py.Personel)
                .WithMany()
                .HasForeignKey(py => py.TcKimlikNo)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PersonelYetki>()
                .HasOne(py => py.Yetki)
                .WithMany(y => y.PersonelYetkileri)
                .HasForeignKey(py => py.YetkiId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PersonelDepartman>()
                .HasOne(pd => pd.Personel)
                .WithMany()
                .HasForeignKey(pd => pd.TcKimlikNo)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PersonelDepartman>()
                .HasOne(pd => pd.Departman)
                .WithMany()
                .HasForeignKey(pd => pd.DepartmanId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureSiramatikRelationships(ModelBuilder modelBuilder)
        {
            // Banko ilişkileri
            modelBuilder.Entity<Banko>()
                .HasOne(b => b.HizmetBinasi)
                .WithMany()
                .HasForeignKey(b => b.HizmetBinasiId)
                .OnDelete(DeleteBehavior.Restrict);

            // BankoKullanici ilişkileri
            modelBuilder.Entity<BankoKullanici>(entity =>
            {
                entity.HasOne(bk => bk.Personel)
                      .WithMany()
                      .HasForeignKey(bk => bk.TcKimlikNo)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(bk => bk.Banko)
                      .WithMany(b => b.BankoKullanicilari)
                      .HasForeignKey(bk => bk.BankoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // TvBanko many-to-many
            modelBuilder.Entity<TvBanko>(entity =>
            {
                entity.HasOne(tb => tb.Tv)
                      .WithMany(t => t.TvBankolar)
                      .HasForeignKey(tb => tb.TvId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(tb => tb.Banko)
                      .WithMany(b => b.TvBankolar)
                      .HasForeignKey(tb => tb.BankoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Tv ilişkileri
            modelBuilder.Entity<Tv>()
                .HasOne(t => t.HizmetBinasi)
                .WithMany()
                .HasForeignKey(t => t.HizmetBinasiId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HubTvConnection>()
                .HasOne(htc => htc.Tv)
                .WithOne(t => t.HubTvConnection)
                .HasForeignKey<HubTvConnection>(htc => htc.TvId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HubConnection>()
                .HasOne(hc => hc.Personel)
                .WithOne(p => p.HubConnection)
                .HasForeignKey<HubConnection>(hc => hc.TcKimlikNo)
                .OnDelete(DeleteBehavior.Restrict);

            // KanalPersonel ilişkisi
            modelBuilder.Entity<KanalPersonel>()
                .HasOne(kp => kp.Personel)
                .WithMany()
                .HasForeignKey(kp => kp.TcKimlikNo)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<KanalPersonel>()
                .HasOne(kp => kp.KanalAltIslem)
                .WithMany(kai => kai.KanalPersonelleri)
                .HasForeignKey(kp => kp.KanalAltIslemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sira>()
                .HasOne(s => s.KanalAltIslem)
                .WithMany(kai => kai.Siralar)
                .HasForeignKey(s => s.KanalAltIslemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sira>()
                .HasOne(s => s.HizmetBinasi)
                .WithMany()
                .HasForeignKey(s => s.HizmetBinasiId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sira>()
                .HasOne(s => s.Personel)
                .WithMany()
                .HasForeignKey(s => s.TcKimlikNo)
                .OnDelete(DeleteBehavior.SetNull);
        }

        private void ConfigureCommonRelationships(ModelBuilder modelBuilder)
        {
            // Il-Ilce ilişkisi
            modelBuilder.Entity<Ilce>()
                .HasOne(i => i.Il)
                .WithMany(il => il.Ilceler)
                .HasForeignKey(i => i.IlId)
                .OnDelete(DeleteBehavior.Restrict);

            // HizmetBinasi-Departman ilişkisi
            modelBuilder.Entity<HizmetBinasi>()
                .HasOne(hb => hb.Departman)
                .WithMany()
                .HasForeignKey(hb => hb.DepartmanId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureAuthorizationRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Yetki>()
                .HasOne(y => y.UstYetki)
                .WithMany()
                .HasForeignKey(y => y.UstYetkiId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ModulController>()
                .HasOne(mc => mc.Modul)
                .WithMany(m => m.ModulControllers)
                .HasForeignKey(mc => mc.ModulId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ModulControllerIslem>()
                .HasOne(mci => mci.ModulController)
                .WithMany(mc => mc.ModulControllerIslemler)
                .HasForeignKey(mci => mci.ModulControllerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
        #endregion

        #region Performance Indexes
        private void ConfigurePerformanceIndexes(ModelBuilder modelBuilder)
        {
            // User indexes
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.TcKimlikNo).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.KullaniciAdi).IsUnique();
            });

            // Personel indexes
            modelBuilder.Entity<Personel>(entity =>
            {
                entity.HasIndex(p => p.TcKimlikNo).IsUnique();
                entity.HasIndex(p => p.SicilNo).IsUnique();
                entity.HasIndex(p => p.Email).IsUnique();
                entity.HasIndex(p => new { p.DepartmanId, p.PersonelAktiflikDurum });
                entity.HasIndex(p => p.PersonelAktiflikDurum);
                entity.HasIndex(p => new { p.HizmetBinasiId, p.PersonelAktiflikDurum });
            });

            // Master data indexes
            modelBuilder.Entity<Departman>()
                .HasIndex(d => d.DepartmanAdi).IsUnique();
            modelBuilder.Entity<Servis>()
                .HasIndex(s => s.ServisAdi).IsUnique();
            modelBuilder.Entity<Unvan>()
                .HasIndex(u => u.UnvanAdi).IsUnique();
            modelBuilder.Entity<Sendika>()
                .HasIndex(s => s.SendikaAdi).IsUnique();

            // Sıramatik indexes
            modelBuilder.Entity<Banko>()
                .HasIndex(b => new { b.HizmetBinasiId, b.BankoNo }).IsUnique();

            modelBuilder.Entity<TvBanko>()
                .HasIndex(tb => new { tb.TvId, tb.BankoId }).IsUnique();


            modelBuilder.Entity<BankoKullanici>()
                .HasIndex(bk => bk.BankoId).IsUnique();
            modelBuilder.Entity<BankoKullanici>()
                .HasIndex(bk => bk.TcKimlikNo).IsUnique();

            modelBuilder.Entity<KanalPersonel>()
                .HasIndex(kp => new { kp.KanalAltIslemId, kp.TcKimlikNo }).IsUnique();

            modelBuilder.Entity<Sira>(entity =>
            {
                entity.HasIndex(s => new { s.SiraNo, s.HizmetBinasiId, s.SiraAlisZamani }).IsUnique();
                entity.HasIndex(s => s.BeklemeDurum);
                entity.HasIndex(s => s.SiraAlisZamani);
                entity.HasIndex(s => new { s.KanalAltIslemId, s.BeklemeDurum });
            });

            modelBuilder.Entity<Il>()
                .HasIndex(i => i.IlAdi).IsUnique();
            modelBuilder.Entity<Ilce>()
                .HasIndex(i => new { i.IlId, i.IlceAdi }).IsUnique();

            modelBuilder.Entity<HubConnection>()
                .HasIndex(hc => new { hc.TcKimlikNo, hc.ConnectionId, hc.ConnectionStatus });
            modelBuilder.Entity<HubConnection>()
                .HasIndex(hc => hc.TcKimlikNo).IsUnique();

            modelBuilder.Entity<HubTvConnection>()
                .HasIndex(htc => new { htc.TvId, htc.ConnectionId, htc.ConnectionStatus });
            modelBuilder.Entity<HubTvConnection>()
                .HasIndex(htc => htc.TvId).IsUnique();

            modelBuilder.Entity<Yetki>()
                .HasIndex(y => new { y.UstYetkiId, y.YetkiAdi }).IsUnique();
            modelBuilder.Entity<Yetki>()
                .HasIndex(y => y.YetkiAdi);
            modelBuilder.Entity<Yetki>()
                .HasIndex(y => y.ControllerAdi);
        }
        #endregion

        #region Enum Conversions
        private void ConfigureEnumConversions(ModelBuilder modelBuilder)
        {
            // Personel enums
            modelBuilder.Entity<Personel>(entity =>
            {
                entity.Property(p => p.Cinsiyet).HasConversion<string>();
                entity.Property(p => p.PersonelTipi).HasConversion<string>();
                entity.Property(p => p.PersonelAktiflikDurum).HasConversion<int>();
                entity.Property(p => p.MedeniDurumu).HasConversion<string>();
                entity.Property(p => p.KanGrubu).HasConversion<string>();
                entity.Property(p => p.OgrenimDurumu).HasConversion<string>();
                entity.Property(p => p.EvDurumu).HasConversion<string>();
                entity.Property(p => p.EsininIsDurumu).HasConversion<string>();
                entity.Property(p => p.SehitYakinligi).HasConversion<string>();
            });

            // PersonelCocuk enums
            modelBuilder.Entity<PersonelCocuk>()
                .Property(pc => pc.OgrenimDurumu).HasConversion<string>();

            // Master data enums
            modelBuilder.Entity<Departman>()
                .Property(d => d.DepartmanAktiflik).HasConversion<int>();
            modelBuilder.Entity<Servis>()
                .Property(s => s.ServisAktiflik).HasConversion<int>();
            modelBuilder.Entity<Unvan>()
                .Property(u => u.UnvanAktiflik).HasConversion<int>();

            // Sıramatik enums
            modelBuilder.Entity<Banko>(entity =>
            {
                entity.Property(b => b.BankoTipi).HasConversion<string>();
                entity.Property(b => b.KatTipi).HasConversion<string>();
                entity.Property(b => b.BankoAktiflik).HasConversion<int>();
            });

            modelBuilder.Entity<BankoIslem>(entity =>
            {
                entity.Property(bi => bi.BankoGrup).HasConversion<string>();
                entity.Property(bi => bi.BankoIslemAktiflik).HasConversion<int>();
            });

            modelBuilder.Entity<Tv>()
                .Property(t => t.KatTipi).HasConversion<string>();

            modelBuilder.Entity<Sira>()
                .Property(s => s.BeklemeDurum).HasConversion<int>();

            modelBuilder.Entity<HubConnection>()
                .Property(hc => hc.ConnectionStatus).HasConversion<string>();
            modelBuilder.Entity<HubTvConnection>()
                .Property(htc => htc.ConnectionStatus).HasConversion<string>();

            // Authorization enums
            modelBuilder.Entity<Yetki>()
                .Property(y => y.YetkiTuru).HasConversion<string>();
            modelBuilder.Entity<PersonelYetki>()
                .Property(py => py.YetkiTipleri).HasConversion<string>();

            // System enums
            modelBuilder.Entity<DatabaseLog>()
                .Property(dl => dl.DatabaseAction).HasConversion<string>();
        }
        #endregion

        #region Business Rules
        private void ConfigureBusinessRules(ModelBuilder modelBuilder)
        {
            // Sira tablosu için özel kurallar
            modelBuilder.Entity<Sira>()
                .ToTable("SGKP_SRM_Siralar", t => t.UseSqlOutputClause(false));

            // Soft delete konfigürasyonu
            modelBuilder.Entity<User>()
                .HasQueryFilter(u => !u.SilindiMi);

            ConfigureStringLengths(modelBuilder);
        }

        private void ConfigureStringLengths(ModelBuilder modelBuilder)
        {
            // User string lengths
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.TcKimlikNo).HasMaxLength(11);
                entity.Property(u => u.KullaniciAdi).HasMaxLength(50);
                entity.Property(u => u.Email).HasMaxLength(100);
                entity.Property(u => u.TelefonNo).HasMaxLength(20);
            });

            // Personel string lengths
            modelBuilder.Entity<Personel>(entity =>
            {
                entity.Property(p => p.TcKimlikNo).HasMaxLength(11);
                entity.Property(p => p.AdSoyad).HasMaxLength(200);
                entity.Property(p => p.Email).HasMaxLength(100);
                entity.Property(p => p.CepTelefonu).HasMaxLength(20);
                entity.Property(p => p.Adres).HasMaxLength(500);
                entity.Property(p => p.NickName).HasMaxLength(50);
                entity.Property(p => p.Gorev).HasMaxLength(100);
                entity.Property(p => p.Uzmanlik).HasMaxLength(100);
                entity.Property(p => p.Resim).HasMaxLength(255);
                entity.Property(p => p.PassWord).HasMaxLength(255);
            });

            // Master data string lengths
            modelBuilder.Entity<Departman>()
                .Property(d => d.DepartmanAdi).HasMaxLength(150);
            modelBuilder.Entity<Servis>()
                .Property(s => s.ServisAdi).HasMaxLength(150);
            modelBuilder.Entity<Unvan>()
                .Property(u => u.UnvanAdi).HasMaxLength(100);
            modelBuilder.Entity<Sendika>()
                .Property(s => s.SendikaAdi).HasMaxLength(150);

            // Location string lengths
            modelBuilder.Entity<Il>()
                .Property(i => i.IlAdi).HasMaxLength(50);
            modelBuilder.Entity<Ilce>()
                .Property(i => i.IlceAdi).HasMaxLength(100);
        }

        private void ConfigureAuditingRules(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property("EklenmeTarihi")
                        .HasDefaultValueSql("GETDATE()");

                    modelBuilder.Entity(entityType.ClrType)
                        .Property("DuzenlenmeTarihi")
                        .HasDefaultValueSql("GETDATE()");
                }
            }
        }
        #endregion
    }
}