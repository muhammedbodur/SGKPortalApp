using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri;
using SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri.Interfaces;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PdksIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SGKPortalApp.BusinessObjectLayer.Entities;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.ZKTeco;

namespace SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri
{
    public class CardSyncComparisonService : ICardSyncComparisonService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IZKTecoApiClient _zktecoApiClient;
        private readonly IDeviceService _deviceService;
        private readonly ILogger<CardSyncComparisonService> _logger;

        public CardSyncComparisonService(
            IUnitOfWork unitOfWork,
            IZKTecoApiClient zktecoApiClient,
            IDeviceService deviceService,
            ILogger<CardSyncComparisonService> logger)
        {
            _unitOfWork = unitOfWork;
            _zktecoApiClient = zktecoApiClient;
            _deviceService = deviceService;
            _logger = logger;
        }

        public async Task<DeviceCardSyncReportDto> GenerateCardSyncReportAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            var report = new DeviceCardSyncReportDto();

            try
            {
                _logger.LogInformation("🔍 Kart senkronizasyon raporu oluşturuluyor...");

                // Aktif cihazları al
                var devices = await _deviceService.GetActiveDevicesAsync();
                report.TotalDevicesChecked = devices.Count;

                if (!devices.Any())
                {
                    _logger.LogWarning("⚠️ Aktif cihaz bulunamadı");
                    return report;
                }

                // Veritabanından tüm personel ve özel kartları al
                var personelRepository = _unitOfWork.GetRepository<IPersonelRepository>();
                var specialCardRepository = _unitOfWork.GetRepository<ISpecialCardRepository>();

                var allPersonel = await personelRepository.GetActiveAsync();
                var allSpecialCards = await specialCardRepository.GetActiveCardsAsync();

                // Veritabanındaki tüm kartları EnrollNumber ile dictionary'e al
                var dbCards = new Dictionary<string, (string Name, long CardNumber, string Type, int Id)>();

                foreach (var p in allPersonel)
                {
                    if (!string.IsNullOrWhiteSpace(p.PersonelKayitNo.ToString()))
                    {
                        dbCards[p.PersonelKayitNo.ToString()] = (p.AdSoyad, p.KartNo, "Personel", p.SicilNo);
                    }
                }

                foreach (var sc in allSpecialCards)
                {
                    if (!string.IsNullOrWhiteSpace(sc.EnrollNumber))
                    {
                        dbCards[sc.EnrollNumber] = (sc.NickName ?? sc.CardName, sc.CardNumber, "SpecialCard", sc.Id);
                    }
                }

                report.TotalCardsInDatabase = dbCards.Count;

                // Her cihazdan asenkron olarak kart bilgilerini çek
                var deviceTasks = devices.Select(device => ProcessDeviceAsync(device, dbCards, report));
                await Task.WhenAll(deviceTasks);

                // İstatistikleri hesapla
                report.OnlyInDeviceCount = report.Mismatches.Count(m => m.MismatchType == "OnlyInDevice");
                report.OnlyInDatabaseCount = report.Mismatches.Count(m => m.MismatchType == "OnlyInDatabase");
                report.DataMismatchCount = report.Mismatches.Count(m => m.MismatchType == "DataMismatch");
                report.TotalMismatches = report.Mismatches.Count;

                stopwatch.Stop();
                report.TotalProcessingTime = stopwatch.Elapsed;

                _logger.LogInformation("✅ Kart senkronizasyon raporu tamamlandı: {TotalMismatches} uyumsuzluk bulundu ({Duration}ms)",
                    report.TotalMismatches, stopwatch.ElapsedMilliseconds);

                return report;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Kart senkronizasyon raporu oluşturulurken hata");
                stopwatch.Stop();
                report.TotalProcessingTime = stopwatch.Elapsed;
                return report;
            }
        }

        private async Task ProcessDeviceAsync(
            Device device,
            Dictionary<string, (string Name, long CardNumber, string Type, int Id)> dbCards,
            DeviceCardSyncReportDto report)
        {
            var deviceStopwatch = Stopwatch.StartNew();
            var status = new DeviceSyncStatus
            {
                DeviceId = device.DeviceId,
                DeviceName = device.DeviceName ?? "Unknown",
                DeviceIp = device.IpAddress
            };

            try
            {
                _logger.LogInformation("📡 Cihazdan kartlar çekiliyor: {DeviceName} ({DeviceIp})", device.DeviceName, device.IpAddress);

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                var deviceUsers = await _zktecoApiClient.GetAllUsersFromDeviceAsync(device.IpAddress, port);

                status.Success = true;
                status.CardCount = deviceUsers?.Count ?? 0;
                report.TotalCardsInDevices += status.CardCount;

                if (deviceUsers == null || !deviceUsers.Any())
                {
                    _logger.LogInformation("ℹ️ Cihazda kart bulunamadı: {DeviceName}", device.DeviceName);
                    deviceStopwatch.Stop();
                    status.ProcessingTime = deviceStopwatch.Elapsed;
                    lock (report.DeviceStatuses)
                    {
                        report.DeviceStatuses.Add(status);
                    }
                    return;
                }

                var deviceEnrollNumbers = new HashSet<string>();

                // Cihazdan gelen kartları kontrol et
                foreach (var deviceUser in deviceUsers)
                {
                    deviceEnrollNumbers.Add(deviceUser.EnrollNumber);

                    if (dbCards.TryGetValue(deviceUser.EnrollNumber, out var dbCard))
                    {
                        // Veritabanında var, bilgileri karşılaştır
                        var nameMismatch = !string.Equals(deviceUser.Name?.Trim(), dbCard.Name?.Trim(), StringComparison.OrdinalIgnoreCase);
                        var cardNumberMismatch = deviceUser.CardNumber != dbCard.CardNumber;

                        if (nameMismatch || cardNumberMismatch)
                        {
                            var mismatch = new CardMismatchDto
                            {
                                EnrollNumber = deviceUser.EnrollNumber,
                                Name = deviceUser.Name ?? "",
                                CardNumber = deviceUser.CardNumber,
                                MismatchType = "DataMismatch",
                                DeviceName = device.DeviceName ?? "Unknown",
                                DeviceIp = device.IpAddress,
                                DeviceId = device.DeviceId,
                                DeviceUserName = deviceUser.Name,
                                DeviceCardNumber = deviceUser.CardNumber,
                                DatabaseUserName = dbCard.Name,
                                DatabaseCardNumber = dbCard.CardNumber,
                                UserType = dbCard.Type,
                                UserId = dbCard.Id,
                                Details = BuildMismatchDetails(nameMismatch, cardNumberMismatch)
                            };

                            lock (report.Mismatches)
                            {
                                report.Mismatches.Add(mismatch);
                            }
                        }
                    }
                    else
                    {
                        // Cihazda var ama veritabanında yok
                        var mismatch = new CardMismatchDto
                        {
                            EnrollNumber = deviceUser.EnrollNumber,
                            Name = deviceUser.Name ?? "",
                            CardNumber = deviceUser.CardNumber,
                            MismatchType = "OnlyInDevice",
                            DeviceName = device.DeviceName ?? "Unknown",
                            DeviceIp = device.IpAddress,
                            DeviceId = device.DeviceId,
                            DeviceUserName = deviceUser.Name,
                            DeviceCardNumber = deviceUser.CardNumber,
                            Details = "Cihazda var ancak veritabanında bulunamadı (silinmiş veya hiç eklenmemiş olabilir)"
                        };

                        lock (report.Mismatches)
                        {
                            report.Mismatches.Add(mismatch);
                        }
                    }
                }

                // Veritabanında var ama cihazda yok olanları bul
                foreach (var dbCard in dbCards)
                {
                    if (!deviceEnrollNumbers.Contains(dbCard.Key))
                    {
                        var mismatch = new CardMismatchDto
                        {
                            EnrollNumber = dbCard.Key,
                            Name = dbCard.Value.Name,
                            CardNumber = dbCard.Value.CardNumber,
                            MismatchType = "OnlyInDatabase",
                            DeviceName = device.DeviceName ?? "Unknown",
                            DeviceIp = device.IpAddress,
                            DeviceId = device.DeviceId,
                            DatabaseUserName = dbCard.Value.Name,
                            DatabaseCardNumber = dbCard.Value.CardNumber,
                            UserType = dbCard.Value.Type,
                            UserId = dbCard.Value.Id,
                            Details = $"Veritabanında var ({dbCard.Value.Type}) ancak cihazda bulunamadı (gönderilmemiş veya cihazdan silinmiş olabilir)"
                        };

                        lock (report.Mismatches)
                        {
                            report.Mismatches.Add(mismatch);
                        }
                    }
                }

                _logger.LogInformation("✅ Cihaz işlendi: {DeviceName} - {CardCount} kart", device.DeviceName, status.CardCount);
            }
            catch (Exception ex)
            {
                status.Success = false;
                status.ErrorMessage = ex.Message;
                _logger.LogError(ex, "❌ Cihaz işlenirken hata: {DeviceName}", device.DeviceName);
            }
            finally
            {
                deviceStopwatch.Stop();
                status.ProcessingTime = deviceStopwatch.Elapsed;
                lock (report.DeviceStatuses)
                {
                    report.DeviceStatuses.Add(status);
                }
            }
        }

        private string BuildMismatchDetails(bool nameMismatch, bool cardNumberMismatch)
        {
            var details = new List<string>();
            if (nameMismatch) details.Add("İsim farklı");
            if (cardNumberMismatch) details.Add("Kart numarası farklı");
            return string.Join(", ", details);
        }
    }
}
