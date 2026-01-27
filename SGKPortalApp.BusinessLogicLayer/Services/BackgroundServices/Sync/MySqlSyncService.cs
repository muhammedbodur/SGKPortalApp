using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.BackgroundServiceManager;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Legacy;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.Common.Helpers;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Legacy;

namespace SGKPortalApp.BusinessLogicLayer.Services.BackgroundServices.Sync
{
    public class MySqlSyncService : BackgroundService, IManagedBackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MySqlSyncService> _logger;
        private TimeSpan _syncInterval = TimeSpan.FromMinutes(120);
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
            
            // Repository'leri al
            var legacyBirimRepo = scope.ServiceProvider.GetRequiredService<ILegacyBirimRepository>();
            var legacyServisRepo = scope.ServiceProvider.GetRequiredService<ILegacyServisRepository>();
            var legacyKullaniciRepo = scope.ServiceProvider.GetRequiredService<ILegacyKullaniciRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            try { await SyncDepartmanlarAsync(legacyBirimRepo, unitOfWork, cancellationToken); }
            catch (Exception ex) { _logger.LogError(ex, "❌ SyncDepartmanlarAsync hatası"); }

            try { await SyncServislerAsync(legacyServisRepo, unitOfWork, cancellationToken); }
            catch (Exception ex) { _logger.LogError(ex, "❌ SyncServislerAsync hatası"); }

            var legacyUnvanRepo = scope.ServiceProvider.GetRequiredService<ILegacyUnvanRepository>();
            try { await SyncUnvanlarAsync(legacyUnvanRepo, unitOfWork, cancellationToken); }
            catch (Exception ex) { _logger.LogError(ex, "❌ SyncUnvanlarAsync hatası"); }

            var legacyBinaRepo = scope.ServiceProvider.GetRequiredService<ILegacyBinaRepository>();
            try { await SyncHizmetBinalariAsync(legacyBinaRepo, unitOfWork, cancellationToken); }
            catch (Exception ex) { _logger.LogError(ex, "❌ SyncHizmetBinalariAsync hatası"); }

            var legacyAtanmaRepo = scope.ServiceProvider.GetRequiredService<ILegacyAtanmaRepository>();
            try { await SyncAtanmaNedenleriAsync(legacyAtanmaRepo, unitOfWork, cancellationToken); }
            catch (Exception ex) { _logger.LogError(ex, "❌ SyncAtanmaNedenleriAsync hatası"); }

            try { await SyncDepartmanHizmetBinalariAsync(legacyKullaniciRepo, unitOfWork, cancellationToken); }
            catch (Exception ex) { _logger.LogError(ex, "❌ SyncDepartmanHizmetBinalariAsync hatası"); }

            try { await SyncPersonellerAsync(legacyKullaniciRepo, unitOfWork, cancellationToken); }
            catch (Exception ex) { _logger.LogError(ex, "❌ SyncPersonellerAsync hatası"); }

