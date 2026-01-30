using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.Common.Helpers;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.PersonelIslemleri
{
    public class PersonelEgitimMappingProfile : Profile
    {
        public PersonelEgitimMappingProfile()
        {
            CreateMap<PersonelEgitim, PersonelEgitimResponseDto>();
            
            CreateMap<PersonelEgitimCreateRequestDto, PersonelEgitim>()
                .ForMember(dest => dest.PersonelEgitimId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.MapFrom(src => DateTimeHelper.Now))
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTimeHelper.Now))
                .ForMember(dest => dest.Personel, opt => opt.Ignore());
        }
    }
}
