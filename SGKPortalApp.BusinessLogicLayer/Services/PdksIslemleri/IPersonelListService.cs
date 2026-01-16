using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.Common.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri
{
    public interface IPersonelListService
    {
        Task<IResult<List<PersonelListResponseDto>>> GetPersonelListAsync(PersonelListFilterRequestDto request, string currentUserTcKimlikNo);
        Task<IResult<bool>> UpdatePersonelAktifDurumAsync(PersonelAktifDurumUpdateDto request, string currentUserTcKimlikNo);
    }
}
