using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel
{
    public interface IPersonelApiService
    {
        Task<ServiceResult<List<PersonelResponseDto>>> GetAllAsync();
        Task<ServiceResult<PersonelResponseDto>> GetByTcKimlikNoAsync(string tcKimlikNo);
        Task<ServiceResult<PersonelResponseDto>> CreateAsync(PersonelCreateRequestDto dto);
        Task<ServiceResult<PersonelResponseDto>> UpdateAsync(string tcKimlikNo, PersonelUpdateRequestDto dto);
        Task<ServiceResult<bool>> DeleteAsync(string tcKimlikNo);

        // Toplu kayıt metodları
        Task<ServiceResult<PersonelResponseDto>> CreateCompleteAsync(PersonelCompleteRequestDto dto);
        Task<ServiceResult<PersonelResponseDto>> UpdateCompleteAsync(string tcKimlikNo, PersonelCompleteRequestDto dto);

        // Ek metodlar
        Task<ServiceResult<List<PersonelResponseDto>>> GetActiveAsync();
        Task<ServiceResult<PagedResponseDto<PersonelListResponseDto>>> GetPagedAsync(PersonelFilterRequestDto filter);
        Task<ServiceResult<List<PersonelResponseDto>>> GetByDepartmanAsync(int departmanId);
        Task<ServiceResult<List<PersonelResponseDto>>> GetByServisAsync(int servisId);
        Task<ServiceResult<List<PersonelResponseDto>>> GetPersonellerByHizmetBinasiIdAsync(int hizmetBinasiId);
    }
}