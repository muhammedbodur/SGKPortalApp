using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.SiramatikIslemleri
{
    public class KioskMappingProfile : Profile
    {
        public KioskMappingProfile()
        {
            // Entity -> DTO
            CreateMap<KioskMenu, KioskMenuResponseDto>();

            CreateMap<Kiosk, KioskResponseDto>()
                .ForMember(dest => dest.HizmetBinasiAdi, 
                    opt => opt.MapFrom(src => src.HizmetBinasi != null ? src.HizmetBinasi.HizmetBinasiAdi : null))
                .ForMember(dest => dest.DepartmanAdi, 
                    opt => opt.MapFrom(src => src.HizmetBinasi != null && src.HizmetBinasi.Departman != null ? src.HizmetBinasi.Departman.DepartmanAdi : null))
                .ForMember(dest => dest.AtananKioskMenuId,
                    opt => opt.MapFrom(src => src.MenuAtamalari != null && src.MenuAtamalari.Any(ma => ma.Aktiflik == Aktiflik.Aktif) 
                        ? src.MenuAtamalari.First(ma => ma.Aktiflik == Aktiflik.Aktif).KioskMenuId 
                        : (int?)null))
                .ForMember(dest => dest.AtananKioskMenuAdi,
                    opt => opt.MapFrom(src => src.MenuAtamalari != null && src.MenuAtamalari.Any(ma => ma.Aktiflik == Aktiflik.Aktif) 
                        ? src.MenuAtamalari.First(ma => ma.Aktiflik == Aktiflik.Aktif).KioskMenu.MenuAdi 
                        : null));

            CreateMap<KioskMenuIslem, KioskMenuIslemResponseDto>()
                .ForMember(dest => dest.KioskMenuAdi, opt => opt.MapFrom(src => src.KioskMenu != null ? src.KioskMenu.MenuAdi : null))
                .ForMember(dest => dest.KanalAltAdi, opt => opt.MapFrom(src => src.KanalAlt != null ? src.KanalAlt.KanalAltAdi : null));

            CreateMap<KioskMenuAtama, KioskMenuAtamaResponseDto>()
                .ForMember(dest => dest.KioskAdi, opt => opt.MapFrom(src => src.Kiosk != null ? src.Kiosk.KioskAdi : null))
                .ForMember(dest => dest.HizmetBinasiAdi, opt => opt.MapFrom(src => src.Kiosk != null && src.Kiosk.HizmetBinasi != null ? src.Kiosk.HizmetBinasi.HizmetBinasiAdi : null))
                .ForMember(dest => dest.KioskMenuAdi, opt => opt.MapFrom(src => src.KioskMenu != null ? src.KioskMenu.MenuAdi : null));

            CreateMap<Kiosk, KioskSummaryDto>();

            // DTO -> Entity
            CreateMap<KioskMenuCreateRequestDto, KioskMenu>();
            CreateMap<KioskMenuUpdateRequestDto, KioskMenu>();
            CreateMap<KioskCreateRequestDto, Kiosk>();
            CreateMap<KioskUpdateRequestDto, Kiosk>();
            CreateMap<KioskMenuIslemCreateRequestDto, KioskMenuIslem>();
            CreateMap<KioskMenuIslemUpdateRequestDto, KioskMenuIslem>();
            CreateMap<KioskMenuAtamaCreateRequestDto, KioskMenuAtama>();
            CreateMap<KioskMenuAtamaUpdateRequestDto, KioskMenuAtama>();
        }
    }
}