            _logger.LogInformation("✅ Sync tamamlandı");
        }

        private async Task SyncDepartmanlarAsync(ILegacyBirimRepository legacyRepo, IUnitOfWork unitOfWork, CancellationToken ct)
        {
            _logger.LogInformation("📦 Departmanlar sync ediliyor...");

            // Legacy MySQL'den oku (Repository üzerinden)
            var legacyBirimler = await legacyRepo.GetAllAsync(ct);

            // SQL Server'dan oku (UnitOfWork üzerinden)
            var existingDepartmanlar = await unitOfWork.Repository<Departman>().GetAllAsync();

            // LegacyKod ile eşleştirme dictionary'si
            var departmanByLegacyKod = existingDepartmanlar
                .Where(d => d.LegacyKod.HasValue)
                .ToDictionary(d => d.LegacyKod!.Value);

            int added = 0, updated = 0;

            foreach (var birim in legacyBirimler)
            {
                // LegacyKod ile eşleştir
                if (departmanByLegacyKod.TryGetValue(birim.Kod, out var existing))
                {
                    // Mevcut kayıt - güncelle
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
                    // Yeni kayıt - IDENTITY_INSERT ile ekle
                    var now = DateTime.Now;
                    await unitOfWork.ExecuteSqlInterpolatedAsync(
                        $@"SET IDENTITY_INSERT [dbo].[PER_Departmanlar] ON;
                        INSERT INTO [dbo].[PER_Departmanlar]
                            ([DepartmanId], [DepartmanAdi], [DepartmanAdiKisa], [LegacyKod], [Aktiflik],
                             [EkleyenKullanici], [EklenmeTarihi], [DuzenleyenKullanici], [DuzenlenmeTarihi], [SilindiMi])
                        VALUES
                            ({birim.Kod}, {birim.BirimAd}, {birim.KisaAd ?? birim.BirimAd}, {birim.Kod}, {(int)Aktiflik.Aktif},
                             {"SYNC"}, {now}, {"SYNC"}, {now}, {false});
                        SET IDENTITY_INSERT [dbo].[PER_Departmanlar] OFF;", ct);

                    added++;
                }
            }

            await unitOfWork.SaveChangesAsync();
            _logger.LogInformation("  ✅ Departmanlar: {Added} eklendi, {Updated} güncellendi", added, updated);
        }

        private async Task SyncServislerAsync(ILegacyServisRepository legacyRepo, IUnitOfWork unitOfWork, CancellationToken ct)
        {
            _logger.LogInformation("📦 Servisler sync ediliyor...");

            var legacyServisler = await legacyRepo.GetAllAsync(ct);
            var existingServisler = await unitOfWork.Repository<Servis>().GetAllAsync();

            // LegacyKod ile eşleştirme dictionary'si
            var servisByLegacyKod = existingServisler
                .Where(s => s.LegacyKod.HasValue)
                .ToDictionary(s => s.LegacyKod!.Value);

            int added = 0, updated = 0;

            foreach (var servis in legacyServisler)
            {
                // LegacyKod ile eşleştir
                if (servisByLegacyKod.TryGetValue(servis.ServisId, out var existing))
                {
                    // Mevcut kayıt - güncelle
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
                    // Yeni kayıt - IDENTITY_INSERT ile ekle
                    var now = DateTime.Now;
                    await unitOfWork.ExecuteSqlInterpolatedAsync(
                        $@"SET IDENTITY_INSERT [dbo].[PER_Servisler] ON;
                        INSERT INTO [dbo].[PER_Servisler]
                            ([ServisId], [ServisAdi], [LegacyKod], [Aktiflik],
                             [EkleyenKullanici], [EklenmeTarihi], [DuzenleyenKullanici], [DuzenlenmeTarihi], [SilindiMi])
                        VALUES
                            ({servis.ServisId}, {servis.ServisAdi}, {servis.ServisId}, {(int)Aktiflik.Aktif},
                             {"SYNC"}, {now}, {"SYNC"}, {now}, {false});
                        SET IDENTITY_INSERT [dbo].[PER_Servisler] OFF;", ct);

                    added++;
                }
            }

            await unitOfWork.SaveChangesAsync();
            _logger.LogInformation("  ✅ Servisler: {Added} eklendi, {Updated} güncellendi", added, updated);
        }

        private async Task SyncUnvanlarAsync(ILegacyUnvanRepository legacyRepo, IUnitOfWork unitOfWork, CancellationToken ct)
        {
            _logger.LogInformation("📦 Ünvanlar sync ediliyor...");

            var legacyUnvanlar = await legacyRepo.GetAllAsync(ct);
            var existingUnvanlar = await unitOfWork.Repository<Unvan>().GetAllAsync();

            // LegacyKod ile eşleştirme dictionary'si
            var unvanByLegacyKod = existingUnvanlar
                .Where(u => u.LegacyKod.HasValue)
                .ToDictionary(u => u.LegacyKod!.Value);

            int added = 0, updated = 0;

            foreach (var unvan in legacyUnvanlar)
            {
                // LegacyKod ile eşleştir
                if (unvanByLegacyKod.TryGetValue(unvan.Id, out var existing))
                {
                    // Mevcut kayıt - güncelle
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
                    // Yeni kayıt - IDENTITY_INSERT ile ekle
                    var now = DateTime.Now;
                    await unitOfWork.ExecuteSqlInterpolatedAsync(
                        $@"SET IDENTITY_INSERT [dbo].[PER_Unvanlar] ON;
                        INSERT INTO [dbo].[PER_Unvanlar]
                            ([UnvanId], [UnvanAdi], [LegacyKod], [Aktiflik],
                             [EkleyenKullanici], [EklenmeTarihi], [DuzenleyenKullanici], [DuzenlenmeTarihi], [SilindiMi])
                        VALUES
                            ({unvan.Id}, {unvan.Unvan}, {unvan.Id}, {(int)Aktiflik.Aktif},
                             {"SYNC"}, {now}, {"SYNC"}, {now}, {false});
                        SET IDENTITY_INSERT [dbo].[PER_Unvanlar] OFF;", ct);

                    added++;
                }
            }

            await unitOfWork.SaveChangesAsync();
            _logger.LogInformation("  ✅ Ünvanlar: {Added} eklendi, {Updated} güncellendi", added, updated);
        }

        private async Task SyncHizmetBinalariAsync(ILegacyBinaRepository legacyRepo, IUnitOfWork unitOfWork, CancellationToken ct)
        {
            _logger.LogInformation("📦 Hizmet Binaları sync ediliyor...");

            var legacyBinalar = await legacyRepo.GetAllAsync(ct);
            var existingBinalar = await unitOfWork.Repository<HizmetBinasi>().GetAllAsync();
            var binaByLegacyKod = existingBinalar.Where(h => h.LegacyKod.HasValue).ToDictionary(h => h.LegacyKod!.Value);
            var binaByAdi = existingBinalar.ToDictionary(h => h.HizmetBinasiAdi.ToUpperInvariant());

            int added = 0, updated = 0, skipped = 0;

            foreach (var bina in legacyBinalar)
            {
                // Önce LegacyKod ile kontrol et
                if (binaByLegacyKod.TryGetValue(bina.Kod, out var existing))
                {
                    // Güncelleme
                    if (existing.HizmetBinasiAdi != bina.BinaAdi)
                    {
                        existing.HizmetBinasiAdi = bina.BinaAdi;
                        existing.DuzenleyenKullanici = "SYNC";
                        existing.DuzenlenmeTarihi = DateTime.Now;
                        updated++;
                    }
                }
                // Aynı isimde bina var mı kontrol et (unique constraint)
                else if (binaByAdi.ContainsKey(bina.BinaAdi.ToUpperInvariant()))
                {
                    // Aynı isimde bina var, LegacyKod'u güncelle
                    var existingByName = binaByAdi[bina.BinaAdi.ToUpperInvariant()];
                    if (!existingByName.LegacyKod.HasValue)
                    {
                        existingByName.LegacyKod = bina.Kod;
                        existingByName.DuzenleyenKullanici = "SYNC";
                        existingByName.DuzenlenmeTarihi = DateTime.Now;
                        updated++;
                    }
                    else
                    {
                        skipped++;
                    }
                }
                else
                {
                    // Yeni kayıt
                    var newBina = new HizmetBinasi
                    {
                        HizmetBinasiAdi = bina.BinaAdi,
                        LegacyKod = bina.Kod,
                        Aktiflik = Aktiflik.Aktif,
                        EkleyenKullanici = "SYNC",
                        EklenmeTarihi = DateTime.Now,
                        DuzenleyenKullanici = "SYNC",
                        DuzenlenmeTarihi = DateTime.Now
                    };
                    await unitOfWork.Repository<HizmetBinasi>().AddAsync(newBina);
                    await unitOfWork.SaveChangesAsync(); // Hemen kaydet - duplicate key hatasını önle
                    
                    // Dictionary'ye ekle
                    binaByLegacyKod[bina.Kod] = newBina;
                    binaByAdi[bina.BinaAdi.ToUpperInvariant()] = newBina;
                    
                    added++;
                }
            }

            _logger.LogInformation("  ✅ Hizmet Binaları: {Added} eklendi, {Updated} güncellendi, {Skipped} atlandı", added, updated, skipped);
        }

        private async Task SyncDepartmanHizmetBinalariAsync(ILegacyKullaniciRepository legacyRepo, IUnitOfWork unitOfWork, CancellationToken ct)
        {
            _logger.LogInformation("📦 Departman-Bina İlişkileri sync ediliyor...");

            // Legacy'den tüm kullanıcıları al ve DISTINCT departman-bina eşleşmelerini bul
            var legacyKullanicilar = await legacyRepo.GetAllActiveAsync(ct);
            var departmanBinaEslesmeleri = legacyKullanicilar
                .Where(k => k.Birim > 0 && k.Bina > 0)
                .Select(k => new { DepartmanId = k.Birim, BinaKod = k.Bina })
                .Distinct()
                .ToList();

            _logger.LogInformation("  📊 {Count} adet benzersiz departman-bina eşleşmesi bulundu", departmanBinaEslesmeleri.Count);

            // Mevcut HizmetBinasi ve Departman kayıtlarını al
            var hizmetBinalari = await unitOfWork.Repository<HizmetBinasi>().GetAllAsync();
            var hizmetBinaDictByLegacy = hizmetBinalari
                .Where(h => h.LegacyKod.HasValue)
                .ToDictionary(h => h.LegacyKod!.Value, h => h.HizmetBinasiId);

            var departmanlar = await unitOfWork.Repository<Departman>().GetAllAsync();
            var departmanIds = departmanlar.Select(d => d.DepartmanId).ToList();

            // Mevcut ilişkileri al
            var mevcutIliskiler = await unitOfWork.Repository<DepartmanHizmetBinasi>().GetAllAsync();
            var mevcutIliskilerAktif = mevcutIliskiler
                .Where(dhb => !dhb.SilindiMi)
                .Select(dhb => new { dhb.DepartmanId, dhb.HizmetBinasiId })
                .ToList();

            var mevcutIliskilerSet = new HashSet<(int, int)>(
                mevcutIliskiler.Select(x => (x.DepartmanId, x.HizmetBinasiId))
            );

            int added = 0, skipped = 0;

            foreach (var eslesme in departmanBinaEslesmeleri)
            {
                // Departman ve Bina var mı kontrol et
                if (!departmanIds.Contains(eslesme.DepartmanId))
                {
                    skipped++;
                    continue;
                }

                if (!hizmetBinaDictByLegacy.TryGetValue(eslesme.BinaKod, out var hizmetBinasiId))
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
                var anaBina = !mevcutIliskilerAktif.Any(x => x.HizmetBinasiId == hizmetBinasiId);

                await unitOfWork.Repository<DepartmanHizmetBinasi>().AddAsync(new DepartmanHizmetBinasi
                {
                    DepartmanId = eslesme.DepartmanId,
                    HizmetBinasiId = hizmetBinasiId,
                    AnaBina = anaBina,
                    EklenmeTarihi = DateTime.Now,
                    DuzenlenmeTarihi = DateTime.Now
                });

                added++;
            }

            await unitOfWork.SaveChangesAsync();
            _logger.LogInformation("  ✅ Departman-Bina İlişkileri: {Added} eklendi, {Skipped} atlandı", added, skipped);
        }

        private async Task SyncPersonellerAsync(ILegacyKullaniciRepository legacyRepo, IUnitOfWork unitOfWork, CancellationToken ct)
        {
            _logger.LogInformation("📦 Personeller sync ediliyor...");

            // Legacy MySQL'den oku (Repository üzerinden)
            var legacyKullanicilar = await legacyRepo.GetAllActiveAsync(ct);

            // SQL Server'dan oku (UnitOfWork üzerinden)
            var existingPersoneller = await unitOfWork.Repository<Personel>().GetAllAsync();
            var personelDict = existingPersoneller.ToDictionary(p => p.TcKimlikNo);

            var existingUsers = await unitOfWork.Repository<User>().GetAllAsync();
            var userDict = existingUsers.ToDictionary(u => u.TcKimlikNo);

            // Lookup tabloları
            var iller = await unitOfWork.Repository<Il>().GetAllAsync();
            var ilDict = iller.ToDictionary(i => i.IlAdi.ToUpperInvariant());
            var ilceler = await unitOfWork.Repository<Ilce>().GetAllAsync();
            
            var departmanlar = await unitOfWork.Repository<Departman>().GetAllAsync();
            var departmanByLegacyKod = departmanlar
                .Where(d => d.LegacyKod.HasValue)
                .ToDictionary(d => d.LegacyKod!.Value, d => d.DepartmanId);
            
            var servisler = await unitOfWork.Repository<Servis>().GetAllAsync();
            var servisByLegacyKod = servisler
                .Where(s => s.LegacyKod.HasValue)
                .ToDictionary(s => s.LegacyKod!.Value, s => s.ServisId);
            
            var unvanlar = await unitOfWork.Repository<Unvan>().GetAllAsync();
            var unvanByLegacyKod = unvanlar
                .Where(u => u.LegacyKod.HasValue)
                .ToDictionary(u => u.LegacyKod!.Value, u => u.UnvanId);
            
            var hizmetBinalari = await unitOfWork.Repository<HizmetBinasi>().GetAllAsync();
            var hizmetBinasiByLegacyKod = hizmetBinalari
                .Where(h => h.LegacyKod.HasValue)
                .ToDictionary(h => h.LegacyKod!.Value, h => h.HizmetBinasiId);

            int added = 0, updated = 0, skipped = 0;

            foreach (var kullanici in legacyKullanicilar)
            {
                var tcKimlikNo = kullanici.Username?.ToString() ?? "";
                if (string.IsNullOrEmpty(tcKimlikNo) || tcKimlikNo.Length != 11)
                {
                    skipped++;
                    continue;
                }

                // FK validasyonları - LegacyKod kullanarak doğru ID'leri bul
                var departmanId = departmanByLegacyKod.TryGetValue(kullanici.Birim, out var dId) ? dId : 1;
                var hizmetBinasiId = hizmetBinasiByLegacyKod.TryGetValue(kullanici.Bina, out var hbId) ? hbId : 1;
                var servisId = int.TryParse(kullanici.Servis, out var sId) && servisByLegacyKod.TryGetValue(sId, out var servId) ? servId : 1;
                var unvanId = int.TryParse(kullanici.Unvan, out var uId) && unvanByLegacyKod.TryGetValue(uId, out var unvId) ? unvId : 1;

                // İl/İlçe lookup
                var ilId = 35; // Default İzmir
                var ilceId = 1;
                if (!string.IsNullOrEmpty(kullanici.Il) && ilDict.TryGetValue(kullanici.Il.ToUpperInvariant(), out var il))
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

                try
                {
                    if (personelDict.TryGetValue(tcKimlikNo, out var existingPersonel))
                    {
                        // Güncelleme
                        var hasChanges = await UpdatePersonelFromLegacyAsync(existingPersonel, kullanici, departmanId, servisId, unvanId, hizmetBinasiId, ilId, ilceId, unitOfWork, ct);
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
                        var personel = await CreatePersonelFromLegacyAsync(kullanici, tcKimlikNo, departmanId, servisId, unvanId, hizmetBinasiId, ilId, ilceId, unitOfWork, ct);
                        await unitOfWork.Repository<Personel>().AddAsync(personel);
                        added++;

                        // User da ekle
                        if (!userDict.ContainsKey(tcKimlikNo))
                        {
                            await unitOfWork.Repository<User>().AddAsync(new User
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

                    await unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Personel sync hatası - TC: {TC}, SicilNo: {SicilNo}, AdSoyad: {AdSoyad}", 
                        tcKimlikNo, kullanici.SicilNo, kullanici.KullaniciAdi);
                    skipped++;
                }
            }
            _logger.LogInformation("  ✅ Personeller: {Added} eklendi, {Updated} güncellendi, {Skipped} atlandı", added, updated, skipped);
        }

        private async Task<Personel> CreatePersonelFromLegacyAsync(LegacyKullanici k, string tcKimlikNo, int departmanId, int servisId, int unvanId, int hizmetBinasiId, int ilId, int ilceId, IUnitOfWork unitOfWork, CancellationToken ct)
        {
            // DepartmanHizmetBinasiId'yi hesapla
            var allDhb = await unitOfWork.Repository<DepartmanHizmetBinasi>().GetAllAsync();
            var departmanHizmetBinasi = allDhb.FirstOrDefault(dhb => dhb.DepartmanId == departmanId && dhb.HizmetBinasiId == hizmetBinasiId && !dhb.SilindiMi);

            // Eğer DepartmanHizmetBinasi yoksa, otomatik oluştur
            if (departmanHizmetBinasi == null)
            {
                _logger.LogInformation("DepartmanHizmetBinasi otomatik oluşturuluyor. DepartmanId: {DepartmanId}, HizmetBinasiId: {HizmetBinasiId}", departmanId, hizmetBinasiId);

                departmanHizmetBinasi = new DepartmanHizmetBinasi
                {
                    DepartmanId = departmanId,
                    HizmetBinasiId = hizmetBinasiId,
                    AnaBina = false,
                    EklenmeTarihi = DateTime.Now,
                    DuzenlenmeTarihi = DateTime.Now
                };

                await unitOfWork.Repository<DepartmanHizmetBinasi>().AddAsync(departmanHizmetBinasi);
                await unitOfWork.SaveChangesAsync();
            }

            // AtanmaNedeniId validasyonu (LegacyKod ile mapping)
            var requestedLegacyKod = k.Atanma ?? 1;
            var allAtanmaNedenleri = await unitOfWork.Repository<AtanmaNedenleri>().GetAllAsync();
            var atanmaNedeni = allAtanmaNedenleri.FirstOrDefault(a => a.LegacyKod == requestedLegacyKod);

            int validAtanmaNedeniId;
            if (atanmaNedeni != null)
            {
                validAtanmaNedeniId = atanmaNedeni.AtanmaNedeniId;
            }
            else
            {
                // LegacyKod ile eşleşen bulunamadı - ilk mevcut AtanmaNedeni'ni kullan veya yoksa oluştur
                var firstAtanmaNedeni = allAtanmaNedenleri.FirstOrDefault();
                if (firstAtanmaNedeni != null)
                {
                    validAtanmaNedeniId = firstAtanmaNedeni.AtanmaNedeniId;
                    _logger.LogWarning("LegacyKod {LegacyKod} ile eşleşen AtanmaNedeni bulunamadı, {UsedId} kullanılıyor. TC: {Tc}", requestedLegacyKod, validAtanmaNedeniId, tcKimlikNo);
                }
                else
                {
                    // Tablo boş - varsayılan kayıt oluştur
                    var newAtanmaNedeni = new AtanmaNedenleri
                    {
                        AtanmaNedeni = "Belirtilmemiş",
                        LegacyKod = null,
                        EklenmeTarihi = DateTime.Now,
                        DuzenlenmeTarihi = DateTime.Now
                    };
                    await unitOfWork.Repository<AtanmaNedenleri>().AddAsync(newAtanmaNedeni);
                    await unitOfWork.SaveChangesAsync();
                    validAtanmaNedeniId = newAtanmaNedeni.AtanmaNedeniId;
                    _logger.LogInformation("Varsayılan AtanmaNedeni oluşturuldu: {Id}", validAtanmaNedeniId);
                }
            }

            // SendikaId validasyonu (LegacyKod ile mapping)
            int? validSendikaId = null;
            if (k.Sendika.HasValue)
            {
                var allSendikalar = await unitOfWork.Repository<Sendika>().GetAllAsync();
                var sendika = allSendikalar.FirstOrDefault(s => s.LegacyKod == k.Sendika.Value);
                
                if (sendika != null)
                {
                    validSendikaId = sendika.SendikaId;
                }
                else
                {
                    _logger.LogWarning("LegacyKod {LegacyKod} ile eşleşen sendika bulunamadı, null olarak set ediliyor. TC: {Tc}", k.Sendika.Value, tcKimlikNo);
                }
            }

            return new Personel
            {
                TcKimlikNo = tcKimlikNo,
                SicilNo = k.SicilNo > 0 ? k.SicilNo : null,
                AdSoyad = k.KullaniciAdi,
                NickName = StringHelper.GenerateNickName(k.KullaniciAdi, 8),
                PersonelKayitNo = null,
                KartNo = int.TryParse(k.KartNo, out var kartNo) ? kartNo : 0,
                KartNoAktiflikTarihi = k.KartAktifTarihi,
                KartNoDuzenlenmeTarihi = k.KartGuncellemeZamani,
                KartTipi = CardType.PersonelKarti,
                KartGonderimIslemBasari = IslemBasari.Basarisiz,
                DepartmanId = departmanId,
                ServisId = servisId,
                UnvanId = unvanId,
                AtanmaNedeniId = validAtanmaNedeniId,
                DepartmanHizmetBinasiId = departmanHizmetBinasi.DepartmanHizmetBinasiId,
                DepartmanHizmetBinasi = null!, // required property - FK yeterli, EF Core ilişkiyi yönetir
                IlId = ilId,
                IlceId = ilceId,
                SendikaId = validSendikaId,
                Gorev = k.Gorev,
                Uzmanlik = k.Brans, // Brans alanı Uzmanlik olarak mapping
                PersonelTipi = PersonelTipi.memur,
                Email = string.IsNullOrWhiteSpace(k.Email) ? null : k.Email,
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
                EmekliSicilNo = ParseEmekliSicilNo(k.EmekliSicilNo),
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

        private async Task<bool> UpdatePersonelFromLegacyAsync(Personel p, LegacyKullanici k, int departmanId, int servisId, int unvanId, int hizmetBinasiId, int ilId, int ilceId, IUnitOfWork unitOfWork, CancellationToken ct)
        {
            bool hasChanges = false;

            if (p.AdSoyad != k.KullaniciAdi) { p.AdSoyad = k.KullaniciAdi; hasChanges = true; }
            var newSicilNo = k.SicilNo > 0 ? k.SicilNo : (int?)null;
            if (p.SicilNo != newSicilNo) { p.SicilNo = newSicilNo; hasChanges = true; }
            if (p.DepartmanId != departmanId) { p.DepartmanId = departmanId; hasChanges = true; }
            if (p.ServisId != servisId) { p.ServisId = servisId; hasChanges = true; }
            if (p.UnvanId != unvanId) { p.UnvanId = unvanId; hasChanges = true; }
            
            // DepartmanHizmetBinasiId kontrolü
            var allDhb = await unitOfWork.Repository<DepartmanHizmetBinasi>().GetAllAsync();
            var departmanHizmetBinasi = allDhb.FirstOrDefault(dhb => dhb.DepartmanId == departmanId && dhb.HizmetBinasiId == hizmetBinasiId && !dhb.SilindiMi);
            
            if (departmanHizmetBinasi != null && p.DepartmanHizmetBinasiId != departmanHizmetBinasi.DepartmanHizmetBinasiId) 
            { 
                p.DepartmanHizmetBinasiId = departmanHizmetBinasi.DepartmanHizmetBinasiId; 
                hasChanges = true; 
            }
            var newEmail = string.IsNullOrWhiteSpace(k.Email) ? null : k.Email;
            if (p.Email != newEmail) { p.Email = newEmail; hasChanges = true; }
            if (p.Dahili != (int)k.Dahili) { p.Dahili = (int)k.Dahili; hasChanges = true; }
            if (p.Gorev != k.Gorev) { p.Gorev = k.Gorev; hasChanges = true; }
            if (p.Uzmanlik != k.Brans) { p.Uzmanlik = k.Brans; hasChanges = true; }

            // KartNo: Mevcut değer varsa (0'dan farklı) korur, sadece boşsa legacy'den alır
            var newKartNo = int.TryParse(k.KartNo, out var kn) ? kn : 0;
            if (p.KartNo == 0 && newKartNo != 0) { p.KartNo = newKartNo; hasChanges = true; }

            var newAktiflik = MapCalisanDurum(k.CalisanDurum);
            if (p.PersonelAktiflikDurum != newAktiflik) { p.PersonelAktiflikDurum = newAktiflik; hasChanges = true; }

            if (p.IlId != ilId) { p.IlId = ilId; hasChanges = true; }
            if (p.IlceId != ilceId) { p.IlceId = ilceId; hasChanges = true; }

            // SendikaId validasyonu (LegacyKod ile mapping)
            int? validSendikaId = null;
            if (k.Sendika.HasValue)
            {
                var allSendikalar = await unitOfWork.Repository<Sendika>().GetAllAsync();
                var sendika = allSendikalar.FirstOrDefault(s => s.LegacyKod == k.Sendika.Value);
                
                if (sendika != null)
                {
                    validSendikaId = sendika.SendikaId;
                }
            }

            if (p.SendikaId != validSendikaId) { p.SendikaId = validSendikaId; hasChanges = true; }

            return hasChanges;
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

        private string? ParseEmekliSicilNo(string? emekliSicilNo)
        {
            if (string.IsNullOrWhiteSpace(emekliSicilNo))
                return null;

            // Eğer int değer DEĞİLSE NULL döndür (sadece int değerler kabul edilir)
            if (!int.TryParse(emekliSicilNo, out _))
                return null;

            // 20 karakterden uzunsa kısalt
            return emekliSicilNo.Length > 20 ? emekliSicilNo.Substring(0, 20) : emekliSicilNo;
        }

        private async Task SyncAtanmaNedenleriAsync(ILegacyAtanmaRepository legacyRepo, IUnitOfWork unitOfWork, CancellationToken ct)
        {
            _logger.LogInformation("📦 Atanma Nedenleri sync ediliyor...");

            var legacyAtanmalar = await legacyRepo.GetAllAsync();
            var atanmaNedenleri = await unitOfWork.Repository<AtanmaNedenleri>().GetAllAsync();

            int added = 0, updated = 0;

            foreach (var la in legacyAtanmalar)
            {
                if (string.IsNullOrWhiteSpace(la.AtanmaNedeni)) continue;

                var existing = atanmaNedenleri.FirstOrDefault(a => a.LegacyKod == la.AtanmaId);

                if (existing == null)
                {
                    var newAtanma = new AtanmaNedenleri
                    {
                        AtanmaNedeni = la.AtanmaNedeni,
                        LegacyKod = la.AtanmaId,
                        EkleyenKullanici = "SYNC",
                        EklenmeTarihi = DateTime.Now
                    };
                    await unitOfWork.Repository<AtanmaNedenleri>().AddAsync(newAtanma);
                    added++;
                }
                else if (existing.AtanmaNedeni != la.AtanmaNedeni)
                {
                    existing.AtanmaNedeni = la.AtanmaNedeni;
                    existing.DuzenleyenKullanici = "SYNC";
                    existing.DuzenlenmeTarihi = DateTime.Now;
                    unitOfWork.Repository<AtanmaNedenleri>().Update(existing);
                    updated++;
                }
            }

            if (added > 0 || updated > 0)
                await unitOfWork.SaveChangesAsync();

            _logger.LogInformation("  ✅ Atanma Nedenleri: {added} eklendi, {updated} güncellendi", added, updated);
        }
    }
}
