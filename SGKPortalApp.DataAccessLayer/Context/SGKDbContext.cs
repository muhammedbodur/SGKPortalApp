using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.DataAccessLayer.Configurations.Common;
using SGKPortalApp.DataAccessLayer.Configurations.PersonelIslemleri;
using SGKPortalApp.DataAccessLayer.Configurations.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Configurations.SignalR;
using SGKPortalApp.DataAccessLayer.Configurations.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Entities.SignalR;

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
        public DbSet<UserDomainMapping> UserDomainMappings { get; set; }
        public DbSet<LoginLogoutLog> LoginLogoutLogs { get; set; }
        public DbSet<HizmetBinasi> HizmetBinalari { get; set; }
        #endregion

        #region Personel İşlemleri
        public DbSet<Personel> Personeller { get; set; }
        public DbSet<PersonelCocuk> PersonelCocuklari { get; set; }
        public DbSet<PersonelDepartman> PersonelDepartmanlar { get; set; }
        public DbSet<PersonelYetki> PersonelYetkileri { get; set; }
        public DbSet<PersonelHizmet> PersonelHizmetleri { get; set; }
        public DbSet<PersonelEgitim> PersonelEgitimleri { get; set; }
        public DbSet<PersonelImzaYetkisi> PersonelImzaYetkileri { get; set; }
        public DbSet<PersonelCeza> PersonelCezalari { get; set; }
        public DbSet<PersonelEngel> PersonelEngelleri { get; set; }
        public DbSet<Departman> Departmanlar { get; set; }
        public DbSet<Servis> Servisler { get; set; }
        public DbSet<Unvan> Unvanlar { get; set; }
        public DbSet<Sendika> Sendikalar { get; set; }
        public DbSet<AtanmaNedenleri> AtanmaNedenleri { get; set; }
        public DbSet<Modul> Moduller { get; set; }
        public DbSet<ModulController> ModulControllers { get; set; }
        public DbSet<ModulControllerIslem> ModulControllerIslemleri { get; set; }
        #endregion

        #region Sıramatik İşlemleri
        public DbSet<Banko> Bankolar { get; set; }
        public DbSet<BankoKullanici> BankoKullanicilari { get; set; }
        public DbSet<BankoHareket> BankoHareketleri { get; set; }
        public DbSet<Kanal> Kanallar { get; set; }
        public DbSet<KanalAlt> KanallarAlt { get; set; }
        public DbSet<KanalIslem> KanalIslemleri { get; set; }
        public DbSet<KanalAltIslem> KanalAltIslemleri { get; set; }
        public DbSet<KanalPersonel> KanalPersonelleri { get; set; }
        public DbSet<KioskMenu> KioskMenuler { get; set; }
        public DbSet<KioskMenuIslem> KioskMenuIslemleri { get; set; }
        public DbSet<Kiosk> Kiosklar { get; set; }
        public DbSet<KioskMenuAtama> KioskMenuAtamalari { get; set; }
        public DbSet<Tv> Tvler { get; set; }
        public DbSet<TvBanko> TvBankolari { get; set; }
        public DbSet<Sira> Siralar { get; set; }
        public DbSet<HubConnection> HubConnections { get; set; }
        public DbSet<HubTvConnection> HubTvConnections { get; set; }
        public DbSet<HubBankoConnection> HubBankoConnections { get; set; }
        #endregion

        #region SignalR
        public DbSet<SignalREventLog> SignalREventLogs { get; set; }
        #endregion

        #region ZKTeco
        public DbSet<ZKTecoDevice> ZKTecoDevices { get; set; }
        public DbSet<ZKTecoSpecialCard> ZKTecoSpecialCards { get; set; }
        public DbSet<CekilenData> CekilenDatalar { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ApplyEntityConfigurations(modelBuilder);

            ConfigureAdditionalRelationships(modelBuilder);
        }

        #region Entity Configurations
        private void ApplyEntityConfigurations(ModelBuilder modelBuilder)
        {
            // Common
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new IlConfiguration());
            modelBuilder.ApplyConfiguration(new IlceConfiguration());
            modelBuilder.ApplyConfiguration(new HizmetBinasiConfiguration());
            modelBuilder.ApplyConfiguration(new DatabaseLogConfiguration());
            modelBuilder.ApplyConfiguration(new UserDomainMappingConfiguration());
            modelBuilder.ApplyConfiguration(new LoginLogoutLogConfiguration());
            modelBuilder.ApplyConfiguration(new ModulConfiguration());
            modelBuilder.ApplyConfiguration(new ModulControllerConfiguration());
            modelBuilder.ApplyConfiguration(new ModulControllerIslemConfiguration());

            // Personel İşlemleri
            modelBuilder.ApplyConfiguration(new DepartmanConfiguration());
            modelBuilder.ApplyConfiguration(new ServisConfiguration());
            modelBuilder.ApplyConfiguration(new UnvanConfiguration());
            modelBuilder.ApplyConfiguration(new SendikaConfiguration());
            modelBuilder.ApplyConfiguration(new AtanmaNedenleriConfiguration());
            modelBuilder.ApplyConfiguration(new PersonelConfiguration());
            modelBuilder.ApplyConfiguration(new PersonelCocukConfiguration());
            modelBuilder.ApplyConfiguration(new PersonelDepartmanConfiguration());
            modelBuilder.ApplyConfiguration(new PersonelYetkiConfiguration());
            modelBuilder.ApplyConfiguration(new PersonelHizmetConfiguration());
            modelBuilder.ApplyConfiguration(new PersonelEgitimConfiguration());
            modelBuilder.ApplyConfiguration(new PersonelImzaYetkisiConfiguration());
            modelBuilder.ApplyConfiguration(new PersonelCezaConfiguration());
            modelBuilder.ApplyConfiguration(new PersonelEngelConfiguration());

            // Sıramatik İşlemleri
            modelBuilder.ApplyConfiguration(new BankoConfiguration());
            modelBuilder.ApplyConfiguration(new BankoKullaniciConfiguration());
            modelBuilder.ApplyConfiguration(new BankoHareketConfiguration());
            modelBuilder.ApplyConfiguration(new KanalConfiguration());
            modelBuilder.ApplyConfiguration(new KanalAltConfiguration());
            modelBuilder.ApplyConfiguration(new KanalIslemConfiguration());
            modelBuilder.ApplyConfiguration(new KanalAltIslemConfiguration());
            modelBuilder.ApplyConfiguration(new KanalPersonelConfiguration());
            modelBuilder.ApplyConfiguration(new KioskMenuConfiguration());
            modelBuilder.ApplyConfiguration(new KioskMenuIslemConfiguration());
            modelBuilder.ApplyConfiguration(new KioskConfiguration());
            modelBuilder.ApplyConfiguration(new KioskMenuAtamaConfiguration());
            modelBuilder.ApplyConfiguration(new TvConfiguration());
            modelBuilder.ApplyConfiguration(new TvBankoConfiguration());
            modelBuilder.ApplyConfiguration(new SiraConfiguration());
            modelBuilder.ApplyConfiguration(new HubConnectionConfiguration());
            modelBuilder.ApplyConfiguration(new HubTvConnectionConfiguration());
            modelBuilder.ApplyConfiguration(new HubBankoConnectionConfiguration());

            // SignalR
            modelBuilder.ApplyConfiguration(new SignalREventLogConfiguration());

            // ZKTeco
            modelBuilder.ApplyConfiguration(new ZKTecoDeviceConfiguration());
            modelBuilder.ApplyConfiguration(new CekilenDataConfiguration());
        }
        #endregion

        #region Additional Relationships
        private void ConfigureAdditionalRelationships(ModelBuilder modelBuilder)
        {
            // Personel'in lookup relationship'leri (Configuration'da olmayan)
            modelBuilder.Entity<Personel>(entity =>
            {

                // Il ve Ilce (multiple reference - Configuration'da yok)
                entity.HasOne(p => p.Il)
                      .WithMany()
                      .HasForeignKey(p => p.IlId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Ilce)
                      .WithMany()
                      .HasForeignKey(p => p.IlceId)
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

            // Sira relationship'leri (Configuration'da kısmi tanımlı)
            modelBuilder.Entity<Sira>(entity =>
            {
                // Personel (nullable) - Configuration'da yok
                entity.HasOne(s => s.Personel)
                      .WithMany()
                      .HasForeignKey(s => s.TcKimlikNo)
                      .OnDelete(DeleteBehavior.SetNull);
            });
        }
        #endregion
    }
}