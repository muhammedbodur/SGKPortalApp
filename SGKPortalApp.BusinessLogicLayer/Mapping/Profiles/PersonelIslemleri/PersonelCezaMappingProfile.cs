using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.PersonelIslemleri
{
    public class PersonelCezaMappingProfile : Profile
    {
        public PersonelCezaMappingProfile()
        {
            CreateMap<PersonelCeza, PersonelCezaResponseDto>();
            
            CreateMap<PersonelCezaCreateRequestDto, PersonelCeza>()
                .ForMember(dest => dest.PersonelCezaId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Personel, opt => opt.Ignore());
        }
    }
}
