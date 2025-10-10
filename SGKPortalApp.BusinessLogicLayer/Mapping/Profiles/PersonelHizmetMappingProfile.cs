using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles
{
    public class PersonelHizmetMappingProfile : Profile
    {
        public PersonelHizmetMappingProfile()
        {
            CreateMap<PersonelHizmet, PersonelHizmetResponseDto>()
                .ForMember(dest => dest.DepartmanAdi, opt => opt.MapFrom(src => src.Departman != null ? src.Departman.DepartmanAdi : null))
                .ForMember(dest => dest.ServisAdi, opt => opt.MapFrom(src => src.Servis != null ? src.Servis.ServisAdi : null));
            
            CreateMap<PersonelHizmetCreateRequestDto, PersonelHizmet>()
                .ForMember(dest => dest.PersonelHizmetId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Personel, opt => opt.Ignore())
                .ForMember(dest => dest.Departman, opt => opt.Ignore())
                .ForMember(dest => dest.Servis, opt => opt.Ignore());
        }
    }
}
