using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles
{
    public class KanalIslemMappingProfile : Profile
    {
        public KanalIslemMappingProfile()
        {
            // Entity -> Response
            CreateMap<KanalIslem, KanalIslemResponseDto>()
                .ForMember(dest => dest.KanalAdi,
                    opt => opt.MapFrom(src => src.Kanal != null ? src.Kanal.KanalAdi : string.Empty))
                .ForMember(dest => dest.KanalAltIslemSayisi,
                    opt => opt.MapFrom(src => src.KanalAltIslemleri != null ? src.KanalAltIslemleri.Count : 0));
        }
    }
}
