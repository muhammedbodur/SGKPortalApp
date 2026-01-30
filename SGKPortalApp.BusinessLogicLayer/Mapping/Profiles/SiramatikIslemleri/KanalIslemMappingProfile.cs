using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.Common.Helpers;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.SiramatikIslemleri
{
    public class KanalIslemMappingProfile : Profile
    {
        public KanalIslemMappingProfile()
        {
            // Request -> Entity
            CreateMap<KanalIslemCreateRequestDto, KanalIslem>()
                .ForMember(dest => dest.KanalIslemId, opt => opt.Ignore())
                .ForMember(dest => dest.Kanal, opt => opt.Ignore())
                .ForMember(dest => dest.DepartmanHizmetBinasi, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.MapFrom(src => DateTimeHelper.Now))
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTimeHelper.Now))
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.KanalAltIslemleri, opt => opt.Ignore());

            CreateMap<KanalIslemUpdateRequestDto, KanalIslem>()
                .ForMember(dest => dest.KanalIslemId, opt => opt.Ignore())
                .ForMember(dest => dest.Kanal, opt => opt.Ignore())
                .ForMember(dest => dest.DepartmanHizmetBinasi, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTimeHelper.Now))
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.KanalAltIslemleri, opt => opt.Ignore());

            // Entity -> Response
            CreateMap<KanalIslem, KanalIslemResponseDto>()
                .ForMember(dest => dest.KanalAdi,
                    opt => opt.MapFrom(src => src.Kanal != null ? src.Kanal.KanalAdi : string.Empty))
                .ForMember(dest => dest.HizmetBinasiAdi,
                    opt => opt.MapFrom(src => src.DepartmanHizmetBinasi != null && src.DepartmanHizmetBinasi.HizmetBinasi != null
                        ? src.DepartmanHizmetBinasi.HizmetBinasi.HizmetBinasiAdi
                        : string.Empty))
                .ForMember(dest => dest.KanalAltIslemSayisi,
                    opt => opt.MapFrom(src => src.KanalAltIslemleri != null
                        ? src.KanalAltIslemleri.Count
                        : 0));
        }
    }
}