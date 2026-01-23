using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Context.Legacy;
using SGKPortalApp.BusinessLogicLayer.Interfaces.BackgroundServiceManager;

namespace SGKPortalApp.BusinessLogicLayer.Services.BackgroundServices.Sync
{
    public class MySqlSyncService : BackgroundService, IManagedBackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MySqlSyncService> _logger;
        private TimeSpan _syncInterval = TimeSpan.FromMinutes(5);
        private bool _isRunning;
        private readonly SemaphoreSlim _triggerSemaphore = new(0, 1);

        // IManagedBackgroundService properties
        public string ServiceName => "MySqlSyncService";
        public string DisplayName => "MySQL Veri Senkronizasyonu";
        public bool IsRunning => _isRunning;
        public bool IsPaused { get; set; }
        public DateTime? LastRunTime { get; private set; }
        public DateTime? NextRunTime { get; private set; }
        public TimeSpan Interval
        {
            get => _syncInterval;
            set => _syncInterval = value;
        }
        public string? LastError { get; private set; }
        public int SuccessCount { get; private set; }
        public int ErrorCount { get; private set; }

        public MySqlSyncService(IServiceProvider serviceProvider, ILogger<MySqlSyncService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task TriggerAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("🔄 {ServiceName} manuel tetiklendi", ServiceName);
            await SyncAllAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🔄 {ServiceName} başlatıldı. Sync aralığı: {Interval} dakika", ServiceName, _syncInterval.TotalMinutes);

            while (!stoppingToken.IsCancellationRequested)
            {
                NextRunTime = DateTime.Now.Add(_syncInterval);

                if (!IsPaused)
                {
                    try
                    {
                        _isRunning = true;
                        await SyncAllAsync(stoppingToken);
                        LastRunTime = DateTime.Now;
                        LastError = null;
                        SuccessCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌ Sync sırasında hata oluştu");
                        LastError = ex.Message;
                        ErrorCount++;
                    }
                    finally
                    {
                        _isRunning = false;
                    }
                }
                else
                {
                    _logger.LogDebug("⏸️ {ServiceName} duraklatıldı, atlanıyor...", ServiceName);
                }

                await Task.Delay(_syncInterval, stoppingToken);
            }
        }

        public async Task SyncAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("🔄 Sync başlıyor...");

            using var scope = _serviceProvider.CreateScope();
            var mysqlContext = scope.ServiceProvider.GetRequiredService<MysqlDbContext>();
            var sqlServerContext = scope.ServiceProvider.GetRequiredService<SGKDbContext>();

            await SyncDepartmanlarAsync(mysqlContext, sqlServerContext, cancellationToken);
            await SyncServislerAsync(mysqlContext, sqlServerContext, cancellationToken);
            await SyncUnvanlarAsync(mysqlContext, sqlServerContext, cancellationToken);
            await SyncHizmetBinalariAsync(mysqlContext, sqlServerContext, cancellationToken);
            await SyncDepartmanHizmetBinalariAsync(mysqlContext, sqlServerContext, cancellationToken);
            await SyncPersonellerAsync(mysqlContext, sqlServerContext, cancellationToken);

