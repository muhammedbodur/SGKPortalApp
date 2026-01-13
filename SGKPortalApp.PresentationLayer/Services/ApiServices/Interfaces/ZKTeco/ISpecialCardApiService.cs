using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.ZKTeco
{
    public interface ISpecialCardApiService
    {
        Task<ApiResponseDto<List<SpecialCardResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<SpecialCardResponseDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<SpecialCardResponseDto>> GetByCardNumberAsync(long cardNumber);
        Task<ApiResponseDto<SpecialCardResponseDto>> GetByEnrollNumberAsync(string enrollNumber);
        Task<ApiResponseDto<List<SpecialCardResponseDto>>> GetByCardTypeAsync(CardType cardType);
        Task<ApiResponseDto<SpecialCardResponseDto>> CreateAsync(SpecialCardCreateRequestDto request);
        Task<ApiResponseDto<SpecialCardResponseDto>> UpdateAsync(int id, SpecialCardUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);
        
        // Device Operations
        Task<ApiResponseDto<CardSyncResultDto>> SendCardToDeviceAsync(int cardId, int deviceId);
        Task<ApiResponseDto<CardSyncResultDto>> SendCardToAllDevicesAsync(int cardId);
        Task<ApiResponseDto<CardSyncResultDto>> DeleteCardFromDeviceAsync(int cardId, int deviceId);
        Task<ApiResponseDto<CardSyncResultDto>> DeleteCardFromAllDevicesAsync(int cardId);
    }
}
