using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PdksIslemleri;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri
{
    public class DeviceBusinessService : IDeviceBusinessService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IZKTecoApiClient _apiClient;
        private readonly ILogger<DeviceBusinessService> _logger;

        public DeviceBusinessService(
            IUnitOfWork unitOfWork,
            IZKTecoApiClient apiClient,
            ILogger<DeviceBusinessService> logger)
        {
            _unitOfWork = unitOfWork;
            _apiClient = apiClient;
            _logger = logger;
        }

        // ========== Database Operations ==========

        public async Task<List<Device>> GetAllDevicesAsync()
        {
            var deviceRepo = _unitOfWork.GetRepository<IDeviceRepository>();
            var devices = await deviceRepo.GetAllAsync();
            return devices.Where(d => !d.SilindiMi).OrderBy(d => d.DeviceName).ToList();
        }

        public async Task<Device?> GetDeviceByIdAsync(int id)
        {
            var deviceRepo = _unitOfWork.GetRepository<IDeviceRepository>();
            var device = await deviceRepo.GetByIdAsync(id);
            return device?.SilindiMi == false ? device : null;
        }

        public async Task<Device?> GetDeviceByIpAsync(string ipAddress)
        {
            var deviceRepo = _unitOfWork.GetRepository<IDeviceRepository>();
            var device = await deviceRepo.GetDeviceByIpAsync(ipAddress);
            return device?.SilindiMi == false ? device : null;
        }

        public async Task<Device> CreateDeviceAsync(Device device)
        {
            var deviceRepo = _unitOfWork.GetRepository<IDeviceRepository>();

            // Check if device with same IP already exists
            var existing = await deviceRepo.GetDeviceByIpAsync(device.IpAddress);
            if (existing != null && !existing.SilindiMi)
            {
                throw new InvalidOperationException($"Bu IP adresine ({device.IpAddress}) sahip bir cihaz zaten mevcut.");
            }

            device.EklenmeTarihi = DateTime.Now;
            device.SilindiMi = false;

            await deviceRepo.AddAsync(device);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Device created: {DeviceName} ({IpAddress})", device.DeviceName, device.IpAddress);
            return device;
        }

        public async Task<Device> UpdateDeviceAsync(Device device)
        {
            var deviceRepo = _unitOfWork.GetRepository<IDeviceRepository>();

            var existing = await deviceRepo.GetByIdAsync(device.DeviceId);
            if (existing == null || existing.SilindiMi)
            {
                throw new InvalidOperationException($"Device with ID {device.DeviceId} not found.");
            }

            // Check if IP address is changing to an already used IP
            if (existing.IpAddress != device.IpAddress)
            {
                var deviceWithSameIp = await deviceRepo.GetDeviceByIpAsync(device.IpAddress);
                if (deviceWithSameIp != null && deviceWithSameIp.DeviceId != device.DeviceId && !deviceWithSameIp.SilindiMi)
                {
                    throw new InvalidOperationException($"Bu IP adresine ({device.IpAddress}) sahip başka bir cihaz mevcut.");
                }
            }

            deviceRepo.Update(device);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Device updated: {DeviceName} ({IpAddress})", device.DeviceName, device.IpAddress);
            return device;
        }

        public async Task<bool> DeleteDeviceAsync(int id)
        {
            var deviceRepo = _unitOfWork.GetRepository<IDeviceRepository>();

            var device = await deviceRepo.GetByIdAsync(id);
            if (device == null || device.SilindiMi)
            {
                return false;
            }

            device.SilindiMi = true;
            device.SilinmeTarihi = DateTime.Now;

            deviceRepo.Update(device);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Device soft-deleted: {DeviceName} ({IpAddress})", device.DeviceName, device.IpAddress);
            return true;
        }

        public async Task<List<Device>> GetActiveDevicesAsync()
        {
            var deviceRepo = _unitOfWork.GetRepository<IDeviceRepository>();
            var devices = await deviceRepo.GetAllAsync();
            return devices.Where(d => !d.SilindiMi && d.IsActive).OrderBy(d => d.DeviceName).ToList();
        }

        // ========== Device Operations (API Calls) ==========

        public async Task<DeviceStatusDto?> GetDeviceStatusAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return null;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.GetDeviceStatusAsync(device.IpAddress, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting device status for device {DeviceId}", deviceId);
                return null;
            }
        }

        public async Task<bool> TestConnectionAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return false;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                var success = await _apiClient.TestConnectionAsync(device.IpAddress, port);

                // Update health check information
                device.LastHealthCheckTime = DateTime.Now;
                device.LastHealthCheckSuccess = success;
                device.HealthCheckCount++;
                device.LastHealthCheckStatus = success ? "Bağlantı başarılı" : "Bağlantı başarısız";

                await UpdateDeviceAsync(device);

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing connection for device {DeviceId}", deviceId);
                return false;
            }
        }

        public async Task<bool> RestartDeviceAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return false;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.RestartDeviceAsync(device.IpAddress, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restarting device {DeviceId}", deviceId);
                return false;
            }
        }

        public async Task<bool> EnableDeviceAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return false;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.EnableDeviceAsync(device.IpAddress, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling device {DeviceId}", deviceId);
                return false;
            }
        }

        public async Task<bool> DisableDeviceAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return false;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.DisableDeviceAsync(device.IpAddress, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling device {DeviceId}", deviceId);
                return false;
            }
        }

        public async Task<DeviceTimeDto?> GetDeviceTimeAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return null;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.GetDeviceTimeAsync(device.IpAddress, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting device time for device {DeviceId}", deviceId);
                return null;
            }
        }

        public async Task<bool> PowerOffDeviceAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return false;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.PowerOffDeviceAsync(device.IpAddress, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error powering off device {DeviceId}", deviceId);
                return false;
            }
        }

        public async Task<bool> SetDeviceTimeAsync(int deviceId, DateTime? dateTime = null)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return false;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                var timeToSet = dateTime ?? DateTime.Now;
                return await _apiClient.SetDeviceTimeAsync(device.IpAddress, timeToSet, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting device time for device {DeviceId}", deviceId);
                return false;
            }
        }

        public async Task<bool> SynchronizeDeviceTimeAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return false;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.SetDeviceTimeAsync(device.IpAddress, DateTime.Now, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error synchronizing device time for device {DeviceId}", deviceId);
                return false;
            }
        }

        // ========== User Management ==========

        public async Task<List<ApiUserDto>> GetDeviceUsersAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return new List<ApiUserDto>();

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.GetAllUsersFromDeviceAsync(device.IpAddress, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting device users for device {DeviceId}", deviceId);
                return new List<ApiUserDto>();
            }
        }

        public async Task<ApiUserDto?> GetDeviceUserAsync(int deviceId, string enrollNumber)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return null;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.GetUserFromDeviceAsync(device.IpAddress, enrollNumber, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting device user {EnrollNumber} for device {DeviceId}", enrollNumber, deviceId);
                return null;
            }
        }

        public async Task<ApiUserDto?> GetDeviceUserByCardAsync(int deviceId, long cardNumber)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return null;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.GetUserByCardNumberAsync(device.IpAddress, cardNumber, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting device user by card {CardNumber} for device {DeviceId}", cardNumber, deviceId);
                return null;
            }
        }

        public async Task<DeviceUserMatch> GetDeviceUserWithMismatchInfoAsync(int deviceId, string enrollNumber)
        {
            var match = new DeviceUserMatch();

            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null)
                {
                    match.Status = MatchStatus.NotFound;
                    return match;
                }

                match.Device = new DeviceResponseDto
                {
                    DeviceId = device.DeviceId,
                    DeviceName = device.DeviceName,
                    IpAddress = device.IpAddress,
                    Port = device.Port,
                    IsActive = device.IsActive
                };

                // Cihazdan kullanıcıyı getir
                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                match.DeviceUser = await _apiClient.GetUserFromDeviceAsync(device.IpAddress, enrollNumber, port);

                if (match.DeviceUser == null)
                {
                    match.Status = MatchStatus.NotFound;
                    match.Mismatches.Add(new MismatchDetail
                    {
                        Field = "DeviceUser",
                        Type = MismatchType.UserNotOnDevice,
                        Description = "Kullanıcı cihazda bulunamadı"
                    });
                    return match;
                }

                // Personel bilgilerini DB'den çek
                var personelRepo = _unitOfWork.GetRepository<DataAccessLayer.Repositories.Interfaces.PersonelIslemleri.IPersonelRepository>();
                if (int.TryParse(enrollNumber, out var personelKayitNo))
                {
                    var personel = await personelRepo.GetByIdAsync(personelKayitNo);
                    if (personel != null && !personel.SilindiMi)
                    {
                        match.PersonelInfo = new PersonelResponseDto
                        {
                            PersonelKayitNo = personel.PersonelKayitNo,
                            AdSoyad = personel.AdSoyad,
                            NickName = personel.NickName,
                            SicilNo = personel.SicilNo,
                            TcKimlikNo = personel.TcKimlikNo,
                            KartNo = personel.KartNo,
                            DepartmanAdi = personel.Departman?.DepartmanAdi,
                            UnvanAdi = personel.Unvan?.UnvanAdi
                        };
                    }
                }

                // Uyuşmazlık kontrolü
                var personelDict = new Dictionary<string, PersonelResponseDto>();
                if (match.PersonelInfo != null)
                {
                    personelDict[enrollNumber] = match.PersonelInfo;
                }

                match.Mismatches = await CheckMismatches(match, personelDict);
                match.Status = DetermineMatchStatus(match);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting device user with mismatch info {EnrollNumber} for device {DeviceId}", enrollNumber, deviceId);
                match.Status = MatchStatus.NotFound;
            }

            return match;
        }

        public async Task<DeviceUserMatch> GetDeviceUserByCardWithMismatchInfoAsync(int deviceId, long cardNumber)
        {
            var match = new DeviceUserMatch();

            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null)
                {
                    match.Status = MatchStatus.NotFound;
                    return match;
                }

                match.Device = new DeviceResponseDto
                {
                    DeviceId = device.DeviceId,
                    DeviceName = device.DeviceName,
                    IpAddress = device.IpAddress,
                    Port = device.Port,
                    IsActive = device.IsActive
                };

                // Cihazdan kullanıcıyı kart numarasıyla getir
                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                match.DeviceUser = await _apiClient.GetUserByCardNumberAsync(device.IpAddress, cardNumber, port);

                if (match.DeviceUser == null)
                {
                    match.Status = MatchStatus.NotFound;
                    match.Mismatches.Add(new MismatchDetail
                    {
                        Field = "DeviceUser",
                        Type = MismatchType.UserNotOnDevice,
                        Description = "Kart numarası cihazda bulunamadı"
                    });
                    return match;
                }

                // Personel bilgilerini DB'den çek (EnrollNumber ile)
                var personelRepo = _unitOfWork.GetRepository<DataAccessLayer.Repositories.Interfaces.PersonelIslemleri.IPersonelRepository>();
                if (int.TryParse(match.DeviceUser.EnrollNumber, out var personelKayitNo))
                {
                    var personel = await personelRepo.GetByIdAsync(personelKayitNo);
                    if (personel != null && !personel.SilindiMi)
                    {
                        match.PersonelInfo = new PersonelResponseDto
                        {
                            PersonelKayitNo = personel.PersonelKayitNo,
                            AdSoyad = personel.AdSoyad,
                            NickName = personel.NickName,
                            SicilNo = personel.SicilNo,
                            TcKimlikNo = personel.TcKimlikNo,
                            KartNo = personel.KartNo,
                            DepartmanAdi = personel.Departman?.DepartmanAdi,
                            UnvanAdi = personel.Unvan?.UnvanAdi
                        };
                    }
                }

                // Uyuşmazlık kontrolü
                var personelDict = new Dictionary<string, PersonelResponseDto>();
                if (match.PersonelInfo != null)
                {
                    personelDict[match.DeviceUser.EnrollNumber] = match.PersonelInfo;
                }

                match.Mismatches = await CheckMismatches(match, personelDict);
                match.Status = DetermineMatchStatus(match);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting device user by card with mismatch info {CardNumber} for device {DeviceId}", cardNumber, deviceId);
                match.Status = MatchStatus.NotFound;
            }

            return match;
        }

        public async Task<CardSearchResponse> SearchUserByCardAsync(CardSearchRequest request)
        {
            var response = new CardSearchResponse
            {
                Success = true,
                Message = "Kart sorgulama tamamlandı"
            };

            try
            {
                Device? device = null;
                List<Device> devicesToSearch = new List<Device>();

                if (request.DeviceId.HasValue)
                {
                    // Tek cihazda ara
                    device = await GetDeviceByIdAsync(request.DeviceId.Value);
                    if (device == null)
                    {
                        response.Success = false;
                        response.Message = $"Cihaz bulunamadı: {request.DeviceId.Value}";
                        return response;
                    }
                    devicesToSearch.Add(device);
                }
                else
                {
                    // Aktif tüm cihazlarda ara
                    devicesToSearch = await GetActiveDevicesAsync();
                }

                response.SearchedDeviceCount = devicesToSearch.Count;

                // Personel bilgilerini önceden çek (performans için)
                Dictionary<string, PersonelResponseDto> personelDict = new Dictionary<string, PersonelResponseDto>();
                if (request.IncludePersonelInfo)
                {
                    var personelRepo = _unitOfWork.GetRepository<DataAccessLayer.Repositories.Interfaces.PersonelIslemleri.IPersonelRepository>();
                    var allPersonel = await personelRepo.GetAllAsync();
                    personelDict = allPersonel
                        .Where(p => !p.SilindiMi)
                        .ToDictionary(p => p.PersonelKayitNo.ToString(), p => new PersonelResponseDto
                        {
                            PersonelKayitNo = p.PersonelKayitNo,
                            AdSoyad = p.AdSoyad,
                            NickName = p.NickName,
                            SicilNo = p.SicilNo,
                            TcKimlikNo = p.TcKimlikNo,
                            KartNo = p.KartNo,
                            DepartmanAdi = p.Departman?.DepartmanAdi,
                            UnvanAdi = p.Unvan?.UnvanAdi
                        });
                }

                // Her cihazda ara
                foreach (var searchDevice in devicesToSearch)
                {
                    var port = int.TryParse(searchDevice.Port, out var p) ? p : 4370;
                    var deviceUser = await _apiClient.GetUserByCardNumberAsync(searchDevice.IpAddress, request.CardNumber, port);

                    var deviceMatch = new DeviceUserMatch
                    {
                        Device = new DeviceResponseDto
                        {
                            DeviceId = searchDevice.DeviceId,
                            DeviceName = searchDevice.DeviceName,
                            IpAddress = searchDevice.IpAddress,
                            Port = searchDevice.Port,
                            IsActive = searchDevice.IsActive
                        },
                        DeviceUser = deviceUser
                    };

                    // Personel bilgisi eşleştir
                    if (deviceUser != null && request.IncludePersonelInfo && personelDict.TryGetValue(deviceUser.EnrollNumber, out var personel))
                    {
                        deviceMatch.PersonelInfo = personel;
                    }

                    // Uyuşmazlık kontrolü
                    if (request.IncludeMismatches)
                    {
                        deviceMatch.Mismatches = await CheckMismatches(deviceMatch, personelDict);
                        deviceMatch.Status = DetermineMatchStatus(deviceMatch);
                    }
                    else
                    {
                        deviceMatch.Status = deviceUser != null ? MatchStatus.PerfectMatch : MatchStatus.NotFound;
                    }

                    response.DeviceResults.Add(deviceMatch);

                    if (deviceUser != null)
                    {
                        response.TotalMatches++;
                    }

                    if (deviceMatch.Mismatches.Any())
                    {
                        response.MismatchCount++;
                    }
                }

                if (response.TotalMatches == 0)
                {
                    response.Message = $"Kart numarası {request.CardNumber} hiçbir cihazda bulunamadı";
                }
                else if (response.MismatchCount > 0)
                {
                    response.Message = $"{response.TotalMatches} eşleşme bulundu, {response.MismatchCount} uyuşmazlık tespit edildi";
                }
                else
                {
                    response.Message = $"{response.TotalMatches} tam eşleşme bulundu";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching user by card {CardNumber}", request.CardNumber);
                response.Success = false;
                response.Message = "Kart sorgulama sırasında hata oluştu";
            }

            return response;
        }

        public async Task<CardSearchResponse> SearchUserByCardOnAllDevicesAsync(long cardNumber)
        {
            var request = new CardSearchRequest
            {
                CardNumber = cardNumber,
                SearchAllDevices = true,
                IncludeMismatches = true,
                IncludePersonelInfo = true
            };

            return await SearchUserByCardAsync(request);
        }

        private async Task<List<MismatchDetail>> CheckMismatches(DeviceUserMatch match, Dictionary<string, PersonelResponseDto> personelDict)
        {
            var mismatches = new List<MismatchDetail>();

            if (match.DeviceUser == null)
            {
                mismatches.Add(new MismatchDetail
                {
                    Field = "DeviceUser",
                    Type = MismatchType.UserNotOnDevice,
                    Description = "Kullanıcı cihazda bulunamadı"
                });
                return mismatches;
            }

            if (match.PersonelInfo == null)
            {
                mismatches.Add(new MismatchDetail
                {
                    Field = "PersonelInfo",
                    Type = MismatchType.PersonelNotInDb,
                    Description = "Personel veritabanında bulunamadı"
                });
                return mismatches;
            }

            // EnrollNumber vs PersonelKayitNo uyuşmazlığı
            if (match.DeviceUser.EnrollNumber != match.PersonelInfo.PersonelKayitNo.ToString())
            {
                mismatches.Add(new MismatchDetail
                {
                    Field = "EnrollNumber/PersonelKayitNo",
                    DeviceValue = match.DeviceUser.EnrollNumber,
                    PersonelValue = match.PersonelInfo.PersonelKayitNo.ToString(),
                    Type = MismatchType.EnrollNumberMismatch,
                    Description = "Cihazdaki EnrollNumber ile DB'deki PersonelKayitNo uyuşmuyor"
                });
            }

            // İsim uyuşmazlığı (Name vs NickName)
            var deviceName = match.DeviceUser.Name?.Trim().ToUpper();
            var personelNickName = match.PersonelInfo.NickName?.Trim().ToUpper();
            if (!string.IsNullOrEmpty(deviceName) && !string.IsNullOrEmpty(personelNickName) && deviceName != personelNickName)
            {
                mismatches.Add(new MismatchDetail
                {
                    Field = "Name/NickName",
                    DeviceValue = match.DeviceUser.Name,
                    PersonelValue = match.PersonelInfo.NickName,
                    Type = MismatchType.NameMismatch,
                    Description = "Cihazdaki Name ile DB'deki NickName uyuşmuyor"
                });
            }

            // Kart numarası uyuşmazlığı (CardNumber vs KartNo)
            if (match.DeviceUser.CardNumber.HasValue && match.PersonelInfo.KartNo > 0)
            {
                if (match.DeviceUser.CardNumber.Value != match.PersonelInfo.KartNo)
                {
                    mismatches.Add(new MismatchDetail
                    {
                        Field = "CardNumber/KartNo",
                        DeviceValue = match.DeviceUser.CardNumber.Value.ToString(),
                        PersonelValue = match.PersonelInfo.KartNo.ToString(),
                        Type = MismatchType.CardNumberMismatch,
                        Description = "Cihazdaki CardNumber ile DB'deki KartNo uyuşmuyor"
                    });
                }
            }
            else if (match.DeviceUser.CardNumber.HasValue || match.PersonelInfo.KartNo > 0)
            {
                mismatches.Add(new MismatchDetail
                {
                    Field = "CardNumber/KartNo",
                    DeviceValue = match.DeviceUser.CardNumber?.ToString() ?? "Yok",
                    PersonelValue = match.PersonelInfo.KartNo > 0 ? match.PersonelInfo.KartNo.ToString() : "Yok",
                    Type = MismatchType.CardNumberMismatch,
                    Description = "Kart numarası sadece bir tarafta mevcut"
                });
            }

            return mismatches;
        }

        private MatchStatus DetermineMatchStatus(DeviceUserMatch match)
        {
            if (match.DeviceUser == null && match.PersonelInfo == null)
                return MatchStatus.NotFound;

            if (match.DeviceUser != null && match.PersonelInfo == null)
                return MatchStatus.DeviceOnly;

            if (match.DeviceUser == null && match.PersonelInfo != null)
                return MatchStatus.PersonelOnly;

            if (match.Mismatches.Any())
                return MatchStatus.PartialMatch;

            return MatchStatus.PerfectMatch;
        }

        public async Task<bool> CreateDeviceUserAsync(int deviceId, UserCreateUpdateDto request, bool force = false)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return false;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                var user = new ApiUserDto
                {
                    EnrollNumber = request.EnrollNumber,
                    Name = request.Name,
                    CardNumber = request.CardNumber,
                    Privilege = request.Privilege,
                    Password = request.Password,
                    Enabled = request.Enabled
                };
                return await _apiClient.AddUserToDeviceAsync(device.IpAddress, user, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating device user for device {DeviceId}", deviceId);
                return false;
            }
        }

        public async Task<bool> UpdateDeviceUserAsync(int deviceId, string enrollNumber, UserCreateUpdateDto request, bool force = false)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return false;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                var user = new ApiUserDto
                {
                    EnrollNumber = request.EnrollNumber,
                    Name = request.Name,
                    CardNumber = request.CardNumber,
                    Privilege = request.Privilege,
                    Password = request.Password,
                    Enabled = request.Enabled
                };
                return await _apiClient.UpdateUserOnDeviceAsync(device.IpAddress, enrollNumber, user, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating device user {EnrollNumber} for device {DeviceId}", enrollNumber, deviceId);
                return false;
            }
        }

        public async Task<bool> DeleteDeviceUserAsync(int deviceId, string enrollNumber)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return false;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.DeleteUserFromDeviceAsync(device.IpAddress, enrollNumber, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting device user {EnrollNumber} for device {DeviceId}", enrollNumber, deviceId);
                return false;
            }
        }

        public async Task<bool> ClearAllDeviceUsersAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return false;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.ClearAllUsersFromDeviceAsync(device.IpAddress, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing all device users for device {DeviceId}", deviceId);
                return false;
            }
        }

        public async Task<int> GetDeviceUserCountAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return 0;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.GetUserCountFromDeviceAsync(device.IpAddress, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting device user count for device {DeviceId}", deviceId);
                return 0;
            }
        }

        public async Task<bool> RemoveCardFromUserAsync(int deviceId, string enrollNumber)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return false;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                // NOT: RemoveCardFromUser için interface'te metod yok, UpdateUser ile card=0 yapılmalı
                var user = await _apiClient.GetUserFromDeviceAsync(device.IpAddress, enrollNumber, port);
                if (user == null) return false;
                user.CardNumber = 0;
                return await _apiClient.UpdateUserOnDeviceAsync(device.IpAddress, enrollNumber, user, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing card from user {EnrollNumber} for device {DeviceId}", enrollNumber, deviceId);
                return false;
            }
        }

        // ========== Attendance Management ==========

        public async Task<List<AttendanceLogDto>> GetAttendanceLogsAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return new List<AttendanceLogDto>();

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                var rawLogs = await _apiClient.GetAttendanceLogsFromDeviceAsync(device.IpAddress, port);
                var logs = rawLogs.Select(r => new AttendanceLogDto
                {
                    EnrollNumber = r.EnrollNumber,
                    DateTime = r.EventTime,
                    VerifyMethod = (BusinessObjectLayer.Enums.PdksIslemleri.VerifyMethod)r.VerifyMethod,
                    InOutMode = (BusinessObjectLayer.Enums.PdksIslemleri.InOutMode)r.InOutMode
                }).ToList();

                // ZKTecoApi'den gelen loglar sadece EnrollNumber içerir
                // Business katmanında DB ile eşleştirme yapıyoruz
                if (logs != null && logs.Any())
                {
                    // Tüm personelleri bir kerede çek (performans için)
                    var personelRepo = _unitOfWork.GetRepository<DataAccessLayer.Repositories.Interfaces.PersonelIslemleri.IPersonelRepository>();
                    var allPersonel = await personelRepo.GetAllAsync();
                    var personelDict = allPersonel
                        .Where(p => !p.SilindiMi)
                        .ToDictionary(p => p.PersonelKayitNo.ToString(), p => p);

                    // Her log için personel bilgilerini eşleştir
                    foreach (var log in logs)
                    {
                        if (personelDict.TryGetValue(log.EnrollNumber, out var personel))
                        {
                            log.PersonelAdSoyad = personel.AdSoyad;
                            log.PersonelSicilNo = personel.SicilNo.ToString();
                            log.PersonelTcKimlikNo = personel.TcKimlikNo;
                            log.PersonelDepartman = personel.Departman?.DepartmanAdi;
                        }
                    }
                }

                return logs ?? new List<AttendanceLogDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting attendance logs for device {DeviceId}", deviceId);
                return new List<AttendanceLogDto>();
            }
        }

        public async Task<bool> ClearAttendanceLogsAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return false;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.ClearAttendanceLogsFromDeviceAsync(device.IpAddress, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing attendance logs for device {DeviceId}", deviceId);
                return false;
            }
        }

        public async Task<int> GetAttendanceLogCountAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return 0;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.GetAttendanceLogCountFromDeviceAsync(device.IpAddress, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting attendance log count for device {DeviceId}", deviceId);
                return 0;
            }
        }

        // ========== Realtime Monitoring ==========

        public async Task<bool> StartRealtimeMonitoringAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return false;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.StartRealtimeMonitoringAsync(device.IpAddress, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting monitoring for device {DeviceId}", deviceId);
                return false;
            }
        }

        public async Task<bool> StopRealtimeMonitoringAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return false;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.StopRealtimeMonitoringAsync(device.IpAddress, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping monitoring for device {DeviceId}", deviceId);
                return false;
            }
        }

        public async Task<bool> GetMonitoringStatusAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return false;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.GetRealtimeMonitoringStatusAsync(device.IpAddress, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting monitoring status for device {DeviceId}", deviceId);
                return false;
            }
        }
    }
}
