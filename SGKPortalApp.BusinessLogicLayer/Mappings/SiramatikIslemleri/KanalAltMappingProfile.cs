using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Mappings.SiramatikIslemleri
{
    public class KanalAltMappingProfile : Profile
    {
        public KanalAltMappingProfile()
        {
            // Entity to Response DTO
            CreateMap<KanalAlt, KanalAltResponseDto>()
                .ForMember(dest => dest.KanalAdi, opt => opt.MapFrom(src => src.Kanal.KanalAdi))
                .ForMember(dest => dest.KanalAltIslemSayisi, opt => opt.MapFrom(src => src.KanalAltIslemleri != null ? src.KanalAltIslemleri.Count : 0));

            // Request DTO to Entity
            CreateMap<KanalAltKanalCreateRequestDto, KanalAlt>();
            CreateMap<KanalAltKanalUpdateRequestDto, KanalAlt>();
        }
    }
}
