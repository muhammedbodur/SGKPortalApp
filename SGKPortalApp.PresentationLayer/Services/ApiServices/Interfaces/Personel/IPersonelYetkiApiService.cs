using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel
{
    public interface IPersonelYetkiApiService
    {
        Task<ServiceResult<PersonelYetkiResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<List<PersonelYetkiResponseDto>>> GetByTcKimlikNoAsync(string tcKimlikNo);
        Task<ServiceResult<List<PersonelYetkiResponseDto>>> GetByYetkiIdAsync(int yetkiId);

        Task<ServiceResult<PersonelYetkiResponseDto>> CreateAsync(PersonelYetkiCreateRequestDto request);
        Task<ServiceResult<PersonelYetkiResponseDto>> UpdateAsync(int id, PersonelYetkiUpdateRequestDto request);
        Task<ServiceResult<bool>> DeleteAsync(int id);

        // Login ile aynı metodu kullanır (atanmış + MinYetkiSeviyesi > None olan varsayılanlar)
        Task<ServiceResult<Dictionary<string, int>>> GetUserPermissionsWithDefaultsAsync(string tcKimlikNo);
    }
}
