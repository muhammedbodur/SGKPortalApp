using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles
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
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Personel, opt => opt.Ignore());
            
            CreateMap<PersonelCocukUpdateRequestDto, PersonelCocuk>()
                .ForMember(dest => dest.PersonelCocukId, opt => opt.Ignore())
                .ForMember(dest => dest.PersonelTcKimlikNo, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Personel, opt => opt.Ignore());
        }
    }
}
