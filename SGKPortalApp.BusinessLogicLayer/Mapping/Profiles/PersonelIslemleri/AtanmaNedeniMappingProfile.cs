using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.PersonelIslemleri
{
    public class AtanmaNedeniMappingProfile : Profile
    {
        public AtanmaNedeniMappingProfile()
        {
            // Entity -> Response
            CreateMap<AtanmaNedenleri, AtanmaNedeniResponseDto>();
            
            // Request -> Entity
            CreateMap<AtanmaNedeniCreateRequestDto, AtanmaNedenleri>()
                .ForMember(dest => dest.AtanmaNedeniId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Personeller, opt => opt.Ignore());
            
            CreateMap<AtanmaNedeniUpdateRequestDto, AtanmaNedenleri>()
                .ForMember(dest => dest.AtanmaNedeniId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Personeller, opt => opt.Ignore());
        }
    }
}
