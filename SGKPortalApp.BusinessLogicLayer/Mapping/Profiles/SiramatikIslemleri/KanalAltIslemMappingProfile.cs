using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.SiramatikIslemleri
{
    public class KanalAltIslemMappingProfile : Profile
    {
        public KanalAltIslemMappingProfile()
        {
            CreateMap<KanalAltIslemCreateRequestDto, KanalAltIslem>()
                .ForMember(dest => dest.KanalAltIslemId, opt => opt.Ignore())
                .ForMember(dest => dest.KanalAlt, opt => opt.Ignore())
                .ForMember(dest => dest.KanalIslem, opt => opt.Ignore())
                .ForMember(dest => dest.KanalPersonelleri, opt => opt.Ignore());

            CreateMap<KanalAltIslem, KanalAltIslemResponseDto>()
                .ForMember(dest => dest.KanalAltAdi,
                    opt => opt.MapFrom(src => src.KanalAlt != null ? src.KanalAlt.KanalAltAdi : string.Empty))
                .ForMember(dest => dest.KanalAdi,
                    opt => opt.MapFrom(src => src.KanalIslem != null && src.KanalIslem.Kanal != null
                        ? src.KanalIslem.Kanal.KanalAdi
                        : string.Empty))
                .ForMember(dest => dest.PersonelSayisi,
                    opt => opt.MapFrom(src => src.KanalPersonelleri != null ? src.KanalPersonelleri.Count : 0));
        }
    }
}