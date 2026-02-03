using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Context;

namespace SGKPortalApp.DataAccessLayer.SeedData
{
    /// <summary>
    /// Dashboard için örnek seed data
    /// </summary>
    public static class DashboardSeedData
    {
        public static async Task SeedDashboardDataAsync(IServiceProvider serviceProvider, ILogger? logger = null)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SGKDbContext>();

            try
            {
                await SeedDuyurularAsync(context, logger);
                await SeedGununMenusuAsync(context, logger);
                await SeedOnemliLinklerAsync(context, logger);
                await SeedSikKullanilanProgramlarAsync(context, logger);

                logger?.LogInformation("Dashboard seed data başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Dashboard seed data eklenirken hata oluştu.");
                throw;
            }
        }

        private static async Task SeedDuyurularAsync(SGKDbContext context, ILogger? logger)
        {
            if (await context.Duyurular.AnyAsync())
            {
                logger?.LogInformation("Duyurular tablosunda zaten veri var, seed atlanıyor.");
                return;
            }

            var duyurular = new List<Duyuru>
            {
                // Slider Duyuruları (Sira 1-5 = Slider)
                new Duyuru
                {
                    Baslik = "İzmir İşverenleri ile Değerlendirme Toplantısı Gerçekleşti",
                    Icerik = "İzmir Sosyal Güvenlik İl Müdürü Mehmet Karagöz, işverenlerle değerlendirme toplantısı gerçekleştirerek sorunları ve çözünleri ele aldı. Toplantıda 2024 yılı hedefleri ve yeni uygulamalar hakkında bilgi verildi.",
                    GorselUrl = "/portal/assets/img/slider/slider1.jpg",
                    Sira = 1,
                    YayinTarihi = DateTime.Now.AddDays(-2),
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                },
                new Duyuru
                {
                    Baslik = "SGK Mobil Uygulama Yenilendi",
                    Icerik = "SGK Mobil uygulaması yeni özellikleriyle güncellendi. Artık e-Devlet şifresi ile giriş yapabilir, hizmet dökümünüzü görüntüleyebilir ve prim borç sorgulama yapabilirsiniz.",
                    GorselUrl = "/portal/assets/img/slider/slider2.jpg",
                    Sira = 2,
                    YayinTarihi = DateTime.Now.AddDays(-5),
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                },
                new Duyuru
                {
                    Baslik = "Emeklilik Yaşı Hesaplama Hizmeti Aktif",
                    Icerik = "Yeni emeklilik yaşı hesaplama hizmeti artık e-Devlet üzerinden aktif. Vatandaşlarımız emeklilik haklarını ve tahmini emeklilik tarihlerini öğrenebilir.",
                    GorselUrl = "/portal/assets/img/slider/slider3.jpg",
                    Sira = 3,
                    YayinTarihi = DateTime.Now.AddDays(-7),
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                },
                new Duyuru
                {
                    Baslik = "Konak İlçe Tarım ve Orman Müdürlüğü Ziyareti",
                    Icerik = "İzmir SGK İl Müdürlüğü yetkilileri, Konak İlçe Tarım ve Orman Müdürlüğü'nü ziyaret ederek tarım sigortası konusunda bilgi alışverişinde bulundu.",
                    GorselUrl = "/portal/assets/img/slider/slider4.jpg",
                    Sira = 4,
                    YayinTarihi = DateTime.Now.AddDays(-10),
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                },

                // Liste Duyuruları (Sira 10+ = Liste)
                new Duyuru
                {
                    Baslik = "Anlaşma Yapılan Özel Hastanelere İlişkin Değerlendirme Tablosu",
                    Icerik = "2024 yılı için anlaşma yapılan özel hastanelerin güncel listesi yayınlandı. Detaylı bilgi için tıklayınız.",
                    Sira = 10,
                    YayinTarihi = DateTime.Now.AddDays(-1),
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                },
                new Duyuru
                {
                    Baslik = "SGK Eğitim Programı Duyurusu",
                    Icerik = "2024 yılı personel eğitim programı açıklandı. Eğitimlere katılım için birim amirlerinize başvurunuz.",
                    Sira = 11,
                    YayinTarihi = DateTime.Now.AddDays(-3),
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                },
                new Duyuru
                {
                    Baslik = "Genel Sağlık Sigortası Düzenlemeleri",
                    Icerik = "Genel sağlık sigortası kapsamında yapılan yeni düzenlemeler hakkında bilgilendirme.",
                    Sira = 12,
                    YayinTarihi = DateTime.Now.AddDays(-5),
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                },
                new Duyuru
                {
                    Baslik = "Kayseri İl Müdürlüğüne Yönelik Eğitim Programı",
                    Icerik = "Kayseri İl Müdürlüğü personeline yönelik düzenlenen eğitim programı hakkında detaylı bilgi.",
                    Sira = 13,
                    YayinTarihi = DateTime.Now.AddDays(-8),
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                },
                new Duyuru
                {
                    Baslik = "Yeni Teşvik Paketinin Yürürlüğe Girmesi",
                    Icerik = "İşverenler için hazırlanan yeni teşvik paketi 01.01.2024 tarihi itibariyle yürürlüğe girdi.",
                    Sira = 14,
                    YayinTarihi = DateTime.Now.AddDays(-12),
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                }
            };

            await context.Duyurular.AddRangeAsync(duyurular);
            await context.SaveChangesAsync();
            logger?.LogInformation("{Count} adet duyuru eklendi.", duyurular.Count);
        }

