using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri;
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
        private readonly IMapper _mapper;
        private readonly ILogger<DeviceBusinessService> _logger;

        public DeviceBusinessService(
            IUnitOfWork unitOfWork,
            IZKTecoApiClient apiClient,
            IMapper mapper,
            ILogger<DeviceBusinessService> logger)
        {
            _unitOfWork = unitOfWork;
            _apiClient = apiClient;
            _mapper = mapper;
            _logger = logger;
        }

        // ========== Database Operations ==========

        public async Task<ApiResponseDto<List<DeviceResponseDto>>> GetAllDevicesAsync()
        {
            try
            {
                var deviceRepo = _unitOfWork.GetRepository<IDeviceRepository>();
                var devices = await deviceRepo.GetAllWithRelationsAsync();
                var deviceDtos = _mapper.Map<List<DeviceResponseDto>>(devices);

                return ApiResponseDto<List<DeviceResponseDto>>
                    .SuccessResult(deviceDtos, "Cihazlar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihazlar getirilirken hata oluştu");
                return ApiResponseDto<List<DeviceResponseDto>>
                    .ErrorResult("Cihazlar getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<DeviceResponseDto>> GetDeviceByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<DeviceResponseDto>.ErrorResult("Geçersiz cihaz ID");

                var deviceRepo = _unitOfWork.GetRepository<IDeviceRepository>();
                var device = await deviceRepo.GetByIdWithRelationsAsync(id);

                if (device == null)
                    return ApiResponseDto<DeviceResponseDto>.ErrorResult("Cihaz bulunamadı");

                var deviceDto = _mapper.Map<DeviceResponseDto>(device);
                return ApiResponseDto<DeviceResponseDto>
                    .SuccessResult(deviceDto, "Cihaz başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz getirilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<DeviceResponseDto>
                    .ErrorResult("Cihaz getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<Device>> GetDeviceByIpAsync(string ipAddress)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ipAddress))
                    return ApiResponseDto<Device>.ErrorResult("IP adresi boş olamaz");

                var deviceRepo = _unitOfWork.GetRepository<IDeviceRepository>();
                var device = await deviceRepo.GetDeviceByIpAsync(ipAddress);

                if (device == null)
                    return ApiResponseDto<Device>.ErrorResult("Cihaz bulunamadı");

                return ApiResponseDto<Device>
                    .SuccessResult(device, "Cihaz başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz IP ile getirilirken hata oluştu. IP: {IpAddress}", ipAddress);
                return ApiResponseDto<Device>
                    .ErrorResult("Cihaz getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<DeviceResponseDto>> CreateDeviceAsync(Device device)
        {
            try
            {
                var deviceRepo = _unitOfWork.GetRepository<IDeviceRepository>();

                var existing = await deviceRepo.GetDeviceByIpAsync(device.IpAddress);
                if (existing != null && !existing.SilindiMi)
                    return ApiResponseDto<DeviceResponseDto>
                        .ErrorResult($"Bu IP adresine ({device.IpAddress}) sahip bir cihaz zaten mevcut");

                device.SilindiMi = false;

                await deviceRepo.AddAsync(device);
                await _unitOfWork.SaveChangesAsync();

                var createdDevice = await deviceRepo.GetByIdWithRelationsAsync(device.DeviceId);
                var deviceDto = _mapper.Map<DeviceResponseDto>(createdDevice);

                _logger.LogInformation("Cihaz oluşturuldu: {DeviceName} ({IpAddress})", device.DeviceName, device.IpAddress);
                return ApiResponseDto<DeviceResponseDto>
                    .SuccessResult(deviceDto, "Cihaz başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz oluşturulurken hata oluştu");
                return ApiResponseDto<DeviceResponseDto>
                    .ErrorResult("Cihaz oluşturulurken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<DeviceResponseDto>> UpdateDeviceAsync(Device device)
        {
            try
            {
                var deviceRepo = _unitOfWork.GetRepository<IDeviceRepository>();

                var existing = await deviceRepo.GetByIdAsync(device.DeviceId);
                if (existing == null || existing.SilindiMi)
                    return ApiResponseDto<DeviceResponseDto>
                        .ErrorResult("Cihaz bulunamadı");

                if (existing.IpAddress != device.IpAddress)
                {
                    var deviceWithSameIp = await deviceRepo.GetDeviceByIpAsync(device.IpAddress);
                    if (deviceWithSameIp != null && deviceWithSameIp.DeviceId != device.DeviceId && !deviceWithSameIp.SilindiMi)
                        return ApiResponseDto<DeviceResponseDto>
                            .ErrorResult($"Bu IP adresine ({device.IpAddress}) sahip başka bir cihaz mevcut");
                }

                deviceRepo.Update(device);
                await _unitOfWork.SaveChangesAsync();

                var updatedDevice = await deviceRepo.GetByIdWithRelationsAsync(device.DeviceId);
                var deviceDto = _mapper.Map<DeviceResponseDto>(updatedDevice);

                _logger.LogInformation("Cihaz güncellendi: {DeviceName} ({IpAddress})", device.DeviceName, device.IpAddress);
                return ApiResponseDto<DeviceResponseDto>
                    .SuccessResult(deviceDto, "Cihaz başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz güncellenirken hata oluştu. ID: {Id}", device.DeviceId);
                return ApiResponseDto<DeviceResponseDto>
                    .ErrorResult("Cihaz güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteDeviceAsync(int id)
        {
            try
            {
                var deviceRepo = _unitOfWork.GetRepository<IDeviceRepository>();

                var device = await deviceRepo.GetByIdAsync(id);
                if (device == null || device.SilindiMi)
                    return ApiResponseDto<bool>.ErrorResult("Cihaz bulunamadı");

                device.SilindiMi = true;
                device.SilinmeTarihi = DateTime.Now;

                deviceRepo.Update(device);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Cihaz silindi: {DeviceName} ({IpAddress})", device.DeviceName, device.IpAddress);
                return ApiResponseDto<bool>
                    .SuccessResult(true, "Cihaz başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz silinirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<bool>
                    .ErrorResult("Cihaz silinirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<DeviceResponseDto>>> GetActiveDevicesAsync()
        {
            try
            {
                var deviceRepo = _unitOfWork.GetRepository<IDeviceRepository>();
                var devices = await deviceRepo.GetAllWithRelationsAsync();
                var activeDevices = devices.Where(d => d.IsActive).ToList();
                var deviceDtos = _mapper.Map<List<DeviceResponseDto>>(activeDevices);

                return ApiResponseDto<List<DeviceResponseDto>>
                    .SuccessResult(deviceDtos, "Aktif cihazlar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif cihazlar getirilirken hata oluştu");
                return ApiResponseDto<List<DeviceResponseDto>>
                    .ErrorResult("Aktif cihazlar getirilirken bir hata oluştu", ex.Message);
            }
        }

        // ========== Device Operations (API Calls) ==========

        // Helper method for internal use - returns Device entity directly
        private async Task<Device?> GetDeviceEntityByIdAsync(int deviceId)
        {
            try
            {
                var deviceRepo = _unitOfWork.GetRepository<IDeviceRepository>();
                return await deviceRepo.GetByIdAsync(deviceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting device entity. ID: {Id}", deviceId);
                return null;
            }
        }

        public async Task<ApiResponseDto<DeviceStatusDto>> GetDeviceStatusAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceEntityByIdAsync(deviceId);
                if (device == null)
                    return ApiResponseDto<DeviceStatusDto>.ErrorResult("Cihaz bulunamadı");

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                var status = await _apiClient.GetDeviceStatusAsync(device.IpAddress, port);
                
                return ApiResponseDto<DeviceStatusDto>
                    .SuccessResult(status, "Cihaz durumu başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz durumu getirilirken hata oluştu. DeviceId: {DeviceId}", deviceId);
                return ApiResponseDto<DeviceStatusDto>
                    .ErrorResult("Cihaz durumu getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> TestConnectionAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceEntityByIdAsync(deviceId);
                if (device == null)
                    return ApiResponseDto<bool>.ErrorResult("Cihaz bulunamadı");

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                var success = await _apiClient.TestConnectionAsync(device.IpAddress, port);

                device.LastHealthCheckTime = DateTime.Now;
                device.LastHealthCheckSuccess = success;
                device.HealthCheckCount++;
                device.LastHealthCheckStatus = success ? "Bağlantı başarılı" : "Bağlantı başarısız";

                await UpdateDeviceAsync(device);

                return ApiResponseDto<bool>
                    .SuccessResult(success, success ? "Bağlantı başarılı" : "Bağlantı başarısız");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bağlantı testi sırasında hata oluştu. DeviceId: {DeviceId}", deviceId);
                return ApiResponseDto<bool>
                    .ErrorResult("Bağlantı testi sırasında bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> RestartDeviceAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceEntityByIdAsync(deviceId);
                if (device == null)
                    return ApiResponseDto<bool>.ErrorResult("Cihaz bulunamadı");

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                var success = await _apiClient.RestartDeviceAsync(device.IpAddress, port);
                
                return ApiResponseDto<bool>
                    .SuccessResult(success, success ? "Cihaz yeniden başlatıldı" : "Cihaz yeniden başlatılamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz yeniden başlatılırken hata oluştu. DeviceId: {DeviceId}", deviceId);
                return ApiResponseDto<bool>
                    .ErrorResult("Cihaz yeniden başlatılırken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> EnableDeviceAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceEntityByIdAsync(deviceId);
                if (device == null)
                    return ApiResponseDto<bool>.ErrorResult("Cihaz bulunamadı");

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                var success = await _apiClient.EnableDeviceAsync(device.IpAddress, port);
                
                return ApiResponseDto<bool>
                    .SuccessResult(success, success ? "Cihaz etkinleştirildi" : "Cihaz etkinleştirilemedi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz etkinleştirilirken hata oluştu. DeviceId: {DeviceId}", deviceId);
                return ApiResponseDto<bool>
                    .ErrorResult("Cihaz etkinleştirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DisableDeviceAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceEntityByIdAsync(deviceId);
                if (device == null)
                    return ApiResponseDto<bool>.ErrorResult("Cihaz bulunamadı");

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                var success = await _apiClient.DisableDeviceAsync(device.IpAddress, port);
                
                return ApiResponseDto<bool>
                    .SuccessResult(success, success ? "Cihaz devre dışı bırakıldı" : "Cihaz devre dışı bırakılamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz devre dışı bırakılırken hata oluştu. DeviceId: {DeviceId}", deviceId);
                return ApiResponseDto<bool>
                    .ErrorResult("Cihaz devre dışı bırakılırken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<DeviceTimeDto>> GetDeviceTimeAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceEntityByIdAsync(deviceId);
                if (device == null)
                    return ApiResponseDto<DeviceTimeDto>.ErrorResult("Cihaz bulunamadı");

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                var time = await _apiClient.GetDeviceTimeAsync(device.IpAddress, port);
                
                return ApiResponseDto<DeviceTimeDto>
                    .SuccessResult(time, "Cihaz saati başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz saati getirilirken hata oluştu. DeviceId: {DeviceId}", deviceId);
                return ApiResponseDto<DeviceTimeDto>
                    .ErrorResult("Cihaz saati getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> PowerOffDeviceAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceEntityByIdAsync(deviceId);
                if (device == null)
                    return ApiResponseDto<bool>.ErrorResult("Cihaz bulunamadı");

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                var success = await _apiClient.PowerOffDeviceAsync(device.IpAddress, port);
                
                return ApiResponseDto<bool>
                    .SuccessResult(success, success ? "Cihaz kapatıldı" : "Cihaz kapatılamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz kapatılırken hata oluştu. DeviceId: {DeviceId}", deviceId);
                return ApiResponseDto<bool>
                    .ErrorResult("Cihaz kapatılırken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> SetDeviceTimeAsync(int deviceId, DateTime? dateTime = null)
        {
            try
            {
                var device = await GetDeviceEntityByIdAsync(deviceId);
                if (device == null)
                    return ApiResponseDto<bool>.ErrorResult("Cihaz bulunamadı");

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                var timeToSet = dateTime ?? DateTime.Now;
                var success = await _apiClient.SetDeviceTimeAsync(device.IpAddress, timeToSet, port);
                
                return ApiResponseDto<bool>
                    .SuccessResult(success, success ? "Cihaz saati ayarlandı" : "Cihaz saati ayarlanamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz saati ayarlanırken hata oluştu. DeviceId: {DeviceId}", deviceId);
                return ApiResponseDto<bool>
                    .ErrorResult("Cihaz saati ayarlanırken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> SynchronizeDeviceTimeAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceEntityByIdAsync(deviceId);
                if (device == null)
                    return ApiResponseDto<bool>.ErrorResult("Cihaz bulunamadı");

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                var success = await _apiClient.SetDeviceTimeAsync(device.IpAddress, DateTime.Now, port);
                
                return ApiResponseDto<bool>
                    .SuccessResult(success, success ? "Cihaz saati senkronize edildi" : "Cihaz saati senkronize edilemedi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz saati senkronize edilirken hata oluştu. DeviceId: {DeviceId}", deviceId);
                return ApiResponseDto<bool>
                    .ErrorResult("Cihaz saati senkronize edilirken bir hata oluştu", ex.Message);
            }
        }

        // ========== User Management ==========

        public async Task<List<ApiUserDto>> GetDeviceUsersAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceEntityByIdAsync(deviceId);
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
                var device = await GetDeviceEntityByIdAsync(deviceId);
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
                var device = await GetDeviceEntityByIdAsync(deviceId);
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
                var device = await GetDeviceEntityByIdAsync(deviceId);
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

        public async Task<List<DeviceUserMatch>> GetAllDeviceUsersWithMismatchInfoAsync(int deviceId)
        {
            var matches = new List<DeviceUserMatch>();

            try
            {
                var device = await GetDeviceEntityByIdAsync(deviceId);
                if (device == null)
                {
                    _logger.LogWarning("Device {DeviceId} not found", deviceId);
                    return matches;
                }

                // Cihazdan tüm kullanıcıları getir
                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                var deviceUsers = await _apiClient.GetAllUsersFromDeviceAsync(device.IpAddress, port);

                if (deviceUsers == null || !deviceUsers.Any())
                {
                    return matches;
                }

                // Veritabanından tüm personelleri getir (cache için)
                var allPersonel = await _unitOfWork.Repository<Personel>().GetAllAsync();
                // PersonelKayitNo 0 olanları ve duplicate'leri filtrele
                var personelDict = allPersonel
                    .Where(p => p.PersonelKayitNo > 0)
                    .GroupBy(p => p.PersonelKayitNo.ToString())
                    .ToDictionary(g => g.Key, g => _mapper.Map<PersonelResponseDto>(g.First()));

                // Her cihaz kullanıcısı için uyumsuzluk kontrolü yap
                foreach (var deviceUser in deviceUsers)
                {
                    var match = new DeviceUserMatch
                    {
                        Device = new DeviceResponseDto
                        {
                            DeviceId = device.DeviceId,
                            DeviceName = device.DeviceName,
                            IpAddress = device.IpAddress,
                            Port = device.Port,
                            IsActive = device.IsActive
                        },
                        DeviceUser = deviceUser
                    };

                    // Personel bilgisini bul
                    if (personelDict.TryGetValue(deviceUser.EnrollNumber, out var personelInfo))
                    {
                        match.PersonelInfo = personelInfo;
                    }

                    // Uyumsuzlukları kontrol et
                    match.Mismatches = await CheckMismatches(match, personelDict);
                    match.Status = DetermineMatchStatus(match);

                    matches.Add(match);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all device users with mismatch info for device {DeviceId}", deviceId);
            }

            return matches;
        }

        public async Task<DeviceUserMatch> GetDeviceUserByCardWithMismatchInfoAsync(int deviceId, long cardNumber)
        {
            var match = new DeviceUserMatch();

            try
            {
                var device = await GetDeviceEntityByIdAsync(deviceId);
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
                    device = await GetDeviceEntityByIdAsync(request.DeviceId.Value);
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
                    var deviceRepo = _unitOfWork.GetRepository<IDeviceRepository>();
                    var allDevices = await deviceRepo.GetActiveDevicesAsync();
                    devicesToSearch = allDevices;
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
                var device = await GetDeviceEntityByIdAsync(deviceId);
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
                var device = await GetDeviceEntityByIdAsync(deviceId);
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
                var device = await GetDeviceEntityByIdAsync(deviceId);
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
                var device = await GetDeviceEntityByIdAsync(deviceId);
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
                var device = await GetDeviceEntityByIdAsync(deviceId);
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
                var device = await GetDeviceEntityByIdAsync(deviceId);
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
                var device = await GetDeviceEntityByIdAsync(deviceId);
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
                var device = await GetDeviceEntityByIdAsync(deviceId);
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
                var device = await GetDeviceEntityByIdAsync(deviceId);
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
                var device = await GetDeviceEntityByIdAsync(deviceId);
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
                var device = await GetDeviceEntityByIdAsync(deviceId);
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
                var device = await GetDeviceEntityByIdAsync(deviceId);
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
