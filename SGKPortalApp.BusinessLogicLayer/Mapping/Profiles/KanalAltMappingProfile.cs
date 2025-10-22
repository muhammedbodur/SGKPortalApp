using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles
{
    public class KanalAltMappingProfile : Profile
    {
        public KanalAltMappingProfile()
        {
            // Request -> Entity
            CreateMap<KanalAltIslemCreateRequestDto, KanalAlt>()
                .ForMember(dest => dest.KanalAltId, opt => opt.Ignore())
                .ForMember(dest => dest.Aktiflik, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.Kanal, opt => opt.Ignore())
                .ForMember(dest => dest.KanalAltIslemleri, opt => opt.Ignore());

            // Entity -> Response
            CreateMap<KanalAlt, KanalAltResponseDto>()
                .ForMember(dest => dest.KanalAdi,
                    opt => opt.MapFrom(src => src.Kanal != null ? src.Kanal.KanalAdi : string.Empty))
                .ForMember(dest => dest.KanalAltIslemSayisi,
                    opt => opt.MapFrom(src => src.KanalAltIslemleri != null ? src.KanalAltIslemleri.Count : 0));
        }
    }
}