        private static async Task SeedGununMenusuAsync(SGKDbContext context, ILogger? logger)
        {
            if (await context.GununMenuleri.AnyAsync())
            {
                logger?.LogInformation("Günün Menüsü tablosunda zaten veri var, seed atlanıyor.");
                return;
            }

            var menuler = new List<GununMenusu>
            {
                new GununMenusu
                {
                    Tarih = DateTime.Today,
                    Icerik = "ARNIKUT CİĞERİ\nMEYANE PİLAVI\nTA KAHNA ÇÖRESAY\nAYRAN",
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                },
                new GununMenusu
                {
                    Tarih = DateTime.Today.AddDays(1),
                    Icerik = "MERCIMEK ÇORBASI\nKARNIYARIK\nPİRİNÇ PİLAVI\nCACIK",
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                },
                new GununMenusu
                {
                    Tarih = DateTime.Today.AddDays(2),
                    Icerik = "DOMATES ÇORBASI\nTAVUK SOTE\nBULGUR PİLAVI\nMEVSİM SALATASI",
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                }
            };

            await context.GununMenuleri.AddRangeAsync(menuler);
            await context.SaveChangesAsync();
            logger?.LogInformation("{Count} adet günün menüsü eklendi.", menuler.Count);
        }

        private static async Task SeedOnemliLinklerAsync(SGKDbContext context, ILogger? logger)
        {
            if (await context.OnemliLinkler.AnyAsync())
            {
                logger?.LogInformation("Önemli Linkler tablosunda zaten veri var, seed atlanıyor.");
                return;
            }

            var linkler = new List<OnemliLink>
            {
                new OnemliLink
                {
                    LinkAdi = "Sosyal Güvenlik Kurumu",
                    Url = "https://www.sgk.gov.tr",
                    Sira = 1,
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                },
                new OnemliLink
                {
                    LinkAdi = "Mevzuatlar",
                    Url = "https://www.mevzuat.gov.tr",
                    Sira = 2,
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                },
                new OnemliLink
                {
                    LinkAdi = "T.C. Resmi Gazete",
                    Url = "https://www.resmigazete.gov.tr",
                    Sira = 3,
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                },
                new OnemliLink
                {
                    LinkAdi = "İşveren Uygulama Rehberi",
                    Url = "https://www.sgk.gov.tr/wps/portal/sgk/tr/calisan/isveren",
                    Sira = 4,
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                },
                new OnemliLink
                {
                    LinkAdi = "SGK Uzmanları Derneği",
                    Url = "https://www.sgkuzmanlari.org.tr",
                    Sira = 5,
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                },
                new OnemliLink
                {
                    LinkAdi = "SGK İl Müdürlükleri",
                    Url = "https://www.sgk.gov.tr/wps/portal/sgk/tr/kurumsal/il_mudurlukleri",
                    Sira = 6,
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                },
                new OnemliLink
                {
                    LinkAdi = "Tüm Tam Memurlar Derneği",
                    Url = "https://www.tumtammemurlar.org.tr",
                    Sira = 7,
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                },
                new OnemliLink
                {
                    LinkAdi = "e-Devlet Kapısı",
                    Url = "https://www.turkiye.gov.tr",
                    Sira = 8,
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                }
            };

            await context.OnemliLinkler.AddRangeAsync(linkler);
            await context.SaveChangesAsync();
            logger?.LogInformation("{Count} adet önemli link eklendi.", linkler.Count);
        }

