using AutoMapper;
using SGKPortalApp.BusinessLogicLayer.Mapping.ValueConverters;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.Common.Helpers;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.SiramatikIslemleri
{
    public class KanalPersonelMappingProfile : Profile
    {
        public KanalPersonelMappingProfile()
        {

            // Request -> Entity
            CreateMap<KanalPersonelCreateRequestDto, KanalPersonel>()
                .ForMember(dest => dest.KanalPersonelId, opt => opt.Ignore())
                .ForMember(dest => dest.Aktiflik, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.MapFrom(src => DateTimeHelper.Now))
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTimeHelper.Now))
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.Personel, opt => opt.Ignore())
                .ForMember(dest => dest.KanalAltIslem, opt => opt.Ignore());

            // Entity -> Response
            CreateMap<KanalPersonel, KanalPersonelResponseDto>()
                .ForMember(dest => dest.PersonelAdSoyad,
                    opt => opt.MapFrom(src => src.Personel != null ? src.Personel.AdSoyad : string.Empty))
                .ForMember(dest => dest.KanalAltIslemAdi,
                    opt => opt.MapFrom(src => src.KanalAltIslem != null && src.KanalAltIslem.KanalAlt != null
                        ? src.KanalAltIslem.KanalAlt.KanalAltAdi
                        : string.Empty))
                .ForMember(dest => dest.Resim,
                    opt => opt.ConvertUsing<ImagePathConverter, string?>(src => src.Personel != null ? src.Personel.Resim : null));
        }
    }
}
