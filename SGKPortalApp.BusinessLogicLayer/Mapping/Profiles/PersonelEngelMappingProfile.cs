using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles
{
    public class PersonelEngelMappingProfile : Profile
    {
        public PersonelEngelMappingProfile()
        {
            CreateMap<PersonelEngel, PersonelEngelResponseDto>();
            
            CreateMap<PersonelEngelCreateRequestDto, PersonelEngel>()
                .ForMember(dest => dest.PersonelEngelId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Personel, opt => opt.Ignore());
        }
    }
}
