using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PdksIslemleri;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SGKPortalApp.BusinessLogicLayer.Helpers;

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

                // EnrollNumber kontrolü
                var existingByEnrollNumber = await repository.GetByEnrollNumberAsync(request.EnrollNumber);
                if (existingByEnrollNumber != null)
                    return ApiResponseDto<SpecialCardResponseDto>.ErrorResult("Bu EnrollNumber zaten kullanılıyor");

                var card = new SpecialCard
                {
                    CardType = request.CardType,
                    CardNumber = request.CardNumber,
                    CardName = request.CardName,
                    EnrollNumber = request.EnrollNumber,
                    NickName = string.IsNullOrWhiteSpace(request.NickName) 
                        ? StringHelper.GenerateNickName(request.CardName, 12) 
                        : request.NickName,
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

                // EnrollNumber kontrolü (kendisi hariç)
                var existingByEnrollNumber = await repository.GetByEnrollNumberAsync(request.EnrollNumber);
                if (existingByEnrollNumber != null && existingByEnrollNumber.Id != id)
                    return ApiResponseDto<SpecialCardResponseDto>.ErrorResult("Bu EnrollNumber zaten kullanılıyor");

                card.CardType = request.CardType;
                card.CardNumber = request.CardNumber;
                card.CardName = request.CardName;
                card.EnrollNumber = request.EnrollNumber;
                card.NickName = StringHelper.GenerateNickName(request.CardName, 12);
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
                        Enabled = true
                    };

                    var port = int.TryParse(device.Port, out var p) ? p : 4370;
                    var success = await _zktecoApiClient.AddUserToDeviceAsync(device.IpAddress, apiUser, port, force: true);

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

                var devices = await _deviceService.GetActiveDevicesAsync();
                if (!devices.Any())
                    return ApiResponseDto<CardSyncResultDto>.ErrorResult("Aktif cihaz bulunamadı");

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
                        var success = await _zktecoApiClient.AddUserToDeviceAsync(device.IpAddress, apiUser, port, force: true);

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
