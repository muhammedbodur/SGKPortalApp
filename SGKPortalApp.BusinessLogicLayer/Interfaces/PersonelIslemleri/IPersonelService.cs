using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri
{
    public interface IPersonelService
    {
        Task<ApiResponseDto<List<PersonelResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<PersonelResponseDto>> GetByTcKimlikNoAsync(string tcKimlikNo);
        Task<ApiResponseDto<bool>> DeleteAsync(string tcKimlikNo);
        Task<ApiResponseDto<List<PersonelResponseDto>>> GetActiveAsync();
        Task<ApiResponseDto<PagedResponseDto<PersonelListResponseDto>>> GetPagedAsync(PersonelFilterRequestDto filter);
        Task<ApiResponseDto<List<PersonelResponseDto>>> GetByDepartmanAsync(int departmanId);
        Task<ApiResponseDto<List<PersonelResponseDto>>> GetByServisAsync(int servisId);
        Task<ApiResponseDto<List<PersonelResponseDto>>> GetByHizmetBinasiIdAsync(int hizmetBinasiId);

        // Toplu kayıt işlemleri (Transaction)
        Task<ApiResponseDto<PersonelResponseDto>> CreateCompleteAsync(PersonelCompleteRequestDto request);
        Task<ApiResponseDto<PersonelResponseDto>> UpdateCompleteAsync(string tcKimlikNo, PersonelCompleteRequestDto request);
    }
}
