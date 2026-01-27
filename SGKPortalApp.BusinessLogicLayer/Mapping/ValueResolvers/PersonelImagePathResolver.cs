using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.Common.Helpers;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.ValueResolvers
{
    /// <summary>
    /// AutoMapper member value resolver for Personel.Resim field.
    /// Converts filename stored in DB to full web path using PersonelImagePathHelper.
    /// </summary>
    public class PersonelImagePathResolver : IMemberValueResolver<Personel, PersonelResponseDto, string?, string?>
    {
        private readonly PersonelImagePathHelper _imagePathHelper;

        public PersonelImagePathResolver(PersonelImagePathHelper imagePathHelper)
        {
            _imagePathHelper = imagePathHelper;
        }

        public string? Resolve(Personel source, PersonelResponseDto destination, string? sourceMember, string? destMember, ResolutionContext context)
        {
            // sourceMember DB'den gelen Resim değeri (filename veya null)
            return _imagePathHelper.GetWebPath(sourceMember);
        }
    }
}
