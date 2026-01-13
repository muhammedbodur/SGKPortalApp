using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Shared.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PdksIslemleri;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SGKPortalApp.Common.Helpers;

namespace SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri
{
    public class SpecialCardBusinessService : ISpecialCardBusinessService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SpecialCardBusinessService> _logger;
        private readonly IZKTecoApiClient _zktecoApiClient;
        private readonly IDeviceService _deviceService;

        public SpecialCardBusinessService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<SpecialCardBusinessService> logger,
            IZKTecoApiClient zktecoApiClient,
            IDeviceService deviceService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _zktecoApiClient = zktecoApiClient;
            _deviceService = deviceService;
        }

        public async Task<ApiResponseDto<List<SpecialCardResponseDto>>> GetAllAsync()
        {
            try
            {
                var repository = _unitOfWork.GetRepository<ISpecialCardRepository>();
                var cards = await repository.GetActiveCardsAsync();
                
                var response = cards.Select(c => new SpecialCardResponseDto
                {
                    Id = c.Id,
                    CardType = c.CardType,
                    CardTypeName = c.CardType.ToString(),
                    CardNumber = c.CardNumber,
                    CardName = c.CardName,
                    EnrollNumber = c.EnrollNumber,
                    NickName = c.NickName,
                    HizmetBinasiId = c.HizmetBinasiId,
                    HizmetBinasiAdi = c.HizmetBinasi?.HizmetBinasiAdi,
                    Notes = c.Notes,
                    EklenmeTarihi = c.EklenmeTarihi,
                    DuzenlenmeTarihi = c.DuzenlenmeTarihi
                }).ToList();

                return ApiResponseDto<List<SpecialCardResponseDto>>.SuccessResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all special cards");
                return ApiResponseDto<List<SpecialCardResponseDto>>.ErrorResult("Özel kartlar getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<SpecialCardResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<ISpecialCardRepository>();
                var card = await repository.GetByIdAsync(id);

                if (card == null)
                    return ApiResponseDto<SpecialCardResponseDto>.ErrorResult("Özel kart bulunamadı");

                var response = new SpecialCardResponseDto
                {
                    Id = card.Id,
                    CardType = card.CardType,
                    CardTypeName = card.CardType.ToString(),
                    CardNumber = card.CardNumber,
                    CardName = card.CardName,
                    EnrollNumber = card.EnrollNumber,
                    NickName = card.NickName,
                    HizmetBinasiId = card.HizmetBinasiId,
                    HizmetBinasiAdi = card.HizmetBinasi?.HizmetBinasiAdi,
                    Notes = card.Notes,
                    EklenmeTarihi = card.EklenmeTarihi,
                    DuzenlenmeTarihi = card.DuzenlenmeTarihi
                };

                return ApiResponseDto<SpecialCardResponseDto>.SuccessResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting special card by id: {Id}", id);
                return ApiResponseDto<SpecialCardResponseDto>.ErrorResult("Özel kart getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<SpecialCardResponseDto>> GetByCardNumberAsync(long cardNumber)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<ISpecialCardRepository>();
                var card = await repository.GetByCardNumberAsync(cardNumber);

                if (card == null)
                    return ApiResponseDto<SpecialCardResponseDto>.ErrorResult("Özel kart bulunamadı");

                var response = new SpecialCardResponseDto
                {
                    Id = card.Id,
                    CardType = card.CardType,
                    CardTypeName = card.CardType.ToString(),
                    CardNumber = card.CardNumber,
                    CardName = card.CardName,
                    EnrollNumber = card.EnrollNumber,
                    Notes = card.Notes,
                    EklenmeTarihi = card.EklenmeTarihi,
                    DuzenlenmeTarihi = card.DuzenlenmeTarihi
                };

                return ApiResponseDto<SpecialCardResponseDto>.SuccessResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting special card by card number: {CardNumber}", cardNumber);
                return ApiResponseDto<SpecialCardResponseDto>.ErrorResult("Özel kart getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<SpecialCardResponseDto>> GetByEnrollNumberAsync(string enrollNumber)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<ISpecialCardRepository>();
                var card = await repository.GetByEnrollNumberAsync(enrollNumber);

                if (card == null)
                    return ApiResponseDto<SpecialCardResponseDto>.ErrorResult("Özel kart bulunamadı");

                var response = new SpecialCardResponseDto
                {
                    Id = card.Id,
                    CardType = card.CardType,
                    CardTypeName = card.CardType.ToString(),
                    CardNumber = card.CardNumber,
                    CardName = card.CardName,
                    EnrollNumber = card.EnrollNumber,
                    Notes = card.Notes,
                    EklenmeTarihi = card.EklenmeTarihi,
                    DuzenlenmeTarihi = card.DuzenlenmeTarihi
                };

                return ApiResponseDto<SpecialCardResponseDto>.SuccessResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting special card by enroll number: {EnrollNumber}", enrollNumber);
                return ApiResponseDto<SpecialCardResponseDto>.ErrorResult("Özel kart getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<List<SpecialCardResponseDto>>> GetByCardTypeAsync(CardType cardType)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<ISpecialCardRepository>();
                var cards = await repository.GetByCardTypeAsync(cardType);

                var response = cards.Select(c => new SpecialCardResponseDto
                {
                    Id = c.Id,
                    CardType = c.CardType,
                    CardTypeName = c.CardType.ToString(),
                    CardNumber = c.CardNumber,
                    CardName = c.CardName,
                    EnrollNumber = c.EnrollNumber,
                    Notes = c.Notes,
                    EklenmeTarihi = c.EklenmeTarihi,
                    DuzenlenmeTarihi = c.DuzenlenmeTarihi
                }).ToList();

                return ApiResponseDto<List<SpecialCardResponseDto>>.SuccessResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting special cards by type: {CardType}", cardType);
                return ApiResponseDto<List<SpecialCardResponseDto>>.ErrorResult("Özel kartlar getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<SpecialCardResponseDto>> CreateAsync(SpecialCardCreateRequestDto request)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<ISpecialCardRepository>();

                // Kart numarası kontrolü
                var existingByCardNumber = await repository.GetByCardNumberAsync(request.CardNumber);
                if (existingByCardNumber != null)
                    return ApiResponseDto<SpecialCardResponseDto>.ErrorResult("Bu kart numarası zaten kullanılıyor");

                // EnrollNumber otomatik atama veya kontrol
                string enrollNumber;
                if (string.IsNullOrWhiteSpace(request.EnrollNumber))
                {
                    // EnrollNumber boşsa otomatik ata (60000-65534 aralığı)
                    enrollNumber = await repository.GetNextAvailableEnrollNumberAsync();
                    _logger.LogInformation("Özel kart için otomatik EnrollNumber atandı: {EnrollNumber}", enrollNumber);
                }
                else
                {
                    // EnrollNumber verilmişse kontrol et
                    enrollNumber = request.EnrollNumber;
                    
                    // Aralık kontrolü (60000-65534)
                    if (int.TryParse(enrollNumber, out var enrollNum))
                    {
                        if (enrollNum < 60000 || enrollNum > 65534)
                        {
                            return ApiResponseDto<SpecialCardResponseDto>.ErrorResult(
                                $"Özel kartlar için EnrollNumber 60000-65534 aralığında olmalıdır. Girilen: {enrollNum}");
                        }
                    }
                    
                    // Duplicate kontrolü
                    var existingByEnrollNumber = await repository.GetByEnrollNumberAsync(enrollNumber);
                    if (existingByEnrollNumber != null)
                        return ApiResponseDto<SpecialCardResponseDto>.ErrorResult("Bu EnrollNumber zaten kullanılıyor");
                }

                var card = new SpecialCard
                {
                    CardType = request.CardType,
                    CardNumber = request.CardNumber,
                    CardName = request.CardName,
                    EnrollNumber = enrollNumber,
                    NickName = string.IsNullOrWhiteSpace(request.NickName) 
                        ? StringHelper.GenerateNickName(request.CardName, 12) 
                        : request.NickName,
                    HizmetBinasiId = request.HizmetBinasiId,
                    Notes = request.Notes
                };

                await repository.AddAsync(card);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Özel kart oluşturuldu: {CardName} ({CardNumber})", card.CardName, card.CardNumber);

                var response = new SpecialCardResponseDto
                {
                    Id = card.Id,
                    CardType = card.CardType,
                    CardTypeName = card.CardType.ToString(),
                    CardNumber = card.CardNumber,
                    CardName = card.CardName,
                    EnrollNumber = card.EnrollNumber,
                    NickName = card.NickName,
                    HizmetBinasiId = card.HizmetBinasiId,
                    HizmetBinasiAdi = card.HizmetBinasi?.HizmetBinasiAdi,
                    Notes = card.Notes,
                    EklenmeTarihi = card.EklenmeTarihi,
                    DuzenlenmeTarihi = card.DuzenlenmeTarihi
                };

                return ApiResponseDto<SpecialCardResponseDto>.SuccessResult(response, "Özel kart başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating special card");
                return ApiResponseDto<SpecialCardResponseDto>.ErrorResult("Özel kart oluşturulurken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<SpecialCardResponseDto>> UpdateAsync(int id, SpecialCardUpdateRequestDto request)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<ISpecialCardRepository>();
                var card = await repository.GetByIdAsync(id);

                if (card == null)
                    return ApiResponseDto<SpecialCardResponseDto>.ErrorResult("Özel kart bulunamadı");

                // Kart numarası kontrolü (kendisi hariç)
                var existingByCardNumber = await repository.GetByCardNumberAsync(request.CardNumber);
                if (existingByCardNumber != null && existingByCardNumber.Id != id)
                    return ApiResponseDto<SpecialCardResponseDto>.ErrorResult("Bu kart numarası zaten kullanılıyor");

                // EnrollNumber kontrolü ve validasyon
                if (!string.IsNullOrWhiteSpace(request.EnrollNumber))
                {
                    // Aralık kontrolü (60000-65534)
                    if (int.TryParse(request.EnrollNumber, out var enrollNum))
                    {
                        if (enrollNum < 60000 || enrollNum > 65534)
                        {
                            return ApiResponseDto<SpecialCardResponseDto>.ErrorResult(
                                $"Özel kartlar için EnrollNumber 60000-65534 aralığında olmalıdır. Girilen: {enrollNum}");
                        }
                    }
                    
                    // Duplicate kontrolü (kendisi hariç)
                    var existingByEnrollNumber = await repository.GetByEnrollNumberAsync(request.EnrollNumber);
                    if (existingByEnrollNumber != null && existingByEnrollNumber.Id != id)
                        return ApiResponseDto<SpecialCardResponseDto>.ErrorResult("Bu EnrollNumber zaten kullanılıyor");
                }

                card.CardType = request.CardType;
                card.CardNumber = request.CardNumber;
                card.CardName = request.CardName;
                card.EnrollNumber = request.EnrollNumber;
                card.NickName = string.IsNullOrWhiteSpace(request.NickName)
                    ? StringHelper.GenerateNickName(request.CardName, 12)
                    : request.NickName;
                card.HizmetBinasiId = request.HizmetBinasiId;
                card.Notes = request.Notes;

                repository.Update(card);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Özel kart güncellendi: {CardName} ({CardNumber})", card.CardName, card.CardNumber);

                var response = new SpecialCardResponseDto
                {
                    Id = card.Id,
                    CardType = card.CardType,
                    CardTypeName = card.CardType.ToString(),
                    CardNumber = card.CardNumber,
                    CardName = card.CardName,
                    EnrollNumber = card.EnrollNumber,
                    NickName = card.NickName,
                    HizmetBinasiId = card.HizmetBinasiId,
                    HizmetBinasiAdi = card.HizmetBinasi?.HizmetBinasiAdi,
                    Notes = card.Notes,
                    EklenmeTarihi = card.EklenmeTarihi,
                    DuzenlenmeTarihi = card.DuzenlenmeTarihi
                };

                return ApiResponseDto<SpecialCardResponseDto>.SuccessResult(response, "Özel kart başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating special card: {Id}", id);
                return ApiResponseDto<SpecialCardResponseDto>.ErrorResult("Özel kart güncellenirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<ISpecialCardRepository>();
                var card = await repository.GetByIdAsync(id);

                if (card == null)
                    return ApiResponseDto<bool>.ErrorResult("Özel kart bulunamadı");

                repository.Delete(card);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Özel kart silindi: {CardName} ({CardNumber})", card.CardName, card.CardNumber);

                return ApiResponseDto<bool>.SuccessResult(true, "Özel kart başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting special card: {Id}", id);
                return ApiResponseDto<bool>.ErrorResult("Özel kart silinirken hata oluştu");
            }
        }

        // ========== Device Operations ==========

        public async Task<ApiResponseDto<CardSyncResultDto>> SendCardToDeviceAsync(int cardId, int deviceId)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<ISpecialCardRepository>();
                var card = await repository.GetByIdAsync(cardId);

                if (card == null)
                    return ApiResponseDto<CardSyncResultDto>.ErrorResult("Özel kart bulunamadı");

                var device = await _deviceService.GetDeviceByIdAsync(deviceId);
                if (device == null)
                    return ApiResponseDto<CardSyncResultDto>.ErrorResult("Cihaz bulunamadı");

                var result = new CardSyncResultDto
                {
                    TotalDevices = 1
                };

                var detail = new DeviceSyncDetail
                {
                    DeviceId = device.DeviceId,
                    DeviceName = device.DeviceName,
                    DeviceIp = device.IpAddress
                };

                try
                {
                    // SpecialCard -> ApiUserDto dönüşümü
                    var apiUser = new ApiUserDto
                    {
                        EnrollNumber = card.EnrollNumber,
                        Name = card.NickName, // NickName kullan (max 12 char, uppercase, no Turkish)
                        CardNumber = card.CardNumber,
                        Privilege = 0, // Normal user
                        Password = string.Empty, // Boş password (bazı cihazlar zorunlu tutar)
                        Enabled = true
                    };

                    var port = int.TryParse(device.Port, out var p) ? p : 4370;
                    
                    // Önce kullanıcının cihazda olup olmadığını kontrol et
                    var existingUser = await _zktecoApiClient.GetUserFromDeviceAsync(
                        device.IpAddress, 
                        card.EnrollNumber, 
                        port);

                    bool success;
                    if (existingUser != null)
                    {
                        // Kullanıcı var, önce sil sonra ekle (güncelleme yerine)
                        _logger.LogInformation("Özel kart cihazda mevcut, siliniyor ve yeniden ekleniyor: {DeviceName} - {EnrollNumber}", 
                            device.DeviceName, card.EnrollNumber);
                        
                        await _zktecoApiClient.DeleteUserFromDeviceAsync(device.IpAddress, card.EnrollNumber, port);
                        await Task.Delay(500); // Cihazın işlemi tamamlaması için kısa bekleme
                        
                        success = await _zktecoApiClient.AddUserToDeviceAsync(device.IpAddress, apiUser, port, force: true);
                    }
                    else
                    {
                        // Kullanıcı yok, ekle
                        _logger.LogInformation("Özel kart cihazda yok, ekleniyor: {DeviceName} - {EnrollNumber}", 
                            device.DeviceName, card.EnrollNumber);
                        success = await _zktecoApiClient.AddUserToDeviceAsync(device.IpAddress, apiUser, port, force: true);
                    }

                    if (success)
                    {
                        detail.Success = true;
                        result.SuccessCount++;
                        _logger.LogInformation("Özel kart cihaza gönderildi: {CardName} (EnrollNumber: {EnrollNumber}) -> {DeviceName}", 
                            card.CardName, card.EnrollNumber, device.DeviceName);
                    }
                    else
                    {
                        detail.Success = false;
                        detail.ErrorMessage = $"Cihaza gönderilemedi (EnrollNumber: {card.EnrollNumber})";
                        result.FailCount++;
                        _logger.LogWarning("Özel kart cihaza gönderilemedi: {CardName} (EnrollNumber: {EnrollNumber}) -> {DeviceName}. " +
                            "Muhtemel sebepler: EnrollNumber çakışması, cihaz kapasitesi dolu, cihaz bağlantı hatası", 
                            card.CardName, card.EnrollNumber, device.DeviceName);
                    }
                }
                catch (Exception ex)
                {
                    detail.Success = false;
                    detail.ErrorMessage = ex.Message;
                    result.FailCount++;
                    _logger.LogError(ex, "Özel kart cihaza gönderilirken hata: {CardName} -> {DeviceName}", card.CardName, device.DeviceName);
                }

                result.Details.Add(detail);
                return ApiResponseDto<CardSyncResultDto>.SuccessResult(result, result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending card to device: CardId={CardId}, DeviceId={DeviceId}", cardId, deviceId);
                return ApiResponseDto<CardSyncResultDto>.ErrorResult("Kart cihaza gönderilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<CardSyncResultDto>> SendCardToAllDevicesAsync(int cardId)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<ISpecialCardRepository>();
                var card = await repository.GetByIdAsync(cardId);

                if (card == null)
                    return ApiResponseDto<CardSyncResultDto>.ErrorResult("Özel kart bulunamadı");

                // Sadece kartın hizmet binasındaki cihazları al
                var allDevices = await _deviceService.GetActiveDevicesAsync();
                
                if (!card.HizmetBinasiId.HasValue)
                    return ApiResponseDto<CardSyncResultDto>.ErrorResult("Kartın hizmet binası tanımlı değil");
                
                var devices = allDevices.Where(d => d.HizmetBinasiId == card.HizmetBinasiId.Value).ToList();
                
                if (!devices.Any())
                    return ApiResponseDto<CardSyncResultDto>.ErrorResult($"Kartın hizmet binasında ({card.HizmetBinasiId.Value}) aktif cihaz bulunamadı");

                var result = new CardSyncResultDto
                {
                    TotalDevices = devices.Count
                };

                // SpecialCard -> ApiUserDto dönüşümü
                var apiUser = new ApiUserDto
                {
                    EnrollNumber = card.EnrollNumber,
                    Name = card.NickName, // NickName kullan (max 12 char, uppercase, no Turkish)
                    CardNumber = card.CardNumber,
                    Privilege = 0, // Normal user
                    Password = string.Empty, // Boş password (bazı cihazlar zorunlu tutar)
                    Enabled = true
                };

                foreach (var device in devices)
                {
                    var detail = new DeviceSyncDetail
                    {
                        DeviceId = device.DeviceId,
                        DeviceName = device.DeviceName,
                        DeviceIp = device.IpAddress
                    };

                    try
                    {
                        var port = int.TryParse(device.Port, out var p) ? p : 4370;
                        
                        // Önce kullanıcının cihazda olup olmadığını kontrol et
                        var existingUser = await _zktecoApiClient.GetUserFromDeviceAsync(
                            device.IpAddress, 
                            card.EnrollNumber, 
                            port);

                        bool success;
                        if (existingUser != null)
                        {
                            // Kullanıcı var, önce sil sonra ekle (güncelleme yerine)
                            _logger.LogInformation("Özel kart cihazda mevcut, siliniyor ve yeniden ekleniyor: {DeviceName} - {EnrollNumber}", 
                                device.DeviceName, card.EnrollNumber);
                            
                            await _zktecoApiClient.DeleteUserFromDeviceAsync(device.IpAddress, card.EnrollNumber, port);
                            await Task.Delay(500); // Cihazın işlemi tamamlaması için kısa bekleme
                            
                            success = await _zktecoApiClient.AddUserToDeviceAsync(device.IpAddress, apiUser, port, force: true);
                        }
                        else
                        {
                            // Kullanıcı yok, ekle
                            _logger.LogInformation("Özel kart cihazda yok, ekleniyor: {DeviceName} - {EnrollNumber}", 
                                device.DeviceName, card.EnrollNumber);
                            success = await _zktecoApiClient.AddUserToDeviceAsync(device.IpAddress, apiUser, port, force: true);
                        }

                        if (success)
                        {
                            detail.Success = true;
                            result.SuccessCount++;
                            _logger.LogInformation("Özel kart cihaza gönderildi: {CardName} -> {DeviceName}", card.CardName, device.DeviceName);
                        }
                        else
                        {
                            detail.Success = false;
                            detail.ErrorMessage = "Cihaza gönderilemedi";
                            result.FailCount++;
                            _logger.LogWarning("Özel kart cihaza gönderilemedi: {CardName} -> {DeviceName}", card.CardName, device.DeviceName);
                        }
                    }
                    catch (Exception ex)
                    {
                        detail.Success = false;
                        detail.ErrorMessage = ex.Message;
                        result.FailCount++;
                        _logger.LogError(ex, "Özel kart cihaza gönderilirken hata: {CardName} -> {DeviceName}", card.CardName, device.DeviceName);
                    }

                    result.Details.Add(detail);
                }

                _logger.LogInformation("Özel kart tüm cihazlara gönderildi: {CardName} - Başarılı: {Success}/{Total}", 
                    card.CardName, result.SuccessCount, result.TotalDevices);

                return ApiResponseDto<CardSyncResultDto>.SuccessResult(result, result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending card to all devices: CardId={CardId}", cardId);
                return ApiResponseDto<CardSyncResultDto>.ErrorResult("Kart tüm cihazlara gönderilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<CardSyncResultDto>> DeleteCardFromDeviceAsync(int cardId, int deviceId)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<ISpecialCardRepository>();
                var card = await repository.GetByIdAsync(cardId);

                if (card == null)
                    return ApiResponseDto<CardSyncResultDto>.ErrorResult("Özel kart bulunamadı");

                var device = await _deviceService.GetDeviceByIdAsync(deviceId);
                if (device == null)
                    return ApiResponseDto<CardSyncResultDto>.ErrorResult("Cihaz bulunamadı");

                var result = new CardSyncResultDto
                {
                    TotalDevices = 1
                };

                var detail = new DeviceSyncDetail
                {
                    DeviceId = device.DeviceId,
                    DeviceName = device.DeviceName,
                    DeviceIp = device.IpAddress
                };

                try
                {
                    var port = int.TryParse(device.Port, out var p) ? p : 4370;
                    var success = await _zktecoApiClient.DeleteUserFromDeviceAsync(device.IpAddress, card.EnrollNumber, port);

                    if (success)
                    {
                        detail.Success = true;
                        result.SuccessCount++;
                        _logger.LogInformation("Özel kart cihazdan silindi: {CardName} -> {DeviceName}", card.CardName, device.DeviceName);
                    }
                    else
                    {
                        detail.Success = false;
                        detail.ErrorMessage = "Cihazdan silinemedi";
                        result.FailCount++;
                        _logger.LogWarning("Özel kart cihazdan silinemedi: {CardName} -> {DeviceName}", card.CardName, device.DeviceName);
                    }
                }
                catch (Exception ex)
                {
                    detail.Success = false;
                    detail.ErrorMessage = ex.Message;
                    result.FailCount++;
                    _logger.LogError(ex, "Özel kart cihazdan silinirken hata: {CardName} -> {DeviceName}", card.CardName, device.DeviceName);
                }

                result.Details.Add(detail);
                return ApiResponseDto<CardSyncResultDto>.SuccessResult(result, result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting card from device: CardId={CardId}, DeviceId={DeviceId}", cardId, deviceId);
                return ApiResponseDto<CardSyncResultDto>.ErrorResult("Kart cihazdan silinirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<CardSyncResultDto>> DeleteCardFromAllDevicesAsync(int cardId)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<ISpecialCardRepository>();
                var card = await repository.GetByIdAsync(cardId);

                if (card == null)
                    return ApiResponseDto<CardSyncResultDto>.ErrorResult("Özel kart bulunamadı");

                var devices = await _deviceService.GetActiveDevicesAsync();
                if (!devices.Any())
                    return ApiResponseDto<CardSyncResultDto>.ErrorResult("Aktif cihaz bulunamadı");

                var result = new CardSyncResultDto
                {
                    TotalDevices = devices.Count
                };

                foreach (var device in devices)
                {
                    var detail = new DeviceSyncDetail
                    {
                        DeviceId = device.DeviceId,
                        DeviceName = device.DeviceName,
                        DeviceIp = device.IpAddress
                    };

                    try
                    {
                        var port = int.TryParse(device.Port, out var p) ? p : 4370;
                        var success = await _zktecoApiClient.DeleteUserFromDeviceAsync(device.IpAddress, card.EnrollNumber, port);

                        if (success)
                        {
                            detail.Success = true;
                            result.SuccessCount++;
                            _logger.LogInformation("Özel kart cihazdan silindi: {CardName} -> {DeviceName}", card.CardName, device.DeviceName);
                        }
                        else
                        {
                            detail.Success = false;
                            detail.ErrorMessage = "Cihazdan silinemedi";
                            result.FailCount++;
                            _logger.LogWarning("Özel kart cihazdan silinemedi: {CardName} -> {DeviceName}", card.CardName, device.DeviceName);
                        }
                    }
                    catch (Exception ex)
                    {
                        detail.Success = false;
                        detail.ErrorMessage = ex.Message;
                        result.FailCount++;
                        _logger.LogError(ex, "Özel kart cihazdan silinirken hata: {CardName} -> {DeviceName}", card.CardName, device.DeviceName);
                    }

                    result.Details.Add(detail);
                }

                _logger.LogInformation("Özel kart tüm cihazlardan silindi: {CardName} - Başarılı: {Success}/{Total}", 
                    card.CardName, result.SuccessCount, result.TotalDevices);

                return ApiResponseDto<CardSyncResultDto>.SuccessResult(result, result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting card from all devices: CardId={CardId}", cardId);
                return ApiResponseDto<CardSyncResultDto>.ErrorResult("Kart tüm cihazlardan silinirken hata oluştu");
            }
        }
    }
}
