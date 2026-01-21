using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Context;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Seeding
{
    /// <summary>
    /// Database başlangıç verilerini oluşturur
    /// Eski projenin DB'sinden gelen verilerle uyumlu olması için ID'ler sabit tutulur
    /// </summary>
    public class DatabaseSeeder
    {
        private readonly SGKDbContext _context;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(SGKDbContext context, ILogger<DatabaseSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Database'i seed eder (başlangıç verilerini oluşturur)
        /// </summary>
        public async Task SeedAsync()
        {
            try
            {
                _logger.LogInformation("🌱 Database seeding başlatılıyor...");

                // ÖNEMLI: Sıralama çok kritik! Bağımlılıklar önce oluşturulmalı
                await SeedDepartmanAsync();      // ID=1 (HizmetBinasi'nın FK'si)
                await SeedServisAsync();         // ID=1 (Personel'in FK'si)
                await SeedUnvanAsync();          // ID=1 (Personel'in FK'si)
                await SeedHizmetBinasiAsync();   // ID=1 (Personel'in FK'si, Departman'a bağımlı)
                await SeedAtanmaNedeniAsync();   // ID=1 (Personel'in nullable FK'si)

                _logger.LogInformation("✅ Database seeding tamamlandı!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Database seeding sırasında hata oluştu!");
                throw;
            }
        }

        /// <summary>
        /// Departman seed - ID=1 garantili
        /// INSERT INTO `PER_Departmanlar` VALUES (1, 'İL MÜDÜRLÜĞÜ', 'İL MÜDÜRLÜĞÜ', '08:00:00', '17:00:00', NULL, NULL);
        /// </summary>
        private async Task SeedDepartmanAsync()
        {
            if (await _context.Departmanlar.AnyAsync())
            {
                _logger.LogInformation("  ⏭️  Departman zaten mevcut, atlanıyor...");
                return;
            }

            // Identity Insert aktif et (MySQL için gerekli değil ama SQL Server için gerekli)
            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT PER_Departmanlar ON");

            var departman = new Departman
            {
                DepartmanId = 1,
                DepartmanAdi = "İL MÜDÜRLÜĞÜ",
                DepartmanAdiKisa = "İL MÜDÜRLÜĞÜ",
                Aktiflik = Aktiflik.Aktif,
                EkleyenKullanici = "System",
                EklenmeTarihi = DateTime.Now,
                DuzenleyenKullanici = "System",
                DuzenlenmeTarihi = DateTime.Now
            };

            await _context.Departmanlar.AddAsync(departman);
            await _context.SaveChangesAsync();

            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT PER_Departmanlar OFF");

            _logger.LogInformation("  ✅ Departman oluşturuldu: ID={Id}, Ad={Ad}", departman.DepartmanId, departman.DepartmanAdi);
        }

        /// <summary>
        /// Servis seed - ID=1 garantili
        /// INSERT INTO `PER_Servisler` VALUES (1, 'SGK RANT TESİS. ARŞİV SERVİSİ');
        /// </summary>
        private async Task SeedServisAsync()
        {
            if (await _context.Servisler.AnyAsync())
            {
                _logger.LogInformation("  ⏭️  Servis zaten mevcut, atlanıyor...");
                return;
            }

            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT PER_Servisler ON");

            var servis = new Servis
            {
                ServisId = 1,
                ServisAdi = "SGK RANT TESİS. ARŞİV SERVİSİ",
                Aktiflik = Aktiflik.Aktif,
                EkleyenKullanici = "System",
                EklenmeTarihi = DateTime.Now,
                DuzenleyenKullanici = "System",
                DuzenlenmeTarihi = DateTime.Now
            };

            await _context.Servisler.AddAsync(servis);
            await _context.SaveChangesAsync();

            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT PER_Servisler OFF");

            _logger.LogInformation("  ✅ Servis oluşturuldu: ID={Id}, Ad={Ad}", servis.ServisId, servis.ServisAdi);
        }

        /// <summary>
        /// Ünvan seed - ID=1 garantili
        /// INSERT INTO `PER_Unvanlar` VALUES ('MEMUR', 1);
        /// </summary>
        private async Task SeedUnvanAsync()
        {
            if (await _context.Unvanlar.AnyAsync())
            {
                _logger.LogInformation("  ⏭️  Ünvan zaten mevcut, atlanıyor...");
                return;
            }

            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT PER_Unvanlar ON");

            var unvan = new Unvan
            {
                UnvanId = 1,
                UnvanAdi = "MEMUR",
                Aktiflik = Aktiflik.Aktif,
                EkleyenKullanici = "System",
                EklenmeTarihi = DateTime.Now,
                DuzenleyenKullanici = "System",
                DuzenlenmeTarihi = DateTime.Now
            };

            await _context.Unvanlar.AddAsync(unvan);
            await _context.SaveChangesAsync();

            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT PER_Unvanlar OFF");

            _logger.LogInformation("  ✅ Ünvan oluşturuldu: ID={Id}, Ad={Ad}", unvan.UnvanId, unvan.UnvanAdi);
        }

        /// <summary>
        /// Hizmet Binası seed - ID=1 garantili
        /// DepartmanId=1'e bağımlı (önce Departman oluşturulmalı)
        /// </summary>
        private async Task SeedHizmetBinasiAsync()
        {
            if (await _context.HizmetBinalari.AnyAsync())
            {
                _logger.LogInformation("  ⏭️  Hizmet Binası zaten mevcut, atlanıyor...");
                return;
            }

            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT CMN_HizmetBinalari ON");

            var hizmetBinasi = new HizmetBinasi
            {
                HizmetBinasiId = 1,
                HizmetBinasiAdi = "İL MÜDÜRLÜĞÜ",
                DepartmanId = 1, // Yukarıda oluşturulan Departman
                Aktiflik = Aktiflik.Aktif,
                EkleyenKullanici = "System",
                EklenmeTarihi = DateTime.Now,
                DuzenleyenKullanici = "System",
                DuzenlenmeTarihi = DateTime.Now
            };

            await _context.HizmetBinalari.AddAsync(hizmetBinasi);
            await _context.SaveChangesAsync();

            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT CMN_HizmetBinalari OFF");

            _logger.LogInformation("  ✅ Hizmet Binası oluşturuldu: ID={Id}, Ad={Ad}", hizmetBinasi.HizmetBinasiId, hizmetBinasi.HizmetBinasiAdi);
        }

        /// <summary>
        /// Atanma Nedeni seed - ID=1 garantili (opsiyonel ama önerilir)
        /// Personel entity'sinde nullable FK olarak kullanılıyor
        /// </summary>
        private async Task SeedAtanmaNedeniAsync()
        {
            if (await _context.AtanmaNedenleri.AnyAsync())
            {
                _logger.LogInformation("  ⏭️  Atanma Nedeni zaten mevcut, atlanıyor...");
                return;
            }

            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT PER_AtanmaNedenleri ON");

            var atanmaNedeni = new AtanmaNedenleri
            {
                AtanmaNedeniId = 1,
                AtanmaNedeni = "Asaleten Atama",
                EkleyenKullanici = "System",
                EklenmeTarihi = DateTime.Now,
                DuzenleyenKullanici = "System",
                DuzenlenmeTarihi = DateTime.Now
            };

            await _context.AtanmaNedenleri.AddAsync(atanmaNedeni);
            await _context.SaveChangesAsync();

            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT PER_AtanmaNedenleri OFF");

            _logger.LogInformation("  ✅ Atanma Nedeni oluşturuldu: ID={Id}, Ad={Ad}", atanmaNedeni.AtanmaNedeniId, atanmaNedeni.AtanmaNedeni);
        }
    }
}
