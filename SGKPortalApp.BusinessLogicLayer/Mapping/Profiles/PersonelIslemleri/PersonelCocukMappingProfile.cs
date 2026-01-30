using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.Common.Helpers;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.PersonelIslemleri
{
    public class PersonelCocukMappingProfile : Profile
    {
        public PersonelCocukMappingProfile()
        {
            // Entity -> Response
            CreateMap<PersonelCocuk, PersonelCocukResponseDto>();
            
            // Request -> Entity
            CreateMap<PersonelCocukCreateRequestDto, PersonelCocuk>()
                .ForMember(dest => dest.PersonelCocukId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.MapFrom(src => DateTimeHelper.Now))
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTimeHelper.Now))
                .ForMember(dest => dest.Personel, opt => opt.Ignore());
            
            CreateMap<PersonelCocukUpdateRequestDto, PersonelCocuk>()
                .ForMember(dest => dest.PersonelCocukId, opt => opt.Ignore())
                .ForMember(dest => dest.PersonelTcKimlikNo, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTimeHelper.Now))
                .ForMember(dest => dest.Personel, opt => opt.Ignore());
        }
    }
}