            _logger.LogInformation("✅ Sync tamamlandı");
        }

        private async Task SyncDepartmanlarAsync(MysqlDbContext mysql, SGKDbContext sqlServer, CancellationToken ct)
        {
            _logger.LogInformation("📦 Departmanlar sync ediliyor...");

            var legacyBirimler = await mysql.Birimler.AsNoTracking().ToListAsync(ct);
            var existingDepartmanlar = await sqlServer.Departmanlar.ToDictionaryAsync(d => d.DepartmanId, ct);

            int added = 0, updated = 0;

            foreach (var birim in legacyBirimler)
            {
                if (existingDepartmanlar.TryGetValue(birim.Kod, out var existing))
                {
                    if (existing.DepartmanAdi != birim.BirimAd || existing.DepartmanAdiKisa != birim.KisaAd)
                    {
                        existing.DepartmanAdi = birim.BirimAd;
                        existing.DepartmanAdiKisa = birim.KisaAd ?? birim.BirimAd;
                        existing.DuzenleyenKullanici = "SYNC";
                        existing.DuzenlenmeTarihi = DateTime.Now;
                        updated++;
                    }
                }
                else
                {
                    await sqlServer.Database.ExecuteSqlRawAsync(
                        "SET IDENTITY_INSERT PER_Departmanlar ON", ct);

                    sqlServer.Departmanlar.Add(new Departman
                    {
                        DepartmanId = birim.Kod,
                        DepartmanAdi = birim.BirimAd,
                        DepartmanAdiKisa = birim.KisaAd ?? birim.BirimAd,
                        Aktiflik = Aktiflik.Aktif,
                        EkleyenKullanici = "SYNC",
                        EklenmeTarihi = DateTime.Now,
                        DuzenleyenKullanici = "SYNC",
                        DuzenlenmeTarihi = DateTime.Now
                    });

                    await sqlServer.SaveChangesAsync(ct);

                    await sqlServer.Database.ExecuteSqlRawAsync(
                        "SET IDENTITY_INSERT PER_Departmanlar OFF", ct);

                    added++;
                }
            }

            await sqlServer.SaveChangesAsync(ct);
            _logger.LogInformation("  ✅ Departmanlar: {Added} eklendi, {Updated} güncellendi", added, updated);
        }

        private async Task SyncServislerAsync(MysqlDbContext mysql, SGKDbContext sqlServer, CancellationToken ct)
        {
            _logger.LogInformation("📦 Servisler sync ediliyor...");

            var legacyServisler = await mysql.Servisler.AsNoTracking().ToListAsync(ct);
            var existingServisler = await sqlServer.Servisler.ToDictionaryAsync(s => s.ServisId, ct);

            int added = 0, updated = 0;

            foreach (var servis in legacyServisler)
            {
                if (existingServisler.TryGetValue(servis.ServisId, out var existing))
                {
                    if (existing.ServisAdi != servis.ServisAdi)
                    {
                        existing.ServisAdi = servis.ServisAdi;
                        existing.DuzenleyenKullanici = "SYNC";
                        existing.DuzenlenmeTarihi = DateTime.Now;
                        updated++;
                    }
                }
                else
                {
                    await sqlServer.Database.ExecuteSqlRawAsync(
                        "SET IDENTITY_INSERT PER_Servisler ON", ct);

                    sqlServer.Servisler.Add(new Servis
                    {
                        ServisId = servis.ServisId,
                        ServisAdi = servis.ServisAdi,
                        Aktiflik = Aktiflik.Aktif,
                        EkleyenKullanici = "SYNC",
                        EklenmeTarihi = DateTime.Now,
                        DuzenleyenKullanici = "SYNC",
                        DuzenlenmeTarihi = DateTime.Now
                    });

                    await sqlServer.SaveChangesAsync(ct);

                    await sqlServer.Database.ExecuteSqlRawAsync(
                        "SET IDENTITY_INSERT PER_Servisler OFF", ct);

                    added++;
                }
            }

            await sqlServer.SaveChangesAsync(ct);
            _logger.LogInformation("  ✅ Servisler: {Added} eklendi, {Updated} güncellendi", added, updated);
        }

        private async Task SyncUnvanlarAsync(MysqlDbContext mysql, SGKDbContext sqlServer, CancellationToken ct)
        {
            _logger.LogInformation("📦 Ünvanlar sync ediliyor...");

            var legacyUnvanlar = await mysql.Unvanlar.AsNoTracking().ToListAsync(ct);
            var existingUnvanlar = await sqlServer.Unvanlar.ToDictionaryAsync(u => u.UnvanId, ct);

            int added = 0, updated = 0;

            foreach (var unvan in legacyUnvanlar)
            {
                if (existingUnvanlar.TryGetValue(unvan.Id, out var existing))
                {
                    if (existing.UnvanAdi != unvan.Unvan)
                    {
                        existing.UnvanAdi = unvan.Unvan;
                        existing.DuzenleyenKullanici = "SYNC";
                        existing.DuzenlenmeTarihi = DateTime.Now;
                        updated++;
                    }
                }
                else
                {
                    await sqlServer.Database.ExecuteSqlRawAsync(
                        "SET IDENTITY_INSERT PER_Unvanlar ON", ct);

                    sqlServer.Unvanlar.Add(new Unvan
                    {
                        UnvanId = unvan.Id,
                        UnvanAdi = unvan.Unvan,
                        Aktiflik = Aktiflik.Aktif,
                        EkleyenKullanici = "SYNC",
                        EklenmeTarihi = DateTime.Now,
                        DuzenleyenKullanici = "SYNC",
                        DuzenlenmeTarihi = DateTime.Now
                    });

                    await sqlServer.SaveChangesAsync(ct);

                    await sqlServer.Database.ExecuteSqlRawAsync(
                        "SET IDENTITY_INSERT PER_Unvanlar OFF", ct);

                    added++;
                }
            }

            await sqlServer.SaveChangesAsync(ct);
            _logger.LogInformation("  ✅ Ünvanlar: {Added} eklendi, {Updated} güncellendi", added, updated);
        }

        private async Task SyncHizmetBinalariAsync(MysqlDbContext mysql, SGKDbContext sqlServer, CancellationToken ct)
        {
            _logger.LogInformation("📦 Hizmet Binaları sync ediliyor...");

            var legacyBinalar = await mysql.Binalar.AsNoTracking().ToListAsync(ct);
            var existingBinalar = await sqlServer.HizmetBinalari.ToDictionaryAsync(h => h.LegacyKod ?? 0, ct);

            int added = 0, updated = 0;

            foreach (var bina in legacyBinalar)
            {
                if (existingBinalar.TryGetValue(bina.Kod, out var existing))
                {
                    // Güncelleme
                    if (existing.HizmetBinasiAdi != bina.BinaAdi || existing.Adres != bina.Adres)
                    {
                        existing.HizmetBinasiAdi = bina.BinaAdi;
                        existing.Adres = bina.Adres;
                        existing.DuzenleyenKullanici = "SYNC";
                        existing.DuzenlenmeTarihi = DateTime.Now;
                        updated++;
                    }
                }
                else
                {
                    // Yeni kayıt
                    sqlServer.HizmetBinalari.Add(new HizmetBinasi
                    {
                        HizmetBinasiAdi = bina.BinaAdi,
                        LegacyKod = bina.Kod,
                        Adres = bina.Adres,
                        Aktiflik = Aktiflik.Aktif,
                        EkleyenKullanici = "SYNC",
                        EklenmeTarihi = DateTime.Now,
                        DuzenleyenKullanici = "SYNC",
                        DuzenlenmeTarihi = DateTime.Now
                    });
                    added++;
                }
            }

            await sqlServer.SaveChangesAsync(ct);
            _logger.LogInformation("  ✅ Hizmet Binaları: {Added} eklendi, {Updated} güncellendi", added, updated);
        }

        private async Task SyncDepartmanHizmetBinalariAsync(MysqlDbContext mysql, SGKDbContext sqlServer, CancellationToken ct)
        {
            _logger.LogInformation("📦 Departman-Bina İlişkileri sync ediliyor...");

            // MySQL'den DISTINCT departman-bina eşleşmelerini al
            var departmanBinaEslesmeleri = await mysql.Kullanicilar
                .Where(k => k.Birim > 0 && k.Bina > 0)
                .Select(k => new { DepartmanId = k.Birim, BinaKod = k.Bina })
                .Distinct()
                .ToListAsync(ct);

            _logger.LogInformation("  📊 {Count} adet benzersiz departman-bina eşleşmesi bulundu", departmanBinaEslesmeleri.Count);

            // Mevcut HizmetBinasi ve Departman kayıtlarını al
            var hizmetBinalari = await sqlServer.HizmetBinalari
                .Where(h => h.LegacyKod.HasValue)
                .ToDictionaryAsync(h => h.LegacyKod!.Value, h => h.HizmetBinasiId, ct);

            var departmanlar = await sqlServer.Departmanlar
                .Select(d => d.DepartmanId)
                .ToListAsync(ct);

            // Mevcut ilişkileri al
            var mevcutIliskiler = await sqlServer.DepartmanHizmetBinalari
                .Where(dhb => !dhb.SilindiMi)
                .Select(dhb => new { dhb.DepartmanId, dhb.HizmetBinasiId })
                .ToListAsync(ct);

            var mevcutIliskilerSet = new HashSet<(int, int)>(
                mevcutIliskiler.Select(x => (x.DepartmanId, x.HizmetBinasiId))
            );

            int added = 0, skipped = 0;

            foreach (var eslesme in departmanBinaEslesmeleri)
            {
                // Departman ve Bina var mı kontrol et
                if (!departmanlar.Contains(eslesme.DepartmanId))
                {
                    skipped++;
                    continue;
                }

                if (!hizmetBinalari.TryGetValue(eslesme.BinaKod, out var hizmetBinasiId))
                {
                    skipped++;
                    continue;
                }

                // İlişki zaten var mı?
                if (mevcutIliskilerSet.Contains((eslesme.DepartmanId, hizmetBinasiId)))
                {
                    continue;
                }

                // Yeni ilişki ekle
                // İlk eşleşmeyi AnaBina olarak işaretle
                var anaBina = !await sqlServer.DepartmanHizmetBinalari
                    .AnyAsync(dhb => dhb.HizmetBinasiId == hizmetBinasiId && !dhb.SilindiMi, ct);

                sqlServer.DepartmanHizmetBinalari.Add(new DepartmanHizmetBinasi
                {
                    DepartmanId = eslesme.DepartmanId,
                    HizmetBinasiId = hizmetBinasiId,
                    AnaBina = anaBina,
                    EklenmeTarihi = DateTime.Now,
                    DuzenlenmeTarihi = DateTime.Now
                });

                added++;
            }

            await sqlServer.SaveChangesAsync(ct);
            _logger.LogInformation("  ✅ Departman-Bina İlişkileri: {Added} eklendi, {Skipped} atlandı", added, skipped);
        }

        private async Task SyncPersonellerAsync(MysqlDbContext mysql, SGKDbContext sqlServer, CancellationToken ct)
        {
            _logger.LogInformation("📦 Personeller sync ediliyor...");

            var legacyKullanicilar = await mysql.Kullanicilar
                .AsNoTracking()
                .Where(k => k.Username != null && k.Username > 10000000000) // Geçerli TC'ler
                .ToListAsync(ct);

            var existingPersoneller = await sqlServer.Personeller
                .ToDictionaryAsync(p => p.TcKimlikNo, ct);

            var existingUsers = await sqlServer.Users
                .ToDictionaryAsync(u => u.TcKimlikNo, ct);

            // Lookup tabloları
            var iller = await sqlServer.Iller.ToDictionaryAsync(i => i.IlAdi.ToUpperInvariant(), ct);
            var ilceler = await sqlServer.Ilceler.ToListAsync(ct);
            var departmanlar = await sqlServer.Departmanlar.Select(d => d.DepartmanId).ToListAsync(ct);
            var servisler = await sqlServer.Servisler.Select(s => s.ServisId).ToListAsync(ct);
            var unvanlar = await sqlServer.Unvanlar.Select(u => u.UnvanId).ToListAsync(ct);
            var hizmetBinalari = await sqlServer.HizmetBinalari.Select(h => h.HizmetBinasiId).ToListAsync(ct);

            int added = 0, updated = 0, skipped = 0;

            foreach (var kullanici in legacyKullanicilar)
            {
                var tcKimlikNo = kullanici.Username?.ToString() ?? "";
                if (string.IsNullOrEmpty(tcKimlikNo) || tcKimlikNo.Length != 11)
                {
                    skipped++;
                    continue;
                }

                // FK validasyonları
                var departmanId = departmanlar.Contains(kullanici.Birim) ? kullanici.Birim : 1;
                var hizmetBinasiId = hizmetBinalari.Contains(departmanId) ? departmanId : 1;
                var servisId = int.TryParse(kullanici.Servis, out var sId) && servisler.Contains(sId) ? sId : 1;
                var unvanId = int.TryParse(kullanici.Unvan, out var uId) && unvanlar.Contains(uId) ? uId : 1;

                // İl/İlçe lookup
                var ilId = 35; // Default İzmir
                var ilceId = 1;
                if (!string.IsNullOrEmpty(kullanici.Il) && iller.TryGetValue(kullanici.Il.ToUpperInvariant(), out var il))
                {
                    ilId = il.IlId;
                    if (!string.IsNullOrEmpty(kullanici.Ilce))
                    {
                        var ilce = ilceler.FirstOrDefault(i => i.IlId == ilId && 
                            i.IlceAdi.ToUpperInvariant() == kullanici.Ilce.ToUpperInvariant());
                        if (ilce != null) ilceId = ilce.IlceId;
                        else ilceId = ilceler.FirstOrDefault(i => i.IlId == ilId)?.IlceId ?? 1;
                    }
                }

                if (existingPersoneller.TryGetValue(tcKimlikNo, out var existingPersonel))
                {
                    // Güncelleme
                    var hasChanges = UpdatePersonelFromLegacy(existingPersonel, kullanici, departmanId, servisId, unvanId, hizmetBinasiId, ilId, ilceId);
                    if (hasChanges)
                    {
                        existingPersonel.DuzenleyenKullanici = "SYNC";
                        existingPersonel.DuzenlenmeTarihi = DateTime.Now;
                        updated++;
                    }
                }
                else
                {
                    // Yeni kayıt
                    var personel = CreatePersonelFromLegacy(kullanici, tcKimlikNo, departmanId, servisId, unvanId, hizmetBinasiId, ilId, ilceId);
                    sqlServer.Personeller.Add(personel);
                    added++;

                    // User da ekle
                    if (!existingUsers.ContainsKey(tcKimlikNo))
                    {
                        sqlServer.Users.Add(new User
                        {
                            TcKimlikNo = tcKimlikNo,
                            UserType = UserType.Personel,
                            PassWord = kullanici.Password ?? tcKimlikNo,
                            AktifMi = kullanici.CalisanDurum == 1,
                            EkleyenKullanici = "SYNC",
                            EklenmeTarihi = DateTime.Now,
                            DuzenleyenKullanici = "SYNC",
                            DuzenlenmeTarihi = DateTime.Now
                        });
                    }
                }
            }

            await sqlServer.SaveChangesAsync(ct);
            _logger.LogInformation("  ✅ Personeller: {Added} eklendi, {Updated} güncellendi, {Skipped} atlandı", added, updated, skipped);
        }

        private Personel CreatePersonelFromLegacy(LegacyKullanici k, string tcKimlikNo, int departmanId, int servisId, int unvanId, int hizmetBinasiId, int ilId, int ilceId)
        {
            return new Personel
            {
                TcKimlikNo = tcKimlikNo,
                SicilNo = k.SicilNo,
                AdSoyad = k.KullaniciAdi,
                NickName = GenerateNickName(k.KullaniciAdi),
                PersonelKayitNo = 0,
                KartNo = int.TryParse(k.KartNo, out var kartNo) ? kartNo : 0,
                KartNoAktiflikTarihi = k.KartAktifTarihi,
                KartNoDuzenlenmeTarihi = k.KartGuncellemeZamani,
                KartTipi = CardType.PersonelKarti,
                KartGonderimIslemBasari = IslemBasari.Basarisiz,
                DepartmanId = departmanId,
                ServisId = servisId,
                UnvanId = unvanId,
                AtanmaNedeniId = k.Atanma ?? 1,
                HizmetBinasiId = hizmetBinasiId,
                IlId = ilId,
                IlceId = ilceId,
                SendikaId = k.Sendika > 0 ? k.Sendika : null,
                Gorev = k.Gorev,
                PersonelTipi = PersonelTipi.memur,
                Email = k.Email ?? "",
                Dahili = (int)k.Dahili,
                CepTelefonu = k.CepTel > 0 ? k.CepTel.ToString() : null,
                CepTelefonu2 = k.CepTel2 > 0 ? k.CepTel2.ToString() : null,
                EvTelefonu = k.EvTel > 0 ? k.EvTel.ToString() : null,
                Adres = k.Adres,
                Semt = k.Semt,
                DogumTarihi = ParseDogumTarihi(k.DogumTarihi),
                Cinsiyet = k.Cinsiyet == "1" ? Cinsiyet.kadin : Cinsiyet.erkek,
                MedeniDurumu = k.MedeniDurum == 1 ? MedeniDurumu.evli : MedeniDurumu.bekar,
                KanGrubu = ParseKanGrubu(k.Kan),
                EvDurumu = (EvDurumu)Math.Min(k.EvDurum, 2),
                UlasimServis1 = k.Servis1,
                UlasimServis2 = k.Servis2,
                Tabldot = k.Tabldot ?? 0,
                PersonelAktiflikDurum = MapCalisanDurum(k.CalisanDurum),
                EmekliSicilNo = k.EmekliSicilNo,
                OgrenimDurumu = OgrenimDurumu.ilkokul,
                BitirdigiOkul = k.BitirdigiOkul,
                BitirdigiBolum = k.BitirdigiBolum,
                OgrenimSuresi = k.OgrenimSuresi ?? 0,
                Bransi = k.Brans,
                SehitYakinligi = k.Sehit == 1 ? SehitYakinligi.babasi : SehitYakinligi.yok,
                EsininAdi = k.EsAdi,
                EsininIsDurumu = MapEsDurumu(k.EsDurumu),
                EsininUnvani = k.EsIsUnvan,
                EsininIsAdresi = k.EsIsAdres,
                EsininIsSemt = k.EsSemt,
                Resim = k.ResimYolu,
                // Aktiflik AuditableEntity'den geliyor, Personel'de yok
                EkleyenKullanici = "SYNC",
                EklenmeTarihi = DateTime.Now,
                DuzenleyenKullanici = "SYNC",
                DuzenlenmeTarihi = DateTime.Now
            };
        }

        private bool UpdatePersonelFromLegacy(Personel p, LegacyKullanici k, int departmanId, int servisId, int unvanId, int hizmetBinasiId, int ilId, int ilceId)
        {
            bool hasChanges = false;

            if (p.AdSoyad != k.KullaniciAdi) { p.AdSoyad = k.KullaniciAdi; hasChanges = true; }
            if (p.SicilNo != k.SicilNo) { p.SicilNo = k.SicilNo; hasChanges = true; }
            if (p.DepartmanId != departmanId) { p.DepartmanId = departmanId; hasChanges = true; }
            if (p.ServisId != servisId) { p.ServisId = servisId; hasChanges = true; }
            if (p.UnvanId != unvanId) { p.UnvanId = unvanId; hasChanges = true; }
            if (p.HizmetBinasiId != hizmetBinasiId) { p.HizmetBinasiId = hizmetBinasiId; hasChanges = true; }
            if (p.Email != (k.Email ?? "")) { p.Email = k.Email ?? ""; hasChanges = true; }
            if (p.Dahili != (int)k.Dahili) { p.Dahili = (int)k.Dahili; hasChanges = true; }

            var newKartNo = int.TryParse(k.KartNo, out var kn) ? kn : 0;
            if (p.KartNo != newKartNo) { p.KartNo = newKartNo; hasChanges = true; }

            var newAktiflik = MapCalisanDurum(k.CalisanDurum);
            if (p.PersonelAktiflikDurum != newAktiflik) { p.PersonelAktiflikDurum = newAktiflik; hasChanges = true; }

            return hasChanges;
        }

        private string GenerateNickName(string adSoyad)
        {
            if (string.IsNullOrEmpty(adSoyad)) return "USER";
            var parts = adSoyad.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var nick = parts[0].ToUpperInvariant()
                .Replace("İ", "I").Replace("Ğ", "G").Replace("Ü", "U")
                .Replace("Ş", "S").Replace("Ö", "O").Replace("Ç", "C");
            return nick.Length > 8 ? nick[..8] : nick;
        }

        private DateTime ParseDogumTarihi(string? dogumTarihi)
        {
            if (string.IsNullOrEmpty(dogumTarihi)) return new DateTime(1970, 1, 1);
            if (DateTime.TryParse(dogumTarihi, out var dt)) return dt;
            return new DateTime(1970, 1, 1);
        }

        private KanGrubu ParseKanGrubu(string? kan)
        {
            if (string.IsNullOrEmpty(kan)) return KanGrubu.sifir_arti;
            kan = kan.ToUpperInvariant().Replace(" ", "");
            return kan switch
            {
                "0" or "0RH(+)" or "0+" => KanGrubu.sifir_arti,
                "0RH(-)" or "0-" => KanGrubu.sifir_eksi,
                "ARH(+)" or "A+" => KanGrubu.a_arti,
                "ARH(-)" or "A-" => KanGrubu.a_eksi,
                "BRH(+)" or "B+" => KanGrubu.b_arti,
                "BRH(-)" or "B-" => KanGrubu.b_eksi,
                "ABRH(+)" or "AB+" => KanGrubu.ab_arti,
                "ABRH(-)" or "AB-" => KanGrubu.ab_eksi,
                _ => KanGrubu.sifir_arti
            };
        }

        private PersonelAktiflikDurum MapCalisanDurum(int? calisanDurum)
        {
            return calisanDurum switch
            {
                1 => PersonelAktiflikDurum.Aktif,
                2 => PersonelAktiflikDurum.Pasif,
                3 => PersonelAktiflikDurum.Emekli,
                _ => PersonelAktiflikDurum.Aktif
            };
        }

        private EsininIsDurumu MapEsDurumu(int esDurumu)
        {
            return esDurumu switch
            {
                1 => EsininIsDurumu.emekli,
                2 => EsininIsDurumu.calismiyor,
                3 => EsininIsDurumu.evhanimi,
                4 => EsininIsDurumu.ozel,
                5 => EsininIsDurumu.kamu,
                6 => EsininIsDurumu.kamu,
                _ => EsininIsDurumu.belirtilmemis
            };
        }
    }
}
