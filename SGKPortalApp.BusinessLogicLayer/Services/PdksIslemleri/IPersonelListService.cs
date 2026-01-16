using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri
{
    public interface IPersonelListService
    {
        Task<ApiResponseDto<List<PersonelListResponseDto>>> GetPersonelListAsync(PersonelListFilterRequestDto request, string currentUserTcKimlikNo);
        Task<ApiResponseDto<bool>> UpdatePersonelAktifDurumAsync(PersonelAktifDurumUpdateDto request, string currentUserTcKimlikNo);
    }
}
