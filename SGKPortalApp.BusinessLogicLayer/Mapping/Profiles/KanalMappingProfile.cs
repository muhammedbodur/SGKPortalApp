using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles
{
    public class KanalMappingProfile : Profile
    {
        public KanalMappingProfile()
        {
            // Request -> Entity
            CreateMap<KanalCreateRequestDto, Kanal>()
                .ForMember(dest => dest.KanalId, opt => opt.Ignore())
                .ForMember(dest => dest.Aktiflik, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.KanalAltlari, opt => opt.Ignore())
                .ForMember(dest => dest.KanalIslemleri, opt => opt.Ignore());

            CreateMap<KanalUpdateRequestDto, Kanal>()
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.KanalAltlari, opt => opt.Ignore())
                .ForMember(dest => dest.KanalIslemleri, opt => opt.Ignore());

            // Entity -> Response
            CreateMap<Kanal, KanalResponseDto>()
                .ForMember(dest => dest.KanalAltSayisi,
                    opt => opt.MapFrom(src => src.KanalAltlari != null ? src.KanalAltlari.Count : 0))
                .ForMember(dest => dest.KanalIslemSayisi,
                    opt => opt.MapFrom(src => src.KanalIslemleri != null ? src.KanalIslemleri.Count : 0));
        }
    }
}