        private static async Task SeedSikKullanilanProgramlarAsync(SGKDbContext context, ILogger? logger)
        {
            if (await context.SikKullanilanProgramlar.AnyAsync())
            {
                logger?.LogInformation("Sık Kullanılan Programlar tablosunda zaten veri var, seed atlanıyor.");
                return;
            }

            var programlar = new List<SikKullanilanProgram>
            {
                new SikKullanilanProgram
                {
                    ProgramAdi = "İşveren Uygulama Rehberi",
                    Url = "https://www.sgk.gov.tr/wps/portal/sgk/tr/calisan/isveren",
                    IkonClass = "bx-book-reader",
                    RenkKodu = "primary",
                    Sira = 1,
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                },
                new SikKullanilanProgram
                {
                    ProgramAdi = "Tevkifat Kontrol",
                    Url = "/tevkifat-kontrol",
                    IkonClass = "bx-check-shield",
                    RenkKodu = "info",
                    Sira = 2,
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                },
                new SikKullanilanProgram
                {
                    ProgramAdi = "Yetki Talep",
                    Url = "/yetki/talep",
                    IkonClass = "bx-key",
                    RenkKodu = "warning",
                    Sira = 3,
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                },
                new SikKullanilanProgram
                {
                    ProgramAdi = "Destek Talep",
                    Url = "/destek",
                    IkonClass = "bx-support",
                    RenkKodu = "success",
                    Sira = 4,
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                },
                new SikKullanilanProgram
                {
                    ProgramAdi = "Ankara Görüşler",
                    Url = "/ankara-gorusler",
                    IkonClass = "bx-conversation",
                    RenkKodu = "danger",
                    Sira = 5,
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                },
                new SikKullanilanProgram
                {
                    ProgramAdi = "e-Bildirge",
                    Url = "https://ebildirge.sgk.gov.tr",
                    IkonClass = "bx-file",
                    RenkKodu = "primary",
                    Sira = 6,
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                },
                new SikKullanilanProgram
                {
                    ProgramAdi = "Hizmet Dökümü",
                    Url = "/hizmet-dokumu",
                    IkonClass = "bx-list-ul",
                    RenkKodu = "info",
                    Sira = 7,
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                },
                new SikKullanilanProgram
                {
                    ProgramAdi = "Prim Borç Sorgulama",
                    Url = "/prim-borc",
                    IkonClass = "bx-search-alt",
                    RenkKodu = "warning",
                    Sira = 8,
                    Aktiflik = Aktiflik.Aktif,
                    EklenmeTarihi = DateTime.Now,
                    EkleyenKullanici = "System"
                }
            };

            await context.SikKullanilanProgramlar.AddRangeAsync(programlar);
            await context.SaveChangesAsync();
            logger?.LogInformation("{Count} adet sık kullanılan program eklendi.", programlar.Count);
        }

        /// <summary>
        /// Dashboard verilerini siler (Test amaçlı)
        /// </summary>
        public static async Task ClearDashboardDataAsync(IServiceProvider serviceProvider, ILogger? logger = null)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SGKDbContext>();

            try
            {
                // Soft delete yerine hard delete yapıyoruz (seed data için)
                await context.Database.ExecuteSqlRawAsync("DELETE FROM dbo.CMN_Duyurular WHERE EkleyenKullanici = 'System'");
                await context.Database.ExecuteSqlRawAsync("DELETE FROM dbo.CMN_GununMenusu WHERE EkleyenKullanici = 'System'");
                await context.Database.ExecuteSqlRawAsync("DELETE FROM dbo.CMN_OnemliLinkler WHERE EkleyenKullanici = 'System'");
                await context.Database.ExecuteSqlRawAsync("DELETE FROM dbo.CMN_SikKullanilanProgramlar WHERE EkleyenKullanici = 'System'");

                logger?.LogInformation("Dashboard seed data temizlendi.");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Dashboard seed data temizlenirken hata oluştu.");
            }
        }
    }
}
